using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Loans.Commands.RegisterLoanProduct;

/// <summary>
/// Command to register a new loan product in the system
/// Supports various loan types with flexible configuration
/// </summary>
[Authorize(UserRole.Administrator, UserRole.LoanOfficer, UserRole.BranchManager)]
public record RegisterLoanProductCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    // Basic Product Information
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty; // Personal, Business, Mortgage, etc.
    public bool IsActive { get; init; } = true;
    
    // Interest Configuration
    public decimal BaseInterestRate { get; init; }
    public string InterestType { get; init; } = "REDUCING"; // REDUCING, FLAT, COMPOUND
    public string InterestCalculationMethod { get; init; } = "DAILY"; // DAILY, MONTHLY, ANNUAL
    public decimal? MinInterestRate { get; init; }
    public decimal? MaxInterestRate { get; init; }
    
    // Loan Limits
    public decimal MinLoanAmount { get; init; }
    public decimal MaxLoanAmount { get; init; }
    public int MinTermMonths { get; init; }
    public int MaxTermMonths { get; init; }
    
    // Fees Configuration
    public List<LoanFeeConfigDto> Fees { get; init; } = new();
    
    // Eligibility Criteria
    public List<EligibilityCriteriaDto> EligibilityCriteria { get; init; } = new();
    
    // Collateral Requirements
    public bool RequiresCollateral { get; init; } = false;
    public decimal? CollateralPercentage { get; init; }
    public List<string> AcceptedCollateralTypes { get; init; } = new();
    
    // Guarantor Requirements
    public bool RequiresGuarantor { get; init; } = false;
    public int? MinGuarantors { get; init; }
    public int? MaxGuarantors { get; init; }
    
    // Approval Configuration
    public bool AutoApprovalEnabled { get; init; } = false;
    public decimal? AutoApprovalLimit { get; init; }
    public List<AutoApprovalCriteriaDto> AutoApprovalCriteria { get; init; } = new();
    
    // Repayment Configuration
    public List<string> AllowedRepaymentFrequencies { get; init; } = new(); // MONTHLY, WEEKLY, DAILY
    public int GracePeriodDays { get; init; } = 0;
    public bool AllowPrepayment { get; init; } = true;
    public decimal? PrepaymentPenaltyRate { get; init; }
    
    // Risk Configuration
    public string RiskCategory { get; init; } = "MEDIUM";
    public decimal ProvisionRate { get; init; } = 0.01m;
    public int MaxDaysInArrears { get; init; } = 90;
    
    // GL Configuration
    public string LoanGLCode { get; init; } = string.Empty;
    public string InterestIncomeGLCode { get; init; } = string.Empty;
    public string ProvisionGLCode { get; init; } = string.Empty;
    public string FeesIncomeGLCode { get; init; } = string.Empty;
}

public record LoanFeeConfigDto
{
    public string FeeType { get; init; } = string.Empty; // PROCESSING, APPRAISAL, INSURANCE, etc.
    public string FeeName { get; init; } = string.Empty;
    public string CalculationType { get; init; } = string.Empty; // PERCENTAGE, FIXED
    public decimal FeeAmount { get; init; }
    public decimal? MinFee { get; init; }
    public decimal? MaxFee { get; init; }
    public bool IsWaivable { get; init; } = false;
    public string GLCode { get; init; } = string.Empty;
}

public record EligibilityCriteriaDto
{
    public string CriteriaType { get; init; } = string.Empty; // AGE, INCOME, EMPLOYMENT, CREDIT_SCORE
    public string Operator { get; init; } = string.Empty; // GREATER_THAN, LESS_THAN, EQUALS, BETWEEN
    public string Value { get; init; } = string.Empty;
    public bool IsRequired { get; init; } = true;
    public string Description { get; init; } = string.Empty;
}

public record AutoApprovalCriteriaDto
{
    public string CriteriaType { get; init; } = string.Empty;
    public string Condition { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public int Weight { get; init; } = 1;
}