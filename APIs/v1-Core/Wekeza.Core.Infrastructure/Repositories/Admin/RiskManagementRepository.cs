using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Infrastructure.Repositories.Admin;

public class RiskManagementRepository
{
    private readonly ApplicationDbContext _context;

    public RiskManagementRepository(ApplicationDbContext context) => _context = context;

    public async Task<LimitDefinition> GetLimitByIdAsync(Guid limitId, CancellationToken cancellationToken = default)
    {
        return await _context.LimitDefinitions.AsNoTracking().FirstOrDefaultAsync(l => l.Id == limitId, cancellationToken);
    }

    public async Task<List<LimitDefinition>> SearchLimitsAsync(string limitType, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.LimitDefinitions.AsNoTracking();
        if (!string.IsNullOrEmpty(limitType)) query = query.Where(l => l.LimitType == limitType);
        if (!string.IsNullOrEmpty(status)) query = query.Where(l => l.Status == status);
        return await query.OrderBy(l => l.LimitType).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<LimitDefinition> AddLimitAsync(LimitDefinition limit, CancellationToken cancellationToken = default)
    {
        await _context.LimitDefinitions.AddAsync(limit, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return limit;
    }

    public async Task<LimitDefinition> UpdateLimitAsync(LimitDefinition limit, CancellationToken cancellationToken = default)
    {
        _context.LimitDefinitions.Update(limit);
        await _context.SaveChangesAsync(cancellationToken);
        return limit;
    }

    // NOTE: ThresholdConfig aggregate not yet created - methods disabled until domain model is implemented
    /*
    public async Task<ThresholdConfig> GetThresholdByIdAsync(Guid thresholdId, CancellationToken cancellationToken = default)
    {
        return await _context.ThresholdConfigs.AsNoTracking().FirstOrDefaultAsync(t => t.Id == thresholdId, cancellationToken);
    }

    public async Task<List<ThresholdConfig>> SearchThresholdsAsync(string thresholdType, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ThresholdConfigs.AsNoTracking();
        if (!string.IsNullOrEmpty(thresholdType)) query = query.Where(t => t.ThresholdType == thresholdType);
        return await query.OrderBy(t => t.ThresholdType).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<ThresholdConfig> AddThresholdAsync(ThresholdConfig threshold, CancellationToken cancellationToken = default)
    {
        await _context.ThresholdConfigs.AddAsync(threshold, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return threshold;
    }

    public async Task<ThresholdConfig> UpdateThresholdAsync(ThresholdConfig threshold, CancellationToken cancellationToken = default)
    {
        _context.ThresholdConfigs.Update(threshold);
        await _context.SaveChangesAsync(cancellationToken);
        return threshold;
    }
    */

    // NOTE: Anomaly aggregate not yet created - methods disabled until domain model is implemented
    /*
    public async Task<Anomaly> GetAnomalyByIdAsync(Guid anomalyId, CancellationToken cancellationToken = default)
    {
        return await _context.Anomalies.AsNoTracking().FirstOrDefaultAsync(a => a.Id == anomalyId, cancellationToken);
    }

    public async Task<List<Anomaly>> SearchAnomaliesAsync(string severity, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Anomalies.AsNoTracking();
        if (!string.IsNullOrEmpty(severity)) query = query.Where(a => a.Severity == severity);
        if (!string.IsNullOrEmpty(status)) query = query.Where(a => a.Status == status);
        return await query.OrderByDescending(a => a.DetectedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<Anomaly> AddAnomalyAsync(Anomaly anomaly, CancellationToken cancellationToken = default)
    {
        await _context.Anomalies.AddAsync(anomaly, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return anomaly;
    }

    public async Task<Anomaly> UpdateAnomalyAsync(Anomaly anomaly, CancellationToken cancellationToken = default)
    {
        _context.Anomalies.Update(anomaly);
        await _context.SaveChangesAsync(cancellationToken);
        return anomaly;
    }

    public async Task<int> GetOpenAnomaliesCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Anomalies.CountAsync(a => a.Status == "Open", cancellationToken);
    }

    public async Task<AnomalyRule> GetAnomalyRuleByIdAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        return await _context.AnomalyRules.AsNoTracking().FirstOrDefaultAsync(r => r.Id == ruleId, cancellationToken);
    }

    public async Task<List<AnomalyRule>> GetAllAnomalyRulesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AnomalyRules.AsNoTracking().OrderBy(r => r.RuleName).ToListAsync(cancellationToken);
    }

    public async Task<AnomalyRule> AddAnomalyRuleAsync(AnomalyRule rule, CancellationToken cancellationToken = default)
    {
        await _context.AnomalyRules.AddAsync(rule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return rule;
    }
    */

    public async Task<int> GetCriticalLimitBreachesCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LimitDefinitions.CountAsync(l => l.Status == "Breached", cancellationToken);
    }
    
    // NOTE: ThresholdConfig, Anomaly, and AnomalyRule aggregates not yet created in Domain layer
    // Methods using these types are temporarily disabled until domain models are implemented
}
