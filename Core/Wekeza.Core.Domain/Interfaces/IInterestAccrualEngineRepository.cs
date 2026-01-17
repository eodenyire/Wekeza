using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Interest Accrual Engine operations
/// </summary>
public interface IInterestAccrualEngineRepository
{
    Task<InterestAccrualEngine?> GetByIdAsync(Guid id);
    Task<InterestAccrualEngine?> GetByProcessingDateAsync(DateTime processingDate);
    Task<IEnumerable<InterestAccrualEngine>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<InterestAccrualEngine>> GetFailedAccrualsAsync();
    Task<InterestAccrualEngine?> GetLatestAccrualAsync();
    Task<IEnumerable<InterestAccrualEngine>> GetAccrualHistoryAsync(int pageSize = 50, int pageNumber = 1);
    Task AddAsync(InterestAccrualEngine accrualEngine);
    Task UpdateAsync(InterestAccrualEngine accrualEngine);
    Task DeleteAsync(InterestAccrualEngine accrualEngine);
}