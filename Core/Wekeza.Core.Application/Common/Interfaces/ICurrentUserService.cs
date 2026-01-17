using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Common.Interfaces;

/// <summary>
/// Service to access current authenticated user information
/// Crucial for the Audit Trail and Risk Management modules.
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Username { get; }
    string? Email { get; }
    IEnumerable<UserRole> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(UserRole role);
}
