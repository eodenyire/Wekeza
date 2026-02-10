using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class SanctionsScreeningRepository : ISanctionsScreeningRepository
{
    private readonly ApplicationDbContext _context;

    public SanctionsScreeningRepository(ApplicationDbContext context) => _context = context;

    public async Task<SanctionsScreening?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.Where(s => s.EntityId == entityId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetByEntityTypeAsync(EntityType entityType, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.Where(s => s.EntityType == entityType).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetByStatusAsync(ScreeningStatus status, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.Where(s => s.Status == status).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetWithMatchesAsync(CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.Where(s => s.Matches.Any()).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetPendingReviewAsync(CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.Where(s => s.Status == ScreeningStatus.UnderReview).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.Where(s => s.ScreeningDate >= fromDate && s.ScreeningDate <= toDate).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetHighConfidenceMatchesAsync(decimal minScore, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.Where(s => s.HighestMatchScore >= minScore).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetByWatchlistAsync(string watchlistName, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetOverdueReviewsAsync(int daysOverdue, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.ToListAsync(cancellationToken);

    public async Task<IEnumerable<SanctionsScreening>> GetByReviewerAsync(string reviewerId, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.Where(s => s.ReviewedBy == reviewerId).ToListAsync(cancellationToken);

    public async Task<int> GetScreeningCountByStatusAsync(ScreeningStatus status, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.CountAsync(s => s.Status == status, cancellationToken);

    public async Task<int> GetMatchCountByWatchlistAsync(string watchlistName, CancellationToken cancellationToken = default) =>
        await Task.FromResult(0);

    public async Task<Dictionary<string, int>> GetScreeningStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default) =>
        await Task.FromResult(new Dictionary<string, int>());

    public async Task<decimal> GetAverageMatchScoreAsync(string watchlistName, CancellationToken cancellationToken = default) =>
        await Task.FromResult(0m);

    public async Task AddAsync(SanctionsScreening screening, CancellationToken cancellationToken = default) =>
        await _context.SanctionsScreenings.AddAsync(screening, cancellationToken);

    public async Task UpdateAsync(SanctionsScreening screening, CancellationToken cancellationToken = default) =>
        _context.SanctionsScreenings.Update(screening);

    public async Task DeleteAsync(SanctionsScreening screening, CancellationToken cancellationToken = default) =>
        _context.SanctionsScreenings.Remove(screening);
}
