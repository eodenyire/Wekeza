using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Api.Middleware;

/// <summary>
///📂 Wekeza.Core.Api/Middleware/ExceptionHandlingMiddleware.cs
///This is the "Standard of Excellence" for banking APIs in 2026. It categorizes errors into specific HTTP status codes so the Mobile App or Web Portal knows exactly how to react.
/// The Universal Guard: Ensures Wekeza Bank never returns a raw system error.
/// Maps internal Domain and Application exceptions to secure HTTP responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[WEKEZA GATEWAY ERROR] {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError; // Default to 500
        var result = string.Empty;

        // The "Beast" Switch: Map specific exceptions to HTTP status codes
        switch (exception)
        {
            // 400: User input errors (FluentValidation)
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { 
                    message = "Validation Failed", 
                    errors = validationException.Errors 
                });
                break;

            // 400: Business Rule violations (e.g., Insufficient Funds)
            case DomainException domainEx:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { 
                    message = domainEx.Message, 
                    errorCode = domainEx.ErrorCode 
                });
                break;

            // 404: Resource missing
            case NotFoundException _:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new { 
                    message = "The requested resource was not found." 
                });
                break;

            // 403: Security/Mandate violations
            case UnauthorizedAccessException _:
                code = HttpStatusCode.Forbidden;
                result = JsonSerializer.Serialize(new { 
                    message = "You do not have permission to perform this action." 
                });
                break;

            // 500: Unexpected system failures (Database down, etc.)
            default:
                code = HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(new { 
                    message = "An internal system error occurred. Our engineering team has been notified." 
                });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
