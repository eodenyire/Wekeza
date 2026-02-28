using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Wekeza.Core.Api.Middleware;

/// <summary>
/// The Performance Watchtower: Tracks the execution time of every request.
/// Logs warnings for "Slow Requests" exceeding the 500ms banking threshold.
/// </summary>
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;
    private readonly Stopwatch _timer;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task Invoke(HttpContext context)
    {
        _timer.Start();

        await _next(context);

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        // In a Tier-1 Bank, any request > 500ms is considered "Slow"
        if (elapsedMilliseconds > 500)
        {
            var requestName = context.Request.Path;
            var userId = context.User.Identity?.Name ?? "Anonymous";

            _logger.LogWarning(
                "[WEKEZA PERFORMANCE] Slow Request: {Name} ({Elapsed} ms) conducted by {UserId}",
                requestName, elapsedMilliseconds, userId);
        }

        _timer.Reset();
    }
}
