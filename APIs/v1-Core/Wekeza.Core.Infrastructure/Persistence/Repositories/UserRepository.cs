using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for User aggregate
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Basic CRUD Operations
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.EmployeeId == employeeId, cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
        return user;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }

    // Query Operations
    public async Task<List<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Department == department)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetByRoleAsync(string roleCode, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Roles.Any(r => r.RoleCode == roleCode))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetBySecurityClearanceAsync(SecurityClearanceLevel clearanceLevel, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.SecurityClearance == clearanceLevel)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Status == UserStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Status == UserStatus.Locked)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersWithExpiredPasswordsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.PasswordExpiresAt.HasValue && u.PasswordExpiresAt.Value < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersWithMfaEnabledAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.MfaEnabled)
            .ToListAsync(cancellationToken);
    }

    // Search Operations
    public async Task<List<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Username.Contains(searchTerm) || 
                       u.Email.Contains(searchTerm) || 
                       u.FirstName.Contains(searchTerm) || 
                       u.LastName.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersByManagerAsync(string managerId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.ManagerId == managerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersLastLoginBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= startDate && u.LastLoginAt.Value <= endDate)
            .ToListAsync(cancellationToken);
    }

    // Pagination Support
    public async Task<(List<User> Users, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        UserStatus? status = null,
        string? department = null,
        string? roleCode = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

        if (status.HasValue)
            query = query.Where(u => u.Status == status.Value);
        
        if (!string.IsNullOrEmpty(department))
            query = query.Where(u => u.Department == department);
        
        if (!string.IsNullOrEmpty(roleCode))
            query = query.Where(u => u.Roles.Any(r => r.RoleCode == roleCode));

        var totalCount = await query.CountAsync(cancellationToken);
        
        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    // Authentication Support
    public async Task<User?> GetUserForAuthenticationAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.Username == username);
        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);
        
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.Email == email);
        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);
        
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmployeeIdAvailableAsync(string employeeId, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.EmployeeId == employeeId);
        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);
        
        return !await query.AnyAsync(cancellationToken);
    }

    // Session Management - Stub implementations
    public async Task<List<User>> GetUsersWithActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Status == UserStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveSessionCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(0);
    }

    public async Task<List<User>> GetUsersWithExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(new List<User>());
    }

    // Security Operations
    public async Task<List<User>> GetUsersWithFailedLoginsAsync(int minFailedAttempts = 1, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.FailedLoginAttempts >= minFailedAttempts)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersRequiringPasswordChangeAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.MustChangePassword || (u.PasswordExpiresAt.HasValue && u.PasswordExpiresAt.Value < DateTime.UtcNow))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUsersWithoutMfaAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => !u.MfaEnabled)
            .ToListAsync(cancellationToken);
    }

    // Reporting & Analytics
    public async Task<Dictionary<UserStatus, int>> GetUserCountByStatusAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .GroupBy(u => u.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetUserCountByDepartmentAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Department != null)
            .GroupBy(u => u.Department!)
            .Select(g => new { Department = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Department, x => x.Count, cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetUserCountByRoleAsync(CancellationToken cancellationToken = default)
    {
        // Simplified - returns empty dictionary
        return await Task.FromResult(new Dictionary<string, int>());
    }

    public async Task<int> GetNewUsersCountAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.CreatedAt >= since)
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveUsersCountAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= since)
            .CountAsync(cancellationToken);
    }

    // Bulk Operations
    public async Task<List<User>> AddRangeAsync(List<User> users, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddRangeAsync(users, cancellationToken);
        return users;
    }

    public async Task<List<User>> UpdateRangeAsync(List<User> users, CancellationToken cancellationToken = default)
    {
        _context.Users.UpdateRange(users);
        await Task.CompletedTask;
        return users;
    }

    public async Task BulkUpdateStatusAsync(List<Guid> userIds, UserStatus status, string updatedBy, CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
        
        // Note: Status is read-only, would need domain methods to change it
        // This is a stub that doesn't actually update the status
        await Task.CompletedTask;
    }

    public async Task BulkAssignRoleAsync(List<Guid> userIds, string roleCode, string assignedBy, CancellationToken cancellationToken = default)
    {
        // Simplified - does nothing in stub
        await Task.CompletedTask;
    }

    // Existence Checks
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.EmployeeId == employeeId, cancellationToken);
    }
}
