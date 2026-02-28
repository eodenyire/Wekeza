using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// System Admin Service - User, Role, and System Configuration Management
/// Production-grade enterprise admin portal for system administrators
/// </summary>
public interface ISystemAdminService
{
    // ===== User Management =====
    Task<UserAdminDTO> GetUserAsync(Guid userId);
    Task<List<UserAdminDTO>> SearchUsersAsync(string? searchTerm = null, UserStatus? status = null, int page = 1, int pageSize = 50);
    Task<UserAdminDTO> CreateUserAsync(CreateUserRequest request);
    Task<UserAdminDTO> UpdateUserAsync(Guid userId, UpdateUserRequest request);
    Task DisableUserAsync(Guid userId, string reason);
    Task EnableUserAsync(Guid userId);
    Task ResetPasswordAsync(Guid userId, string tempPassword);
    Task UnlockUserAsync(Guid userId);
    
    // ===== Role Management =====
    Task<RoleAdminDTO> GetRoleAsync(Guid roleId);
    Task<List<RoleAdminDTO>> GetAllRolesAsync();
    Task<List<RoleAdminDTO>> SearchRolesAsync(string? searchTerm = null, int page = 1, int pageSize = 50);
    Task<RoleAdminDTO> CreateRoleAsync(CreateRoleRequest request);
    Task<RoleAdminDTO> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request);
    Task DeleteRoleAsync(Guid roleId);
    Task<List<PermissionDTO>> GetRolePermissionsAsync(Guid roleId);
    Task AddPermissionsToRoleAsync(Guid roleId, List<string> permissionCodes);
    Task RemovePermissionsFromRoleAsync(Guid roleId, List<string> permissionCodes);
    
    // ===== User Role Assignment =====
    Task AssignRolesToUserAsync(Guid userId, List<Guid> roleIds);
    Task RemoveRolesFromUserAsync(Guid userId, List<Guid> roleIds);
    
    // ===== Session Management =====
    Task<AdminSessionDTO> GetUserActiveSessionAsync(Guid userId);
    Task<List<AdminSessionDTO>> GetAllActiveSessionsAsync(int page = 1, int pageSize = 50);
    Task TerminateSessionAsync(Guid sessionId);
    Task TerminateAllUserSessionsAsync(Guid userId);
    
    // ===== System Configuration =====
    Task<SystemConfigurationDTO> GetConfigurationAsync(Guid configId);
    Task<List<SystemConfigurationDTO>> SearchConfigurationsAsync(string? category = null, string? configCode = null, int page = 1, int pageSize = 50);
    Task<SystemConfigurationDTO> CreateConfigurationAsync(CreateConfigurationRequest request);
    Task<SystemConfigurationDTO> UpdateConfigurationAsync(Guid configId, UpdateConfigurationRequest request);
    Task SubmitConfigurationForApprovalAsync(Guid configId, string reason);
    Task ApproveConfigurationAsync(Guid configId, Guid approverUserId, string approvalReason);
    Task RejectConfigurationAsync(Guid configId, Guid approverUserId, string rejectionReason);
    Task ActivateConfigurationAsync(Guid configId);
    
    // ===== Audit Trail =====
    Task<List<AuditLogDTO>> GetAuditTrailAsync(Guid? userId = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 100);
    Task<List<AuditLogDTO>> GetUserChangeHistoryAsync(Guid userId);
    Task<List<AuditLogDTO>> GetConfigurationChangeHistoryAsync(Guid configId);
}

// ===== DTOs =====
public record UserAdminDTO(
    Guid UserId,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string? Department,
    string? JobTitle,
    UserStatus Status,
    bool MfaEnabled,
    int FailedLoginAttempts,
    DateTime? LockedUntil,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    List<Guid> RoleIds = null);

public record RoleAdminDTO(
    Guid RoleId,
    string RoleCode,
    string RoleName,
    string Description,
    RoleType Type,
    RoleStatus Status,
    int PermissionCount,
    DateTime CreatedAt);

public record PermissionDTO(
    string Code,
    string Name,
    string Resource,
    string Action);

public record AdminSessionDTO(
    Guid SessionId,
    Guid UserId,
    string Username,
    string IpAddress,
    string UserAgent,
    DateTime LoginAt,
    DateTime? LastActivityAt,
    TimeSpan? SessionDuration,
    int ActionCount,
    string RiskLevel,
    AdminSessionStatus Status);

public record SystemConfigurationDTO(
    Guid ConfigId,
    string ConfigCode,
    string ConfigName,
    string Category,
    ConfigurationStatus Status,
    string Version,
    DateTime? ActivationDate,
    bool IsProductionReady,
    DateTime CreatedAt,
    string CreatedBy);

public record AuditLogDTO(
    Guid AuditId,
    Guid? UserId,
    string? Username,
    string Action,
    string Resource,
    string Details,
    DateTime Timestamp);

// ===== Commands/Requests =====
public record CreateUserRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string EmployeeId,
    string? Department = null,
    string? JobTitle = null,
    string TimeZone = "UTC",
    string Language = "en-US");

public record UpdateUserRequest(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? PhoneNumber = null,
    string? Department = null,
    string? JobTitle = null);

public record CreateRoleRequest(
    string RoleCode,
    string RoleName,
    string Description,
    RoleType Type,
    SecurityClearanceLevel RequiredClearance = SecurityClearanceLevel.Standard);

public record UpdateRoleRequest(
    string? RoleName = null,
    string? Description = null);

public record CreateConfigurationRequest(
    string ConfigCode,
    string ConfigName,
    string Category,
    string ConfigType,
    Dictionary<string, object> ConfigurationData);

public record UpdateConfigurationRequest(
    Dictionary<string, object> ConfigurationData,
    string? ChangeReason = null);
