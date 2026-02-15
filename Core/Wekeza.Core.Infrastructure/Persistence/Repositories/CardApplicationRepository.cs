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

    public async Task<CardApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .FirstOrDefaultAsync(ca => ca.Id == id, cancellationToken);
    }

    public async Task<CardApplication?> GetByApplicationNumberAsync(string applicationNumber, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .FirstOrDefaultAsync(ca => ca.ApplicationNumber == applicationNumber, cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.CustomerId == customerId)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.AccountId == accountId)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetByStatusAsync(CardApplicationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == status)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetByCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.RequestedCardType == cardType)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetPendingApplicationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == CardApplicationStatus.Submitted)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetApplicationsRequiringDocumentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == CardApplicationStatus.DocumentsRequired)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetApplicationsUnderReviewAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == CardApplicationStatus.UnderReview)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetApplicationsPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == CardApplicationStatus.PendingApproval)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetApprovedApplicationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.Status == CardApplicationStatus.Approved)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetApplicationsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.ApplicationDate.Date >= fromDate.Date && ca.ApplicationDate.Date <= toDate.Date)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetApplicationsByRiskCategoryAsync(string riskCategory, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.RiskCategory == riskCategory)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetApplicationsRequiringManualReviewAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.RequiresManualReview)
            .OrderBy(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CardApplication>> GetApplicationsByWorkflowInstanceAsync(Guid workflowInstanceId, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .Where(ca => ca.WorkflowInstanceId == workflowInstanceId)
            .OrderByDescending(ca => ca.ApplicationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetApplicationCountByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .CountAsync(ca => ca.CustomerId == customerId, cancellationToken);
    }

    public async Task<int> GetApplicationCountByStatusAsync(CardApplicationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.CardApplications
            .CountAsync(ca => ca.Status == status, cancellationToken);
    }

    public async Task AddAsync(CardApplication application, CancellationToken cancellationToken = default)
    {
        await _context.CardApplications.AddAsync(application, cancellationToken);
    }

    public void Update(CardApplication application)
    {
        _context.CardApplications.Update(application);
    }

    public async Task UpdateAsync(CardApplication application, CancellationToken cancellationToken = default)
    {
        _context.CardApplications.Update(application);
        await Task.CompletedTask;
    }
}