using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Webhook Subscription Aggregate - Manages webhook subscriptions and event delivery
/// Supports event filtering, retry mechanisms, and delivery tracking
/// Industry Standard: Webhook patterns for real-time event notifications
/// </summary>
public class WebhookSubscription : AggregateRoot<Guid>
{
    // Core Properties
    public string SubscriptionCode { get; private set; }
    public string SubscriptionName { get; private set; }
    public string Description { get; private set; }
    public string CallbackUrl { get; private set; }
    public WebhookStatus Status { get; private set; }
    
    // Event Configuration
    public List<string> EventTypes { get; private set; }
    public Dictionary<string, object> EventFilters { get; private set; }
    public WebhookFormat Format { get; private set; }
    public string ContentType { get; private set; }
    
    // Security
    public string SecretKey { get; private set; }
    public SignatureMethod SignatureMethod { get; private set; }
    public Dictionary<string, string> Headers { get; private set; }
    public bool RequireHttps { get; private set; }
    public List<string> AllowedIpAddresses { get; private set; }
    
    // Delivery Configuration
    public int MaxRetryAttempts { get; private set; }
    public TimeSpan RetryDelay { get; private set; }
    public TimeSpan Timeout { get; private set; }
    public RetryStrategy RetryStrategy { get; private set; }
    public bool EnableBatching { get; private set; }
    public int BatchSize { get; private set; }
    public TimeSpan BatchTimeout { get; private set; }
    
    // Statistics
    public int SuccessfulDeliveries { get; private set; }
    public int FailedDeliveries { get; private set; }
    public int TotalDeliveryAttempts { get; private set; }
    public DateTime? LastDeliveryAt { get; private set; }
    public DateTime? LastSuccessfulDeliveryAt { get; private set; }
    public DateTime? LastFailedDeliveryAt { get; private set; }
    public string LastErrorMessage { get; private set; }
    public TimeSpan AverageDeliveryTime { get; private set; }
    
    // Health & Monitoring
    public WebhookHealthStatus HealthStatus { get; private set; }
    public DateTime? LastHealthCheck { get; private set; }
    public int ConsecutiveFailures { get; private set; }
    public DateTime? SuspendedAt { get; private set; }
    public string SuspensionReason { get; private set; }
    
    // Rate Limiting
    public int RateLimitPerMinute { get; private set; }
    public int RateLimitPerHour { get; private set; }
    public int CurrentMinuteDeliveries { get; private set; }
    public int CurrentHourDeliveries { get; private set; }
    public DateTime LastRateLimitReset { get; private set; }
    
    // Delivery History (for recent tracking)
    public List<WebhookDelivery> RecentDeliveries { get; private set; }
    public int MaxRecentDeliveries { get; private set; }
    
    // Metadata
    public Dictionary<string, object> Metadata { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string LastModifiedBy { get; private set; }

    // Private constructor for EF Core
    private WebhookSubscription() 
    {
        EventTypes = new List<string>();
        EventFilters = new Dictionary<string, object>();
        Headers = new Dictionary<string, string>();
        AllowedIpAddresses = new List<string>();
        RecentDeliveries = new List<WebhookDelivery>();
        Metadata = new Dictionary<string, object>();
    }

    // Factory method for creating new webhook subscription
    public static WebhookSubscription Create(
        string subscriptionCode,
        string subscriptionName,
        string callbackUrl,
        List<string> eventTypes,
        string createdBy,
        string description = null,
        WebhookFormat format = WebhookFormat.JSON,
        string secretKey = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(subscriptionCode))
            throw new ArgumentException("Subscription code cannot be empty", nameof(subscriptionCode));
        
        if (string.IsNullOrWhiteSpace(subscriptionName))
            throw new ArgumentException("Subscription name cannot be empty", nameof(subscriptionName));
        
        if (string.IsNullOrWhiteSpace(callbackUrl))
            throw new ArgumentException("Callback URL cannot be empty", nameof(callbackUrl));
        
        if (eventTypes == null || !eventTypes.Any())
            throw new ArgumentException("Event types cannot be empty", nameof(eventTypes));
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by cannot be empty", nameof(createdBy));

        // Validate callback URL
        if (!Uri.TryCreate(callbackUrl, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid callback URL format", nameof(callbackUrl));

        // Generate secret key if not provided
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            secretKey = GenerateSecretKey();
        }

        var subscription = new WebhookSubscription
        {
            Id = Guid.NewGuid(),
            SubscriptionCode = subscriptionCode,
            SubscriptionName = subscriptionName,
            Description = description,
            CallbackUrl = callbackUrl,
            Status = WebhookStatus.Active,
            EventTypes = new List<string>(eventTypes),
            EventFilters = new Dictionary<string, object>(),
            Format = format,
            ContentType = GetContentTypeForFormat(format),
            SecretKey = secretKey,
            SignatureMethod = SignatureMethod.HMACSHA256,
            Headers = new Dictionary<string, string>(),
            RequireHttps = uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase),
            AllowedIpAddresses = new List<string>(),
            MaxRetryAttempts = 3,
            RetryDelay = TimeSpan.FromSeconds(5),
            Timeout = TimeSpan.FromSeconds(30),
            RetryStrategy = RetryStrategy.ExponentialBackoff,
            EnableBatching = false,
            BatchSize = 10,
            BatchTimeout = TimeSpan.FromMinutes(5),
            SuccessfulDeliveries = 0,
            FailedDeliveries = 0,
            TotalDeliveryAttempts = 0,
            AverageDeliveryTime = TimeSpan.Zero,
            HealthStatus = WebhookHealthStatus.Healthy,
            ConsecutiveFailures = 0,
            RateLimitPerMinute = 60,
            RateLimitPerHour = 3600,
            CurrentMinuteDeliveries = 0,
            CurrentHourDeliveries = 0,
            LastRateLimitReset = DateTime.UtcNow,
            RecentDeliveries = new List<WebhookDelivery>(),
            MaxRecentDeliveries = 100,
            Metadata = new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        // Add creation event
        subscription.AddDomainEvent(new WebhookSubscriptionCreatedDomainEvent(
            subscription.Id,
            subscription.SubscriptionCode,
            subscription.SubscriptionName,
            subscription.CallbackUrl,
            subscription.EventTypes,
            subscription.CreatedBy));

        return subscription;
    }

    // Subscribe to additional event types
    public void Subscribe(List<string> eventTypes)
    {
        if (eventTypes == null || !eventTypes.Any())
            throw new ArgumentException("Event types cannot be empty", nameof(eventTypes));

        var newEventTypes = eventTypes.Where(et => !EventTypes.Contains(et)).ToList();
        if (newEventTypes.Any())
        {
            EventTypes.AddRange(newEventTypes);
            LastModifiedAt = DateTime.UtcNow;

            // Update metadata
            Metadata["EventTypesUpdatedAt"] = DateTime.UtcNow;
            Metadata["TotalEventTypes"] = EventTypes.Count;

            AddDomainEvent(new WebhookSubscriptionUpdatedDomainEvent(
                Id,
                SubscriptionCode,
                "EventTypes",
                string.Join(",", newEventTypes)));
        }
    }

    // Unsubscribe from event types
    public void Unsubscribe(List<string> eventTypes)
    {
        if (eventTypes == null || !eventTypes.Any())
            return;

        var removedEventTypes = eventTypes.Where(et => EventTypes.Contains(et)).ToList();
        if (removedEventTypes.Any())
        {
            foreach (var eventType in removedEventTypes)
            {
                EventTypes.Remove(eventType);
            }

            if (!EventTypes.Any())
            {
                throw new InvalidOperationException("Cannot remove all event types. Subscription must have at least one event type.");
            }

            LastModifiedAt = DateTime.UtcNow;

            // Update metadata
            Metadata["EventTypesUpdatedAt"] = DateTime.UtcNow;
            Metadata["TotalEventTypes"] = EventTypes.Count;

            AddDomainEvent(new WebhookSubscriptionUpdatedDomainEvent(
                Id,
                SubscriptionCode,
                "EventTypes",
                $"Removed: {string.Join(",", removedEventTypes)}"));
        }
    }

    // Update callback URL
    public void UpdateCallbackUrl(string newUrl, string modifiedBy = null)
    {
        if (string.IsNullOrWhiteSpace(newUrl))
            throw new ArgumentException("Callback URL cannot be empty", nameof(newUrl));

        // Validate URL format
        if (!Uri.TryCreate(newUrl, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid callback URL format", nameof(newUrl));

        var oldUrl = CallbackUrl;
        CallbackUrl = newUrl;
        RequireHttps = uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Reset health status and consecutive failures
        HealthStatus = WebhookHealthStatus.Healthy;
        ConsecutiveFailures = 0;

        // Update metadata
        Metadata["CallbackUrlUpdatedAt"] = DateTime.UtcNow;
        Metadata["PreviousCallbackUrl"] = oldUrl;
        if (!string.IsNullOrWhiteSpace(modifiedBy))
            Metadata["CallbackUrlUpdatedBy"] = modifiedBy;

        AddDomainEvent(new WebhookSubscriptionUpdatedDomainEvent(
            Id,
            SubscriptionCode,
            "CallbackUrl",
            newUrl));
    }

    // Update secret key
    public void UpdateSecretKey(string newSecret = null, string modifiedBy = null)
    {
        SecretKey = string.IsNullOrWhiteSpace(newSecret) ? GenerateSecretKey() : newSecret;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;

        // Update metadata
        Metadata["SecretKeyUpdatedAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(modifiedBy))
            Metadata["SecretKeyUpdatedBy"] = modifiedBy;

        AddDomainEvent(new WebhookSubscriptionUpdatedDomainEvent(
            Id,
            SubscriptionCode,
            "SecretKey",
            "Updated"));
    }

    // Record successful delivery
    public void RecordSuccessfulDelivery(TimeSpan deliveryTime, int httpStatusCode = 200, string response = null)
    {
        SuccessfulDeliveries++;
        TotalDeliveryAttempts++;
        LastDeliveryAt = DateTime.UtcNow;
        LastSuccessfulDeliveryAt = DateTime.UtcNow;
        ConsecutiveFailures = 0;

        // Update average delivery time
        if (SuccessfulDeliveries > 1)
        {
            var totalTime = AverageDeliveryTime.TotalMilliseconds * (SuccessfulDeliveries - 1) + deliveryTime.TotalMilliseconds;
            AverageDeliveryTime = TimeSpan.FromMilliseconds(totalTime / SuccessfulDeliveries);
        }
        else
        {
            AverageDeliveryTime = deliveryTime;
        }

        // Update health status
        if (HealthStatus != WebhookHealthStatus.Healthy)
        {
            HealthStatus = WebhookHealthStatus.Healthy;
            SuspendedAt = null;
            SuspensionReason = null;
        }

        // Update rate limiting
        UpdateRateLimitCounters();

        // Add to recent deliveries
        AddRecentDelivery(new WebhookDelivery
        {
            DeliveryId = Guid.NewGuid().ToString(),
            AttemptedAt = DateTime.UtcNow,
            Success = true,
            HttpStatusCode = httpStatusCode,
            ResponseBody = response,
            DeliveryTime = deliveryTime,
            AttemptNumber = 1
        });

        AddDomainEvent(new WebhookDeliverySucceededDomainEvent(
            Id,
            SubscriptionCode,
            CallbackUrl,
            httpStatusCode,
            deliveryTime,
            DateTime.UtcNow));
    }

    // Record failed delivery
    public void RecordFailedDelivery(string errorMessage, int? httpStatusCode = null, string response = null, int attemptNumber = 1)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));

        FailedDeliveries++;
        TotalDeliveryAttempts++;
        LastDeliveryAt = DateTime.UtcNow;
        LastFailedDeliveryAt = DateTime.UtcNow;
        LastErrorMessage = errorMessage;
        ConsecutiveFailures++;

        // Update health status based on consecutive failures
        if (ConsecutiveFailures >= 5)
        {
            HealthStatus = WebhookHealthStatus.Failing;
        }
        else if (ConsecutiveFailures >= 10)
        {
            HealthStatus = WebhookHealthStatus.Suspended;
            SuspendedAt = DateTime.UtcNow;
            SuspensionReason = $"Too many consecutive failures: {ConsecutiveFailures}";
        }

        // Update rate limiting
        UpdateRateLimitCounters();

        // Add to recent deliveries
        AddRecentDelivery(new WebhookDelivery
        {
            DeliveryId = Guid.NewGuid().ToString(),
            AttemptedAt = DateTime.UtcNow,
            Success = false,
            HttpStatusCode = httpStatusCode,
            ResponseBody = response,
            ErrorMessage = errorMessage,
            AttemptNumber = attemptNumber
        });

        AddDomainEvent(new WebhookDeliveryFailedDomainEvent(
            Id,
            SubscriptionCode,
            CallbackUrl,
            errorMessage,
            httpStatusCode,
            attemptNumber,
            DateTime.UtcNow));
    }

    // Enable subscription
    public void Enable(string enabledBy = null)
    {
        if (Status == WebhookStatus.Active)
            return;

        Status = WebhookStatus.Active;
        HealthStatus = WebhookHealthStatus.Healthy;
        ConsecutiveFailures = 0;
        SuspendedAt = null;
        SuspensionReason = null;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = enabledBy;

        // Update metadata
        Metadata["EnabledAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(enabledBy))
            Metadata["EnabledBy"] = enabledBy;

        AddDomainEvent(new WebhookSubscriptionEnabledDomainEvent(
            Id,
            SubscriptionCode,
            enabledBy));
    }

    // Disable subscription
    public void Disable(string disabledBy = null, string reason = null)
    {
        if (Status == WebhookStatus.Inactive)
            return;

        Status = WebhookStatus.Inactive;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = disabledBy;

        // Update metadata
        Metadata["DisabledAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(disabledBy))
            Metadata["DisabledBy"] = disabledBy;
        if (!string.IsNullOrWhiteSpace(reason))
            Metadata["DisabledReason"] = reason;

        AddDomainEvent(new WebhookSubscriptionDisabledDomainEvent(
            Id,
            SubscriptionCode,
            disabledBy,
            reason));
    }

    // Suspend subscription
    public void Suspend(string suspendedBy = null, string reason = null)
    {
        if (Status == WebhookStatus.Suspended)
            return;

        Status = WebhookStatus.Suspended;
        HealthStatus = WebhookHealthStatus.Suspended;
        SuspendedAt = DateTime.UtcNow;
        SuspensionReason = reason ?? "Manually suspended";
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = suspendedBy;

        // Update metadata
        Metadata["SuspendedAt"] = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(suspendedBy))
            Metadata["SuspendedBy"] = suspendedBy;
        if (!string.IsNullOrWhiteSpace(reason))
            Metadata["SuspensionReason"] = reason;

        AddDomainEvent(new WebhookSubscriptionSuspendedDomainEvent(
            Id,
            SubscriptionCode,
            suspendedBy,
            reason));
    }

    // Check if event type is subscribed
    public bool IsSubscribedToEvent(string eventType)
    {
        return EventTypes.Contains(eventType, StringComparer.OrdinalIgnoreCase);
    }

    // Check if event matches filters
    public bool MatchesEventFilters(string eventType, Dictionary<string, object> eventData)
    {
        if (!IsSubscribedToEvent(eventType))
            return false;

        if (!EventFilters.Any())
            return true;

        // Apply filters (simplified implementation)
        foreach (var filter in EventFilters)
        {
            if (eventData.ContainsKey(filter.Key))
            {
                var eventValue = eventData[filter.Key]?.ToString();
                var filterValue = filter.Value?.ToString();
                
                if (!string.Equals(eventValue, filterValue, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Check if rate limit is exceeded
    public bool IsRateLimitExceeded()
    {
        UpdateRateLimitCounters();
        
        return CurrentMinuteDeliveries >= RateLimitPerMinute ||
               CurrentHourDeliveries >= RateLimitPerHour;
    }

    // Get next retry delay based on strategy
    public TimeSpan GetNextRetryDelay(int attemptNumber)
    {
        return RetryStrategy switch
        {
            RetryStrategy.FixedDelay => RetryDelay,
            RetryStrategy.ExponentialBackoff => TimeSpan.FromMilliseconds(
                RetryDelay.TotalMilliseconds * Math.Pow(2, attemptNumber - 1)),
            RetryStrategy.LinearBackoff => TimeSpan.FromMilliseconds(
                RetryDelay.TotalMilliseconds * attemptNumber),
            _ => RetryDelay
        };
    }

    // Update rate limit counters
    private void UpdateRateLimitCounters()
    {
        var now = DateTime.UtcNow;
        
        // Reset counters if time windows have passed
        if (now.Subtract(LastRateLimitReset).TotalMinutes >= 1)
        {
            CurrentMinuteDeliveries = 0;
        }
        
        if (now.Subtract(LastRateLimitReset).TotalHours >= 1)
        {
            CurrentHourDeliveries = 0;
            LastRateLimitReset = now;
        }

        // Increment counters
        CurrentMinuteDeliveries++;
        CurrentHourDeliveries++;
    }

    // Add recent delivery
    private void AddRecentDelivery(WebhookDelivery delivery)
    {
        RecentDeliveries.Add(delivery);
        
        // Keep only the most recent deliveries
        if (RecentDeliveries.Count > MaxRecentDeliveries)
        {
            RecentDeliveries.RemoveAt(0);
        }
    }

    // Generate secret key
    private static string GenerateSecretKey()
    {
        var bytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes);
    }

    // Get content type for format
    private static string GetContentTypeForFormat(WebhookFormat format)
    {
        return format switch
        {
            WebhookFormat.JSON => "application/json",
            WebhookFormat.XML => "application/xml",
            WebhookFormat.FormData => "application/x-www-form-urlencoded",
            _ => "application/json"
        };
    }

    // Get success rate percentage
    public decimal GetSuccessRate()
    {
        var totalDeliveries = SuccessfulDeliveries + FailedDeliveries;
        if (totalDeliveries == 0)
            return 0;

        return Math.Round((decimal)SuccessfulDeliveries / totalDeliveries * 100, 2);
    }

    // Get delivery statistics
    public Dictionary<string, object> GetStatistics()
    {
        return new Dictionary<string, object>
        {
            ["SubscriptionCode"] = SubscriptionCode,
            ["Status"] = Status.ToString(),
            ["HealthStatus"] = HealthStatus.ToString(),
            ["EventTypes"] = EventTypes,
            ["SuccessfulDeliveries"] = SuccessfulDeliveries,
            ["FailedDeliveries"] = FailedDeliveries,
            ["TotalDeliveryAttempts"] = TotalDeliveryAttempts,
            ["SuccessRate"] = GetSuccessRate(),
            ["ConsecutiveFailures"] = ConsecutiveFailures,
            ["AverageDeliveryTime"] = AverageDeliveryTime.TotalMilliseconds,
            ["LastDeliveryAt"] = LastDeliveryAt,
            ["LastSuccessfulDeliveryAt"] = LastSuccessfulDeliveryAt,
            ["LastFailedDeliveryAt"] = LastFailedDeliveryAt,
            ["CreatedAt"] = CreatedAt
        };
    }
}

/// <summary>
/// Webhook Delivery - Represents a single webhook delivery attempt
/// </summary>
public class WebhookDelivery
{
    public string DeliveryId { get; set; }
    public DateTime AttemptedAt { get; set; }
    public bool Success { get; set; }
    public int? HttpStatusCode { get; set; }
    public string ResponseBody { get; set; }
    public string ErrorMessage { get; set; }
    public TimeSpan? DeliveryTime { get; set; }
    public int AttemptNumber { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}