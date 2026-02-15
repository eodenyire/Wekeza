using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public class RBACService : IRBACService
{
    private readonly MVP4DbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RBACService> _logger;

    public RBACService(MVP4DbContext context, IConfiguration configuration, ILogger<RBACService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    #region Authentication

    public async Task<AuthResult> AuthenticateUserAsync(UserCredentials credentials)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == credentials.Username && u.IsActive);

            if (user == null)
            {
                await LogFailedLoginAttemptAsync(credentials.Username, "User not found");
                return new AuthResult { IsSuccess = false, ErrorMessage = "Invalid credentials" };
            }

            if (!BCrypt.Net.BCrypt.Verify(credentials.Password, user.PasswordHash))
            {
                await IncrementFailedLoginAttemptsAsync(user.Id);
                await LogFailedLoginAttemptAsync(credentials.Username, "Invalid password");
                return new AuthResult { IsSuccess = false, ErrorMessage = "Invalid credentials" };
            }

            // Check if account is locked due to failed attempts
            if (user.FailedLoginAttempts >= 5)
            {
                await LogFailedLoginAttemptAsync(credentials.Username, "Account locked");
                return new AuthResult { IsSuccess = false, ErrorMessage = "Account is locked due to multiple failed attempts" };
            }

            // Reset failed attempts on successful login
            await ResetFailedLoginAttemptsAsync(user.Id);
            await UpdateLastLoginAsync(user.Id);

            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(8);

            await LogUserActionAsync(user.Id, "Login", "Authentication", user.Id);

            return new AuthResult
            {
                IsSuccess = true,
                User = user,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for user {Username}", credentials.Username);
            return new AuthResult { IsSuccess = false, ErrorMessage = "Authentication failed" };
        }
    }

    public async Task<bool> ValidateSessionAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating session for user {UserId}", userId);
            return false;
        }
    }

    public async Task LogoutUserAsync(Guid userId)
    {
        try
        {
            await LogUserActionAsync(userId, "Logout", "Authentication", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", userId);
        }
    }

    #endregion

    #region Authorization

    public async Task<bool> AuthorizeActionAsync(Guid userId, string resource, string action)
    {
        try
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            
            // Check if user has the specific permission
            var hasPermission = userPermissions.Any(p => 
                (p.Resource == resource || p.Resource == "All") && 
                (p.Action == action || p.Action == "All"));

            if (!hasPermission)
            {
                await LogUserActionAsync(userId, $"Unauthorized access attempt: {action} on {resource}", 
                    "Authorization", userId);
            }

            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authorizing action {Action} on {Resource} for user {UserId}", 
                action, resource, userId);
            return false;
        }
    }

    public async Task<List<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        try
        {
            var permissions = await _context.UserRoleAssignments
                .Where(ura => ura.UserId == userId && ura.IsActive)
                .Include(ura => ura.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .Where(ura => ura.Role.IsActive)
                .SelectMany(ura => ura.Role.RolePermissions.Select(rp => rp.Permission))
                .Distinct()
                .ToListAsync();

            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for user {UserId}", userId);
            return new List<Permission>();
        }
    }

    public async Task<ApprovalRequirement> EnforceApprovalLimitsAsync(Guid userId, decimal amount, string transactionType)
    {
        try
        {
            var userRoles = await _context.UserRoleAssignments
                .Where(ura => ura.UserId == userId && ura.IsActive)
                .Include(ura => ura.Role)
                .Where(ura => ura.Role.IsActive)
                .Select(ura => ura.Role)
                .ToListAsync();

            var maxApprovalLimit = userRoles.Max(r => r.ApprovalLimit);

            if (amount <= maxApprovalLimit)
            {
                return new ApprovalRequirement
                {
                    RequiresApproval = false,
                    ApprovalLimit = maxApprovalLimit
                };
            }

            // Determine appropriate approver role based on amount
            string approverRole = amount switch
            {
                <= 100000 => "BranchManager",
                <= 500000 => "RegionalManager",
                _ => "HeadOffice"
            };

            return new ApprovalRequirement
            {
                RequiresApproval = true,
                ApproverRole = approverRole,
                ApprovalLimit = maxApprovalLimit,
                Reason = $"Amount {amount:C} exceeds user approval limit of {maxApprovalLimit:C}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enforcing approval limits for user {UserId}, amount {Amount}", 
                userId, amount);
            return new ApprovalRequirement
            {
                RequiresApproval = true,
                ApproverRole = "BranchManager",
                Reason = "Error determining approval requirements"
            };
        }
    }

    #endregion

    #region Role Management

    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .OrderBy(r => r.RoleName)
            .ToListAsync();
    }

    public async Task<Role?> GetRoleByIdAsync(Guid roleId)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.RoleName == roleName);
    }

    public async Task<Role> CreateRoleAsync(CreateRoleRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var role = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = request.RoleName,
                Description = request.Description,
                ApprovalLimit = request.ApprovalLimit,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // Assign permissions to role
            foreach (var permissionId in request.PermissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RolePermissionId = Guid.NewGuid(),
                    RoleId = role.RoleId,
                    PermissionId = permissionId,
                    AssignedAt = DateTime.UtcNow
                };
                _context.RolePermissions.Add(rolePermission);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetRoleByIdAsync(role.RoleId) ?? role;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating role {RoleName}", request.RoleName);
            throw;
        }
    }

    public async Task<Role> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
                throw new ArgumentException("Role not found");

            if (!string.IsNullOrEmpty(request.RoleName))
                role.RoleName = request.RoleName;
            
            if (!string.IsNullOrEmpty(request.Description))
                role.Description = request.Description;
            
            if (request.ApprovalLimit.HasValue)
                role.ApprovalLimit = request.ApprovalLimit.Value;
            
            if (request.IsActive.HasValue)
                role.IsActive = request.IsActive.Value;

            // Update permissions if provided
            if (request.PermissionIds != null)
            {
                // Remove existing permissions
                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();
                _context.RolePermissions.RemoveRange(existingPermissions);

                // Add new permissions
                foreach (var permissionId in request.PermissionIds)
                {
                    var rolePermission = new RolePermission
                    {
                        RolePermissionId = Guid.NewGuid(),
                        RoleId = roleId,
                        PermissionId = permissionId,
                        AssignedAt = DateTime.UtcNow
                    };
                    _context.RolePermissions.Add(rolePermission);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetRoleByIdAsync(roleId) ?? role;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating role {RoleId}", roleId);
            throw;
        }
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Check if role is in use
            var roleInUse = await _context.UserRoleAssignments
                .AnyAsync(ura => ura.RoleId == roleId);

            if (roleInUse)
            {
                // Soft delete - deactivate role
                var role = await _context.Roles.FindAsync(roleId);
                if (role != null)
                {
                    role.IsActive = false;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Hard delete - remove role and permissions
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();
                _context.RolePermissions.RemoveRange(rolePermissions);

                var role = await _context.Roles.FindAsync(roleId);
                if (role != null)
                {
                    _context.Roles.Remove(role);
                }

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
            return false;
        }
    }

    #endregion

    #region User Role Assignment

    public async Task<UserRoleAssignment> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy)
    {
        try
        {
            // Check if assignment already exists
            var existingAssignment = await _context.UserRoleAssignments
                .FirstOrDefaultAsync(ura => ura.UserId == userId && ura.RoleId == roleId);

            if (existingAssignment != null)
            {
                existingAssignment.IsActive = true;
                existingAssignment.AssignedAt = DateTime.UtcNow;
                existingAssignment.AssignedBy = assignedBy;
                await _context.SaveChangesAsync();
                return existingAssignment;
            }

            var assignment = new UserRoleAssignment
            {
                UserRoleId = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.UserRoleAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            await LogUserActionAsync(assignedBy, "Role assigned", "UserRole", assignment.UserRoleId,
                newValues: JsonSerializer.Serialize(new { userId, roleId }));

            return assignment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            throw;
        }
    }

    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        try
        {
            var assignment = await _context.UserRoleAssignments
                .FirstOrDefaultAsync(ura => ura.UserId == userId && ura.RoleId == roleId);

            if (assignment != null)
            {
                assignment.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            return false;
        }
    }

    public async Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _context.UserRoleAssignments
            .Where(ura => ura.UserId == userId && ura.IsActive)
            .Include(ura => ura.Role)
            .Where(ura => ura.Role.IsActive)
            .Select(ura => ura.Role)
            .ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetUsersInRoleAsync(Guid roleId)
    {
        return await _context.UserRoleAssignments
            .Where(ura => ura.RoleId == roleId && ura.IsActive)
            .Include(ura => ura.User)
            .Where(ura => ura.User.IsActive)
            .Select(ura => ura.User)
            .ToListAsync();
    }

    #endregion

    #region Permission Management

    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
        return await _context.Permissions
            .OrderBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync();
    }

    public async Task<Permission?> GetPermissionByIdAsync(Guid permissionId)
    {
        return await _context.Permissions.FindAsync(permissionId);
    }

    public async Task<Permission> CreatePermissionAsync(CreatePermissionRequest request)
    {
        try
        {
            var permission = new Permission
            {
                PermissionId = Guid.NewGuid(),
                Resource = request.Resource,
                Action = request.Action,
                Conditions = request.Conditions,
                CreatedAt = DateTime.UtcNow
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return permission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission {Resource}:{Action}", request.Resource, request.Action);
            throw;
        }
    }

    public async Task<bool> DeletePermissionAsync(Guid permissionId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Remove role permissions first
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.PermissionId == permissionId)
                .ToListAsync();
            _context.RolePermissions.RemoveRange(rolePermissions);

            // Remove permission
            var permission = await _context.Permissions.FindAsync(permissionId);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting permission {PermissionId}", permissionId);
            return false;
        }
    }

    #endregion

    #region Role Permission Management

    public async Task<bool> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
    {
        try
        {
            var existingAssignment = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (existingAssignment != null)
                return true;

            var rolePermission = new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = permissionId,
                AssignedAt = DateTime.UtcNow
            };

            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permission {PermissionId} to role {RoleId}", permissionId, roleId);
            return false;
        }
    }

    public async Task<bool> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
        try
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (rolePermission != null)
            {
                _context.RolePermissions.Remove(rolePermission);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permission {PermissionId} from role {RoleId}", permissionId, roleId);
            return false;
        }
    }

    public async Task<List<Permission>> GetRolePermissionsAsync(Guid roleId)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission)
            .ToListAsync();
    }

    #endregion

    #region Audit and Logging

    public async Task LogUserActionAsync(Guid userId, string action, string resourceType, Guid resourceId, 
        string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var auditLog = new BankingAuditLog
            {
                AuditId = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                ResourceType = resourceType,
                ResourceId = resourceId,
                OldValues = oldValues,
                NewValues = newValues,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            _context.BankingAuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging user action for user {UserId}", userId);
        }
    }

    #endregion

    #region Private Helper Methods

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "WekeezaMVP4SecretKeyThatIsAtLeast32CharactersLong123456";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "WekeezaMVP4";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "WekeezaMVP4Users";

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task LogFailedLoginAttemptAsync(string username, string reason)
    {
        _logger.LogWarning("Failed login attempt for username {Username}: {Reason}", username, reason);
    }

    private async Task IncrementFailedLoginAttemptsAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.FailedLoginAttempts++;
            await _context.SaveChangesAsync();
        }
    }

    private async Task ResetFailedLoginAttemptsAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.FailedLoginAttempts = 0;
            await _context.SaveChangesAsync();
        }
    }

    private async Task UpdateLastLoginAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    #endregion
}