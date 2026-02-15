using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Exceptions;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// POS Processing Service - Handles Point-of-Sale transaction processing and authorization
/// Supports purchase, refund, and pre-authorization transactions with merchant settlement
/// </summary>
public class POSProcessingService
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IGLAccountRepository _glAccountRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly CardManagementService _cardManagementService;

    public POSProcessingService(
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

    public async Task<POSTransaction> ProcessPurchaseAsync(
        string cardNumber,
        string? encryptedPIN,
        decimal amount,
        decimal? tipAmount,
        string merchantId,
        string merchantName,
        string merchantCategory,
        string terminalId,
        bool isContactless = false,
        bool requiresPIN = true)
    {
        // Get card by number
        var card = await _cardRepository.GetByCardNumberAsync(cardNumber);
        if (card == null)
        {
            var failedTransaction = CreateFailedPurchase(cardNumber, amount, tipAmount, merchantId, merchantName, merchantCategory, terminalId, "Invalid card number");
            return failedTransaction;
        }

        // Validate PIN if required
        if (requiresPIN && !string.IsNullOrEmpty(encryptedPIN))
        {
            if (!card.ValidatePIN(encryptedPIN))
            {
                var failedTransaction = POSTransaction.CreatePurchase(
                    merchantId, merchantName, merchantCategory, terminalId,
                    card.Id, card.AccountId, card.CustomerId,
                    MaskCardNumber(cardNumber), new Money(amount, Currency.KES),
                    tipAmount.HasValue ? new Money(tipAmount.Value, Currency.KES) : null,
                    Money.Zero(Currency.KES), isContactless);
                
                failedTransaction.SetPINVerificationResult(false);
                failedTransaction.Decline("55", "Incorrect PIN", "PIN validation failed");
                
                await _cardManagementService.ProcessCardTransactionAsync(
                    card.Id, amount + (tipAmount ?? 0), CardTransactionType.POSPurchase, false, merchantId);
                
                return failedTransaction;
            }
        }

        // Get account and validate
        var account = await _accountRepository.GetByIdAsync(card.AccountId);
        if (account == null)
        {
            var failedTransaction = POSTransaction.CreatePurchase(
                merchantId, merchantName, merchantCategory, terminalId,
                card.Id, card.AccountId, card.CustomerId,
                MaskCardNumber(cardNumber), new Money(amount, Currency.KES),
                tipAmount.HasValue ? new Money(tipAmount.Value, Currency.KES) : null,
                Money.Zero(Currency.KES), isContactless);
            
            failedTransaction.Decline("14", "Invalid account", "Account not found");
            return failedTransaction;
        }

        var purchaseAmount = new Money(amount, account.Currency);
        var tip = tipAmount.HasValue ? new Money(tipAmount.Value, account.Currency) : null;
        var totalAmount = tip != null ? purchaseAmount + tip : purchaseAmount;
        
        // Create POS transaction
        var posTransaction = POSTransaction.CreatePurchase(
            merchantId, merchantName, merchantCategory, terminalId,
            card.Id, card.AccountId, card.CustomerId,
            MaskCardNumber(cardNumber), purchaseAmount, tip, account.Balance, isContactless);

        if (requiresPIN)
            posTransaction.SetPINVerificationResult(true);

        // Validate card can process transaction
        if (!card.CanProcessTransaction(totalAmount.Amount, CardTransactionType.POSPurchase))
        {
            posTransaction.Decline("61", "Exceeds purchase limit", "Daily purchase limit exceeded");
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, totalAmount.Amount, CardTransactionType.POSPurchase, false, merchantId);
            return posTransaction;
        }

        // Check account balance
        if (account.Balance < totalAmount)
        {
            posTransaction.Decline("51", "Insufficient funds", "Account balance insufficient");
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, totalAmount.Amount, CardTransactionType.POSPurchase, false, merchantId);
            return posTransaction;
        }

        // Check account status
        if (account.Status != AccountStatus.Active)
        {
            posTransaction.Decline("62", "Restricted card", "Account is not active");
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, totalAmount.Amount, CardTransactionType.POSPurchase, false, merchantId);
            return posTransaction;
        }

        // Check merchant category restrictions
        if (!IsAllowedMerchantCategory(card, merchantCategory))
        {
            posTransaction.Decline("57", "Transaction not permitted", "Merchant category restricted");
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, totalAmount.Amount, CardTransactionType.POSPurchase, false, merchantId);
            return posTransaction;
        }

        try
        {
            // Process purchase
            account.Withdraw(totalAmount, $"POS Purchase - {merchantName}", "POS_SYSTEM");
            
            // Authorize transaction
            var authCode = GenerateAuthorizationCode();
            posTransaction.Authorize(authCode, account.Balance);
            
            // Post to GL
            await PostPOSPurchaseToGLAsync(account, totalAmount, merchantId, merchantName, merchantCategory);
            
            // Complete transaction
            var receiptNumber = GenerateReceiptNumber(terminalId);
            var batchNumber = GenerateBatchNumber();
            posTransaction.Complete(receiptNumber, batchNumber);
            
            // Update card transaction tracking
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, totalAmount.Amount, CardTransactionType.POSPurchase, true, merchantId);
            
            // Update repositories
            await _accountRepository.UpdateAsync(account);
            await _cardRepository.UpdateAsync(card);
            
            return posTransaction;
        }
        catch (DomainException ex)
        {
            posTransaction.Decline("96", "System error", ex.Message);
            await _cardManagementService.ProcessCardTransactionAsync(
                card.Id, totalAmount.Amount, CardTransactionType.POSPurchase, false, merchantId);
            return posTransaction;
        }
    }

    public async Task<POSTransaction> ProcessRefundAsync(
        string cardNumber,
        decimal refundAmount,
        string merchantId,
        string merchantName,
        string merchantCategory,
        string terminalId,
        Guid originalTransactionId)
    {
        // Get card by number
        var card = await _cardRepository.GetByCardNumberAsync(cardNumber);
        if (card == null)
        {
            var failedTransaction = CreateFailedRefund(cardNumber, refundAmount, merchantId, merchantName, merchantCategory, terminalId, "Invalid card number");
            return failedTransaction;
        }

        // Get account
        var account = await _accountRepository.GetByIdAsync(card.AccountId);
        if (account == null)
        {
            var failedTransaction = POSTransaction.CreateRefund(
                merchantId, merchantName, merchantCategory, terminalId,
                card.Id, card.AccountId, card.CustomerId,
                MaskCardNumber(cardNumber), new Money(refundAmount, Currency.KES),
                Money.Zero(Currency.KES), originalTransactionId);
            
            failedTransaction.Decline("14", "Invalid account", "Account not found");
            return failedTransaction;
        }

        var refund = new Money(refundAmount, account.Currency);
        
        // Create POS refund transaction
        var posTransaction = POSTransaction.CreateRefund(
            merchantId, merchantName, merchantCategory, terminalId,
            card.Id, card.AccountId, card.CustomerId,
            MaskCardNumber(cardNumber), refund, account.Balance, originalTransactionId);

        try
        {
            // Process refund (credit customer account)
            account.Deposit(refund, $"POS Refund - {merchantName}", "POS_SYSTEM");
            
            // Authorize transaction
            var authCode = GenerateAuthorizationCode();
            posTransaction.Authorize(authCode, account.Balance);
            
            // Post to GL
            await PostPOSRefundToGLAsync(account, refund, merchantId, merchantName);
            
            // Complete transaction
            var receiptNumber = GenerateReceiptNumber(terminalId);
            var batchNumber = GenerateBatchNumber();
            posTransaction.Complete(receiptNumber, batchNumber);
            
            // Update repositories
            await _accountRepository.UpdateAsync(account);
            
            return posTransaction;
        }
        catch (DomainException ex)
        {
            posTransaction.Decline("96", "System error", ex.Message);
            return posTransaction;
        }
    }

    public async Task<bool> ValidateCardForPOSAsync(string cardNumber, decimal amount, string merchantCategory)
    {
        var card = await _cardRepository.GetByCardNumberAsync(cardNumber);
        if (card == null)
            return false;

        // Check if POS is enabled
        if (!card.POSEnabled)
            return false;

        // Check transaction limits
        if (!card.CanProcessTransaction(amount, CardTransactionType.POSPurchase))
            return false;

        // Check merchant category restrictions
        if (!IsAllowedMerchantCategory(card, merchantCategory))
            return false;

        return true;
    }

    public async Task SettleTransactionsAsync(List<Guid> transactionIds, string settlementBatchId)
    {
        // This would typically be called by a batch settlement process
        // For now, we'll create a placeholder implementation
        foreach (var transactionId in transactionIds)
        {
            // Get transaction and mark as settled
            // This would involve updating the POSTransaction aggregate
            // and posting settlement entries to GL
        }
    }

    private async Task PostPOSPurchaseToGLAsync(Account account, Money amount, string merchantId, string merchantName, string merchantCategory)
    {
        // Get GL accounts
        var customerAccountGL = await _glAccountRepository.GetByCodeAsync(account.GLAccountCode);
        var merchantSettlementGL = await _glAccountRepository.GetByCodeAsync("2100"); // Merchant Settlement Payable
        
        if (customerAccountGL == null || merchantSettlementGL == null)
            throw new GenericDomainException("Required GL accounts not found for POS purchase posting");

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"POS-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "POS_PURCHASE",
            Guid.NewGuid(),
            $"POS Purchase - Merchant: {merchantName} - Account: {account.AccountNumber}",
            account.Currency.Code,
            "POS_SYSTEM",
            $"POS Purchase - Merchant: {merchantName} - Account: {account.AccountNumber}");

        // Dr. Merchant Settlement Payable, Cr. Customer Account
        journalEntry.AddDebitEntry(merchantSettlementGL.GLCode, amount.Amount, $"POS purchase - {merchantName}");
        journalEntry.AddCreditEntry(customerAccountGL.GLCode, amount.Amount, $"POS purchase - Account: {account.AccountNumber}");

        await _journalEntryRepository.AddAsync(journalEntry);

        // Post interchange income
        await PostInterchangeIncomeAsync(account, amount, merchantCategory);
    }

    private async Task PostPOSRefundToGLAsync(Account account, Money amount, string merchantId, string merchantName)
    {
        // Get GL accounts
        var customerAccountGL = await _glAccountRepository.GetByCodeAsync(account.GLAccountCode);
        var merchantSettlementGL = await _glAccountRepository.GetByCodeAsync("2100"); // Merchant Settlement Payable
        
        if (customerAccountGL == null || merchantSettlementGL == null)
            throw new GenericDomainException("Required GL accounts not found for POS refund posting");

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"POS-REF-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "POS_REFUND",
            Guid.NewGuid(),
            $"POS Refund - Merchant: {merchantName} - Account: {account.AccountNumber}",
            account.Currency.Code,
            "POS_SYSTEM",
            $"POS Refund - Merchant: {merchantName} - Account: {account.AccountNumber}");

        // Dr. Customer Account, Cr. Merchant Settlement Payable (reverse of purchase)
        journalEntry.AddDebitEntry(customerAccountGL.GLCode, amount.Amount, $"POS refund - Account: {account.AccountNumber}");
        journalEntry.AddCreditEntry(merchantSettlementGL.GLCode, amount.Amount, $"POS refund - {merchantName}");

        await _journalEntryRepository.AddAsync(journalEntry);
    }

    private async Task PostInterchangeIncomeAsync(Account account, Money transactionAmount, string merchantCategory)
    {
        // Calculate interchange income (varies by merchant category)
        var interchangeRate = GetInterchangeRate(merchantCategory);
        var interchangeIncome = new Money(transactionAmount.Amount * interchangeRate, transactionAmount.Currency);
        
        var merchantSettlementGL = await _glAccountRepository.GetByCodeAsync("2100"); // Merchant Settlement Payable
        var interchangeIncomeGL = await _glAccountRepository.GetByCodeAsync("4300"); // Interchange Income
        
        if (merchantSettlementGL == null || interchangeIncomeGL == null)
            return; // Skip posting if GL accounts not found

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"POS-INT-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "INTERCHANGE_INCOME",
            Guid.NewGuid(),
            $"Interchange Income - Category: {merchantCategory} - Account: {account.AccountNumber}",
            interchangeIncome.Currency.Code,
            "POS_SYSTEM",
            $"Interchange Income - Category: {merchantCategory} - Account: {account.AccountNumber}");

        // Dr. Merchant Settlement Payable, Cr. Interchange Income
        journalEntry.AddDebitEntry(merchantSettlementGL.GLCode, interchangeIncome.Amount, $"Interchange income - {merchantCategory}");
        journalEntry.AddCreditEntry(interchangeIncomeGL.GLCode, interchangeIncome.Amount, "Interchange income");

        await _journalEntryRepository.AddAsync(journalEntry);
    }

    private bool IsAllowedMerchantCategory(Card card, string merchantCategory)
    {
        // This would typically check against customer preferences or bank policies
        // For now, we'll implement basic restrictions
        var restrictedCategories = new[] { "7995", "6010", "6011" }; // Gambling, ATM, Cash Advance
        
        return !restrictedCategories.Contains(merchantCategory);
    }

    private decimal GetInterchangeRate(string merchantCategory)
    {
        // Interchange rates vary by merchant category
        return merchantCategory switch
        {
            "5411" => 0.0175m, // Grocery stores - 1.75%
            "5812" => 0.0225m, // Restaurants - 2.25%
            "5541" => 0.0250m, // Service stations - 2.50%
            "5999" => 0.0200m, // Miscellaneous retail - 2.00%
            _ => 0.0200m       // Default rate - 2.00%
        };
    }

    private POSTransaction CreateFailedPurchase(string cardNumber, decimal amount, decimal? tipAmount, string merchantId, string merchantName, string merchantCategory, string terminalId, string failureReason)
    {
        var failedTransaction = POSTransaction.CreatePurchase(
            merchantId, merchantName, merchantCategory, terminalId,
            Guid.Empty, Guid.Empty, Guid.Empty,
            MaskCardNumber(cardNumber), new Money(amount, Currency.KES),
            tipAmount.HasValue ? new Money(tipAmount.Value, Currency.KES) : null,
            Money.Zero(Currency.KES));
        
        failedTransaction.Decline("14", "Invalid card", failureReason);
        return failedTransaction;
    }

    private POSTransaction CreateFailedRefund(string cardNumber, decimal refundAmount, string merchantId, string merchantName, string merchantCategory, string terminalId, string failureReason)
    {
        var failedTransaction = POSTransaction.CreateRefund(
            merchantId, merchantName, merchantCategory, terminalId,
            Guid.Empty, Guid.Empty, Guid.Empty,
            MaskCardNumber(cardNumber), new Money(refundAmount, Currency.KES),
            Money.Zero(Currency.KES), Guid.Empty);
        
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

    private string GenerateReceiptNumber(string terminalId)
    {
        return $"{terminalId}{DateTime.UtcNow:yyyyMMddHHmmss}";
    }

    private string GenerateBatchNumber()
    {
        return $"B{DateTime.UtcNow:yyyyMMdd}{new Random().Next(1000, 9999)}";
    }
}