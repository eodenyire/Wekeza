using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for FixedDeposit aggregate
/// </summary>
public class FixedDepositRepository : IFixedDepositRepository
{
    private readonly ApplicationDbContext _context;

    public FixedDepositRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FixedDeposit?> GetByIdAsync(Guid id)
    {
        return await _context.FixedDeposits
            .FirstOrDefaultAsync(fd => fd.Id == id);
    }

    public async Task<FixedDeposit?> GetByDepositNumberAsync(string depositNumber)
    {
        return await _context.FixedDeposits
            .FirstOrDefaultAsync(fd => fd.DepositNumber == depositNumber);
    }

    public async Task<IEnumerable<FixedDeposit>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.FixedDeposits
            .Where(fd => fd.CustomerId == customerId)
            .OrderByDescending(fd => fd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<FixedDeposit>> GetByAccountIdAsync(Guid accountId)
    {
        return await _context.FixedDeposits
            .Where(fd => fd.AccountId == accountId)
            .OrderByDescending(fd => fd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<FixedDeposit>> GetActiveDepositsAsync(string? branchCode = null)
    {
        var query = _context.FixedDeposits.AsQueryable();
        
        if (!string.IsNullOrEmpty(branchCode))
        {
            query = query.Where(fd => fd.BranchCode == branchCode);
        }
        
        return await query
            .Where(fd => fd.Status == DepositStatus.Active)
            .OrderByDescending(fd => fd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<FixedDeposit>> GetMaturedDepositsAsync(DateTime? asOfDate = null)
    {
        var date = asOfDate ?? DateTime.UtcNow;
        return await _context.FixedDeposits
            .Where(fd => fd.MaturityDate <= date && fd.Status == DepositStatus.Active)
            .OrderBy(fd => fd.MaturityDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FixedDeposit>> GetDepositsForRenewalAsync(DateTime renewalDate)
    {
        return await _context.FixedDeposits
            .Where(fd => fd.MaturityDate.Date == renewalDate.Date && fd.AutoRenewal)
            .OrderBy(fd => fd.MaturityDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FixedDeposit>> GetDepositsByBranchAsync(string branchCode)
    {
        return await _context.FixedDeposits
            .Where(fd => fd.BranchCode == branchCode)
            .OrderByDescending(fd => fd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<FixedDeposit>> GetDepositsByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.FixedDeposits
            .Where(fd => fd.CreatedAt >= fromDate && fd.CreatedAt <= toDate)
            .OrderByDescending(fd => fd.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(FixedDeposit fixedDeposit)
    {
        await _context.FixedDeposits.AddAsync(fixedDeposit);
    }

    public async Task UpdateAsync(FixedDeposit fixedDeposit)
    {
        _context.FixedDeposits.Update(fixedDeposit);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(FixedDeposit fixedDeposit)
    {
        _context.FixedDeposits.Remove(fixedDeposit);
        await Task.CompletedTask;
    }
}
