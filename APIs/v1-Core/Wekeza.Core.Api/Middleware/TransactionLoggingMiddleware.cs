using Microsoft.Extensions.Logging;

namespace Wekeza.Core.Api.Middleware;

/// <summary>
/// The Audit Guard: Captures raw request metadata for forensics and risk analysis.
/// Ensures every attempt to touch the bank's ledger is recorded.
/// </summary>
public class TransactionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TransactionLoggingMiddleware> _logger;

    public TransactionLoggingMiddleware(RequestDelegate next, ILogger<TransactionLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Capture metadata: IP, Method, Path, and User Agent
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation(
            "[WEKEZA AUDIT] Incoming {Method} request to {Path} from IP: {IP}",
            method, path, ipAddress);

        await _next(context);
    }
}
