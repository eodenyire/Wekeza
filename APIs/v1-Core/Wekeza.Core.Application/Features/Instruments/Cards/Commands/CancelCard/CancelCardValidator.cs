using FluentValidation;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.CancelCard;

public class CancelCardValidator : AbstractValidator<CancelCardCommand>
{
    public CancelCardValidator()
    {
        RuleFor(x => x.CardId)
            .NotEmpty()
            .WithMessage("Card ID is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required.")
            .MinimumLength(5)
            .WithMessage("Reason must be at least 5 characters.")
            .MaximumLength(200)
            .WithMessage("Reason cannot exceed 200 characters.");
    }
}
