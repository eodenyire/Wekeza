namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Impact scale for risk assessment (5-point scale)
/// Used in the risk matrix calculation
/// </summary>
public enum RiskImpact
{
    /// <summary>
    /// Insignificant - Minimal financial impact (< $10K), no regulatory concern
    /// </summary>
    Insignificant = 1,

    /// <summary>
    /// Minor - Small financial impact ($10K-$100K), minor regulatory concern
    /// </summary>
    Minor = 2,

    /// <summary>
    /// Moderate - Moderate financial impact ($100K-$1M), some regulatory concern
    /// </summary>
    Moderate = 3,

    /// <summary>
    /// Major - Significant financial impact ($1M-$10M), major regulatory concern
    /// </summary>
    Major = 4,

    /// <summary>
    /// Catastrophic - Severe financial impact (> $10M), severe regulatory/reputational damage
    /// </summary>
    Catastrophic = 5
}
