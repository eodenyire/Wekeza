using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Compliance Admin Service - AML, KYC, Sanctions Screening, and Regulatory Compliance
/// Compliance portal for managing AML cases, KYC verification, sanctions screening, and regulatory reporting
/// </summary>
public interface IComplianceAdminService
{
    // ===== AML Case Management =====
    Task<AMLCaseDTO> GetAMLCaseAsync(Guid caseId);
    Task<List<AMLCaseDTO>> SearchAMLCasesAsync(string? status = null, string? riskLevel = null, int page = 1, int pageSize = 50);
    Task<AMLCaseDTO> CreateAMLCaseAsync(CreateAMLCaseRequest request);
    Task<AMLCaseDTO> UpdateAMLCaseAsync(Guid caseId, UpdateAMLCaseRequest request);
    Task ApproveAMLCaseAsync(Guid caseId, string approvalReason, Guid approverUserId);
    Task RejectAMLCaseAsync(Guid caseId, string rejectionReason, Guid approverUserId);
    Task EscalateAMLCaseAsync(Guid caseId, string escalationReason, Guid escalatedByUserId);
    Task<AMLCaseStatsDTO> GetAMLCaseStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Sanctions Screening =====
    Task<SanctionsScreeningDTO> GetScreeningResultAsync(Guid screeningId);
    Task<List<SanctionsScreeningDTO>> SearchScreeningsAsync(string? partyName = null, string? status = null, int page = 1, int pageSize = 50);
    Task<SanctionsScreeningDTO> RunScreeningAsync(Guid partyId, string screeningReason, Guid screenedByUserId);
    Task ResolveSanctionsHitAsync(Guid screeningId, string resolution, string resolutionReason, Guid resolvedByUserId);
    Task<SanctionsScreeningStatsDTO> GetScreeningStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Transaction Monitoring =====
    Task<TransactionMonitoringDTO> GetMonitoringAlertAsync(Guid alertId);
    Task<List<TransactionMonitoringDTO>> SearchMonitoringAlertsAsync(string? status = null, string? severity = null, int page = 1, int pageSize = 50);
    Task InvestigateMonitoringAlertAsync(Guid alertId, string investigationNotes, Guid investigatedByUserId);
    Task ResolveMonitoringAlertAsync(Guid alertId, string resolution, Guid resolvedByUserId);
    Task<TransactionMonitoringStatsDTO> GetMonitoringStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== KYC Management =====
    Task<KYCVerificationDTO> GetKYCAsync(Guid customerId);
    Task<List<KYCVerificationDTO>> SearchKYCAsync(string? status = null, string? riskLevel = null, int page = 1, int pageSize = 50);
    Task ApproveKYCAsync(Guid customerId, string approvalNotes, Guid approverUserId);
    Task RejectKYCAsync(Guid customerId, string rejectionReason, Guid approverUserId);
    Task RequestKYCUpdateAsync(Guid customerId, List<string> requiredFields, string reason, Guid requestedByUserId);
    Task<KYCExpiryAlertsDTO> GetKYCExpiryAlertsAsync();

    // ===== Regulatory Reporting =====
    Task<RegulatoryReportDTO> GetReportAsync(Guid reportId);
    Task<List<RegulatoryReportDTO>> SearchReportsAsync(string? reportType = null, string? status = null, int page = 1, int pageSize = 50);
    Task<RegulatoryReportDTO> GenerateReportAsync(GenerateRegulatoryReportRequest request, Guid generatedByUserId);
    Task SubmitReportAsync(Guid reportId, string submissionReference, Guid submittedByUserId);
    Task ApproveReportAsync(Guid reportId, string approvalReason, Guid approverUserId);
    Task<RegulatoryReportingStatsDTO> GetReportingStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Compliance Dashboard =====
    Task<ComplianceDashboardDTO> GetComplianceDashboardAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<ComplianceIssueDTO>> GetOpenComplianceIssuesAsync(int pageSize = 20);
    Task<List<ComplianceMetricDTO>> GetComplianceMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null);
}

// DTOs
public class AMLCaseDTO
{
    public Guid Id { get; set; }
    public Guid? PartyId { get; set; }
    public string CaseNumber { get; set; }
    public string Status { get; set; }
    public string RiskLevel { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string ResolutionReason { get; set; }
}

public class CreateAMLCaseRequest
{
    public Guid? PartyId { get; set; }
    public string Description { get; set; }
    public string RiskLevel { get; set; }
    public List<string> Indicators { get; set; }
    public string CreatedBy { get; set; }
}

public class UpdateAMLCaseRequest
{
    public string Description { get; set; }
    public string RiskLevel { get; set; }
    public List<string> Indicators { get; set; }
    public string ModifiedBy { get; set; }
}

public class AMLCaseStatsDTO
{
    public int TotalCases { get; set; }
    public int OpenCases { get; set; }
    public int HighRiskCases { get; set; }
    public int ResolvedCases { get; set; }
    public double ComplianceScore { get; set; }
}

public class SanctionsScreeningDTO
{
    public Guid Id { get; set; }
    public Guid PartyId { get; set; }
    public string PartyName { get; set; }
    public string Status { get; set; }
    public bool HasMatch { get; set; }
    public List<SanctionsHitDTO> Matches { get; set; }
    public DateTime ScreenedAt { get; set; }
    public string ScreenedBy { get; set; }
}

public class SanctionsHitDTO
{
    public string Database { get; set; }
    public string MatchName { get; set; }
    public decimal ConfidenceScore { get; set; }
}

public class SanctionsScreeningStatsDTO
{
    public int TotalScreenings { get; set; }
    public int HitsFound { get; set; }
    public int Resolved { get; set; }
    public int Pending { get; set; }
}

public class TransactionMonitoringDTO
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string AlertType { get; set; }
    public string Severity { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime AlertedAt { get; set; }
}

public class TransactionMonitoringStatsDTO
{
    public int TotalAlerts { get; set; }
    public int OpenAlerts { get; set; }
    public int CriticalAlerts { get; set; }
    public int ResolvedAlerts { get; set; }
}

public class KYCVerificationDTO
{
    public Guid CustomerId { get; set; }
    public string Status { get; set; }
    public string RiskLevel { get; set; }
    public DateTime ApprovedAt { get; set; }
    public string ApprovedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<string> DocumentsVerified { get; set; }
}

public class KYCExpiryAlertsDTO
{
    public List<KYCExpiringDTO> ExpiringIn30Days { get; set; }
    public List<KYCExpiringDTO> ExpiringInNextQuarter { get; set; }
}

public class KYCExpiringDTO
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int DaysUntilExpiry { get; set; }
}

public class RegulatoryReportDTO
{
    public Guid Id { get; set; }
    public string ReportType { get; set; }
    public string Status { get; set; }
    public DateTime ReportingPeriod { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string SubmissionReference { get; set; }
}

public class GenerateRegulatoryReportRequest
{
    public string ReportType { get; set; }
    public DateTime ReportingPeriod { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}

public class RegulatoryReportingStatsDTO
{
    public int TotalReports { get; set; }
    public int SubmittedReports { get; set; }
    public int PendingReports { get; set; }
}

public class ComplianceDashboardDTO
{
    public AMLCaseStatsDTO AMLStats { get; set; }
    public SanctionsScreeningStatsDTO ScreeningStats { get; set; }
    public TransactionMonitoringStatsDTO MonitoringStats { get; set; }
    public int OpenKYCRequests { get; set; }
    public double OverallComplianceScore { get; set; }
    public List<ComplianceIssueDTO> HighPriorityIssues { get; set; }
}

public class ComplianceIssueDTO
{
    public string IssueType { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ComplianceMetricDTO
{
    public string MetricName { get; set; }
    public double Value { get; set; }
    public DateTime RecordedAt { get; set; }
}
