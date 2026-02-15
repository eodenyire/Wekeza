using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

public interface IFXDealRepository
{
    Task<FXDeal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FXDeal?> GetByDealNumberAsync(string dealNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<FXDeal>> GetByCounterpartyIdAsync(Guid counterpartyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FXDeal>> GetByTraderIdAsync(string traderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FXDeal>> GetByStatusAsync(DealStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<FXDeal>> GetByDealTypeAsync(FXDealType dealType, CancellationToken cancellationToken = default);
    Task<IEnumerable<FXDeal>> GetByCurrencyPairAsync(string baseCurrency, string quoteCurrency, CancellationToken cancellationToken = default);
    Task<IEnumerable<FXDeal>> GetMaturingDealsAsync(DateTime maturityDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<FXDeal>> GetActiveDealsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FXDeal>> GetDealsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<decimal> GetNetPositionAsync(string currency, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalExposureAsync(Guid counterpartyId, CancellationToken cancellationToken = default);
    Task<Dictionary<string, decimal>> GetCurrencyPositionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetDealCountByStatusAsync(DealStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(FXDeal deal, CancellationToken cancellationToken = default);
    Task UpdateAsync(FXDeal deal, CancellationToken cancellationToken = default);
    Task DeleteAsync(FXDeal deal, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string dealNumber, CancellationToken cancellationToken = default);
}