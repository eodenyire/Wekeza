using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Incident Entity - Represents a risk incident or event
/// Aligned with Riskonnect incident management capabilities
/// </summary>
public class Incident
{
    /// <summary>
    /// Unique identifier for the incident
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Human-readable incident ID (e.g., INC-2024-001)
    /// </summary>
    public string IncidentCode { get; private set; }

    /// <summary>
    /// Incident title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Detailed description of the incident
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Severity of the incident
    /// </summary>
    public IncidentSeverity Severity { get; private set; }

    /// <summary>
    /// Current status of the incident
    /// </summary>
    public IncidentStatus Status { get; private set; }

    /// <summary>
    /// Risk category associated with this incident
    /// </summary>
    public RiskCategory Category { get; private set; }

    /// <summary>
    /// Related risk ID if this incident is linked to a known risk
    /// </summary>
    public Guid? RelatedRiskId { get; private set; }

    /// <summary>
    /// Date and time when the incident occurred
    /// </summary>
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// Date and time when the incident was reported
    /// </summary>
    public DateTime ReportedAt { get; private set; }

    /// <summary>
    /// User ID who reported the incident
    /// </summary>
    public Guid ReportedBy { get; private set; }

    /// <summary>
    /// User ID assigned to investigate the incident
    /// </summary>
    public Guid? AssignedTo { get; private set; }

    /// <summary>
    /// Department affected by the incident
    /// </summary>
    public string AffectedDepartment { get; private set; }

    /// <summary>
    /// Location where the incident occurred
    /// </summary>
    public string? Location { get; private set; }

    /// <summary>
    /// Root cause analysis findings
    /// </summary>
    public string? RootCause { get; private set; }

    /// <summary>
    /// Immediate actions taken to address the incident
    /// </summary>
    public string? ImmediateActions { get; private set; }

    /// <summary>
    /// Remediation plan to prevent recurrence
    /// </summary>
    public string? RemediationPlan { get; private set; }

    /// <summary>
    /// Lessons learned from the incident
    /// </summary>
    public string? LessonsLearned { get; private set; }

    /// <summary>
    /// Financial impact of the incident
    /// </summary>
    public decimal? FinancialImpact { get; private set; }

    /// <summary>
    /// Date when remediation was completed
    /// </summary>
    public DateTime? RemediationCompletedAt { get; private set; }

    /// <summary>
    /// Date when incident was resolved
    /// </summary>
    public DateTime? ResolvedAt { get; private set; }

    /// <summary>
    /// Date when incident was closed
    /// </summary>
    public DateTime? ClosedAt { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private Incident() { }

    public static Incident Create(
        string incidentCode,
        string title,
        string description,
        IncidentSeverity severity,
        RiskCategory category,
        DateTime occurredAt,
        Guid reportedBy,
        string affectedDepartment,
        string? location = null,
        Guid? relatedRiskId = null)
    {
        return new Incident
        {
            Id = Guid.NewGuid(),
            IncidentCode = incidentCode,
            Title = title,
            Description = description,
            Severity = severity,
            Status = IncidentStatus.Reported,
            Category = category,
            OccurredAt = occurredAt,
            ReportedAt = DateTime.UtcNow,
            ReportedBy = reportedBy,
            AffectedDepartment = affectedDepartment,
            Location = location,
            RelatedRiskId = relatedRiskId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AssignInvestigator(Guid investigatorId, Guid assignedBy)
    {
        AssignedTo = investigatorId;
        Status = IncidentStatus.Investigating;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = assignedBy;
    }

    public void RecordRootCause(string rootCause, Guid updatedBy)
    {
        RootCause = rootCause;
        Status = IncidentStatus.RootCauseIdentified;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void RecordImmediateActions(string actions, Guid updatedBy)
    {
        ImmediateActions = actions;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void CreateRemediationPlan(string plan, Guid updatedBy)
    {
        RemediationPlan = plan;
        Status = IncidentStatus.Remediating;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void CompleteRemediation(Guid completedBy)
    {
        RemediationCompletedAt = DateTime.UtcNow;
        Status = IncidentStatus.PendingVerification;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = completedBy;
    }

    public void Resolve(Guid resolvedBy)
    {
        ResolvedAt = DateTime.UtcNow;
        Status = IncidentStatus.Resolved;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = resolvedBy;
    }

    public void Close(string lessonsLearned, Guid closedBy)
    {
        LessonsLearned = lessonsLearned;
        ClosedAt = DateTime.UtcNow;
        Status = IncidentStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = closedBy;
    }

    public void RecordFinancialImpact(decimal amount, Guid updatedBy)
    {
        FinancialImpact = amount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
