using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.CIF.Commands.CreateCorporateParty;

/// <summary>
/// Command to create a corporate party (business customer)
/// Similar to Finacle Corporate CIF creation
/// </summary>
[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record CreateCorporatePartyCommand : ICommand<string>
{
    // Company Information
    public string CompanyName { get; init; } = string.Empty;
    public string RegistrationNumber { get; init; } = string.Empty;
    public DateTime IncorporationDate { get; init; }
    public string CompanyType { get; init; } = string.Empty; // LLC, PLC, Partnership, etc.
    public string Industry { get; init; } = string.Empty;

    // Contact Information
    public string PrimaryEmail { get; init; } = string.Empty;
    public string PrimaryPhone { get; init; } = string.Empty;
    public string? Website { get; init; }

    // Registered Address
    public AddressDto RegisteredAddress { get; init; } = default!;

    // Directors/Authorized Signatories
    public List<DirectorDto> Directors { get; init; } = new();

    // Business Details
    public decimal? AnnualTurnover { get; init; }
    public int? NumberOfEmployees { get; init; }
    public string? TaxIdentificationNumber { get; init; }
}

public record DirectorDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string IdentificationNumber { get; init; } = string.Empty;
    public string Nationality { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty; // Director, Secretary, Shareholder
    public decimal? ShareholdingPercentage { get; init; }
}

public record AddressDto
{
    public string AddressType { get; init; } = "Registered";
    public string AddressLine1 { get; init; } = string.Empty;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public bool IsPrimary { get; init; } = true;
}
