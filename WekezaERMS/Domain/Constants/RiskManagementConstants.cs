namespace WekezaERMS.Domain.Constants;

/// <summary>
/// Constants for risk scoring and classification
/// These values are based on industry standards and can be adjusted based on organizational requirements
/// </summary>
public static class RiskScoringConstants
{
    /// <summary>
    /// Maximum score for Low risk level
    /// </summary>
    public const int LowRiskThreshold = 4;

    /// <summary>
    /// Maximum score for Medium risk level
    /// </summary>
    public const int MediumRiskThreshold = 9;

    /// <summary>
    /// Maximum score for High risk level
    /// </summary>
    public const int HighRiskThreshold = 15;

    /// <summary>
    /// Maximum score for Very High risk level
    /// </summary>
    public const int VeryHighRiskThreshold = 20;

    // Scores above VeryHighRiskThreshold are considered Critical
}

/// <summary>
/// Constants for third-party vendor risk management
/// </summary>
public static class VendorRiskConstants
{
    /// <summary>
    /// Minimum business criticality for High risk classification
    /// </summary>
    public const int HighRiskCriticalityThreshold = 3;

    /// <summary>
    /// Minimum business criticality for Critical risk classification
    /// </summary>
    public const int CriticalRiskCriticalityThreshold = 4;

    /// <summary>
    /// Minimum business criticality for Medium risk classification
    /// </summary>
    public const int MediumRiskCriticalityThreshold = 2;

    /// <summary>
    /// SLA compliance rate below which vendor is considered high risk (percentage)
    /// </summary>
    public const decimal MinimumSLAComplianceRate = 80m;
}

/// <summary>
/// Constants for AI governance
/// </summary>
public static class AIGovernanceConstants
{
    /// <summary>
    /// Bias score threshold above which AI system is considered high risk
    /// </summary>
    public const decimal HighRiskBiasThreshold = 50m;
}

/// <summary>
/// Constants for business continuity planning
/// </summary>
public static class BusinessContinuityConstants
{
    /// <summary>
    /// Default months between business continuity tests
    /// </summary>
    public const int DefaultTestFrequencyMonths = 6;

    /// <summary>
    /// Default months between business continuity plan reviews
    /// </summary>
    public const int DefaultReviewFrequencyMonths = 12;
}

/// <summary>
/// Constants for insurance policy management
/// </summary>
public static class InsurancePolicyConstants
{
    /// <summary>
    /// Months before expiry to trigger policy review
    /// </summary>
    public const int ReviewLeadTimeMonths = 3;

    /// <summary>
    /// Days before expiry to consider policy as expiring soon
    /// </summary>
    public const int ExpiringSoonThresholdDays = 90;
}

/// <summary>
/// Constants for ESG risk management
/// </summary>
public static class ESGRiskConstants
{
    /// <summary>
    /// Default months between ESG risk reviews
    /// </summary>
    public const int DefaultReviewFrequencyMonths = 6;
}

/// <summary>
/// Constants for risk correlation analysis
/// </summary>
public static class RiskCorrelationConstants
{
    /// <summary>
    /// Minimum correlation strength value
    /// </summary>
    public const decimal MinimumCorrelationStrength = 0m;

    /// <summary>
    /// Maximum correlation strength value
    /// </summary>
    public const decimal MaximumCorrelationStrength = 100m;
}
