using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Teller Transaction Aggregate - Complete teller transaction management
/// Inspired by Finacle and T24 teller transaction processing
/// Tracks all transactions processed through teller channels with full audit trail
/// </summary>
public class TellerTransaction : AggregateRoot
{
    // Transaction identification
    public string TransactionNumber { get; private set; } // Unique transaction identifier
    public string? ExternalReference { get; private set; } // Customer or system reference
    public Guid SessionId { get; private set; }
    public string SessionNumber { get; private set; }
    
    // Transaction details
    public TellerTransactionType TransactionType { get; private set; }
    public Money Amount { get; private set; }
    public string Currency { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public DateTime ValueDate { get; private set; }
    
    // Account information
    public Guid? AccountId { get; private set; }
    public string? AccountNumber { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    
    // Teller and branch information
    public Guid TellerId { get; private set; }
    public string TellerCode { get; private set; }
    public string TellerName { get; private set; }
    public Guid BranchId { get; private set; }
    public string BranchCode { get; private set; }
    
    // Transaction status and processing
    public TellerTransactionStatus Status { get; private set; }
    public DateTime ProcessedDate { get; private set; }
    public string? ReversalReason { get; private set; }
    public DateTime? ReversalDate { get; private set; }
    public string? ReversedBy { get; private set; }
    
    // Authorization and approval
    public bool RequiredSupervisorApproval { get; private set; }
    public string? SupervisorId { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApprovalComments { get; private set; }
    
    // GL Integration
    public string? JournalEntryId { get; private set; }
    public string? DebitGLCode { get; private set; }
    public string? CreditGLCode { get; private set; }
    
    // Customer verification
    public CustomerVerificationMethod VerificationMethod { get; private set; }
    public string? VerificationDetails { get; private set; }
    public bool CustomerPresent { get; private set; }
    
    // Transaction fees and charges
    public Money? FeeAmount { get; private set; }
    public string? FeeDescription { get; private set; }
    public string? FeeGLCode { get; private set; }
    
    // Multi-currency handling
    public decimal? ExchangeRate { get; private set; }
    public Money? EquivalentAmount { get; private set; }
    public string? BaseCurrency { get; private set; }
    
    // Transaction notes and comments
    public string? TransactionNotes { get; private set; }
    public string? CustomerComments { get; private set; }
    public string? InternalNotes { get; private set; }
    
    // Audit trail
    public string ProcessedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }

    private TellerTransaction() : base(Guid.NewGuid()) { }

    public static TellerTransaction Create(
        Guid sessionId,
        string sessionNumber,
        TellerTransactionType transactionType,
        Money amount,
        Guid tellerId,
        string tellerCode,
        string tellerName,
        Guid branchId,
        string branchCode,
        string processedBy,
        CustomerVerificationMethod verificationMethod,
        bool customerPresent,
        string? externalReference = null)
    {
        var transactionNumber = GenerateTransactionNumber(branchCode, tellerCode);
        
        var transaction = new TellerTransaction
        {
            Id = Guid.NewGuid(),
            TransactionNumber = transactionNumber,
            ExternalReference = externalReference,
            SessionId = sessionId,
            SessionNumber = sessionNumber,
            TransactionType = transactionType,
            Amount = amount,
            Currency = amount.Currency.Code,
            TransactionDate = DateTime.UtcNow,
            ValueDate = DateTime.UtcNow.Date,
            TellerId = tellerId,
            TellerCode = tellerCode,
            TellerName = tellerName,
            BranchId = branchId,
            BranchCode = branchCode,
            Status = TellerTransactionStatus.Pending,
            ProcessedDate = DateTime.UtcNow,
            VerificationMethod = verificationMethod,
            CustomerPresent = customerPresent,
            ProcessedBy = processedBy,
            CreatedDate = DateTime.UtcNow
        };

        transaction.AddDomainEvent(new TellerTransactionCreatedDomainEvent(
            transaction.Id, transactionNumber, transactionType, amount, tellerId, branchId));

        return transaction;
    }

    public void SetAccountDetails(Guid accountId, string accountNumber, Guid customerId, string customerName)
    {
        AccountId = accountId;
        AccountNumber = accountNumber;
        CustomerId = customerId;
        CustomerName = customerName;
    }

    public void SetVerificationDetails(string verificationDetails)
    {
        VerificationDetails = verificationDetails;
    }

    public void AddFee(Money feeAmount, string feeDescription, string feeGLCode)
    {
        FeeAmount = feeAmount;
        FeeDescription = feeDescription;
        FeeGLCode = feeGLCode;
    }

    public void SetExchangeRate(decimal exchangeRate, Money equivalentAmount, string baseCurrency)
    {
        ExchangeRate = exchangeRate;
        EquivalentAmount = equivalentAmount;
        BaseCurrency = baseCurrency;
    }

    public void SetGLCodes(string debitGLCode, string creditGLCode)
    {
        DebitGLCode = debitGLCode;
        CreditGLCode = creditGLCode;
    }

    public void SetJournalEntry(string journalEntryId)
    {
        JournalEntryId = journalEntryId;
    }

    public void AddNotes(string? transactionNotes, string? customerComments, string? internalNotes)
    {
        TransactionNotes = transactionNotes;
        CustomerComments = customerComments;
        InternalNotes = internalNotes;
    }

    public void RequestSupervisorApproval(string reason, string requestedBy)
    {
        if (Status != TellerTransactionStatus.Pending)
            throw new DomainException("Only pending transactions can request supervisor approval");

        RequiredSupervisorApproval = true;
        Status = TellerTransactionStatus.PendingApproval;
        InternalNotes = $"Supervisor approval requested: {reason}";
        
        LastModifiedBy = requestedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerTransactionApprovalRequestedDomainEvent(
            Id, TransactionNumber, reason, Amount));
    }

    public void ApproveBySupervisor(string supervisorId, string? comments, string approvedBy)
    {
        if (Status != TellerTransactionStatus.PendingApproval)
            throw new DomainException("Transaction is not pending supervisor approval");

        SupervisorId = supervisorId;
        ApprovalDate = DateTime.UtcNow;
        ApprovalComments = comments;
        Status = TellerTransactionStatus.Approved;
        
        LastModifiedBy = approvedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerTransactionApprovedDomainEvent(
            Id, TransactionNumber, supervisorId, comments));
    }

    public void RejectBySupervisor(string supervisorId, string rejectionReason, string rejectedBy)
    {
        if (Status != TellerTransactionStatus.PendingApproval)
            throw new DomainException("Transaction is not pending supervisor approval");

        SupervisorId = supervisorId;
        ApprovalComments = rejectionReason;
        Status = TellerTransactionStatus.Rejected;
        
        LastModifiedBy = rejectedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerTransactionRejectedDomainEvent(
            Id, TransactionNumber, supervisorId, rejectionReason));
    }

    public void Complete(string completedBy)
    {
        if (Status != TellerTransactionStatus.Pending && Status != TellerTransactionStatus.Approved)
            throw new DomainException("Only pending or approved transactions can be completed");

        Status = TellerTransactionStatus.Completed;
        LastModifiedBy = completedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerTransactionCompletedDomainEvent(
            Id, TransactionNumber, Amount, AccountNumber));
    }

    public void Reverse(string reversalReason, string reversedBy)
    {
        if (Status != TellerTransactionStatus.Completed)
            throw new DomainException("Only completed transactions can be reversed");

        Status = TellerTransactionStatus.Reversed;
        ReversalReason = reversalReason;
        ReversalDate = DateTime.UtcNow;
        ReversedBy = reversedBy;
        
        LastModifiedBy = reversedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerTransactionReversedDomainEvent(
            Id, TransactionNumber, reversalReason, Amount));
    }

    public void Fail(string failureReason, string failedBy)
    {
        if (Status == TellerTransactionStatus.Completed || Status == TellerTransactionStatus.Reversed)
            throw new DomainException("Cannot fail completed or reversed transactions");

        Status = TellerTransactionStatus.Failed;
        InternalNotes = $"Transaction failed: {failureReason}";
        
        LastModifiedBy = failedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerTransactionFailedDomainEvent(
            Id, TransactionNumber, failureReason));
    }

    public bool IsReversible()
    {
        return Status == TellerTransactionStatus.Completed && 
               TransactionDate.Date == DateTime.UtcNow.Date; // Same day reversal only
    }

    public bool RequiresApproval(Money approvalThreshold)
    {
        return Amount.IsGreaterThan(approvalThreshold);
    }

    public Money GetTotalAmount()
    {
        return FeeAmount != null ? Amount + FeeAmount : Amount;
    }

    private static string GenerateTransactionNumber(string branchCode, string tellerCode)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
        return $"TXN{branchCode}{tellerCode}{timestamp}";
    }
}

// Supporting enums
public enum TellerTransactionType
{
    CashDeposit,
    CashWithdrawal,
    AccountTransfer,
    ChequeDeposit,
    ChequeWithdrawal,
    CurrencyExchange,
    CashierCheque,
    DraftIssuance,
    BillPayment,
    LoanRepayment,
    FixedDepositOpening,
    FixedDepositClosure,
    AccountOpening,
    AccountClosure,
    CardIssuance,
    Other
}

public enum TellerTransactionStatus
{
    Pending,
    PendingApproval,
    Approved,
    Completed,
    Rejected,
    Failed,
    Reversed
}

public enum CustomerVerificationMethod
{
    None,
    IDCard,
    Passport,
    DrivingLicense,
    Biometric,
    PIN,
    Signature,
    KnownCustomer,
    ThirdPartyVerification
}

// Domain Events
public record TellerTransactionCreatedDomainEvent(
    Guid TransactionId,
    string TransactionNumber,
    TellerTransactionType TransactionType,
    Money Amount,
    Guid TellerId,
    Guid BranchId) : IDomainEvent;

public record TellerTransactionApprovalRequestedDomainEvent(
    Guid TransactionId,
    string TransactionNumber,
    string Reason,
    Money Amount) : IDomainEvent;

public record TellerTransactionApprovedDomainEvent(
    Guid TransactionId,
    string TransactionNumber,
    string SupervisorId,
    string? Comments) : IDomainEvent;

public record TellerTransactionRejectedDomainEvent(
    Guid TransactionId,
    string TransactionNumber,
    string SupervisorId,
    string RejectionReason) : IDomainEvent;

public record TellerTransactionCompletedDomainEvent(
    Guid TransactionId,
    string TransactionNumber,
    Money Amount,
    string? AccountNumber) : IDomainEvent;

public record TellerTransactionReversedDomainEvent(
    Guid TransactionId,
    string TransactionNumber,
    string ReversalReason,
    Money Amount) : IDomainEvent;

public record TellerTransactionFailedDomainEvent(
    Guid TransactionId,
    string TransactionNumber,
    string FailureReason) : IDomainEvent;