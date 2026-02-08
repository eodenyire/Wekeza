using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class CardApplicationRepository : ICardApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public CardApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CardApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Include(ca => ca.Customer)
            .FirstOrDefaultAsync(ca => ca.Id == id, ct);
    }

    public async Task<CardApplication?> GetByApplicationNumberAsync(string applicationNumber, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Include(ca => ca.Customer)
            .FirstOrDefaultAsync(ca => ca.ApplicationNumber == applicationNumber, ct);
    }

    public async Task<IEnumerable<CardApplication>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.CustomerId == customerId)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CardApplication>> GetByAccountIdAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.AccountId == accountId)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CardApplication>> GetByStatusAsync(CardApplicationStatus status, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Include(ca => ca.Customer)
            .Where(ca => ca.Status == status)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CardApplication>> GetByCardTypeAsync(CardType cardType, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Include(ca => ca.Customer)
            .Where(ca => ca.CardType == cardType)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CardApplication>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.ApplicationDate >= fromDate && ca.ApplicationDate <= toDate)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CardApplication>> GetPendingApplicationsAsync(CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == CardApplicationStatus.Pending)
            .Include(ca => ca.Customer)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CardApplication>> GetApprovedApplicationsAsync(CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == CardApplicationStatus.Approved)
            .Include(ca => ca.Customer)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CardApplication>> GetRejectedApplicationsAsync(CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == CardApplicationStatus.Rejected)
            .Include(ca => ca.Customer)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CardApplication>> GetByBranchCodeAsync(string branchCode, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.BranchCode == branchCode)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(string applicationNumber, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .AnyAsync(ca => ca.ApplicationNumber == applicationNumber, ct);
    }

    public async Task<int> GetApplicationCountByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .CountAsync(ca => ca.CustomerId == customerId, ct);
    }

    public async Task<int> GetPendingCountByBranchAsync(string branchCode, CancellationToken ct = default)
    {
        return await _context.CardApplications
            .CountAsync(ca => ca.BranchCode == branchCode && ca.Status == CardApplicationStatus.Pending, ct);
    }

    public async Task AddAsync(CardApplication application, CancellationToken ct = default)
    {
        await _context.CardApplications.AddAsync(application, ct);
    }

    public async Task UpdateAsync(CardApplication application, CancellationToken ct = default)
    {
        _context.CardApplications.Update(application);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var application = await GetByIdAsync(id, ct);
        if (application != null)
        {
            _context.CardApplications.Remove(application);
        }
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