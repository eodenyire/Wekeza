using MediatR;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that enforces authorization rules
/// </summary>
public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationBehavior(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType()
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .Cast<AuthorizeAttribute>()
            .ToList();

        if (!authorizeAttributes.Any())
        {
            // No authorization required
            return await next();
        }

        // User must be authenticated
        if (_currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        // Check if user has any of the required roles
        var requiredRoles = authorizeAttributes.SelectMany(a => a.Roles).Distinct();
        var userRoles = _currentUserService.Roles;

        var hasRequiredRole = requiredRoles.Any(role => userRoles.Contains(role));

        if (!hasRequiredRole)
        {
            throw new ForbiddenAccessException(
                $"User does not have permission to perform this action. Required roles: {string.Join(", ", requiredRoles)}");
        }

        return await next();
    }
}
