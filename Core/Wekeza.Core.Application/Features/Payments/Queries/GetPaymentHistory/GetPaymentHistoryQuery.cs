using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Payments.Queries.GetPaymentHistory;

public record GetPaymentHistoryQuery : IQuery<GetPaymentHistoryResult>
{
    public Guid? AccountId { get; init; }
    public string? AccountNumber { get; init; }
    public Guid? CustomerId { get; init; }
    public PaymentType? Type { get; init; }
    public PaymentStatus? Status { get; init; }
    public PaymentChannel? Channel { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageSize { get; init; } = 50;
    public int PageNumber { get; init; } = 1;
    public string? SearchTerm { get; init; }
}

public record GetPaymentHistoryResult
{
    public IEnumerable<PaymentHistoryDto> Payments { get; init; } = new List<PaymentHistoryDto>();
    public int TotalCount { get; init; }
    public int PageSize { get; init; }
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public decimal TotalAmount { get; init; }
}

public record PaymentHistoryDto
{
    public Guid Id { get; init; }
    public string PaymentReference { get; init; } = string.Empty;
    public PaymentType Type { get; init; }
    public PaymentChannel Channel { get; init; }
    public PaymentStatus Status { get; init; }
    public PaymentPriority Priority { get; init; }
    
    // Account details
    public string? FromAccountNumber { get; init; }
    public string? ToAccountNumber { get; init; }
    
    // Beneficiary details
    public string? BeneficiaryName { get; init; }
    public string? BeneficiaryBank { get; init; }
    
    // Payment details
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? CustomerReference { get; init; }
    public string? ExternalReference { get; init; }
    
    // Fees
    public decimal? FeeAmount { get; init; }
    public FeeBearer FeeBearer { get; init; }
    
    // Dates
    public DateTime RequestedDate { get; init; }
    public DateTime? ValueDate { get; init; }
    public DateTime? ProcessedDate { get; init; }
    public DateTime? SettledDate { get; init; }
    
    // Workflow
    public bool RequiresApproval { get; init; }
    public string? ApprovedBy { get; init; }
    public DateTime? ApprovedDate { get; init; }
    
    // Status details
    public string? FailureReason { get; init; }
    public int RetryCount { get; init; }
    
    // Audit
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
}