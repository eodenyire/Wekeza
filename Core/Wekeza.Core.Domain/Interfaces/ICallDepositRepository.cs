using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for CallDeposit aggregate
/// </summary>
public interface ICallDepositRepository
{
    Task<CallDeposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CallDeposit?> GetByDepositNumberAsync(string depositNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<CallDeposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CallDeposit>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CallDeposit>> GetByStatusAsync(DepositStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<CallDeposit>> GetByBranchCodeAsync(string branchCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<CallDeposit>> GetWithPendingNoticesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CallDeposit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<CallDeposit>> GetInstantAccessDepositsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CallDeposit callDeposit, CancellationToken cancellationToken = default);
    Task UpdateAsync(CallDeposit callDeposit, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string depositNumber, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalBalanceByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CallDeposit>> GetDepositsForInterestAccrualAsync(CancellationToken cancellationToken = default);
}