using FluentValidation;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CIF.Commands.CreateIndividualParty;

public class CreateIndividualPartyValidator : AbstractValidator<CreateIndividualPartyCommand>
{
    public CreateIndividualPartyValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("Last name contains invalid characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow.AddYears(-18)).WithMessage("Customer must be at least 18 years old")
            .GreaterThan(DateTime.UtcNow.AddYears(-120)).WithMessage("Invalid date of birth");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(g => new[] { "Male", "Female", "Other" }.Contains(g))
            .WithMessage("Gender must be Male, Female, or Other");

        RuleFor(x => x.Nationality)
            .NotEmpty().WithMessage("Nationality is required")
            .MaximumLength(50).WithMessage("Nationality cannot exceed 50 characters");

        RuleFor(x => x.PrimaryEmail)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.PrimaryPhone)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^254[0-9]{9}$").WithMessage("Phone number must be in format 254XXXXXXXXX");

        // Address validation
        RuleFor(x => x.PrimaryAddress)
            .NotNull().WithMessage("Primary address is required");

        When(x => x.PrimaryAddress != null, () =>
        {
            RuleFor(x => x.PrimaryAddress.AddressLine1)
                .NotEmpty().WithMessage("Address line 1 is required")
                .MaximumLength(200).WithMessage("Address line 1 cannot exceed 200 characters");

            RuleFor(x => x.PrimaryAddress.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.PrimaryAddress.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters");

            RuleFor(x => x.PrimaryAddress.PostalCode)
                .NotEmpty().WithMessage("Postal code is required")
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");
        });

        // Identification validation
        RuleFor(x => x.PrimaryIdentification)
            .NotNull().WithMessage("Primary identification is required");

        When(x => x.PrimaryIdentification != null, () =>
        {
            RuleFor(x => x.PrimaryIdentification.DocumentType)
                .NotEmpty().WithMessage("Document type is required")
                .Must(t => new[] { "Passport", "NationalID", "DrivingLicense" }.Contains(t))
                .WithMessage("Document type must be Passport, NationalID, or DrivingLicense");

            RuleFor(x => x.PrimaryIdentification.DocumentNumber)
                .NotEmpty().WithMessage("Document number is required")
                .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters");

            RuleFor(x => x.PrimaryIdentification.IssuingCountry)
                .NotEmpty().WithMessage("Issuing country is required");

            RuleFor(x => x.PrimaryIdentification.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Document must not be expired");
        });
    }
}
