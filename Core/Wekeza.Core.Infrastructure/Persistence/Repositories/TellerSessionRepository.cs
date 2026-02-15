using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Teller Session Repository Implementation - Complete teller session management
/// Manages teller sessions with comprehensive querying capabilities
/// </summary>
public class TellerSessionRepository : ITellerSessionRepository
{
    private readonly ApplicationDbContext _context;

    public TellerSessionRepository(ApplicationDbContext context) => _context = context;

    // Basic CRUD operations
    public async Task<TellerSession?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<TellerSession?> GetBySessionIdAsync(string sessionId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId, ct);
    }

    public async Task AddAsync(TellerSession session, CancellationToken ct = default)
    {
        await _context.TellerSessions.AddAsync(session, ct);
    }

    public void Add(TellerSession session)
    {
        _context.TellerSessions.Add(session);
    }

    public void Update(TellerSession session)
    {
        _context.TellerSessions.Update(session);
    }

    public void Remove(TellerSession session)
    {
        _context.TellerSessions.Remove(session);
    }

    // Teller-based queries
    public async Task<TellerSession?> GetActiveSessionByTellerIdAsync(Guid tellerId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .FirstOrDefaultAsync(s => s.TellerId == tellerId && s.Status == TellerSessionStatus.Active, ct);
    }

    public async Task<TellerSession?> GetActiveSessionByUserAsync(Guid userId, CancellationToken ct = default)
    {
        // TODO: Implement proper userId to tellerId mapping
        // Currently assuming userId is the same as tellerId
        // This should be updated when user-teller relationship is clarified
        return await _context.TellerSessions
            .FirstOrDefaultAsync(s => s.TellerId == userId && s.Status == TellerSessionStatus.Active, ct);
    }

    public async Task<IEnumerable<TellerSession>> GetSessionsByTellerIdAsync(Guid tellerId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.TellerId == tellerId)
            .OrderByDescending(s => s.SessionStartTime)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerSession>> GetSessionsByTellerCodeAsync(string tellerCode, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.TellerCode == tellerCode)
            .OrderByDescending(s => s.SessionStartTime)
            .ToListAsync(ct);
    }

    // Branch-based queries
    public async Task<IEnumerable<TellerSession>> GetActiveSessionsByBranchIdAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.BranchId == branchId && s.Status == TellerSessionStatus.Active)
            .OrderBy(s => s.TellerCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerSession>> GetSessionsByBranchIdAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.BranchId == branchId)
            .OrderByDescending(s => s.SessionStartTime)
            .ToListAsync(ct);
    }

    // Status-based queries
    public async Task<IEnumerable<TellerSession>> GetByStatusAsync(TellerSessionStatus status, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.SessionStartTime)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerSession>> GetActiveSessions(CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.Status == TellerSessionStatus.Active)
            .OrderBy(s => s.BranchCode)
            .ThenBy(s => s.TellerCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerSession>> GetSuspendedSessions(CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.Status == TellerSessionStatus.Suspended)
            .OrderByDescending(s => s.LastModifiedDate)
            .ToListAsync(ct);
    }

    // Date-based queries
    public async Task<IEnumerable<TellerSession>> GetSessionsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.SessionStartTime.Date >= fromDate.Date && s.SessionStartTime.Date <= toDate.Date)
            .OrderByDescending(s => s.SessionStartTime)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerSession>> GetTodaysSessions(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.TellerSessions
            .Where(s => s.SessionStartTime.Date == today)
            .OrderByDescending(s => s.SessionStartTime)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TellerSession>> GetSessionsRequiringReconciliation(CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.Status == TellerSessionStatus.Active && s.CashDifference != null)
            .OrderBy(s => s.SessionStartTime)
            .ToListAsync(ct);
    }

    // Analytics and reporting
    public async Task<int> GetActiveSessionCountAsync(CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .CountAsync(s => s.Status == TellerSessionStatus.Active, ct);
    }

    public async Task<int> GetActiveSessionCountByBranchAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .CountAsync(s => s.BranchId == branchId && s.Status == TellerSessionStatus.Active, ct);
    }

    public async Task<decimal> GetTotalCashInSessionsAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .Where(s => s.BranchId == branchId && s.Status == TellerSessionStatus.Active)
            .SumAsync(s => s.CurrentCashBalance.Amount, ct);
    }

    // Validation helpers
    public async Task<bool> HasActiveSessionAsync(Guid tellerId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .AnyAsync(s => s.TellerId == tellerId && s.Status == TellerSessionStatus.Active, ct);
    }

    public async Task<bool> ExistsBySessionIdAsync(string sessionId, CancellationToken ct = default)
    {
        return await _context.TellerSessions
            .AnyAsync(s => s.SessionId == sessionId, ct);
    }
}