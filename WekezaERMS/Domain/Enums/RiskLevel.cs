namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Overall risk level derived from the risk matrix
/// Determines required actions and escalation
/// </summary>
public enum RiskLevel
{
    /// <summary>
    /// Low Risk (Score 1-4) - Accept with monitoring
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium Risk (Score 5-9) - Accept with enhanced controls
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High Risk (Score 10-15) - Mitigate with action plans
    /// </summary>
    High = 3,

    /// <summary>
    /// Very High Risk (Score 16-20) - Immediate action required
    /// </summary>
    VeryHigh = 4,

    /// <summary>
    /// Critical Risk (Score 21-25) - Escalate to Board/Executive
    /// </summary>
    Critical = 5
}
