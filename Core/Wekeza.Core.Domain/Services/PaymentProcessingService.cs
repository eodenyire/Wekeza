using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// Payment Processing Service - Core payment processing logic
/// Handles payment validation, processing, and GL integration
/// Inspired by Finacle Payment Hub and T24 Payment Processing
/// </summary>
public class PaymentProcessingService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IGLAccountRepository _glAccountRepository;

    public PaymentProcessingService(
        IAccountRepository accountRepository,
        IJournalEntryRepository journalEntryRepository,
        IGLAccountRepository glAccountRepository)
    {
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
        _glAccountRepository = glAccountRepository;
    }

    /// <summary>
    /// Process internal transfer between accounts within the bank
    /// </summary>
    public async Task<PaymentProcessingResult> ProcessInternalTransferAsync(
        PaymentOrder paymentOrder,
        string processedBy)
    {
        try
        {
            // 1. Validate payment order
            var validationResult = await ValidatePaymentOrderAsync(paymentOrder);
            if (!validationResult.IsValid)
            {
                return PaymentProcessingResult.Failed(validationResult.ErrorMessage);
            }

            // 2. Get accounts
            var fromAccount = await _accountRepository.GetByIdAsync(paymentOrder.FromAccountId!.Value);
            var toAccount = await _accountRepository.GetByIdAsync(paymentOrder.ToAccountId!.Value);

            if (fromAccount == null || toAccount == null)
            {
                return PaymentProcessingResult.Failed("One or both accounts not found");
            }

            // 3. Validate account balances and limits
            if (!CanDebitAccount(fromAccount, paymentOrder.Amount))
            {
                return PaymentProcessingResult.Failed("Insufficient funds or account limits exceeded");
            }

            // 4. Calculate fees
            var feeAmount = await CalculateTransferFeeAsync(fromAccount, paymentOrder.Amount, paymentOrder.Type);
            if (feeAmount.Amount > 0)
            {
                paymentOrder.SetFee(feeAmount, await GetFeeGLCodeAsync(paymentOrder.Type));
            }

            // 5. Process the transfer
            paymentOrder.Process(processedBy);

            // 6. Update account balances
            var totalDebitAmount = paymentOrder.Amount;
            if (paymentOrder.FeeAmount != null && paymentOrder.FeeBearer == FeeBearer.Sender)
            {
                totalDebitAmount += paymentOrder.FeeAmount;
            }

            fromAccount.Debit(totalDebitAmount, paymentOrder.PaymentReference, paymentOrder.Description);
            toAccount.Credit(paymentOrder.Amount, paymentOrder.PaymentReference, paymentOrder.Description);

            // 7. Create GL entries
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Standard);
            var journalEntry = GLPostingService.CreateTransferEntry(
                fromAccount,
                toAccount,
                paymentOrder.Amount,
                paymentOrder.PaymentReference,
                paymentOrder.Description,
                journalNumber,
                processedBy);

            // Add fee entry if applicable
            if (paymentOrder.FeeAmount != null && paymentOrder.FeeAmount.Amount > 0)
            {
                var feeIncomeGLCode = await GetFeeIncomeGLCodeAsync();
                journalEntry.AddDebitLine(
                    fromAccount.CustomerGLCode,
                    paymentOrder.FeeAmount.Amount,
                    description: $"Transfer fee - {paymentOrder.PaymentReference}");
                journalEntry.AddCreditLine(
                    feeIncomeGLCode,
                    paymentOrder.FeeAmount.Amount,
                    description: $"Transfer fee income - {paymentOrder.PaymentReference}");
            }

            journalEntry.Post(processedBy);
            _journalEntryRepository.Add(journalEntry);

            // 8. Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            // 9. Complete the payment
            paymentOrder.Complete(processedBy);

            return PaymentProcessingResult.Success(paymentOrder.PaymentReference, journalNumber);
        }
        catch (Exception ex)
        {
            paymentOrder.Fail(ex.Message, processedBy);
            return PaymentProcessingResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// Process external payment to another bank
    /// </summary>
    public async Task<PaymentProcessingResult> ProcessExternalPaymentAsync(
        PaymentOrder paymentOrder,
        string processedBy)
    {
        try
        {
            // 1. Validate payment order
            var validationResult = await ValidatePaymentOrderAsync(paymentOrder);
            if (!validationResult.IsValid)
            {
                return PaymentProcessingResult.Failed(validationResult.ErrorMessage);
            }

            // 2. Get sender account
            var fromAccount = await _accountRepository.GetByIdAsync(paymentOrder.FromAccountId!.Value);
            if (fromAccount == null)
            {
                return PaymentProcessingResult.Failed("Sender account not found");
            }

            // 3. Calculate fees (external payments typically have higher fees)
            var feeAmount = await CalculateTransferFeeAsync(fromAccount, paymentOrder.Amount, paymentOrder.Type);
            if (feeAmount.Amount > 0)
            {
                paymentOrder.SetFee(feeAmount, await GetFeeGLCodeAsync(paymentOrder.Type));
            }

            // 4. Validate total debit amount
            var totalDebitAmount = paymentOrder.Amount;
            if (paymentOrder.FeeAmount != null && paymentOrder.FeeBearer == FeeBearer.Sender)
            {
                totalDebitAmount += paymentOrder.FeeAmount;
            }

            if (!CanDebitAccount(fromAccount, totalDebitAmount))
            {
                return PaymentProcessingResult.Failed("Insufficient funds for payment and fees");
            }

            // 5. Process the payment
            paymentOrder.Process(processedBy);

            // 6. Debit sender account
            fromAccount.Debit(totalDebitAmount, paymentOrder.PaymentReference, paymentOrder.Description);

            // 7. Create GL entries for external payment
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Standard);
            var nostroGLCode = await GetNostroGLCodeAsync(paymentOrder.Channel);
            
            var journalEntry = JournalEntry.Create(
                journalNumber,
                DateTime.UtcNow.Date,
                paymentOrder.ValueDate ?? DateTime.UtcNow.Date,
                JournalType.Standard,
                "ExternalPayment",
                paymentOrder.Id,
                paymentOrder.PaymentReference,
                paymentOrder.Amount.Currency.Code,
                processedBy,
                $"External payment: {paymentOrder.Description}");

            // Debit: Customer Account (Liability decreases)
            journalEntry.AddDebitLine(
                fromAccount.CustomerGLCode,
                paymentOrder.Amount.Amount,
                description: $"External payment from {fromAccount.AccountNumber}");

            // Credit: Nostro Account (Asset decreases - money leaving the bank)
            journalEntry.AddCreditLine(
                nostroGLCode,
                paymentOrder.Amount.Amount,
                description: $"External payment to {paymentOrder.BeneficiaryName}");

            // Add fee entries if applicable
            if (paymentOrder.FeeAmount != null && paymentOrder.FeeAmount.Amount > 0)
            {
                var feeIncomeGLCode = await GetFeeIncomeGLCodeAsync();
                journalEntry.AddDebitLine(
                    fromAccount.CustomerGLCode,
                    paymentOrder.FeeAmount.Amount,
                    description: $"External payment fee - {paymentOrder.PaymentReference}");
                journalEntry.AddCreditLine(
                    feeIncomeGLCode,
                    paymentOrder.FeeAmount.Amount,
                    description: $"External payment fee income - {paymentOrder.PaymentReference}");
            }

            journalEntry.Post(processedBy);
            _journalEntryRepository.Add(journalEntry);

            // 8. Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            // 9. Send to external system (simulated)
            var externalReference = await SendToExternalSystemAsync(paymentOrder);
            paymentOrder.SetExternalReference(externalReference);

            // 10. Complete the payment (in real system, this would be done after external confirmation)
            paymentOrder.Complete(processedBy, externalReference);

            return PaymentProcessingResult.Success(paymentOrder.PaymentReference, journalNumber, externalReference);
        }
        catch (Exception ex)
        {
            paymentOrder.Fail(ex.Message, processedBy);
            return PaymentProcessingResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// Validate payment order before processing
    /// </summary>
    private async Task<PaymentValidationResult> ValidatePaymentOrderAsync(PaymentOrder paymentOrder)
    {
        // Basic validation
        if (paymentOrder.Amount.IsZero() || paymentOrder.Amount.IsNegative())
        {
            return PaymentValidationResult.Invalid("Payment amount must be positive");
        }

        if (paymentOrder.Status != PaymentStatus.Pending && paymentOrder.Status != PaymentStatus.Authorized)
        {
            return PaymentValidationResult.Invalid($"Cannot process payment in {paymentOrder.Status} status");
        }

        // Check if payment requires approval
        if (paymentOrder.RequiresApproval && paymentOrder.Status != PaymentStatus.Authorized)
        {
            return PaymentValidationResult.Invalid("Payment requires approval before processing");
        }

        // Validate accounts exist
        if (paymentOrder.FromAccountId.HasValue)
        {
            var fromAccount = await _accountRepository.GetByIdAsync(paymentOrder.FromAccountId.Value);
            if (fromAccount == null)
            {
                return PaymentValidationResult.Invalid("Sender account not found");
            }

            if (fromAccount.Status != AccountStatus.Active)
            {
                return PaymentValidationResult.Invalid($"Sender account is {fromAccount.Status}");
            }
        }

        if (paymentOrder.ToAccountId.HasValue)
        {
            var toAccount = await _accountRepository.GetByIdAsync(paymentOrder.ToAccountId.Value);
            if (toAccount == null)
            {
                return PaymentValidationResult.Invalid("Recipient account not found");
            }

            if (toAccount.Status == AccountStatus.Closed)
            {
                return PaymentValidationResult.Invalid("Recipient account is closed");
            }
        }

        return PaymentValidationResult.Valid();
    }

    private bool CanDebitAccount(Account account, Money amount)
    {
        var availableFunds = account.Balance + account.OverdraftLimit;
        return amount.IsLessThanOrEqualTo(availableFunds);
    }

    private async Task<Money> CalculateTransferFeeAsync(Account fromAccount, Money amount, PaymentType paymentType)
    {
        // Fee calculation logic - could be enhanced with product-based fees
        var feeRate = paymentType switch
        {
            PaymentType.InternalTransfer => 0.001m, // 0.1%
            PaymentType.ExternalTransfer => 0.005m, // 0.5%
            _ => 0m
        };

        var feeAmount = amount.Amount * feeRate;
        var minFee = paymentType == PaymentType.ExternalTransfer ? 50m : 10m;
        var maxFee = paymentType == PaymentType.ExternalTransfer ? 500m : 100m;

        feeAmount = Math.Max(feeAmount, minFee);
        feeAmount = Math.Min(feeAmount, maxFee);

        return new Money(feeAmount, amount.Currency);
    }

    private async Task<string> GetFeeGLCodeAsync(PaymentType paymentType)
    {
        // Return appropriate fee GL code based on payment type
        return paymentType switch
        {
            PaymentType.InternalTransfer => "4002", // Internal transfer fee income
            PaymentType.ExternalTransfer => "4003", // External transfer fee income
            _ => "4001" // General fee income
        };
    }

    private async Task<string> GetFeeIncomeGLCodeAsync()
    {
        return "4001"; // Fee income GL account
    }

    private async Task<string> GetNostroGLCodeAsync(PaymentChannel channel)
    {
        // Return appropriate nostro account based on channel
        return channel switch
        {
            PaymentChannel.Swift => "1101", // SWIFT nostro account
            PaymentChannel.Rtgs => "1102", // RTGS nostro account
            PaymentChannel.Eft => "1103", // EFT nostro account
            _ => "1100" // General nostro account
        };
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

    private async Task<string> SendToExternalSystemAsync(PaymentOrder paymentOrder)
    {
        // Simulate external system integration
        // In real implementation, this would integrate with SWIFT, EFT, etc.
        await Task.Delay(100); // Simulate network call
        
        return $"EXT{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }
}

/// <summary>
/// Payment processing result
/// </summary>
public class PaymentProcessingResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? PaymentReference { get; private set; }
    public string? JournalNumber { get; private set; }
    public string? ExternalReference { get; private set; }

    private PaymentProcessingResult() { }

    public static PaymentProcessingResult Success(string paymentReference, string journalNumber, string? externalReference = null)
    {
        return new PaymentProcessingResult
        {
            IsSuccess = true,
            PaymentReference = paymentReference,
            JournalNumber = journalNumber,
            ExternalReference = externalReference
        };
    }

    public static PaymentProcessingResult Failed(string errorMessage)
    {
        return new PaymentProcessingResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// Payment validation result
/// </summary>
public class PaymentValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private PaymentValidationResult() { }

    public static PaymentValidationResult Valid()
    {
        return new PaymentValidationResult { IsValid = true };
    }

    public static PaymentValidationResult Invalid(string errorMessage)
    {
        return new PaymentValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }
}