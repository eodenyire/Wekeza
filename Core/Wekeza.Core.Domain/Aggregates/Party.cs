using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Party Aggregate - Represents any entity that can have a relationship with the bank
/// (Individual, Corporate, Government, Financial Institution)
/// Inspired by Finacle CIF and T24 CUSTOMER
/// </summary>
public class Party : AggregateRoot
{
    public string PartyNumber { get; private set; } // Unique party identifier (like CIF number)
    public PartyType PartyType { get; private set; }
    public PartyStatus Status { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }
    public string CreatedBy { get; private set; }
    public string? LastModifiedBy { get; private set; }

    // Individual Details
    public string? FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string? Gender { get; private set; }
    public string? MaritalStatus { get; private set; }
    public string? Nationality { get; private set; }

    // Corporate Details
    public string? CompanyName { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public DateTime? IncorporationDate { get; private set; }
    public string? CompanyType { get; private set; }
    public string? Industry { get; private set; }

    // Contact Information
    public string? PrimaryEmail { get; private set; }
    public string? PrimaryPhone { get; private set; }
    public string? SecondaryPhone { get; private set; }
    public string? PreferredLanguage { get; private set; }

    // Address Information
    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    // Identification Documents
    private readonly List<IdentificationDocument> _identifications = new();
    public IReadOnlyCollection<IdentificationDocument> Identifications => _identifications.AsReadOnly();

    // KYC & Risk
    public KYCStatus KYCStatus { get; private set; }
    public DateTime? KYCCompletedDate { get; private set; }
    public DateTime? KYCExpiryDate { get; private set; }
    public RiskRating RiskRating { get; private set; }
    public bool IsPEP { get; private set; } // Politically Exposed Person
    public bool IsSanctioned { get; private set; }

    // Relationships
    private readonly List<PartyRelationship> _relationships = new();
    public IReadOnlyCollection<PartyRelationship> Relationships => _relationships.AsReadOnly();

    // Segmentation
    public CustomerSegment Segment { get; private set; }
    public string? SubSegment { get; private set; }

    // Preferences
    public bool OptInMarketing { get; private set; }
    public bool OptInSMS { get; private set; }
    public bool OptInEmail { get; private set; }

    private Party() : base(Guid.NewGuid()) { }

    // Individual Party Constructor
    public static Party CreateIndividual(
        string partyNumber,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string nationality,
        string createdBy)
    {
        var party = new Party
        {
            Id = Guid.NewGuid(),
            PartyNumber = partyNumber,
            PartyType = PartyType.Individual,
            Status = PartyStatus.Active,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Nationality = nationality,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            KYCStatus = KYCStatus.Pending,
            RiskRating = RiskRating.Low,
            Segment = CustomerSegment.Retail,
            IsPEP = false,
            IsSanctioned = false
        };

        return party;
    }

    // Corporate Party Constructor
    public static Party CreateCorporate(
        string partyNumber,
        string companyName,
        string registrationNumber,
        DateTime incorporationDate,
        string industry,
        string createdBy)
    {
        var party = new Party
        {
            Id = Guid.NewGuid(),
            PartyNumber = partyNumber,
            PartyType = PartyType.Corporate,
            Status = PartyStatus.Active,
            CompanyName = companyName,
            RegistrationNumber = registrationNumber,
            IncorporationDate = incorporationDate,
            Industry = industry,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            KYCStatus = KYCStatus.Pending,
            RiskRating = RiskRating.Medium,
            Segment = CustomerSegment.SME,
            IsPEP = false,
            IsSanctioned = false
        };

        return party;
    }

    public void AddAddress(Address address)
    {
        _addresses.Add(address);
        LastModifiedDate = DateTime.UtcNow;
    }

    public void AddIdentification(IdentificationDocument document)
    {
        _identifications.Add(document);
        LastModifiedDate = DateTime.UtcNow;
    }

    public void UpdateKYCStatus(KYCStatus status, DateTime? expiryDate = null)
    {
        KYCStatus = status;
        if (status == KYCStatus.Completed)
        {
            KYCCompletedDate = DateTime.UtcNow;
            KYCExpiryDate = expiryDate ?? DateTime.UtcNow.AddYears(2);
        }
        LastModifiedDate = DateTime.UtcNow;
    }

    public void UpdateRiskRating(RiskRating rating)
    {
        RiskRating = rating;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void MarkAsPEP()
    {
        IsPEP = true;
        RiskRating = RiskRating.High; // PEPs are automatically high risk
        LastModifiedDate = DateTime.UtcNow;
    }

    public void MarkAsSanctioned()
    {
        IsSanctioned = true;
        Status = PartyStatus.Blocked;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void AddRelationship(PartyRelationship relationship)
    {
        _relationships.Add(relationship);
        LastModifiedDate = DateTime.UtcNow;
    }

    public void UpdateContactInfo(string email, string phone)
    {
        PrimaryEmail = email;
        PrimaryPhone = phone;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void UpdateSegment(CustomerSegment segment, string? subSegment = null)
    {
        Segment = segment;
        SubSegment = subSegment;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Deactivate(string reason)
    {
        Status = PartyStatus.Inactive;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Block(string reason)
    {
        Status = PartyStatus.Blocked;
        LastModifiedDate = DateTime.UtcNow;
    }
}

// Value Objects and Supporting Classes
public record Address(
    string AddressType, // Residential, Office, Mailing
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string Country,
    string PostalCode,
    bool IsPrimary
);

public record IdentificationDocument(
    string DocumentType, // Passport, National ID, Driving License
    string DocumentNumber,
    string IssuingCountry,
    DateTime IssueDate,
    DateTime ExpiryDate,
    bool IsVerified
);

public record PartyRelationship(
    Guid RelatedPartyId,
    string RelationshipType, // Parent, Subsidiary, Guarantor, Beneficiary, Director
    DateTime EffectiveDate,
    DateTime? EndDate,
    string? Notes
);
