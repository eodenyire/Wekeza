using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

public interface ISecurityDealRepository
{
    Task<SecurityDeal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SecurityDeal?> GetByDealNumberAsync(string dealNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetBySecurityIdAsync(string securityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetByTraderIdAsync(string traderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetByStatusAsync(DealStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetBySecurityTypeAsync(SecurityType securityType, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetByTradeTypeAsync(Wekeza.Core.Domain.Enums.TradeType tradeType, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetByCounterpartyIdAsync(Guid counterpartyId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetSettlingDealsAsync(DateTime settlementDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetActiveDealsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetDealsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecurityDeal>> GetPortfolioHoldingsAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetNetPositionAsync(string securityId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalInvestmentAsync(SecurityType securityType, CancellationToken cancellationToken = default);
    Task<Dictionary<string, decimal>> GetSecurityPositionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetDealCountByStatusAsync(DealStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(SecurityDeal deal, CancellationToken cancellationToken = default);
    Task UpdateAsync(SecurityDeal deal, CancellationToken cancellationToken = default);
    Task DeleteAsync(SecurityDeal deal, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string dealNumber, CancellationToken cancellationToken = default);
}

