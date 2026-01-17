using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Infrastructure.Services;

/// <summary>
/// Implementation of ICurrentUserService using ASP.NET Core HttpContext
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
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public IEnumerable<UserRole> Roles
    {
        get
        {
            var roleClaims = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
            if (roleClaims == null) return Enumerable.Empty<UserRole>();

            return roleClaims
                .Select(c => Enum.TryParse<UserRole>(c.Value, out var role) ? role : (UserRole?)null)
                .Where(r => r.HasValue)
                .Select(r => r!.Value)
                .ToList();
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(UserRole role)
    {
        return Roles.Contains(role);
    }
}
