using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Cash Drawer Repository Implementation - Complete cash drawer management
/// Manages cash drawers with comprehensive querying capabilities
/// </summary>
public class CashDrawerRepository : ICashDrawerRepository
{
    private readonly ApplicationDbContext _context;

    public CashDrawerRepository(ApplicationDbContext context) => _context = context;

    // Basic CRUD operations
    public async Task<CashDrawer?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task<CashDrawer?> GetByDrawerIdAsync(string drawerId, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .FirstOrDefaultAsync(d => d.DrawerId == drawerId, ct);
    }

    public async Task AddAsync(CashDrawer drawer, CancellationToken ct = default)
    {
        await _context.CashDrawers.AddAsync(drawer, ct);
    }

    public void Add(CashDrawer drawer)
    {
        _context.CashDrawers.Add(drawer);
    }

    public void Update(CashDrawer drawer)
    {
        _context.CashDrawers.Update(drawer);
    }

    public void Remove(CashDrawer drawer)
    {
        _context.CashDrawers.Remove(drawer);
    }

    // Teller-based queries
    public async Task<CashDrawer?> GetByTellerIdAsync(Guid tellerId, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .FirstOrDefaultAsync(d => d.TellerId == tellerId, ct);
    }

    public async Task<IEnumerable<CashDrawer>> GetDrawersByTellerCodeAsync(string tellerCode, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.TellerCode == tellerCode)
            .ToListAsync(ct);
    }

    // Branch-based queries
    public async Task<IEnumerable<CashDrawer>> GetByBranchIdAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.BranchId == branchId)
            .OrderBy(d => d.TellerCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CashDrawer>> GetOpenDrawersByBranchIdAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.BranchId == branchId && d.Status == CashDrawerStatus.Open)
            .OrderBy(d => d.TellerCode)
            .ToListAsync(ct);
    }

    // Status-based queries
    public async Task<IEnumerable<CashDrawer>> GetByStatusAsync(CashDrawerStatus status, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.Status == status)
            .OrderBy(d => d.BranchCode)
            .ThenBy(d => d.TellerCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CashDrawer>> GetOpenDrawersAsync(CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.Status == CashDrawerStatus.Open)
            .OrderBy(d => d.BranchCode)
            .ThenBy(d => d.TellerCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CashDrawer>> GetLockedDrawersAsync(CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.Status == CashDrawerStatus.Locked)
            .OrderByDescending(d => d.LastModifiedDate)
            .ToListAsync(ct);
    }

    // Cash position queries
    public async Task<IEnumerable<CashDrawer>> GetDrawersWithCashAboveLimitAsync(decimal limitAmount, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.Status == CashDrawerStatus.Open && d.TotalCashIn.Amount > limitAmount)
            .OrderByDescending(d => d.TotalCashIn.Amount)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CashDrawer>> GetDrawersWithCashBelowLimitAsync(decimal limitAmount, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.Status == CashDrawerStatus.Open && d.TotalCashIn.Amount < limitAmount)
            .OrderBy(d => d.TotalCashIn.Amount)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CashDrawer>> GetDrawersRequiringReconciliationAsync(CancellationToken ct = default)
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        return await _context.CashDrawers
            .Where(d => d.Status == CashDrawerStatus.Open && 
                       (d.LastReconciliationDate == null || d.LastReconciliationDate < yesterday))
            .OrderBy(d => d.LastReconciliationDate)
            .ToListAsync(ct);
    }

    // Analytics and reporting
    public async Task<decimal> GetTotalCashInBranchAsync(Guid branchId, string currencyCode, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.BranchId == branchId && d.Status == CashDrawerStatus.Open)
            .SumAsync(d => d.TotalCashIn.Amount, ct);
    }

    public async Task<decimal> GetTotalCashInAllDrawersAsync(string currencyCode, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .Where(d => d.Status == CashDrawerStatus.Open)
            .SumAsync(d => d.TotalCashIn.Amount, ct);
    }

    public async Task<int> GetOpenDrawerCountAsync(CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .CountAsync(d => d.Status == CashDrawerStatus.Open, ct);
    }

    public async Task<int> GetOpenDrawerCountByBranchAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .CountAsync(d => d.BranchId == branchId && d.Status == CashDrawerStatus.Open, ct);
    }

    // Validation helpers
    public async Task<bool> ExistsByDrawerIdAsync(string drawerId, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .AnyAsync(d => d.DrawerId == drawerId, ct);
    }

    public async Task<bool> HasOpenDrawerAsync(Guid tellerId, CancellationToken ct = default)
    {
        return await _context.CashDrawers
            .AnyAsync(d => d.TellerId == tellerId && d.Status == CashDrawerStatus.Open, ct);
    }
}