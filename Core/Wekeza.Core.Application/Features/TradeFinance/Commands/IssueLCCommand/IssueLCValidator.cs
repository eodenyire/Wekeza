using FluentValidation;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.TradeFinance.Commands.IssueLCCommand;

public class IssueLCValidator : AbstractValidator<IssueLCCommand>
{
    public IssueLCValidator()
    {
        RuleFor(x => x.LCNumber)
            .NotEmpty()
            .WithMessage("LC Number is required")
            .MaximumLength(50)
            .WithMessage("LC Number cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9\-/]+$")
            .WithMessage("LC Number can only contain uppercase letters, numbers, hyphens, and forward slashes");

        RuleFor(x => x.ApplicantId)
            .NotEmpty()
            .WithMessage("Applicant ID is required");

        RuleFor(x => x.BeneficiaryId)
            .NotEmpty()
            .WithMessage("Beneficiary ID is required");

        RuleFor(x => x.IssuingBankId)
            .NotEmpty()
            .WithMessage("Issuing Bank ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("LC Amount must be greater than zero")
            .LessThanOrEqualTo(100_000_000)
            .WithMessage("LC Amount cannot exceed 100 million");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO code")
            .Must(BeValidCurrency)
            .WithMessage("Currency must be a valid ISO 4217 code");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow.AddDays(1))
            .WithMessage("Expiry date must be at least 1 day in the future")
            .LessThanOrEqualTo(DateTime.UtcNow.AddYears(5))
            .WithMessage("Expiry date cannot be more than 5 years in the future");

        RuleFor(x => x.LastShipmentDate)
            .LessThanOrEqualTo(x => x.ExpiryDate)
            .When(x => x.LastShipmentDate.HasValue)
            .WithMessage("Last shipment date cannot be after expiry date");

        RuleFor(x => x.Terms)
            .MaximumLength(2000)
            .WithMessage("Terms cannot exceed 2000 characters");

        RuleFor(x => x.GoodsDescription)
            .NotEmpty()
            .WithMessage("Goods description is required")
            .MaximumLength(1000)
            .WithMessage("Goods description cannot exceed 1000 characters");

        RuleFor(x => x.ApplicantId)
            .NotEqual(x => x.BeneficiaryId)
            .WithMessage("Applicant and Beneficiary cannot be the same party");

        RuleForEach(x => x.DocumentRequirements)
            .SetValidator(new DocumentRequirementValidator());
    }

    private bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[]
        {
            "USD", "EUR", "GBP", "JPY", "CHF", "CAD", "AUD", "NZD",
            "KES", "UGX", "TZS", "RWF", "ETB", "ZAR", "NGN", "GHS"
        };
        
        return validCurrencies.Contains(currency.ToUpper());
    }
}

public class DocumentRequirementValidator : AbstractValidator<DocumentRequirement>
{
    public DocumentRequirementValidator()
    {
        RuleFor(x => x.DocumentType)
            .NotEmpty()
            .WithMessage("Document type is required")
            .MaximumLength(100)
            .WithMessage("Document type cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Document description cannot exceed 500 characters");

        RuleFor(x => x.Copies)
            .GreaterThan(0)
            .WithMessage("Number of copies must be greater than zero")
            .LessThanOrEqualTo(10)
            .WithMessage("Number of copies cannot exceed 10");
    }
}
