using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Loans.Commands.ApplyForLoan;

/// <summary>
/// Apply for Loan Command - Loan origination request
/// Initiates the loan application process with credit assessment
/// </summary>
public record ApplyForLoanCommand : IRequest<ApplyForLoanResult>
{
    public Guid CustomerId { get; init; }
    public Guid ProductId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public int TermInMonths { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public DateTime? PreferredDisbursementDate { get; init; }
    public List<LoanCollateralDto>? Collaterals { get; init; }
    public List<LoanGuarantorDto>? Guarantors { get; init; }
}

public record ApplyForLoanResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public Guid? LoanId { get; init; }
    public string? LoanNumber { get; init; }
    public decimal? CreditScore { get; init; }
    public string? RiskGrade { get; init; }
    public decimal? RecommendedInterestRate { get; init; }
    public bool IsAutoApproved { get; init; }
    public string? NextSteps { get; init; }

    public static ApplyForLoanResult Success(
        Guid loanId, 
        string loanNumber, 
        decimal? creditScore = null,
        string? riskGrade = null,
        decimal? recommendedRate = null,
        bool isAutoApproved = false,
        string? nextSteps = null)
    {
        return new ApplyForLoanResult
        {
            IsSuccess = true,
            LoanId = loanId,
            LoanNumber = loanNumber,
            CreditScore = creditScore,
            RiskGrade = riskGrade,
            RecommendedInterestRate = recommendedRate,
            IsAutoApproved = isAutoApproved,
            NextSteps = nextSteps ?? (isAutoApproved ? "Loan approved - ready for disbursement" : "Loan application submitted for review")
        };
    }

    public static ApplyForLoanResult Failed(string errorMessage)
    {
        return new ApplyForLoanResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public record LoanCollateralDto
{
    public string CollateralType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Currency { get; init; } = "KES";
    public DateTime ValuationDate { get; init; }
    public string? ValuedBy { get; init; }
}

public record LoanGuarantorDto
{
    public Guid GuarantorId { get; init; }
    public string GuarantorName { get; init; } = string.Empty;
    public decimal GuaranteeAmount { get; init; }
    public string Currency { get; init; } = "KES";
    public string? GuaranteeDocument { get; init; }
}