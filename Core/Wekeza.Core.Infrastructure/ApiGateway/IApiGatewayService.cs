using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Wekeza.Core.Infrastructure.ApiGateway;

/// <summary>
/// API Gateway service interface
/// Provides enterprise-grade API management, routing, and security
/// </summary>
public interface IApiGatewayService
{
    // Request Routing
    Task<ApiResponse> RouteRequestAsync(ApiRequest request);
    Task<List<ApiRoute>> GetRoutesAsync();
    Task<ApiRoute?> GetRouteAsync(string routeId);
    Task<ApiRoute> CreateRouteAsync(CreateRouteRequest request);
    Task<ApiRoute> UpdateRouteAsync(string routeId, UpdateRouteRequest request);
    Task DeleteRouteAsync(string routeId);

    // Load Balancing
    Task<string> SelectUpstreamServerAsync(string routeId, LoadBalancingStrategy strategy = LoadBalancingStrategy.RoundRobin);
    Task<List<UpstreamServer>> GetUpstreamServersAsync(string routeId);
    Task AddUpstreamServerAsync(string routeId, UpstreamServer server);
    Task RemoveUpstreamServerAsync(string routeId, string serverId);
    Task UpdateServerHealthAsync(string serverId, bool isHealthy);

    // Rate Limiting
    Task<RateLimitResult> CheckRateLimitAsync(string clientId, string routeId);
    Task<RateLimitConfiguration> GetRateLimitConfigAsync(string routeId);
    Task UpdateRateLimitConfigAsync(string routeId, RateLimitConfiguration config);
    Task<Dictionary<string, RateLimitStatus>> GetRateLimitStatusAsync(string clientId);

    // Authentication & Authorization
    Task<AuthenticationResult> AuthenticateRequestAsync(ApiRequest request);
    Task<AuthorizationResult> AuthorizeRequestAsync(ApiRequest request, string userId);
    Task<ApiKey> CreateApiKeyAsync(CreateApiKeyRequest request);
    Task<ApiKey?> GetApiKeyAsync(string keyId);
    Task RevokeApiKeyAsync(string keyId);
    Task<List<ApiKey>> GetApiKeysAsync(string? clientId = null);

    // Request/Response Transformation
    Task<ApiRequest> TransformRequestAsync(ApiRequest request, string routeId);
    Task<ApiResponse> TransformResponseAsync(ApiResponse response, string routeId);
    Task<List<TransformationRule>> GetTransformationRulesAsync(string routeId);
    Task AddTransformationRuleAsync(string routeId, TransformationRule rule);

    // Circuit Breaker
    Task<CircuitBreakerState> GetCircuitBreakerStateAsync(string routeId);
    Task UpdateCircuitBreakerConfigAsync(string routeId, CircuitBreakerConfig config);
    Task<bool> IsCircuitBreakerOpenAsync(string routeId);
    Task RecordRequestResultAsync(string routeId, bool success, TimeSpan responseTime);

    // Caching
    Task<ApiResponse?> GetCachedResponseAsync(string cacheKey);
    Task SetCachedResponseAsync(string cacheKey, ApiResponse response, TimeSpan expiration);
    Task InvalidateCacheAsync(string pattern);
    Task<CacheStatistics> GetCacheStatisticsAsync();

    // Analytics & Monitoring
    Task<ApiAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, string? routeId = null);
    Task<List<ApiMetric>> GetMetricsAsync(string metricName, DateTime startDate, DateTime endDate);
    Task RecordApiCallAsync(ApiCallRecord record);
    Task<List<ApiCallRecord>> GetApiCallHistoryAsync(string? routeId = null, int pageSize = 100, int pageNumber = 1);

    // Health Checks
    Task<HealthCheckResult> PerformHealthCheckAsync(string routeId);
    Task<Dictionary<string, HealthCheckResult>> PerformAllHealthChecksAsync();
    Task<List<HealthCheckResult>> GetHealthCheckHistoryAsync(string routeId);

    // Configuration Management
    Task<ApiGatewayConfiguration> GetConfigurationAsync();
    Task UpdateConfigurationAsync(ApiGatewayConfiguration config);
    Task ReloadConfigurationAsync();
    Task<bool> ValidateConfigurationAsync(ApiGatewayConfiguration config);

    // Security
    Task<SecurityScanResult> PerformSecurityScanAsync(ApiRequest request);
    Task<List<SecurityThreat>> GetSecurityThreatsAsync(DateTime? since = null);
    Task BlockClientAsync(string clientId, TimeSpan duration, string reason);
    Task UnblockClientAsync(string clientId);
    Task<List<BlockedClient>> GetBlockedClientsAsync();
}

/// <summary>
/// API request model
/// </summary>
public class ApiRequest
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// API response model
/// </summary>
public class ApiResponse
{
    public string RequestId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public TimeSpan ProcessingTime { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool FromCache { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// API route configuration
/// </summary>
public class ApiRoute
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public List<string> Methods { get; set; } = new();
    public List<UpstreamServer> UpstreamServers { get; set; } = new();
    public LoadBalancingStrategy LoadBalancingStrategy { get; set; }
    public RateLimitConfiguration RateLimit { get; set; } = new();
    public AuthenticationConfig Authentication { get; set; } = new();
    public CircuitBreakerConfig CircuitBreaker { get; set; } = new();
    public CacheConfig Cache { get; set; } = new();
    public List<TransformationRule> Transformations { get; set; } = new();
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Upstream server configuration
/// </summary>
public class UpstreamServer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Scheme { get; set; } = "https";
    public int Weight { get; set; } = 1;
    public bool IsHealthy { get; set; } = true;
    public DateTime LastHealthCheck { get; set; }
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);
    public string HealthCheckPath { get; set; } = "/health";
    public Dictionary<string, string> Headers { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Rate limit configuration
/// </summary>
public class RateLimitConfiguration
{
    public bool IsEnabled { get; set; }
    public int RequestsPerMinute { get; set; } = 100;
    public int RequestsPerHour { get; set; } = 1000;
    public int RequestsPerDay { get; set; } = 10000;
    public RateLimitStrategy Strategy { get; set; } = RateLimitStrategy.FixedWindow;
    public List<string> ExemptClients { get; set; } = new();
    public Dictionary<string, int> ClientSpecificLimits { get; set; } = new();
    public string ErrorMessage { get; set; } = "Rate limit exceeded";
    public Dictionary<string, object> CustomRules { get; set; } = new();
}

/// <summary>
/// Rate limit result
/// </summary>
public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public int RemainingRequests { get; set; }
    public DateTime ResetTime { get; set; }
    public string LimitType { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalInfo { get; set; } = new();
}

/// <summary>
/// Authentication configuration
/// </summary>
public class AuthenticationConfig
{
    public bool IsRequired { get; set; }
    public List<AuthenticationType> SupportedTypes { get; set; } = new();
    public JwtConfig? JwtConfig { get; set; }
    public ApiKeyConfig? ApiKeyConfig { get; set; }
    public OAuthConfig? OAuthConfig { get; set; }
    public List<string> RequiredScopes { get; set; } = new();
    public List<string> RequiredRoles { get; set; } = new();
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// Circuit breaker configuration
/// </summary>
public class CircuitBreakerConfig
{
    public bool IsEnabled { get; set; }
    public int FailureThreshold { get; set; } = 5;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan RecoveryTimeout { get; set; } = TimeSpan.FromMinutes(1);
    public double SuccessThreshold { get; set; } = 0.5;
    public int MinimumThroughput { get; set; } = 10;
    public string FallbackResponse { get; set; } = string.Empty;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// Cache configuration
/// </summary>
public class CacheConfig
{
    public bool IsEnabled { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(5);
    public List<string> CacheableStatusCodes { get; set; } = new() { "200" };
    public List<string> CacheKeys { get; set; } = new();
    public bool VaryByUser { get; set; }
    public bool VaryByHeaders { get; set; }
    public List<string> VaryByHeaderNames { get; set; } = new();
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// Transformation rule
/// </summary>
public class TransformationRule
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public TransformationType Type { get; set; }
    public TransformationTarget Target { get; set; }
    public string Pattern { get; set; } = string.Empty;
    public string Replacement { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool IsEnabled { get; set; } = true;
    public int Order { get; set; }
}

/// <summary>
/// API analytics
/// </summary>
public class ApiAnalytics
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long TotalRequests { get; set; }
    public long SuccessfulRequests { get; set; }
    public long FailedRequests { get; set; }
    public double SuccessRate { get; set; }
    public double AverageResponseTime { get; set; }
    public Dictionary<string, long> RequestsByRoute { get; set; } = new();
    public Dictionary<string, long> RequestsByStatusCode { get; set; } = new();
    public Dictionary<string, long> RequestsByClient { get; set; } = new();
    public List<ApiTrend> Trends { get; set; } = new();
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}

/// <summary>
/// API call record
/// </summary>
public class ApiCallRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RequestId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public long RequestSize { get; set; }
    public long ResponseSize { get; set; }
    public DateTime Timestamp { get; set; }
    public bool FromCache { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Enumerations
/// </summary>
public enum LoadBalancingStrategy
{
    RoundRobin,
    Random,
    WeightedRoundRobin,
    LeastConnections,
    IpHash
}

public enum RateLimitStrategy
{
    FixedWindow,
    SlidingWindow,
    TokenBucket,
    LeakyBucket
}

public enum AuthenticationType
{
    None,
    ApiKey,
    JWT,
    OAuth,
    Basic,
    Custom
}

public enum TransformationType
{
    RequestHeader,
    ResponseHeader,
    RequestBody,
    ResponseBody,
    QueryParameter,
    PathParameter
}

public enum TransformationTarget
{
    Request,
    Response
}

public enum CircuitBreakerState
{
    Closed,
    Open,
    HalfOpen
}

/// <summary>
/// Supporting classes
/// </summary>
public class JwtConfig
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);
}

public class ApiKeyConfig
{
    public string HeaderName { get; set; } = "X-API-Key";
    public string QueryParameterName { get; set; } = "api_key";
    public bool AllowInHeader { get; set; } = true;
    public bool AllowInQuery { get; set; } = false;
}

public class OAuthConfig
{
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = new();
}

public class ApiKey
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public List<string> AllowedRoutes { get; set; } = new();
    public RateLimitConfiguration RateLimit { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ApiTrend
{
    public DateTime Timestamp { get; set; }
    public long RequestCount { get; set; }
    public double AverageResponseTime { get; set; }
    public double ErrorRate { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class CreateRouteRequest
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public List<string> Methods { get; set; } = new();
    public List<UpstreamServer> UpstreamServers { get; set; } = new();
    public LoadBalancingStrategy LoadBalancingStrategy { get; set; }
    public RateLimitConfiguration? RateLimit { get; set; }
    public AuthenticationConfig? Authentication { get; set; }
    public CircuitBreakerConfig? CircuitBreaker { get; set; }
    public CacheConfig? Cache { get; set; }
}

public class UpdateRouteRequest : CreateRouteRequest
{
    public bool? IsEnabled { get; set; }
}

public class CreateApiKeyRequest
{
    public string Name { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public List<string> AllowedRoutes { get; set; } = new();
    public RateLimitConfiguration? RateLimit { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class AuthenticationResult
{
    public bool IsAuthenticated { get; set; }
    public string? UserId { get; set; }
    public string? ClientId { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Scopes { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Claims { get; set; } = new();
}

public class AuthorizationResult
{
    public bool IsAuthorized { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> MissingPermissions { get; set; } = new();
    public Dictionary<string, object> Context { get; set; } = new();
}

public class RateLimitStatus
{
    public string RouteId { get; set; } = string.Empty;
    public int RemainingRequests { get; set; }
    public DateTime ResetTime { get; set; }
    public bool IsBlocked { get; set; }
}

public class SecurityScanResult
{
    public bool IsThreat { get; set; }
    public List<SecurityThreat> Threats { get; set; } = new();
    public string RiskLevel { get; set; } = "Low";
    public string Action { get; set; } = "Allow";
    public Dictionary<string, object> Details { get; set; } = new();
}

public class SecurityThreat
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
}

public class BlockedClient
{
    public string ClientId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public DateTime? UnblockedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}

public class ApiGatewayConfiguration
{
    public string Version { get; set; } = "1.0";
    public Dictionary<string, object> GlobalSettings { get; set; } = new();
    public List<ApiRoute> Routes { get; set; } = new();
    public RateLimitConfiguration GlobalRateLimit { get; set; } = new();
    public AuthenticationConfig GlobalAuthentication { get; set; } = new();
    public Dictionary<string, object> Plugins { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class ApiMetric
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Tags { get; set; } = new();
}