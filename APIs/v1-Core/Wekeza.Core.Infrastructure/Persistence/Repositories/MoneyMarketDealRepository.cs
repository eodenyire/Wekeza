using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class MoneyMarketDealRepository : IMoneyMarketDealRepository
{
    private readonly ApplicationDbContext _context;
    public MoneyMarketDealRepository(ApplicationDbContext context) => _context = context;

    public async Task<MoneyMarketDeal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.FindAsync(new object[] { id }, cancellationToken);

    public async Task<MoneyMarketDeal?> GetByDealNumberAsync(string dealNumber, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.FirstOrDefaultAsync(d => d.DealNumber == dealNumber, cancellationToken);

    public async Task<IEnumerable<MoneyMarketDeal>> GetByCounterpartyIdAsync(Guid counterpartyId, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.Where(d => d.CounterpartyId == counterpartyId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<MoneyMarketDeal>> GetByTraderIdAsync(string traderId, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.Where(d => d.TraderId == traderId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<MoneyMarketDeal>> GetByStatusAsync(DealStatus status, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.Where(d => d.Status == status).ToListAsync(cancellationToken);

    public async Task<IEnumerable<MoneyMarketDeal>> GetByDealTypeAsync(MoneyMarketDealType dealType, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.Where(d => d.DealType == dealType).ToListAsync(cancellationToken);

    public async Task<IEnumerable<MoneyMarketDeal>> GetMaturingDealsAsync(DateTime maturityDate, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.Where(d => d.MaturityDate.Date == maturityDate.Date).ToListAsync(cancellationToken);

    public async Task<IEnumerable<MoneyMarketDeal>> GetActiveDealsAsync(CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.Where(d => d.Status == DealStatus.Booked).ToListAsync(cancellationToken);

    public async Task<IEnumerable<MoneyMarketDeal>> GetDealsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.Where(d => d.CreatedAt >= fromDate && d.CreatedAt <= toDate).ToListAsync(cancellationToken);

    public async Task<decimal> GetTotalExposureAsync(Guid counterpartyId, CancellationToken cancellationToken = default) =>
        await Task.FromResult(0m);

    public async Task<decimal> GetTotalExposureByCurrencyAsync(string currency, CancellationToken cancellationToken = default) =>
        await Task.FromResult(0m);

    public async Task<int> GetDealCountByStatusAsync(DealStatus status, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.CountAsync(d => d.Status == status, cancellationToken);

    public async Task AddAsync(MoneyMarketDeal deal, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.AddAsync(deal, cancellationToken);

    public async Task UpdateAsync(MoneyMarketDeal deal, CancellationToken cancellationToken = default) =>
        _context.MoneyMarketDeals.Update(deal);

    public async Task DeleteAsync(MoneyMarketDeal deal, CancellationToken cancellationToken = default) =>
        _context.MoneyMarketDeals.Remove(deal);

    public async Task<bool> ExistsAsync(string dealNumber, CancellationToken cancellationToken = default) =>
        await _context.MoneyMarketDeals.AnyAsync(d => d.DealNumber == dealNumber, cancellationToken);
}
