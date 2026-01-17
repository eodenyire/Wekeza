using FluentValidation;

namespace Wekeza.Core.Application.Features.CIF.Commands.CreateCorporateParty;

public class CreateCorporatePartyValidator : AbstractValidator<CreateCorporatePartyCommand>
{
    public CreateCorporatePartyValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters");

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty().WithMessage("Registration number is required")
            .MaximumLength(50).WithMessage("Registration number must not exceed 50 characters");

        RuleFor(x => x.IncorporationDate)
            .NotEmpty().WithMessage("Incorporation date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Incorporation date cannot be in the future");

        RuleFor(x => x.CompanyType)
            .NotEmpty().WithMessage("Company type is required")
            .Must(BeValidCompanyType).WithMessage("Invalid company type");

        RuleFor(x => x.Industry)
            .NotEmpty().WithMessage("Industry is required")
            .MaximumLength(100).WithMessage("Industry must not exceed 100 characters");

        RuleFor(x => x.PrimaryEmail)
            .NotEmpty().WithMessage("Primary email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PrimaryPhone)
            .NotEmpty().WithMessage("Primary phone is required")
            .Matches(@"^254\d{9}$").WithMessage("Phone must be in format 254XXXXXXXXX");

        RuleFor(x => x.RegisteredAddress)
            .NotNull().WithMessage("Registered address is required");

        RuleFor(x => x.RegisteredAddress.AddressLine1)
            .NotEmpty().WithMessage("Address line 1 is required")
            .When(x => x.RegisteredAddress != null);

        RuleFor(x => x.RegisteredAddress.City)
            .NotEmpty().WithMessage("City is required")
            .When(x => x.RegisteredAddress != null);

        RuleFor(x => x.RegisteredAddress.Country)
            .NotEmpty().WithMessage("Country is required")
            .When(x => x.RegisteredAddress != null);

        RuleFor(x => x.Directors)
            .NotEmpty().WithMessage("At least one director is required")
            .Must(x => x.Count >= 1).WithMessage("At least one director is required");

        RuleForEach(x => x.Directors).ChildRules(director =>
        {
            director.RuleFor(d => d.FirstName)
                .NotEmpty().WithMessage("Director first name is required");

            director.RuleFor(d => d.LastName)
                .NotEmpty().WithMessage("Director last name is required");

            director.RuleFor(d => d.IdentificationNumber)
                .NotEmpty().WithMessage("Director identification number is required");

            director.RuleFor(d => d.Nationality)
                .NotEmpty().WithMessage("Director nationality is required");

            director.RuleFor(d => d.Role)
                .NotEmpty().WithMessage("Director role is required")
                .Must(BeValidDirectorRole).WithMessage("Invalid director role");
        });

        RuleFor(x => x.AnnualTurnover)
            .GreaterThanOrEqualTo(0).WithMessage("Annual turnover must be positive")
            .When(x => x.AnnualTurnover.HasValue);

        RuleFor(x => x.NumberOfEmployees)
            .GreaterThanOrEqualTo(0).WithMessage("Number of employees must be positive")
            .When(x => x.NumberOfEmployees.HasValue);
    }

    private bool BeValidCompanyType(string companyType)
    {
        var validTypes = new[] { "LLC", "PLC", "Partnership", "Sole Proprietorship", "NGO", "Trust", "Cooperative" };
        return validTypes.Contains(companyType, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeValidDirectorRole(string role)
    {
        var validRoles = new[] { "Director", "Managing Director", "Secretary", "Shareholder", "Authorized Signatory" };
        return validRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
