using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for TermDeposit aggregate
/// </summary>
public interface ITermDepositRepository
{
    Task<TermDeposit?> GetByIdAsync(Guid id);
    Task<TermDeposit?> GetByDepositNumberAsync(string depositNumber);
    Task<IEnumerable<TermDeposit>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<TermDeposit>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<TermDeposit>> GetByStatusAsync(DepositStatus status);
    Task<IEnumerable<TermDeposit>> GetMaturingDepositsAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<TermDeposit>> GetByBranchCodeAsync(string branchCode);
    Task<IEnumerable<TermDeposit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task AddAsync(TermDeposit termDeposit);
    Task UpdateAsync(TermDeposit termDeposit);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(string depositNumber);
    Task<decimal> GetTotalDepositsByCustomerAsync(Guid customerId);
    Task<IEnumerable<TermDeposit>> GetRenewableDepositsAsync();
}