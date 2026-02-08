using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Fixed Deposit operations
/// </summary>
public interface IFixedDepositRepository
{
    Task<FixedDeposit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FixedDeposit?> GetByDepositNumberAsync(string depositNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<FixedDeposit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FixedDeposit>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FixedDeposit>> GetActiveDepositsAsync(string? branchCode = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<FixedDeposit>> GetMaturedDepositsAsync(DateTime? asOfDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<FixedDeposit>> GetDepositsForRenewalAsync(DateTime renewalDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<FixedDeposit>> GetDepositsByBranchAsync(string branchCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<FixedDeposit>> GetDepositsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task AddAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default);
    Task UpdateAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default);
    Task DeleteAsync(FixedDeposit fixedDeposit, CancellationToken cancellationToken = default);
}