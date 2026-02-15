using FluentValidation;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.WithdrawFromAtm;

public class WithdrawFromAtmValidator : AbstractValidator<WithdrawFromAtmCommand>
{
    public WithdrawFromAtmValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty()
            .WithMessage("Card ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Withdrawal amount must be greater than zero.")
            .LessThanOrEqualTo(50_000)
            .WithMessage("Single ATM withdrawal cannot exceed 50,000.");

        RuleFor(x => x.TerminalId)
            .NotEmpty()
            .WithMessage("Terminal ID is required.")
            .MaximumLength(50)
            .WithMessage("Terminal ID cannot exceed 50 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3)
            .WithMessage("Currency must be a 3-letter code (e.g., KES, USD).");
    }
}
