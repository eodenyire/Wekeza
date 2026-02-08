namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Risk level for AI systems
/// Aligned with Riskonnect AI governance capabilities
/// </summary>
public enum AIRiskLevel
{
    /// <summary>
    /// Minimal risk AI system
    /// </summary>
    Minimal = 1,

    /// <summary>
    /// Limited risk AI system
    /// </summary>
    Limited = 2,

    /// <summary>
    /// High risk AI system requiring oversight
    /// </summary>
    High = 3,

    /// <summary>
    /// Unacceptable risk AI system
    /// </summary>
    Unacceptable = 4
}
