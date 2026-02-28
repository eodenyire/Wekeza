using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Cash Drawer Repository - Complete cash drawer management
/// Contract for managing cash drawers with comprehensive querying capabilities
/// </summary>
public interface ICashDrawerRepository
{
    // Basic CRUD operations
    Task<CashDrawer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CashDrawer?> GetByDrawerIdAsync(string drawerId, CancellationToken ct = default);
    Task AddAsync(CashDrawer drawer, CancellationToken ct = default);
    void Add(CashDrawer drawer);
    void Update(CashDrawer drawer);
    void Remove(CashDrawer drawer);

    // Teller-based queries
    Task<CashDrawer?> GetByTellerIdAsync(Guid tellerId, CancellationToken ct = default);
    Task<IEnumerable<CashDrawer>> GetDrawersByTellerCodeAsync(string tellerCode, CancellationToken ct = default);

    // Branch-based queries
    Task<IEnumerable<CashDrawer>> GetByBranchIdAsync(Guid branchId, CancellationToken ct = default);
    Task<IEnumerable<CashDrawer>> GetOpenDrawersByBranchIdAsync(Guid branchId, CancellationToken ct = default);

    // Status-based queries
    Task<IEnumerable<CashDrawer>> GetByStatusAsync(CashDrawerStatus status, CancellationToken ct = default);
    Task<IEnumerable<CashDrawer>> GetOpenDrawersAsync(CancellationToken ct = default);
    Task<IEnumerable<CashDrawer>> GetLockedDrawersAsync(CancellationToken ct = default);

    // Cash position queries
    Task<IEnumerable<CashDrawer>> GetDrawersWithCashAboveLimitAsync(decimal limitAmount, CancellationToken ct = default);
    Task<IEnumerable<CashDrawer>> GetDrawersWithCashBelowLimitAsync(decimal limitAmount, CancellationToken ct = default);
    Task<IEnumerable<CashDrawer>> GetDrawersRequiringReconciliationAsync(CancellationToken ct = default);

    // Analytics and reporting
    Task<decimal> GetTotalCashInBranchAsync(Guid branchId, string currencyCode, CancellationToken ct = default);
    Task<decimal> GetTotalCashInAllDrawersAsync(string currencyCode, CancellationToken ct = default);
    Task<int> GetOpenDrawerCountAsync(CancellationToken ct = default);
    Task<int> GetOpenDrawerCountByBranchAsync(Guid branchId, CancellationToken ct = default);

    // Validation helpers
    Task<bool> ExistsByDrawerIdAsync(string drawerId, CancellationToken ct = default);
    Task<bool> HasOpenDrawerAsync(Guid tellerId, CancellationToken ct = default);
}