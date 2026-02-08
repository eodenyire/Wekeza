using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Interest Accrual Engine operations
/// </summary>
public interface IInterestAccrualEngineRepository
{
    Task<InterestAccrualEngine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InterestAccrualEngine?> GetByProcessingDateAsync(DateTime processingDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<InterestAccrualEngine>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<InterestAccrualEngine>> GetFailedAccrualsAsync(CancellationToken cancellationToken = default);
    Task<InterestAccrualEngine?> GetLatestAccrualAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InterestAccrualEngine>> GetAccrualHistoryAsync(int pageSize = 50, int pageNumber = 1, CancellationToken cancellationToken = default);
    Task AddAsync(InterestAccrualEngine accrualEngine, CancellationToken cancellationToken = default);
    Task UpdateAsync(InterestAccrualEngine accrualEngine, CancellationToken cancellationToken = default);
    Task DeleteAsync(InterestAccrualEngine accrualEngine, CancellationToken cancellationToken = default);
}