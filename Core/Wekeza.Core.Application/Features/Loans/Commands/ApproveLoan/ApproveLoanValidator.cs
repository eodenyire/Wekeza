using FluentValidation;

namespace Wekeza.Core.Application.Features.Loans.Commands.ApproveLoan;

public class ApproveLoanValidator : AbstractValidator<ApproveLoanCommand>
{
    public ApproveLoanValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty()
            .WithMessage("Loan ID is required.");

        RuleFor(x => x.ApprovedBy)
            .NotEmpty()
            .WithMessage("Approver name is required.")
            .MaximumLength(100)
            .WithMessage("Approver name cannot exceed 100 characters.");
    }
}
