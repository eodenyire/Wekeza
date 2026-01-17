using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class CardApplicationRepository : ICardApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public CardApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CardApplication?> GetByIdAsync(Guid id)
    {
        return await _context.CardApplications
            .Include(ca => ca.Customer)
            .FirstOrDefaultAsync(ca => ca.Id == id);
    }

    public async Task<IEnumerable<CardApplication>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.CardApplications
            .Where(ca => ca.CustomerId == customerId)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<CardApplication>> GetPendingApplicationsAsync()
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == "Pending")
            .Include(ca => ca.Customer)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync();
    }

    public async Task AddAsync(CardApplication application)
    {
        await _context.CardApplications.AddAsync(application);
    }

    public void Update(CardApplication application)
    {
        _context.CardApplications.Update(application);
    }

    public void Delete(CardApplication application)
    {
        _context.CardApplications.Remove(application);
    }
}