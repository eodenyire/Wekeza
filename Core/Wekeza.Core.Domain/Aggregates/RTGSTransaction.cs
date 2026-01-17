using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// RTGS (Real-Time Gross Settlement) Transaction aggregate
/// Handles high-value, time-critical interbank transfers
/// </summary>
public class RTGSTransaction : AggregateRoot<Guid>
{
    public string RTGSReference { get; private set; }
    public string CustomerReference { get; private set; }
    public Guid DebitAccountId { get; private set; }
    public string CreditBankCode { get; private set; }
    public string CreditAccountNumber { get; private set; }
    public string CreditAccountName { get; private set; }
    public Money Amount { get; private set; }
    public Money Charges { get; private set; }
    public string Purpose { get; private set; }
    public RTGSTransactionType TransactionType { get; private set; }
    public RTGSStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public DateTime ValueDate { get; private set; }
    public DateTime InitiatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? SettledAt { get; private set; }
    public string InitiatedBy { get; private set; }
    public string? ProcessedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? CentralBankReference { get; private set; }
    public string? SettlementReference { get; private set; }
    public string BranchCode { get; private set; }
    public bool IsUrgent { get; private set; }
    public DateTime? CutoffTime { get; private set; }

    // Navigation properties
    public virtual Account DebitAccount { get; private set; } = null!;

    private RTGSTransaction() { } // EF Core

    public RTGSTransaction(
        Guid id,
        string rtgsReference,
        string customerReference,
        Guid debitAccountId,
        string creditBankCode,
        string creditAccountNumber,
        string creditAccountName,
        Money amount,
        Money charges,
        string purpose,
        RTGSTransactionType transactionType,
        Priority priority,
        DateTime valueDate,
        string initiatedBy,
        string branchCode,
        bool isUrgent = false)
    {
        Id = id;
        RTGSReference = rtgsReference;
        CustomerReference = customerReference;
        DebitAccountId = debitAccountId;
        CreditBankCode = creditBankCode;
        CreditAccountNumber = creditAccountNumber;
        CreditAccountName = creditAccountName;
        Amount = amount;
        Charges = charges;
        Purpose = purpose;
        TransactionType = transactionType;
        Priority = priority;
        ValueDate = valueDate;
        InitiatedBy = initiatedBy;
        BranchCode = branchCode;
        IsUrgent = isUrgent;
        Status = RTGSStatus.Initiated;
        InitiatedAt = DateTime.UtcNow;

        // Set cutoff time based on urgency
        SetCutoffTime();

        AddDomainEvent(new RTGSTransactionInitiatedDomainEvent(Id, RTGSReference, Amount, CreditBankCode));
    }

    private void SetCutoffTime()
    {
        var today = DateTime.Today;
        CutoffTime = IsUrgent ? today.AddHours(15) : today.AddHours(14); // 3 PM for urgent, 2 PM for normal
    }

    public void ValidateTransaction()
    {
        if (Status != RTGSStatus.Initiated)
            throw new InvalidOperationException("Transaction is not in initiated state");

        var validationErrors = new List<string>();

        // Amount validation
        if (Amount.Amount < 1000000) // Minimum 1M for RTGS
            validationErrors.Add("Amount below RTGS minimum threshold");

        if (Amount.Amount > 1000000000) // Maximum 1B
            validationErrors.Add("Amount exceeds RTGS maximum threshold");

        // Bank code validation
        if (string.IsNullOrEmpty(CreditBankCode) || CreditBankCode.Length != 3)
            validationErrors.Add("Invalid credit bank code");

        // Account number validation
        if (string.IsNullOrEmpty(CreditAccountNumber) || CreditAccountNumber.Length < 10)
            validationErrors.Add("Invalid credit account number");

        // Cutoff time validation
        if (DateTime.UtcNow > CutoffTime)
            validationErrors.Add("Transaction submitted after cutoff time");

        // Value date validation
        if (ValueDate < DateTime.Today)
            validationErrors.Add("Value date cannot be in the past");

        if (validationErrors.Any())
        {
            Status = RTGSStatus.ValidationFailed;
            RejectionReason = string.Join("; ", validationErrors);
            AddDomainEvent(new RTGSTransactionValidationFailedDomainEvent(Id, RTGSReference, RejectionReason));
        }
        else
        {
            Status = RTGSStatus.Validated;
            AddDomainEvent(new RTGSTransactionValidatedDomainEvent(Id, RTGSReference));
        }
    }

    public void ProcessTransaction(string processedBy)
    {
        if (Status != RTGSStatus.Validated)
            throw new InvalidOperationException("Transaction must be validated before processing");

        Status = RTGSStatus.Processing;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new RTGSTransactionProcessingDomainEvent(Id, RTGSReference, processedBy));
    }

    public void SendToCentralBank(string centralBankReference)
    {
        if (Status != RTGSStatus.Processing)
            throw new InvalidOperationException("Transaction must be in processing state");

        Status = RTGSStatus.SentToCentralBank;
        CentralBankReference = centralBankReference;

        AddDomainEvent(new RTGSTransactionSentToCentralBankDomainEvent(Id, RTGSReference, centralBankReference));
    }

    public void SettleTransaction(string settlementReference)
    {
        if (Status != RTGSStatus.SentToCentralBank)
            throw new InvalidOperationException("Transaction must be sent to central bank before settlement");

        Status = RTGSStatus.Settled;
        SettlementReference = settlementReference;
        SettledAt = DateTime.UtcNow;

        AddDomainEvent(new RTGSTransactionSettledDomainEvent(Id, RTGSReference, settlementReference, SettledAt.Value));
    }

    public void RejectTransaction(string rejectionReason, string rejectedBy)
    {
        if (Status == RTGSStatus.Settled)
            throw new InvalidOperationException("Cannot reject settled transaction");

        Status = RTGSStatus.Rejected;
        RejectionReason = rejectionReason;
        ProcessedBy = rejectedBy;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new RTGSTransactionRejectedDomainEvent(Id, RTGSReference, rejectionReason, rejectedBy));
    }

    public void TimeoutTransaction()
    {
        if (Status == RTGSStatus.Settled || Status == RTGSStatus.Rejected)
            return;

        Status = RTGSStatus.Timeout;
        RejectionReason = "Transaction timed out";
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new RTGSTransactionTimeoutDomainEvent(Id, RTGSReference));
    }

    public void ReturnTransaction(string returnReason)
    {
        if (Status != RTGSStatus.SentToCentralBank)
            throw new InvalidOperationException("Only transactions sent to central bank can be returned");

        Status = RTGSStatus.Returned;
        RejectionReason = returnReason;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new RTGSTransactionReturnedDomainEvent(Id, RTGSReference, returnReason));
    }

    public bool IsWithinCutoffTime()
    {
        return CutoffTime.HasValue && DateTime.UtcNow <= CutoffTime.Value;
    }

    public TimeSpan GetProcessingDuration()
    {
        if (ProcessedAt.HasValue)
            return ProcessedAt.Value - InitiatedAt;
        
        return DateTime.UtcNow - InitiatedAt;
    }

    public TimeSpan? GetSettlementDuration()
    {
        if (SettledAt.HasValue && ProcessedAt.HasValue)
            return SettledAt.Value - ProcessedAt.Value;
        
        return null;
    }

    public bool RequiresManualIntervention()
    {
        return Status == RTGSStatus.ValidationFailed || 
               Status == RTGSStatus.Rejected || 
               Status == RTGSStatus.Timeout ||
               Status == RTGSStatus.Returned;
    }

    public Money GetTotalDebitAmount()
    {
        return new Money(Amount.Amount + Charges.Amount, Amount.Currency);
    }
}

// Enums
public enum RTGSTransactionType
{
    CustomerTransfer = 1,
    InterbankTransfer = 2,
    GovernmentPayment = 3,
    CorporatePayment = 4,
    UrgentPayment = 5,
    LiquidityTransfer = 6
}

public enum RTGSStatus
{
    Initiated = 1,
    Validated = 2,
    ValidationFailed = 3,
    Processing = 4,
    SentToCentralBank = 5,
    Settled = 6,
    Rejected = 7,
    Timeout = 8,
    Returned = 9
}

public enum Priority
{
    Normal = 1,
    High = 2,
    Urgent = 3,
    Critical = 4
}

// Domain Events
public record RTGSTransactionInitiatedDomainEvent(
    Guid TransactionId,
    string RTGSReference,
    Money Amount,
    string CreditBankCode) : IDomainEvent;

public record RTGSTransactionValidatedDomainEvent(
    Guid TransactionId,
    string RTGSReference) : IDomainEvent;

public record RTGSTransactionValidationFailedDomainEvent(
    Guid TransactionId,
    string RTGSReference,
    string ValidationErrors) : IDomainEvent;

public record RTGSTransactionProcessingDomainEvent(
    Guid TransactionId,
    string RTGSReference,
    string ProcessedBy) : IDomainEvent;

public record RTGSTransactionSentToCentralBankDomainEvent(
    Guid TransactionId,
    string RTGSReference,
    string CentralBankReference) : IDomainEvent;

public record RTGSTransactionSettledDomainEvent(
    Guid TransactionId,
    string RTGSReference,
    string SettlementReference,
    DateTime SettledAt) : IDomainEvent;

public record RTGSTransactionRejectedDomainEvent(
    Guid TransactionId,
    string RTGSReference,
    string RejectionReason,
    string RejectedBy) : IDomainEvent;

public record RTGSTransactionTimeoutDomainEvent(
    Guid TransactionId,
    string RTGSReference) : IDomainEvent;

public record RTGSTransactionReturnedDomainEvent(
    Guid TransactionId,
    string RTGSReference,
    string ReturnReason) : IDomainEvent;