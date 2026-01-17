using FluentValidation;

namespace Wekeza.Core.Application.Features.Accounts.Commands.AddSignatory;

public class AddSignatoryValidator : AbstractValidator<AddSignatoryCommand>
{
    public AddSignatoryValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required.");

        RuleFor(x => x.SignatoryName)
            .NotEmpty()
            .WithMessage("Signatory name is required.")
            .MaximumLength(100)
            .WithMessage("Signatory name cannot exceed 100 characters.");

        RuleFor(x => x.IdNumber)
            .NotEmpty()
            .WithMessage("ID number is required.")
            .MaximumLength(50)
            .WithMessage("ID number cannot exceed 50 characters.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .Must(role => new[] { "Viewer", "Initiator", "Approver" }.Contains(role))
            .WithMessage("Role must be one of: Viewer, Initiator, Approver.");

        RuleFor(x => x.SignatureLimit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Signature limit cannot be negative.");
    }
}
