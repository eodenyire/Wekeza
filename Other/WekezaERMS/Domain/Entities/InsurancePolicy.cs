using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Insurance Policy Entity - Represents insurance coverage for insurable risks
/// Aligned with Riskonnect insurable risk management capabilities
/// </summary>
public class InsurancePolicy
{
    /// <summary>
    /// Unique identifier for the policy
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Policy number from insurance provider
    /// </summary>
    public string PolicyNumber { get; private set; }

    /// <summary>
    /// Policy name or title
    /// </summary>
    public string PolicyName { get; private set; }

    /// <summary>
    /// Insurance provider/carrier name
    /// </summary>
    public string InsuranceProvider { get; private set; }

    /// <summary>
    /// Type of coverage (e.g., Property, Liability, Cyber, Professional Indemnity)
    /// </summary>
    public string CoverageType { get; private set; }

    /// <summary>
    /// Policy status
    /// </summary>
    public InsurancePolicyStatus Status { get; private set; }

    /// <summary>
    /// Policy effective date
    /// </summary>
    public DateTime EffectiveDate { get; private set; }

    /// <summary>
    /// Policy expiration date
    /// </summary>
    public DateTime ExpirationDate { get; private set; }

    /// <summary>
    /// Coverage limit amount
    /// </summary>
    public decimal CoverageLimit { get; private set; }

    /// <summary>
    /// Deductible amount
    /// </summary>
    public decimal Deductible { get; private set; }

    /// <summary>
    /// Annual premium amount
    /// </summary>
    public decimal AnnualPremium { get; private set; }

    /// <summary>
    /// Currency of the policy
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Risks covered by this policy
    /// </summary>
    public string CoveredRisks { get; private set; }

    /// <summary>
    /// Policy exclusions
    /// </summary>
    public string? Exclusions { get; private set; }

    /// <summary>
    /// Department or business unit covered
    /// </summary>
    public string CoveredDepartment { get; private set; }

    /// <summary>
    /// Policy owner/administrator
    /// </summary>
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// Broker contact information
    /// </summary>
    public string? BrokerContact { get; private set; }

    /// <summary>
    /// Claims made under this policy
    /// </summary>
    public List<InsuranceClaim> Claims { get; private set; }

    /// <summary>
    /// Total claims amount filed
    /// </summary>
    public decimal TotalClaimsAmount { get; private set; }

    /// <summary>
    /// Total claims paid
    /// </summary>
    public decimal TotalClaimsPaid { get; private set; }

    /// <summary>
    /// Notes and additional information
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Date of last review
    /// </summary>
    public DateTime? LastReviewDate { get; private set; }

    /// <summary>
    /// Date of next review
    /// </summary>
    public DateTime NextReviewDate { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private InsurancePolicy() 
    {
        Claims = new List<InsuranceClaim>();
    }

    public static InsurancePolicy Create(
        string policyNumber,
        string policyName,
        string insuranceProvider,
        string coverageType,
        DateTime effectiveDate,
        DateTime expirationDate,
        decimal coverageLimit,
        decimal deductible,
        decimal annualPremium,
        string currency,
        string coveredRisks,
        string coveredDepartment,
        Guid ownerId,
        Guid createdBy)
    {
        return new InsurancePolicy
        {
            Id = Guid.NewGuid(),
            PolicyNumber = policyNumber,
            PolicyName = policyName,
            InsuranceProvider = insuranceProvider,
            CoverageType = coverageType,
            Status = InsurancePolicyStatus.Active,
            EffectiveDate = effectiveDate,
            ExpirationDate = expirationDate,
            CoverageLimit = coverageLimit,
            Deductible = deductible,
            AnnualPremium = annualPremium,
            Currency = currency,
            CoveredRisks = coveredRisks,
            CoveredDepartment = coveredDepartment,
            OwnerId = ownerId,
            NextReviewDate = expirationDate.AddMonths(-3), // Review 3 months before expiry
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AddClaim(InsuranceClaim claim)
    {
        Claims.Add(claim);
        TotalClaimsAmount += claim.ClaimAmount;
    }

    public void RecordClaimPayment(Guid claimId, decimal paidAmount)
    {
        var claim = Claims.FirstOrDefault(c => c.Id == claimId);
        if (claim != null)
        {
            TotalClaimsPaid += paidAmount;
        }
    }

    public void Renew(DateTime newExpirationDate, decimal newPremium, Guid renewedBy)
    {
        Status = InsurancePolicyStatus.Renewed;
        ExpirationDate = newExpirationDate;
        AnnualPremium = newPremium;
        NextReviewDate = newExpirationDate.AddMonths(-3);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = renewedBy;
    }

    public void Review(Guid reviewedBy)
    {
        LastReviewDate = DateTime.UtcNow;
        NextReviewDate = ExpirationDate.AddMonths(-3);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = reviewedBy;
    }

    public void Cancel(string reason, Guid cancelledBy)
    {
        Status = InsurancePolicyStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = cancelledBy;
    }

    public decimal CalculateCoverageGap(decimal totalRiskExposure)
    {
        return Math.Max(0, totalRiskExposure - CoverageLimit);
    }

    public bool IsExpiringSoon(int daysThreshold = 90)
    {
        return (ExpirationDate - DateTime.UtcNow).TotalDays <= daysThreshold;
    }
}
