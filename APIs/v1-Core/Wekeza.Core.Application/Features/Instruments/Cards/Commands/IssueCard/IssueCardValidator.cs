using FluentValidation;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.IssueCard;

public class IssueCardValidator : AbstractValidator<IssueCardCommand>
{
    public IssueCardValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required.");

        RuleFor(x => x.CardType)
            .NotEmpty()
            .WithMessage("Card type is required.")
            .Must(type => new[] { "Debit", "Credit", "Prepaid" }.Contains(type))
            .WithMessage("Card type must be one of: Debit, Credit, Prepaid.");

        RuleFor(x => x.DailyWithdrawalLimit)
            .GreaterThan(0)
            .WithMessage("Daily withdrawal limit must be greater than zero.")
            .LessThanOrEqualTo(200_000)
            .WithMessage("Daily withdrawal limit cannot exceed 200,000.");
    }
}
