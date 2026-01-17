using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Wekeza.Core.Api.Extensions;

/// <summary>
/// Rate limiting configuration to protect against abuse
/// </summary>
public static class RateLimitingExtensions
{
    public static IServiceCollection AddWekezaRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Global rate limit: 100 requests per minute per IP
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            // Strict policy for authentication endpoints
            options.AddPolicy("authentication", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 5,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            // Moderate policy for transaction endpoints
            options.AddPolicy("transactions", context =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: context.User.Identity?.Name ?? "anonymous",
                    factory: partition => new SlidingWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 50,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4
                    }));

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests",
                    message = "Rate limit exceeded. Please try again later.",
                    retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) 
                        ? retryAfter.TotalSeconds 
                        : null
                }, cancellationToken: token);
            };
        });

        return services;
    }
}
