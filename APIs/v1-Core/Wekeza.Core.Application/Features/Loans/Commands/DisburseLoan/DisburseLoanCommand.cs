using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Loans.Commands.DisburseLoan;

/// <summary>
/// Disburse Loan Command - Loan fund disbursement
/// Transfers approved loan funds to customer account with GL posting
/// Requires LoanOfficer or Administrator role
/// </summary>
[Authorize(UserRole.LoanOfficer, UserRole.Administrator)]
public record DisburseLoanCommand : IRequest<DisburseLoanResult>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid LoanId { get; init; }
    public Guid DisbursementAccountId { get; init; }
    public DateTime? DisbursementDate { get; init; }
    public string DisbursedBy { get; init; } = string.Empty;
    public string? Comments { get; init; }
}

public record DisburseLoanResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? LoanNumber { get; init; }
    public decimal? DisbursedAmount { get; init; }
    public string? DisbursementAccountNumber { get; init; }
    public DateTime? DisbursementDate { get; init; }
    public string? JournalNumber { get; init; }
    public string? Message { get; init; }

    public static DisburseLoanResult Success(
        string loanNumber,
        decimal disbursedAmount,
        string disbursementAccountNumber,
        DateTime disbursementDate,
        string? journalNumber = null,
        string? message = null)
    {
        return new DisburseLoanResult
        {
            IsSuccess = true,
            LoanNumber = loanNumber,
            DisbursedAmount = disbursedAmount,
            DisbursementAccountNumber = disbursementAccountNumber,
            DisbursementDate = disbursementDate,
            JournalNumber = journalNumber,
            Message = message ?? "Loan disbursed successfully"
        };
    }

    public static DisburseLoanResult Failed(string errorMessage)
    {
        return new DisburseLoanResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
