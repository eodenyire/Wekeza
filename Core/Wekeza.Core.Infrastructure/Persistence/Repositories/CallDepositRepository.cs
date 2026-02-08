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

    public async Task<CallDeposit?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .FirstOrDefaultAsync(cd => cd.Id == id, ct);
    }

    public async Task<CallDeposit?> GetByDepositNumberAsync(string depositNumber, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .FirstOrDefaultAsync(cd => cd.DepositNumber == depositNumber, ct);
    }

    public async Task<IEnumerable<CallDeposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Where(cd => cd.CustomerId == customerId)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CallDeposit>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Where(cd => cd.AccountId == accountId)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CallDeposit>> GetByStatusAsync(DepositStatus status, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.Status == status)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CallDeposit>> GetByBranchCodeAsync(string branchCode, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.BranchCode == branchCode)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CallDeposit>> GetWithPendingNoticesAsync(CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.WithdrawalNotices.Any(wn => wn.Status == WithdrawalNoticeStatus.Pending))
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CallDeposit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.CreatedAt >= startDate && cd.CreatedAt <= endDate)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CallDeposit>> GetInstantAccessDepositsAsync(CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Include(cd => cd.WithdrawalNotices)
            .Include(cd => cd.Account)
            .Include(cd => cd.Customer)
            .Where(cd => cd.InstantAccess && cd.Status == DepositStatus.Active)
            .OrderByDescending(cd => cd.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(CallDeposit callDeposit, CancellationToken ct = default)
    {
        await _context.CallDeposits.AddAsync(callDeposit, ct);
    }

    public async Task UpdateAsync(CallDeposit callDeposit, CancellationToken ct = default)
    {
        _context.CallDeposits.Update(callDeposit);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var callDeposit = await _context.CallDeposits.FindAsync(new object[] { id }, ct);
        if (callDeposit != null)
        {
            _context.CallDeposits.Remove(callDeposit);
        }
    }

    public async Task<bool> ExistsAsync(string depositNumber, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .AnyAsync(cd => cd.DepositNumber == depositNumber, ct);
    }

    public async Task<decimal> GetTotalBalanceByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Where(cd => cd.CustomerId == customerId && cd.Status == DepositStatus.Active)
            .SumAsync(cd => cd.Balance.Amount + cd.AccruedInterest.Amount, ct);
    }

    public async Task<IEnumerable<CallDeposit>> GetDepositsForInterestAccrualAsync(CancellationToken ct = default)
    {
        return await _context.CallDeposits
            .Include(cd => cd.Transactions)
            .Where(cd => cd.Status == DepositStatus.Active && cd.Balance.Amount > 0)
            .OrderBy(cd => cd.LastInterestPostingDate)
            .ToListAsync(ct);
    }
}