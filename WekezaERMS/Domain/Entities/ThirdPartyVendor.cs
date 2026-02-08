using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Third-Party Vendor Entity - Represents external vendors and suppliers
/// Aligned with Riskonnect third-party risk management capabilities
/// </summary>
public class ThirdPartyVendor
{
    /// <summary>
    /// Unique identifier for the vendor
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Human-readable vendor ID (e.g., VEN-2024-001)
    /// </summary>
    public string VendorCode { get; private set; }

    /// <summary>
    /// Vendor name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Vendor description and business overview
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Current status of the vendor relationship
    /// </summary>
    public VendorStatus Status { get; private set; }

    /// <summary>
    /// Overall risk level of the vendor
    /// </summary>
    public VendorRiskLevel RiskLevel { get; private set; }

    /// <summary>
    /// Services provided by the vendor
    /// </summary>
    public string ServicesProvided { get; private set; }

    /// <summary>
    /// Primary contact person at vendor
    /// </summary>
    public string PrimaryContact { get; private set; }

    /// <summary>
    /// Contact email
    /// </summary>
    public string ContactEmail { get; private set; }

    /// <summary>
    /// Contact phone
    /// </summary>
    public string? ContactPhone { get; private set; }

    /// <summary>
    /// Vendor address
    /// </summary>
    public string Address { get; private set; }

    /// <summary>
    /// Country of operation
    /// </summary>
    public string Country { get; private set; }

    /// <summary>
    /// Date when vendor relationship started
    /// </summary>
    public DateTime ContractStartDate { get; private set; }

    /// <summary>
    /// Date when current contract ends
    /// </summary>
    public DateTime ContractEndDate { get; private set; }

    /// <summary>
    /// Annual contract value
    /// </summary>
    public decimal AnnualContractValue { get; private set; }

    /// <summary>
    /// Service Level Agreement (SLA) details
    /// </summary>
    public string? SLADetails { get; private set; }

    /// <summary>
    /// SLA compliance percentage
    /// </summary>
    public decimal? SLAComplianceRate { get; private set; }

    /// <summary>
    /// Date of last risk assessment
    /// </summary>
    public DateTime? LastRiskAssessmentDate { get; private set; }

    /// <summary>
    /// Date of next scheduled audit
    /// </summary>
    public DateTime? NextAuditDate { get; private set; }

    /// <summary>
    /// Business criticality (1-5, with 5 being most critical)
    /// </summary>
    public int BusinessCriticality { get; private set; }

    /// <summary>
    /// Whether vendor has access to sensitive data
    /// </summary>
    public bool HasDataAccess { get; private set; }

    /// <summary>
    /// Data security certification (e.g., ISO 27001, SOC 2)
    /// </summary>
    public string? SecurityCertifications { get; private set; }

    /// <summary>
    /// Vendor owner/manager in the organization
    /// </summary>
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// Department managing the vendor
    /// </summary>
    public string Department { get; private set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private ThirdPartyVendor() { }

    public static ThirdPartyVendor Create(
        string vendorCode,
        string name,
        string description,
        string servicesProvided,
        string primaryContact,
        string contactEmail,
        string address,
        string country,
        DateTime contractStartDate,
        DateTime contractEndDate,
        decimal annualContractValue,
        int businessCriticality,
        bool hasDataAccess,
        Guid ownerId,
        string department,
        Guid createdBy)
    {
        var vendor = new ThirdPartyVendor
        {
            Id = Guid.NewGuid(),
            VendorCode = vendorCode,
            Name = name,
            Description = description,
            Status = VendorStatus.UnderEvaluation,
            ServicesProvided = servicesProvided,
            PrimaryContact = primaryContact,
            ContactEmail = contactEmail,
            Address = address,
            Country = country,
            ContractStartDate = contractStartDate,
            ContractEndDate = contractEndDate,
            AnnualContractValue = annualContractValue,
            BusinessCriticality = businessCriticality,
            HasDataAccess = hasDataAccess,
            OwnerId = ownerId,
            Department = department,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        vendor.CalculateRiskLevel();
        return vendor;
    }

    public void UpdateStatus(VendorStatus newStatus, Guid updatedBy)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateSLACompliance(decimal complianceRate, Guid updatedBy)
    {
        SLAComplianceRate = complianceRate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Adjust risk level based on SLA compliance
        if (complianceRate < 80m)
        {
            RiskLevel = VendorRiskLevel.High;
        }
    }

    public void RecordRiskAssessment(VendorRiskLevel assessedRiskLevel, Guid assessedBy)
    {
        RiskLevel = assessedRiskLevel;
        LastRiskAssessmentDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = assessedBy;
    }

    public void ScheduleAudit(DateTime auditDate, Guid scheduledBy)
    {
        NextAuditDate = auditDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = scheduledBy;
    }

    public void UpdateSecurityCertifications(string certifications, Guid updatedBy)
    {
        SecurityCertifications = certifications;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void RenewContract(DateTime newEndDate, decimal newValue, Guid renewedBy)
    {
        ContractEndDate = newEndDate;
        AnnualContractValue = newValue;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = renewedBy;
    }

    private void CalculateRiskLevel()
    {
        // Simple risk calculation based on criticality and data access
        if (BusinessCriticality >= 4 && HasDataAccess)
        {
            RiskLevel = VendorRiskLevel.Critical;
        }
        else if (BusinessCriticality >= 3)
        {
            RiskLevel = VendorRiskLevel.High;
        }
        else if (BusinessCriticality >= 2)
        {
            RiskLevel = VendorRiskLevel.Medium;
        }
        else
        {
            RiskLevel = VendorRiskLevel.Low;
        }
    }
}
