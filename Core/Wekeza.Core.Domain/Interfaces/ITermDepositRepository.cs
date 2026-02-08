using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for TermDeposit aggregate
/// </summary>
public interface ITermDepositRepository
{
    Task<TermDeposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TermDeposit?> GetByDepositNumberAsync(string depositNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermDeposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermDeposit>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermDeposit>> GetByStatusAsync(DepositStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermDeposit>> GetMaturingDepositsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermDeposit>> GetByBranchCodeAsync(string branchCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermDeposit>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(TermDeposit termDeposit, CancellationToken cancellationToken = default);
    Task UpdateAsync(TermDeposit termDeposit, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string depositNumber, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalDepositsByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TermDeposit>> GetRenewableDepositsAsync(CancellationToken cancellationToken = default);
}