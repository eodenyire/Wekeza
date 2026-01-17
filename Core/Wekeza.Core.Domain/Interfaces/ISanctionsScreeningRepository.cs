using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

public interface ISanctionsScreeningRepository
{
    Task<SanctionsScreening?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetByEntityTypeAsync(EntityType entityType, CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetByStatusAsync(ScreeningStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetWithMatchesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetPendingReviewAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetHighConfidenceMatchesAsync(decimal minScore, CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetByWatchlistAsync(string watchlistName, CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetOverdueReviewsAsync(int daysOverdue, CancellationToken cancellationToken = default);
    Task<IEnumerable<SanctionsScreening>> GetByReviewerAsync(string reviewerId, CancellationToken cancellationToken = default);
    Task<int> GetScreeningCountByStatusAsync(ScreeningStatus status, CancellationToken cancellationToken = default);
    Task<int> GetMatchCountByWatchlistAsync(string watchlistName, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetScreeningStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<decimal> GetAverageMatchScoreAsync(string watchlistName, CancellationToken cancellationToken = default);
    Task AddAsync(SanctionsScreening screening, CancellationToken cancellationToken = default);
    Task UpdateAsync(SanctionsScreening screening, CancellationToken cancellationToken = default);
    Task DeleteAsync(SanctionsScreening screening, CancellationToken cancellationToken = default);
}