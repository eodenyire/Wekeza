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

    public async Task<DocumentaryCollection?> GetByIdAsync(Guid id)
    {
        return await _context.DocumentaryCollections
            .Include(dc => dc.Customer)
            .FirstOrDefaultAsync(dc => dc.Id == id);
    }

    public async Task<DocumentaryCollection?> GetByReferenceNumberAsync(string referenceNumber)
    {
        return await _context.DocumentaryCollections
            .Include(dc => dc.Customer)
            .FirstOrDefaultAsync(dc => dc.ReferenceNumber == referenceNumber);
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.CustomerId == customerId)
            .OrderByDescending(dc => dc.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<DocumentaryCollection>> GetActiveCollectionsAsync()
    {
        return await _context.DocumentaryCollections
            .Where(dc => dc.Status == "Active")
            .Include(dc => dc.Customer)
            .OrderBy(dc => dc.MaturityDate)
            .ToListAsync();
    }

    public async Task AddAsync(DocumentaryCollection collection)
    {
        await _context.DocumentaryCollections.AddAsync(collection);
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