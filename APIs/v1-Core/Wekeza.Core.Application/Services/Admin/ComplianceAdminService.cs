using Wekeza.Core.Application.Admin;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Services.Admin;

public class ComplianceAdminService : IComplianceAdminService
{
    private readonly dynamic _repository;
    private readonly ILogger<ComplianceAdminService> _logger;

    public ComplianceAdminService(dynamic repository, ILogger<ComplianceAdminService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ===== AML Case Management =====
    public async Task<AMLCaseDTO> GetAMLCaseAsync(Guid caseId)
    {
        try
        {
            var amlCase = await _repository.GetAMLCaseByIdAsync(caseId);
            if (amlCase == null)
            {
                _logger.LogWarning($"AML case not found: {caseId}");
                return null;
            }

            return MapToAMLCaseDTO(amlCase);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving AML case {caseId}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<AMLCaseDTO>> SearchAMLCasesAsync(string status = null, string riskLevel = null, int page = 1, int pageSize = 50)
    {
        try
        {
            var cases = await _repository.SearchAMLCasesAsync(status, riskLevel, page, pageSize);
            return cases.Select(MapToAMLCaseDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching AML cases: {ex.Message}");
            throw;
        }
    }

    public async Task<AMLCaseDTO> CreateAMLCaseAsync(CreateAMLCaseRequest request)
    {
        try
        {
            var amlCase = new Domain.Aggregates.AMLCase
            {
                Id = Guid.NewGuid(),
                PartyId = request.PartyId,
                CaseNumber = $"AML-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5)}",
                Status = "Open",
                RiskLevel = request.RiskLevel,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy
            };

            var created = await _repository.AddAMLCaseAsync(amlCase);
            _logger.LogInformation($"AML case created: {amlCase.CaseNumber}");
            return MapToAMLCaseDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating AML case: {ex.Message}");
            throw;
        }
    }

    public async Task<AMLCaseDTO> UpdateAMLCaseAsync(Guid caseId, UpdateAMLCaseRequest request)
    {
        try
        {
            var amlCase = await _repository.GetAMLCaseByIdAsync(caseId);
            if (amlCase == null)
                throw new InvalidOperationException($"AML case not found: {caseId}");

            amlCase.Description = request.Description;
            amlCase.RiskLevel = request.RiskLevel;

            var updated = await _repository.UpdateAMLCaseAsync(amlCase);
            _logger.LogInformation($"AML case updated: {caseId}");
            return MapToAMLCaseDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating AML case {caseId}: {ex.Message}");
            throw;
        }
    }

    public async Task ApproveAMLCaseAsync(Guid caseId, string approvalReason, Guid approverUserId)
    {
        try
        {
            var amlCase = await _repository.GetAMLCaseByIdAsync(caseId);
            if (amlCase == null)
                throw new InvalidOperationException($"AML case not found: {caseId}");

            amlCase.Status = "Approved";
            await _repository.UpdateAMLCaseAsync(amlCase);
            _logger.LogInformation($"AML case approved: {caseId} by {approverUserId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error approving AML case {caseId}: {ex.Message}");
            throw;
        }
    }

    public async Task RejectAMLCaseAsync(Guid caseId, string rejectionReason, Guid approverUserId)
    {
        try
        {
            var amlCase = await _repository.GetAMLCaseByIdAsync(caseId);
            if (amlCase == null)
                throw new InvalidOperationException($"AML case not found: {caseId}");

            amlCase.Status = "Rejected";
            await _repository.UpdateAMLCaseAsync(amlCase);
            _logger.LogInformation($"AML case rejected: {caseId} by {approverUserId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error rejecting AML case {caseId}: {ex.Message}");
            throw;
        }
    }

    public async Task EscalateAMLCaseAsync(Guid caseId, string escalationReason, Guid escalatedByUserId)
    {
        try
        {
            var amlCase = await _repository.GetAMLCaseByIdAsync(caseId);
            if (amlCase == null)
                throw new InvalidOperationException($"AML case not found: {caseId}");

            amlCase.Status = "Escalated";
            await _repository.UpdateAMLCaseAsync(amlCase);
            _logger.LogInformation($"AML case escalated: {caseId} by {escalatedByUserId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error escalating AML case {caseId}: {ex.Message}");
            throw;
        }
    }

    public async Task<AMLCaseStatsDTO> GetAMLCaseStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            var totalCases = await _repository.GetTotalAMLCaseCountAsync();
            var openCases = await _repository.GetOpenAMLCaseCountAsync();
            var highRiskCases = await _repository.GetHighRiskAMLCaseCountAsync();

            return new AMLCaseStatsDTO
            {
                TotalCases = totalCases,
                OpenCases = openCases,
                HighRiskCases = highRiskCases,
                ResolvedCases = totalCases - openCases,
                ComplianceScore = CalculateAMLComplianceScore(totalCases, openCases, highRiskCases)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving AML case statistics: {ex.Message}");
            throw;
        }
    }

    // ===== Sanctions Screening =====
    public async Task<SanctionsScreeningDTO> GetScreeningResultAsync(Guid screeningId)
    {
        try
        {
            var screening = await _repository.GetScreeningByIdAsync(screeningId);
            if (screening == null)
                return null;

            return MapToSanctionsScreeningDTO(screening);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving sanctions screening {screeningId}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<SanctionsScreeningDTO>> SearchScreeningsAsync(string partyName = null, string status = null, int page = 1, int pageSize = 50)
    {
        try
        {
            var screenings = await _repository.SearchScreeningsAsync(partyName, status, page, pageSize);
            return screenings.Select(MapToSanctionsScreeningDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching sanctions screenings: {ex.Message}");
            throw;
        }
    }

    public async Task<SanctionsScreeningDTO> RunScreeningAsync(Guid partyId, string screeningReason, Guid screenedByUserId)
    {
        try
        {
            var screening = new Domain.Aggregates.SanctionsScreening
            {
                Id = Guid.NewGuid(),
                PartyId = partyId,
                PartyName = "Party Name",
                Status = "Screening",
                ScreenedAt = DateTime.UtcNow,
                ScreenedBy = screenedByUserId.ToString()
            };

            var created = await _repository.AddScreeningAsync(screening);
            _logger.LogInformation($"Sanctions screening created: {partyId}");
            return MapToSanctionsScreeningDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error running sanctions screening: {ex.Message}");
            throw;
        }
    }

    public async Task ResolveSanctionsHitAsync(Guid screeningId, string resolution, string resolutionReason, Guid resolvedByUserId)
    {
        try
        {
            var screening = await _repository.GetScreeningByIdAsync(screeningId);
            if (screening == null)
                throw new InvalidOperationException($"Screening not found: {screeningId}");

            screening.Status = resolution;
            await _repository.UpdateScreeningAsync(screening);
            _logger.LogInformation($"Sanctions screening resolved: {screeningId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error resolving sanctions screening {screeningId}: {ex.Message}");
            throw;
        }
    }

    public async Task<SanctionsScreeningStatsDTO> GetScreeningStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            return new SanctionsScreeningStatsDTO
            {
                TotalScreenings = 0,
                HitsFound = 0,
                Resolved = 0,
                Pending = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving sanctions screening statistics: {ex.Message}");
            throw;
        }
    }

    // ===== Transaction Monitoring =====
    public async Task<TransactionMonitoringDTO> GetMonitoringAlertAsync(Guid alertId)
    {
        try
        {
            var alert = await _repository.GetMonitoringAlertByIdAsync(alertId);
            if (alert == null)
                return null;

            return MapToTransactionMonitoringDTO(alert);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving monitoring alert {alertId}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<TransactionMonitoringDTO>> SearchMonitoringAlertsAsync(string status = null, string severity = null, int page = 1, int pageSize = 50)
    {
        try
        {
            var alerts = await _repository.SearchMonitoringAlertsAsync(status, severity, page, pageSize);
            return alerts.Select(MapToTransactionMonitoringDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching monitoring alerts: {ex.Message}");
            throw;
        }
    }

    public async Task InvestigateMonitoringAlertAsync(Guid alertId, string investigationNotes, Guid investigatedByUserId)
    {
        try
        {
            var alert = await _repository.GetMonitoringAlertByIdAsync(alertId);
            if (alert == null)
                throw new InvalidOperationException($"Alert not found: {alertId}");

            alert.Status = "Investigating";
            await _repository.UpdateMonitoringAlertAsync(alert);
            _logger.LogInformation($"Monitoring alert investigation started: {alertId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error investigating monitoring alert {alertId}: {ex.Message}");
            throw;
        }
    }

    public async Task ResolveMonitoringAlertAsync(Guid alertId, string resolution, Guid resolvedByUserId)
    {
        try
        {
            var alert = await _repository.GetMonitoringAlertByIdAsync(alertId);
            if (alert == null)
                throw new InvalidOperationException($"Alert not found: {alertId}");

            alert.Status = resolution;
            await _repository.UpdateMonitoringAlertAsync(alert);
            _logger.LogInformation($"Monitoring alert resolved: {alertId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error resolving monitoring alert {alertId}: {ex.Message}");
            throw;
        }
    }

    public async Task<TransactionMonitoringStatsDTO> GetMonitoringStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            var totalAlerts = await _repository.GetTotalMonitoringAlertsAsync();
            var openAlerts = await _repository.GetOpenMonitoringAlertCountAsync();
            var criticalAlerts = await _repository.GetCriticalMonitoringAlertCountAsync();

            return new TransactionMonitoringStatsDTO
            {
                TotalAlerts = totalAlerts,
                OpenAlerts = openAlerts,
                CriticalAlerts = criticalAlerts,
                ResolvedAlerts = totalAlerts - openAlerts
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving monitoring statistics: {ex.Message}");
            throw;
        }
    }

    // ===== KYC Management =====
    public async Task<KYCVerificationDTO> GetKYCAsync(Guid customerId)
    {
        try
        {
            var kyc = await _repository.GetKYCByCustomerIdAsync(customerId);
            if (kyc == null)
                return null;

            return MapToKYCVerificationDTO(kyc);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving KYC for customer {customerId}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<KYCVerificationDTO>> SearchKYCAsync(string status = null, string riskLevel = null, int page = 1, int pageSize = 50)
    {
        try
        {
            var verifications = await _repository.SearchKYCAsync(status, riskLevel, page, pageSize);
            return verifications.Select(MapToKYCVerificationDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching KYC verifications: {ex.Message}");
            throw;
        }
    }

    public async Task ApproveKYCAsync(Guid customerId, string approvalNotes, Guid approverUserId)
    {
        try
        {
            var kyc = await _repository.GetKYCByCustomerIdAsync(customerId);
            if (kyc == null)
                throw new InvalidOperationException($"KYC not found for customer: {customerId}");

            kyc.Status = "Approved";
            kyc.ApprovedAt = DateTime.UtcNow;
            kyc.ApprovedBy = approverUserId.ToString();
            kyc.ExpiresAt = DateTime.UtcNow.AddYears(3);

            await _repository.UpdateKYCAsync(kyc);
            _logger.LogInformation($"KYC approved for customer {customerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error approving KYC for customer {customerId}: {ex.Message}");
            throw;
        }
    }

    public async Task RejectKYCAsync(Guid customerId, string rejectionReason, Guid approverUserId)
    {
        try
        {
            var kyc = await _repository.GetKYCByCustomerIdAsync(customerId);
            if (kyc == null)
                throw new InvalidOperationException($"KYC not found for customer: {customerId}");

            kyc.Status = "Rejected";
            await _repository.UpdateKYCAsync(kyc);
            _logger.LogInformation($"KYC rejected for customer {customerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error rejecting KYC for customer {customerId}: {ex.Message}");
            throw;
        }
    }

    public async Task RequestKYCUpdateAsync(Guid customerId, List<string> requiredFields, string reason, Guid requestedByUserId)
    {
        try
        {
            var kyc = await _repository.GetKYCByCustomerIdAsync(customerId);
            if (kyc == null)
                throw new InvalidOperationException($"KYC not found for customer: {customerId}");

            kyc.Status = "UpdateRequired";
            await _repository.UpdateKYCAsync(kyc);
            _logger.LogInformation($"KYC update requested for customer {customerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error requesting KYC update for customer {customerId}: {ex.Message}");
            throw;
        }
    }

    public async Task<KYCExpiryAlertsDTO> GetKYCExpiryAlertsAsync()
    {
        try
        {
            var expiring30Days = await _repository.GetExpiringKYCAsync(30);
            var expiringQuarter = await _repository.GetExpiringKYCAsync(90);

            return new KYCExpiryAlertsDTO
            {
                ExpiringIn30Days = expiring30Days.Select(k => new KYCExpiringDTO
                {
                    CustomerId = k.CustomerId,
                    CustomerName = "Customer Name",
                    ExpiryDate = k.ExpiresAt.Value,
                    DaysUntilExpiry = (int)Math.Ceiling((k.ExpiresAt.Value - DateTime.UtcNow).TotalDays)
                }).ToList(),
                ExpiringInNextQuarter = expiringQuarter.Select(k => new KYCExpiringDTO
                {
                    CustomerId = k.CustomerId,
                    CustomerName = "Customer Name",
                    ExpiryDate = k.ExpiresAt.Value,
                    DaysUntilExpiry = (int)Math.Ceiling((k.ExpiresAt.Value - DateTime.UtcNow).TotalDays)
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving KYC expiry alerts: {ex.Message}");
            throw;
        }
    }

    // ===== Regulatory Reporting =====
    public async Task<RegulatoryReportDTO> GetReportAsync(Guid reportId)
    {
        try
        {
            var report = await _repository.GetReportByIdAsync(reportId);
            if (report == null)
                return null;

            return MapToRegulatoryReportDTO(report);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving regulatory report {reportId}: {ex.Message}");
            throw;
        }
    }

    public async Task<List<RegulatoryReportDTO>> SearchReportsAsync(string reportType = null, string status = null, int page = 1, int pageSize = 50)
    {
        try
        {
            var reports = await _repository.SearchReportsAsync(reportType, status, page, pageSize);
            return reports.Select(MapToRegulatoryReportDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching regulatory reports: {ex.Message}");
            throw;
        }
    }

    public async Task<RegulatoryReportDTO> GenerateReportAsync(GenerateRegulatoryReportRequest request, Guid generatedByUserId)
    {
        try
        {
            var report = new Domain.Aggregates.RegulatoryReport
            {
                Id = Guid.NewGuid(),
                ReportType = request.ReportType,
                Status = "Generated",
                GeneratedAt = DateTime.UtcNow,
                ReportingPeriod = request.ReportingPeriod
            };

            var created = await _repository.AddReportAsync(report);
            _logger.LogInformation($"Regulatory report generated: {request.ReportType}");
            return MapToRegulatoryReportDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating regulatory report: {ex.Message}");
            throw;
        }
    }

    public async Task SubmitReportAsync(Guid reportId, string submissionReference, Guid submittedByUserId)
    {
        try
        {
            var report = await _repository.GetReportByIdAsync(reportId);
            if (report == null)
                throw new InvalidOperationException($"Report not found: {reportId}");

            report.Status = "Submitted";
            await _repository.UpdateReportAsync(report);
            _logger.LogInformation($"Regulatory report submitted: {reportId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error submitting regulatory report {reportId}: {ex.Message}");
            throw;
        }
    }

    public async Task ApproveReportAsync(Guid reportId, string approvalReason, Guid approverUserId)
    {
        try
        {
            var report = await _repository.GetReportByIdAsync(reportId);
            if (report == null)
                throw new InvalidOperationException($"Report not found: {reportId}");

            report.Status = "Approved";
            await _repository.UpdateReportAsync(report);
            _logger.LogInformation($"Regulatory report approved: {reportId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error approving regulatory report {reportId}: {ex.Message}");
            throw;
        }
    }

    public async Task<RegulatoryReportingStatsDTO> GetReportingStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            return new RegulatoryReportingStatsDTO
            {
                TotalReports = 0,
                SubmittedReports = 0,
                PendingReports = await _repository.GetPendingReportCountAsync()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving regulatory reporting statistics: {ex.Message}");
            throw;
        }
    }

    // ===== Compliance Dashboard =====
    public async Task<ComplianceDashboardDTO> GetComplianceDashboardAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            return new ComplianceDashboardDTO
            {
                AMLStats = await GetAMLCaseStatsAsync(fromDate, toDate),
                ScreeningStats = await GetScreeningStatsAsync(fromDate, toDate),
                MonitoringStats = await GetMonitoringStatsAsync(fromDate, toDate),
                OpenKYCRequests = 0,
                OverallComplianceScore = 0.85,
                HighPriorityIssues = new List<ComplianceIssueDTO>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving compliance dashboard: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ComplianceIssueDTO>> GetOpenComplianceIssuesAsync(int pageSize = 20)
    {
        try
        {
            return new List<ComplianceIssueDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving open compliance issues: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ComplianceMetricDTO>> GetComplianceMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            return new List<ComplianceMetricDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving compliance metrics: {ex.Message}");
            throw;
        }
    }

    // ===== Helper Methods =====
    private AMLCaseDTO MapToAMLCaseDTO(Domain.Aggregates.AMLCase amlCase)
    {
        return new AMLCaseDTO
        {
            Id = amlCase.Id,
            PartyId = amlCase.PartyId,
            CaseNumber = amlCase.CaseNumber,
            Status = amlCase.Status,
            RiskLevel = amlCase.RiskLevel,
            Description = amlCase.Description,
            CreatedAt = amlCase.CreatedAt,
            CreatedBy = amlCase.CreatedBy
        };
    }

    private SanctionsScreeningDTO MapToSanctionsScreeningDTO(Domain.Aggregates.SanctionsScreening screening)
    {
        return new SanctionsScreeningDTO
        {
            Id = screening.Id,
            PartyId = screening.PartyId,
            PartyName = screening.PartyName,
            Status = screening.Status,
            HasMatch = false,
            Matches = new List<SanctionsHitDTO>(),
            ScreenedAt = screening.ScreenedAt,
            ScreenedBy = screening.ScreenedBy
        };
    }

    private TransactionMonitoringDTO MapToTransactionMonitoringDTO(Domain.Aggregates.TransactionMonitoring alert)
    {
        return new TransactionMonitoringDTO
        {
            Id = alert.Id,
            TransactionId = alert.TransactionId,
            AlertType = "SuspiciousActivity",
            Severity = alert.Severity,
            Status = alert.Status,
            Description = "Suspicious transaction pattern detected",
            AlertedAt = alert.AlertedAt
        };
    }

    private KYCVerificationDTO MapToKYCVerificationDTO(Domain.Aggregates.KYCVerification kyc)
    {
        return new KYCVerificationDTO
        {
            CustomerId = kyc.CustomerId,
            Status = kyc.Status,
            RiskLevel = kyc.RiskLevel,
            ApprovedAt = kyc.ApprovedAt ?? DateTime.MinValue,
            ApprovedBy = kyc.ApprovedBy,
            ExpiresAt = kyc.ExpiresAt,
            DocumentsVerified = new List<string>()
        };
    }

    private RegulatoryReportDTO MapToRegulatoryReportDTO(Domain.Aggregates.RegulatoryReport report)
    {
        return new RegulatoryReportDTO
        {
            Id = report.Id,
            ReportType = report.ReportType,
            Status = report.Status,
            ReportingPeriod = report.ReportingPeriod,
            GeneratedAt = report.GeneratedAt,
            SubmittedAt = null,
            SubmissionReference = null
        };
    }

    private double CalculateAMLComplianceScore(int totalCases, int openCases, int highRiskCases)
    {
        if (totalCases == 0)
            return 100.0;

        var closureRate = ((totalCases - openCases) / (double)totalCases) * 100;
        var highRiskPenalty = (highRiskCases / (double)totalCases) * 50;
        return Math.Max(0, closureRate - highRiskPenalty);
    }
}
