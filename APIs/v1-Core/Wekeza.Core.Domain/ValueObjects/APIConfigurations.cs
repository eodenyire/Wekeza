using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.ValueObjects;

public class RateLimitConfig : ValueObject
{
    public int RequestsPerMinute { get; private set; }
    public int RequestsPerHour { get; private set; }
    public int RequestsPerDay { get; private set; }
    public RateLimitType Type { get; private set; }

    private RateLimitConfig() { } // EF Core

    public RateLimitConfig(int requestsPerMinute, int requestsPerHour, int requestsPerDay, RateLimitType type)
    {
        RequestsPerMinute = requestsPerMinute;
        RequestsPerHour = requestsPerHour;
        RequestsPerDay = requestsPerDay;
        Type = type;
    }

    public bool IsRateLimited(string clientId)
    {
        // Simple implementation - in real scenario this would check against a cache/database
        // For now, always return false to avoid blocking
        return false;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RequestsPerMinute;
        yield return RequestsPerHour;
        yield return RequestsPerDay;
        yield return Type;
    }
}

public class AuthenticationConfig : ValueObject
{
    public bool RequireHttps { get; private set; }
    public int TokenExpiryMinutes { get; private set; }
    public AuthenticationMethod Method { get; private set; }

    private AuthenticationConfig() { } // EF Core

    public AuthenticationConfig(bool requireHttps, int tokenExpiryMinutes, AuthenticationMethod method)
    {
        RequireHttps = requireHttps;
        TokenExpiryMinutes = tokenExpiryMinutes;
        Method = method;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RequireHttps;
        yield return TokenExpiryMinutes;
        yield return Method;
    }
}

public class CacheConfig : ValueObject
{
    public int TTLSeconds { get; private set; }
    public CacheStrategy Strategy { get; private set; }
    public int MaxSize { get; private set; }

    private CacheConfig() { } // EF Core

    public CacheConfig(int ttlSeconds, CacheStrategy strategy, int maxSize)
    {
        TTLSeconds = ttlSeconds;
        Strategy = strategy;
        MaxSize = maxSize;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TTLSeconds;
        yield return Strategy;
        yield return MaxSize;
    }
}

public class UpstreamServer : ValueObject
{
    public string Name { get; private set; }
    public string BaseUrl { get; private set; }
    public string Url => BaseUrl; // Alias for compatibility
    public int Weight { get; private set; }
    public bool IsHealthy { get; private set; }
    public int ActiveConnections { get; private set; }

    private UpstreamServer() { } // EF Core

    public UpstreamServer(string name, string baseUrl, int weight = 1, bool isHealthy = true)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        Weight = weight;
        IsHealthy = isHealthy;
        ActiveConnections = 0;
    }

    public void UpdateHealth(bool isHealthy, string? healthCheckResponse = null)
    {
        IsHealthy = isHealthy;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return BaseUrl;
        yield return Weight;
        yield return IsHealthy;
    }
}

public class HealthCheckConfig : ValueObject
{
    public string Endpoint { get; private set; }
    public int IntervalSeconds { get; private set; }
    public int TimeoutSeconds { get; private set; }

    private HealthCheckConfig() { } // EF Core

    public HealthCheckConfig(string endpoint, int intervalSeconds, int timeoutSeconds)
    {
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        IntervalSeconds = intervalSeconds;
        TimeoutSeconds = timeoutSeconds;
    }

    public static HealthCheckConfig Default()
    {
        return new HealthCheckConfig("/health", 30, 5);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Endpoint;
        yield return IntervalSeconds;
        yield return TimeoutSeconds;
    }
}

public class APIMetrics : ValueObject
{
    public int RequestCount { get; private set; }
    public long TotalRequests => RequestCount; // Alias for compatibility
    public double AverageResponseTime { get; private set; }
    public int ErrorCount { get; private set; }
    public double ErrorRate => RequestCount > 0 ? (double)ErrorCount / RequestCount * 100 : 0;
    public DateTime LastUpdated { get; private set; }

    public APIMetrics() // Public constructor
    {
        RequestCount = 0;
        AverageResponseTime = 0;
        ErrorCount = 0;
        LastUpdated = DateTime.UtcNow;
    }

    public APIMetrics(int requestCount, double averageResponseTime, int errorCount, DateTime lastUpdated)
    {
        RequestCount = requestCount;
        AverageResponseTime = averageResponseTime;
        ErrorCount = errorCount;
        LastUpdated = lastUpdated;
    }

    public void RecordRequest(string endpoint, string method, TimeSpan responseTime, int statusCode)
    {
        RequestCount++;
        if (statusCode >= 400)
        {
            ErrorCount++;
        }
        
        // Update average response time
        AverageResponseTime = ((AverageResponseTime * (RequestCount - 1)) + responseTime.TotalMilliseconds) / RequestCount;
        LastUpdated = DateTime.UtcNow;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RequestCount;
        yield return AverageResponseTime;
        yield return ErrorCount;
        yield return LastUpdated;
    }
}

public class APILog : ValueObject
{
    public string Method { get; private set; }
    public string Path { get; private set; }
    public string Endpoint => Path; // Alias for compatibility
    public int StatusCode { get; private set; }
    public double ResponseTime { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? ClientIP { get; private set; }

    public APILog() // Public constructor
    {
        Method = string.Empty;
        Path = string.Empty;
        StatusCode = 0;
        ResponseTime = 0;
        Timestamp = DateTime.UtcNow;
    }

    public APILog(string method, string path, int statusCode, double responseTime, DateTime timestamp)
    {
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        StatusCode = statusCode;
        ResponseTime = responseTime;
        Timestamp = timestamp;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Method;
        yield return Path;
        yield return StatusCode;
        yield return ResponseTime;
        yield return Timestamp;
    }
}

public class CircuitBreakerState : ValueObject
{
    public int FailureCount { get; private set; }
    public int SuccessCount { get; private set; }
    public DateTime? LastFailureTime { get; private set; }
    public DateTime? CircuitOpenedAt { get; private set; }
    public bool IsOpen { get; private set; }
    public TimeSpan? Timeout { get; private set; }

    public CircuitBreakerState()
    {
        FailureCount = 0;
        SuccessCount = 0;
        IsOpen = false;
    }

    public void RecordSuccess()
    {
        SuccessCount++;
        FailureCount = 0; // Reset failure count on success
        
        // Close circuit if it was open
        if (IsOpen)
        {
            IsOpen = false;
            CircuitOpenedAt = null;
            Timeout = null;
        }
    }

    public void RecordFailure()
    {
        FailureCount++;
        LastFailureTime = DateTime.UtcNow;
    }

    public bool ShouldOpenCircuit(int failureThreshold)
    {
        return FailureCount >= failureThreshold && !IsOpen;
    }

    public void OpenCircuit(TimeSpan timeout)
    {
        IsOpen = true;
        CircuitOpenedAt = DateTime.UtcNow;
        Timeout = timeout;
    }

    public bool CanAttemptRequest()
    {
        if (!IsOpen)
            return true;

        if (CircuitOpenedAt.HasValue && Timeout.HasValue)
        {
            var timeElapsed = DateTime.UtcNow - CircuitOpenedAt.Value;
            if (timeElapsed >= Timeout.Value)
            {
                // Allow one test request
                return true;
            }
        }

        return false;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FailureCount;
        yield return SuccessCount;
        yield return IsOpen;
        yield return LastFailureTime ?? DateTime.MinValue;
    }
}
