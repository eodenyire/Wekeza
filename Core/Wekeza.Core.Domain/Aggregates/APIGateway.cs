using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// API Gateway Aggregate - Manages API routing, load balancing, and security
/// Provides centralized API management with rate limiting and monitoring
/// Industry Standard: Kong, Ocelot, and enterprise API gateway patterns
/// </summary>
public class APIGateway : AggregateRoot
{
    // Core Properties
    public string GatewayCode { get; private set; }
    public string GatewayName { get; private set; }
    public string Description { get; private set; }
    public string BaseUrl { get; private set; }
    public GatewayStatus Status { get; private set; }
    public string Version { get; private set; }
    
    // Route Management
    public List<APIRoute> Routes { get; private set; }
    public Dictionary<string, RateLimitConfig> RateLimits { get; private set; }
    public Dictionary<string, AuthenticationConfig> AuthConfigs { get; private set; }
    public Dictionary<string, CacheConfig> CacheConfigs { get; private set; }
    
    // Load Balancing
    public LoadBalancingStrategy LoadBalancing { get; private set; }
    public List<UpstreamServer> UpstreamServers { get; private set; }
    public HealthCheckConfig HealthCheck { get; private set; }
    public int MaxConnections { get; private set; }
    public TimeSpan ConnectionTimeout { get; private set; }
    
    // Security
    public bool RequireAuthentication { get; private set; }
    public List<string> AllowedOrigins { get; private set; }
    public Dictionary<string, string> SecurityHeaders { get; private set; }
    public bool EnableCORS { get; private set; }
    public bool EnableHTTPS { get; private set; }
    public string SSLCertificate { get; private set; }
    
    // Monitoring & Analytics
    public APIMetrics Metrics { get; private set; }
    public List<APILog> RecentLogs { get; private set; }
    public bool EnableLogging { get; private set; }
    public bool EnableMetrics { get; private set; }
    public LogLevel LogLevel { get; private set; }
    
    // Circuit Breaker
    public bool EnableCircuitBreaker { get; private set; }
    public int FailureThreshold { get; private set; }
    public TimeSpan CircuitBreakerTimeout { get; private set; }
    public Dictionary<string, ValueObjects.CircuitBreakerState> CircuitBreakerStates { get; private set; }
    
    // Maintenance
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string LastModifiedBy { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    // Private constructor for EF Core
    private APIGateway() : base(Guid.NewGuid()) {
        Routes = new List<APIRoute>();
        RateLimits = new Dictionary<string, RateLimitConfig>();
        AuthConfigs = new Dictionary<string, AuthenticationConfig>();
        CacheConfigs = new Dictionary<string, CacheConfig>();
        UpstreamServers = new List<UpstreamServer>();
        AllowedOrigins = new List<string>();
        SecurityHeaders = new Dictionary<string, string>();
        RecentLogs = new List<APILog>();
        CircuitBreakerStates = new Dictionary<string, ValueObjects.CircuitBreakerState>();
        Metadata = new Dictionary<string, object>();
    }

    // Factory method for creating new API gateway
    public static APIGateway Create(
        string gatewayCode,
        string gatewayName,
        string baseUrl,
        string createdBy,
        string description = null,
        string version = "1.0")
    {
        // Validation
        if (string.IsNullOrWhiteSpace(gatewayCode))
            throw new ArgumentException("Gateway code cannot be empty", nameof(gatewayCode));
        
        if (string.IsNullOrWhiteSpace(gatewayName))
            throw new ArgumentException("Gateway name cannot be empty", nameof(gatewayName));
        
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by cannot be empty", nameof(createdBy));

        // Validate URL format
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid base URL format", nameof(baseUrl));

        var gateway = new APIGateway
        {
            Id = Guid.NewGuid(),
            GatewayCode = gatewayCode,
            GatewayName = gatewayName,
            Description = description,
            BaseUrl = baseUrl,
            Status = GatewayStatus.Inactive,
            Version = version,
            Routes = new List<APIRoute>(),
            RateLimits = new Dictionary<string, RateLimitConfig>(),
            AuthConfigs = new Dictionary<string, AuthenticationConfig>(),
            CacheConfigs = new Dictionary<string, CacheConfig>(),
            LoadBalancing = LoadBalancingStrategy.RoundRobin,
            UpstreamServers = new List<UpstreamServer>(),
            HealthCheck = HealthCheckConfig.Default(),
            MaxConnections = 1000,
            ConnectionTimeout = TimeSpan.FromSeconds(30),
            RequireAuthentication = true,
            AllowedOrigins = new List<string>(),
            SecurityHeaders = new Dictionary<string, string>(),
            EnableCORS = true,
            EnableHTTPS = true,
            Metrics = new APIMetrics(),
            RecentLogs = new List<APILog>(),
            EnableLogging = true,
            EnableMetrics = true,
            LogLevel = LogLevel.Information,
            EnableCircuitBreaker = true,
            FailureThreshold = 5,
            CircuitBreakerTimeout = TimeSpan.FromMinutes(1),
            CircuitBreakerStates = new Dictionary<string, ValueObjects.CircuitBreakerState>(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Metadata = new Dictionary<string, object>()
        };

        // Add default security headers
        gateway.SecurityHeaders["X-Content-Type-Options"] = "nosniff";
        gateway.SecurityHeaders["X-Frame-Options"] = "DENY";
        gateway.SecurityHeaders["X-XSS-Protection"] = "1; mode=block";
        gateway.SecurityHeaders["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

        // Add creation event
        gateway.AddDomainEvent(new APIGatewayCreatedDomainEvent(
            gateway.Id,
            gateway.GatewayCode,
            gateway.GatewayName,
            gateway.BaseUrl,
            gateway.CreatedBy));

        return gateway;
    }

    // Add API route
    public void AddRoute(APIRoute route, string modifiedBy)
    {
        if (route == null)
            throw new ArgumentNullException(nameof(route));

        // Check for duplicate routes
        if (Routes.Any(r => r.Path == route.Path && r.Method == route.Method))
            throw new InvalidOperationException($"Route {route.Method} {route.Path} already exists");

        Routes.Add(route);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["RouteCount"] = Routes.Count;
        Metadata["LastRouteAddedAt"] = DateTime.UtcNow;

        AddDomainEvent(new APIRouteAddedDomainEvent(
            Id,
            GatewayCode,
            route.Path,
            route.Method,
            route.UpstreamUrl,
            modifiedBy));
    }

    // Remove API route
    public void RemoveRoute(string path, string method, string modifiedBy)
    {
        var route = Routes.FirstOrDefault(r => r.Path == path && r.Method == method);
        if (route == null)
            throw new InvalidOperationException($"Route {method} {path} not found");

        Routes.Remove(route);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Remove associated configurations
        var routeKey = $"{method}:{path}";
        RateLimits.Remove(routeKey);
        AuthConfigs.Remove(routeKey);
        CacheConfigs.Remove(routeKey);

        // Update metadata
        Metadata["RouteCount"] = Routes.Count;
        Metadata["LastRouteRemovedAt"] = DateTime.UtcNow;

        AddDomainEvent(new APIRouteRemovedDomainEvent(
            Id,
            GatewayCode,
            path,
            method,
            modifiedBy));
    }

    // Update rate limit configuration
    public void UpdateRateLimit(string endpoint, RateLimitConfig config, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new ArgumentException("Endpoint cannot be empty", nameof(endpoint));
        
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        RateLimits[endpoint] = config;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["RateLimitConfigCount"] = RateLimits.Count;
        Metadata["LastRateLimitUpdatedAt"] = DateTime.UtcNow;

        AddDomainEvent(new APIRateLimitUpdatedDomainEvent(
            Id,
            GatewayCode,
            endpoint,
            config.RequestsPerMinute,
            modifiedBy));
    }

    // Add upstream server
    public void AddUpstreamServer(UpstreamServer server, string modifiedBy)
    {
        if (server == null)
            throw new ArgumentNullException(nameof(server));

        // Check for duplicate servers
        if (UpstreamServers.Any(s => s.Url == server.Url))
            throw new InvalidOperationException($"Upstream server {server.Url} already exists");

        UpstreamServers.Add(server);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["UpstreamServerCount"] = UpstreamServers.Count;
        Metadata["LastUpstreamServerAddedAt"] = DateTime.UtcNow;

        AddDomainEvent(new APIUpstreamServerAddedDomainEvent(
            Id,
            GatewayCode,
            server.Url,
            server.Weight,
            modifiedBy));
    }

    // Update server health status
    public void UpdateHealthStatus(string serverUrl, bool isHealthy, string healthCheckResponse = null)
    {
        var server = UpstreamServers.FirstOrDefault(s => s.Url == serverUrl);
        if (server == null)
            return;

        var wasHealthy = server.IsHealthy;
        server.UpdateHealth(isHealthy, healthCheckResponse);

        // Update circuit breaker state if needed
        if (EnableCircuitBreaker)
        {
            UpdateCircuitBreakerState(serverUrl, isHealthy);
        }

        // Emit event if health status changed
        if (wasHealthy != isHealthy)
        {
            AddDomainEvent(new APIServerHealthChangedDomainEvent(
                Id,
                GatewayCode,
                serverUrl,
                isHealthy,
                healthCheckResponse));
        }
    }

    // Record API request
    public void RecordRequest(string endpoint, string method, TimeSpan responseTime, int statusCode, string clientIP = null)
    {
        // Update metrics
        Metrics.RecordRequest(endpoint, method, responseTime, statusCode);

        // Add to recent logs if logging is enabled
        if (EnableLogging && RecentLogs.Count < 1000) // Keep last 1000 logs
        {
            var log = new APILog(method, endpoint, statusCode, responseTime.TotalMilliseconds, DateTime.UtcNow);
            
            RecentLogs.Add(log);
            
            // Remove old logs if we exceed the limit
            if (RecentLogs.Count > 1000)
            {
                RecentLogs.RemoveAt(0);
            }
        }

        // Update circuit breaker for failed requests
        if (EnableCircuitBreaker && statusCode >= 500)
        {
            UpdateCircuitBreakerState(endpoint, false);
        }

        // Emit high-level metrics event periodically
        if (Metrics.TotalRequests % 1000 == 0)
        {
            AddDomainEvent(new APIMetricsUpdatedDomainEvent(
                Id,
                GatewayCode,
                Metrics.TotalRequests,
                Metrics.AverageResponseTime,
                Metrics.ErrorRate));
        }
    }

    // Activate gateway
    public void Activate(string activatedBy)
    {
        if (Status == GatewayStatus.Active)
            return;

        // Validate that we have at least one route and upstream server
        if (!Routes.Any())
            throw new InvalidOperationException("Cannot activate gateway without routes");

        if (!UpstreamServers.Any())
            throw new InvalidOperationException("Cannot activate gateway without upstream servers");

        Status = GatewayStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = activatedBy;

        // Update metadata
        Metadata["ActivatedAt"] = DateTime.UtcNow;
        Metadata["ActivatedBy"] = activatedBy;

        AddDomainEvent(new APIGatewayActivatedDomainEvent(
            Id,
            GatewayCode,
            activatedBy));
    }

    // Deactivate gateway
    public void Deactivate(string deactivatedBy, string reason = null)
    {
        if (Status == GatewayStatus.Inactive)
            return;

        Status = GatewayStatus.Inactive;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = deactivatedBy;

        // Update metadata
        Metadata["DeactivatedAt"] = DateTime.UtcNow;
        Metadata["DeactivatedBy"] = deactivatedBy;
        if (!string.IsNullOrWhiteSpace(reason))
            Metadata["DeactivationReason"] = reason;

        AddDomainEvent(new APIGatewayDeactivatedDomainEvent(
            Id,
            GatewayCode,
            deactivatedBy,
            reason));
    }

    // Update security configuration
    public void UpdateSecurityConfig(
        bool requireAuth,
        List<string> allowedOrigins,
        Dictionary<string, string> securityHeaders,
        string modifiedBy)
    {
        RequireAuthentication = requireAuth;
        AllowedOrigins = allowedOrigins ?? new List<string>();
        SecurityHeaders = securityHeaders ?? new Dictionary<string, string>();
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["SecurityConfigUpdatedAt"] = DateTime.UtcNow;
        Metadata["SecurityConfigUpdatedBy"] = modifiedBy;

        AddDomainEvent(new APISecurityConfigUpdatedDomainEvent(
            Id,
            GatewayCode,
            requireAuth,
            modifiedBy));
    }

    // Get route by path and method
    public APIRoute GetRoute(string path, string method)
    {
        return Routes.FirstOrDefault(r => r.Path == path && r.Method == method);
    }

    // Get healthy upstream servers
    public List<UpstreamServer> GetHealthyServers()
    {
        return UpstreamServers.Where(s => s.IsHealthy).ToList();
    }

    // Get next server using load balancing strategy
    public UpstreamServer GetNextServer()
    {
        var healthyServers = GetHealthyServers();
        if (!healthyServers.Any())
            return null;

        return LoadBalancing switch
        {
            LoadBalancingStrategy.RoundRobin => GetRoundRobinServer(healthyServers),
            LoadBalancingStrategy.WeightedRoundRobin => GetWeightedRoundRobinServer(healthyServers),
            LoadBalancingStrategy.Random => GetRandomServer(healthyServers),
            LoadBalancingStrategy.LeastConnections => GetLeastConnectionsServer(healthyServers),
            _ => healthyServers.First()
        };
    }

    // Check if endpoint is rate limited
    public bool IsRateLimited(string endpoint, string clientId)
    {
        if (!RateLimits.ContainsKey(endpoint))
            return false;

        var config = RateLimits[endpoint];
        return config.IsRateLimited(clientId);
    }

    // Get gateway health status
    public GatewayHealthStatus GetHealthStatus()
    {
        if (Status != GatewayStatus.Active)
            return GatewayHealthStatus.Inactive;

        var healthyServers = GetHealthyServers();
        var totalServers = UpstreamServers.Count;

        if (totalServers == 0)
            return GatewayHealthStatus.NoUpstreams;

        var healthyPercentage = (decimal)healthyServers.Count / totalServers * 100;

        if (healthyPercentage == 100)
            return GatewayHealthStatus.Healthy;

        if (healthyPercentage >= 50)
            return GatewayHealthStatus.Degraded;

        return GatewayHealthStatus.Unhealthy;
    }

    // Private methods
    private void UpdateCircuitBreakerState(string endpoint, bool success)
    {
        if (!CircuitBreakerStates.ContainsKey(endpoint))
        {
            CircuitBreakerStates[endpoint] = new ValueObjects.CircuitBreakerState();
        }

        var state = CircuitBreakerStates[endpoint];
        
        if (success)
        {
            state.RecordSuccess();
        }
        else
        {
            state.RecordFailure();
            
            if (state.ShouldOpenCircuit(FailureThreshold))
            {
                state.OpenCircuit(CircuitBreakerTimeout);
                
                AddDomainEvent(new APICircuitBreakerOpenedDomainEvent(
                    Id,
                    GatewayCode,
                    endpoint,
                    state.FailureCount));
            }
        }
    }

    private UpstreamServer GetRoundRobinServer(List<UpstreamServer> servers)
    {
        // Simple round-robin implementation
        var index = (int)(Metrics.TotalRequests % servers.Count);
        return servers[index];
    }

    private UpstreamServer GetWeightedRoundRobinServer(List<UpstreamServer> servers)
    {
        var totalWeight = servers.Sum(s => s.Weight);
        var random = new Random().Next(1, totalWeight + 1);
        var currentWeight = 0;

        foreach (var server in servers)
        {
            currentWeight += server.Weight;
            if (random <= currentWeight)
                return server;
        }

        return servers.First();
    }

    private UpstreamServer GetRandomServer(List<UpstreamServer> servers)
    {
        var random = new Random();
        var index = random.Next(servers.Count);
        return servers[index];
    }

    private UpstreamServer GetLeastConnectionsServer(List<UpstreamServer> servers)
    {
        return servers.OrderBy(s => s.ActiveConnections).First();
    }
}

/// <summary>
/// Gateway Health Status enumeration
/// </summary>
public enum GatewayHealthStatus
{
    Healthy,
    Degraded,
    Unhealthy,
    NoUpstreams,
    Inactive
}

