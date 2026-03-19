namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Status of an incident through its lifecycle
/// Aligned with Riskonnect incident management workflow
/// </summary>
public enum IncidentStatus
{
    /// <summary>
    /// Incident has been reported but not yet assigned
    /// </summary>
    Reported = 1,

    /// <summary>
    /// Incident is assigned and under investigation
    /// </summary>
    Investigating = 2,

    /// <summary>
    /// Root cause has been identified
    /// </summary>
    RootCauseIdentified = 3,

    /// <summary>
    /// Remediation actions are in progress
    /// </summary>
    Remediating = 4,

    /// <summary>
    /// Remediation is complete, awaiting verification
    /// </summary>
    PendingVerification = 5,

    /// <summary>
    /// Incident has been resolved
    /// </summary>
    Resolved = 6,

    /// <summary>
    /// Incident is closed with lessons learned documented
    /// </summary>
    Closed = 7
}
