using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// SystemMonitor Aggregate - Real-time system health monitoring and alerting
/// Supports enterprise-grade monitoring, performance tracking, and automated alerting
/// </summary>
public class SystemMonitor : AggregateRoot
{
    // Core Properties
    public string MonitorCode { get; private set; }
    public string MonitorName { get; private set; }
    public MonitorType Type { get; private set; }
    public MonitorStatus Status { get; private set; }
    
    // Monitoring Configuration
    public string TargetResource { get; private set; }
    public Dictionary<string, object> MonitoringRules { get; private set; }
    public TimeSpan CheckInterval { get; private set; }
    public int RetryAttempts { get; private set; }
    public TimeSpan Timeout { get; private set; }
    
    // Thresholds & Alerts
    public Dictionary<string, decimal> Thresholds { get; private set; }
    public List<AlertRule> AlertRules { get; private set; }
    public AlertSeverity DefaultSeverity { get; private set; }
    public List<string> NotificationChannels { get; private set; }
    
    // Current State
    public MonitorHealth Health { get; private set; }
    public DateTime LastCheckAt { get; private set; }
    public Dictionary<string, object> LastCheckResult { get; private set; }
    public string? LastErrorMessage { get; private set; }
    public int ConsecutiveFailures { get; private set; }
    
    // Performance Metrics
    public TimeSpan AverageResponseTime { get; private set; }
    public decimal SuccessRate { get; private set; }
    public int TotalChecks { get; private set; }
    public int SuccessfulChecks { get; private set; }
    public int FailedChecks { get; private set; }
    
    // Configuration & Metadata
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private SystemMonitor()
    {
        MonitoringRules = new Dictionary<string, object>();
        Thresholds = new Dictionary<string, decimal>();
        AlertRules = new List<AlertRule>();
        NotificationChannels = new List<string>();
        LastCheckResult = new Dictionary<string, object>();
        Metadata = new Dictionary<string, object>();
    }

    public SystemMonitor(
        string monitorCode,
        string monitorName,
        MonitorType type,
        string targetResource,
        string createdBy,
        TimeSpan? checkInterval = null) : this()
    {
        if (string.IsNullOrWhiteSpace(monitorCode))
            throw new ArgumentException("Monitor code cannot be empty", nameof(monitorCode));
        if (string.IsNullOrWhiteSpace(monitorName))
            throw new ArgumentException("Monitor name cannot be empty", nameof(monitorName));
        if (string.IsNullOrWhiteSpace(targetResource))
            throw new ArgumentException("Target resource cannot be empty", nameof(targetResource));

        Id = Guid.NewGuid();
        MonitorCode = monitorCode;
        MonitorName = monitorName;
        Type = type;
        TargetResource = targetResource;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        
        Status = MonitorStatus.Active;
        Health = MonitorHealth.Unknown;
        CheckInterval = checkInterval ?? TimeSpan.FromMinutes(5);
        RetryAttempts = 3;
        Timeout = TimeSpan.FromSeconds(30);
        DefaultSeverity = AlertSeverity.Medium;
        
        // Initialize performance metrics
        SuccessRate = 100m;
        AverageResponseTime = TimeSpan.Zero;

        AddDomainEvent(new SystemMonitorCreatedDomainEvent(Id, MonitorCode, MonitorName, Type, CreatedBy));
    }

    // Business Methods
    public void UpdateConfiguration(Dictionary<string, object> rules, string updatedBy)
    {
        MonitoringRules = rules ?? new Dictionary<string, object>();
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new SystemMonitorConfigurationUpdatedDomainEvent(Id, MonitorCode, updatedBy));
    }

    public void SetCheckInterval(TimeSpan interval, string updatedBy)
    {
        if (interval < TimeSpan.FromSeconds(10))
            throw new ArgumentException("Check interval cannot be less than 10 seconds");

        CheckInterval = interval;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new SystemMonitorIntervalUpdatedDomainEvent(Id, MonitorCode, interval, updatedBy));
    }

    public void SetThreshold(string metric, decimal value, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(metric))
            throw new ArgumentException("Metric name cannot be empty", nameof(metric));

        Thresholds[metric] = value;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new SystemMonitorThresholdUpdatedDomainEvent(Id, MonitorCode, metric, value, updatedBy));
    }

    public void RemoveThreshold(string metric, string updatedBy)
    {
        if (Thresholds.Remove(metric))
        {
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = updatedBy;

            AddDomainEvent(new SystemMonitorThresholdRemovedDomainEvent(Id, MonitorCode, metric, updatedBy));
        }
    }

    public void AddAlertRule(AlertRule rule, string addedBy)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        // Remove existing rule with same condition if exists
        AlertRules.RemoveAll(r => r.Condition == rule.Condition);
        AlertRules.Add(rule);
        
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = addedBy;

        AddDomainEvent(new SystemMonitorAlertRuleAddedDomainEvent(Id, MonitorCode, rule.Condition, rule.Severity, addedBy));
    }

    public void RemoveAlertRule(string condition, string removedBy)
    {
        var removed = AlertRules.RemoveAll(r => r.Condition == condition);
        if (removed > 0)
        {
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = removedBy;

            AddDomainEvent(new SystemMonitorAlertRuleRemovedDomainEvent(Id, MonitorCode, condition, removedBy));
        }
    }

    public void AddNotificationChannel(string channel, string addedBy)
    {
        if (string.IsNullOrWhiteSpace(channel))
            throw new ArgumentException("Channel cannot be empty", nameof(channel));

        if (!NotificationChannels.Contains(channel))
        {
            NotificationChannels.Add(channel);
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = addedBy;

            AddDomainEvent(new SystemMonitorNotificationChannelAddedDomainEvent(Id, MonitorCode, channel, addedBy));
        }
    }

    public void RemoveNotificationChannel(string channel, string removedBy)
    {
        if (NotificationChannels.Remove(channel))
        {
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = removedBy;

            AddDomainEvent(new SystemMonitorNotificationChannelRemovedDomainEvent(Id, MonitorCode, channel, removedBy));
        }
    }

    public void RecordCheckResult(bool success, Dictionary<string, object> result, TimeSpan responseTime)
    {
        LastCheckAt = DateTime.UtcNow;
        LastCheckResult = result ?? new Dictionary<string, object>();
        TotalChecks++;

        if (success)
        {
            SuccessfulChecks++;
            ConsecutiveFailures = 0;
            LastErrorMessage = null;
            
            // Update health based on consecutive successes
            if (Health == MonitorHealth.Unhealthy && ConsecutiveFailures == 0)
            {
                UpdateHealth(MonitorHealth.Healthy);
            }
        }
        else
        {
            FailedChecks++;
            ConsecutiveFailures++;
            
            if (result.ContainsKey("Error"))
            {
                LastErrorMessage = result["Error"]?.ToString();
            }

            // Update health based on consecutive failures
            if (ConsecutiveFailures >= 3)
            {
                UpdateHealth(MonitorHealth.Unhealthy);
            }
            else if (ConsecutiveFailures >= 1)
            {
                UpdateHealth(MonitorHealth.Degraded);
            }
        }

        // Update performance metrics
        UpdatePerformanceMetrics(responseTime);

        // Check thresholds and trigger alerts
        CheckThresholdsAndTriggerAlerts(result);

        AddDomainEvent(new SystemMonitorCheckCompletedDomainEvent(Id, MonitorCode, success, Health, responseTime));
    }

    public void TriggerAlert(AlertSeverity severity, string message, Dictionary<string, object>? context = null)
    {
        var alert = new MonitorAlert
        {
            Id = Guid.NewGuid(),
            MonitorId = Id,
            MonitorCode = MonitorCode,
            Severity = severity,
            Message = message,
            Context = context ?? new Dictionary<string, object>(),
            TriggeredAt = DateTime.UtcNow,
            Status = AlertStatus.Active
        };

        Metadata[$"LastAlert_{severity}"] = DateTime.UtcNow;
        Metadata["LastAlertMessage"] = message;

        AddDomainEvent(new SystemMonitorAlertTriggeredDomainEvent(Id, MonitorCode, severity, message, alert.Id));
    }

    public void UpdateHealth(MonitorHealth health)
    {
        if (Health == health)
            return;

        var previousHealth = Health;
        Health = health;

        AddDomainEvent(new SystemMonitorHealthChangedDomainEvent(Id, MonitorCode, previousHealth, health));
    }

    public void Enable(string enabledBy)
    {
        if (Status == MonitorStatus.Active)
            return;

        Status = MonitorStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = enabledBy;

        AddDomainEvent(new SystemMonitorEnabledDomainEvent(Id, MonitorCode, enabledBy));
    }

    public void Disable(string disabledBy)
    {
        if (Status == MonitorStatus.Disabled)
            return;

        Status = MonitorStatus.Disabled;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = disabledBy;

        AddDomainEvent(new SystemMonitorDisabledDomainEvent(Id, MonitorCode, disabledBy));
    }

    public void Pause(string pausedBy)
    {
        if (Status == MonitorStatus.Paused)
            return;

        Status = MonitorStatus.Paused;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = pausedBy;

        AddDomainEvent(new SystemMonitorPausedDomainEvent(Id, MonitorCode, pausedBy));
    }

    public void Resume(string resumedBy)
    {
        if (Status != MonitorStatus.Paused)
            return;

        Status = MonitorStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = resumedBy;

        AddDomainEvent(new SystemMonitorResumedDomainEvent(Id, MonitorCode, resumedBy));
    }

    // Query Methods
    public bool IsHealthy()
    {
        return Health == MonitorHealth.Healthy;
    }

    public bool IsActive()
    {
        return Status == MonitorStatus.Active;
    }

    public bool ShouldCheck()
    {
        if (!IsActive())
            return false;

        return DateTime.UtcNow - LastCheckAt >= CheckInterval;
    }

    public decimal GetFailureRate()
    {
        return TotalChecks == 0 ? 0 : (decimal)FailedChecks / TotalChecks * 100;
    }

    public TimeSpan GetTimeSinceLastCheck()
    {
        return DateTime.UtcNow - LastCheckAt;
    }

    public bool HasThreshold(string metric)
    {
        return Thresholds.ContainsKey(metric);
    }

    public decimal? GetThreshold(string metric)
    {
        return Thresholds.TryGetValue(metric, out var value) ? value : null;
    }

    public List<AlertRule> GetActiveAlertRules()
    {
        return AlertRules.Where(r => r.IsActive).ToList();
    }

    public Dictionary<string, object> GetHealthSummary()
    {
        return new Dictionary<string, object>
        {
            ["MonitorCode"] = MonitorCode,
            ["Health"] = Health.ToString(),
            ["Status"] = Status.ToString(),
            ["SuccessRate"] = SuccessRate,
            ["AverageResponseTime"] = AverageResponseTime.TotalMilliseconds,
            ["ConsecutiveFailures"] = ConsecutiveFailures,
            ["LastCheckAt"] = LastCheckAt,
            ["TimeSinceLastCheck"] = GetTimeSinceLastCheck().TotalMinutes
        };
    }

    // Private Methods
    private void UpdatePerformanceMetrics(TimeSpan responseTime)
    {
        // Update success rate
        SuccessRate = TotalChecks == 0 ? 100m : (decimal)SuccessfulChecks / TotalChecks * 100;

        // Update average response time (simple moving average)
        if (TotalChecks == 1)
        {
            AverageResponseTime = responseTime;
        }
        else
        {
            var totalMs = AverageResponseTime.TotalMilliseconds * (TotalChecks - 1) + responseTime.TotalMilliseconds;
            AverageResponseTime = TimeSpan.FromMilliseconds(totalMs / TotalChecks);
        }
    }

    private void CheckThresholdsAndTriggerAlerts(Dictionary<string, object> result)
    {
        foreach (var threshold in Thresholds)
        {
            if (result.TryGetValue(threshold.Key, out var value) && 
                decimal.TryParse(value?.ToString(), out var numericValue))
            {
                if (numericValue > threshold.Value)
                {
                    var alertRule = AlertRules.FirstOrDefault(r => r.Condition.Contains(threshold.Key));
                    var severity = alertRule?.Severity ?? DefaultSeverity;
                    
                    TriggerAlert(severity, 
                        $"Threshold exceeded for {threshold.Key}: {numericValue} > {threshold.Value}",
                        new Dictionary<string, object>
                        {
                            ["Metric"] = threshold.Key,
                            ["Value"] = numericValue,
                            ["Threshold"] = threshold.Value
                        });
                }
            }
        }
    }
}

// Supporting Classes
public class AlertRule
{
    public string Condition { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Message { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public DateTime CreatedAt { get; set; }

    public AlertRule(string condition, AlertSeverity severity, string message)
    {
        Condition = condition;
        Severity = severity;
        Message = message;
        IsActive = true;
        Parameters = new Dictionary<string, object>();
        CreatedAt = DateTime.UtcNow;
    }
}

public class MonitorAlert
{
    public Guid Id { get; set; }
    public Guid MonitorId { get; set; }
    public string MonitorCode { get; set; }
    public AlertSeverity Severity { get; set; }
    public string Message { get; set; }
    public Dictionary<string, object> Context { get; set; }
    public DateTime TriggeredAt { get; set; }
    public AlertStatus Status { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }

    public MonitorAlert()
    {
        Context = new Dictionary<string, object>();
    }
}

// Enumerations
public enum MonitorType
{
    System,
    Application,
    Database,
    Network,
    Security,
    Performance,
    Business
}

public enum MonitorStatus
{
    Active,
    Disabled,
    Paused,
    Error,
    Maintenance
}

public enum MonitorHealth
{
    Healthy,
    Degraded,
    Unhealthy,
    Unknown,
    Maintenance
}

public enum AlertSeverity
{
    Low,
    Medium,
    High,
    Critical,
    Emergency
}

public enum AlertStatus
{
    Active,
    Acknowledged,
    Resolved,
    Suppressed
}