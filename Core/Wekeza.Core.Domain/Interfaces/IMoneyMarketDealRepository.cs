using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

public interface IMoneyMarketDealRepository
{
    Task<MoneyMarketDeal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MoneyMarketDeal?> GetByDealNumberAsync(string dealNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<MoneyMarketDeal>> GetByCounterpartyIdAsync(Guid counterpartyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MoneyMarketDeal>> GetByTraderIdAsync(string traderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MoneyMarketDeal>> GetByStatusAsync(DealStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<MoneyMarketDeal>> GetByDealTypeAsync(MoneyMarketDealType dealType, CancellationToken cancellationToken = default);
    Task<IEnumerable<MoneyMarketDeal>> GetMaturingDealsAsync(DateTime maturityDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<MoneyMarketDeal>> GetActiveDealsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MoneyMarketDeal>> GetDealsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalExposureAsync(Guid counterpartyId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalExposureByCurrencyAsync(string currency, CancellationToken cancellationToken = default);
    Task<int> GetDealCountByStatusAsync(DealStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(MoneyMarketDeal deal, CancellationToken cancellationToken = default);
    Task UpdateAsync(MoneyMarketDeal deal, CancellationToken cancellationToken = default);
    Task DeleteAsync(MoneyMarketDeal deal, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string dealNumber, CancellationToken cancellationToken = default);
}