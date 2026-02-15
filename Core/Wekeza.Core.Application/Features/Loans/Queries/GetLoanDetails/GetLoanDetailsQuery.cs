using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Loans.Queries.GetLoanDetails;

/// <summary>
/// Get Loan Details Query - Comprehensive loan information
/// Returns complete loan details including schedule, payments, and status
/// </summary>
public record GetLoanDetailsQuery : IRequest<LoanDetailsDto?>
{
    public Guid? LoanId { get; init; }
    public string? LoanNumber { get; init; }
    public bool IncludeSchedule { get; init; } = true;
    public bool IncludeCollaterals { get; init; } = true;
    public bool IncludeGuarantors { get; init; } = true;
}

public record LoanDetailsDto
{
    public Guid Id { get; init; }
    public string LoanNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    
    // Loan amounts and terms
    public decimal PrincipalAmount { get; init; }
    public decimal OutstandingPrincipal { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal InterestRate { get; init; }
    public int TermInMonths { get; init; }
    public DateTime? FirstPaymentDate { get; init; }
    public DateTime? MaturityDate { get; init; }
    
    // Loan status
    public string Status { get; init; } = string.Empty;
    public string SubStatus { get; init; } = string.Empty;
    public DateTime ApplicationDate { get; init; }
    public DateTime? ApprovalDate { get; init; }
    public DateTime? DisbursementDate { get; init; }
    public DateTime? ClosureDate { get; init; }
    
    // Credit assessment
    public decimal? CreditScore { get; init; }
    public string? RiskGrade { get; init; }
    public decimal? RiskPremium { get; init; }
    
    // Interest and payments
    public decimal AccruedInterest { get; init; }
    public decimal TotalInterestPaid { get; init; }
    public decimal TotalAmountPaid { get; init; }
    public DateTime? LastPaymentDate { get; init; }
    public int DaysPastDue { get; init; }
    public decimal PastDueAmount { get; init; }
    
    // Provisioning
    public decimal ProvisionRate { get; init; }
    public decimal ProvisionAmount { get; init; }
    
    // Disbursement account
    public Guid? DisbursementAccountId { get; init; }
    public string? DisbursementAccountNumber { get; init; }
    
    // Audit information
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
    public string? ApprovedBy { get; init; }
    public string? DisbursedBy { get; init; }
    
    // Related data
    public List<LoanScheduleItemDto>? Schedule { get; init; }
    public List<LoanCollateralDto>? Collaterals { get; init; }
    public List<LoanGuarantorDto>? Guarantors { get; init; }
    public List<LoanConditionDto>? Conditions { get; init; }
}

public record LoanScheduleItemDto
{
    public int ScheduleNumber { get; init; }
    public DateTime DueDate { get; init; }
    public decimal PrincipalAmount { get; init; }
    public decimal InterestAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal OutstandingBalance { get; init; }
    public bool IsPaid { get; init; }
    public DateTime? PaidDate { get; init; }
    public decimal PaidAmount { get; init; }
}

public record LoanCollateralDto
{
    public Guid CollateralId { get; init; }
    public string CollateralType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime ValuationDate { get; init; }
    public string? ValuedBy { get; init; }
}

public record LoanGuarantorDto
{
    public Guid GuarantorId { get; init; }
    public string GuarantorName { get; init; } = string.Empty;
    public decimal GuaranteeAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime GuaranteeDate { get; init; }
    public string? GuaranteeDocument { get; init; }
}

public record LoanConditionDto
{
    public string ConditionType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsMandatory { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsComplied { get; init; }
}
