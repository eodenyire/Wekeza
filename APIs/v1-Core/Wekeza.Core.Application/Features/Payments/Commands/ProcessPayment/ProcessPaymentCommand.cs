using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Payments.Commands.ProcessPayment;

/// <summary>
/// Process Payment Command - Handles all types of payment processing
/// Supports internal transfers, external payments, and bulk payments
/// </summary>
public record ProcessPaymentCommand : ICommand<ProcessPaymentResult>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    // Payment type and channel
    public PaymentType Type { get; init; }
    public PaymentChannel Channel { get; init; } = PaymentChannel.Internal;
    public PaymentPriority Priority { get; init; } = PaymentPriority.Normal;

    // Sender details
    public Guid? FromAccountId { get; init; }
    public string? FromAccountNumber { get; init; }

    // Recipient details (for internal transfers)
    public Guid? ToAccountId { get; init; }
    public string? ToAccountNumber { get; init; }

    // Beneficiary details (for external payments)
    public string? BeneficiaryName { get; init; }
    public string? BeneficiaryAccountNumber { get; init; }
    public string? BeneficiaryBank { get; init; }
    public string? BeneficiaryBankCode { get; init; }

    // Payment details
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string Description { get; init; } = string.Empty;
    public string? CustomerReference { get; init; }

    // Fee handling
    public FeeBearer FeeBearer { get; init; } = FeeBearer.Sender;

    // Scheduling
    public DateTime? ValueDate { get; init; }
    public bool ProcessImmediately { get; init; } = true;

    // Additional metadata
    public Dictionary<string, string> AdditionalData { get; init; } = new();
}

public record ProcessPaymentResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public Guid PaymentOrderId { get; init; }
    public string PaymentReference { get; init; } = string.Empty;
    public string? JournalNumber { get; init; }
    public string? ExternalReference { get; init; }
    public PaymentStatus Status { get; init; }
    public decimal? FeeAmount { get; init; }
    public bool RequiresApproval { get; init; }
    public Guid? WorkflowInstanceId { get; init; }
}