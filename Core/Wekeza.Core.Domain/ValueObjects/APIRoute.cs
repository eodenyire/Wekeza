using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.ValueObjects;

/// <summary>
/// API Route Value Object - Represents routing configuration for API Gateway
/// Immutable value object containing route path, upstream configuration, and policies
/// Industry Standard: API Gateway patterns (Kong, Zuul, Ocelot, AWS API Gateway)
/// </summary>
public class ApiRoute : ValueObject
{
    public string Path { get; }
    public string Method { get; }
    public string UpstreamUrl { get; }
    public string UpstreamPath { get; }
    public Dictionary<string, string> Headers { get; }
    public Dictionary<string, string> QueryParameters { get; }
    public TimeSpan Timeout { get; }
    public int RetryCount { get; }
    public bool RequiresAuthentication { get; }
    public List<string> AllowedRoles { get; }
    public List<string> AllowedScopes { get; }
    
    // Rate limiting
    public bool RateLimitEnabled { get; }
    public int RateLimit { get; }
    public TimeSpan RateLimitWindow { get; }
    public RateLimitType RateLimitType { get; }
    
    // Caching
    public bool CacheEnabled { get; }
    public TimeSpan CacheDuration { get; }
    public List<string> CacheKeys { get; }
    public CacheStrategy CacheStrategy { get; }
    
    // Load balancing
    public LoadBalancingStrategy LoadBalancing { get; }
    public List<string> UpstreamServers { get; }
    public Dictionary<string, int> ServerWeights { get; }
    
    // Circuit breaker
    public bool CircuitBreakerEnabled { get; }
    public int CircuitBreakerThreshold { get; }
    public TimeSpan CircuitBreakerTimeout { get; }
    
    // Transformation
    public bool RequestTransformationEnabled { get; }
    public bool ResponseTransformationEnabled { get; }
    public string RequestTransformationScript { get; }
    public string ResponseTransformationScript { get; }
    
    // Security
    public SecurityLevel SecurityLevel { get; }
    public bool RequireHttps { get; }
    public List<string> AllowedOrigins { get; }
    public List<string> AllowedIpAddresses { get; }
    
    // Monitoring
    public MonitoringLevel MonitoringLevel { get; }
    public bool LogRequests { get; }
    public bool LogResponses { get; }
    public LogLevel LogLevel { get; }
    
    // Compression
    public CompressionType CompressionType { get; }
    public int CompressionThreshold { get; }
    
    // Additional properties
    public Dictionary<string, object> Properties { get; }
    public bool IsActive { get; }
    public int Priority { get; }
    public string Description { get; }

    public ApiRoute(
        string path,
        string method,
        string upstreamUrl,
        string upstreamPath = null,
        Dictionary<string, string> headers = null,
        Dictionary<string, string> queryParameters = null,
        TimeSpan? timeout = null,
        int retryCount = 3,
        bool requiresAuthentication = true,
        List<string> allowedRoles = null,
        List<string> allowedScopes = null,
        bool rateLimitEnabled = true,
        int rateLimit = 100,
        TimeSpan? rateLimitWindow = null,
        RateLimitType rateLimitType = RateLimitType.PerMinute,
        bool cacheEnabled = false,
        TimeSpan? cacheDuration = null,
        List<string> cacheKeys = null,
        CacheStrategy cacheStrategy = CacheStrategy.TimeToLive,
        LoadBalancingStrategy loadBalancing = LoadBalancingStrategy.RoundRobin,
        List<string> upstreamServers = null,
        Dictionary<string, int> serverWeights = null,
        bool circuitBreakerEnabled = true,
        int circuitBreakerThreshold = 5,
        TimeSpan? circuitBreakerTimeout = null,
        bool requestTransformationEnabled = false,
        bool responseTransformationEnabled = false,
        string requestTransformationScript = null,
        string responseTransformationScript = null,
        SecurityLevel securityLevel = SecurityLevel.Standard,
        bool requireHttps = true,
        List<string> allowedOrigins = null,
        List<string> allowedIpAddresses = null,
        MonitoringLevel monitoringLevel = MonitoringLevel.Standard,
        bool logRequests = true,
        bool logResponses = false,
        LogLevel logLevel = LogLevel.Information,
        CompressionType compressionType = CompressionType.Gzip,
        int compressionThreshold = 1024,
        Dictionary<string, object> properties = null,
        bool isActive = true,
        int priority = 0,
        string description = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));
        
        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("Method cannot be empty", nameof(method));
        
        if (string.IsNullOrWhiteSpace(upstreamUrl))
            throw new ArgumentException("Upstream URL cannot be empty", nameof(upstreamUrl));

        // Validate path format
        if (!path.StartsWith("/"))
            throw new ArgumentException("Path must start with '/'", nameof(path));

        // Validate HTTP method
        var validMethods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };
        if (!validMethods.Contains(method.ToUpperInvariant()))
            throw new ArgumentException($"Invalid HTTP method: {method}", nameof(method));

        // Validate upstream URL
        if (!Uri.TryCreate(upstreamUrl, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid upstream URL format", nameof(upstreamUrl));

        // Validate rate limit
        if (rateLimitEnabled && rateLimit <= 0)
            throw new ArgumentException("Rate limit must be positive when enabled", nameof(rateLimit));

        // Validate retry count
        if (retryCount < 0)
            throw new ArgumentException("Retry count cannot be negative", nameof(retryCount));

        // Validate circuit breaker threshold
        if (circuitBreakerEnabled && circuitBreakerThreshold <= 0)
            throw new ArgumentException("Circuit breaker threshold must be positive when enabled", nameof(circuitBreakerThreshold));

        Path = NormalizePath(path);
        Method = method.ToUpperInvariant();
        UpstreamUrl = upstreamUrl.TrimEnd('/');
        UpstreamPath = string.IsNullOrWhiteSpace(upstreamPath) ? path : NormalizePath(upstreamPath);
        Headers = headers ?? new Dictionary<string, string>();
        QueryParameters = queryParameters ?? new Dictionary<string, string>();
        Timeout = timeout ?? TimeSpan.FromSeconds(30);
        RetryCount = retryCount;
        RequiresAuthentication = requiresAuthentication;
        AllowedRoles = allowedRoles ?? new List<string>();
        AllowedScopes = allowedScopes ?? new List<string>();
        
        RateLimitEnabled = rateLimitEnabled;
        RateLimit = rateLimit;
        RateLimitWindow = rateLimitWindow ?? TimeSpan.FromMinutes(1);
        RateLimitType = rateLimitType;
        
        CacheEnabled = cacheEnabled;
        CacheDuration = cacheDuration ?? TimeSpan.FromMinutes(5);
        CacheKeys = cacheKeys ?? new List<string>();
        CacheStrategy = cacheStrategy;
        
        LoadBalancing = loadBalancing;
        UpstreamServers = upstreamServers ?? new List<string> { upstreamUrl };
        ServerWeights = serverWeights ?? new Dictionary<string, int>();
        
        CircuitBreakerEnabled = circuitBreakerEnabled;
        CircuitBreakerThreshold = circuitBreakerThreshold;
        CircuitBreakerTimeout = circuitBreakerTimeout ?? TimeSpan.FromMinutes(1);
        
        RequestTransformationEnabled = requestTransformationEnabled;
        ResponseTransformationEnabled = responseTransformationEnabled;
        RequestTransformationScript = requestTransformationScript;
        ResponseTransformationScript = responseTransformationScript;
        
        SecurityLevel = securityLevel;
        RequireHttps = requireHttps;
        AllowedOrigins = allowedOrigins ?? new List<string>();
        AllowedIpAddresses = allowedIpAddresses ?? new List<string>();
        
        MonitoringLevel = monitoringLevel;
        LogRequests = logRequests;
        LogResponses = logResponses;
        LogLevel = logLevel;
        
        CompressionType = compressionType;
        CompressionThreshold = compressionThreshold;
        
        Properties = properties ?? new Dictionary<string, object>();
        IsActive = isActive;
        Priority = priority;
        Description = description;
    }

    /// <summary>
    /// Create a new route with updated upstream URL
    /// </summary>
    public ApiRoute WithUpstreamUrl(string newUpstreamUrl)
    {
        if (string.IsNullOrWhiteSpace(newUpstreamUrl))
            throw new ArgumentException("Upstream URL cannot be empty", nameof(newUpstreamUrl));

        if (!Uri.TryCreate(newUpstreamUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid upstream URL format", nameof(newUpstreamUrl));

        return new ApiRoute(
            Path, Method, newUpstreamUrl, UpstreamPath, Headers, QueryParameters,
            Timeout, RetryCount, RequiresAuthentication, AllowedRoles, AllowedScopes,
            RateLimitEnabled, RateLimit, RateLimitWindow, RateLimitType,
            CacheEnabled, CacheDuration, CacheKeys, CacheStrategy,
            LoadBalancing, UpstreamServers, ServerWeights,
            CircuitBreakerEnabled, CircuitBreakerThreshold, CircuitBreakerTimeout,
            RequestTransformationEnabled, ResponseTransformationEnabled,
            RequestTransformationScript, ResponseTransformationScript,
            SecurityLevel, RequireHttps, AllowedOrigins, AllowedIpAddresses,
            MonitoringLevel, LogRequests, LogResponses, LogLevel,
            CompressionType, CompressionThreshold, Properties, IsActive, Priority, Description);
    }

    /// <summary>
    /// Create a new route with updated rate limiting
    /// </summary>
    public ApiRoute WithRateLimit(bool enabled, int limit = 100, TimeSpan? window = null, RateLimitType type = RateLimitType.PerMinute)
    {
        return new ApiRoute(
            Path, Method, UpstreamUrl, UpstreamPath, Headers, QueryParameters,
            Timeout, RetryCount, RequiresAuthentication, AllowedRoles, AllowedScopes,
            enabled, limit, window ?? RateLimitWindow, type,
            CacheEnabled, CacheDuration, CacheKeys, CacheStrategy,
            LoadBalancing, UpstreamServers, ServerWeights,
            CircuitBreakerEnabled, CircuitBreakerThreshold, CircuitBreakerTimeout,
            RequestTransformationEnabled, ResponseTransformationEnabled,
            RequestTransformationScript, ResponseTransformationScript,
            SecurityLevel, RequireHttps, AllowedOrigins, AllowedIpAddresses,
            MonitoringLevel, LogRequests, LogResponses, LogLevel,
            CompressionType, CompressionThreshold, Properties, IsActive, Priority, Description);
    }

    /// <summary>
    /// Create a new route with updated caching
    /// </summary>
    public ApiRoute WithCaching(bool enabled, TimeSpan? duration = null, List<string> keys = null, CacheStrategy strategy = CacheStrategy.TimeToLive)
    {
        return new ApiRoute(
            Path, Method, UpstreamUrl, UpstreamPath, Headers, QueryParameters,
            Timeout, RetryCount, RequiresAuthentication, AllowedRoles, AllowedScopes,
            RateLimitEnabled, RateLimit, RateLimitWindow, RateLimitType,
            enabled, duration ?? CacheDuration, keys ?? CacheKeys, strategy,
            LoadBalancing, UpstreamServers, ServerWeights,
            CircuitBreakerEnabled, CircuitBreakerThreshold, CircuitBreakerTimeout,
            RequestTransformationEnabled, ResponseTransformationEnabled,
            RequestTransformationScript, ResponseTransformationScript,
            SecurityLevel, RequireHttps, AllowedOrigins, AllowedIpAddresses,
            MonitoringLevel, LogRequests, LogResponses, LogLevel,
            CompressionType, CompressionThreshold, Properties, IsActive, Priority, Description);
    }

    /// <summary>
    /// Create a new route with updated authentication
    /// </summary>
    public ApiRoute WithAuthentication(bool required, List<string> roles = null, List<string> scopes = null)
    {
        return new ApiRoute(
            Path, Method, UpstreamUrl, UpstreamPath, Headers, QueryParameters,
            Timeout, RetryCount, required, roles ?? AllowedRoles, scopes ?? AllowedScopes,
            RateLimitEnabled, RateLimit, RateLimitWindow, RateLimitType,
            CacheEnabled, CacheDuration, CacheKeys, CacheStrategy,
            LoadBalancing, UpstreamServers, ServerWeights,
            CircuitBreakerEnabled, CircuitBreakerThreshold, CircuitBreakerTimeout,
            RequestTransformationEnabled, ResponseTransformationEnabled,
            RequestTransformationScript, ResponseTransformationScript,
            SecurityLevel, RequireHttps, AllowedOrigins, AllowedIpAddresses,
            MonitoringLevel, LogRequests, LogResponses, LogLevel,
            CompressionType, CompressionThreshold, Properties, IsActive, Priority, Description);
    }

    /// <summary>
    /// Create a new route with updated headers
    /// </summary>
    public ApiRoute WithHeaders(Dictionary<string, string> additionalHeaders)
    {
        var newHeaders = new Dictionary<string, string>(Headers);
        if (additionalHeaders != null)
        {
            foreach (var header in additionalHeaders)
            {
                newHeaders[header.Key] = header.Value;
            }
        }

        return new ApiRoute(
            Path, Method, UpstreamUrl, UpstreamPath, newHeaders, QueryParameters,
            Timeout, RetryCount, RequiresAuthentication, AllowedRoles, AllowedScopes,
            RateLimitEnabled, RateLimit, RateLimitWindow, RateLimitType,
            CacheEnabled, CacheDuration, CacheKeys, CacheStrategy,
            LoadBalancing, UpstreamServers, ServerWeights,
            CircuitBreakerEnabled, CircuitBreakerThreshold, CircuitBreakerTimeout,
            RequestTransformationEnabled, ResponseTransformationEnabled,
            RequestTransformationScript, ResponseTransformationScript,
            SecurityLevel, RequireHttps, AllowedOrigins, AllowedIpAddresses,
            MonitoringLevel, LogRequests, LogResponses, LogLevel,
            CompressionType, CompressionThreshold, Properties, IsActive, Priority, Description);
    }

    /// <summary>
    /// Create a new route with updated status
    /// </summary>
    public ApiRoute WithStatus(bool active)
    {
        return new ApiRoute(
            Path, Method, UpstreamUrl, UpstreamPath, Headers, QueryParameters,
            Timeout, RetryCount, RequiresAuthentication, AllowedRoles, AllowedScopes,
            RateLimitEnabled, RateLimit, RateLimitWindow, RateLimitType,
            CacheEnabled, CacheDuration, CacheKeys, CacheStrategy,
            LoadBalancing, UpstreamServers, ServerWeights,
            CircuitBreakerEnabled, CircuitBreakerThreshold, CircuitBreakerTimeout,
            RequestTransformationEnabled, ResponseTransformationEnabled,
            RequestTransformationScript, ResponseTransformationScript,
            SecurityLevel, RequireHttps, AllowedOrigins, AllowedIpAddresses,
            MonitoringLevel, LogRequests, LogResponses, LogLevel,
            CompressionType, CompressionThreshold, Properties, active, Priority, Description);
    }

    /// <summary>
    /// Check if route matches the given path and method
    /// </summary>
    public bool Matches(string requestPath, string requestMethod)
    {
        if (!IsActive)
            return false;

        if (!string.Equals(Method, requestMethod?.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase))
            return false;

        return MatchesPath(requestPath);
    }

    /// <summary>
    /// Check if route path matches the request path (supports wildcards)
    /// </summary>
    public bool MatchesPath(string requestPath)
    {
        if (string.IsNullOrWhiteSpace(requestPath))
            return false;

        var normalizedRequestPath = NormalizePath(requestPath);
        
        // Exact match
        if (string.Equals(Path, normalizedRequestPath, StringComparison.OrdinalIgnoreCase))
            return true;

        // Wildcard matching
        if (Path.Contains("*"))
        {
            var pattern = Path.Replace("*", ".*");
            return System.Text.RegularExpressions.Regex.IsMatch(normalizedRequestPath, $"^{pattern}$", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        // Path parameter matching (e.g., /api/users/{id})
        if (Path.Contains("{") && Path.Contains("}"))
        {
            var pathSegments = Path.Split('/');
            var requestSegments = normalizedRequestPath.Split('/');

            if (pathSegments.Length != requestSegments.Length)
                return false;

            for (int i = 0; i < pathSegments.Length; i++)
            {
                var pathSegment = pathSegments[i];
                var requestSegment = requestSegments[i];

                // Skip parameter segments
                if (pathSegment.StartsWith("{") && pathSegment.EndsWith("}"))
                    continue;

                if (!string.Equals(pathSegment, requestSegment, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if user has required roles
    /// </summary>
    public bool HasRequiredRoles(List<string> userRoles)
    {
        if (!RequiresAuthentication || !AllowedRoles.Any())
            return true;

        if (userRoles == null || !userRoles.Any())
            return false;

        return AllowedRoles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Check if user has required scopes
    /// </summary>
    public bool HasRequiredScopes(List<string> userScopes)
    {
        if (!RequiresAuthentication || !AllowedScopes.Any())
            return true;

        if (userScopes == null || !userScopes.Any())
            return false;

        return AllowedScopes.Any(scope => userScopes.Contains(scope, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Check if IP address is allowed
    /// </summary>
    public bool IsIpAddressAllowed(string ipAddress)
    {
        if (!AllowedIpAddresses.Any())
            return true;

        if (string.IsNullOrWhiteSpace(ipAddress))
            return false;

        return AllowedIpAddresses.Contains(ipAddress) || 
               AllowedIpAddresses.Any(allowed => IsIpInRange(ipAddress, allowed));
    }

    /// <summary>
    /// Check if origin is allowed for CORS
    /// </summary>
    public bool IsOriginAllowed(string origin)
    {
        if (!AllowedOrigins.Any())
            return true;

        if (string.IsNullOrWhiteSpace(origin))
            return false;

        return AllowedOrigins.Contains("*") || 
               AllowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Get the full upstream URL for the request
    /// </summary>
    public string GetUpstreamUrl(string requestPath = null)
    {
        var baseUrl = UpstreamUrl;
        var path = string.IsNullOrWhiteSpace(requestPath) ? UpstreamPath : requestPath;
        
        return $"{baseUrl}{path}";
    }

    /// <summary>
    /// Get cache key for the request
    /// </summary>
    public string GetCacheKey(string requestPath, Dictionary<string, string> queryParams = null)
    {
        if (!CacheEnabled)
            return null;

        var keyParts = new List<string> { Method, requestPath ?? Path };

        if (CacheKeys.Any() && queryParams != null)
        {
            foreach (var key in CacheKeys)
            {
                if (queryParams.ContainsKey(key))
                {
                    keyParts.Add($"{key}={queryParams[key]}");
                }
            }
        }

        return string.Join("|", keyParts);
    }

    /// <summary>
    /// Get route summary for logging
    /// </summary>
    public string GetSummary()
    {
        return $"Route[{Method} {Path}] -> {UpstreamUrl}{UpstreamPath} " +
               $"Auth:{RequiresAuthentication} RateLimit:{RateLimitEnabled}({RateLimit}/{RateLimitWindow.TotalMinutes}min) " +
               $"Cache:{CacheEnabled}({CacheDuration.TotalMinutes}min) Active:{IsActive}";
    }

    /// <summary>
    /// Normalize path by ensuring it starts with / and removing trailing /
    /// </summary>
    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "/";

        path = path.Trim();
        
        if (!path.StartsWith("/"))
            path = "/" + path;

        if (path.Length > 1 && path.EndsWith("/"))
            path = path.TrimEnd('/');

        return path;
    }

    /// <summary>
    /// Check if IP is in range (simplified CIDR check)
    /// </summary>
    private static bool IsIpInRange(string ipAddress, string allowedRange)
    {
        // Simplified implementation - in production, use proper CIDR matching
        if (allowedRange.Contains("/"))
        {
            // CIDR notation - would need proper implementation
            return false;
        }

        return string.Equals(ipAddress, allowedRange, StringComparison.OrdinalIgnoreCase);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Path;
        yield return Method;
        yield return UpstreamUrl;
        yield return UpstreamPath;
        yield return RequiresAuthentication;
        yield return RateLimitEnabled;
        yield return RateLimit;
        yield return CacheEnabled;
        yield return IsActive;
        yield return Priority;
    }

    public override string ToString()
    {
        return GetSummary();
    }
}