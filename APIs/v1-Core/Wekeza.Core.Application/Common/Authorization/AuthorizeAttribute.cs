using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Common.Authorization;

/// <summary>
/// Specifies the roles required to execute a command or query
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AuthorizeAttribute : Attribute
{
    public UserRole[] Roles { get; }

    public AuthorizeAttribute(params UserRole[] roles)
    {
        Roles = roles;
    }
}
