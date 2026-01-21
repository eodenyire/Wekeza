using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Exceptions;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// Card Management Service - Handles card lifecycle operations and business rules
/// Integrates with GL for automatic posting of card-related transactions
/// </summary>
public class CardManagementService
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IGLAccountRepository _glAccountRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;

    public CardManagementService(
        ICardRepository cardRepository,
        IAccountRepository accountRepository,
        ICustomerRepository customerRepository,
        IGLAccountRepository glAccountRepository,
        IJournalEntryRepository journalEntryRepository)
    {
        _cardRepository = cardRepository;
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
        _glAccountRepository = glAccountRepository;
        _journalEntryRepository = journalEntryRepository;
    }

    public async Task<Card> IssueCardAsync(
        Guid customerId,
        Guid accountId,
        CardType cardType,
        string nameOnCard,
        string deliveryAddress,
        Money dailyWithdrawalLimit,
        Money dailyPurchaseLimit,
        Money monthlyLimit,
        int maxMonthlyTransactions = 100)
    {
        // Validate customer exists and is active
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new GenericDomainException("Customer not found");

        // Validate account exists and belongs to customer
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
            throw new GenericDomainException("Account not found");

        if (account.CustomerId != customerId)
            throw new GenericDomainException("Account does not belong to the specified customer");

        // Check if customer already has active cards of this type for this account
        var existingCards = await _cardRepository.GetActiveCardsByAccountIdAsync(accountId);
        var existingCardOfType = existingCards.FirstOrDefault(c => c.CardType == cardType);
        
        if (existingCardOfType != null)
            throw new GenericDomainException($"Customer already has an active {cardType} card for this account");

        // Issue the card
        var card = Card.IssueCard(
            customerId,
            accountId,
            cardType,
            nameOnCard,
            deliveryAddress,
            dailyWithdrawalLimit,
            dailyPurchaseLimit,
            monthlyLimit,
            maxMonthlyTransactions);

        // Post card issuance fee to GL if applicable
        await PostCardIssuanceFeeAsync(card, account);

        return card;
    }

    public async Task<bool> ValidateCardForTransactionAsync(
        Guid cardId,
        decimal amount,
        TransactionType transactionType,
        string? merchantCategory = null)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            return false;

        // Basic card validation
        if (!card.CanProcessTransaction(amount, transactionType))
            return false;

        // Additional business rules based on transaction type
        switch (transactionType)
        {
            case TransactionType.ATMWithdrawal:
                return ValidateATMTransaction(card, amount);
            
            case TransactionType.POSPurchase:
                return ValidatePOSTransaction(card, amount, merchantCategory);
            
            case TransactionType.OnlinePurchase:
                return ValidateOnlineTransaction(card, amount, merchantCategory);
            
            default:
                return false;
        }
    }

    public async Task ProcessCardTransactionAsync(
        Guid cardId,
        decimal amount,
        TransactionType transactionType,
        bool isSuccessful,
        string? merchantId = null,
        string? atmId = null)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            throw new GenericDomainException("Card not found");

        // Record transaction on card
        card.RecordTransaction(amount, transactionType, isSuccessful);

        // Post transaction fees to GL if applicable
        if (isSuccessful)
        {
            await PostTransactionFeesAsync(card, amount, transactionType, merchantId, atmId);
        }

        await _cardRepository.UpdateAsync(card);
    }

    public async Task<Card> ReplaceCardAsync(
        Guid cardId,
        string replacementReason,
        string deliveryAddress,
        string requestedBy)
    {
        var existingCard = await _cardRepository.GetByIdAsync(cardId);
        if (existingCard == null)
            throw new GenericDomainException("Card not found");

        // Create replacement card
        var newCard = existingCard.Replace(replacementReason, deliveryAddress);

        // Post card replacement fee to GL
        var account = await _accountRepository.GetByIdAsync(existingCard.AccountId);
        await PostCardReplacementFeeAsync(newCard, account!);

        // Save both cards
        await _cardRepository.UpdateAsync(existingCard);
        await _cardRepository.AddAsync(newCard);

        return newCard;
    }

    public async Task BlockCardAsync(Guid cardId, string reason, string blockedBy)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            throw new GenericDomainException("Card not found");

        card.Block(reason, blockedBy);
        await _cardRepository.UpdateAsync(card);
    }

    public async Task UnblockCardAsync(Guid cardId, string unblockedBy, string reason)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            throw new GenericDomainException("Card not found");

        card.Unblock(unblockedBy, reason);
        await _cardRepository.UpdateAsync(card);
    }

    public async Task CancelCardAsync(Guid cardId, string reason, string cancelledBy)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            throw new GenericDomainException("Card not found");

        card.Cancel(reason, cancelledBy);
        await _cardRepository.UpdateAsync(card);
    }

    public async Task UpdateCardLimitsAsync(
        Guid cardId,
        Money dailyWithdrawalLimit,
        Money dailyPurchaseLimit,
        Money monthlyLimit,
        string updatedBy)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            throw new GenericDomainException("Card not found");

        card.UpdateLimits(dailyWithdrawalLimit, dailyPurchaseLimit, monthlyLimit);
        await _cardRepository.UpdateAsync(card);
    }

    public async Task UpdateChannelControlsAsync(
        Guid cardId,
        bool atmEnabled,
        bool posEnabled,
        bool onlineEnabled,
        bool internationalEnabled,
        bool contactlessEnabled,
        string updatedBy)
    {
        var card = await _cardRepository.GetByIdAsync(cardId);
        if (card == null)
            throw new GenericDomainException("Card not found");

        card.UpdateChannelControls(atmEnabled, posEnabled, onlineEnabled, internationalEnabled, contactlessEnabled);
        await _cardRepository.UpdateAsync(card);
    }

    private bool ValidateATMTransaction(Card card, decimal amount)
    {
        // Additional ATM-specific validations
        if (!card.ATMEnabled)
            return false;

        // Check for suspicious patterns (e.g., multiple withdrawals in short time)
        // This would typically integrate with fraud detection systems
        return true;
    }

    private bool ValidatePOSTransaction(Card card, decimal amount, string? merchantCategory)
    {
        // Additional POS-specific validations
        if (!card.POSEnabled)
            return false;

        // Check merchant category restrictions if any
        // This would typically check against customer preferences or bank policies
        return true;
    }

    private bool ValidateOnlineTransaction(Card card, decimal amount, string? merchantCategory)
    {
        // Additional online-specific validations
        if (!card.OnlineEnabled)
            return false;

        // Enhanced fraud checks for online transactions
        // This would typically integrate with fraud detection systems
        return true;
    }

    private async Task PostCardIssuanceFeeAsync(Card card, Account account)
    {
        // Get card issuance fee from product configuration
        var cardIssuanceFee = GetCardIssuanceFee(card.CardType);
        if (cardIssuanceFee.Amount <= 0)
            return;

        // Get GL accounts
        var customerAccountGL = await _glAccountRepository.GetByCodeAsync(account.GLAccountCode);
        var cardFeeIncomeGL = await _glAccountRepository.GetByCodeAsync("4200"); // Card Fee Income

        if (customerAccountGL == null || cardFeeIncomeGL == null)
            throw new GenericDomainException("Required GL accounts not found for card issuance fee posting");

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"CARD-ISS-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "CARD_ISSUANCE",
            card.Id,
            $"Card issuance fee - Card: {card.CardNumber[^4..]}",
            cardIssuanceFee.Currency.Code,
            "CARD_SYSTEM",
            $"Card issuance fee - Card: {card.CardNumber[^4..]}");

        // Dr. Customer Account, Cr. Card Fee Income
        journalEntry.AddDebitEntry(customerAccountGL.GLCode, cardIssuanceFee.Amount, $"Card issuance fee - {card.CardType}");
        journalEntry.AddCreditEntry(cardFeeIncomeGL.GLCode, cardIssuanceFee.Amount, $"Card issuance fee income - {card.CardType}");

        await _journalEntryRepository.AddAsync(journalEntry);
    }

    private async Task PostCardReplacementFeeAsync(Card card, Account account)
    {
        // Get card replacement fee from product configuration
        var cardReplacementFee = GetCardReplacementFee(card.CardType);
        if (cardReplacementFee.Amount <= 0)
            return;

        // Get GL accounts
        var customerAccountGL = await _glAccountRepository.GetByCodeAsync(account.GLAccountCode);
        var cardFeeIncomeGL = await _glAccountRepository.GetByCodeAsync("4200"); // Card Fee Income

        if (customerAccountGL == null || cardFeeIncomeGL == null)
            throw new GenericDomainException("Required GL accounts not found for card replacement fee posting");

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"CARD-REP-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "CARD_REPLACEMENT",
            card.Id,
            $"Card replacement fee - Card: {card.CardNumber[^4..]}",
            cardReplacementFee.Currency.Code,
            "CARD_SYSTEM",
            $"Card replacement fee - Card: {card.CardNumber[^4..]}");

        // Dr. Customer Account, Cr. Card Fee Income
        journalEntry.AddDebitEntry(customerAccountGL.GLCode, cardReplacementFee.Amount, $"Card replacement fee - {card.CardType}");
        journalEntry.AddCreditEntry(cardFeeIncomeGL.GLCode, cardReplacementFee.Amount, $"Card replacement fee income - {card.CardType}");

        await _journalEntryRepository.AddAsync(journalEntry);
    }

    private async Task PostTransactionFeesAsync(
        Card card,
        decimal amount,
        TransactionType transactionType,
        string? merchantId,
        string? atmId)
    {
        var transactionFee = GetTransactionFee(card.CardType, transactionType, amount);
        if (transactionFee.Amount <= 0)
            return;

        var account = await _accountRepository.GetByIdAsync(card.AccountId);
        var customerAccountGL = await _glAccountRepository.GetByCodeAsync(account!.GLAccountCode);
        var transactionFeeIncomeGL = await _glAccountRepository.GetByCodeAsync("4210"); // Transaction Fee Income

        if (customerAccountGL == null || transactionFeeIncomeGL == null)
            return; // Skip fee posting if GL accounts not found

        // Create journal entry
        var journalEntry = JournalEntry.Create(
            $"CARD-TXN-{DateTime.UtcNow:yyyyMMddHHmmss}",
            DateTime.UtcNow,
            DateTime.UtcNow,
            JournalType.Standard,
            "CARD_TRANSACTION_FEE",
            card.Id,
            $"Card transaction fee - {transactionType} - Card: {card.CardNumber[^4..]}",
            transactionFee.Currency.Code,
            "CARD_SYSTEM",
            $"Card transaction fee - {transactionType} - Card: {card.CardNumber[^4..]}");

        // Dr. Customer Account, Cr. Transaction Fee Income
        journalEntry.AddDebitEntry(customerAccountGL.GLCode, transactionFee.Amount, $"{transactionType} fee");
        journalEntry.AddCreditEntry(transactionFeeIncomeGL.GLCode, transactionFee.Amount, $"{transactionType} fee income");

        await _journalEntryRepository.AddAsync(journalEntry);
    }

    private Money GetCardIssuanceFee(CardType cardType)
    {
        // This would typically come from product configuration
        return cardType switch
        {
            CardType.Debit => new Money(500, Currency.KES), // KES 500
            CardType.Credit => new Money(1000, Currency.KES), // KES 1000
            CardType.Prepaid => new Money(200, Currency.KES), // KES 200
            _ => Money.Zero(Currency.KES)
        };
    }

    private Money GetCardReplacementFee(CardType cardType)
    {
        // This would typically come from product configuration
        return cardType switch
        {
            CardType.Debit => new Money(300, Currency.KES), // KES 300
            CardType.Credit => new Money(500, Currency.KES), // KES 500
            CardType.Prepaid => new Money(200, Currency.KES), // KES 200
            _ => Money.Zero(Currency.KES)
        };
    }

    private Money GetTransactionFee(CardType cardType, TransactionType transactionType, decimal amount)
    {
        // This would typically come from product configuration
        return transactionType switch
        {
            TransactionType.ATMWithdrawal when amount > 10000 => new Money(50, Currency.KES), // KES 50 for large withdrawals
            TransactionType.POSPurchase => Money.Zero(Currency.KES), // No POS fees
            TransactionType.OnlinePurchase => new Money(20, Currency.KES), // KES 20 for online purchases
            _ => Money.Zero(Currency.KES)
        };
    }
}