using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// KYC Verification Aggregate - Know Your Customer verification records
/// Critical for regulatory compliance (AML/KYC regulations)
/// </summary>
public class KYCVerification : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public string VerificationType { get; private set; } // Initial, Enhanced, Periodic, Update
    public string Status { get; private set; } // Pending, Approved, Rejected, Expired
    public DateTime VerificationDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    
    // Verification Details
    public string DocumentType { get; private set; } // Passport, NationalID, DriversLicense, etc.
    public string DocumentNumber { get; private set; }
    public DateTime DocumentExpiryDate { get; private set; }
    public string IssuingCountry { get; private set; }
    
    // Verification Source
    public string VerificationSource { get; private set; } // Manual, Automated, API, Biometric
    public string VerificationMethod { get; private set; } // Document Scan, Face Recognition, etc.
    public decimal VerificationScore { get; private set; } // 0-100
    
    // Address Verification
    public bool AddressVerified { get; private set; }
    public string VerifiedAddress { get; private set; }
    public DateTime? AddressVerificationDate { get; private set; }
    
    // Screening Results
    public bool SanctionsScreeningPassed { get; private set; }
    public bool PEPScreeningPassed { get; private set; }
    public string ScreeningNotes { get; private set; }
    
    // Metadata
    public string VerifiedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    // Compatibility properties for service layer
    public string RiskLevel { get; private set; } = "Medium";
    public DateTime? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? ExpiresAt => ExpiryDate;

    private KYCVerification() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static KYCVerification Create(
        Guid customerId,
        string verificationType,
        string documentType,
        string documentNumber,
        string verificationSource,
        string verifiedBy)
    {
        var kyc = new KYCVerification
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            VerificationType = verificationType,
            Status = "Pending",
            VerificationDate = DateTime.UtcNow,
            DocumentType = documentType,
            DocumentNumber = documentNumber,
            VerificationSource = verificationSource,
            VerifiedBy = verifiedBy,
            CreatedAt = DateTime.UtcNow,
            VerificationScore = 0,
            AddressVerified = false,
            SanctionsScreeningPassed = false,
            PEPScreeningPassed = false,
            ScreeningNotes = string.Empty,
            VerifiedAddress = string.Empty,
            Metadata = new Dictionary<string, object>()
        };

        return kyc;
    }

    public void Approve(string approvalNotes, string approvedBy)
    {
        Status = "Approved";
        ApprovalNotes = approvalNotes;
        UpdatedAt = DateTime.UtcNow;
        VerifiedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedBy;
    }

    public void Reject(string reason, string rejectedBy)
    {
        Status = "Rejected";
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
        VerifiedBy = rejectedBy;
    }

    public void UpdateScreeningResults(bool sanctionsPassed, bool pepPassed, string notes)
    {
        SanctionsScreeningPassed = sanctionsPassed;
        PEPScreeningPassed = pepPassed;
        ScreeningNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyAddress(string address, string verifiedBy)
    {
        AddressVerified = true;
        VerifiedAddress = address;
        AddressVerificationDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        VerifiedBy = verifiedBy;
    }

    public bool IsFullyVerified => SanctionsScreeningPassed && PEPScreeningPassed && AddressVerified && Status == "Approved";
}
