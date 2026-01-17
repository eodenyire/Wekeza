using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

// User Domain Events
public record UserCreatedDomainEvent(Guid UserId, string Username, string Email, string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserActivatedDomainEvent(Guid UserId, string Username, string ActivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserDeactivatedDomainEvent(Guid UserId, string Username, string DeactivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserPasswordChangedDomainEvent(Guid UserId, string Username, string ChangedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserAccountLockedDomainEvent(Guid UserId, string Username, string Reason, DateTime LockedUntil, string LockedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserAccountUnlockedDomainEvent(Guid UserId, string Username, string UnlockedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserMfaEnabledDomainEvent(Guid UserId, string Username, MfaMethod Method, string EnabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserMfaDisabledDomainEvent(Guid UserId, string Username, string DisabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserRoleAddedDomainEvent(Guid UserId, string Username, string RoleCode, string AddedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserRoleRemovedDomainEvent(Guid UserId, string Username, string RoleCode, string RemovedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserProfileUpdatedDomainEvent(Guid UserId, string Username, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserLoggedInDomainEvent(Guid UserId, string Username, string IpAddress, string SessionId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserLoginFailedDomainEvent(Guid UserId, string Username, string IpAddress, int FailedAttempts) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserSessionStartedDomainEvent(Guid UserId, string Username, string SessionId, string IpAddress) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserSessionEndedDomainEvent(Guid UserId, string Username, string SessionId, string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record UserSecurityClearanceUpdatedDomainEvent(Guid UserId, string Username, SecurityClearanceLevel OldLevel, SecurityClearanceLevel NewLevel, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Role Domain Events
public record RoleCreatedDomainEvent(Guid RoleId, string RoleCode, string RoleName, RoleType Type, string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleActivatedDomainEvent(Guid RoleId, string RoleCode, string ActivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleDeactivatedDomainEvent(Guid RoleId, string RoleCode, string DeactivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RolePermissionAddedDomainEvent(Guid RoleId, string RoleCode, string PermissionCode, string AddedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RolePermissionRemovedDomainEvent(Guid RoleId, string RoleCode, string PermissionCode, string RemovedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleModuleAccessUpdatedDomainEvent(Guid RoleId, string RoleCode, string Module, AccessLevel Level, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleIpRestrictionAddedDomainEvent(Guid RoleId, string RoleCode, string IpAddress, string AddedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleIpRestrictionRemovedDomainEvent(Guid RoleId, string RoleCode, string IpAddress, string RemovedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleTimeRestrictionAddedDomainEvent(Guid RoleId, string RoleCode, TimeWindow TimeWindow, string AddedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleTransactionLimitsUpdatedDomainEvent(Guid RoleId, string RoleCode, decimal? TransactionLimit, decimal? DailyLimit, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleApprovalWorkflowUpdatedDomainEvent(Guid RoleId, string RoleCode, List<string> Workflow, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleHierarchyUpdatedDomainEvent(Guid RoleId, string RoleCode, Guid ParentRoleId, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleChildAddedDomainEvent(Guid RoleId, string RoleCode, Guid ChildRoleId, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleChildRemovedDomainEvent(Guid RoleId, string RoleCode, Guid ChildRoleId, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleMfaRequirementEnabledDomainEvent(Guid RoleId, string RoleCode, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RoleMfaRequirementDisabledDomainEvent(Guid RoleId, string RoleCode, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Audit Log Domain Events
public record AuditLogCreatedDomainEvent(Guid AuditLogId, string EventType, string? UserId, string Action) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityAuditLogCreatedDomainEvent(Guid AuditLogId, string EventType, string? UserId, RiskLevel RiskLevel) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record TransactionAuditLogCreatedDomainEvent(Guid AuditLogId, string UserId, string TransactionType, decimal Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record AuditLogMarkedForReviewDomainEvent(Guid AuditLogId, string EventType, string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record AuditLogArchivedDomainEvent(Guid AuditLogId, string EventType, string ArchiveLocation) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record AuditLogRetentionExtendedDomainEvent(Guid AuditLogId, string EventType, DateTime NewRetentionDate, string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// System Parameter Domain Events
public record SystemParameterCreatedDomainEvent(Guid ParameterId, string ParameterCode, string ParameterName, ParameterType Type, string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterValueUpdatedDomainEvent(Guid ParameterId, string ParameterCode, string OldValue, string NewValue, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterActivatedDomainEvent(Guid ParameterId, string ParameterCode, DateTime EffectiveFrom, string? ActivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterDeactivatedDomainEvent(Guid ParameterId, string ParameterCode, DateTime EffectiveTo, string? DeactivatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterSecurityLevelUpdatedDomainEvent(Guid ParameterId, string ParameterCode, SecurityLevel OldLevel, SecurityLevel NewLevel, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterAllowedRoleAddedDomainEvent(Guid ParameterId, string ParameterCode, string Role, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterAllowedRoleRemovedDomainEvent(Guid ParameterId, string ParameterCode, string Role, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterValidationUpdatedDomainEvent(Guid ParameterId, string ParameterCode, string? UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterApprovalWorkflowUpdatedDomainEvent(Guid ParameterId, string ParameterCode, string Workflow, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterEncryptionEnabledDomainEvent(Guid ParameterId, string ParameterCode, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemParameterEncryptionDisabledDomainEvent(Guid ParameterId, string ParameterCode, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// System Monitor Domain Events
public record SystemMonitorCreatedDomainEvent(Guid MonitorId, string MonitorCode, string MonitorName, MonitorType Type, string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorEnabledDomainEvent(Guid MonitorId, string MonitorCode, string EnabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorDisabledDomainEvent(Guid MonitorId, string MonitorCode, string DisabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorPausedDomainEvent(Guid MonitorId, string MonitorCode, string PausedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorResumedDomainEvent(Guid MonitorId, string MonitorCode, string ResumedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorConfigurationUpdatedDomainEvent(Guid MonitorId, string MonitorCode, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorIntervalUpdatedDomainEvent(Guid MonitorId, string MonitorCode, TimeSpan Interval, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorThresholdUpdatedDomainEvent(Guid MonitorId, string MonitorCode, string Metric, decimal Value, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorThresholdRemovedDomainEvent(Guid MonitorId, string MonitorCode, string Metric, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorAlertRuleAddedDomainEvent(Guid MonitorId, string MonitorCode, string Condition, AlertSeverity Severity, string AddedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorAlertRuleRemovedDomainEvent(Guid MonitorId, string MonitorCode, string Condition, string RemovedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorNotificationChannelAddedDomainEvent(Guid MonitorId, string MonitorCode, string Channel, string AddedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorNotificationChannelRemovedDomainEvent(Guid MonitorId, string MonitorCode, string Channel, string RemovedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorCheckCompletedDomainEvent(Guid MonitorId, string MonitorCode, bool Success, MonitorHealth Health, TimeSpan ResponseTime) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorAlertTriggeredDomainEvent(Guid MonitorId, string MonitorCode, AlertSeverity Severity, string Message, Guid AlertId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMonitorHealthChangedDomainEvent(Guid MonitorId, string MonitorCode, MonitorHealth PreviousHealth, MonitorHealth NewHealth) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Security Policy Domain Events
public record SecurityPolicyCreatedDomainEvent(string PolicyCode, string PolicyName, PolicyType Type, SecurityLevel Level, string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityPolicyUpdatedDomainEvent(string PolicyCode, string PolicyName, string UpdatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityPolicyEnforcedDomainEvent(string PolicyCode, string PolicyName, string EnforcedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityPolicyDisabledDomainEvent(string PolicyCode, string PolicyName, string DisabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityPolicyViolationDomainEvent(string PolicyCode, string? UserId, string Resource, string ViolationDetails) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityPolicyEvaluatedDomainEvent(string PolicyCode, string? UserId, string Resource, bool IsCompliant, Dictionary<string, object> Context) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Authentication & Authorization Events
public record AuthenticationAttemptDomainEvent(string? UserId, string Username, string IpAddress, bool Success, string? FailureReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record AuthorizationCheckDomainEvent(string UserId, string Resource, string Action, bool Authorized, string? DenialReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record PermissionGrantedDomainEvent(string UserId, string Permission, string Resource, string GrantedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record PermissionRevokedDomainEvent(string UserId, string Permission, string Resource, string RevokedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record PrivilegeEscalationAttemptDomainEvent(string UserId, string AttemptedAction, string Resource, string IpAddress) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SuspiciousActivityDetectedDomainEvent(string? UserId, string ActivityType, string Details, RiskLevel RiskLevel, string IpAddress) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// System Administration Events
public record SystemConfigurationChangedDomainEvent(string ConfigurationKey, string? OldValue, string NewValue, string ChangedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMaintenanceModeEnabledDomainEvent(string Reason, DateTime ScheduledEnd, string EnabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemMaintenanceModeDisabledDomainEvent(string DisabledBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemBackupInitiatedDomainEvent(string BackupType, string InitiatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemBackupCompletedDomainEvent(string BackupType, bool Success, string? ErrorMessage, TimeSpan Duration) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemHealthCheckCompletedDomainEvent(string CheckType, bool Healthy, Dictionary<string, object> Results) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemPerformanceThresholdExceededDomainEvent(string Metric, decimal Value, decimal Threshold, DateTime Timestamp) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SystemResourceUsageAlertDomainEvent(string Resource, decimal UsagePercentage, AlertSeverity Severity) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Compliance & Regulatory Events
public record ComplianceCheckInitiatedDomainEvent(string CheckType, string InitiatedBy, DateTime ScheduledFor) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record ComplianceCheckCompletedDomainEvent(string CheckType, bool Compliant, List<string> Violations, DateTime CompletedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record RegulatoryReportGeneratedDomainEvent(string ReportType, string ReportId, DateTime ReportDate, string GeneratedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record DataRetentionPolicyAppliedDomainEvent(string PolicyCode, int RecordsProcessed, int RecordsArchived, int RecordsDeleted) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record PrivacyRequestProcessedDomainEvent(string RequestType, string SubjectId, bool Completed, string ProcessedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Security Incident Events
public record SecurityIncidentCreatedDomainEvent(Guid IncidentId, string IncidentType, AlertSeverity Severity, string Description, string? UserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityIncidentEscalatedDomainEvent(Guid IncidentId, AlertSeverity NewSeverity, string EscalatedBy, string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityIncidentResolvedDomainEvent(Guid IncidentId, string Resolution, string ResolvedBy, TimeSpan Duration) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityThreatDetectedDomainEvent(string ThreatType, string Source, RiskLevel RiskLevel, Dictionary<string, object> ThreatData) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
public record SecurityControlTriggeredDomainEvent(string ControlType, string TriggerReason, string? UserId, string IpAddress) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}