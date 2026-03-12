using FluentValidation;
using WekezaERMS.Application.Commands.Controls;

namespace WekezaERMS.Application.Validators;

public class UpdateControlEffectivenessCommandValidator : AbstractValidator<UpdateControlEffectivenessCommand>
{
    public UpdateControlEffectivenessCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Control ID is required");

        RuleFor(x => x.EffectivenessData.Effectiveness)
            .IsInEnum().WithMessage("Invalid effectiveness value");

        RuleFor(x => x.EffectivenessData.TestingEvidence)
            .NotEmpty().WithMessage("Testing evidence is required")
            .MaximumLength(2000).WithMessage("Testing evidence cannot exceed 2000 characters");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("UpdatedBy is required");
    }
}
