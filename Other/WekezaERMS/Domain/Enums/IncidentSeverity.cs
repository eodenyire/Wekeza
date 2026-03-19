namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Severity levels for incidents
/// Aligned with Riskonnect incident management framework
/// </summary>
public enum IncidentSeverity
{
    /// <summary>
    /// Minor incident with minimal impact
    /// </summary>
    Minor = 1,

    /// <summary>
    /// Moderate incident with some business impact
    /// </summary>
    Moderate = 2,

    /// <summary>
    /// Major incident with significant business impact
    /// </summary>
    Major = 3,

    /// <summary>
    /// Critical incident requiring immediate attention
    /// </summary>
    Critical = 4,

    /// <summary>
    /// Catastrophic incident with severe business impact
    /// </summary>
    Catastrophic = 5
}
