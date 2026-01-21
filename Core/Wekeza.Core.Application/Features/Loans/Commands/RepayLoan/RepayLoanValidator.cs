/*
 * TEMPORARILY COMMENTED OUT - FIXING COMPILATION ERRORS
 * This file will be restored and fixed incrementally
 * Missing RepayLoanCommand class
 */

/*
using FluentValidation;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Loans.Commands.RepayLoan;

public class RepayLoanValidator : AbstractValidator<RepayLoanCommand>
{
    public RepayLoanValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty()
            .WithMessage("Loan ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Repayment amount must be greater than zero.")
            .LessThanOrEqualTo(100_000_000)
            .WithMessage("Repayment amount cannot exceed 100,000,000.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required.")
            .Must(method => new[] { "Cash", "Transfer", "MobileMoney", "Cheque" }.Contains(method))
            .WithMessage("Payment method must be one of: Cash, Transfer, MobileMoney, Cheque.");
    }
}
*/
