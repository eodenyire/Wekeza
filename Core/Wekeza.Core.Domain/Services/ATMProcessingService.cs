using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Exceptions;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// ATM Processing Service - Handles ATM transaction processing and authorization
/// Integrates with card validation, account management, and GL posting
/// </summary>
public class ATMProcessingService
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IGLAccountRepository _glAccountRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly CardManagementService _cardManagementService;

    public ATMProcessingService(
        ICardRepository cardRepository,
        IAccountRepository accountRepository,
        IGLAccountRepository glAccountRepository,
        IJournalEntryRepository journalEntryRepository,
        CardManagementService cardManagementService)
    {
        _cardRepository = cardRepository;
        _accountRepository = accountRepository;
        _glAccountRepository = glAccountRepository;
        _journalEntryRepository = journalEntryRepository;
        _cardManagementService = cardManagementService;
    }

    public async Task<ATMTransaction> ProcessWithdrawalAsync(
        string cardNumber,
        string encryptedPIN,
        decimal amount,
        string atmId,
        string atmLocation,
        bool isOnUs = true)
    {
        // Get card by number
        var card = await _cardRepository.GetByCardNumberAsync(cardNumber);
        if (card == null)
        {
            var failedTransaction = CreateFailedTransaction(cardNumber, amount, atmId, atmLocation, "Invalid card number");
            return failedTransaction;
        }

        // Validate PIN
        if (!card.ValidatePIN(encryptedPIN))
        {
            var failedTransaction = ATMTransaction.CreateWithdrawal(
                atmId, atmLocation, card.Id, card.AccountId, card.CustomerId,
                MaskCardNumber(cardNumber), new Money(amount, Currency.KES), Money.Zero(Currency.KES), isOnUs);
            
            failedTransaction.SetPINVerificationResult(false);
            failedTransaction.Decline("55", "Incorrect PIN", "PIN validation failed");
            
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, amount, CardTransactionType.ATMWithdrawal, false, null, atmId);
            
            return failedTransaction;
        }

        // Get account and validate
        var account = await _accountRepository.GetByIdAsync(card.AccountId);
        if (account == null)
        {
            var failedTransaction = ATMTransaction.CreateWithdrawal(
                atmId, atmLocation, card.Id, card.AccountId, card.CustomerId,
                MaskCardNumber(cardNumber), new Money(amount, Currency.KES), Money.Zero(Currency.KES), isOnUs);
            
            failedTransaction.Decline("14", "Invalid account", "Account not found");
            return failedTransaction;
        }

        var withdrawalAmount = new Money(amount, account.Currency);
        
        // Create ATM transaction
        var atmTransaction = ATMTransaction.CreateWithdrawal(
            atmId, atmLocation, card.Id, card.AccountId, card.CustomerId,
            MaskCardNumber(cardNumber), withdrawalAmount, account.Balance, isOnUs);

        atmTransaction.SetPINVerificationResult(true);

        // Validate card can process transaction
        if (!card.CanProcessTransaction(amount, CardTransactionType.ATMWithdrawal))
        {
            atmTransaction.Decline("61", "Exceeds withdrawal limit", "Daily withdrawal limit exceeded");
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, amount, CardTransactionType.ATMWithdrawal, false, null, atmId);
            return atmTransaction;
        }

        // Check account balance
        if (account.Balance < withdrawalAmount)
        {
            atmTransaction.Decline("51", "Insufficient funds", "Account balance insufficient");
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, amount, CardTransactionType.ATMWithdrawal, false, null, atmId);
            return atmTransaction;
        }

        // Check account status
        if (account.Status != AccountStatus.Active)
        {
            atmTransaction.Decline("62", "Restricted card", "Account is not active");
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, amount, CardTransactionType.ATMWithdrawal, false, null, atmId);
            return atmTransaction;
        }

        try
        {
            // Process withdrawal
            account.Withdraw(withdrawalAmount, $"ATM Withdrawal - {atmId}", "ATM_SYSTEM");
            
            // Authorize transaction
            var authCode = GenerateAuthorizationCode();
            atmTransaction.Authorize(authCode, account.Balance);
            
            // Post to GL
            await PostATMWithdrawalToGLAsync(account, withdrawalAmount, atmId, isOnUs);
            
            // Complete transaction
            var receiptNumber = GenerateReceiptNumber(atmId);
            atmTransaction.Complete(receiptNumber, true);
            
            // Update card transaction tracking
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, amount, CardTransactionType.ATMWithdrawal, true, null, atmId);
            
            // Update repositories
            await _accountRepository.UpdateAsync(account);
            await _cardRepository.UpdateAsync(card);
            
            return atmTransaction;
        }
        catch (DomainException ex)
        {
            atmTransaction.Decline("96", "System error", ex.Message);
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, amount, CardTransactionType.ATMWithdrawal, false, null, atmId);
            return atmTransaction;
        }
    }

    public async Task<ATMTransaction> ProcessBalanceInquiryAsync(
        string cardNumber,
        string encryptedPIN,
        string atmId,
        string atmLocation,
        bool isOnUs = true)
    {
        // Get card by number
        var card = await _cardRepository.GetByCardNumberAsync(cardNumber);
        if (card == null)
        {
            var failedTransaction = CreateFailedBalanceInquiry(cardNumber, atmId, atmLocation, "Invalid card number");
            return failedTransaction;
        }

        // Validate PIN
        if (!card.ValidatePIN(encryptedPIN))
        {
            var failedTransaction = ATMTransaction.CreateBalanceInquiry(
                atmId, atmLocation, card.Id, card.AccountId, card.CustomerId,
                MaskCardNumber(cardNumber), Money.Zero(Currency.KES), isOnUs);
            
            failedTransaction.SetPINVerificationResult(false);
            failedTransaction.Decline("55", "Incorrect PIN", "PIN validation failed");
            return failedTransaction;
        }

        // Get account
        var account = await _accountRepository.GetByIdAsync(card.AccountId);
        if (account == null)
        {
            var failedTransaction = ATMTransaction.CreateBalanceInquiry(
                atmId, atmLocation, card.Id, card.AccountId, card.CustomerId,
                MaskCardNumber(cardNumber), Money.Zero(Currency.KES), isOnUs);
            
            failedTransaction.Decline("14", "Invalid account", "Account not found");
            return failedTransaction;
        }

        // Create balance inquiry transaction
        var atmTransaction = ATMTransaction.CreateBalanceInquiry(
            atmId, atmLocation, card.Id, card.AccountId, card.CustomerId,
            MaskCardNumber(cardNumber), account.Balance, isOnUs);

        atmTransaction.SetPINVerificationResult(true);

        // Check account status
        if (account.Status != AccountStatus.Active)
        {
            atmTransaction.Decline("62", "Restricted card", "Account is not active");
            return atmTransaction;
        }

        try
        {
            // Authorize and complete transaction
            var authCode = GenerateAuthorizationCode();
            atmTransaction.Authorize(authCode, account.Balance);
            
            var receiptNumber = GenerateReceiptNumber(atmId);
            atmTransaction.Complete(receiptNumber, true);
            
            // Post balance inquiry fee if applicable
            await PostBalanceInquiryFeeAsync(account, atmId, isOnUs);
            
            return atmTransaction;
        }
        catch (DomainException ex)
        {
            atmTransaction.Decline("96", "System error", ex.Message);
            return atmTransaction;
        }
    }

    public async Task<bool> ValidatePINAsync(string cardNumber, string encryptedPIN)
    {
        var card = await _cardRepository.GetByCardNumberAsync(cardNumber);
        if (card == null)
            return false;

        return card.ValidatePIN(encryptedPIN);
    }

    public async Task<ATMTransaction> ReverseTransactionAsync(
        Guid originalTransactionId,
        string reversalReason,
        string atmId)
    {
        // This would typically be implemented for transaction reversals
        // For now, we'll create a placeholder implementation
        throw new NotImplementedException("ATM transaction reversal not yet implemented");
    }

    private async Task PostATMWithdrawalToGLAsync(Account account, Money amount, string atmId, bool isOnUs)
    {
        // Get GL accounts
        var customerAccountGL = await _glAccountRepository.GetByCodeAsync(account.GLAccountCode);
        var atmCashGL = await _glAccountRepository.GetByCodeAsync("1050"); // ATM Cash account
        
        if (customerAccountGL == null || atmCashGL == null)
            throw new GenericDomainException("Required GL accounts not found for ATM withdrawal posting");

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"ATM-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "ATM_WITHDRAWAL",
            Guid.NewGuid(),
            $"ATM Withdrawal - ATM: {atmId} - Account: {account.AccountNumber}",
            amount.Currency.Code,
            "ATM_SYSTEM",
            $"ATM Withdrawal - ATM: {atmId} - Account: {account.AccountNumber}");

        // Dr. ATM Cash, Cr. Customer Account (money flows from customer account to ATM cash)
        journalEntry.AddDebitEntry(atmCashGL.GLCode, amount.Amount, $"ATM withdrawal - {atmId}");
        journalEntry.AddCreditEntry(customerAccountGL.GLCode, amount.Amount, $"ATM withdrawal - Account: {account.AccountNumber}");

        await _journalEntryRepository.AddAsync(journalEntry);

        // Post interchange fee if not on-us transaction
        if (!isOnUs)
        {
            await PostInterchangeFeeAsync(account, amount, "ATM_WITHDRAWAL");
        }
    }

    private async Task PostBalanceInquiryFeeAsync(Account account, string atmId, bool isOnUs)
    {
        // Only charge fee for off-us transactions
        if (isOnUs)
            return;

        var inquiryFee = new Money(10, account.Currency); // KES 10 for off-us balance inquiry
        
        var customerAccountGL = await _glAccountRepository.GetByCodeAsync(account.GLAccountCode);
        var feeIncomeGL = await _glAccountRepository.GetByCodeAsync("4210"); // Transaction Fee Income
        
        if (customerAccountGL == null || feeIncomeGL == null)
            return; // Skip fee posting if GL accounts not found

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"ATM-BAL-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "ATM_BALANCE_INQUIRY",
            Guid.NewGuid(),
            $"ATM Balance Inquiry Fee - ATM: {atmId} - Account: {account.AccountNumber}",
            inquiryFee.Currency.Code,
            "ATM_SYSTEM",
            $"ATM Balance Inquiry Fee - ATM: {atmId} - Account: {account.AccountNumber}");

        // Dr. Customer Account, Cr. Fee Income
        journalEntry.AddDebitEntry(customerAccountGL.GLCode, inquiryFee.Amount, $"Balance inquiry fee - {atmId}");
        journalEntry.AddCreditEntry(feeIncomeGL.GLCode, inquiryFee.Amount, "Balance inquiry fee income");

        await _journalEntryRepository.AddAsync(journalEntry);
    }

    private async Task PostInterchangeFeeAsync(Account account, Money transactionAmount, string transactionType)
    {
        // Calculate interchange fee (typically a percentage of transaction amount)
        var interchangeRate = 0.0125m; // 1.25% interchange fee
        var interchangeFee = new Money(transactionAmount.Amount * interchangeRate, transactionAmount.Currency);
        
        var customerAccountGL = await _glAccountRepository.GetByCodeAsync(account.GLAccountCode);
        var interchangeExpenseGL = await _glAccountRepository.GetByCodeAsync("5300"); // Interchange Expense
        
        if (customerAccountGL == null || interchangeExpenseGL == null)
            return; // Skip fee posting if GL accounts not found

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"ATM-INT-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "INTERCHANGE_FEE",
            Guid.NewGuid(),
            $"Interchange Fee - {transactionType} - Account: {account.AccountNumber}",
            interchangeFee.Currency.Code,
            "ATM_SYSTEM",
            $"Interchange Fee - {transactionType} - Account: {account.AccountNumber}");

        // Dr. Interchange Expense, Cr. Customer Account
        journalEntry.AddDebitEntry(interchangeExpenseGL.GLCode, interchangeFee.Amount, $"Interchange fee - {transactionType}");
        journalEntry.AddCreditEntry(customerAccountGL.GLCode, interchangeFee.Amount, "Interchange fee recovery");

        await _journalEntryRepository.AddAsync(journalEntry);
    }

    private ATMTransaction CreateFailedTransaction(string cardNumber, decimal amount, string atmId, string atmLocation, string failureReason)
    {
        var failedTransaction = ATMTransaction.CreateWithdrawal(
            atmId, atmLocation, Guid.Empty, Guid.Empty, Guid.Empty,
            MaskCardNumber(cardNumber), new Money(amount, Currency.KES), Money.Zero(Currency.KES));
        
        failedTransaction.Decline("14", "Invalid card", failureReason);
        return failedTransaction;
    }

    private ATMTransaction CreateFailedBalanceInquiry(string cardNumber, string atmId, string atmLocation, string failureReason)
    {
        var failedTransaction = ATMTransaction.CreateBalanceInquiry(
            atmId, atmLocation, Guid.Empty, Guid.Empty, Guid.Empty,
            MaskCardNumber(cardNumber), Money.Zero(Currency.KES));
        
        failedTransaction.Decline("14", "Invalid card", failureReason);
        return failedTransaction;
    }

    private string MaskCardNumber(string cardNumber)
    {
        if (cardNumber.Length < 8)
            return "****";
        
        return $"{cardNumber[..4]}****{cardNumber[^4..]}";
    }

    private string GenerateAuthorizationCode()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    private string GenerateReceiptNumber(string atmId)
    {
        return $"{atmId}{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}