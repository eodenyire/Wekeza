using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;
using System.Text.Json;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class JournalEntryRepository : IJournalEntryRepository
{
    private readonly ApplicationDbContext _context;

    public JournalEntryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .FirstOrDefaultAsync(j => j.Id == id, ct);
    }

    public async Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .FirstOrDefaultAsync(j => j.JournalNumber == journalNumber, ct);
    }

    public async Task<IEnumerable<JournalEntry>> GetBySourceAsync(string sourceType, Guid sourceId, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.SourceType == sourceType && j.SourceId == sourceId)
            .OrderByDescending(j => j.PostingDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.PostingDate >= fromDate && j.PostingDate <= toDate)
            .OrderBy(j => j.PostingDate)
            .ThenBy(j => j.JournalNumber)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JournalEntry>> GetByGLCodeAsync(string glCode, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default)
    {
        var query = _context.JournalEntries.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(j => j.PostingDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(j => j.PostingDate <= toDate.Value);

        // Filter by GL Code in journal lines (stored as JSON)
        var entries = await query.ToListAsync(ct);
        
        return entries.Where(j => j.Lines.Any(l => l.GLCode == glCode))
            .OrderBy(j => j.PostingDate)
            .ThenBy(j => j.JournalNumber);
    }

    public async Task<IEnumerable<JournalEntry>> GetByStatusAsync(JournalStatus status, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.Status == status)
            .OrderByDescending(j => j.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JournalEntry>> GetByTypeAsync(JournalType type, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.Type == type)
            .OrderByDescending(j => j.PostingDate)
            .ToListAsync(ct);
    }

    public async Task<string> GenerateJournalNumberAsync(JournalType type, CancellationToken ct = default)
    {
        var prefix = type switch
        {
            JournalType.Standard => "JV",
            JournalType.Adjustment => "AJ",
            JournalType.Reversal => "RV",
            JournalType.Opening => "OB",
            JournalType.Closing => "CB",
            JournalType.Accrual => "AC",
            JournalType.Provision => "PR",
            _ => "JV"
        };

        var today = DateTime.UtcNow;
        var datePrefix = today.ToString("yyyyMMdd");
        
        // Find the next sequence number for today
        var existingNumbers = await _context.JournalEntries
            .Where(j => j.JournalNumber.StartsWith($"{prefix}{datePrefix}"))
            .Select(j => j.JournalNumber)
            .ToListAsync(ct);

        var nextSequence = 1;
        string newNumber;
        
        do
        {
            newNumber = $"{prefix}{datePrefix}{nextSequence:0000}";
            nextSequence++;
        } while (existingNumbers.Contains(newNumber));

        return newNumber;
    }

    public async Task<decimal> GetGLAccountBalanceAsync(string glCode, DateTime? asOfDate = null, CancellationToken ct = default)
    {
        var query = _context.JournalEntries
            .Where(j => j.Status == JournalStatus.Posted);

        if (asOfDate.HasValue)
            query = query.Where(j => j.PostingDate <= asOfDate.Value);

        var entries = await query.ToListAsync(ct);
        
        decimal debitTotal = 0;
        decimal creditTotal = 0;

        foreach (var entry in entries)
        {
            foreach (var line in entry.Lines.Where(l => l.GLCode == glCode))
            {
                debitTotal += line.DebitAmount;
                creditTotal += line.CreditAmount;
            }
        }

        return debitTotal - creditTotal; // Net balance
    }

    public async Task<Dictionary<string, decimal>> GetTrialBalanceAsync(DateTime asOfDate, CancellationToken ct = default)
    {
        var entries = await _context.JournalEntries
            .Where(j => j.Status == JournalStatus.Posted && j.PostingDate <= asOfDate)
            .ToListAsync(ct);

        var balances = new Dictionary<string, decimal>();

        foreach (var entry in entries)
        {
            foreach (var line in entry.Lines)
            {
                if (!balances.ContainsKey(line.GLCode))
                    balances[line.GLCode] = 0;

                balances[line.GLCode] += line.DebitAmount - line.CreditAmount;
            }
        }

        return balances;
    }

    public async Task AddAsync(JournalEntry journalEntry, CancellationToken ct = default)
    {
        await _context.JournalEntries.AddAsync(journalEntry, ct);
    }

    public void Add(JournalEntry journalEntry)
    {
        _context.JournalEntries.Add(journalEntry);
    }

    public void Update(JournalEntry journalEntry)
    {
        _context.JournalEntries.Update(journalEntry);
    }

    public void Remove(JournalEntry journalEntry)
    {
        _context.JournalEntries.Remove(journalEntry);
    }
}