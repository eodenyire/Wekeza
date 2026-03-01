using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Infrastructure.Repositories.Admin;

public class ComplianceRepository
{
    private readonly ApplicationDbContext _context;

    public ComplianceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ===== AML Case Operations =====
    public async Task<AMLCase> GetAMLCaseByIdAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        return await _context.AMLCases
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == caseId, cancellationToken);
    }

    public async Task<List<AMLCase>> SearchAMLCasesAsync(string status, string riskLevel, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.AMLCases.AsNoTracking();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(c => c.Status == status);

        if (!string.IsNullOrEmpty(riskLevel))
            query = query.Where(c => c.RiskLevel == riskLevel);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<AMLCase> AddAMLCaseAsync(AMLCase amlCase, CancellationToken cancellationToken = default)
    {
        await _context.AMLCases.AddAsync(amlCase, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return amlCase;
    }

    public async Task<AMLCase> UpdateAMLCaseAsync(AMLCase amlCase, CancellationToken cancellationToken = default)
    {
        _context.AMLCases.Update(amlCase);
        await _context.SaveChangesAsync(cancellationToken);
        return amlCase;
    }

    public async Task<int> GetOpenAMLCaseCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AMLCases.CountAsync(c => c.Status == "Open", cancellationToken);
    }

    public async Task<int> GetHighRiskAMLCaseCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AMLCases.CountAsync(c => c.RiskLevel == "High", cancellationToken);
    }

    // ===== Sanctions Screening Operations =====
    public async Task<SanctionsScreening> GetScreeningByIdAsync(Guid screeningId, CancellationToken cancellationToken = default)
    {
        return await _context.SanctionsScreenings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == screeningId, cancellationToken);
    }

    public async Task<List<SanctionsScreening>> SearchScreeningsAsync(string partyName, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.SanctionsScreenings.AsNoTracking();

        if (!string.IsNullOrEmpty(partyName))
            query = query.Where(s => EF.Functions.ILike(s.PartyName, $"%{partyName}%"));

        if (!string.IsNullOrEmpty(status))
            query = query.Where(s => s.Status == status);

        return await query
            .OrderByDescending(s => s.ScreenedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<SanctionsScreening> AddScreeningAsync(SanctionsScreening screening, CancellationToken cancellationToken = default)
    {
        await _context.SanctionsScreenings.AddAsync(screening, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return screening;
    }

    public async Task<SanctionsScreening> UpdateScreeningAsync(SanctionsScreening screening, CancellationToken cancellationToken = default)
    {
        _context.SanctionsScreenings.Update(screening);
        await _context.SaveChangesAsync(cancellationToken);
        return screening;
    }

    // ===== Transaction Monitoring Operations =====
    public async Task<TransactionMonitoring> GetMonitoringAlertByIdAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitoringAlerts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == alertId, cancellationToken);
    }

    public async Task<List<TransactionMonitoring>> SearchMonitoringAlertsAsync(string status, string severity, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.TransactionMonitoringAlerts.AsNoTracking();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(a => a.Status == status);

        if (!string.IsNullOrEmpty(severity))
            query = query.Where(a => a.Severity == severity);

        return await query
            .OrderByDescending(a => a.AlertedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<TransactionMonitoring> AddMonitoringAlertAsync(TransactionMonitoring alert, CancellationToken cancellationToken = default)
    {
        await _context.TransactionMonitoringAlerts.AddAsync(alert, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return alert;
    }

    public async Task<TransactionMonitoring> UpdateMonitoringAlertAsync(TransactionMonitoring alert, CancellationToken cancellationToken = default)
    {
        _context.TransactionMonitoringAlerts.Update(alert);
        await _context.SaveChangesAsync(cancellationToken);
        return alert;
    }

    public async Task<int> GetOpenMonitoringAlertCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitoringAlerts.CountAsync(a => a.Status == "Open", cancellationToken);
    }

    public async Task<int> GetCriticalMonitoringAlertCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TransactionMonitoringAlerts.CountAsync(a => a.Severity == "Critical", cancellationToken);
    }

    // ===== KYC Verification Operations =====
    public async Task<KYCVerification> GetKYCByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.KYCVerifications
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.CustomerId == customerId, cancellationToken);
    }

    public async Task<List<KYCVerification>> SearchKYCAsync(string status, string riskLevel, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.KYCVerifications.AsNoTracking();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(k => k.Status == status);

        if (!string.IsNullOrEmpty(riskLevel))
            query = query.Where(k => k.RiskLevel == riskLevel);

        return await query
            .OrderByDescending(k => k.ApprovedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<KYCVerification>> GetExpiringKYCAsync(int daysUntilExpiry, CancellationToken cancellationToken = default)
    {
        var expiryDate = DateTime.UtcNow.AddDays(daysUntilExpiry);
        return await _context.KYCVerifications
            .AsNoTracking()
            .Where(k => k.ExpiresAt.HasValue && k.ExpiresAt <= expiryDate && k.Status == "Approved")
            .OrderBy(k => k.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<KYCVerification> AddKYCAsync(KYCVerification kyc, CancellationToken cancellationToken = default)
    {
        await _context.KYCVerifications.AddAsync(kyc, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return kyc;
    }

    public async Task<KYCVerification> UpdateKYCAsync(KYCVerification kyc, CancellationToken cancellationToken = default)
    {
        _context.KYCVerifications.Update(kyc);
        await _context.SaveChangesAsync(cancellationToken);
        return kyc;
    }

    // ===== Regulatory Report Operations =====
    public async Task<RegulatoryReport> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        return await _context.RegulatoryReports
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);
    }

    public async Task<List<RegulatoryReport>> SearchReportsAsync(string reportType, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.RegulatoryReports.AsNoTracking();

        if (!string.IsNullOrEmpty(reportType))
            query = query.Where(r => r.ReportType == reportType);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        return await query
            .OrderByDescending(r => r.GeneratedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<RegulatoryReport> AddReportAsync(RegulatoryReport report, CancellationToken cancellationToken = default)
    {
        await _context.RegulatoryReports.AddAsync(report, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return report;
    }

    public async Task<RegulatoryReport> UpdateReportAsync(RegulatoryReport report, CancellationToken cancellationToken = default)
    {
        _context.RegulatoryReports.Update(report);
        await _context.SaveChangesAsync(cancellationToken);
        return report;
    }

    public async Task<int> GetPendingReportCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RegulatoryReports.CountAsync(r => r.Status == "Pending", cancellationToken);
    }
}

// Placeholder domain entities (actual models would be in Domain layer)
public class AMLCase { public Guid Id { get; set; } public Guid? PartyId { get; set; } public string CaseNumber { get; set; } public string Status { get; set; } public string RiskLevel { get; set; } public DateTime CreatedAt { get; set; } }
public class SanctionsScreening { public Guid Id { get; set; } public Guid PartyId { get; set; } public string PartyName { get; set; } public string Status { get; set; } public DateTime ScreenedAt { get; set; } }
public class TransactionMonitoring { public Guid Id { get; set; } public Guid TransactionId { get; set; } public string Status { get; set; } public string Severity { get; set; } public DateTime AlertedAt { get; set; } }
public class KYCVerification { public Guid Id { get; set; } public Guid CustomerId { get; set; } public string Status { get; set; } public string RiskLevel { get; set; } public DateTime? ApprovedAt { get; set; } public DateTime? ExpiresAt { get; set; } }
public class RegulatoryReport { public Guid Id { get; set; } public string ReportType { get; set; } public string Status { get; set; } public DateTime GeneratedAt { get; set; } }
