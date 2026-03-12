namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Risk level classification for third-party vendors
/// Aligned with Riskonnect third-party risk management framework
/// </summary>
public enum VendorRiskLevel
{
    /// <summary>
    /// Low risk vendor with minimal business impact
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium risk vendor requiring standard monitoring
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High risk vendor requiring enhanced due diligence
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical vendor with significant business impact requiring continuous monitoring
    /// </summary>
    Critical = 4
}
