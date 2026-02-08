using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class DocumentaryCollectionRepository : IDocumentaryCollectionRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentaryCollectionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentaryCollection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .FirstOrDefaultAsync(dc => dc.Id == id, cancellationToken);
    }

    public async Task<DocumentaryCollection?> GetByCollectionNumberAsync(string collectionNumber, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .FirstOrDefaultAsync(dc => dc.CollectionNumber == collectionNumber, cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByDrawerIdAsync(Guid drawerId, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.DrawerId == drawerId)
            .OrderByDescending(dc => dc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByDraweeIdAsync(Guid draweeId, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.DraweeId == draweeId)
            .OrderByDescending(dc => dc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByRemittingBankIdAsync(Guid remittingBankId, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.RemittingBankId == remittingBankId)
            .OrderByDescending(dc => dc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByCollectingBankIdAsync(Guid collectingBankId, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.CollectingBankId == collectingBankId)
            .OrderByDescending(dc => dc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByStatusAsync(CollectionStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.Status == status)
            .OrderByDescending(dc => dc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByTypeAsync(CollectionType type, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.Type == type)
            .OrderByDescending(dc => dc.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.CollectionDate >= fromDate && dc.CollectionDate <= toDate)
            .OrderBy(dc => dc.CollectionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetMaturedCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.DocumentaryCollections
            .Where(dc => dc.MaturityDate.HasValue && dc.MaturityDate.Value.Date <= today && dc.Status == CollectionStatus.PresentedToDrawee)
            .OrderBy(dc => dc.MaturityDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetOutstandingCollectionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.Status == CollectionStatus.PresentedToDrawee)
            .OrderBy(dc => dc.MaturityDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetOverdueCollectionsAsync(int daysOverdue, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.Date.AddDays(-daysOverdue);
        return await _context.DocumentaryCollections
            .Where(dc => dc.MaturityDate.HasValue && 
                        dc.MaturityDate.Value.Date <= cutoffDate && 
                        dc.Status == CollectionStatus.PresentedToDrawee)
            .OrderBy(dc => dc.MaturityDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalCollectionAmountAsync(Guid drawerId, CancellationToken cancellationToken = default)
    {
        var collections = await _context.DocumentaryCollections
            .Where(dc => dc.DrawerId == drawerId && dc.Status == CollectionStatus.PresentedToDrawee)
            .ToListAsync(cancellationToken);
        
        return collections.Sum(dc => dc.Amount.Amount);
    }

    public async Task<int> GetCollectionCountByStatusAsync(CollectionStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .CountAsync(dc => dc.Status == status, cancellationToken);
    }

    public async Task AddAsync(DocumentaryCollection collection, CancellationToken cancellationToken = default)
    {
        await _context.DocumentaryCollections.AddAsync(collection, cancellationToken);
    }

    public async Task UpdateAsync(DocumentaryCollection collection, CancellationToken cancellationToken = default)
    {
        _context.DocumentaryCollections.Update(collection);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(DocumentaryCollection collection, CancellationToken cancellationToken = default)
    {
        _context.DocumentaryCollections.Remove(collection);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string collectionNumber, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentaryCollections
            .AnyAsync(dc => dc.CollectionNumber == collectionNumber, cancellationToken);
    }
}