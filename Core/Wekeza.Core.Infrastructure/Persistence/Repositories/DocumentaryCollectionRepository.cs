using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class DocumentaryCollectionRepository : IDocumentaryCollectionRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentaryCollectionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentaryCollection?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Include(dc => dc.Customer)
            .FirstOrDefaultAsync(dc => dc.Id == id, ct);
    }

    public async Task<DocumentaryCollection?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Include(dc => dc.Customer)
            .FirstOrDefaultAsync(dc => dc.ReferenceNumber == referenceNumber, ct);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.CustomerId == customerId)
            .OrderByDescending(dc => dc.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.AccountId == accountId)
            .OrderByDescending(dc => dc.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByStatusAsync(DocumentaryCollectionStatus status, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Include(dc => dc.Customer)
            .Where(dc => dc.Status == status)
            .OrderByDescending(dc => dc.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByBranchCodeAsync(string branchCode, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.BranchCode == branchCode)
            .OrderByDescending(dc => dc.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.CreatedDate >= fromDate && dc.CreatedDate <= toDate)
            .OrderByDescending(dc => dc.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetMaturingCollectionsAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Include(dc => dc.Customer)
            .Where(dc => dc.MaturityDate >= fromDate && dc.MaturityDate <= toDate && dc.Status == DocumentaryCollectionStatus.Active)
            .OrderBy(dc => dc.MaturityDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetActiveCollectionsAsync(CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.Status == DocumentaryCollectionStatus.Active)
            .Include(dc => dc.Customer)
            .OrderBy(dc => dc.MaturityDate)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetTotalCollectionsByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.CustomerId == customerId && dc.Status == DocumentaryCollectionStatus.Active)
            .SumAsync(dc => dc.Amount.Amount, ct);
    }

    public async Task<bool> ExistsAsync(string referenceNumber, CancellationToken ct = default)
    {
        return await _context.DocumentaryCollections
            .AnyAsync(dc => dc.ReferenceNumber == referenceNumber, ct);
    }

    public async Task AddAsync(DocumentaryCollection collection, CancellationToken ct = default)
    {
        await _context.DocumentaryCollections.AddAsync(collection, ct);
    }

    public async Task UpdateAsync(DocumentaryCollection collection, CancellationToken ct = default)
    {
        _context.DocumentaryCollections.Update(collection);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var collection = await GetByIdAsync(id, ct);
        if (collection != null)
        {
            _context.DocumentaryCollections.Remove(collection);
        }
    }

    public void Update(DocumentaryCollection collection)
    {
        _context.DocumentaryCollections.Update(collection);
    }

    public void Delete(DocumentaryCollection collection)
    {
        _context.DocumentaryCollections.Remove(collection);
    }
}