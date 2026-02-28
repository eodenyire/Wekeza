using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Anomaly Rule Aggregate - Rules for detecting anomalies
/// Defines thresholds and patterns for automated anomaly detection
/// </summary>
public class AnomalyRule : AggregateRoot
{
    public string RuleName { get; private set; }
    public string RuleCode { get; private set; }
    public string AnomalyType { get; private set; } // Behavioral, Transactional, etc.
    public string EntityType { get; private set; } // Account, Transaction, Card, Customer
    public string Status { get; private set; } // Active, Inactive, Archived
    public int Priority { get; private set; }
    public bool IsEnabled { get; private set; }
    
    // Detection Configuration
    public string RuleLogic { get; private set; } // JSON: condition definitions
    public string ThresholdDefinition { get; private set; } // JSON: thresholds
    public int SeverityLevel { get; private set; } // 1-5
    public int TimeWindowMinutes { get; private set; } // Detection window
    
    // Metadata
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public DateTime? LastTriggeredAt { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private AnomalyRule() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static AnomalyRule Create(
        string ruleName,
        string ruleCode,
        string anomalyType,
        string entityType,
        int severityLevel,
        string createdBy)
    {
        var rule = new AnomalyRule
        {
            Id = Guid.NewGuid(),
            RuleName = ruleName,
            RuleCode = ruleCode,
            AnomalyType = anomalyType,
            EntityType = entityType,
            SeverityLevel = severityLevel,
            Status = "Active",
            IsEnabled = true,
            Priority = 10,
            TimeWindowMinutes = 60,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Description = string.Empty,
            RuleLogic = "{}",
            ThresholdDefinition = "{}",
            Metadata = new Dictionary<string, object>()
        };

        return rule;
    }

    public void Enable(string updatedBy)
    {
        IsEnabled = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Disable(string disabledBy)
    {
        IsEnabled = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = disabledBy;
    }

    public void UpdateThresholds(string thresholdDefinition, string updatedBy)
    {
        ThresholdDefinition = thresholdDefinition;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void RecordTrigger()
    {
        LastTriggeredAt = DateTime.UtcNow;
    }
}
