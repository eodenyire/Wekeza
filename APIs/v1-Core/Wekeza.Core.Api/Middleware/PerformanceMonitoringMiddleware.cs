using System.Diagnostics;
using Wekeza.Core.Infrastructure.Monitoring;

namespace Wekeza.Core.Api.Middleware;

/// <summary>
/// Performance monitoring middleware for tracking request performance
/// Provides comprehensive performance metrics and monitoring for all API requests
/// </summary>
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
    private readonly IPerformanceMonitoringService _performanceMonitoring;

    public PerformanceMonitoringMiddleware(
        RequestDelegate next,
        ILogger<PerformanceMonitoringMiddleware> logger,
        IPerformanceMonitoringService performanceMonitoring)
    {
        _next = next;
        _logger = logger;
        _performanceMonitoring = performanceMonitoring;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        var operationName = GetOperationName(context);
        
        // Add request ID to context
        context.Items["RequestId"] = requestId;
        context.Items["OperationName"] = operationName;
        
        // Start performance monitoring
        using var performanceScope = await _performanceMonitoring.StartRequestMonitoringAsync(
            operationName, 
            GetRequestProperties(context, requestId));

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        Exception? exception = null;
        var statusCode = 200;

        try
        {
            await _next(context);
            statusCode = context.Response.StatusCode;
        }
        catch (Exception ex)
        {
            exception = ex;
            statusCode = 500;
            
            // Record exception
            await _performanceMonitoring.RecordExceptionAsync(
                ex, 
                operationName, 
                GetRequestProperties(context, requestId));
            
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            // Record request performance
            var success = statusCode >= 200 && statusCode < 400;
            await _performanceMonitoring.RecordRequestAsync(
                operationName,
                stopwatch.Elapsed,
                success,
                GetResponseProperties(context, requestId, statusCode, stopwatch.Elapsed));

            // Log performance metrics
            LogPerformanceMetrics(context, requestId, operationName, stopwatch.Elapsed, statusCode, exception);

            // Copy response back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private string GetOperationName(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";
        
        // Extract controller and action from route data
        var controller = context.GetRouteValue("controller")?.ToString();
        var action = context.GetRouteValue("action")?.ToString();
        
        if (!string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action))
        {
            return $"{controller}.{action}";
        }
        
        return $"{method} {path}";
    }

    private Dictionary<string, object> GetRequestProperties(HttpContext context, string requestId)
    {
        var properties = new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["Method"] = context.Request.Method,
            ["Path"] = context.Request.Path.Value ?? "/",
            ["QueryString"] = context.Request.QueryString.Value ?? "",
            ["UserAgent"] = context.Request.Headers.UserAgent.ToString(),
            ["IpAddress"] = GetClientIpAddress(context),
            ["ContentLength"] = context.Request.ContentLength ?? 0,
            ["Timestamp"] = DateTime.UtcNow
        };

        // Add user information if available
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            properties["UserId"] = context.User.Identity.Name ?? "Unknown";
            properties["IsAuthenticated"] = true;
        }
        else
        {
            properties["IsAuthenticated"] = false;
        }

        // Add route values
        foreach (var routeValue in context.Request.RouteValues)
        {
            properties[$"Route.{routeValue.Key}"] = routeValue.Value?.ToString() ?? "";
        }

        // Add important headers
        var importantHeaders = new[] { "Authorization", "Content-Type", "Accept", "X-API-Key" };
        foreach (var header in importantHeaders)
        {
            if (context.Request.Headers.ContainsKey(header))
            {
                var value = context.Request.Headers[header].ToString();
                // Mask sensitive headers
                if (header == "Authorization" && !string.IsNullOrEmpty(value))
                {
                    value = value.Length > 10 ? value.Substring(0, 10) + "***" : "***";
                }
                properties[$"Header.{header}"] = value;
            }
        }

        return properties;
    }

    private Dictionary<string, object> GetResponseProperties(HttpContext context, string requestId, int statusCode, TimeSpan duration)
    {
        var properties = new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["StatusCode"] = statusCode,
            ["Duration"] = duration.TotalMilliseconds,
            ["ResponseSize"] = context.Response.Body.Length,
            ["ContentType"] = context.Response.ContentType ?? "",
            ["Timestamp"] = DateTime.UtcNow
        };

        // Add response headers
        var importantResponseHeaders = new[] { "Content-Type", "Content-Length", "Cache-Control" };
        foreach (var header in importantResponseHeaders)
        {
            if (context.Response.Headers.ContainsKey(header))
            {
                properties[$"ResponseHeader.{header}"] = context.Response.Headers[header].ToString();
            }
        }

        // Categorize response
        properties["ResponseCategory"] = GetResponseCategory(statusCode);
        properties["IsSuccess"] = statusCode >= 200 && statusCode < 400;
        properties["IsClientError"] = statusCode >= 400 && statusCode < 500;
        properties["IsServerError"] = statusCode >= 500;

        return properties;
    }

    private void LogPerformanceMetrics(HttpContext context, string requestId, string operationName, TimeSpan duration, int statusCode, Exception? exception)
    {
        var logLevel = GetLogLevel(statusCode, duration);
        var message = "Request {OperationName} completed in {Duration}ms with status {StatusCode}";
        
        var logData = new
        {
            RequestId = requestId,
            OperationName = operationName,
            Duration = duration.TotalMilliseconds,
            StatusCode = statusCode,
            Method = context.Request.Method,
            Path = context.Request.Path.Value,
            IpAddress = GetClientIpAddress(context),
            UserAgent = context.Request.Headers.UserAgent.ToString(),
            UserId = context.User?.Identity?.Name,
            IsAuthenticated = context.User?.Identity?.IsAuthenticated == true,
            ResponseSize = context.Response.Body.Length,
            Exception = exception?.Message
        };

        _logger.Log(logLevel, exception, message, operationName, duration.TotalMilliseconds, statusCode);
        
        // Log additional details at debug level
        _logger.LogDebug("Request details: {@LogData}", logData);
    }

    private LogLevel GetLogLevel(int statusCode, TimeSpan duration)
    {
        // Log as warning if request takes too long
        if (duration.TotalMilliseconds > 5000) // 5 seconds
            return LogLevel.Warning;
            
        // Log as error for server errors
        if (statusCode >= 500)
            return LogLevel.Error;
            
        // Log as warning for client errors
        if (statusCode >= 400)
            return LogLevel.Warning;
            
        // Log as warning for slow requests
        if (duration.TotalMilliseconds > 1000) // 1 second
            return LogLevel.Warning;
            
        return LogLevel.Information;
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP first (for load balancers/proxies)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private string GetResponseCategory(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => "Success",
            >= 300 and < 400 => "Redirection",
            >= 400 and < 500 => "ClientError",
            >= 500 => "ServerError",
            _ => "Unknown"
        };
    }
}

/// <summary>
/// Extension methods for adding performance monitoring middleware
/// </summary>
public static class PerformanceMonitoringMiddlewareExtensions
{
    /// <summary>
    /// Add performance monitoring middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceMonitoringMiddleware>();
    }
}