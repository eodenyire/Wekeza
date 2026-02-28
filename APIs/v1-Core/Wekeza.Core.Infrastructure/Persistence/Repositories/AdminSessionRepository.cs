using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for AdminSession aggregate
/// </summary>
public class AdminSessionRepository
{
    private readonly ApplicationDbContext _context;

    public AdminSessionRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AdminSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AdminSessions
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<AdminSession?> GetBySessionTokenAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        return await _context.AdminSessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken, cancellationToken);
    }

    public async Task<AdminSession?> GetUserActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AdminSessions
            .Where(s => s.UserId == userId && s.Status == AdminSessionStatus.Active)
            .OrderByDescending(s => s.LoginAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AdminSession>> GetUserActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AdminSessions
            .Where(s => s.UserId == userId && s.Status == AdminSessionStatus.Active)
            .OrderByDescending(s => s.LoginAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AdminSession>> GetAllActiveSessionsAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        return await _context.AdminSessions
            .Where(s => s.Status == AdminSessionStatus.Active)
            .OrderByDescending(s => s.LastActivityAt ?? s.LoginAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveSessionCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AdminSessions
            .CountAsync(s => s.Status == AdminSessionStatus.Active, cancellationToken);
    }

    public async Task AddAsync(AdminSession session, CancellationToken cancellationToken = default)
    {
        await _context.AdminSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AdminSession session, CancellationToken cancellationToken = default)
    {
        _context.AdminSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await GetByIdAsync(id, cancellationToken);
        if (session != null)
        {
            _context.AdminSessions.Remove(session);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<AdminSession>> GetSuspiciousSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AdminSessions
            .Where(s => s.Status == AdminSessionStatus.Suspicious || s.RiskLevel == "High" || s.RiskLevel == "Critical")
            .OrderByDescending(s => s.LoginAt)
            .ToListAsync(cancellationToken);
    }

    public async Task TerminateAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _context.AdminSessions
            .Where(s => s.UserId == userId && s.Status == AdminSessionStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Terminate("Terminated by admin");
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
