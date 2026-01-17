using FluentValidation;

namespace Wekeza.Core.Application.Features.Accounts.Commands.DeactivateAccount;

public class DeactivateAccountValidator : AbstractValidator<DeactivateAccountCommand>
{
    public DeactivateAccountValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .WithMessage("Account number is required.")
            .Matches(@"^[0-9]{10,16}$")
            .WithMessage("Account number must be between 10 and 16 digits.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason for deactivation is required.")
            .MinimumLength(10)
            .WithMessage("Reason must be at least 10 characters.")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");
    }
}
