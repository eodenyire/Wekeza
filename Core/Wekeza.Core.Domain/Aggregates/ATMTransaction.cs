using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// ATM Transaction Aggregate - Handles all ATM-related transactions
/// Supports cash withdrawal, balance inquiry, PIN change, and mini-statement
/// </summary>
public class ATMTransaction : AggregateRoot
{
    public string ATMId { get; private set; }
    public string ATMLocation { get; private set; }
    public Guid CardId { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CardNumber { get; private set; } // Masked for security
    public ATMTransactionType TransactionType { get; private set; }
    public Money Amount { get; private set; }
    public ATMTransactionStatus Status { get; private set; }
    public string? ResponseCode { get; private set; }
    public string? ResponseMessage { get; private set; }
    public DateTime TransactionDateTime { get; private set; }
    public string? AuthorizationCode { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public Money AccountBalanceBefore { get; private set; }
    public Money AccountBalanceAfter { get; private set; }
    public string? FailureReason { get; private set; }
    public int RetryCount { get; private set; }
    public bool IsReversed { get; private set; }
    public DateTime? ReversedDate { get; private set; }
    public string? ReversalReason { get; private set; }
    public Guid? ReversalTransactionId { get; private set; }
    
    // ATM Specific Details
    public string? ReceiptNumber { get; private set; }
    public bool ReceiptPrinted { get; private set; }
    public string? ATMTerminalId { get; private set; }
    public string? NetworkId { get; private set; }
    public bool IsOnUs { get; private set; } // True if our bank's ATM
    public Money? InterchangeFee { get; private set; }
    public Money? ATMFee { get; private set; }
    
    // Security and Fraud
    public string? PINVerificationResult { get; private set; }
    public bool IsSuspicious { get; private set; }
    public string? SuspiciousReason { get; private set; }
    public string? FraudScore { get; private set; }

    private ATMTransaction() : base(Guid.NewGuid()) { }

    public static ATMTransaction CreateWithdrawal(
        string atmId,
        string atmLocation,
        Guid cardId,
        Guid accountId,
        Guid customerId,
        string maskedCardNumber,
        Money amount,
        Money accountBalance,
        bool isOnUs = true)
    {
        var transaction = new ATMTransaction
        {
            Id = Guid.NewGuid(),
            ATMId = atmId,
            ATMLocation = atmLocation,
            CardId = cardId,
            AccountId = accountId,
            CustomerId = customerId,
            CardNumber = maskedCardNumber,
            TransactionType = ATMTransactionType.CashWithdrawal,
            Amount = amount,
            Status = ATMTransactionStatus.Initiated,
            TransactionDateTime = DateTime.UtcNow,
            ReferenceNumber = GenerateReferenceNumber(),
            AccountBalanceBefore = accountBalance,
            IsOnUs = isOnUs,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "ATM_SYSTEM"
        };

        transaction.AddDomainEvent(new ATMTransactionInitiatedDomainEvent(
            transaction.Id, cardId, accountId, ATMTransactionType.CashWithdrawal, amount));

        return transaction;
    }

    public static ATMTransaction CreateBalanceInquiry(
        string atmId,
        string atmLocation,
        Guid cardId,
        Guid accountId,
        Guid customerId,
        string maskedCardNumber,
        Money accountBalance,
        bool isOnUs = true)
    {
        var transaction = new ATMTransaction
        {
            Id = Guid.NewGuid(),
            ATMId = atmId,
            ATMLocation = atmLocation,
            CardId = cardId,
            AccountId = accountId,
            CustomerId = customerId,
            CardNumber = maskedCardNumber,
            TransactionType = ATMTransactionType.BalanceInquiry,
            Amount = Money.Zero(accountBalance.Currency),
            Status = ATMTransactionStatus.Initiated,
            TransactionDateTime = DateTime.UtcNow,
            ReferenceNumber = GenerateReferenceNumber(),
            AccountBalanceBefore = accountBalance,
            AccountBalanceAfter = accountBalance,
            IsOnUs = isOnUs,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "ATM_SYSTEM"
        };

        transaction.AddDomainEvent(new ATMTransactionInitiatedDomainEvent(
            transaction.Id, cardId, accountId, ATMTransactionType.BalanceInquiry, Money.Zero(accountBalance.Currency)));

        return transaction;
    }

    public void Authorize(string authorizationCode, Money newAccountBalance)
    {
        if (Status != ATMTransactionStatus.Initiated)
            throw new DomainException($"Cannot authorize transaction in {Status} status");

        Status = ATMTransactionStatus.Authorized;
        AuthorizationCode = authorizationCode;
        AccountBalanceAfter = newAccountBalance;
        ResponseCode = "00"; // Success
        ResponseMessage = "Transaction Approved";

        AddDomainEvent(new ATMTransactionAuthorizedDomainEvent(Id, CardId, AccountId, Amount));
    }

    public void Complete(string receiptNumber, bool receiptPrinted = true)
    {
        if (Status != ATMTransactionStatus.Authorized)
            throw new DomainException($"Cannot complete transaction in {Status} status");

        Status = ATMTransactionStatus.Completed;
        ReceiptNumber = receiptNumber;
        ReceiptPrinted = receiptPrinted;

        AddDomainEvent(new ATMTransactionCompletedDomainEvent(
            Id, CardId, AccountId, TransactionType, Amount, AccountBalanceAfter));
    }

    public void Decline(string responseCode, string responseMessage, string? failureReason = null)
    {
        if (Status == ATMTransactionStatus.Completed)
            throw new DomainException("Cannot decline a completed transaction");

        Status = ATMTransactionStatus.Declined;
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        FailureReason = failureReason;

        AddDomainEvent(new ATMTransactionDeclinedDomainEvent(
            Id, CardId, AccountId, responseCode, responseMessage));
    }

    public void Fail(string failureReason)
    {
        if (Status == ATMTransactionStatus.Completed)
            throw new DomainException("Cannot fail a completed transaction");

        Status = ATMTransactionStatus.Failed;
        FailureReason = failureReason;
        ResponseCode = "96"; // System Error
        ResponseMessage = "Transaction Failed";

        AddDomainEvent(new ATMTransactionFailedDomainEvent(Id, CardId, AccountId, failureReason));
    }

    public void Timeout()
    {
        if (Status == ATMTransactionStatus.Completed)
            throw new DomainException("Cannot timeout a completed transaction");

        Status = ATMTransactionStatus.Timeout;
        FailureReason = "Transaction timeout";
        ResponseCode = "68"; // Response received too late
        ResponseMessage = "Transaction Timeout";

        AddDomainEvent(new ATMTransactionTimeoutDomainEvent(Id, CardId, AccountId));
    }

    public void Reverse(string reversalReason, Guid reversalTransactionId)
    {
        if (Status != ATMTransactionStatus.Completed)
            throw new DomainException("Can only reverse completed transactions");

        if (IsReversed)
            throw new DomainException("Transaction is already reversed");

        IsReversed = true;
        ReversedDate = DateTime.UtcNow;
        ReversalReason = reversalReason;
        ReversalTransactionId = reversalTransactionId;

        AddDomainEvent(new ATMTransactionReversedDomainEvent(
            Id, CardId, AccountId, Amount, reversalReason));
    }

    public void SetPINVerificationResult(bool isValid)
    {
        PINVerificationResult = isValid ? "VALID" : "INVALID";
        
        if (!isValid)
        {
            AddDomainEvent(new ATMPINVerificationFailedDomainEvent(Id, CardId, CustomerId));
        }
    }

    public void MarkAsSuspicious(string reason, string fraudScore)
    {
        IsSuspicious = true;
        SuspiciousReason = reason;
        FraudScore = fraudScore;

        AddDomainEvent(new ATMTransactionMarkedSuspiciousDomainEvent(
            Id, CardId, AccountId, reason, fraudScore));
    }

    public void SetFees(Money? interchangeFee, Money? atmFee)
    {
        InterchangeFee = interchangeFee;
        ATMFee = atmFee;
    }

    public void IncrementRetryCount()
    {
        RetryCount++;
    }

    private static string GenerateReferenceNumber()
    {
        return $"ATM{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }
}

// Enums for ATM Transaction Management
public enum ATMTransactionType
{
    CashWithdrawal = 1,
    BalanceInquiry = 2,
    PINChange = 3,
    MiniStatement = 4,
    FastCash = 5,
    Transfer = 6
}

public enum ATMTransactionStatus
{
    Initiated = 1,
    Authorized = 2,
    Completed = 3,
    Declined = 4,
    Failed = 5,
    Timeout = 6,
    Reversed = 7
}