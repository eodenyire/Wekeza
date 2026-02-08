using FluentValidation;
using WekezaERMS.Application.Commands.Controls;

namespace WekezaERMS.Application.Validators;

public class UpdateControlCommandValidator : AbstractValidator<UpdateControlCommand>
{
    public UpdateControlCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Control ID is required");

        RuleFor(x => x.ControlData.ControlName)
            .NotEmpty().WithMessage("Control name is required")
            .MaximumLength(200).WithMessage("Control name cannot exceed 200 characters");

        RuleFor(x => x.ControlData.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.ControlData.ControlType)
            .NotEmpty().WithMessage("Control type is required")
            .Must(type => new[] { "Preventive", "Detective", "Corrective" }.Contains(type))
            .WithMessage("Control type must be Preventive, Detective, or Corrective");

        RuleFor(x => x.ControlData.TestingFrequency)
            .NotEmpty().WithMessage("Testing frequency is required")
            .Must(freq => new[] { "Monthly", "Quarterly", "Annually" }.Contains(freq))
            .WithMessage("Testing frequency must be Monthly, Quarterly, or Annually");

        RuleFor(x => x.ControlData.OwnerId)
            .NotEmpty().WithMessage("Control owner is required");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("UpdatedBy is required");
    }
}
