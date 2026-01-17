using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository for GL Account management
/// </summary>
public interface IGLAccountRepository
{
    Task<GLAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GLAccount?> GetByGLCodeAsync(string glCode, CancellationToken cancellationToken = default);
    Task AddAsync(GLAccount account, CancellationToken cancellationToken = default);
    void Update(GLAccount account);
    
    Task<IEnumerable<GLAccount>> GetByTypeAsync(GLAccountType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetByCategoryAsync(GLAccountCategory category, CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetLeafAccountsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetByParentAsync(string parentGLCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetChartOfAccountsAsync(CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByGLCodeAsync(string glCode, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Journal Entry management
/// </summary>
public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber, CancellationToken cancellationToken = default);
    Task AddAsync(JournalEntry entry, CancellationToken cancellationToken = default);
    void Update(JournalEntry entry);
    
    Task<IEnumerable<JournalEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetByGLCodeAsync(string glCode, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetBySourceAsync(string sourceType, Guid sourceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetByStatusAsync(JournalStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<JournalEntry>> GetUnpostedEntriesAsync(CancellationToken cancellationToken = default);
    
    Task<string> GenerateJournalNumberAsync(CancellationToken cancellationToken = default);
}
