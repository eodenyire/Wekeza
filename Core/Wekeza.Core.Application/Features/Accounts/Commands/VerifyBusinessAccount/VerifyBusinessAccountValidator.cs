using FluentValidation;

namespace Wekeza.Core.Application.Features.Accounts.Commands.VerifyBusinessAccount;

public class VerifyBusinessAccountValidator : AbstractValidator<VerifyBusinessAccountCommand>
{
    public VerifyBusinessAccountValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required.");

        RuleFor(x => x.VerifiedBy)
            .NotEmpty()
            .WithMessage("Verifier name is required.")
            .MaximumLength(100)
            .WithMessage("Verifier name cannot exceed 100 characters.");

        RuleFor(x => x.DailyLimit)
            .GreaterThan(0)
            .WithMessage("Daily limit must be greater than zero.")
            .LessThanOrEqualTo(10_000_000)
            .WithMessage("Daily limit cannot exceed 10,000,000.");
    }
}
