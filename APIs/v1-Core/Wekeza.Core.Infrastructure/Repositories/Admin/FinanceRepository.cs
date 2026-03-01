using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Infrastructure.Repositories.Admin;

public class FinanceRepository
{
    private readonly ApplicationDbContext _context;

    public FinanceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ===== GL Account Operations =====
    public async Task<GLAccount?> GetGLAccountByCodeAsync(string glCode, CancellationToken cancellationToken = default)
    {
        return await _context.GLAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GLCode == glCode, cancellationToken);
    }

    public async Task<List<GLAccount>> SearchGLAccountsAsync(string accountType, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.GLAccounts.AsNoTracking();

        if (!string.IsNullOrEmpty(accountType) && Enum.TryParse<GLAccountType>(accountType, true, out var parsedType))
            query = query.Where(g => g.AccountType == parsedType);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<GLAccountStatus>(status, true, out var parsedStatus))
            query = query.Where(g => g.Status == parsedStatus);

        return await query
            .OrderBy(g => g.GLCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<GLAccount> AddGLAccountAsync(GLAccount glAccount, CancellationToken cancellationToken = default)
    {
        await _context.GLAccounts.AddAsync(glAccount, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return glAccount;
    }

    public async Task<GLAccount> UpdateGLAccountAsync(GLAccount glAccount, CancellationToken cancellationToken = default)
    {
        _context.GLAccounts.Update(glAccount);
        await _context.SaveChangesAsync(cancellationToken);
        return glAccount;
    }

    // ===== Journal Entry Operations =====
    public async Task<JournalEntry?> GetJournalEntryByIdAsync(Guid journalId, CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == journalId, cancellationToken);
    }

    public async Task<List<JournalEntry>> SearchJournalEntriesAsync(string glCode, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.JournalEntries.AsNoTracking();

        if (!string.IsNullOrEmpty(glCode))
            query = query.AsEnumerable().Where(j => j.Lines.Any(l => l.GLCode == glCode)).AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(j => j.PostingDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(j => j.PostingDate <= toDate.Value);

        return await query
            .OrderByDescending(j => j.PostingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<JournalEntry> AddJournalEntryAsync(JournalEntry entry, CancellationToken cancellationToken = default)
    {
        await _context.JournalEntries.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<JournalEntry> UpdateJournalEntryAsync(JournalEntry entry, CancellationToken cancellationToken = default)
    {
        _context.JournalEntries.Update(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<int> GetPendingJournalEntriesCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.JournalEntries.CountAsync(j => j.Status == JournalStatus.Draft, cancellationToken);
    }

    // ===== Reconciliation Operations =====
    public async Task<Reconciliation?> GetReconciliationByIdAsync(Guid reconciliationId, CancellationToken cancellationToken = default)
    {
        return await _context.Reconciliations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == reconciliationId, cancellationToken);
    }

    public async Task<List<Reconciliation>> SearchReconciliationsAsync(string status, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Reconciliations.AsNoTracking();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        if (fromDate.HasValue)
            query = query.Where(r => r.ReconciliationDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(r => r.ReconciliationDate <= toDate.Value);

        return await query
            .OrderByDescending(r => r.ReconciliationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Reconciliation> AddReconciliationAsync(Reconciliation reconciliation, CancellationToken cancellationToken = default)
    {
        await _context.Reconciliations.AddAsync(reconciliation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return reconciliation;
    }

    public async Task<Reconciliation> UpdateReconciliationAsync(Reconciliation reconciliation, CancellationToken cancellationToken = default)
    {
        _context.Reconciliations.Update(reconciliation);
        await _context.SaveChangesAsync(cancellationToken);
        return reconciliation;
    }

    public async Task<int> GetOpenReconciliationsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reconciliations.CountAsync(r => r.Status == "Open", cancellationToken);
    }

    // ===== Interest Accrual Operations =====
    public async Task<InterestAccrual?> GetAccrualByIdAsync(Guid accrualId, CancellationToken cancellationToken = default)
    {
        return await _context.InterestAccruals
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accrualId, cancellationToken);
    }

    public async Task<List<InterestAccrual>> GetPendingAccrualsAsync(DateTime asOfDate, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.InterestAccruals
            .AsNoTracking()
            .Where(a => a.Status == "Pending" && a.AccrualDate <= asOfDate)
            .OrderBy(a => a.AccrualDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<InterestAccrual> AddAccrualAsync(InterestAccrual accrual, CancellationToken cancellationToken = default)
    {
        await _context.InterestAccruals.AddAsync(accrual, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return accrual;
    }

    public async Task<InterestAccrual> UpdateAccrualAsync(InterestAccrual accrual, CancellationToken cancellationToken = default)
    {
        _context.InterestAccruals.Update(accrual);
        await _context.SaveChangesAsync(cancellationToken);
        return accrual;
    }

    public async Task<decimal> GetTotalAccruedAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _context.InterestAccruals
            .Where(a => a.AccrualDate <= asOfDate && a.Status == "Posted")
            .SumAsync(a => a.AccrualAmount, cancellationToken);
    }

    // ===== Financial Dashboard Operations =====
    public async Task<decimal> GetTotalAssetsAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _context.GLAccounts
            .Where(g => g.AccountType == GLAccountType.Asset && g.Status == GLAccountStatus.Active)
            .SumAsync(g => g.DebitBalance, cancellationToken);
    }

    public async Task<decimal> GetTotalLiabilitiesAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _context.GLAccounts
            .Where(g => g.AccountType == GLAccountType.Liability && g.Status == GLAccountStatus.Active)
            .SumAsync(g => g.CreditBalance, cancellationToken);
    }

    public async Task<decimal> GetTotalEquityAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _context.GLAccounts
            .Where(g => g.AccountType == GLAccountType.Equity && g.Status == GLAccountStatus.Active)
            .SumAsync(g => g.CreditBalance, cancellationToken);
    }

    public async Task<int> GetGLAccountCountAsync(string status, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<GLAccountStatus>(status, true, out var parsedStatus))
        {
            return 0;
        }

        return await _context.GLAccounts
            .CountAsync(g => g.Status == parsedStatus, cancellationToken);
    }
}
