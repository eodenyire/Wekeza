using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.CIF.Commands.CreateIndividualParty;

/// <summary>
/// Command to create an individual party (retail customer)
/// Similar to Finacle CIF creation
/// </summary>
[Authorize(UserRole.Teller, UserRole.RiskOfficer, UserRole.Administrator)]
public record CreateIndividualPartyCommand : ICommand<string>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    // Personal Information
    public string FirstName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string? MaritalStatus { get; init; }
    public string Nationality { get; init; } = string.Empty;

    // Contact Information
    public string PrimaryEmail { get; init; } = string.Empty;
    public string PrimaryPhone { get; init; } = string.Empty;
    public string? SecondaryPhone { get; init; }
    public string? PreferredLanguage { get; init; }

    // Address
    public AddressDto PrimaryAddress { get; init; } = default!;

    // Identification
    public IdentificationDto PrimaryIdentification { get; init; } = default!;

    // Preferences
    public bool OptInMarketing { get; init; }
    public bool OptInSMS { get; init; } = true;
    public bool OptInEmail { get; init; } = true;
}

public record AddressDto
{
    public string AddressType { get; init; } = "Residential";
    public string AddressLine1 { get; init; } = string.Empty;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public bool IsPrimary { get; init; } = true;
}

public record IdentificationDto
{
    public string DocumentType { get; init; } = string.Empty; // Passport, NationalID, DrivingLicense
    public string DocumentNumber { get; init; } = string.Empty;
    public string IssuingCountry { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public DateTime ExpiryDate { get; init; }
}
