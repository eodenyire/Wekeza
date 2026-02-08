using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Exceptions;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Enhanced Card Aggregate - Complete card lifecycle management for enterprise CBS
/// Supports debit, credit, and prepaid cards with full security controls
/// </summary>
public class Card : AggregateRoot
{
    public Guid AccountId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CardNumber { get; private set; }
    public CardType CardType { get; private set; }
    public CardStatus Status { get; private set; }
    public string NameOnCard { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public string CVV { get; private set; }
    public string EncryptedPIN { get; private set; }
    public bool IsPINSet { get; private set; }
    public int PINAttempts { get; private set; }
    public bool IsPINBlocked { get; private set; }
    public DateTime? PINBlockedUntil { get; private set; }
    
    // Computed properties for compatibility
    public string MaskedCardNumber => $"****-****-****-{CardNumber.Substring(CardNumber.Length - 4)}";
    public bool IsCancelled => Status == CardStatus.Cancelled;
    public bool CanWithdraw(decimal amount) => CanProcessTransaction(amount, TransactionType.ATMWithdrawal);
    
    // Limits and Controls
    public Money DailyWithdrawalLimit { get; private set; }
    public Money DailyPurchaseLimit { get; private set; }
    public Money MonthlyLimit { get; private set; }
    public Money DailyWithdrawnToday { get; private set; }
    public Money DailyPurchasedToday { get; private set; }
    public Money MonthlySpent { get; private set; }
    public DateTime LastTransactionDate { get; private set; }
    public int MonthlyTransactionCount { get; private set; }
    public int MaxMonthlyTransactions { get; private set; }
    
    // Card Management
    public string? BlockReason { get; private set; }
    public string? BlockedBy { get; private set; }
    public DateTime? BlockedDate { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? CancelledBy { get; private set; }
    public DateTime? CancelledDate { get; private set; }
    public string DeliveryAddress { get; private set; }
    public CardDeliveryStatus DeliveryStatus { get; private set; }
    public DateTime? DeliveredDate { get; private set; }
    public string? ActivationCode { get; private set; }
    public DateTime? ActivatedDate { get; private set; }
    public string? ActivatedBy { get; private set; }
    
    // Replacement Management
    public Guid? ReplacedCardId { get; private set; }
    public string? ReplacementReason { get; private set; }
    public DateTime? ReplacementDate { get; private set; }
    
    // Security and Fraud
    public bool IsHotlisted { get; private set; }
    public string? HotlistReason { get; private set; }
    public DateTime? HotlistedDate { get; private set; }
    public int ConsecutiveFailedTransactions { get; private set; }
    public DateTime? LastFailedTransactionDate { get; private set; }
    
    // Channel Controls
    public bool ATMEnabled { get; private set; }
    public bool POSEnabled { get; private set; }
    public bool OnlineEnabled { get; private set; }
    public bool InternationalEnabled { get; private set; }
    public bool ContactlessEnabled { get; private set; }
    
    private Card() : base(Guid.NewGuid()) { }

    public static Card IssueCard(
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
        var card = new Card
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            AccountId = accountId,
            CardNumber = GenerateCardNumber(),
            CardType = cardType,
            Status = CardStatus.Issued,
            NameOnCard = nameOnCard,
            ExpiryDate = DateTime.UtcNow.AddYears(3),
            CVV = GenerateCVV(),
            ActivationCode = GenerateActivationCode(),
            DeliveryAddress = deliveryAddress,
            DeliveryStatus = CardDeliveryStatus.Pending,
            DailyWithdrawalLimit = dailyWithdrawalLimit,
            DailyPurchaseLimit = dailyPurchaseLimit,
            MonthlyLimit = monthlyLimit,
            MaxMonthlyTransactions = maxMonthlyTransactions,
            DailyWithdrawnToday = Money.Zero(dailyWithdrawalLimit.Currency),
            DailyPurchasedToday = Money.Zero(dailyWithdrawalLimit.Currency),
            MonthlySpent = Money.Zero(dailyWithdrawalLimit.Currency),
            ATMEnabled = true,
            POSEnabled = true,
            OnlineEnabled = false, // Requires activation
            InternationalEnabled = false,
            ContactlessEnabled = true
        };

        card.SetAuditInfo("SYSTEM");

        card.AddDomainEvent(new CardIssuedDomainEvent(card.Id, customerId, accountId, cardType));
        return card;
    }

    public void Activate(string activationCode, string activatedBy)
    {
        if (Status != CardStatus.Issued)
            throw new GenericDomainException($"Card cannot be activated. Current status: {Status}");

        if (ActivationCode != activationCode)
            throw new GenericDomainException("Invalid activation code");

        if (DateTime.UtcNow > ExpiryDate)
            throw new GenericDomainException("Card has expired and cannot be activated");

        Status = CardStatus.Active;
        ActivatedDate = DateTime.UtcNow;
        ActivatedBy = activatedBy;
        OnlineEnabled = true; // Enable online transactions after activation
        
        AddDomainEvent(new CardActivatedDomainEvent(Id, CustomerId, AccountId));
    }

    public void SetPIN(string encryptedPIN, string setBy)
    {
        if (Status != CardStatus.Active)
            throw new GenericDomainException("Card must be active to set PIN");

        EncryptedPIN = encryptedPIN;
        IsPINSet = true;
        PINAttempts = 0;
        IsPINBlocked = false;
        PINBlockedUntil = null;
        
        AddDomainEvent(new CardPINSetDomainEvent(Id, CustomerId));
    }

    public bool ValidatePIN(string encryptedPIN)
    {
        if (!IsPINSet)
            throw new GenericDomainException("PIN is not set for this card");

        if (IsPINBlocked && PINBlockedUntil > DateTime.UtcNow)
            throw new GenericDomainException($"PIN is blocked until {PINBlockedUntil}");

        if (EncryptedPIN == encryptedPIN)
        {
            PINAttempts = 0;
            IsPINBlocked = false;
            PINBlockedUntil = null;
            return true;
        }

        PINAttempts++;
        if (PINAttempts >= 3)
        {
            IsPINBlocked = true;
            PINBlockedUntil = DateTime.UtcNow.AddHours(24);
            AddDomainEvent(new CardPINBlockedDomainEvent(Id, CustomerId, PINAttempts));
        }

        return false;
    }

    public void Block(string reason, string blockedBy)
    {
        if (Status == CardStatus.Blocked)
            throw new GenericDomainException("Card is already blocked");

        if (Status == CardStatus.Cancelled)
            throw new GenericDomainException("Cannot block a cancelled card");

        Status = CardStatus.Blocked;
        BlockReason = reason;
        BlockedBy = blockedBy;
        BlockedDate = DateTime.UtcNow;
        
        AddDomainEvent(new CardBlockedDomainEvent(Id, CustomerId, reason));
    }

    public void Unblock(string unblockedBy, string reason)
    {
        if (Status != CardStatus.Blocked)
            throw new GenericDomainException("Card is not blocked");

        Status = CardStatus.Active;
        BlockReason = null;
        BlockedBy = null;
        BlockedDate = null;
        
        AddDomainEvent(new CardUnblockedDomainEvent(Id, CustomerId, reason));
    }

    public void Cancel(string reason, string cancelledBy)
    {
        if (Status == CardStatus.Cancelled)
            throw new GenericDomainException("Card is already cancelled");

        Status = CardStatus.Cancelled;
        CancellationReason = reason;
        CancelledBy = cancelledBy;
        CancelledDate = DateTime.UtcNow;
        
        AddDomainEvent(new CardCancelledDomainEvent(Id, CustomerId, AccountId, reason));
    }

    public Card Replace(string replacementReason, string deliveryAddress)
    {
        if (Status == CardStatus.Cancelled)
            throw new GenericDomainException("Cannot replace a cancelled card");

        // Create new card
        var newCard = IssueCard(
            CustomerId,
            AccountId,
            CardType,
            NameOnCard,
            deliveryAddress,
            DailyWithdrawalLimit,
            DailyPurchaseLimit,
            MonthlyLimit,
            MaxMonthlyTransactions);

        // Update current card
        Status = CardStatus.Replaced;
        ReplacementReason = replacementReason;
        ReplacementDate = DateTime.UtcNow;
        
        // Link cards
        newCard.ReplacedCardId = Id;
        
        AddDomainEvent(new CardReplacedDomainEvent(Id, newCard.Id, CustomerId, replacementReason));
        
        return newCard;
    }

    public bool CanProcessTransaction(decimal amount, TransactionType transactionType)
    {
        if (Status != CardStatus.Active)
            return false;

        if (DateTime.UtcNow > ExpiryDate)
            return false;

        if (IsHotlisted)
            return false;

        // Reset daily counters if new day
        if (LastTransactionDate.Date < DateTime.UtcNow.Date)
        {
            DailyWithdrawnToday = Money.Zero(DailyWithdrawalLimit.Currency);
            DailyPurchasedToday = Money.Zero(DailyPurchaseLimit.Currency);
        }

        // Reset monthly counters if new month
        if (LastTransactionDate.Month != DateTime.UtcNow.Month || 
            LastTransactionDate.Year != DateTime.UtcNow.Year)
        {
            MonthlySpent = Money.Zero(MonthlyLimit.Currency);
            MonthlyTransactionCount = 0;
        }

        // Check transaction count limit
        if (MonthlyTransactionCount >= MaxMonthlyTransactions)
            return false;

        var transactionAmount = new Money(amount, DailyWithdrawalLimit.Currency);

        // Check limits based on transaction type
        return transactionType switch
        {
            TransactionType.ATMWithdrawal => 
                ATMEnabled && (DailyWithdrawnToday + transactionAmount) <= DailyWithdrawalLimit &&
                (MonthlySpent + transactionAmount) <= MonthlyLimit,
            TransactionType.POSPurchase => 
                POSEnabled && (DailyPurchasedToday + transactionAmount) <= DailyPurchaseLimit &&
                (MonthlySpent + transactionAmount) <= MonthlyLimit,
            TransactionType.OnlinePurchase => 
                OnlineEnabled && (DailyPurchasedToday + transactionAmount) <= DailyPurchaseLimit &&
                (MonthlySpent + transactionAmount) <= MonthlyLimit,
            _ => false
        };
    }

    public void RecordWithdrawal(decimal amount, string transactionReference)
    {
        RecordTransaction(amount, TransactionType.ATMWithdrawal, true);
        AddDomainEvent(new CardTransactionProcessedDomainEvent(Id, CustomerId, AccountId, amount, TransactionType.ATMWithdrawal, transactionReference));
    }

    public void RecordTransaction(decimal amount, TransactionType transactionType, bool isSuccessful)
    {
        var transactionAmount = new Money(amount, DailyWithdrawalLimit.Currency);
        
        if (isSuccessful)
        {
            // Reset daily counters if new day
            if (LastTransactionDate.Date < DateTime.UtcNow.Date)
            {
                DailyWithdrawnToday = Money.Zero(DailyWithdrawalLimit.Currency);
                DailyPurchasedToday = Money.Zero(DailyPurchaseLimit.Currency);
            }

            // Reset monthly counters if new month
            if (LastTransactionDate.Month != DateTime.UtcNow.Month || 
                LastTransactionDate.Year != DateTime.UtcNow.Year)
            {
                MonthlySpent = Money.Zero(MonthlyLimit.Currency);
                MonthlyTransactionCount = 0;
            }

            // Update counters
            switch (transactionType)
            {
                case TransactionType.ATMWithdrawal:
                    DailyWithdrawnToday += transactionAmount;
                    break;
                case TransactionType.POSPurchase:
                case TransactionType.OnlinePurchase:
                    DailyPurchasedToday += transactionAmount;
                    break;
            }

            MonthlySpent += transactionAmount;
            MonthlyTransactionCount++;
            ConsecutiveFailedTransactions = 0;
        }
        else
        {
            ConsecutiveFailedTransactions++;
            LastFailedTransactionDate = DateTime.UtcNow;
            
            // Auto-block after 5 consecutive failures
            if (ConsecutiveFailedTransactions >= 5)
            {
                Block("Consecutive failed transactions", "SYSTEM");
            }
        }

        LastTransactionDate = DateTime.UtcNow;
    }

    public void UpdateChannelControls(
        bool atmEnabled = true,
        bool posEnabled = true,
        bool onlineEnabled = true,
        bool internationalEnabled = false,
        bool contactlessEnabled = true)
    {
        ATMEnabled = atmEnabled;
        POSEnabled = posEnabled;
        OnlineEnabled = onlineEnabled;
        InternationalEnabled = internationalEnabled;
        ContactlessEnabled = contactlessEnabled;
        
        AddDomainEvent(new CardChannelControlsUpdatedDomainEvent(Id, CustomerId));
    }

    public void UpdateLimits(Money dailyWithdrawalLimit, Money dailyPurchaseLimit, Money monthlyLimit)
    {
        DailyWithdrawalLimit = dailyWithdrawalLimit;
        DailyPurchaseLimit = dailyPurchaseLimit;
        MonthlyLimit = monthlyLimit;
        
        AddDomainEvent(new CardLimitsUpdatedDomainEvent(Id, CustomerId));
    }

    public void Hotlist(string reason)
    {
        IsHotlisted = true;
        HotlistReason = reason;
        HotlistedDate = DateTime.UtcNow;
        
        AddDomainEvent(new CardHotlistedDomainEvent(Id, CustomerId, reason));
    }

    public void RemoveFromHotlist()
    {
        IsHotlisted = false;
        HotlistReason = null;
        HotlistedDate = null;
        
        AddDomainEvent(new CardRemovedFromHotlistDomainEvent(Id, CustomerId));
    }

    public void MarkAsDelivered()
    {
        DeliveryStatus = CardDeliveryStatus.Delivered;
        DeliveredDate = DateTime.UtcNow;
        
        AddDomainEvent(new CardDeliveredDomainEvent(Id, CustomerId));
    }

    private static string GenerateCardNumber()
    {
        // Wekeza Bank BIN: 5399 (Mastercard range)
        var random = new Random();
        var cardNumber = "5399";
        
        // Generate 12 more digits
        for (int i = 0; i < 12; i++)
        {
            cardNumber += random.Next(0, 10).ToString();
        }
        
        return cardNumber;
    }

    private static string GenerateCVV()
    {
        var random = new Random();
        return random.Next(100, 999).ToString();
    }

    private static string GenerateActivationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}

// Enums for Card Management
public enum CardType
{
    Debit = 1,
    Credit = 2,
    Prepaid = 3
}

public enum CardStatus
{
    Issued = 1,
    Active = 2,
    Blocked = 3,
    Cancelled = 4,
    Expired = 5,
    Replaced = 6
}

public enum CardDeliveryStatus
{
    Pending = 1,
    InTransit = 2,
    Delivered = 3,
    Failed = 4,
    Returned = 5
}

public enum TransactionType
{
    Deposit = 0,          // Account deposit
    Withdrawal = 1,       // Account withdrawal  
    ATMWithdrawal = 2,
    POSPurchase = 3,
    OnlinePurchase = 4,
    BalanceInquiry = 5,
    PINChange = 6
}



