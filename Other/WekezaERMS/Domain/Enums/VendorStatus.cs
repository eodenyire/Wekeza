namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Status of vendor relationship
/// Aligned with Riskonnect third-party risk management framework
/// </summary>
public enum VendorStatus
{
    /// <summary>
    /// Vendor is under evaluation
    /// </summary>
    UnderEvaluation = 1,

    /// <summary>
    /// Vendor has been approved
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Vendor is active and providing services
    /// </summary>
    Active = 3,

    /// <summary>
    /// Vendor is under review due to performance or risk concerns
    /// </summary>
    UnderReview = 4,

    /// <summary>
    /// Vendor has been suspended temporarily
    /// </summary>
    Suspended = 5,

    /// <summary>
    /// Vendor relationship has been terminated
    /// </summary>
    Terminated = 6
}
