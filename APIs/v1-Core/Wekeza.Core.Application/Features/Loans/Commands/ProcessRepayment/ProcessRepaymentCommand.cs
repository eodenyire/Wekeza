using MediatR;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Loans.Commands.ProcessRepayment;

/// <summary>
/// Process Loan Repayment Command - Loan payment processing
/// Processes loan repayments with automatic allocation and GL posting
/// </summary>
[Authorize(UserRole.Teller, UserRole.LoanOfficer, UserRole.Administrator)]
public record ProcessRepaymentCommand : IRequest<ProcessRepaymentResult>
{
    public Guid LoanId { get; init; }
    public Guid PaymentAccountId { get; init; }
    public decimal PaymentAmount { get; init; }
    public string Currency { get; init; } = "KES";
    public DateTime? PaymentDate { get; init; }
    public string? PaymentReference { get; init; }
    public string ProcessedBy { get; init; } = string.Empty;
    public string? Comments { get; init; }
}

public record ProcessRepaymentResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? LoanNumber { get; init; }
    public decimal? TotalPayment { get; init; }
    public decimal? PrincipalPayment { get; init; }
    public decimal? InterestPayment { get; init; }
    public decimal? RemainingBalance { get; init; }
    public DateTime? PaymentDate { get; init; }
    public string? JournalNumber { get; init; }
    public bool IsLoanPaidInFull { get; init; }
    public string? Message { get; init; }

    public static ProcessRepaymentResult Success(
        string loanNumber,
        decimal totalPayment,
        decimal principalPayment,
        decimal interestPayment,
        decimal remainingBalance,
        DateTime paymentDate,
        string? journalNumber = null,
        bool isLoanPaidInFull = false,
        string? message = null)
    {
        return new ProcessRepaymentResult
        {
            IsSuccess = true,
            LoanNumber = loanNumber,
            TotalPayment = totalPayment,
            PrincipalPayment = principalPayment,
            InterestPayment = interestPayment,
            RemainingBalance = remainingBalance,
            PaymentDate = paymentDate,
            JournalNumber = journalNumber,
            IsLoanPaidInFull = isLoanPaidInFull,
            Message = message ?? (isLoanPaidInFull ? "Loan paid in full" : "Repayment processed successfully")
        };
    }

    public static ProcessRepaymentResult Failed(string errorMessage)
    {
        return new ProcessRepaymentResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}