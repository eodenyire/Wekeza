using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Teller Session Repository - Complete teller session management
/// Contract for managing teller sessions with comprehensive querying capabilities
/// </summary>
public interface ITellerSessionRepository
{
    // Basic CRUD operations
    Task<TellerSession?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TellerSession?> GetBySessionIdAsync(string sessionId, CancellationToken ct = default);
    Task AddAsync(TellerSession session, CancellationToken ct = default);
    void Add(TellerSession session);
    void Update(TellerSession session);
    void Remove(TellerSession session);

    // Teller-based queries
    Task<TellerSession?> GetActiveSessionByTellerIdAsync(Guid tellerId, CancellationToken ct = default);
    Task<TellerSession?> GetActiveSessionByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<TellerSession>> GetSessionsByTellerIdAsync(Guid tellerId, CancellationToken ct = default);
    Task<IEnumerable<TellerSession>> GetSessionsByTellerCodeAsync(string tellerCode, CancellationToken ct = default);

    // Branch-based queries
    Task<IEnumerable<TellerSession>> GetActiveSessionsByBranchIdAsync(Guid branchId, CancellationToken ct = default);
    Task<IEnumerable<TellerSession>> GetSessionsByBranchIdAsync(Guid branchId, CancellationToken ct = default);

    // Status-based queries
    Task<IEnumerable<TellerSession>> GetByStatusAsync(TellerSessionStatus status, CancellationToken ct = default);
    Task<IEnumerable<TellerSession>> GetActiveSessions(CancellationToken ct = default);
    Task<IEnumerable<TellerSession>> GetSuspendedSessions(CancellationToken ct = default);

    // Date-based queries
    Task<IEnumerable<TellerSession>> GetSessionsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<IEnumerable<TellerSession>> GetTodaysSessions(CancellationToken ct = default);
    Task<IEnumerable<TellerSession>> GetSessionsRequiringReconciliation(CancellationToken ct = default);

    // Analytics and reporting
    Task<int> GetActiveSessionCountAsync(CancellationToken ct = default);
    Task<int> GetActiveSessionCountByBranchAsync(Guid branchId, CancellationToken ct = default);
    Task<decimal> GetTotalCashInSessionsAsync(Guid branchId, CancellationToken ct = default);

    // Validation helpers
    Task<bool> HasActiveSessionAsync(Guid tellerId, CancellationToken ct = default);
    Task<bool> ExistsBySessionIdAsync(string sessionId, CancellationToken ct = default);
}