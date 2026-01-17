using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Branch aggregate
/// </summary>
public class BranchRepository : IBranchRepository
{
    private readonly ApplicationDbContext _context;

    public BranchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Branch?> GetByIdAsync(Guid id)
    {
        return await _context.Branches
            .Include(b => b.Vaults)
            .Include(b => b.Limits)
            .Include(b => b.PerformanceMetrics)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Branch?> GetByCodeAsync(string branchCode)
    {
        return await _context.Branches
            .Include(b => b.Vaults)
            .Include(b => b.Limits)
            .Include(b => b.PerformanceMetrics)
            .FirstOrDefaultAsync(b => b.BranchCode == branchCode);
    }

    public async Task<IEnumerable<Branch>> GetAllAsync()
    {
        return await _context.Branches
            .Include(b => b.Vaults)
            .Include(b => b.Limits)
            .OrderBy(b => b.BranchName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Branch>> GetByStatusAsync(BranchStatus status)
    {
        return await _context.Branches
            .Include(b => b.Vaults)
            .Include(b => b.Limits)
            .Where(b => b.Status == status)
            .OrderBy(b => b.BranchName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Branch>> GetByTypeAsync(BranchType branchType)
    {
        return await _context.Branches
            .Include(b => b.Vaults)
            .Include(b => b.Limits)
            .Where(b => b.BranchType == branchType)
            .OrderBy(b => b.BranchName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Branch>> GetByManagerAsync(string managerId)
    {
        return await _context.Branches
            .Include(b => b.Vaults)
            .Include(b => b.Limits)
            .Where(b => b.ManagerId == managerId || b.DeputyManagerId == managerId)
            .OrderBy(b => b.BranchName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Branch>> GetOperationalAsync()
    {
        return await _context.Branches
            .Include(b => b.Vaults)
            .Include(b => b.Limits)
            .Where(b => b.Status == BranchStatus.Active && b.IsBODCompleted && !b.IsEODCompleted)
            .OrderBy(b => b.BranchName)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodeAsync(string branchCode)
    {
        return await _context.Branches.AnyAsync(b => b.BranchCode == branchCode);
    }

    public async Task AddAsync(Branch branch)
    {
        await _context.Branches.AddAsync(branch);
    }

    public async Task UpdateAsync(Branch branch)
    {
        _context.Branches.Update(branch);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var branch = await _context.Branches.FindAsync(id);
        if (branch != null)
        {
            _context.Branches.Remove(branch);
        }
    }
}