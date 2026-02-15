using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id);
    Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber);
    Task<IEnumerable<JournalEntry>> GetBySourceAsync(string sourceType, Guid sourceId);
    Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<JournalEntry>> GetByGLCodeAsync(string glCode, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<JournalEntry>> GetByStatusAsync(JournalStatus status);
    Task<IEnumerable<JournalEntry>> GetByTypeAsync(JournalType type);
    Task<string> GenerateJournalNumberAsync(JournalType type);
    Task<decimal> GetGLAccountBalanceAsync(string glCode, DateTime? asOfDate = null);
    Task<Dictionary<string, decimal>> GetTrialBalanceAsync(DateTime asOfDate);
    Task AddAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
    void Add(JournalEntry journalEntry);
    void Update(JournalEntry journalEntry);
    void Remove(JournalEntry journalEntry);
}