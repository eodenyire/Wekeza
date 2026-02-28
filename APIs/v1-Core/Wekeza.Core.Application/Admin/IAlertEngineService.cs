using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Alert Engine Service Interface
/// Manages threshold-based alerts, SLA tracking, and escalation workflows
/// </summary>
public interface IAlertEngineService
{
    // ==================== ALERT RULE MANAGEMENT ====================
    Task<AlertRuleDTO> GetAlertRuleAsync(Guid ruleId, CancellationToken cancellationToken = default);
    Task<List<AlertRuleDTO>> GetAllAlertRulesAsync(CancellationToken cancellationToken = default);
    Task<List<AlertRuleDTO>> GetActiveAlertRulesAsync(CancellationToken cancellationToken = default);
    Task<AlertRuleDTO> CreateAlertRuleAsync(CreateAlertRuleDTO createDto, CancellationToken cancellationToken = default);
    Task<AlertRuleDTO> UpdateAlertRuleAsync(Guid ruleId, UpdateAlertRuleDTO updateDto, CancellationToken cancellationToken = default);
    Task<AlertRuleDTO> ActivateAlertRuleAsync(Guid ruleId, CancellationToken cancellationToken = default);
    Task<AlertRuleDTO> DeactivateAlertRuleAsync(Guid ruleId, CancellationToken cancellationToken = default);
    
    // ==================== ALERT EVALUATION & TRIGGERING ====================
    Task<List<AlertInstanceDTO>> EvaluateAlertConditionsAsync(CancellationToken cancellationToken = default);
    Task<AlertInstanceDTO> TriggerAlertAsync(Guid ruleId, Dictionary<string, object> context, CancellationToken cancellationToken = default);
    Task<int> BulkEvaluateAlertsAsync(List<Guid> ruleIds, CancellationToken cancellationToken = default);
    Task<bool> TestAlertRuleAsync(Guid ruleId, Dictionary<string, object> testContext, CancellationToken cancellationToken = default);
    
    // ==================== ALERT INSTANCE MANAGEMENT ====================
    Task<AlertInstanceDTO> GetAlertAsync(Guid alertId, CancellationToken cancellationToken = default);
    Task<List<AlertInstanceDTO>> SearchAlertsAsync(string severity, string status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<List<AlertInstanceDTO>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
    Task<AlertInstanceDTO> AcknowledgeAlertAsync(Guid alertId, Guid acknowledgedBy, CancellationToken cancellationToken = default);
    Task<AlertInstanceDTO> ResolveAlertAsync(Guid alertId, string resolution, CancellationToken cancellationToken = default);
    Task<AlertInstanceDTO> EscalateAlertAsync(Guid alertId, Guid escalatedTo, CancellationToken cancellationToken = default);
    
    // ==================== SLA MANAGEMENT ====================
    Task<SLAStatusDTO> GetSLAStatusAsync(Guid alertId, CancellationToken cancellationToken = default);
    Task<List<SLAStatusDTO>> GetViolatedSLAsAsync(CancellationToken cancellationToken = default);
    Task RecalculateSLAAsync(Guid alertId, CancellationToken cancellationToken = default);
    
    // ==================== THRESHOLD MANAGEMENT ====================
    Task<ThresholdConfigDTO> GetThresholdAsync(Guid thresholdId, CancellationToken cancellationToken = default);
    Task<List<ThresholdConfigDTO>> GetAllThresholdsAsync(CancellationToken cancellationToken = default);
    Task<ThresholdConfigDTO> CreateThresholdAsync(CreateThresholdDTO createDto, CancellationToken cancellationToken = default);
    Task<ThresholdConfigDTO> UpdateThresholdAsync(Guid thresholdId, UpdateThresholdDTO updateDto, CancellationToken cancellationToken = default);
    Task<List<ThresholdBreachDTO>> GetThresholdBreachesAsync(Guid thresholdId, CancellationToken cancellationToken = default);
    
    // ==================== ESCALATION MANAGEMENT ====================
    Task<AlertEscalationHistoryDTO> GetEscalationHistoryAsync(Guid alertId, CancellationToken cancellationToken = default);
    Task<List<AlertEscalationHistoryDTO>> GetEscalationsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    
    // ==================== ANALYTICS & REPORTING ====================
    Task<AlertDashboardDTO> GetAlertDashboardAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<AlertMetricsDTO> GetAlertMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
}

// ==================== DTOs ====================

public class AlertRuleDTO
{
    public Guid Id { get; set; }
    public string RuleCode { get; set; }
    public string RuleName { get; set; }
    public string Severity { get; set; }
    public string Status { get; set; }
    public string Condition { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAlertRuleDTO
{
    public string RuleCode { get; set; }
    public string RuleName { get; set; }
    public string Severity { get; set; }
    public string Condition { get; set; }
    public int EscalationMinutes { get; set; }
}

public class UpdateAlertRuleDTO
{
    public string RuleName { get; set; }
    public string Status { get; set; }
    public string Condition { get; set; }
}

public class AlertInstanceDTO
{
    public Guid Id { get; set; }
    public Guid RuleId { get; set; }
    public string Status { get; set; }
    public string Severity { get; set; }
    public DateTime TriggeredAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class SLAStatusDTO
{
    public Guid AlertId { get; set; }
    public int ResponseTimeMinutes { get; set; }
    public int SLATargetMinutes { get; set; }
    public bool IsBreach { get; set; }
    public DateTime CheckedAt { get; set; }
}

public class ThresholdConfigDTO
{
    public Guid Id { get; set; }
    public string ThresholdCode { get; set; }
    public string ThresholdType { get; set; }
    public decimal ThresholdValue { get; set; }
    public decimal WarningLevel { get; set; }
    public decimal CriticalLevel { get; set; }
}

public class CreateThresholdDTO
{
    public string ThresholdCode { get; set; }
    public string ThresholdType { get; set; }
    public decimal ThresholdValue { get; set; }
    public decimal WarningLevel { get; set; }
    public decimal CriticalLevel { get; set; }
}

public class UpdateThresholdDTO
{
    public decimal? ThresholdValue { get; set; }
    public decimal? WarningLevel { get; set; }
    public decimal? CriticalLevel { get; set; }
}

public class ThresholdBreachDTO
{
    public Guid ThresholdId { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal ThresholdValue { get; set; }
    public string BreachType { get; set; }
    public DateTime CheckedAt { get; set; }
}

public class AlertEscalationHistoryDTO
{
    public Guid AlertId { get; set; }
    public int Level { get; set; }
    public Guid EscalatedTo { get; set; }
    public DateTime EscalatedAt { get; set; }
    public string Reason { get; set; }
}

public class AlertDashboardDTO
{
    public int ActiveAlerts { get; set; }
    public int AcknowledgedAlerts { get; set; }
    public int ResolvedAlerts { get; set; }
    public int SLABreachers { get; set; }
    public Dictionary<string, int> AlertsBySeverity { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class AlertMetricsDTO
{
    public decimal AverageResponseTime { get; set; }
    public decimal SLACompliancePercentage { get; set; }
    public int TotalAlertsTriggered { get; set; }
    public int TotalAlertsResolved { get; set; }
    public decimal EscalationRate { get; set; }
}
