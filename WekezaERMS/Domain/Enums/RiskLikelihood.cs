namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Likelihood scale for risk assessment (5-point scale)
/// Used in the risk matrix calculation
/// </summary>
public enum RiskLikelihood
{
    /// <summary>
    /// Rare - May occur only in exceptional circumstances (< 5% probability)
    /// </summary>
    Rare = 1,

    /// <summary>
    /// Unlikely - Could occur at some time (5-25% probability)
    /// </summary>
    Unlikely = 2,

    /// <summary>
    /// Possible - Might occur at some time (25-50% probability)
    /// </summary>
    Possible = 3,

    /// <summary>
    /// Likely - Will probably occur in most circumstances (50-75% probability)
    /// </summary>
    Likely = 4,

    /// <summary>
    /// Almost Certain - Expected to occur in most circumstances (> 75% probability)
    /// </summary>
    AlmostCertain = 5
}
