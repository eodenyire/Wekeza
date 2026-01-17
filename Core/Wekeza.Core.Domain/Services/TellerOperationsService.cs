using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// Teller Operations Service - Complete teller transaction processing
/// Handles cash deposits, withdrawals, transfers with GL integration
/// Inspired by Finacle Teller and T24 TELLER operations
/// </summary>
public class TellerOperationsService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITellerSessionRepository _tellerSessionRepository;
    private readonly ICashDrawerRepository _cashDrawerRepository;
    private readonly ITellerTransactionRepository _tellerTransactionRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IGLAccountRepository _glAccountRepository;

    public TellerOperationsService(
        IAccountRepository accountRepository,
        ITellerSessionRepository tellerSessionRepository,
        ICashDrawerRepository cashDrawerRepository,
        ITellerTransactionRepository tellerTransactionRepository,
        IJournalEntryRepository journalEntryRepository,
        IGLAccountRepository glAccountRepository)
    {
        _accountRepository = accountRepository;
        _tellerSessionRepository = tellerSessionRepository;
        _cashDrawerRepository = cashDrawerRepository;
        _tellerTransactionRepository = tellerTransactionRepository;
        _journalEntryRepository = journalEntryRepository;
        _glAccountRepository = glAccountRepository;
    }

    /// <summary>
    /// Process cash deposit through teller
    /// </summary>
    public async Task<TellerOperationResult> ProcessCashDepositAsync(
        Guid sessionId,
        Guid accountId,
        Money depositAmount,
        CustomerVerificationMethod verificationMethod,
        bool customerPresent,
        string? reference = null,
        string? notes = null)
    {
        try
        {
            // 1. Get teller session
            var session = await _tellerSessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                return TellerOperationResult.Failed("Teller session not found");

            // 2. Get customer account
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                return TellerOperationResult.Failed("Account not found");

            // 3. Get cash drawer
            var cashDrawer = await _cashDrawerRepository.GetByTellerIdAsync(session.TellerId);
            if (cashDrawer == null)
                return TellerOperationResult.Failed("Cash drawer not found");

            // 4. Create teller transaction
            var tellerTransaction = TellerTransaction.Create(
                sessionId,
                session.SessionId,
                TellerTransactionType.CashDeposit,
                depositAmount,
                session.TellerId,
                session.TellerCode,
                session.TellerName,
                session.BranchId,
                session.BranchCode,
                session.CreatedBy,
                verificationMethod,
                customerPresent,
                reference);

            tellerTransaction.SetAccountDetails(accountId, account.AccountNumber.Value, account.CustomerId, "Customer Name");
            tellerTransaction.AddNotes(notes, null, null);

            // 5. Process cash deposit in session
            session.ProcessCashDeposit(depositAmount, account.AccountNumber.Value, session.CreatedBy, reference);

            // 6. Add cash to drawer
            cashDrawer.AddCash(depositAmount, CashSource.CashDelivery, session.CreatedBy, reference);

            // 7. Credit customer account
            account.Credit(depositAmount, tellerTransaction.TransactionNumber, $"Cash deposit - {reference}");

            // 8. Create GL entries
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Standard);
            var journalEntry = CreateCashDepositGLEntry(
                depositAmount, account, cashDrawer, journalNumber, session.CreatedBy, tellerTransaction.TransactionNumber);

            journalEntry.Post(session.CreatedBy);
            _journalEntryRepository.Add(journalEntry);

            // 9. Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            // 10. Complete transaction
            tellerTransaction.SetJournalEntry(journalNumber);
            tellerTransaction.Complete(session.CreatedBy);

            // 11. Save transaction
            await _tellerTransactionRepository.AddAsync(tellerTransaction);

            return TellerOperationResult.Success(
                tellerTransaction.TransactionNumber,
                depositAmount,
                account.Balance,
                journalNumber,
                "Cash deposit processed successfully");
        }
        catch (Exception ex)
        {
            return TellerOperationResult.Failed($"Error processing cash deposit: {ex.Message}");
        }
    }

    /// <summary>
    /// Process cash withdrawal through teller
    /// </summary>
    public async Task<TellerOperationResult> ProcessCashWithdrawalAsync(
        Guid sessionId,
        Guid accountId,
        Money withdrawalAmount,
        CustomerVerificationMethod verificationMethod,
        bool customerPresent,
        string? reference = null,
        string? notes = null)
    {
        try
        {
            // 1. Get teller session
            var session = await _tellerSessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                return TellerOperationResult.Failed("Teller session not found");

            // 2. Get customer account
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                return TellerOperationResult.Failed("Account not found");

            // 3. Validate account balance
            if (withdrawalAmount.IsGreaterThan(account.Balance))
                return TellerOperationResult.Failed("Insufficient account balance");

            // 4. Get cash drawer
            var cashDrawer = await _cashDrawerRepository.GetByTellerIdAsync(session.TellerId);
            if (cashDrawer == null)
                return TellerOperationResult.Failed("Cash drawer not found");

            // 5. Validate cash availability
            if (!cashDrawer.HasSufficientCash(withdrawalAmount))
                return TellerOperationResult.Failed("Insufficient cash in drawer");

            // 6. Create teller transaction
            var tellerTransaction = TellerTransaction.Create(
                sessionId,
                session.SessionId,
                TellerTransactionType.CashWithdrawal,
                withdrawalAmount,
                session.TellerId,
                session.TellerCode,
                session.TellerName,
                session.BranchId,
                session.BranchCode,
                session.CreatedBy,
                verificationMethod,
                customerPresent,
                reference);

            tellerTransaction.SetAccountDetails(accountId, account.AccountNumber.Value, account.CustomerId, "Customer Name");
            tellerTransaction.AddNotes(notes, null, null);

            // 7. Process cash withdrawal in session
            session.ProcessCashWithdrawal(withdrawalAmount, account.AccountNumber.Value, session.CreatedBy, reference);

            // 8. Remove cash from drawer
            cashDrawer.RemoveCash(withdrawalAmount, CashDestination.CashPickup, session.CreatedBy, reference);

            // 9. Debit customer account
            account.Debit(withdrawalAmount, tellerTransaction.TransactionNumber, $"Cash withdrawal - {reference}");

            // 10. Create GL entries
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Standard);
            var journalEntry = CreateCashWithdrawalGLEntry(
                withdrawalAmount, account, cashDrawer, journalNumber, session.CreatedBy, tellerTransaction.TransactionNumber);

            journalEntry.Post(session.CreatedBy);
            _journalEntryRepository.Add(journalEntry);

            // 11. Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            // 12. Complete transaction
            tellerTransaction.SetJournalEntry(journalNumber);
            tellerTransaction.Complete(session.CreatedBy);

            // 13. Save transaction
            await _tellerTransactionRepository.AddAsync(tellerTransaction);

            return TellerOperationResult.Success(
                tellerTransaction.TransactionNumber,
                withdrawalAmount,
                account.Balance,
                journalNumber,
                "Cash withdrawal processed successfully");
        }
        catch (Exception ex)
        {
            return TellerOperationResult.Failed($"Error processing cash withdrawal: {ex.Message}");
        }
    }

    /// <summary>
    /// Process account transfer through teller
    /// </summary>
    public async Task<TellerOperationResult> ProcessAccountTransferAsync(
        Guid sessionId,
        Guid fromAccountId,
        Guid toAccountId,
        Money transferAmount,
        CustomerVerificationMethod verificationMethod,
        bool customerPresent,
        string? reference = null,
        string? notes = null)
    {
        try
        {
            // 1. Get teller session
            var session = await _tellerSessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                return TellerOperationResult.Failed("Teller session not found");

            // 2. Get accounts
            var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(toAccountId);
            
            if (fromAccount == null)
                return TellerOperationResult.Failed("Source account not found");
            if (toAccount == null)
                return TellerOperationResult.Failed("Destination account not found");

            // 3. Validate account balance
            if (transferAmount.IsGreaterThan(fromAccount.Balance))
                return TellerOperationResult.Failed("Insufficient balance in source account");

            // 4. Create teller transaction
            var tellerTransaction = TellerTransaction.Create(
                sessionId,
                session.SessionId,
                TellerTransactionType.AccountTransfer,
                transferAmount,
                session.TellerId,
                session.TellerCode,
                session.TellerName,
                session.BranchId,
                session.BranchCode,
                session.CreatedBy,
                verificationMethod,
                customerPresent,
                reference);

            tellerTransaction.SetAccountDetails(fromAccountId, fromAccount.AccountNumber.Value, fromAccount.CustomerId, "Customer Name");
            tellerTransaction.AddNotes(notes, null, $"Transfer to {toAccount.AccountNumber.Value}");

            // 5. Process transfer in session
            session.ProcessTransfer(transferAmount, fromAccount.AccountNumber.Value, toAccount.AccountNumber.Value, session.CreatedBy, reference);

            // 6. Process account transfers
            fromAccount.Debit(transferAmount, tellerTransaction.TransactionNumber, $"Transfer to {toAccount.AccountNumber.Value} - {reference}");
            toAccount.Credit(transferAmount, tellerTransaction.TransactionNumber, $"Transfer from {fromAccount.AccountNumber.Value} - {reference}");

            // 7. Create GL entries
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Standard);
            var journalEntry = CreateAccountTransferGLEntry(
                transferAmount, fromAccount, toAccount, journalNumber, session.CreatedBy, tellerTransaction.TransactionNumber);

            journalEntry.Post(session.CreatedBy);
            _journalEntryRepository.Add(journalEntry);

            // 8. Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            // 9. Complete transaction
            tellerTransaction.SetJournalEntry(journalNumber);
            tellerTransaction.Complete(session.CreatedBy);

            // 10. Save transaction
            await _tellerTransactionRepository.AddAsync(tellerTransaction);

            return TellerOperationResult.Success(
                tellerTransaction.TransactionNumber,
                transferAmount,
                fromAccount.Balance,
                journalNumber,
                $"Transfer completed from {fromAccount.AccountNumber.Value} to {toAccount.AccountNumber.Value}");
        }
        catch (Exception ex)
        {
            return TellerOperationResult.Failed($"Error processing account transfer: {ex.Message}");
        }
    }

    /// <summary>
    /// Create GL entry for cash deposit
    /// Dr. Cash (Asset)
    /// Cr. Customer Account (Liability)
    /// </summary>
    private JournalEntry CreateCashDepositGLEntry(
        Money amount,
        Account account,
        CashDrawer cashDrawer,
        string journalNumber,
        string createdBy,
        string reference)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "CashDeposit",
            account.Id,
            reference,
            amount.Currency.Code,
            createdBy,
            $"Cash deposit - {reference}");

        // Debit: Cash (Asset increases)
        journalEntry.AddDebitLine(
            GetCashGLCode(),
            amount.Amount,
            description: $"Cash deposit - {account.AccountNumber.Value}");

        // Credit: Customer Account (Liability increases)
        journalEntry.AddCreditLine(
            account.CustomerGLCode,
            amount.Amount,
            description: $"Cash deposit to {account.AccountNumber.Value}");

        return journalEntry;
    }

    /// <summary>
    /// Create GL entry for cash withdrawal
    /// Dr. Customer Account (Liability decreases)
    /// Cr. Cash (Asset decreases)
    /// </summary>
    private JournalEntry CreateCashWithdrawalGLEntry(
        Money amount,
        Account account,
        CashDrawer cashDrawer,
        string journalNumber,
        string createdBy,
        string reference)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "CashWithdrawal",
            account.Id,
            reference,
            amount.Currency.Code,
            createdBy,
            $"Cash withdrawal - {reference}");

        // Debit: Customer Account (Liability decreases)
        journalEntry.AddDebitLine(
            account.CustomerGLCode,
            amount.Amount,
            description: $"Cash withdrawal from {account.AccountNumber.Value}");

        // Credit: Cash (Asset decreases)
        journalEntry.AddCreditLine(
            GetCashGLCode(),
            amount.Amount,
            description: $"Cash withdrawal - {account.AccountNumber.Value}");

        return journalEntry;
    }

    /// <summary>
    /// Create GL entry for account transfer
    /// Dr. Source Customer Account (Liability decreases)
    /// Cr. Destination Customer Account (Liability increases)
    /// </summary>
    private JournalEntry CreateAccountTransferGLEntry(
        Money amount,
        Account fromAccount,
        Account toAccount,
        string journalNumber,
        string createdBy,
        string reference)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "AccountTransfer",
            fromAccount.Id,
            reference,
            amount.Currency.Code,
            createdBy,
            $"Account transfer - {reference}");

        // Debit: Source Customer Account (Liability decreases)
        journalEntry.AddDebitLine(
            fromAccount.CustomerGLCode,
            amount.Amount,
            description: $"Transfer from {fromAccount.AccountNumber.Value}");

        // Credit: Destination Customer Account (Liability increases)
        journalEntry.AddCreditLine(
            toAccount.CustomerGLCode,
            amount.Amount,
            description: $"Transfer to {toAccount.AccountNumber.Value}");

        return journalEntry;
    }

    private async Task UpdateGLAccountBalancesAsync(JournalEntry journalEntry)
    {
        foreach (var line in journalEntry.Lines)
        {
            var glAccount = await _glAccountRepository.GetByGLCodeAsync(line.GLCode);
            if (glAccount != null)
            {
                if (line.DebitAmount > 0)
                    glAccount.PostDebit(line.DebitAmount);
                if (line.CreditAmount > 0)
                    glAccount.PostCredit(line.CreditAmount);
            }
        }
    }

    // GL Code helpers - these would typically come from configuration
    private string GetCashGLCode() => "1001"; // Cash and Cash Equivalents
}

/// <summary>
/// Teller operation result
/// </summary>
public class TellerOperationResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? TransactionNumber { get; private set; }
    public Money? TransactionAmount { get; private set; }
    public Money? AccountBalance { get; private set; }
    public string? JournalNumber { get; private set; }
    public string? Message { get; private set; }

    private TellerOperationResult() { }

    public static TellerOperationResult Success(
        string transactionNumber,
        Money transactionAmount,
        Money accountBalance,
        string? journalNumber = null,
        string? message = null)
    {
        return new TellerOperationResult
        {
            IsSuccess = true,
            TransactionNumber = transactionNumber,
            TransactionAmount = transactionAmount,
            AccountBalance = accountBalance,
            JournalNumber = journalNumber,
            Message = message
        };
    }

    public static TellerOperationResult Failed(string errorMessage)
    {
        return new TellerOperationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}