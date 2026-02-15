using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Fixed Deposit operations
/// </summary>
public interface IFixedDepositRepository
{
    Task<FixedDeposit?> GetByIdAsync(Guid id);
    Task<FixedDeposit?> GetByDepositNumberAsync(string depositNumber);
    Task<IEnumerable<FixedDeposit>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<FixedDeposit>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<FixedDeposit>> GetActiveDepositsAsync(string? branchCode = null);
    Task<IEnumerable<FixedDeposit>> GetMaturedDepositsAsync(DateTime? asOfDate = null);
    Task<IEnumerable<FixedDeposit>> GetDepositsForRenewalAsync(DateTime renewalDate);
    Task<IEnumerable<FixedDeposit>> GetDepositsByBranchAsync(string branchCode);
    Task<IEnumerable<FixedDeposit>> GetDepositsByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task AddAsync(FixedDeposit fixedDeposit);
    Task UpdateAsync(FixedDeposit fixedDeposit);
    Task DeleteAsync(FixedDeposit fixedDeposit);
}