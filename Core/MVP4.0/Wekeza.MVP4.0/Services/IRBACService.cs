using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public interface IRBACService
{
    // Authentication
    Task<AuthResult> AuthenticateUserAsync(UserCredentials credentials);
    Task<bool> ValidateSessionAsync(Guid userId);
    Task LogoutUserAsync(Guid userId);

    // Authorization
    Task<bool> AuthorizeActionAsync(Guid userId, string resource, string action);
    Task<List<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<ApprovalRequirement> EnforceApprovalLimitsAsync(Guid userId, decimal amount, string transactionType);

    // Role Management
    Task<List<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByIdAsync(Guid roleId);
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task<Role> CreateRoleAsync(CreateRoleRequest request);
    Task<Role> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request);
    Task<bool> DeleteRoleAsync(Guid roleId);

    // User Role Assignment
    Task<UserRoleAssignment> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy);
    Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    Task<List<Role>> GetUserRolesAsync(Guid userId);
    Task<List<ApplicationUser>> GetUsersInRoleAsync(Guid roleId);

    // Permission Management
    Task<List<Permission>> GetAllPermissionsAsync();
    Task<Permission?> GetPermissionByIdAsync(Guid permissionId);
    Task<Permission> CreatePermissionAsync(CreatePermissionRequest request);
    Task<bool> DeletePermissionAsync(Guid permissionId);

    // Role Permission Management
    Task<bool> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
    Task<bool> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
    Task<List<Permission>> GetRolePermissionsAsync(Guid roleId);

    // Audit and Logging
    Task LogUserActionAsync(Guid userId, string action, string resourceType, Guid resourceId, 
        string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null);
}

// DTOs and Request Models
public class UserCredentials
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public ApplicationUser? User { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class ApprovalRequirement
{
    public bool RequiresApproval { get; set; }
    public string ApproverRole { get; set; } = string.Empty;
    public decimal ApprovalLimit { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class CreateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ApprovalLimit { get; set; } = 0;
    public List<Guid> PermissionIds { get; set; } = new();
}

public class UpdateRoleRequest
{
    public string? RoleName { get; set; }
    public string? Description { get; set; }
    public decimal? ApprovalLimit { get; set; }
    public bool? IsActive { get; set; }
    public List<Guid>? PermissionIds { get; set; }
}

public class CreatePermissionRequest
{
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Conditions { get; set; }
}