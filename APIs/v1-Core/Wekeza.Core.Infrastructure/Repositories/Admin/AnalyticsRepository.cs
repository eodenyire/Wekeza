using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Infrastructure.Persistence;
using SavedAnalysisEntity = Wekeza.Core.Infrastructure.Persistence.Configurations.SavedAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Infrastructure.Repositories.Admin;

public class AnalyticsRepository
{
    private readonly ApplicationDbContext _context;

    public AnalyticsRepository(ApplicationDbContext context) => _context = context;

    public async Task<CustomDashboard?> GetCustomDashboardByIdAsync(Guid dashboardId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomDashboards.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dashboardId, cancellationToken);
    }

    public async Task<List<CustomDashboard>> GetUserDashboardsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomDashboards.AsNoTracking()
            .Where(d => d.UserId == userId)
            .OrderBy(d => d.DashboardName)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomDashboard> AddCustomDashboardAsync(CustomDashboard dashboard, CancellationToken cancellationToken = default)
    {
        await _context.CustomDashboards.AddAsync(dashboard, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return dashboard;
    }

    public async Task<CustomDashboard> UpdateCustomDashboardAsync(CustomDashboard dashboard, CancellationToken cancellationToken = default)
    {
        _context.CustomDashboards.Update(dashboard);
        await _context.SaveChangesAsync(cancellationToken);
        return dashboard;
    }

    public async Task DeleteCustomDashboardAsync(Guid dashboardId, CancellationToken cancellationToken = default)
    {
        var dashboard = await _context.CustomDashboards.FirstOrDefaultAsync(d => d.Id == dashboardId, cancellationToken);
        if (dashboard != null)
        {
            _context.CustomDashboards.Remove(dashboard);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<KPIDefinition?> GetKPIByIdAsync(Guid kpiId, CancellationToken cancellationToken = default)
    {
        return await _context.KPIDefinitions.AsNoTracking().FirstOrDefaultAsync(k => k.Id == kpiId, cancellationToken);
    }

    public async Task<List<KPIDefinition>> GetAllKPIsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.KPIDefinitions.AsNoTracking().OrderBy(k => k.KPIName).ToListAsync(cancellationToken);
    }

    public async Task<KPIDefinition> AddKPIAsync(KPIDefinition kpi, CancellationToken cancellationToken = default)
    {
        await _context.KPIDefinitions.AddAsync(kpi, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return kpi;
    }

    public async Task<KPIDefinition> UpdateKPIAsync(KPIDefinition kpi, CancellationToken cancellationToken = default)
    {
        _context.KPIDefinitions.Update(kpi);
        await _context.SaveChangesAsync(cancellationToken);
        return kpi;
    }

    public async Task<Report?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports.AsNoTracking().FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);
    }

    public async Task<List<Report>> SearchReportsAsync(string reportType, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Reports.AsNoTracking();
        if (!string.IsNullOrEmpty(reportType) && Enum.TryParse<ReportType>(reportType, true, out var parsedType))
        {
            query = query.Where(r => r.ReportType == parsedType);
        }

        return await query.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<Report> AddReportAsync(Report report, CancellationToken cancellationToken = default)
    {
        await _context.Reports.AddAsync(report, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return report;
    }

    public async Task<Report> UpdateReportAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Update(report);
        await _context.SaveChangesAsync(cancellationToken);
        return report;
    }

    public async Task<SavedAnalysisEntity?> GetSavedAnalysisByIdAsync(Guid analysisId, CancellationToken cancellationToken = default)
    {
        return await _context.SavedAnalyses.AsNoTracking().FirstOrDefaultAsync(a => a.Id == analysisId, cancellationToken);
    }

    public async Task<List<SavedAnalysisEntity>> GetUserSavedAnalysesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.SavedAnalyses.AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.SavedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<SavedAnalysisEntity> AddSavedAnalysisAsync(SavedAnalysisEntity analysis, CancellationToken cancellationToken = default)
    {
        await _context.SavedAnalyses.AddAsync(analysis, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return analysis;
    }

    public async Task<int> GetActiveCustomersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers.CountAsync(c => c.IsActive, cancellationToken);
    }

    public async Task<decimal> GetTotalDepositsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Where(a => a.Status == AccountStatus.Active && a.AccountType == "Deposit")
            .SumAsync(a => a.Balance.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalLoansAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Where(a => a.Status == AccountStatus.Active && a.AccountType == "Loan")
            .SumAsync(a => a.Balance.Amount, cancellationToken);
    }
}
