namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Status of AI system in its lifecycle
/// Aligned with Riskonnect AI governance capabilities
/// </summary>
public enum AISystemStatus
{
    /// <summary>
    /// AI system is in development
    /// </summary>
    Development = 1,

    /// <summary>
    /// AI system is in testing phase
    /// </summary>
    Testing = 2,

    /// <summary>
    /// AI system has been approved for deployment
    /// </summary>
    Approved = 3,

    /// <summary>
    /// AI system is deployed and operational
    /// </summary>
    Deployed = 4,

    /// <summary>
    /// AI system is under monitoring for issues
    /// </summary>
    Monitoring = 5,

    /// <summary>
    /// AI system is under review due to concerns
    /// </summary>
    UnderReview = 6,

    /// <summary>
    /// AI system has been suspended
    /// </summary>
    Suspended = 7,

    /// <summary>
    /// AI system has been decommissioned
    /// </summary>
    Decommissioned = 8
}
