using FluentValidation;
using WekezaERMS.Application.Commands.Controls;

namespace WekezaERMS.Application.Validators;

public class RecordControlTestCommandValidator : AbstractValidator<RecordControlTestCommand>
{
    public RecordControlTestCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Control ID is required");

        RuleFor(x => x.TestData.Effectiveness)
            .IsInEnum().WithMessage("Invalid effectiveness value");

        RuleFor(x => x.TestData.TestingEvidence)
            .NotEmpty().WithMessage("Testing evidence is required")
            .MaximumLength(2000).WithMessage("Testing evidence cannot exceed 2000 characters");

        RuleFor(x => x.TestData.TestDate)
            .NotEmpty().WithMessage("Test date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Test date cannot be in the future");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("UpdatedBy is required");
    }
}
