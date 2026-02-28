using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class FXDealRepository : IFXDealRepository
{
    private readonly ApplicationDbContext _context;
    public FXDealRepository(ApplicationDbContext context) => _context = context;

    public async Task<FXDeal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.FindAsync(new object[] { id }, cancellationToken);

    public async Task<FXDeal?> GetByDealNumberAsync(string dealNumber, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.FirstOrDefaultAsync(d => d.DealNumber == dealNumber, cancellationToken);

    public async Task<IEnumerable<FXDeal>> GetByCounterpartyIdAsync(Guid counterpartyId, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.Where(d => d.CounterpartyId == counterpartyId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<FXDeal>> GetByTraderIdAsync(string traderId, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.Where(d => d.TraderId == traderId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<FXDeal>> GetByStatusAsync(DealStatus status, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.Where(d => d.Status == status).ToListAsync(cancellationToken);

    public async Task<IEnumerable<FXDeal>> GetByDealTypeAsync(FXDealType dealType, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.Where(d => d.DealType == dealType).ToListAsync(cancellationToken);

    public async Task<IEnumerable<FXDeal>> GetByCurrencyPairAsync(string baseCurrency, string quoteCurrency, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.Where(d => d.BaseCurrency == baseCurrency && d.QuoteCurrency == quoteCurrency).ToListAsync(cancellationToken);

    public async Task<IEnumerable<FXDeal>> GetMaturingDealsAsync(DateTime maturityDate, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.Where(d => d.ValueDate.Date == maturityDate.Date).ToListAsync(cancellationToken);

    public async Task<IEnumerable<FXDeal>> GetActiveDealsAsync(CancellationToken cancellationToken = default) =>
        await _context.FXDeals.Where(d => d.Status == DealStatus.Booked).ToListAsync(cancellationToken);

    public async Task<IEnumerable<FXDeal>> GetDealsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.Where(d => d.CreatedAt >= fromDate && d.CreatedAt <= toDate).ToListAsync(cancellationToken);

    public async Task<decimal> GetNetPositionAsync(string currency, CancellationToken cancellationToken = default) =>
        await Task.FromResult(0m);

    public async Task<decimal> GetTotalExposureAsync(Guid counterpartyId, CancellationToken cancellationToken = default) =>
        await Task.FromResult(0m);

    public async Task<Dictionary<string, decimal>> GetCurrencyPositionsAsync(CancellationToken cancellationToken = default) =>
        await Task.FromResult(new Dictionary<string, decimal>());

    public async Task<int> GetDealCountByStatusAsync(DealStatus status, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.CountAsync(d => d.Status == status, cancellationToken);

    public async Task AddAsync(FXDeal deal, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.AddAsync(deal, cancellationToken);

    public async Task UpdateAsync(FXDeal deal, CancellationToken cancellationToken = default) =>
        _context.FXDeals.Update(deal);

    public async Task DeleteAsync(FXDeal deal, CancellationToken cancellationToken = default) =>
        _context.FXDeals.Remove(deal);

    public async Task<bool> ExistsAsync(string dealNumber, CancellationToken cancellationToken = default) =>
        await _context.FXDeals.AnyAsync(d => d.DealNumber == dealNumber, cancellationToken);
}
