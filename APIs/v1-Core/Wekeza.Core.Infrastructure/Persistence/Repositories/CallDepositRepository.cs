using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for CallDeposit aggregate
/// </summary>
public class CallDepositRepository : ICallDepositRepository
{
    private readonly ApplicationDbContext _context;

    public CallDepositRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CallDeposit?> GetByIdAsync(Guid id)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .FirstOrDefaultAsync(cd => cd.Id == id);
    }

    public async Task<CallDeposit?> GetByDepositNumberAsync(string depositNumber)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .FirstOrDefaultAsync(cd => cd.DepositNumber == depositNumber);
    }

    public async Task<IEnumerable<CallDeposit>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Where(cd => cd.CustomerId == customerId)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CallDeposit>> GetByAccountIdAsync(Guid accountId)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Where(cd => cd.AccountId == accountId)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CallDeposit>> GetByStatusAsync(DepositStatus status)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.Status == status)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CallDeposit>> GetByBranchCodeAsync(string branchCode)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.BranchCode == branchCode)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CallDeposit>> GetWithPendingNoticesAsync()
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.WithdrawalNotices.Any(wn => wn.Status == WithdrawalNoticeStatus.Pending))
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CallDeposit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.CreatedAt >= startDate && cd.CreatedAt <= endDate)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CallDeposit>> GetInstantAccessDepositsAsync()
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.InstantAccess && cd.Status == DepositStatus.Active)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(CallDeposit callDeposit)
    {
        await _context.CallDeposits.AddAsync(callDeposit);
    }

    public async Task UpdateAsync(CallDeposit callDeposit)
    {
        _context.CallDeposits.Update(callDeposit);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var callDeposit = await _context.CallDeposits.FindAsync(id);
        if (callDeposit != null)
        {
            _context.CallDeposits.Remove(callDeposit);
        }
    }

    public async Task<bool> ExistsAsync(string depositNumber)
    {
        return await _context.CallDeposits
            .AnyAsync(cd => cd.DepositNumber == depositNumber);
    }

    public async Task<decimal> GetTotalBalanceByCustomerAsync(Guid customerId)
    {
        return await _context.CallDeposits
            .Where(cd => cd.CustomerId == customerId && cd.Status == DepositStatus.Active)
            .SumAsync(cd => cd.Balance.Amount + cd.AccruedInterest.Amount);
    }

    public async Task<IEnumerable<CallDeposit>> GetDepositsForInterestAccrualAsync()
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Where(cd => cd.Status == DepositStatus.Active && cd.Balance.Amount > 0)
            .OrderBy(cd => cd.LastInterestPostingDate)
            .ToListAsync();
    }
}