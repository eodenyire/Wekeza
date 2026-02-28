using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Security Admin Service - User Access Control, Security Policies, Incident Management, Audit Logs
/// Security operations portal for managing user access, security policies, incidents, and audit trails
/// </summary>
public interface ISecurityAdminService
{
    // ===== Access Control Management =====
    Task<UserAccessDTO> GetUserAccessAsync(Guid userId);
    Task<List<UserAccessDTO>> SearchUserAccessAsync(string? status = null, int page = 1, int pageSize = 50);
    Task GrantAccessAsync(Guid userId, string resourceCode, string accessLevel, string grantedByUserId);
    Task RevokeAccessAsync(Guid userId, string resourceCode, string reason, string revokedByUserId);
    Task UpdateAccessExpiryAsync(Guid userId, string resourceCode, DateTime newExpiryDate, string updatedByUserId);
    Task<AccessApprovalDTO> RequestAccessChangeAsync(Guid userId, AccessChangeRequest request, Guid requestedByUserId);
    Task ApproveAccessChangeAsync(Guid approvalId, string approvalReason, Guid approverUserId);
    Task RejectAccessChangeAsync(Guid approvalId, string rejectionReason, Guid approverUserId);

    // ===== Security Policy Management =====
    Task<SecurityPolicyDTO> GetPolicyAsync(Guid policyId);
    Task<List<SecurityPolicyDTO>> GetAllPoliciesAsync(int page = 1, int pageSize = 50);
    Task<SecurityPolicyDTO> CreatePolicyAsync(CreateSecurityPolicyRequest request, Guid createdByUserId);
    Task<SecurityPolicyDTO> UpdatePolicyAsync(Guid policyId, UpdateSecurityPolicyRequest request, Guid updatedByUserId);
    Task ActivatePolicyAsync(Guid policyId, Guid activatedByUserId);
    Task DeactivatePolicyAsync(Guid policyId, string reason, Guid deactivatedByUserId);
    Task<List<PolicyComplianceDTO>> CheckPolicyComplianceAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Incident Management =====
    Task<SecurityIncidentDTO> GetIncidentAsync(Guid incidentId);
    Task<List<SecurityIncidentDTO>> SearchIncidentsAsync(string? severity = null, string? status = null, int page = 1, int pageSize = 50);
    Task<SecurityIncidentDTO> ReportIncidentAsync(ReportSecurityIncidentRequest request, Guid reportedByUserId);
    Task InvestigateIncidentAsync(Guid incidentId, string investigationNotes, Guid investigatedByUserId);
    Task ResolveIncidentAsync(Guid incidentId, string resolution, Guid resolvedByUserId);
    Task EscalateIncidentAsync(Guid incidentId, string escalationReason, Guid escalatedByUserId);
    Task<IncidentAnalysisDTO> AnalyzeIncidentAsync(Guid incidentId);

    // ===== Audit Log Management =====
    Task<AuditLogEntryDTO> GetAuditLogAsync(Guid logId);
    Task<List<AuditLogEntryDTO>> SearchAuditLogsAsync(string? userId = null, string? action = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 100);
    Task<List<AuditLogEntryDTO>> GetUserAuditTrailAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<AuditLogEntryDTO>> GetEntityAuditTrailAsync(string entityType, string entityId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<AuditAnalysisDTO> AnalyzeAuditLogsAsync(string? userId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<AuditComplianceReportDTO> GenerateAuditComplianceReportAsync(DateTime fromDate, DateTime toDate, Guid requestedByUserId);

    // ===== Session Management =====
    Task<UserSessionDTO> GetSessionAsync(Guid sessionId);
    Task<List<UserSessionDTO>> GetUserSessionsAsync(Guid userId);
    Task<List<UserSessionDTO>> GetAllActiveSessions(int page = 1, int pageSize = 50);
    Task TerminateSessionAsync(Guid sessionId, string reason, Guid terminatedByUserId);
    Task TerminateAllUserSessionsAsync(Guid userId, string reason, Guid terminatedByUserId);
    Task<SuspiciousSessionDTO> DetectedSuspiciousSessionsAsync();

    // ===== Security Dashboard =====
    Task<SecurityDashboardDTO> GetSecurityDashboardAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<SecurityAlertDTO>> GetSecurityAlertsAsync(int pageSize = 10);
    Task<SecurityMetricsDTO> GetSecurityMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<PolicyViolationDTO>> GetPolicyViolationsAsync(DateTime? fromDate = null, DateTime? toDate = null);
}

// DTOs
public class UserAccessDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public List<UserResourceAccessDTO> ResourceAccess { get; set; }
    public DateTime LastAccessAt { get; set; }
    public string AccessStatus { get; set; }
}

public class UserResourceAccessDTO
{
    public string ResourceCode { get; set; }
    public string ResourceName { get; set; }
    public string AccessLevel { get; set; }
    public DateTime GrantedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string GrantedBy { get; set; }
}

public class AccessChangeRequest
{
    public string ResourceCode { get; set; }
    public string RequestedAccessLevel { get; set; }
    public string BusinessJustification { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class AccessApprovalDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RequestDetails { get; set; }
    public string Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class SecurityPolicyDTO
{
    public Guid Id { get; set; }
    public string PolicyCode { get; set; }
    public string PolicyName { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public Dictionary<string, object> PolicyRules { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
}

public class CreateSecurityPolicyRequest
{
    public string PolicyCode { get; set; }
    public string PolicyName { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> PolicyRules { get; set; }
}

public class UpdateSecurityPolicyRequest
{
    public string PolicyName { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> PolicyRules { get; set; }
}

public class PolicyComplianceDTO
{
    public Guid PolicyId { get; set; }
    public string PolicyName { get; set; }
    public decimal CompliancePercentage { get; set; }
    public int ViolationsFound { get; set; }
    public List<PolicyViolationDTO> Violations { get; set; }
}

public class PolicyViolationDTO
{
    public Guid ViolationId { get; set; }
    public string PolicyCode { get; set; }
    public string ViolationType { get; set; }
    public string Description { get; set; }
    public string Severity { get; set; }
    public DateTime DetectedAt { get; set; }
    public string AffectedUser { get; set; }
}

public class SecurityIncidentDTO
{
    public Guid Id { get; set; }
    public string IncidentType { get; set; }
    public string Severity { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime ReportedAt { get; set; }
    public string ReportedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string Resolution { get; set; }
}

public class ReportSecurityIncidentRequest
{
    public string IncidentType { get; set; }
    public string Severity { get; set; }
    public string Description { get; set; }
    public List<string> AffectedSystems { get; set; }
    public Dictionary<string, object> IncidentDetails { get; set; }
}

public class IncidentAnalysisDTO
{
    public Guid IncidentId { get; set; }
    public string RootCause { get; set; }
    public List<string> ImpactedServices { get; set; }
    public List<string> RecommendedActions { get; set; }
    public DateTime AnalyzedAt { get; set; }
}

public class AuditLogEntryDTO
{
    public Guid Id { get; set; }
    public string EventType { get; set; }
    public string Action { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
    public int? ResponseStatus { get; set; }
    public string ResourceAffected { get; set; }
    public Dictionary<string, object> Details { get; set; }
}

public class AuditAnalysisDTO
{
    public int TotalEvents { get; set; }
    public int FailedAttempts { get; set; }
    public List<string> TopUsers { get; set; }
    public List<string> TopActions { get; set; }
    public double AnomalyScore { get; set; }
}

public class AuditComplianceReportDTO
{
    public Guid ReportId { get; set; }
    public DateTime ReportingPeriod { get; set; }
    public int TotalAuditEvents { get; set; }
    public int SecurityIncidents { get; set; }
    public int PolicyViolations { get; set; }
    public double ComplianceScore { get; set; }
}

public class UserSessionDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string IpAddress { get; set; }
    public DateTime LoginAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public string Status { get; set; }
    public TimeSpan? Duration { get; set; }
}

public class SuspiciousSessionDTO
{
    public List<UserSessionDTO> SuspiciousSessions { get; set; }
    public List<SessionAnomalyDTO> AnomaliesDetected { get; set; }
}

public class SessionAnomalyDTO
{
    public Guid SessionId { get; set; }
    public string AnomalyType { get; set; }
    public string Description { get; set; }
    public string RiskLevel { get; set; }
}

public class SecurityDashboardDTO
{
    public int ActiveUsers { get; set; }
    public int ActiveSessions { get; set; }
    public int OpenIncidents { get; set; }
    public int CriticalIncidents { get; set; }
    public int PolicyViolations { get; set; }
    public double OverallSecurityScore { get; set; }
    public List<SecurityAlertDTO> RecentAlerts { get; set; }
}

public class SecurityAlertDTO
{
    public Guid AlertId { get; set; }
    public string AlertType { get; set; }
    public string Severity { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SecurityMetricsDTO
{
    public int TotalIncidents { get; set; }
    public int ResolvedIncidents { get; set; }
    public double AverageResolutionTime { get; set; }
    public int PolicyViolations { get; set; }
    public double CompliancePercentage { get; set; }
}
