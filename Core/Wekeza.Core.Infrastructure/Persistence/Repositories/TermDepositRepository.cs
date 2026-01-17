using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TermDeposit aggregate
/// </summary>
public class TermDepositRepository : ITermDepositRepository
{
    private readonly ApplicationDbContext _context;

    public TermDepositRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TermDeposit?> GetByIdAsync(Guid id)
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Include(td => td.Account)
            .Include(td => td.Customer)
            .FirstOrDefaultAsync(td => td.Id == id);
    }

    public async Task<TermDeposit?> GetByDepositNumberAsync(string depositNumber)
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Include(td => td.Account)
            .Include(td => td.Customer)
            .FirstOrDefaultAsync(td => td.DepositNumber == depositNumber);
    }

    public async Task<IEnumerable<TermDeposit>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Include(td => td.Account)
            .Where(td => td.CustomerId == customerId)
            .OrderByDescending(td => td.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TermDeposit>> GetByAccountIdAsync(Guid accountId)
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Where(td => td.AccountId == accountId)
            .OrderByDescending(td => td.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TermDeposit>> GetByStatusAsync(DepositStatus status)
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Include(td => td.Account)
            .Include(td => td.Customer)
            .Where(td => td.Status == status)
            .OrderByDescending(td => td.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TermDeposit>> GetMaturingDepositsAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Include(td => td.Account)
            .Include(td => td.Customer)
            .Where(td => td.Status == DepositStatus.Active && 
                        td.MaturityDate >= fromDate && 
                        td.MaturityDate <= toDate)
            .OrderBy(td => td.MaturityDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TermDeposit>> GetByBranchCodeAsync(string branchCode)
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Include(td => td.Account)
            .Include(td => td.Customer)
            .Where(td => td.BranchCode == branchCode)
            .OrderByDescending(td => td.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TermDeposit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Include(td => td.Account)
            .Include(td => td.Customer)
            .Where(td => td.CreatedAt >= startDate && td.CreatedAt <= endDate)
            .OrderByDescending(td => td.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(TermDeposit termDeposit)
    {
        await _context.TermDeposits.AddAsync(termDeposit);
    }

    public async Task UpdateAsync(TermDeposit termDeposit)
    {
        _context.TermDeposits.Update(termDeposit);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var termDeposit = await _context.TermDeposits.FindAsync(id);
        if (termDeposit != null)
        {
            _context.TermDeposits.Remove(termDeposit);
        }
    }

    public async Task<bool> ExistsAsync(string depositNumber)
    {
        return await _context.TermDeposits
            .AnyAsync(td => td.DepositNumber == depositNumber);
    }

    public async Task<decimal> GetTotalDepositsByCustomerAsync(Guid customerId)
    {
        return await _context.TermDeposits
            .Where(td => td.CustomerId == customerId && td.Status == DepositStatus.Active)
            .SumAsync(td => td.PrincipalAmount.Amount);
    }

    public async Task<IEnumerable<TermDeposit>> GetRenewableDepositsAsync()
    {
        return await _context.TermDeposits
            .Include(td => td.Transactions)
            .Include(td => td.Account)
            .Include(td => td.Customer)
            .Where(td => td.Status == DepositStatus.Matured && td.AutoRenewal)
            .OrderBy(td => td.MaturityDate)
            .ToListAsync();
    }
}