namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Strategy for treating/responding to identified risks
/// Aligned with ISO 31000 risk management framework
/// </summary>
public enum RiskTreatmentStrategy
{
    /// <summary>
    /// Accept - Accept the risk and take no further action
    /// </summary>
    Accept = 1,

    /// <summary>
    /// Mitigate - Implement controls to reduce likelihood or impact
    /// </summary>
    Mitigate = 2,

    /// <summary>
    /// Transfer - Transfer the risk to another party (e.g., insurance)
    /// </summary>
    Transfer = 3,

    /// <summary>
    /// Avoid - Eliminate the activity causing the risk
    /// </summary>
    Avoid = 4,

    /// <summary>
    /// Share - Share the risk with partners or stakeholders
    /// </summary>
    Share = 5
}
