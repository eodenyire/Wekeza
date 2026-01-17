using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Role aggregate
/// Provides data access methods for role management operations
/// </summary>
public interface IRoleRepository
{
    // Basic CRUD Operations
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetByCodeAsync(string roleCode, CancellationToken cancellationToken = default);
    Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default);
    Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Query Operations
    Task<List<Role>> GetByTypeAsync(RoleType type, CancellationToken cancellationToken = default);
    Task<List<Role>> GetByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default);
    Task<List<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetFunctionalRolesAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetCustomRolesAsync(CancellationToken cancellationToken = default);

    // Hierarchy Operations
    Task<List<Role>> GetChildRolesAsync(Guid parentRoleId, CancellationToken cancellationToken = default);
    Task<Role?> GetParentRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRoleHierarchyAsync(Guid rootRoleId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetTopLevelRolesAsync(CancellationToken cancellationToken = default);

    // Permission Operations
    Task<List<Role>> GetRolesWithPermissionAsync(string permissionCode, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesWithModuleAccessAsync(string module, AccessLevel minLevel = AccessLevel.Read, CancellationToken cancellationToken = default);
    Task<List<string>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, List<string>>> GetRolePermissionMappingAsync(CancellationToken cancellationToken = default);

    // Security & Restrictions
    Task<List<Role>> GetRolesRequiringMfaAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesWithIpRestrictionsAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesWithTimeRestrictionsAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesWithTransactionLimitsAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesBySecurityClearanceAsync(SecurityClearanceLevel clearanceLevel, CancellationToken cancellationToken = default);

    // Search Operations
    Task<List<Role>> SearchRolesAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesModifiedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    // Pagination Support
    Task<(List<Role> Roles, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        RoleType? type = null,
        RoleStatus? status = null,
        CancellationToken cancellationToken = default);

    // Validation Support
    Task<bool> IsRoleCodeAvailableAsync(string roleCode, Guid? excludeRoleId = null, CancellationToken cancellationToken = default);
    Task<bool> CanDeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> HasCircularHierarchyAsync(Guid roleId, Guid parentRoleId, CancellationToken cancellationToken = default);

    // User Assignment Operations
    Task<List<Role>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUserCountByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, int>> GetUserCountByRolesAsync(List<Guid> roleIds, CancellationToken cancellationToken = default);

    // Approval & Workflow
    Task<List<Role>> GetRolesRequiringApprovalAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesWithWorkflowAsync(CancellationToken cancellationToken = default);

    // Reporting & Analytics
    Task<Dictionary<RoleType, int>> GetRoleCountByTypeAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<RoleStatus, int>> GetRoleCountByStatusAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetPermissionUsageCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetNewRolesCountAsync(DateTime since, CancellationToken cancellationToken = default);

    // Bulk Operations
    Task<List<Role>> AddRangeAsync(List<Role> roles, CancellationToken cancellationToken = default);
    Task<List<Role>> UpdateRangeAsync(List<Role> roles, CancellationToken cancellationToken = default);
    Task BulkUpdateStatusAsync(List<Guid> roleIds, RoleStatus status, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkAddPermissionAsync(List<Guid> roleIds, string permissionCode, string addedBy, CancellationToken cancellationToken = default);
    Task BulkRemovePermissionAsync(List<Guid> roleIds, string permissionCode, string removedBy, CancellationToken cancellationToken = default);

    // Existence Checks
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string roleCode, CancellationToken cancellationToken = default);
    Task<bool> HasUsersAssignedAsync(Guid roleId, CancellationToken cancellationToken = default);

    // Configuration & Settings
    Task<List<Role>> GetRolesWithSessionTimeoutAsync(TimeSpan? minTimeout = null, TimeSpan? maxTimeout = null, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesWithConcurrentUserLimitAsync(int? minLimit = null, int? maxLimit = null, CancellationToken cancellationToken = default);

    // Module Access Operations
    Task<Dictionary<string, AccessLevel>> GetModuleAccessForRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesWithAccessToModuleAsync(string module, CancellationToken cancellationToken = default);
    Task<Dictionary<string, List<string>>> GetModuleRoleMappingAsync(CancellationToken cancellationToken = default);
}