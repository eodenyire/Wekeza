using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Integration Endpoint Aggregate - Manages external system integration endpoints
/// Supports REST, SOAP, GraphQL, gRPC, WebSocket, FTP, and database connections
/// Industry Standard: Enterprise Service Bus (ESB) and API Gateway patterns
/// </summary>
public class IntegrationEndpoint : AggregateRoot
{
    // Core Properties
    public string EndpointCode { get; private set; }
    public string EndpointName { get; private set; }
    public string Description { get; private set; }
    public EndpointType Type { get; private set; }
    public EndpointProtocol Protocol { get; private set; }
    public EndpointStatus Status { get; private set; }
    
    // Connection Details
    public string BaseUrl { get; private set; }
    public int Port { get; private set; }
    public string Path { get; private set; }
    public Dictionary<string, string> Headers { get; private set; }
    public Dictionary<string, object> Configuration { get; private set; }
    public TimeSpan Timeout { get; private set; }
    
    // Authentication
    public AuthenticationMethod AuthMethod { get; private set; }
    public Dictionary<string, string> Credentials { get; private set; }
    public DateTime? CredentialsExpiryDate { get; private set; }
    public bool RequiresCertificate { get; private set; }
    public string CertificatePath { get; private set; }
    
    // Health & Monitoring
    public DateTime LastHealthCheck { get; private set; }
    public HealthStatus HealthStatus { get; private set; }
    public string LastHealthCheckResult { get; private set; }
    public int SuccessfulCalls { get; private set; }
    public int FailedCalls { get; private set; }
    public TimeSpan AverageResponseTime { get; private set; }
    public DateTime? LastSuccessfulCall { get; private set; }
    public DateTime? LastFailedCall { get; private set; }
    public string LastErrorMessage { get; private set; }
    
    // Rate Limiting
    public int RateLimitPerMinute { get; private set; }
    public int RateLimitPerHour { get; private set; }
    public int RateLimitPerDay { get; private set; }
    public int CurrentMinuteCallCount { get; private set; }
    public int CurrentHourCallCount { get; private set; }
    public int CurrentDayCallCount { get; private set; }
    public DateTime LastRateLimitReset { get; private set; }
    
    // Retry & Circuit Breaker
    public int MaxRetryAttempts { get; private set; }
    public TimeSpan RetryDelay { get; private set; }
    public bool CircuitBreakerEnabled { get; private set; }
    public int CircuitBreakerThreshold { get; private set; }
    public CircuitBreakerState CircuitBreakerState { get; private set; }
    public DateTime? CircuitBreakerOpenedAt { get; private set; }
    public TimeSpan CircuitBreakerTimeout { get; private set; }
    
    // Load Balancing
    public LoadBalancingStrategy LoadBalancingStrategy { get; private set; }
    public List<string> UpstreamServers { get; private set; }
    public int CurrentServerIndex { get; private set; }
    
    // Caching
    public bool CacheEnabled { get; private set; }
    public TimeSpan CacheDuration { get; private set; }
    public List<string> CacheableOperations { get; private set; }
    
    // Metadata
    public Dictionary<string, object> Metadata { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string LastModifiedBy { get; private set; }

    // Private constructor for EF Core
    private IntegrationEndpoint() 
    {
        Headers = new Dictionary<string, string>();
        Configuration = new Dictionary<string, object>();
        Credentials = new Dictionary<string, string>();
        UpstreamServers = new List<string>();
        CacheableOperations = new List<string>();
        Metadata = new Dictionary<string, object>();
    }

    // Factory method for creating new integration endpoint
    public static IntegrationEndpoint Create(
        string endpointCode,
        string endpointName,
        EndpointType type,
        EndpointProtocol protocol,
        string baseUrl,
        string createdBy,
        string description = null,
        AuthenticationMethod authMethod = AuthenticationMethod.None,
        Dictionary<string, string> credentials = null,
        Dictionary<string, object> configuration = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(endpointCode))
            throw new ArgumentException("Endpoint code cannot be empty", nameof(endpointCode));
        
        if (string.IsNullOrWhiteSpace(endpointName))
            throw new ArgumentException("Endpoint name cannot be empty", nameof(endpointName));
        
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by cannot be empty", nameof(createdBy));

        // Validate URL format
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid base URL format", nameof(baseUrl));

        var endpoint = new IntegrationEndpoint
        {
            Id = Guid.NewGuid(),
            EndpointCode = endpointCode,
            EndpointName = endpointName,
            Description = description,
            Type = type,
            Protocol = protocol,
            Status = EndpointStatus.Inactive,
            BaseUrl = baseUrl,
            Port = uri.Port,
            Path = uri.AbsolutePath,
            Headers = new Dictionary<string, string>(),
            Configuration = configuration ?? new Dictionary<string, object>(),
            Timeout = TimeSpan.FromSeconds(30),
            AuthMethod = authMethod,
            Credentials = credentials ?? new Dictionary<string, string>(),
            HealthStatus = HealthStatus.Unknown,
            SuccessfulCalls = 0,
            FailedCalls = 0,
            AverageResponseTime = TimeSpan.Zero,
            RateLimitPerMinute = 60,
            RateLimitPerHour = 3600,
            RateLimitPerDay = 86400,
            CurrentMinuteCallCount = 0,
            CurrentHourCallCount = 0,
            CurrentDayCallCount = 0,
            LastRateLimitReset = DateTime.UtcNow,
            MaxRetryAttempts = 3,
            RetryDelay = TimeSpan.FromSeconds(1),
            CircuitBreakerEnabled = true,
            CircuitBreakerThreshold = 5,
            CircuitBreakerState = CircuitBreakerState.Closed,
            CircuitBreakerTimeout = TimeSpan.FromMinutes(5),
            LoadBalancingStrategy = LoadBalancingStrategy.RoundRobin,
            UpstreamServers = new List<string> { baseUrl },
            CurrentServerIndex = 0,
            CacheEnabled = false,
            CacheDuration = TimeSpan.FromMinutes(5),
            CacheableOperations = new List<string>(),
            Metadata = new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        // Add creation event
        endpoint.AddDomainEvent(new IntegrationEndpointCreatedDomainEvent(
            endpoint.Id,
            endpoint.EndpointCode,
            endpoint.EndpointName,
            endpoint.Type,
            endpoint.Protocol,
            endpoint.CreatedBy));

        return endpoint;
    }

    // Update endpoint configuration
    public void UpdateConfiguration(Dictionary<string, object> configuration, string modifiedBy)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("Modified by cannot be empty", nameof(modifiedBy));

        Configuration = new Dictionary<string, object>(configuration);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["ConfigurationUpdatedAt"] = DateTime.UtcNow;
        Metadata["ConfigurationUpdatedBy"] = modifiedBy;

        AddDomainEvent(new IntegrationEndpointConfigurationUpdatedDomainEvent(
            Id,
            EndpointCode,
            modifiedBy));
    }

    // Update credentials
    public void UpdateCredentials(Dictionary<string, string> credentials, DateTime? expiryDate = null, string modifiedBy = null)
    {
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        Credentials = new Dictionary<string, string>(credentials);
        CredentialsExpiryDate = expiryDate;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["CredentialsUpdatedAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(modifiedBy))
            Metadata["CredentialsUpdatedBy"] = modifiedBy;

        AddDomainEvent(new IntegrationEndpointCredentialsUpdatedDomainEvent(
            Id,
            EndpointCode,
            expiryDate));
    }

    // Record successful API call
    public void RecordSuccessfulCall(TimeSpan responseTime)
    {
        SuccessfulCalls++;
        LastSuccessfulCall = DateTime.UtcNow;
        
        // Update average response time
        var totalCalls = SuccessfulCalls + FailedCalls;
        if (totalCalls > 1)
        {
            var totalTime = AverageResponseTime.TotalMilliseconds * (totalCalls - 1) + responseTime.TotalMilliseconds;
            AverageResponseTime = TimeSpan.FromMilliseconds(totalTime / totalCalls);
        }
        else
        {
            AverageResponseTime = responseTime;
        }

        // Update rate limit counters
        UpdateRateLimitCounters();
        
        // Reset circuit breaker if it was open
        if (CircuitBreakerState == CircuitBreakerState.Open)
        {
            ResetCircuitBreaker();
        }

        // Update health status to healthy if it wasn't
        if (HealthStatus != HealthStatus.Healthy)
        {
            UpdateHealthStatus(HealthStatus.Healthy, "Successful API call recorded");
        }

        AddDomainEvent(new IntegrationEndpointCallSucceededDomainEvent(
            Id,
            EndpointCode,
            responseTime,
            DateTime.UtcNow));
    }

    // Record failed API call
    public void RecordFailedCall(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));

        FailedCalls++;
        LastFailedCall = DateTime.UtcNow;
        LastErrorMessage = errorMessage;

        // Update rate limit counters
        UpdateRateLimitCounters();

        // Check circuit breaker threshold
        if (CircuitBreakerEnabled && ShouldTriggerCircuitBreaker())
        {
            TriggerCircuitBreaker();
        }

        // Update health status
        UpdateHealthStatus(HealthStatus.Unhealthy, errorMessage);

        AddDomainEvent(new IntegrationEndpointCallFailedDomainEvent(
            Id,
            EndpointCode,
            errorMessage,
            DateTime.UtcNow));
    }

    // Update health status
    public void UpdateHealthStatus(HealthStatus status, string result = null)
    {
        var previousStatus = HealthStatus;
        HealthStatus = status;
        LastHealthCheck = DateTime.UtcNow;
        LastHealthCheckResult = result;

        // Update metadata
        Metadata["HealthStatusUpdatedAt"] = DateTime.UtcNow;
        Metadata["PreviousHealthStatus"] = previousStatus.ToString();

        if (previousStatus != status)
        {
            AddDomainEvent(new IntegrationEndpointHealthStatusChangedDomainEvent(
                Id,
                EndpointCode,
                previousStatus,
                status,
                result));
        }
    }

    // Enable endpoint
    public void Enable(string enabledBy = null)
    {
        if (Status == EndpointStatus.Active)
            return;

        Status = EndpointStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = enabledBy;

        // Update metadata
        Metadata["EnabledAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(enabledBy))
            Metadata["EnabledBy"] = enabledBy;

        AddDomainEvent(new IntegrationEndpointEnabledDomainEvent(
            Id,
            EndpointCode,
            enabledBy));
    }

    // Disable endpoint
    public void Disable(string disabledBy = null, string reason = null)
    {
        if (Status == EndpointStatus.Inactive)
            return;

        Status = EndpointStatus.Inactive;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = disabledBy;

        // Update metadata
        Metadata["DisabledAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(disabledBy))
            Metadata["DisabledBy"] = disabledBy;
        if (!string.IsNullOrWhiteSpace(reason))
            Metadata["DisabledReason"] = reason;

        AddDomainEvent(new IntegrationEndpointDisabledDomainEvent(
            Id,
            EndpointCode,
            disabledBy,
            reason));
    }

    // Trigger circuit breaker
    public void TriggerCircuitBreaker()
    {
        if (!CircuitBreakerEnabled || CircuitBreakerState == CircuitBreakerState.Open)
            return;

        CircuitBreakerState = CircuitBreakerState.Open;
        CircuitBreakerOpenedAt = DateTime.UtcNow;

        // Update metadata
        Metadata["CircuitBreakerTriggeredAt"] = DateTime.UtcNow;
        Metadata["CircuitBreakerReason"] = $"Failed calls exceeded threshold: {CircuitBreakerThreshold}";

        AddDomainEvent(new IntegrationEndpointCircuitBreakerTriggeredDomainEvent(
            Id,
            EndpointCode,
            CircuitBreakerThreshold,
            DateTime.UtcNow));
    }

    // Reset circuit breaker
    public void ResetCircuitBreaker()
    {
        if (CircuitBreakerState == CircuitBreakerState.Closed)
            return;

        CircuitBreakerState = CircuitBreakerState.Closed;
        CircuitBreakerOpenedAt = null;

        // Update metadata
        Metadata["CircuitBreakerResetAt"] = DateTime.UtcNow;

        AddDomainEvent(new IntegrationEndpointCircuitBreakerResetDomainEvent(
            Id,
            EndpointCode,
            DateTime.UtcNow));
    }

    // Check if circuit breaker should be triggered
    private bool ShouldTriggerCircuitBreaker()
    {
        if (!CircuitBreakerEnabled || CircuitBreakerState == CircuitBreakerState.Open)
            return false;

        // Check recent failure rate
        var totalRecentCalls = Math.Max(1, SuccessfulCalls + FailedCalls);
        var recentFailureRate = (double)FailedCalls / totalRecentCalls;

        return FailedCalls >= CircuitBreakerThreshold && recentFailureRate > 0.5;
    }

    // Update rate limit counters
    private void UpdateRateLimitCounters()
    {
        var now = DateTime.UtcNow;
        
        // Reset counters if time windows have passed
        if (now.Subtract(LastRateLimitReset).TotalMinutes >= 1)
        {
            CurrentMinuteCallCount = 0;
        }
        
        if (now.Subtract(LastRateLimitReset).TotalHours >= 1)
        {
            CurrentHourCallCount = 0;
        }
        
        if (now.Subtract(LastRateLimitReset).TotalDays >= 1)
        {
            CurrentDayCallCount = 0;
            LastRateLimitReset = now;
        }

        // Increment counters
        CurrentMinuteCallCount++;
        CurrentHourCallCount++;
        CurrentDayCallCount++;
    }

    // Check if rate limit is exceeded
    public bool IsRateLimitExceeded()
    {
        UpdateRateLimitCounters();
        
        return CurrentMinuteCallCount > RateLimitPerMinute ||
               CurrentHourCallCount > RateLimitPerHour ||
               CurrentDayCallCount > RateLimitPerDay;
    }

    // Check if circuit breaker allows calls
    public bool IsCircuitBreakerOpen()
    {
        if (!CircuitBreakerEnabled || CircuitBreakerState == CircuitBreakerState.Closed)
            return false;

        if (CircuitBreakerState == CircuitBreakerState.Open && CircuitBreakerOpenedAt.HasValue)
        {
            // Check if timeout has passed to move to half-open state
            if (DateTime.UtcNow.Subtract(CircuitBreakerOpenedAt.Value) >= CircuitBreakerTimeout)
            {
                CircuitBreakerState = CircuitBreakerState.HalfOpen;
                return false; // Allow one test call
            }
            return true; // Still open
        }

        return false;
    }

    // Get next upstream server for load balancing
    public string GetNextUpstreamServer()
    {
        if (!UpstreamServers.Any())
            return BaseUrl;

        switch (LoadBalancingStrategy)
        {
            case LoadBalancingStrategy.RoundRobin:
                var server = UpstreamServers[CurrentServerIndex];
                CurrentServerIndex = (CurrentServerIndex + 1) % UpstreamServers.Count;
                return server;
            
            case LoadBalancingStrategy.Random:
                var random = new Random();
                return UpstreamServers[random.Next(UpstreamServers.Count)];
            
            default:
                return UpstreamServers.First();
        }
    }

    // Check if credentials are expired
    public bool AreCredentialsExpired()
    {
        return CredentialsExpiryDate.HasValue && DateTime.UtcNow > CredentialsExpiryDate.Value;
    }

    // Get success rate percentage
    public decimal GetSuccessRate()
    {
        var totalCalls = SuccessfulCalls + FailedCalls;
        if (totalCalls == 0)
            return 0;

        return Math.Round((decimal)SuccessfulCalls / totalCalls * 100, 2);
    }

    // Check if endpoint is healthy
    public bool IsHealthy()
    {
        return Status == EndpointStatus.Active &&
               HealthStatus == HealthStatus.Healthy &&
               !IsCircuitBreakerOpen() &&
               !AreCredentialsExpired();
    }

    // Add header
    public void AddHeader(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Header key cannot be empty", nameof(key));

        Headers[key] = value ?? string.Empty;
    }

    // Remove header
    public void RemoveHeader(string key)
    {
        if (Headers.ContainsKey(key))
        {
            Headers.Remove(key);
        }
    }

    // Add upstream server
    public void AddUpstreamServer(string serverUrl)
    {
        if (string.IsNullOrWhiteSpace(serverUrl))
            throw new ArgumentException("Server URL cannot be empty", nameof(serverUrl));

        if (!Uri.TryCreate(serverUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid server URL format", nameof(serverUrl));

        if (!UpstreamServers.Contains(serverUrl))
        {
            UpstreamServers.Add(serverUrl);
        }
    }

    // Remove upstream server
    public void RemoveUpstreamServer(string serverUrl)
    {
        if (UpstreamServers.Contains(serverUrl))
        {
            UpstreamServers.Remove(serverUrl);
            
            // Reset current index if it's out of bounds
            if (CurrentServerIndex >= UpstreamServers.Count)
            {
                CurrentServerIndex = 0;
            }
        }
    }

    // Update rate limits
    public void UpdateRateLimits(int perMinute, int perHour, int perDay)
    {
        if (perMinute <= 0 || perHour <= 0 || perDay <= 0)
            throw new ArgumentException("Rate limits must be positive values");

        RateLimitPerMinute = perMinute;
        RateLimitPerHour = perHour;
        RateLimitPerDay = perDay;

        // Update metadata
        Metadata["RateLimitsUpdatedAt"] = DateTime.UtcNow;
    }

    // Update retry configuration
    public void UpdateRetryConfiguration(int maxAttempts, TimeSpan delay)
    {
        if (maxAttempts < 0)
            throw new ArgumentException("Max retry attempts cannot be negative", nameof(maxAttempts));

        if (delay < TimeSpan.Zero)
            throw new ArgumentException("Retry delay cannot be negative", nameof(delay));

        MaxRetryAttempts = maxAttempts;
        RetryDelay = delay;

        // Update metadata
        Metadata["RetryConfigurationUpdatedAt"] = DateTime.UtcNow;
    }
}