using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Anomaly Aggregate - Detected anomalies in transactions or behaviors
/// Core to fraud detection and monitoring
/// </summary>
public class Anomaly : AggregateRoot
{
    public string AnomalyType { get; private set; } // Behavioral, Transactional, Structural, etc.
    public Guid? CustomerId { get; private set; }
    public Guid? EntityId { get; private set; }
    public string EntityType { get; private set; } // Account, Transaction, Card, etc.
    public decimal SeverityScore { get; private set; } // 0-100
    public string Status { get; private set; } // Detected, Investigated, Resolved, FalsePositive
    
    // Detection Details
    public string DetectionMethod { get; private set; }
    public DateTime DetectedAt { get; private set; }
    public string Description { get; private set; }
    public string Evidence { get; private set; } // JSON: supporting data
    
    // Investigation
    public DateTime? InvestigatedAt { get; private set; }
    public string? InvestigatedBy { get; private set; }
    public string? InvestigationFindings { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }
    public string? Resolution { get; private set; }
    
    // Metadata
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private Anomaly() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static Anomaly Create(
        string anomalyType,
        Guid? customerId,
        Guid? entityId,
        string entityType,
        decimal severityScore,
        string detectionMethod,
        string description)
    {
        var anomaly = new Anomaly
        {
            Id = Guid.NewGuid(),
            AnomalyType = anomalyType,
            CustomerId = customerId,
            EntityId = entityId,
            EntityType = entityType,
            SeverityScore = severityScore,
            DetectionMethod = detectionMethod,
            Description = description,
            Status = "Detected",
            DetectedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Evidence = "{}",
            Metadata = new Dictionary<string, object>()
        };

        return anomaly;
    }

    public void Investigate(string investigatedBy, string findings)
    {
        Status = "Investigated";
        InvestigatedAt = DateTime.UtcNow;
        InvestigatedBy = investigatedBy;
        InvestigationFindings = findings;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolve(string resolvedBy, string resolution)
    {
        Status = "Resolved";
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        Resolution = resolution;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFalsePositive(string resolvedBy, string reason)
    {
        Status = "FalsePositive";
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedBy;
        Resolution = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}
