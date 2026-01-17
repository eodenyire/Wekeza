using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Loans.Commands.ApproveLoan;

/// <summary>
/// Approve Loan Command - Manual loan approval
/// Used when loans require manual approval through workflow
/// Requires LoanOfficer or Administrator role
/// </summary>
[Authorize(UserRole.LoanOfficer, UserRole.Administrator)]
public record ApproveLoanCommand : IRequest<ApproveLoanResult>
{
    public Guid LoanId { get; init; }
    public string ApprovedBy { get; init; } = string.Empty;
    public DateTime? FirstPaymentDate { get; init; }
    public decimal? ApprovedAmount { get; init; } // Can approve for less than requested
    public decimal? ApprovedInterestRate { get; init; } // Can override product rate
    public int? ApprovedTermInMonths { get; init; } // Can modify term
    public List<LoanConditionDto>? Conditions { get; init; }
    public string? Comments { get; init; }
}

public record ApproveLoanResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? LoanNumber { get; init; }
    public decimal? ApprovedAmount { get; init; }
    public decimal? InterestRate { get; init; }
    public DateTime? FirstPaymentDate { get; init; }
    public DateTime? MaturityDate { get; init; }
    public string? Message { get; init; }

    public static ApproveLoanResult Success(
        string loanNumber,
        decimal approvedAmount,
        decimal interestRate,
        DateTime? firstPaymentDate,
        DateTime? maturityDate,
        string? message = null)
    {
        return new ApproveLoanResult
        {
            IsSuccess = true,
            LoanNumber = loanNumber,
            ApprovedAmount = approvedAmount,
            InterestRate = interestRate,
            FirstPaymentDate = firstPaymentDate,
            MaturityDate = maturityDate,
            Message = message ?? "Loan approved successfully"
        };
    }

    public static ApproveLoanResult Failed(string errorMessage)
    {
        return new ApproveLoanResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public record LoanConditionDto
{
    public string ConditionType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsMandatory { get; init; }
    public DateTime? DueDate { get; init; }
}
