using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Infrastructure.Services;

/// <summary>
/// Provides access to the current authenticated user's information
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }

    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public IEnumerable<UserRole> Roles
    {
        get
        {
            var roleClaims = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role) 
                ?? Enumerable.Empty<Claim>();
            
            var roles = new List<UserRole>();
            foreach (var roleClaim in roleClaims)
            {
                if (Enum.TryParse<UserRole>(roleClaim.Value, true, out var role))
                {
                    roles.Add(role);
                }
            }
            return roles;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(UserRole role)
    {
        return Roles.Contains(role);
    }
}