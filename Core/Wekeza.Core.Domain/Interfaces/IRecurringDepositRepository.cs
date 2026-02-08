using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Recurring Deposit operations
/// </summary>
public interface IRecurringDepositRepository
{
    Task<RecurringDeposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RecurringDeposit?> GetByDepositNumberAsync(string depositNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecurringDeposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecurringDeposit>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecurringDeposit>> GetActiveDepositsAsync(string? branchCode = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecurringDeposit>> GetMaturedDepositsAsync(DateTime? asOfDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecurringDeposit>> GetOverdueInstallmentsAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecurringDeposit>> GetAutoDebitDepositsAsync(DateTime processingDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecurringDeposit>> GetDepositsByBranchAsync(string branchCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecurringDeposit>> GetDepositsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task AddAsync(RecurringDeposit recurringDeposit, CancellationToken cancellationToken = default);
    Task UpdateAsync(RecurringDeposit recurringDeposit, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecurringDeposit recurringDeposit, CancellationToken cancellationToken = default);
}