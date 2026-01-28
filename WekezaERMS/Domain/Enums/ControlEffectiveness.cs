namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Effectiveness rating of risk controls
/// Used to assess control performance
/// </summary>
public enum ControlEffectiveness
{
    /// <summary>
    /// Ineffective - Control is not working as intended
    /// </summary>
    Ineffective = 1,

    /// <summary>
    /// Partially Effective - Control is working but with gaps
    /// </summary>
    PartiallyEffective = 2,

    /// <summary>
    /// Effective - Control is working as intended
    /// </summary>
    Effective = 3,

    /// <summary>
    /// Highly Effective - Control exceeds expectations
    /// </summary>
    HighlyEffective = 4
}
