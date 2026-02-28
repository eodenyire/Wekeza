using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class LetterOfCreditRepository : ILetterOfCreditRepository
{
    private readonly ApplicationDbContext _context;

    public LetterOfCreditRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LetterOfCredit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Include(lc => lc.Documents)
            .FirstOrDefaultAsync(lc => lc.Id == id, cancellationToken);
    }

    public async Task<LetterOfCredit?> GetByLCNumberAsync(string lcNumber, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Include(lc => lc.Documents)
            .FirstOrDefaultAsync(lc => lc.LCNumber == lcNumber, cancellationToken);
    }

    public async Task<IEnumerable<LetterOfCredit>> GetByApplicantIdAsync(Guid applicantId, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Include(lc => lc.Documents)
            .Where(lc => lc.ApplicantId == applicantId)
            .OrderByDescending(lc => lc.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LetterOfCredit>> GetByBeneficiaryIdAsync(Guid beneficiaryId, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Include(lc => lc.Documents)
            .Where(lc => lc.BeneficiaryId == beneficiaryId)
            .OrderByDescending(lc => lc.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LetterOfCredit>> GetByIssuingBankIdAsync(Guid issuingBankId, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Include(lc => lc.Documents)
            .Where(lc => lc.IssuingBankId == issuingBankId)
            .OrderByDescending(lc => lc.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LetterOfCredit>> GetExpiringLCsAsync(DateTime expiryDate, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Where(lc => lc.ExpiryDate <= expiryDate && lc.Status == LCStatus.Issued)
            .OrderBy(lc => lc.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LetterOfCredit>> GetOutstandingLCsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Include(lc => lc.Documents)
            .Where(lc => lc.Status == LCStatus.Issued || 
                        lc.Status == LCStatus.DocumentsPresented || 
                        lc.Status == LCStatus.DocumentsAccepted)
            .OrderByDescending(lc => lc.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LetterOfCredit>> GetLCsByStatusAsync(LCStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Include(lc => lc.Documents)
            .Where(lc => lc.Status == status)
            .OrderByDescending(lc => lc.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LetterOfCredit>> GetLCsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Include(lc => lc.Amendments)
            .Include(lc => lc.Documents)
            .Where(lc => lc.IssueDate >= fromDate && lc.IssueDate <= toDate)
            .OrderByDescending(lc => lc.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalLCExposureAsync(Guid applicantId, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .Where(lc => lc.ApplicantId == applicantId && 
                        (lc.Status == LCStatus.Issued || 
                         lc.Status == LCStatus.DocumentsPresented || 
                         lc.Status == LCStatus.DocumentsAccepted))
            .SumAsync(lc => lc.Amount.Amount, cancellationToken);
    }

    public async Task<int> GetLCCountByStatusAsync(LCStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .CountAsync(lc => lc.Status == status, cancellationToken);
    }

    public async Task AddAsync(LetterOfCredit letterOfCredit, CancellationToken cancellationToken = default)
    {
        await _context.LetterOfCredits.AddAsync(letterOfCredit, cancellationToken);
    }

    public Task UpdateAsync(LetterOfCredit letterOfCredit, CancellationToken cancellationToken = default)
    {
        _context.LetterOfCredits.Update(letterOfCredit);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(LetterOfCredit letterOfCredit, CancellationToken cancellationToken = default)
    {
        _context.LetterOfCredits.Remove(letterOfCredit);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string lcNumber, CancellationToken cancellationToken = default)
    {
        return await _context.LetterOfCredits
            .AnyAsync(lc => lc.LCNumber == lcNumber, cancellationToken);
    }
}