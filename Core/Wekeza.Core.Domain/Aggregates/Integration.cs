using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Integration Aggregate - Manages third-party system integrations
/// Supports payment gateways, credit bureaus, KYC providers, and other external services
/// Industry Standard: Finacle SOA & T24 Integration Framework
/// </summary>
public class Integration : AggregateRoot<Guid>
{
    // Core Properties
    public string IntegrationCode { get; private set; }
    public string IntegrationName { get; private set; }
    public IntegrationType Type { get; private set; }
    public IntegrationStatus Status { get; private set; }
    public string Description { get; private set; }
    public string Version { get; private set; }
    
    // Connection Details
    public string EndpointUrl { get; private set; }
    public AuthenticationType AuthType { get; private set; }
    public Dictionary<string, string> AuthenticationConfig { get; private set; }
    public Dictionary<string, string> Headers { get; private set; }
    public int TimeoutSeconds { get; private set; }
    public int RetryAttempts { get; private set; }
    public TimeSpan RetryDelay { get; private set; }
    
    // Configuration
    public Dictionary<string, object> Configuration { get; private set; }
    public string DataFormat { get; private set; } // JSON, XML, CSV
    public string Protocol { get; private set; } // REST, SOAP, GraphQL
    public bool IsActive { get; private set; }
    public bool EnableLogging { get; private set; }
    public bool EnableMetrics { get; private set; }
    
    // Monitoring & Performance
    public DateTime? LastSuccessfulCall { get; private set; }
    public DateTime? LastFailedCall { get; private set; }
    public int SuccessCount { get; private set; }
    public int FailureCount { get; private set; }
    public decimal SuccessRate { get; private set; }
    public TimeSpan AverageResponseTime { get; private set; }
    public TimeSpan MaxResponseTime { get; private set; }
    public TimeSpan MinResponseTime { get; private set; }
    
    // Error Handling
    public string LastErrorMessage { get; private set; }
    public DateTime? LastErrorAt { get; private set; }
    public int ConsecutiveFailures { get; private set; }
    public bool IsCircuitBreakerOpen { get; private set; }
    public DateTime? CircuitBreakerOpenedAt { get; private set; }
    
    // Maintenance
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string LastModifiedBy { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    // Private constructor for EF Core
    private Integration() 
    {
        AuthenticationConfig = new Dictionary<string, string>();
        Headers = new Dictionary<string, string>();
        Configuration = new Dictionary<string, object>();
        Metadata = new Dictionary<string, object>();
    }

    // Factory method for creating new integration
    public static Integration Create(
        string integrationCode,
        string integrationName,
        IntegrationType type,
        string endpointUrl,
        string createdBy,
        string description = null,
        string version = "1.0")
    {
        // Validation
        if (string.IsNullOrWhiteSpace(integrationCode))
            throw new ArgumentException("Integration code cannot be empty", nameof(integrationCode));
        
        if (string.IsNullOrWhiteSpace(integrationName))
            throw new ArgumentException("Integration name cannot be empty", nameof(integrationName));
        
        if (string.IsNullOrWhiteSpace(endpointUrl))
            throw new ArgumentException("Endpoint URL cannot be empty", nameof(endpointUrl));
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by cannot be empty", nameof(createdBy));

        // Validate URL format
        if (!Uri.TryCreate(endpointUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid endpoint URL format", nameof(endpointUrl));

        var integration = new Integration
        {
            Id = Guid.NewGuid(),
            IntegrationCode = integrationCode,
            IntegrationName = integrationName,
            Type = type,
            Status = IntegrationStatus.Inactive,
            Description = description,
            Version = version,
            EndpointUrl = endpointUrl,
            AuthType = AuthenticationType.None,
            AuthenticationConfig = new Dictionary<string, string>(),
            Headers = new Dictionary<string, string>(),
            TimeoutSeconds = 30,
            RetryAttempts = 3,
            RetryDelay = TimeSpan.FromSeconds(1),
            Configuration = new Dictionary<string, object>(),
            DataFormat = "JSON",
            Protocol = "REST",
            IsActive = false,
            EnableLogging = true,
            EnableMetrics = true,
            SuccessCount = 0,
            FailureCount = 0,
            SuccessRate = 0,
            AverageResponseTime = TimeSpan.Zero,
            MaxResponseTime = TimeSpan.Zero,
            MinResponseTime = TimeSpan.MaxValue,
            ConsecutiveFailures = 0,
            IsCircuitBreakerOpen = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Metadata = new Dictionary<string, object>()
        };

        // Add creation event
        integration.AddDomainEvent(new IntegrationCreatedDomainEvent(
            integration.Id,
            integration.IntegrationCode,
            integration.IntegrationName,
            integration.Type,
            integration.CreatedBy));

        return integration;
    }

    // Update endpoint URL
    public void UpdateEndpoint(string endpointUrl, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(endpointUrl))
            throw new ArgumentException("Endpoint URL cannot be empty", nameof(endpointUrl));

        if (!Uri.TryCreate(endpointUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid endpoint URL format", nameof(endpointUrl));

        var oldEndpoint = EndpointUrl;
        EndpointUrl = endpointUrl;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Reset circuit breaker if endpoint changed
        if (IsCircuitBreakerOpen)
        {
            CloseCircuitBreaker();
        }

        AddDomainEvent(new IntegrationEndpointUpdatedDomainEvent(
            Id,
            IntegrationCode,
            oldEndpoint,
            endpointUrl,
            modifiedBy));
    }

    // Update authentication configuration
    public void UpdateAuthentication(
        AuthenticationType authType, 
        Dictionary<string, string> authConfig,
        string modifiedBy)
    {
        if (authConfig == null)
            throw new ArgumentNullException(nameof(authConfig));

        // Validate required auth config based on type
        ValidateAuthenticationConfig(authType, authConfig);

        AuthType = authType;
        AuthenticationConfig = new Dictionary<string, string>(authConfig);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        AddDomainEvent(new IntegrationAuthenticationUpdatedDomainEvent(
            Id,
            IntegrationCode,
            authType,
            modifiedBy));
    }

    // Record successful call
    public void RecordSuccess(TimeSpan responseTime)
    {
        SuccessCount++;
        LastSuccessfulCall = DateTime.UtcNow;
        ConsecutiveFailures = 0;

        // Update response time metrics
        UpdateResponseTimeMetrics(responseTime);

        // Calculate success rate
        var totalCalls = SuccessCount + FailureCount;
        SuccessRate = totalCalls > 0 ? Math.Round((decimal)SuccessCount / totalCalls * 100, 2) : 0;

        // Close circuit breaker if it was open
        if (IsCircuitBreakerOpen)
        {
            CloseCircuitBreaker();
        }

        // Update metadata
        Metadata["LastSuccessAt"] = LastSuccessfulCall;
        Metadata["TotalCalls"] = totalCalls;

        AddDomainEvent(new IntegrationCallSucceededDomainEvent(
            Id,
            IntegrationCode,
            responseTime,
            SuccessRate));
    }

    // Record failed call
    public void RecordFailure(string errorMessage, int? statusCode = null)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));

        FailureCount++;
        LastFailedCall = DateTime.UtcNow;
        LastErrorMessage = errorMessage;
        LastErrorAt = DateTime.UtcNow;
        ConsecutiveFailures++;

        // Calculate success rate
        var totalCalls = SuccessCount + FailureCount;
        SuccessRate = totalCalls > 0 ? Math.Round((decimal)SuccessCount / totalCalls * 100, 2) : 0;

        // Open circuit breaker if too many consecutive failures
        if (ConsecutiveFailures >= 5 && !IsCircuitBreakerOpen)
        {
            OpenCircuitBreaker();
        }

        // Update metadata
        Metadata["LastFailureAt"] = LastFailedCall;
        Metadata["LastError"] = errorMessage;
        Metadata["StatusCode"] = statusCode;
        Metadata["TotalCalls"] = totalCalls;

        AddDomainEvent(new IntegrationCallFailedDomainEvent(
            Id,
            IntegrationCode,
            errorMessage,
            statusCode,
            ConsecutiveFailures,
            SuccessRate));
    }

    // Activate integration
    public void Activate(string activatedBy)
    {
        if (string.IsNullOrWhiteSpace(activatedBy))
            throw new ArgumentException("Activated by cannot be empty", nameof(activatedBy));

        if (Status == IntegrationStatus.Active)
            return;

        Status = IntegrationStatus.Active;
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = activatedBy;

        // Reset circuit breaker
        if (IsCircuitBreakerOpen)
        {
            CloseCircuitBreaker();
        }

        // Update metadata
        Metadata["ActivatedAt"] = DateTime.UtcNow;
        Metadata["ActivatedBy"] = activatedBy;

        AddDomainEvent(new IntegrationActivatedDomainEvent(
            Id,
            IntegrationCode,
            activatedBy));
    }

    // Deactivate integration
    public void Deactivate(string deactivatedBy, string reason = null)
    {
        if (string.IsNullOrWhiteSpace(deactivatedBy))
            throw new ArgumentException("Deactivated by cannot be empty", nameof(deactivatedBy));

        if (Status == IntegrationStatus.Inactive)
            return;

        Status = IntegrationStatus.Inactive;
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = deactivatedBy;

        // Update metadata
        Metadata["DeactivatedAt"] = DateTime.UtcNow;
        Metadata["DeactivatedBy"] = deactivatedBy;
        if (!string.IsNullOrWhiteSpace(reason))
            Metadata["DeactivationReason"] = reason;

        AddDomainEvent(new IntegrationDeactivatedDomainEvent(
            Id,
            IntegrationCode,
            deactivatedBy,
            reason));
    }

    // Update configuration
    public void UpdateConfiguration(Dictionary<string, object> configuration, string modifiedBy)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        Configuration = new Dictionary<string, object>(configuration);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["ConfigurationUpdatedAt"] = DateTime.UtcNow;
        Metadata["ConfigurationUpdatedBy"] = modifiedBy;

        AddDomainEvent(new IntegrationConfigurationUpdatedDomainEvent(
            Id,
            IntegrationCode,
            modifiedBy));
    }

    // Set maintenance mode
    public void SetMaintenanceMode(bool inMaintenance, string modifiedBy, string reason = null)
    {
        var oldStatus = Status;
        Status = inMaintenance ? IntegrationStatus.Maintenance : IntegrationStatus.Active;
        IsActive = !inMaintenance;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["MaintenanceModeChangedAt"] = DateTime.UtcNow;
        Metadata["MaintenanceModeChangedBy"] = modifiedBy;
        if (!string.IsNullOrWhiteSpace(reason))
            Metadata["MaintenanceReason"] = reason;

        AddDomainEvent(new IntegrationMaintenanceModeChangedDomainEvent(
            Id,
            IntegrationCode,
            inMaintenance,
            reason,
            modifiedBy));
    }

    // Add custom header
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

    // Add configuration value
    public void AddConfiguration(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Configuration key cannot be empty", nameof(key));

        Configuration[key] = value;
    }

    // Get configuration value
    public T GetConfiguration<T>(string key, T defaultValue = default)
    {
        if (Configuration.ContainsKey(key) && Configuration[key] is T value)
            return value;
        
        return defaultValue;
    }

    // Check if integration is healthy
    public bool IsHealthy()
    {
        if (!IsActive || IsCircuitBreakerOpen)
            return false;

        // Consider healthy if success rate > 95% and no recent failures
        var isRecentlySuccessful = LastSuccessfulCall.HasValue && 
            LastSuccessfulCall.Value > DateTime.UtcNow.AddMinutes(-5);
        
        var hasGoodSuccessRate = SuccessRate >= 95;
        var hasLowConsecutiveFailures = ConsecutiveFailures < 3;

        return hasGoodSuccessRate && hasLowConsecutiveFailures && isRecentlySuccessful;
    }

    // Get health status
    public IntegrationHealthStatus GetHealthStatus()
    {
        if (!IsActive)
            return IntegrationHealthStatus.Inactive;

        if (IsCircuitBreakerOpen)
            return IntegrationHealthStatus.CircuitBreakerOpen;

        if (Status == IntegrationStatus.Maintenance)
            return IntegrationHealthStatus.Maintenance;

        if (ConsecutiveFailures >= 3)
            return IntegrationHealthStatus.Degraded;

        if (SuccessRate >= 99)
            return IntegrationHealthStatus.Excellent;

        if (SuccessRate >= 95)
            return IntegrationHealthStatus.Good;

        if (SuccessRate >= 90)
            return IntegrationHealthStatus.Fair;

        return IntegrationHealthStatus.Poor;
    }

    // Private methods
    private void UpdateResponseTimeMetrics(TimeSpan responseTime)
    {
        var totalCalls = SuccessCount + FailureCount;
        
        // Update average response time
        if (totalCalls == 1)
        {
            AverageResponseTime = responseTime;
            MinResponseTime = responseTime;
            MaxResponseTime = responseTime;
        }
        else
        {
            // Calculate running average
            var totalTime = AverageResponseTime.TotalMilliseconds * (totalCalls - 1) + responseTime.TotalMilliseconds;
            AverageResponseTime = TimeSpan.FromMilliseconds(totalTime / totalCalls);
            
            // Update min/max
            if (responseTime < MinResponseTime)
                MinResponseTime = responseTime;
            
            if (responseTime > MaxResponseTime)
                MaxResponseTime = responseTime;
        }
    }

    private void OpenCircuitBreaker()
    {
        IsCircuitBreakerOpen = true;
        CircuitBreakerOpenedAt = DateTime.UtcNow;
        
        AddDomainEvent(new IntegrationCircuitBreakerOpenedDomainEvent(
            Id,
            IntegrationCode,
            ConsecutiveFailures,
            CircuitBreakerOpenedAt.Value));
    }

    private void CloseCircuitBreaker()
    {
        IsCircuitBreakerOpen = false;
        CircuitBreakerOpenedAt = null;
        ConsecutiveFailures = 0;
        
        AddDomainEvent(new IntegrationCircuitBreakerClosedDomainEvent(
            Id,
            IntegrationCode,
            DateTime.UtcNow));
    }

    private void ValidateAuthenticationConfig(AuthenticationType authType, Dictionary<string, string> config)
    {
        switch (authType)
        {
            case AuthenticationType.BasicAuth:
                if (!config.ContainsKey("username") || !config.ContainsKey("password"))
                    throw new ArgumentException("Basic auth requires username and password");
                break;
            
            case AuthenticationType.BearerToken:
            case AuthenticationType.APIKey:
                if (!config.ContainsKey("token") && !config.ContainsKey("key"))
                    throw new ArgumentException("Token/API key authentication requires token or key");
                break;
            
            case AuthenticationType.OAuth2:
                if (!config.ContainsKey("clientId") || !config.ContainsKey("clientSecret"))
                    throw new ArgumentException("OAuth2 requires clientId and clientSecret");
                break;
            
            case AuthenticationType.JWT:
                if (!config.ContainsKey("secret") && !config.ContainsKey("publicKey"))
                    throw new ArgumentException("JWT requires secret or publicKey");
                break;
        }
    }
}

/// <summary>
/// Integration Health Status enumeration
/// </summary>
public enum IntegrationHealthStatus
{
    Excellent,
    Good,
    Fair,
    Poor,
    Degraded,
    CircuitBreakerOpen,
    Maintenance,
    Inactive
}