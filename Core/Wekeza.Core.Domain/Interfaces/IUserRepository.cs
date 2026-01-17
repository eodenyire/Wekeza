using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for User aggregate
/// Provides data access methods for user management operations
/// </summary>
public interface IUserRepository
{
    // Basic CRUD Operations
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Query Operations
    Task<List<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    Task<List<User>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
    Task<List<User>> GetByRoleAsync(string roleCode, CancellationToken cancellationToken = default);
    Task<List<User>> GetBySecurityClearanceAsync(SecurityClearanceLevel clearanceLevel, CancellationToken cancellationToken = default);
    Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersWithExpiredPasswordsAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersWithMfaEnabledAsync(CancellationToken cancellationToken = default);

    // Search Operations
    Task<List<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersByManagerAsync(string managerId, CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersLastLoginBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    // Pagination Support
    Task<(List<User> Users, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        UserStatus? status = null,
        string? department = null,
        string? roleCode = null,
        CancellationToken cancellationToken = default);

    // Authentication Support
    Task<User?> GetUserForAuthenticationAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmployeeIdAvailableAsync(string employeeId, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    // Session Management
    Task<List<User>> GetUsersWithActiveSessionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveSessionCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersWithExpiredSessionsAsync(CancellationToken cancellationToken = default);

    // Security Operations
    Task<List<User>> GetUsersWithFailedLoginsAsync(int minFailedAttempts = 1, CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersRequiringPasswordChangeAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetUsersWithoutMfaAsync(CancellationToken cancellationToken = default);

    // Reporting & Analytics
    Task<Dictionary<UserStatus, int>> GetUserCountByStatusAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetUserCountByDepartmentAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetUserCountByRoleAsync(CancellationToken cancellationToken = default);
    Task<int> GetNewUsersCountAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<int> GetActiveUsersCountAsync(DateTime since, CancellationToken cancellationToken = default);

    // Bulk Operations
    Task<List<User>> AddRangeAsync(List<User> users, CancellationToken cancellationToken = default);
    Task<List<User>> UpdateRangeAsync(List<User> users, CancellationToken cancellationToken = default);
    Task BulkUpdateStatusAsync(List<Guid> userIds, UserStatus status, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkAssignRoleAsync(List<Guid> userIds, string roleCode, string assignedBy, CancellationToken cancellationToken = default);

    // Existence Checks
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
}