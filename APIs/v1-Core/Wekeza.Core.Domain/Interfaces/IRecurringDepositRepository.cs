using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Recurring Deposit operations
/// </summary>
public interface IRecurringDepositRepository
{
    Task<RecurringDeposit?> GetByIdAsync(Guid id);
    Task<RecurringDeposit?> GetByDepositNumberAsync(string depositNumber);
    Task<IEnumerable<RecurringDeposit>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<RecurringDeposit>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<RecurringDeposit>> GetActiveDepositsAsync(string? branchCode = null);
    Task<IEnumerable<RecurringDeposit>> GetMaturedDepositsAsync(DateTime? asOfDate = null);
    Task<IEnumerable<RecurringDeposit>> GetOverdueInstallmentsAsync(DateTime asOfDate);
    Task<IEnumerable<RecurringDeposit>> GetAutoDebitDepositsAsync(DateTime processingDate);
    Task<IEnumerable<RecurringDeposit>> GetDepositsByBranchAsync(string branchCode);
    Task<IEnumerable<RecurringDeposit>> GetDepositsByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task AddAsync(RecurringDeposit recurringDeposit);
    Task UpdateAsync(RecurringDeposit recurringDeposit);
    Task DeleteAsync(RecurringDeposit recurringDeposit);
}