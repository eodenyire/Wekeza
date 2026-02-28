using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Administration.Commands.CreateUser;

/// <summary>
/// Command to create a new user in the system
/// </summary>
[Authorize(UserRole.Administrator, UserRole.ITAdministrator)]
public record CreateUserCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    // Basic Information
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string EmployeeId { get; init; } = string.Empty;
    
    // Contact Information
    public string? PhoneNumber { get; init; }
    public string? Department { get; init; }
    public string? JobTitle { get; init; }
    public string? ManagerId { get; init; }
    
    // Role and Security
    public List<UserRole> Roles { get; init; } = new();
    public SecurityClearanceLevel SecurityClearance { get; init; } = SecurityClearanceLevel.Basic;
    public bool MustChangePassword { get; init; } = true;
    public bool MfaEnabled { get; init; } = false;
    
    // Branch Assignment
    public Guid? BranchId { get; init; }
    public string? BranchCode { get; init; }
    
    // Preferences
    public string TimeZone { get; init; } = "UTC";
    public string Language { get; init; } = "en-US";
    
    // Temporary Password (will be generated if not provided)
    public string? TemporaryPassword { get; init; }
}