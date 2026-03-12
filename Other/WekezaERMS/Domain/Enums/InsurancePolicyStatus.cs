namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Status of an insurance policy
/// Aligned with Riskonnect insurable risk management capabilities
/// </summary>
public enum InsurancePolicyStatus
{
    /// <summary>
    /// Policy is under review
    /// </summary>
    UnderReview = 1,

    /// <summary>
    /// Policy is active and in force
    /// </summary>
    Active = 2,

    /// <summary>
    /// Policy is pending renewal
    /// </summary>
    PendingRenewal = 3,

    /// <summary>
    /// Policy has been renewed
    /// </summary>
    Renewed = 4,

    /// <summary>
    /// Policy has expired
    /// </summary>
    Expired = 5,

    /// <summary>
    /// Policy has been cancelled
    /// </summary>
    Cancelled = 6
}
