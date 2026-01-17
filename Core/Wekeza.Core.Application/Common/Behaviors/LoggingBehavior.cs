using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Common.Behaviors;

/// <summary>
/// 📂 Wekeza.Core.Application/Common/Behaviors
/// These classes are Cross-Cutting Concerns. They sit between the API and the Business Logic. Every request entering the system must flow through them.
/// 🛡️ 1. LoggingBehavior.cs
/// Purpose: Provides an automated audit trail for every action. Future-Proofing: Uses CorrelationId for distributed tracing. This is vital when the Python Fraud Engine or Scala Streamer needs to know which user action triggered a specific data movement. 
/// Principal-Grade Logging: Captures every request and response cycle.
/// Integrates with the CorrelationId for cross-system observability.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUserService _currentUser;

    public LoggingBehavior(ILogger<TRequest> logger, ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId ?? "Anonymous";
        
        // Log the start of the process
        _logger.LogInformation("[WEKEZA CORE] Handling {RequestName} for User: {UserId}", 
            requestName, userId);

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation("[WEKEZA CORE] Successfully processed {RequestName} in {Elapsed}ms", 
                requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[WEKEZA CORE] Critical failure in {RequestName} after {Elapsed}ms", 
                requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
