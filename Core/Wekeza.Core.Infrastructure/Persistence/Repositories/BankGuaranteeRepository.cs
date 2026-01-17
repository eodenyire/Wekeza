using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class BankGuaranteeRepository : IBankGuaranteeRepository
{
    private readonly ApplicationDbContext _context;

    public BankGuaranteeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BankGuarantee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .FirstOrDefaultAsync(bg => bg.Id == id, cancellationToken);
    }

    public async Task<BankGuarantee?> GetByBGNumberAsync(string bgNumber, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .FirstOrDefaultAsync(bg => bg.BGNumber == bgNumber, cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetByPrincipalIdAsync(Guid principalId, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .Where(bg => bg.PrincipalId == principalId)
            .OrderByDescending(bg => bg.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetByBeneficiaryIdAsync(Guid beneficiaryId, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .Where(bg => bg.BeneficiaryId == beneficiaryId)
            .OrderByDescending(bg => bg.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetByIssuingBankIdAsync(Guid issuingBankId, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .Where(bg => bg.IssuingBankId == issuingBankId)
            .OrderByDescending(bg => bg.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetExpiringBGsAsync(DateTime expiryDate, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Where(bg => bg.ExpiryDate <= expiryDate && bg.Status == BGStatus.Issued)
            .OrderBy(bg => bg.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetOutstandingBGsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .Where(bg => bg.Status == BGStatus.Issued || bg.Status == BGStatus.ClaimSubmitted)
            .OrderByDescending(bg => bg.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetBGsByStatusAsync(BGStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .Where(bg => bg.Status == status)
            .OrderByDescending(bg => bg.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetBGsByTypeAsync(GuaranteeType type, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .Where(bg => bg.Type == type)
            .OrderByDescending(bg => bg.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetBGsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .Where(bg => bg.IssueDate >= fromDate && bg.IssueDate <= toDate)
            .OrderByDescending(bg => bg.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BankGuarantee>> GetBGsWithClaimsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Include(bg => bg.Amendments)
            .Include(bg => bg.Claims)
            .Where(bg => bg.Claims.Any())
            .OrderByDescending(bg => bg.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalBGExposureAsync(Guid principalId, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .Where(bg => bg.PrincipalId == principalId && 
                        (bg.Status == BGStatus.Issued || bg.Status == BGStatus.ClaimSubmitted))
            .SumAsync(bg => bg.Amount.Amount, cancellationToken);
    }

    public async Task<int> GetBGCountByStatusAsync(BGStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .CountAsync(bg => bg.Status == status, cancellationToken);
    }

    public async Task AddAsync(BankGuarantee bankGuarantee, CancellationToken cancellationToken = default)
    {
        await _context.BankGuarantees.AddAsync(bankGuarantee, cancellationToken);
    }

    public Task UpdateAsync(BankGuarantee bankGuarantee, CancellationToken cancellationToken = default)
    {
        _context.BankGuarantees.Update(bankGuarantee);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(BankGuarantee bankGuarantee, CancellationToken cancellationToken = default)
    {
        _context.BankGuarantees.Remove(bankGuarantee);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string bgNumber, CancellationToken cancellationToken = default)
    {
        return await _context.BankGuarantees
            .AnyAsync(bg => bg.BGNumber == bgNumber, cancellationToken);
    }
}