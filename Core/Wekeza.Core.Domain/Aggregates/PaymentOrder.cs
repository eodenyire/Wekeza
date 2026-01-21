using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Exceptions;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// PaymentOrder Aggregate - Core payment instruction
/// Inspired by Finacle Payment Hub and T24 Payment Order
/// Handles all types of payments: internal transfers, external payments, bulk payments
/// </summary>
public class PaymentOrder : AggregateRoot
{
    public string PaymentReference { get; private set; } // Unique payment reference
    public PaymentType Type { get; private set; }
    public PaymentChannel Channel { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentPriority Priority { get; private set; }
    
    // Payment parties
    public Guid? FromAccountId { get; private set; } // Null for incoming payments
    public Guid? ToAccountId { get; private set; } // Null for outgoing external payments
    public string? FromAccountNumber { get; private set; }
    public string? ToAccountNumber { get; private set; }
    public string? BeneficiaryName { get; private set; }
    public string? BeneficiaryBank { get; private set; }
    public string? BeneficiaryBankCode { get; private set; }
    
    // Payment details
    public Money Amount { get; private set; }
    public Money? ExchangeRate { get; private set; } // For cross-currency payments
    public Money? ConvertedAmount { get; private set; } // Amount in destination currency
    public string Description { get; private set; }
    public string? CustomerReference { get; private set; }
    public string? BankReference { get; private set; }
    
    // Fees and charges
    public Money? FeeAmount { get; private set; }
    public string? FeeGLCode { get; private set; }
    public FeeBearer FeeBearer { get; private set; }
    
    // Timing
    public DateTime RequestedDate { get; private set; }
    public DateTime? ValueDate { get; private set; }
    public DateTime? ProcessedDate { get; private set; }
    public DateTime? SettledDate { get; private set; }
    
    // Workflow and authorization
    public Guid? WorkflowInstanceId { get; private set; }
    public bool RequiresApproval { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    
    // External system integration
    public string? ExternalReference { get; private set; }
    public string? SwiftMessage { get; private set; }
    public string? GatewayResponse { get; private set; }
    
    // Audit trail
    public string CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }
    public string? FailureReason { get; private set; }
    public int RetryCount { get; private set; }

    private PaymentOrder() : base(Guid.NewGuid()) { }

    public static PaymentOrder CreateInternalTransfer(
        Guid fromAccountId,
        Guid toAccountId,
        Money amount,
        string description,
        string createdBy,
        string? customerReference = null,
        PaymentPriority priority = PaymentPriority.Normal)
    {
        var paymentReference = GeneratePaymentReference(PaymentType.InternalTransfer);
        
        var payment = new PaymentOrder
        {
            Id = Guid.NewGuid(),
            PaymentReference = paymentReference,
            Type = PaymentType.InternalTransfer,
            Channel = PaymentChannel.Internal,
            Status = PaymentStatus.Pending,
            Priority = priority,
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            Description = description,
            CustomerReference = customerReference,
            RequestedDate = DateTime.UtcNow,
            ValueDate = DateTime.UtcNow.Date,
            RequiresApproval = DetermineApprovalRequirement(amount, PaymentType.InternalTransfer),
            FeeBearer = FeeBearer.Sender,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            RetryCount = 0
        };

        payment.AddDomainEvent(new PaymentOrderCreatedDomainEvent(payment.Id, payment.PaymentReference, payment.Type));
        return payment;
    }

    public static PaymentOrder CreateExternalPayment(
        Guid fromAccountId,
        string toAccountNumber,
        string beneficiaryName,
        string beneficiaryBank,
        string beneficiaryBankCode,
        Money amount,
        string description,
        PaymentChannel channel,
        string createdBy,
        string? customerReference = null,
        PaymentPriority priority = PaymentPriority.Normal)
    {
        var paymentReference = GeneratePaymentReference(PaymentType.ExternalTransfer);
        
        var payment = new PaymentOrder
        {
            Id = Guid.NewGuid(),
            PaymentReference = paymentReference,
            Type = PaymentType.ExternalTransfer,
            Channel = channel,
            Status = PaymentStatus.Pending,
            Priority = priority,
            FromAccountId = fromAccountId,
            ToAccountNumber = toAccountNumber,
            BeneficiaryName = beneficiaryName,
            BeneficiaryBank = beneficiaryBank,
            BeneficiaryBankCode = beneficiaryBankCode,
            Amount = amount,
            Description = description,
            CustomerReference = customerReference,
            RequestedDate = DateTime.UtcNow,
            ValueDate = DateTime.UtcNow.Date,
            RequiresApproval = DetermineApprovalRequirement(amount, PaymentType.ExternalTransfer),
            FeeBearer = FeeBearer.Sender,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            RetryCount = 0
        };

        payment.AddDomainEvent(new PaymentOrderCreatedDomainEvent(payment.Id, payment.PaymentReference, payment.Type));
        return payment;
    }

    public void Authorize(string approvedBy)
    {
        if (Status != PaymentStatus.Pending)
            throw new GenericDomainException($"Cannot authorize payment in {Status} status");

        if (!RequiresApproval)
            throw new GenericDomainException("Payment does not require approval");

        Status = PaymentStatus.Authorized;
        ApprovedBy = approvedBy;
        ApprovedDate = DateTime.UtcNow;
        LastModifiedBy = approvedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentOrderAuthorizedDomainEvent(Id, PaymentReference, approvedBy));
    }

    public void Process(string processedBy)
    {
        if (RequiresApproval && Status != PaymentStatus.Authorized)
            throw new GenericDomainException("Payment must be authorized before processing");

        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Authorized)
            throw new GenericDomainException($"Cannot process payment in {Status} status");

        Status = PaymentStatus.Processing;
        ProcessedDate = DateTime.UtcNow;
        LastModifiedBy = processedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentOrderProcessingDomainEvent(Id, PaymentReference, Amount));
    }

    public void Complete(string completedBy, string? externalReference = null)
    {
        if (Status != PaymentStatus.Processing)
            throw new GenericDomainException($"Cannot complete payment in {Status} status");

        Status = PaymentStatus.Completed;
        SettledDate = DateTime.UtcNow;
        ExternalReference = externalReference;
        LastModifiedBy = completedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentOrderCompletedDomainEvent(Id, PaymentReference, Amount, SettledDate.Value));
    }

    public void Fail(string reason, string failedBy)
    {
        if (Status == PaymentStatus.Completed || Status == PaymentStatus.Cancelled)
            throw new GenericDomainException($"Cannot fail payment in {Status} status");

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        LastModifiedBy = failedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentOrderFailedDomainEvent(Id, PaymentReference, reason));
    }

    public void Cancel(string reason, string cancelledBy)
    {
        if (Status == PaymentStatus.Processing || Status == PaymentStatus.Completed)
            throw new GenericDomainException($"Cannot cancel payment in {Status} status");

        Status = PaymentStatus.Cancelled;
        FailureReason = reason;
        LastModifiedBy = cancelledBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentOrderCancelledDomainEvent(Id, PaymentReference, reason));
    }

    public void Retry(string retriedBy)
    {
        if (Status != PaymentStatus.Failed)
            throw new GenericDomainException("Can only retry failed payments");

        if (RetryCount >= 3)
            throw new GenericDomainException("Maximum retry attempts exceeded");

        Status = PaymentStatus.Pending;
        RetryCount++;
        FailureReason = null;
        LastModifiedBy = retriedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentOrderRetriedDomainEvent(Id, PaymentReference, RetryCount));
    }

    public void SetFee(Money feeAmount, string feeGLCode, FeeBearer feeBearer = FeeBearer.Sender)
    {
        FeeAmount = feeAmount;
        FeeGLCode = feeGLCode;
        FeeBearer = feeBearer;
    }

    public void SetExchangeRate(Money exchangeRate, Money convertedAmount)
    {
        ExchangeRate = exchangeRate;
        ConvertedAmount = convertedAmount;
    }

    public void SetWorkflowInstance(Guid workflowInstanceId)
    {
        WorkflowInstanceId = workflowInstanceId;
    }

    public void SetExternalReference(string externalReference)
    {
        ExternalReference = externalReference;
    }

    public void SetSwiftMessage(string swiftMessage)
    {
        SwiftMessage = swiftMessage;
    }

    public void SetGatewayResponse(string gatewayResponse)
    {
        GatewayResponse = gatewayResponse;
    }

    public bool IsHighValue()
    {
        // High value threshold - could be configurable
        return Amount.Amount >= 100000; // 100K threshold
    }

    public bool IsInternational()
    {
        return Channel == PaymentChannel.Swift || 
               Channel == PaymentChannel.Correspondent ||
               !string.IsNullOrEmpty(BeneficiaryBankCode);
    }

    public bool CanBeRetried()
    {
        return Status == PaymentStatus.Failed && RetryCount < 3;
    }

    private static string GeneratePaymentReference(PaymentType type)
    {
        var prefix = type switch
        {
            PaymentType.InternalTransfer => "INT",
            PaymentType.ExternalTransfer => "EXT",
            PaymentType.BulkPayment => "BLK",
            PaymentType.StandingOrder => "STO",
            _ => "PAY"
        };

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        
        return $"{prefix}{timestamp}{random}";
    }

    private static bool DetermineApprovalRequirement(Money amount, PaymentType type)
    {
        // Business rules for approval requirements
        return type switch
        {
            PaymentType.InternalTransfer => amount.Amount >= 50000, // 50K threshold
            PaymentType.ExternalTransfer => amount.Amount >= 10000, // 10K threshold
            PaymentType.BulkPayment => true, // Always require approval
            _ => false
        };
    }
}

