using FluentValidation;

namespace Wekeza.Core.Application.Features.Loans.Commands.DisburseLoan;

public class DisburseLoanValidator : AbstractValidator<DisburseLoanCommand>
{
    public DisburseLoanValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty()
            .WithMessage("Loan ID is required.");

        RuleFor(x => x.DisbursedBy)
            .NotEmpty()
            .WithMessage("Disbursed by is required.")
            .MaximumLength(100)
            .WithMessage("Disbursed by cannot exceed 100 characters.");
    }
}
