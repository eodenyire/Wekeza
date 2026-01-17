using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

// User Domain Events
public record UserCreatedDomainEvent(Guid UserId, string Username, string Email, string CreatedBy) : IDomainEvent;
public record UserActivatedDomainEvent(Guid UserId, string Username, string ActivatedBy) : IDomainEvent;
public record UserDeactivatedDomainEvent(Guid UserId, string Username, string DeactivatedBy) : IDomainEvent;
public record UserPasswordChangedDomainEvent(Guid UserId, string Username, string ChangedBy) : IDomainEvent;
public record UserAccountLockedDomainEvent(Guid UserId, string Username, string Reason, DateTime LockedUntil, string LockedBy) : IDomainEvent;
public record UserAccountUnlockedDomainEvent(Guid UserId, string Username, string UnlockedBy) : IDomainEvent;
public record UserMfaEnabledDomainEvent(Guid UserId, string Username, MfaMethod Method, string EnabledBy) : IDomainEvent;
public record UserMfaDisabledDomainEvent(Guid UserId, string Username, string DisabledBy) : IDomainEvent;
public record UserRoleAddedDomainEvent(Guid UserId, string Username, string RoleCode, string AddedBy) : IDomainEvent;
public record UserRoleRemovedDomainEvent(Guid UserId, string Username, string RoleCode, string RemovedBy) : IDomainEvent;
public record UserProfileUpdatedDomainEvent(Guid UserId, string Username, string UpdatedBy) : IDomainEvent;
public record UserLoggedInDomainEvent(Guid UserId, string Username, string IpAddress, string SessionId) : IDomainEvent;
public record UserLoginFailedDomainEvent(Guid UserId, string Username, string IpAddress, int FailedAttempts) : IDomainEvent;
public record UserSessionStartedDomainEvent(Guid UserId, string Username, string SessionId, string IpAddress) : IDomainEvent;
public record UserSessionEndedDomainEvent(Guid UserId, string Username, string SessionId, string Reason) : IDomainEvent;
public record UserSecurityClearanceUpdatedDomainEvent(Guid UserId, string Username, SecurityClearanceLevel OldLevel, SecurityClearanceLevel NewLevel, string UpdatedBy) : IDomainEvent;

// Role Domain Events
public record RoleCreatedDomainEvent(Guid RoleId, string RoleCode, string RoleName, RoleType Type, string CreatedBy) : IDomainEvent;
public record RoleActivatedDomainEvent(Guid RoleId, string RoleCode, string ActivatedBy) : IDomainEvent;
public record RoleDeactivatedDomainEvent(Guid RoleId, string RoleCode, string DeactivatedBy) : IDomainEvent;
public record RolePermissionAddedDomainEvent(Guid RoleId, string RoleCode, string PermissionCode, string AddedBy) : IDomainEvent;
public record RolePermissionRemovedDomainEvent(Guid RoleId, string RoleCode, string PermissionCode, string RemovedBy) : IDomainEvent;
public record RoleModuleAccessUpdatedDomainEvent(Guid RoleId, string RoleCode, string Module, AccessLevel Level, string UpdatedBy) : IDomainEvent;
public record RoleIpRestrictionAddedDomainEvent(Guid RoleId, string RoleCode, string IpAddress, string AddedBy) : IDomainEvent;
public record RoleIpRestrictionRemovedDomainEvent(Guid RoleId, string RoleCode, string IpAddress, string RemovedBy) : IDomainEvent;
public record RoleTimeRestrictionAddedDomainEvent(Guid RoleId, string RoleCode, TimeWindow TimeWindow, string AddedBy) : IDomainEvent;
public record RoleTransactionLimitsUpdatedDomainEvent(Guid RoleId, string RoleCode, decimal? TransactionLimit, decimal? DailyLimit, string UpdatedBy) : IDomainEvent;
public record RoleApprovalWorkflowUpdatedDomainEvent(Guid RoleId, string RoleCode, List<string> Workflow, string UpdatedBy) : IDomainEvent;
public record RoleHierarchyUpdatedDomainEvent(Guid RoleId, string RoleCode, Guid ParentRoleId, string UpdatedBy) : IDomainEvent;
public record RoleChildAddedDomainEvent(Guid RoleId, string RoleCode, Guid ChildRoleId, string UpdatedBy) : IDomainEvent;
public record RoleChildRemovedDomainEvent(Guid RoleId, string RoleCode, Guid ChildRoleId, string UpdatedBy) : IDomainEvent;
public record RoleMfaRequirementEnabledDomainEvent(Guid RoleId, string RoleCode, string UpdatedBy) : IDomainEvent;
public record RoleMfaRequirementDisabledDomainEvent(Guid RoleId, string RoleCode, string UpdatedBy) : IDomainEvent;

// Audit Log Domain Events
public record AuditLogCreatedDomainEvent(Guid AuditLogId, string EventType, string? UserId, string Action) : IDomainEvent;
public record SecurityAuditLogCreatedDomainEvent(Guid AuditLogId, string EventType, string? UserId, RiskLevel RiskLevel) : IDomainEvent;
public record TransactionAuditLogCreatedDomainEvent(Guid AuditLogId, string UserId, string TransactionType, decimal Amount) : IDomainEvent;
public record AuditLogMarkedForReviewDomainEvent(Guid AuditLogId, string EventType, string Reason) : IDomainEvent;
public record AuditLogArchivedDomainEvent(Guid AuditLogId, string EventType, string ArchiveLocation) : IDomainEvent;
public record AuditLogRetentionExtendedDomainEvent(Guid AuditLogId, string EventType, DateTime NewRetentionDate, string Reason) : IDomainEvent;

// System Parameter Domain Events
public record SystemParameterCreatedDomainEvent(Guid ParameterId, string ParameterCode, string ParameterName, ParameterType Type, string CreatedBy) : IDomainEvent;
public record SystemParameterValueUpdatedDomainEvent(Guid ParameterId, string ParameterCode, string OldValue, string NewValue, string UpdatedBy) : IDomainEvent;
public record SystemParameterActivatedDomainEvent(Guid ParameterId, string ParameterCode, DateTime EffectiveFrom, string? ActivatedBy) : IDomainEvent;
public record SystemParameterDeactivatedDomainEvent(Guid ParameterId, string ParameterCode, DateTime EffectiveTo, string? DeactivatedBy) : IDomainEvent;
public record SystemParameterSecurityLevelUpdatedDomainEvent(Guid ParameterId, string ParameterCode, SecurityLevel OldLevel, SecurityLevel NewLevel, string UpdatedBy) : IDomainEvent;
public record SystemParameterAllowedRoleAddedDomainEvent(Guid ParameterId, string ParameterCode, string Role, string UpdatedBy) : IDomainEvent;
public record SystemParameterAllowedRoleRemovedDomainEvent(Guid ParameterId, string ParameterCode, string Role, string UpdatedBy) : IDomainEvent;
public record SystemParameterValidationUpdatedDomainEvent(Guid ParameterId, string ParameterCode, string? UpdatedBy) : IDomainEvent;
public record SystemParameterApprovalWorkflowUpdatedDomainEvent(Guid ParameterId, string ParameterCode, string Workflow, string UpdatedBy) : IDomainEvent;
public record SystemParameterEncryptionEnabledDomainEvent(Guid ParameterId, string ParameterCode, string UpdatedBy) : IDomainEvent;
public record SystemParameterEncryptionDisabledDomainEvent(Guid ParameterId, string ParameterCode, string UpdatedBy) : IDomainEvent;

// System Monitor Domain Events
public record SystemMonitorCreatedDomainEvent(Guid MonitorId, string MonitorCode, string MonitorName, MonitorType Type, string CreatedBy) : IDomainEvent;
public record SystemMonitorEnabledDomainEvent(Guid MonitorId, string MonitorCode, string EnabledBy) : IDomainEvent;
public record SystemMonitorDisabledDomainEvent(Guid MonitorId, string MonitorCode, string DisabledBy) : IDomainEvent;
public record SystemMonitorPausedDomainEvent(Guid MonitorId, string MonitorCode, string PausedBy) : IDomainEvent;
public record SystemMonitorResumedDomainEvent(Guid MonitorId, string MonitorCode, string ResumedBy) : IDomainEvent;
public record SystemMonitorConfigurationUpdatedDomainEvent(Guid MonitorId, string MonitorCode, string UpdatedBy) : IDomainEvent;
public record SystemMonitorIntervalUpdatedDomainEvent(Guid MonitorId, string MonitorCode, TimeSpan Interval, string UpdatedBy) : IDomainEvent;
public record SystemMonitorThresholdUpdatedDomainEvent(Guid MonitorId, string MonitorCode, string Metric, decimal Value, string UpdatedBy) : IDomainEvent;
public record SystemMonitorThresholdRemovedDomainEvent(Guid MonitorId, string MonitorCode, string Metric, string UpdatedBy) : IDomainEvent;
public record SystemMonitorAlertRuleAddedDomainEvent(Guid MonitorId, string MonitorCode, string Condition, AlertSeverity Severity, string AddedBy) : IDomainEvent;
public record SystemMonitorAlertRuleRemovedDomainEvent(Guid MonitorId, string MonitorCode, string Condition, string RemovedBy) : IDomainEvent;
public record SystemMonitorNotificationChannelAddedDomainEvent(Guid MonitorId, string MonitorCode, string Channel, string AddedBy) : IDomainEvent;
public record SystemMonitorNotificationChannelRemovedDomainEvent(Guid MonitorId, string MonitorCode, string Channel, string RemovedBy) : IDomainEvent;
public record SystemMonitorCheckCompletedDomainEvent(Guid MonitorId, string MonitorCode, bool Success, MonitorHealth Health, TimeSpan ResponseTime) : IDomainEvent;
public record SystemMonitorAlertTriggeredDomainEvent(Guid MonitorId, string MonitorCode, AlertSeverity Severity, string Message, Guid AlertId) : IDomainEvent;
public record SystemMonitorHealthChangedDomainEvent(Guid MonitorId, string MonitorCode, MonitorHealth PreviousHealth, MonitorHealth NewHealth) : IDomainEvent;

// Security Policy Domain Events
public record SecurityPolicyCreatedDomainEvent(string PolicyCode, string PolicyName, PolicyType Type, SecurityLevel Level, string CreatedBy) : IDomainEvent;
public record SecurityPolicyUpdatedDomainEvent(string PolicyCode, string PolicyName, string UpdatedBy) : IDomainEvent;
public record SecurityPolicyEnforcedDomainEvent(string PolicyCode, string PolicyName, string EnforcedBy) : IDomainEvent;
public record SecurityPolicyDisabledDomainEvent(string PolicyCode, string PolicyName, string DisabledBy) : IDomainEvent;
public record SecurityPolicyViolationDomainEvent(string PolicyCode, string? UserId, string Resource, string ViolationDetails) : IDomainEvent;
public record SecurityPolicyEvaluatedDomainEvent(string PolicyCode, string? UserId, string Resource, bool IsCompliant, Dictionary<string, object> Context) : IDomainEvent;

// Authentication & Authorization Events
public record AuthenticationAttemptDomainEvent(string? UserId, string Username, string IpAddress, bool Success, string? FailureReason) : IDomainEvent;
public record AuthorizationCheckDomainEvent(string UserId, string Resource, string Action, bool Authorized, string? DenialReason) : IDomainEvent;
public record PermissionGrantedDomainEvent(string UserId, string Permission, string Resource, string GrantedBy) : IDomainEvent;
public record PermissionRevokedDomainEvent(string UserId, string Permission, string Resource, string RevokedBy) : IDomainEvent;
public record PrivilegeEscalationAttemptDomainEvent(string UserId, string AttemptedAction, string Resource, string IpAddress) : IDomainEvent;
public record SuspiciousActivityDetectedDomainEvent(string? UserId, string ActivityType, string Details, RiskLevel RiskLevel, string IpAddress) : IDomainEvent;

// System Administration Events
public record SystemConfigurationChangedDomainEvent(string ConfigurationKey, string? OldValue, string NewValue, string ChangedBy) : IDomainEvent;
public record SystemMaintenanceModeEnabledDomainEvent(string Reason, DateTime ScheduledEnd, string EnabledBy) : IDomainEvent;
public record SystemMaintenanceModeDisabledDomainEvent(string DisabledBy) : IDomainEvent;
public record SystemBackupInitiatedDomainEvent(string BackupType, string InitiatedBy) : IDomainEvent;
public record SystemBackupCompletedDomainEvent(string BackupType, bool Success, string? ErrorMessage, TimeSpan Duration) : IDomainEvent;
public record SystemHealthCheckCompletedDomainEvent(string CheckType, bool Healthy, Dictionary<string, object> Results) : IDomainEvent;
public record SystemPerformanceThresholdExceededDomainEvent(string Metric, decimal Value, decimal Threshold, DateTime Timestamp) : IDomainEvent;
public record SystemResourceUsageAlertDomainEvent(string Resource, decimal UsagePercentage, AlertSeverity Severity) : IDomainEvent;

// Compliance & Regulatory Events
public record ComplianceCheckInitiatedDomainEvent(string CheckType, string InitiatedBy, DateTime ScheduledFor) : IDomainEvent;
public record ComplianceCheckCompletedDomainEvent(string CheckType, bool Compliant, List<string> Violations, DateTime CompletedAt) : IDomainEvent;
public record RegulatoryReportGeneratedDomainEvent(string ReportType, string ReportId, DateTime ReportDate, string GeneratedBy) : IDomainEvent;
public record DataRetentionPolicyAppliedDomainEvent(string PolicyCode, int RecordsProcessed, int RecordsArchived, int RecordsDeleted) : IDomainEvent;
public record PrivacyRequestProcessedDomainEvent(string RequestType, string SubjectId, bool Completed, string ProcessedBy) : IDomainEvent;

// Security Incident Events
public record SecurityIncidentCreatedDomainEvent(Guid IncidentId, string IncidentType, AlertSeverity Severity, string Description, string? UserId) : IDomainEvent;
public record SecurityIncidentEscalatedDomainEvent(Guid IncidentId, AlertSeverity NewSeverity, string EscalatedBy, string Reason) : IDomainEvent;
public record SecurityIncidentResolvedDomainEvent(Guid IncidentId, string Resolution, string ResolvedBy, TimeSpan Duration) : IDomainEvent;
public record SecurityThreatDetectedDomainEvent(string ThreatType, string Source, RiskLevel RiskLevel, Dictionary<string, object> ThreatData) : IDomainEvent;
public record SecurityControlTriggeredDomainEvent(string ControlType, string TriggerReason, string? UserId, string IpAddress) : IDomainEvent;