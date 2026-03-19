using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// ESG Risk Entity - Represents Environmental, Social, and Governance risks
/// Aligned with Riskonnect ESG risk management capabilities
/// </summary>
public class ESGRisk
{
    /// <summary>
    /// Unique identifier for the ESG risk
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Human-readable ESG risk ID (e.g., ESG-2024-001)
    /// </summary>
    public string RiskCode { get; private set; }

    /// <summary>
    /// ESG risk title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Detailed description
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// ESG category (Environmental, Social, or Governance)
    /// </summary>
    public RiskCategory ESGCategory { get; private set; }

    /// <summary>
    /// Current status
    /// </summary>
    public RiskStatus Status { get; private set; }

    /// <summary>
    /// Risk likelihood
    /// </summary>
    public RiskLikelihood Likelihood { get; private set; }

    /// <summary>
    /// Risk impact
    /// </summary>
    public RiskImpact Impact { get; private set; }

    /// <summary>
    /// Risk score
    /// </summary>
    public int RiskScore { get; private set; }

    /// <summary>
    /// Risk level
    /// </summary>
    public RiskLevel RiskLevel { get; private set; }

    /// <summary>
    /// Environmental impact description (for Environmental risks)
    /// </summary>
    public string? EnvironmentalImpact { get; private set; }

    /// <summary>
    /// Carbon footprint impact in tons CO2e (for Environmental risks)
    /// </summary>
    public decimal? CarbonFootprintTons { get; private set; }

    /// <summary>
    /// Social impact description (for Social risks)
    /// </summary>
    public string? SocialImpact { get; private set; }

    /// <summary>
    /// Number of people affected (for Social risks)
    /// </summary>
    public int? PeopleAffected { get; private set; }

    /// <summary>
    /// Governance framework impacted (for Governance risks)
    /// </summary>
    public string? GovernanceFramework { get; private set; }

    /// <summary>
    /// Regulatory requirements related to this risk
    /// </summary>
    public string? RegulatoryRequirements { get; private set; }

    /// <summary>
    /// ESG reporting metrics
    /// </summary>
    public string? ESGMetrics { get; private set; }

    /// <summary>
    /// Sustainability goals alignment
    /// </summary>
    public string? SustainabilityGoals { get; private set; }

    /// <summary>
    /// Stakeholder impact assessment
    /// </summary>
    public string? StakeholderImpact { get; private set; }

    /// <summary>
    /// Mitigation strategies
    /// </summary>
    public string? MitigationStrategies { get; private set; }

    /// <summary>
    /// Target reduction percentage
    /// </summary>
    public decimal? TargetReduction { get; private set; }

    /// <summary>
    /// Current progress towards target (percentage)
    /// </summary>
    public decimal? CurrentProgress { get; private set; }

    /// <summary>
    /// Target completion date
    /// </summary>
    public DateTime? TargetDate { get; private set; }

    /// <summary>
    /// Risk owner
    /// </summary>
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// Department responsible
    /// </summary>
    public string Department { get; private set; }

    /// <summary>
    /// Date of last assessment
    /// </summary>
    public DateTime? LastAssessmentDate { get; private set; }

    /// <summary>
    /// Next review date
    /// </summary>
    public DateTime NextReviewDate { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private ESGRisk() { }

    public static ESGRisk Create(
        string riskCode,
        string title,
        string description,
        RiskCategory esgCategory,
        RiskLikelihood likelihood,
        RiskImpact impact,
        Guid ownerId,
        string department,
        Guid createdBy)
    {
        if (esgCategory != RiskCategory.Environmental && 
            esgCategory != RiskCategory.Social && 
            esgCategory != RiskCategory.Governance)
        {
            throw new ArgumentException("ESG Risk must be Environmental, Social, or Governance category");
        }

        var esgRisk = new ESGRisk
        {
            Id = Guid.NewGuid(),
            RiskCode = riskCode,
            Title = title,
            Description = description,
            ESGCategory = esgCategory,
            Status = RiskStatus.Identified,
            Likelihood = likelihood,
            Impact = impact,
            OwnerId = ownerId,
            Department = department,
            NextReviewDate = DateTime.UtcNow.AddMonths(6),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        esgRisk.CalculateRiskScore();
        return esgRisk;
    }

    public void UpdateEnvironmentalImpact(string impact, decimal carbonFootprintTons, Guid updatedBy)
    {
        EnvironmentalImpact = impact;
        CarbonFootprintTons = carbonFootprintTons;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateSocialImpact(string impact, int peopleAffected, Guid updatedBy)
    {
        SocialImpact = impact;
        PeopleAffected = peopleAffected;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateGovernanceFramework(string framework, Guid updatedBy)
    {
        GovernanceFramework = framework;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetTarget(decimal targetReduction, DateTime targetDate, Guid updatedBy)
    {
        TargetReduction = targetReduction;
        TargetDate = targetDate;
        CurrentProgress = 0;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateProgress(decimal progressPercentage, Guid updatedBy)
    {
        CurrentProgress = progressPercentage;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Assess(RiskLikelihood likelihood, RiskImpact impact, Guid assessedBy)
    {
        Likelihood = likelihood;
        Impact = impact;
        CalculateRiskScore();
        LastAssessmentDate = DateTime.UtcNow;
        Status = RiskStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = assessedBy;
    }

    private void CalculateRiskScore()
    {
        RiskScore = (int)Likelihood * (int)Impact;
        RiskLevel = DetermineRiskLevel(RiskScore);
    }

    private static RiskLevel DetermineRiskLevel(int score)
    {
        return score switch
        {
            <= 4 => RiskLevel.Low,
            <= 9 => RiskLevel.Medium,
            <= 15 => RiskLevel.High,
            <= 20 => RiskLevel.VeryHigh,
            _ => RiskLevel.Critical
        };
    }
}
