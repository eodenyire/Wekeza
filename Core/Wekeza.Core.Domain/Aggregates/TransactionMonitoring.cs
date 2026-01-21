using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class TransactionMonitoring : AggregateRoot
{
    public Guid TransactionId { get; private set; }
    public List<string> AppliedRules { get; private set; }
    public ScreeningResult Result { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public MonitoringStatus Status { get; private set; }
    public RiskScore? RiskScore { get; private set; }
    public string? ReviewNotes { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DateTime? ReviewedDate { get; private set; }
    public List<MonitoringAlert> Alerts { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private TransactionMonitoring() : base(Guid.NewGuid()) {
        AppliedRules = new List<string>();
        Alerts = new List<MonitoringAlert>();
    }

    public static TransactionMonitoring Create(
        Guid transactionId,
        List<string> appliedRules,
        ScreeningResult result,
        AlertSeverity severity,
        RiskScore? riskScore = null)
    {
        if (transactionId == Guid.Empty)
            throw new ArgumentException("Transaction ID cannot be empty", nameof(transactionId));

        if (appliedRules == null || !appliedRules.Any())
            throw new ArgumentException("At least one monitoring rule must be applied", nameof(appliedRules));

        var monitoring = new TransactionMonitoring
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            AppliedRules = appliedRules.ToList(),
            Result = result,
            Severity = severity,
            Status = result == ScreeningResult.Clear ? MonitoringStatus.Cleared : MonitoringStatus.Pending,
            RiskScore = riskScore,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Generate alerts based on screening result
        if (result != ScreeningResult.Clear)
        {
            monitoring.GenerateAlerts();
        }

        monitoring.AddDomainEvent(new TransactionMonitoringCompletedDomainEvent(
            monitoring.Id, transactionId, result, severity));

        return monitoring;
    }

    public void Review(string reviewedBy, string reviewNotes, MonitoringDecision decision)
    {
        if (Status == MonitoringStatus.Cleared)
            throw new InvalidOperationException("Cannot review already cleared transaction");

        if (string.IsNullOrWhiteSpace(reviewedBy))
            throw new ArgumentException("Reviewer cannot be empty", nameof(reviewedBy));

        ReviewedBy = reviewedBy;
        ReviewNotes = reviewNotes;
        ReviewedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Status = decision switch
        {
            MonitoringDecision.Allow => MonitoringStatus.Cleared,
            MonitoringDecision.Block => MonitoringStatus.Blocked,
            MonitoringDecision.Escalate => MonitoringStatus.Escalated,
            MonitoringDecision.Review => MonitoringStatus.PendingInfo,
            _ => MonitoringStatus.Pending
        };

        AddDomainEvent(new TransactionMonitoringReviewedDomainEvent(
            Id, TransactionId, decision, reviewedBy));

        // Create AML case if escalated
        if (decision == MonitoringDecision.Escalate)
        {
            AddDomainEvent(new AMLCaseRequiredDomainEvent(
                TransactionId, null, AMLAlertType.SuspiciousTransaction, RiskScore ?? ValueObjects.RiskScore.ForTransaction(0, "UNKNOWN")));
        }
    }

    public void AddAlert(string alertType, string description, AlertSeverity severity)
    {
        var alert = new MonitoringAlert
        {
            Id = Guid.NewGuid(),
            TransactionMonitoringId = Id,
            AlertType = alertType,
            Description = description,
            Severity = severity,
            CreatedAt = DateTime.UtcNow
        };

        Alerts.Add(alert);
        
        // Update overall severity if this alert is more severe
        if (severity > Severity)
        {
            Severity = severity;
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MonitoringAlertGeneratedDomainEvent(
            Id, TransactionId, alertType, severity));
    }

    public void UpdateRiskScore(RiskScore newRiskScore, string reason)
    {
        var oldRiskScore = RiskScore;
        RiskScore = newRiskScore;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TransactionRiskScoreUpdatedDomainEvent(
            Id, TransactionId, oldRiskScore, newRiskScore, reason));
    }

    public void Block(string blockedBy, string reason)
    {
        if (Status == MonitoringStatus.Cleared)
            throw new InvalidOperationException("Cannot block already cleared transaction");

        Status = MonitoringStatus.Blocked;
        ReviewedBy = blockedBy;
        ReviewNotes = reason;
        ReviewedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TransactionBlockedDomainEvent(Id, TransactionId, reason));
    }

    public void Clear(string clearedBy, string reason = "")
    {
        if (Status == MonitoringStatus.Blocked)
            throw new InvalidOperationException("Cannot clear blocked transaction without proper review");

        Status = MonitoringStatus.Cleared;
        ReviewedBy = clearedBy;
        ReviewNotes = reason;
        ReviewedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TransactionClearedDomainEvent(Id, TransactionId, reason));
    }

    private void GenerateAlerts()
    {
        foreach (var rule in AppliedRules)
        {
            var alertSeverity = DetermineAlertSeverity(rule);
            var description = GenerateAlertDescription(rule);
            
            AddAlert(rule, description, alertSeverity);
        }
    }

    private AlertSeverity DetermineAlertSeverity(string rule)
    {
        return rule.ToUpper() switch
        {
            var r when r.Contains("SANCTIONS") => AlertSeverity.Critical,
            var r when r.Contains("STRUCTURING") => AlertSeverity.High,
            var r when r.Contains("PEP") => AlertSeverity.High,
            var r when r.Contains("CASH") => AlertSeverity.Medium,
            var r when r.Contains("THRESHOLD") => AlertSeverity.Medium,
            var r when r.Contains("VELOCITY") => AlertSeverity.Medium,
            _ => AlertSeverity.Low
        };
    }

    private string GenerateAlertDescription(string rule)
    {
        return rule.ToUpper() switch
        {
            var r when r.Contains("SANCTIONS") => "Potential sanctions match detected",
            var r when r.Contains("STRUCTURING") => "Possible structuring activity identified",
            var r when r.Contains("PEP") => "Transaction involves Politically Exposed Person",
            var r when r.Contains("CASH") => "Large cash transaction detected",
            var r when r.Contains("THRESHOLD") => "Reporting threshold exceeded",
            var r when r.Contains("VELOCITY") => "High transaction velocity detected",
            var r when r.Contains("GEOGRAPHIC") => "Unusual geographic pattern",
            var r when r.Contains("TIME") => "Unusual timing pattern",
            _ => $"Monitoring rule triggered: {rule}"
        };
    }

    public bool IsPending => Status == MonitoringStatus.Pending;
    public bool IsCleared => Status == MonitoringStatus.Cleared;
    public bool IsBlocked => Status == MonitoringStatus.Blocked;
    public bool IsEscalated => Status == MonitoringStatus.Escalated;
    public bool RequiresReview => Status == MonitoringStatus.Pending || Status == MonitoringStatus.PendingInfo;
    public bool IsHighSeverity => Severity >= AlertSeverity.High;
    public int AlertCount => Alerts.Count;
    public int DaysPending => Status == MonitoringStatus.Pending ? (DateTime.UtcNow - CreatedAt).Days : 0;
}

public class MonitoringAlert
{
    public Guid Id { get; set; }
    public Guid TransactionMonitoringId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum MonitoringStatus
{
    Pending,
    Cleared,
    Blocked,
    Escalated,
    PendingInfo
}





