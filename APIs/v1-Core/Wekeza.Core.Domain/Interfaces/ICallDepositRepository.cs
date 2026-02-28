using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for CallDeposit aggregate
/// </summary>
public interface ICallDepositRepository
{
    Task<CallDeposit?> GetByIdAsync(Guid id);
    Task<CallDeposit?> GetByDepositNumberAsync(string depositNumber);
    Task<IEnumerable<CallDeposit>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<CallDeposit>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<CallDeposit>> GetByStatusAsync(DepositStatus status);
    Task<IEnumerable<CallDeposit>> GetByBranchCodeAsync(string branchCode);
    Task<IEnumerable<CallDeposit>> GetWithPendingNoticesAsync();
    Task<IEnumerable<CallDeposit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<CallDeposit>> GetInstantAccessDepositsAsync();
    Task AddAsync(CallDeposit callDeposit);
    Task UpdateAsync(CallDeposit callDeposit);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string depositNumber);
    Task<decimal> GetTotalBalanceByCustomerAsync(Guid customerId);
    Task<IEnumerable<CallDeposit>> GetDepositsForInterestAccrualAsync();
}