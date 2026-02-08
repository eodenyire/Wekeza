using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class JournalEntryRepository : IJournalEntryRepository
{
    private readonly ApplicationDbContext _context;

    public JournalEntryRepository(ApplicationDbContext context) => _context = context;

    public async Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.JournalEntries.FirstOrDefaultAsync(j => j.Id == id, ct);
    }

    public async Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .FirstOrDefaultAsync(j => j.JournalNumber == journalNumber, ct);
    }

    public async Task AddAsync(JournalEntry entry, CancellationToken ct = default)
    {
        await _context.JournalEntries.AddAsync(entry, ct);
    }

    public void Update(JournalEntry entry)
    {
        _context.JournalEntries.Update(entry);
    }

    public async Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.PostingDate >= fromDate && j.PostingDate <= toDate)
            .OrderBy(j => j.PostingDate)
            .ThenBy(j => j.JournalNumber)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JournalEntry>> GetByGLCodeAsync(string glCode, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.PostingDate >= fromDate && 
                       j.PostingDate <= toDate &&
                       j.Lines.Any(l => l.GLCode == glCode))
            .OrderBy(j => j.PostingDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JournalEntry>> GetBySourceAsync(string sourceType, Guid sourceId, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.SourceType == sourceType && j.SourceId == sourceId)
            .OrderBy(j => j.PostingDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JournalEntry>> GetByStatusAsync(JournalStatus status, CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.Status == status)
            .OrderByDescending(j => j.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JournalEntry>> GetUnpostedEntriesAsync(CancellationToken ct = default)
    {
        return await _context.JournalEntries
            .Where(j => j.Status == JournalStatus.Draft)
            .OrderBy(j => j.CreatedDate)
            .ToListAsync(ct);
    }

    public async Task<string> GenerateJournalNumberAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow;
        var prefix = $"JV{today:yyyyMMdd}";
        
        var lastJournal = await _context.JournalEntries
            .Where(j => j.JournalNumber.StartsWith(prefix))
            .OrderByDescending(j => j.JournalNumber)
            .FirstOrDefaultAsync(ct);
        
        int sequence = 1;
        if (lastJournal != null)
        {
            var lastSequence = lastJournal.JournalNumber.Substring(prefix.Length);
            if (int.TryParse(lastSequence, out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }
}
