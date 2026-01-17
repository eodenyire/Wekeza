using FluentValidation;

namespace Wekeza.Core.Application.Features.Accounts.Commands.OpenProductBasedAccount;

public class OpenProductBasedAccountValidator : AbstractValidator<OpenProductBasedAccountCommand>
{
    public OpenProductBasedAccountValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a valid 3-letter code");

        RuleFor(x => x.InitialDeposit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Initial deposit cannot be negative");

        RuleFor(x => x.AccountAlias)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.AccountAlias))
            .WithMessage("Account alias cannot exceed 100 characters");
    }
}