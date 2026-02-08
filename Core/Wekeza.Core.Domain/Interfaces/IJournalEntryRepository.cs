using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetBySourceAsync(string sourceType, Guid sourceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetByGLCodeAsync(string glCode, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetByStatusAsync(JournalStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetByTypeAsync(JournalType type, CancellationToken cancellationToken = default);
    Task<string> GenerateJournalNumberAsync(JournalType type, CancellationToken cancellationToken = default);
    Task<decimal> GetGLAccountBalanceAsync(string glCode, DateTime? asOfDate = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, decimal>> GetTrialBalanceAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntry journalEntry, CancellationToken cancellationToken = default);
    void Add(JournalEntry journalEntry);
    void Update(JournalEntry journalEntry);
    void Remove(JournalEntry journalEntry);
}