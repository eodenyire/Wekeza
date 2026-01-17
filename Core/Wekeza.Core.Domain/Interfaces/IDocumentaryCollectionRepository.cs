using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

public interface IDocumentaryCollectionRepository
{
    Task<DocumentaryCollection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DocumentaryCollection?> GetByCollectionNumberAsync(string collectionNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetByDrawerIdAsync(Guid drawerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetByDraweeIdAsync(Guid draweeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetByRemittingBankIdAsync(Guid remittingBankId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetByCollectingBankIdAsync(Guid collectingBankId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetByStatusAsync(CollectionStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetByTypeAsync(CollectionType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetMaturedCollectionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetOutstandingCollectionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentaryCollection>> GetOverdueCollectionsAsync(int daysOverdue, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalCollectionAmountAsync(Guid drawerId, CancellationToken cancellationToken = default);
    Task<int> GetCollectionCountByStatusAsync(CollectionStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(DocumentaryCollection collection, CancellationToken cancellationToken = default);
    Task UpdateAsync(DocumentaryCollection collection, CancellationToken cancellationToken = default);
    Task DeleteAsync(DocumentaryCollection collection, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string collectionNumber, CancellationToken cancellationToken = default);
}