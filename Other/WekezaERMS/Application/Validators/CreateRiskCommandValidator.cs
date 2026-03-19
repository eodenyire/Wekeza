using FluentValidation;
using WekezaERMS.Application.Commands.Risks;

namespace WekezaERMS.Application.Validators;

public class CreateRiskCommandValidator : AbstractValidator<CreateRiskCommand>
{
    public CreateRiskCommandValidator()
    {
        RuleFor(x => x.RiskData.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.RiskData.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.RiskData.Department)
            .NotEmpty().WithMessage("Department is required")
            .MaximumLength(100).WithMessage("Department cannot exceed 100 characters");

        RuleFor(x => x.RiskData.Category)
            .IsInEnum().WithMessage("Invalid risk category");

        RuleFor(x => x.RiskData.InherentLikelihood)
            .IsInEnum().WithMessage("Invalid likelihood value");

        RuleFor(x => x.RiskData.InherentImpact)
            .IsInEnum().WithMessage("Invalid impact value");

        RuleFor(x => x.RiskData.TreatmentStrategy)
            .IsInEnum().WithMessage("Invalid treatment strategy");

        RuleFor(x => x.RiskData.OwnerId)
            .NotEmpty().WithMessage("Risk owner is required");

        RuleFor(x => x.RiskData.RiskAppetite)
            .InclusiveBetween(1, 25).WithMessage("Risk appetite must be between 1 and 25");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required");
    }
}
