namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Status of an insurance claim
/// Aligned with Riskonnect claims tracking capabilities
/// </summary>
public enum InsuranceClaimStatus
{
    /// <summary>
    /// Claim has been submitted
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// Claim is under review by insurer
    /// </summary>
    UnderReview = 2,

    /// <summary>
    /// Additional information requested
    /// </summary>
    InformationRequested = 3,

    /// <summary>
    /// Claim has been approved
    /// </summary>
    Approved = 4,

    /// <summary>
    /// Claim has been partially approved
    /// </summary>
    PartiallyApproved = 5,

    /// <summary>
    /// Claim has been denied
    /// </summary>
    Denied = 6,

    /// <summary>
    /// Payment has been received
    /// </summary>
    Settled = 7,

    /// <summary>
    /// Claim has been closed
    /// </summary>
    Closed = 8
}
