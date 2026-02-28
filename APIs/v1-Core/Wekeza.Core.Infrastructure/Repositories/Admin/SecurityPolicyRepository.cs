using Wekeza.Core.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Infrastructure.Repositories.Admin;

public class SecurityPolicyRepository
{
    private readonly ApplicationDbContext _context;

    public SecurityPolicyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ===== User Access Operations =====
    public async Task<UserAccess> GetUserAccessByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccess
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }

    public async Task<List<UserAccess>> GetAllUserAccessAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccess
            .AsNoTracking()
            .OrderByDescending(u => u.LastAccessAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAccess> AddUserAccessAsync(UserAccess userAccess, CancellationToken cancellationToken = default)
    {
        await _context.UserAccess.AddAsync(userAccess, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return userAccess;
    }

    public async Task<UserAccess> UpdateUserAccessAsync(UserAccess userAccess, CancellationToken cancellationToken = default)
    {
        _context.UserAccess.Update(userAccess);
        await _context.SaveChangesAsync(cancellationToken);
        return userAccess;
    }

    // ===== Security Policy Operations =====
    public async Task<SecurityPolicy> GetPolicyByIdAsync(Guid policyId, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityPolicies
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == policyId, cancellationToken);
    }

    public async Task<List<SecurityPolicy>> GetAllPoliciesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityPolicies
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<SecurityPolicy> AddPolicyAsync(SecurityPolicy policy, CancellationToken cancellationToken = default)
    {
        await _context.SecurityPolicies.AddAsync(policy, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return policy;
    }

    public async Task<SecurityPolicy> UpdatePolicyAsync(SecurityPolicy policy, CancellationToken cancellationToken = default)
    {
        _context.SecurityPolicies.Update(policy);
        await _context.SaveChangesAsync(cancellationToken);
        return policy;
    }

    public async Task<int> GetActivePoliciesCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityPolicies.CountAsync(p => p.Status == "Active", cancellationToken);
    }

    // ===== Security Incident Operations =====
    public async Task<SecurityIncident> GetIncidentByIdAsync(Guid incidentId, CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == incidentId, cancellationToken);
    }

    public async Task<List<SecurityIncident>> SearchIncidentsAsync(string severity, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.SecurityIncidents.AsNoTracking();

        if (!string.IsNullOrEmpty(severity))
            query = query.Where(i => i.Severity == severity);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(i => i.Status == status);

        return await query
            .OrderByDescending(i => i.ReportedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<SecurityIncident> AddIncidentAsync(SecurityIncident incident, CancellationToken cancellationToken = default)
    {
        await _context.SecurityIncidents.AddAsync(incident, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return incident;
    }

    public async Task<SecurityIncident> UpdateIncidentAsync(SecurityIncident incident, CancellationToken cancellationToken = default)
    {
        _context.SecurityIncidents.Update(incident);
        await _context.SaveChangesAsync(cancellationToken);
        return incident;
    }

    public async Task<int> GetOpenIncidentsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents.CountAsync(i => i.Status == "Open", cancellationToken);
    }

    public async Task<int> GetCriticalIncidentsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecurityIncidents.CountAsync(i => i.Severity == "Critical", cancellationToken);
    }

    // ===== Security Session Operations =====
    public async Task<SecuritySession> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.SecuritySessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
    }

    public async Task<List<SecuritySession>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.SecuritySessions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.LoginAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SecuritySession>> GetAllActiveSessionsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.SecuritySessions
            .AsNoTracking()
            .Where(s => s.Status == "Active")
            .OrderByDescending(s => s.LoginAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<SecuritySession> UpdateSessionAsync(SecuritySession session, CancellationToken cancellationToken = default)
    {
        _context.SecuritySessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task<int> GetActiveSessionsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecuritySessions.CountAsync(s => s.Status == "Active", cancellationToken);
    }

    public async Task<List<SecuritySession>> GetSuspiciousSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SecuritySessions
            .AsNoTracking()
            .Where(s => s.Status == "Active" && s.LastActivity < DateTime.UtcNow.AddHours(-4))
            .ToListAsync(cancellationToken);
    }

    // ===== Policy Violation Operations =====
    public async Task<PolicyViolation> GetViolationByIdAsync(Guid violationId, CancellationToken cancellationToken = default)
    {
        return await _context.PolicyViolations
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == violationId, cancellationToken);
    }

    public async Task<List<PolicyViolation>> GetViolationsByPolicyAsync(Guid policyId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = _context.PolicyViolations.AsNoTracking()
            .Where(v => v.PolicyId == policyId);

        if (fromDate.HasValue)
            query = query.Where(v => v.DetectedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(v => v.DetectedAt <= toDate.Value);

        return await query
            .OrderByDescending(v => v.DetectedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PolicyViolation> AddViolationAsync(PolicyViolation violation, CancellationToken cancellationToken = default)
    {
        await _context.PolicyViolations.AddAsync(violation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return violation;
    }

    public async Task<int> GetViolationCountAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = _context.PolicyViolations.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(v => v.DetectedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(v => v.DetectedAt <= toDate.Value);

        return await query.CountAsync(cancellationToken);
    }
}

// Placeholder domain entities
public class UserAccess { public Guid UserId { get; set; } public string UserName { get; set; } public DateTime LastAccessAt { get; set; } public string AccessStatus { get; set; } }
public class SecurityPolicy { public Guid Id { get; set; } public string PolicyCode { get; set; } public string Status { get; set; } public DateTime CreatedAt { get; set; } }
public class SecurityIncident { public Guid Id { get; set; } public string Severity { get; set; } public string Status { get; set; } public DateTime ReportedAt { get; set; } }
public class SecuritySession { public Guid Id { get; set; } public Guid UserId { get; set; } public string Status { get; set; } public DateTime LoginAt { get; set; } public DateTime LastActivity { get; set; } }
public class PolicyViolation { public Guid Id { get; set; } public Guid PolicyId { get; set; } public DateTime DetectedAt { get; set; } }
