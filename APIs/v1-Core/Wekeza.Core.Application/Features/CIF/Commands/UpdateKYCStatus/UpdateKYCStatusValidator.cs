using FluentValidation;

namespace Wekeza.Core.Application.Features.CIF.Commands.UpdateKYCStatus;

public class UpdateKYCStatusValidator : AbstractValidator<UpdateKYCStatusCommand>
{
    public UpdateKYCStatusValidator()
    {
        RuleFor(x => x.PartyNumber)
            .NotEmpty().WithMessage("Party number is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid KYC status");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future")
            .When(x => x.ExpiryDate.HasValue);

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}
