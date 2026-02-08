using MediatR;
using Microsoft.Extensions.Logging;
using Wekeza.Core.Application.Common.Interfaces;
using System.Text.Json;
///
///2. AuditBehavior.cs (The Global Watcher)
///This behavior sits in the pipeline and specifically targets Commands (Actions) rather than Queries (Viewing).
///
///


namespace Wekeza.Core.Application.Common.Behaviors;

public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<TRequest> _logger;

    public AuditBehavior(ICurrentUserService currentUser, ILogger<TRequest> logger)
    {
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;

        // We only audit Commands (Actions that change state)
        if (requestName.EndsWith("Command"))
        {
            var userId = _currentUser.UserId?.ToString() ?? "System";
            var userName = _currentUser.UserName ?? "Unauthorized_Access";

            // LOG: Audit Trail for Risk Management
            _logger.LogInformation("[WEKEZA AUDIT] User: {User} ({Id}) attempted {Action} with Data: {Data}",
                userName, userId, requestName, JsonSerializer.Serialize(request));
        }

        return await next();
    }
}
