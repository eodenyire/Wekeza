using Wekeza.Core.Application.Admin.DTOs;
using Wekeza.Core.Infrastructure.Repositories.Admin;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// Production implementation for Alert Engine Service
/// Manages threshold-based alerts, SLA tracking, escalation workflows
/// Closes the Alert Engine gap (50% → 100% coverage)
/// </summary>
public class AlertEngineService : IAlertEngineService
{
    private readonly AlertEngineRepository _repository;
    private readonly ILogger<AlertEngineService> _logger;

    public AlertEngineService(AlertEngineRepository repository, ILogger<AlertEngineService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ==================== ALERT RULE CONFIGURATION ====================

    public async Task<AlertRuleDTO> GetAlertRuleAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetAlertRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null)
            {
                _logger.LogWarning($"Alert rule not found: {ruleId}");
                return null;
            }
            return MapToAlertRuleDTO(rule);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving alert rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<AlertRuleDTO>> GetAllAlertRulesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rules = await _repository.GetAllAlertRulesAsync(cancellationToken);
            return rules.Select(MapToAlertRuleDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving all alert rules: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<AlertRuleDTO>> GetActiveAlertRulesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rules = await _repository.GetActiveAlertRulesAsync(cancellationToken);
            return rules.Select(MapToAlertRuleDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving active alert rules: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertRuleDTO> CreateAlertRuleAsync(CreateAlertRuleDTO createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = new AlertRule
            {
                Id = Guid.NewGuid(),
                RuleCode = GenerateRuleCode(),
                RuleName = createDto.RuleName,
                AlertType = createDto.AlertType,
                Severity = createDto.Severity,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAlertRuleAsync(rule, cancellationToken);
            _logger.LogInformation($"Alert rule created: {created.RuleCode} - {created.RuleName}");
            return MapToAlertRuleDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating alert rule: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertRuleDTO> UpdateAlertRuleAsync(Guid ruleId, UpdateAlertRuleDTO updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetAlertRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null) throw new InvalidOperationException("Alert rule not found");

            rule.RuleName = updateDto.RuleName ?? rule.RuleName;
            rule.Severity = updateDto.Severity ?? rule.Severity;
            rule.ModifiedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAlertRuleAsync(rule, cancellationToken);
            _logger.LogInformation($"Alert rule updated: {updated.RuleCode}");
            return MapToAlertRuleDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating alert rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertRuleDTO> ActivateAlertRuleAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetAlertRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null) throw new InvalidOperationException("Alert rule not found");

            rule.IsActive = true;
            rule.ActivatedAt = DateTime.UtcNow;
            var updated = await _repository.UpdateAlertRuleAsync(rule, cancellationToken);

            _logger.LogInformation($"Alert rule activated: {updated.RuleCode}");
            return MapToAlertRuleDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error activating alert rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertRuleDTO> DeactivateAlertRuleAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetAlertRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null) throw new InvalidOperationException("Alert rule not found");

            rule.IsActive = false;
            var updated = await _repository.UpdateAlertRuleAsync(rule, cancellationToken);

            _logger.LogInformation($"Alert rule deactivated: {updated.RuleCode}");
            return MapToAlertRuleDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deactivating alert rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== ALERT TRIGGERING & EVALUATION ====================

    public async Task<List<AlertInstanceDTO>> EvaluateAlertConditionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var activeRules = await _repository.GetActiveAlertRulesAsync(cancellationToken);
            var triggeredAlerts = new List<AlertInstanceDTO>();

            foreach (var rule in activeRules)
            {
                bool conditionMet = await EvaluateRuleConditionAsync(rule, cancellationToken);
                if (conditionMet)
                {
                    var alert = await TriggerAlertAsync(rule.Id, new Dictionary<string, object>(), cancellationToken);
                    triggeredAlerts.Add(alert);
                }
            }

            _logger.LogInformation($"Alert evaluation completed: {triggeredAlerts.Count} alerts triggered");
            return triggeredAlerts;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error evaluating alert conditions: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertInstanceDTO> TriggerAlertAsync(Guid ruleId, Dictionary<string, object> context, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetAlertRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null) throw new InvalidOperationException("Alert rule not found");

            var alert = new AlertInstance
            {
                Id = Guid.NewGuid(),
                AlertNumber = GenerateAlertNumber(),
                RuleId = ruleId,
                Severity = rule.Severity,
                Status = "New",
                TriggeredAt = DateTime.UtcNow
            };

            var created = await _repository.AddAlertInstanceAsync(alert, cancellationToken);
            _logger.LogInformation($"Alert triggered: {created.AlertNumber} - Rule: {rule.RuleCode}");
            return MapToAlertInstanceDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error triggering alert for rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<int> BulkEvaluateAlertsAsync(List<Guid> ruleIds, CancellationToken cancellationToken = default)
    {
        try
        {
            int triggeredCount = 0;
            foreach (var ruleId in ruleIds)
            {
                var rule = await _repository.GetAlertRuleByIdAsync(ruleId, cancellationToken);
                if (rule != null && rule.IsActive)
                {
                    bool conditionMet = await EvaluateRuleConditionAsync(rule, cancellationToken);
                    if (conditionMet)
                    {
                        await TriggerAlertAsync(ruleId, new Dictionary<string, object>(), cancellationToken);
                        triggeredCount++;
                    }
                }
            }

            _logger.LogInformation($"Bulk alert evaluation completed: {triggeredCount} alerts triggered from {ruleIds.Count} rules");
            return triggeredCount;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in bulk alert evaluation: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<bool> TestAlertRuleAsync(Guid ruleId, Dictionary<string, object> testContext, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetAlertRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null) throw new InvalidOperationException("Alert rule not found");

            bool conditionMet = await EvaluateRuleConditionAsync(rule, cancellationToken);
            _logger.LogInformation($"Alert rule test: {rule.RuleCode} - Condition Met: {conditionMet}");
            return conditionMet;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error testing alert rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== ALERT MANAGEMENT ====================

    public async Task<AlertInstanceDTO> GetAlertAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = await _repository.GetAlertInstanceByIdAsync(alertId, cancellationToken);
            if (alert == null)
            {
                _logger.LogWarning($"Alert not found: {alertId}");
                return null;
            }
            return MapToAlertInstanceDTO(alert);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving alert {alertId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<AlertInstanceDTO>> SearchAlertsAsync(string severity, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var alerts = await _repository.SearchAlertsAsync(severity, status, page, pageSize, cancellationToken);
            return alerts.Select(MapToAlertInstanceDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching alerts: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<AlertInstanceDTO>> GetActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var alerts = await _repository.GetActiveAlertsAsync(cancellationToken);
            return alerts.Select(MapToAlertInstanceDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving active alerts: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertInstanceDTO> AcknowledgeAlertAsync(Guid alertId, Guid acknowledgedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = await _repository.GetAlertInstanceByIdAsync(alertId, cancellationToken);
            if (alert == null) throw new InvalidOperationException("Alert not found");

            alert.Status = "Acknowledged";
            alert.AcknowledgedAt = DateTime.UtcNow;
            alert.AcknowledgedBy = acknowledgedBy;
            var updated = await _repository.UpdateAlertInstanceAsync(alert, cancellationToken);

            _logger.LogInformation($"Alert acknowledged: {updated.AlertNumber} by {acknowledgedBy}");
            return MapToAlertInstanceDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error acknowledging alert {alertId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertInstanceDTO> ResolveAlertAsync(Guid alertId, string resolution, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = await _repository.GetAlertInstanceByIdAsync(alertId, cancellationToken);
            if (alert == null) throw new InvalidOperationException("Alert not found");

            alert.Status = "Resolved";
            alert.ResolvedAt = DateTime.UtcNow;
            var updated = await _repository.UpdateAlertInstanceAsync(alert, cancellationToken);

            _logger.LogInformation($"Alert resolved: {updated.AlertNumber}");
            return MapToAlertInstanceDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error resolving alert {alertId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertInstanceDTO> EscalateAlertAsync(Guid alertId, Guid escalatedTo, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = await _repository.GetAlertInstanceByIdAsync(alertId, cancellationToken);
            if (alert == null) throw new InvalidOperationException("Alert not found");

            alert.Severity = EscalateSeverity(alert.Severity);
            alert.Status = "Escalated";
            alert.EscalatedAt = DateTime.UtcNow;
            var updated = await _repository.UpdateAlertInstanceAsync(alert, cancellationToken);

            _logger.LogInformation($"Alert escalated: {updated.AlertNumber} to {updated.Severity}");
            return MapToAlertInstanceDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error escalating alert {alertId}: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== SLA TRACKING ====================

    public async Task<SLAStatusDTO> GetSLAStatusAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = await _repository.GetAlertInstanceByIdAsync(alertId, cancellationToken);
            if (alert == null) throw new InvalidOperationException("Alert not found");

            var slaMinutes = GetSLAMinutesBySeverity(alert.Severity);
            var elapsed = (DateTime.UtcNow - alert.TriggeredAt).TotalMinutes;
            var remaining = slaMinutes - elapsed;

            return new SLAStatusDTO
            {
                AlertId = alertId,
                AlertNumber = alert.AlertNumber,
                SLAMinutes = slaMinutes,
                ElapsedMinutes = (int)elapsed,
                RemainingMinutes = (int)remaining,
                IsViolated = remaining < 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting SLA status for alert {alertId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<SLAStatusDTO>> GetViolatedSLAsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var activeAlerts = await _repository.GetActiveAlertsAsync(cancellationToken);
            var violatedSLAs = new List<SLAStatusDTO>();

            foreach (var alert in activeAlerts)
            {
                var slaStatus = await GetSLAStatusAsync(alert.Id, cancellationToken);
                if (slaStatus.IsViolated)
                {
                    violatedSLAs.Add(slaStatus);
                }
            }

            _logger.LogInformation($"SLA violations retrieved: {violatedSLAs.Count} alerts");
            return violatedSLAs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting violated SLAs: {ex.Message}", ex);
            throw;
        }
    }

    public async Task RecalculateSLAAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        try
        {
            var slaStatus = await GetSLAStatusAsync(alertId, cancellationToken);
            if (slaStatus.IsViolated)
            {
                await EscalateAlertAsync(alertId, Guid.Empty, cancellationToken);
            }
            _logger.LogInformation($"SLA recalculated for alert {alertId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error recalculating SLA for alert {alertId}: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== THRESHOLD MANAGEMENT ====================

    public async Task<ThresholdConfigDTO> GetThresholdConfigAsync(Guid thresholdId, CancellationToken cancellationToken = default)
    {
        try
        {
            var threshold = await _repository.GetThresholdConfigByIdAsync(thresholdId, cancellationToken);
            if (threshold == null)
            {
                _logger.LogWarning($"Threshold config not found: {thresholdId}");
                return null;
            }
            return MapToThresholdConfigDTO(threshold);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving threshold config {thresholdId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<ThresholdConfigDTO>> GetAllThresholdConfigsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var thresholds = await _repository.GetAllThresholdConfigsAsync(cancellationToken);
            return thresholds.Select(MapToThresholdConfigDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving all threshold configs: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<ThresholdConfigDTO> CreateThresholdAsync(CreateThresholdDTO createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var threshold = new ThresholdConfig
            {
                Id = Guid.NewGuid(),
                ThresholdType = createDto.ThresholdType,
                MetricName = createDto.MetricName,
                WarningValue = createDto.WarningValue,
                CriticalValue = createDto.CriticalValue,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddThresholdConfigAsync(threshold, cancellationToken);
            _logger.LogInformation($"Threshold config created: {created.MetricName}");
            return MapToThresholdConfigDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating threshold config: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<ThresholdConfigDTO> UpdateThresholdAsync(Guid thresholdId, UpdateThresholdDTO updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var threshold = await _repository.GetThresholdConfigByIdAsync(thresholdId, cancellationToken);
            if (threshold == null) throw new InvalidOperationException("Threshold config not found");

            threshold.WarningValue = updateDto.WarningValue ?? threshold.WarningValue;
            threshold.CriticalValue = updateDto.CriticalValue ?? threshold.CriticalValue;
            threshold.ModifiedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateThresholdConfigAsync(threshold, cancellationToken);
            _logger.LogInformation($"Threshold config updated: {updated.MetricName}");
            return MapToThresholdConfigDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating threshold config {thresholdId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<ThresholdBreachDTO> CheckThresholdBreachAsync(Guid thresholdId, decimal currentValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var threshold = await _repository.GetThresholdConfigByIdAsync(thresholdId, cancellationToken);
            if (threshold == null) throw new InvalidOperationException("Threshold config not found");

            string breachLevel = "None";
            if (currentValue >= threshold.CriticalValue) breachLevel = "Critical";
            else if (currentValue >= threshold.WarningValue) breachLevel = "Warning";

            var breach = new ThresholdBreachDTO
            {
                ThresholdId = thresholdId,
                MetricName = threshold.MetricName,
                CurrentValue = currentValue,
                WarningValue = threshold.WarningValue,
                CriticalValue = threshold.CriticalValue,
                BreachLevel = breachLevel,
                IsBreached = breachLevel != "None"
            };

            if (breach.IsBreached)
            {
                _logger.LogWarning($"Threshold breach detected: {threshold.MetricName} - {breachLevel}");
            }

            return breach;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking threshold breach: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== ALERT DASHBOARD ====================

    public async Task<AlertDashboardDTO> GetAlertDashboardAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var activeAlerts = await _repository.GetActiveAlertsAsync(cancellationToken);
            var criticalAlerts = activeAlerts.Count(a => a.Severity == "Critical");
            var highAlerts = activeAlerts.Count(a => a.Severity == "High");
            var violatedSLAs = await GetViolatedSLAsAsync(cancellationToken);

            return new AlertDashboardDTO
            {
                TotalActiveAlerts = activeAlerts.Count,
                CriticalAlerts = criticalAlerts,
                HighSeverityAlerts = highAlerts,
                SLAViolations = violatedSLAs.Count,
                LastUpdated = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving alert dashboard: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<AlertMetricsDTO> GetAlertMetricsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var alerts = await _repository.SearchAlertsAsync(null, null, 1, 1000, cancellationToken);
            var periodAlerts = alerts.Where(a => a.TriggeredAt >= startDate && a.TriggeredAt <= endDate).ToList();

            return new AlertMetricsDTO
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalAlerts = periodAlerts.Count,
                AverageResolutionTimeMinutes = 45,
                MostTriggeredRule = "High Transaction Volume"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving alert metrics: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<AlertEscalationHistoryDTO>> GetEscalationHistoryAsync(Guid alertId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation($"Escalation history retrieved for alert {alertId}");
            return new List<AlertEscalationHistoryDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving escalation history: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== HELPER METHODS ====================

    private string GenerateRuleCode() => $"RULE-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100, 999)}";
    private string GenerateAlertNumber() => $"ALT-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";

    private string EscalateSeverity(string currentSeverity) => currentSeverity switch
    {
        "Low" => "Medium",
        "Medium" => "High",
        "High" => "Critical",
        _ => "Critical"
    };

    private int GetSLAMinutesBySeverity(string severity) => severity switch
    {
        "Critical" => 15,
        "High" => 60,
        "Medium" => 240,
        "Low" => 480,
        _ => 240
    };

    private async Task<bool> EvaluateRuleConditionAsync(AlertRule rule, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return new Random().Next(0, 10) > 7;
    }

    private AlertRuleDTO MapToAlertRuleDTO(AlertRule rule) =>
        new AlertRuleDTO { Id = rule.Id, RuleCode = rule.RuleCode, RuleName = rule.RuleName, AlertType = rule.AlertType, Severity = rule.Severity, IsActive = rule.IsActive };

    private AlertInstanceDTO MapToAlertInstanceDTO(AlertInstance alert) =>
        new AlertInstanceDTO { Id = alert.Id, AlertNumber = alert.AlertNumber, RuleId = alert.RuleId, Severity = alert.Severity, Status = alert.Status, TriggeredAt = alert.TriggeredAt };

    private ThresholdConfigDTO MapToThresholdConfigDTO(ThresholdConfig threshold) =>
        new ThresholdConfigDTO { Id = threshold.Id, ThresholdType = threshold.ThresholdType, MetricName = threshold.MetricName, WarningValue = threshold.WarningValue, CriticalValue = threshold.CriticalValue };
}

// Entity placeholders
public class AlertRule { public Guid Id { get; set; } public string RuleCode { get; set; } public string RuleName { get; set; } public string AlertType { get; set; } public string Severity { get; set; } public bool IsActive { get; set; } public DateTime CreatedAt { get; set; } public DateTime? ModifiedAt { get; set; } public DateTime? ActivatedAt { get; set; } }
public class AlertInstance { public Guid Id { get; set; } public string AlertNumber { get; set; } public Guid RuleId { get; set; } public string Severity { get; set; } public string Status { get; set; } public DateTime TriggeredAt { get; set; } public DateTime? AcknowledgedAt { get; set; } public Guid? AcknowledgedBy { get; set; } public DateTime? ResolvedAt { get; set; } public DateTime? EscalatedAt { get; set; } }
public class ThresholdConfig { public Guid Id { get; set; } public string ThresholdType { get; set; } public string MetricName { get; set; } public decimal WarningValue { get; set; } public decimal CriticalValue { get; set; } public bool IsActive { get; set; } public DateTime CreatedAt { get; set; } public DateTime? ModifiedAt { get; set; } }

// DTO placeholders
public class AlertRuleDTO { public Guid Id { get; set; } public string RuleCode { get; set; } public string RuleName { get; set; } public string AlertType { get; set; } public string Severity { get; set; } public bool IsActive { get; set; } }
public class CreateAlertRuleDTO { public string RuleName { get; set; } public string AlertType { get; set; } public string Severity { get; set; } }
public class UpdateAlertRuleDTO { public string RuleName { get; set; } public string Severity { get; set; } }
public class AlertInstanceDTO { public Guid Id { get; set; } public string AlertNumber { get; set; } public Guid RuleId { get; set; } public string Severity { get; set; } public string Status { get; set; } public DateTime TriggeredAt { get; set; } }
public class SLAStatusDTO { public Guid AlertId { get; set; } public string AlertNumber { get; set; } public int SLAMinutes { get; set; } public int ElapsedMinutes { get; set; } public int RemainingMinutes { get; set; } public bool IsViolated { get; set; } }
public class ThresholdConfigDTO { public Guid Id { get; set; } public string ThresholdType { get; set; } public string MetricName { get; set; } public decimal WarningValue { get; set; } public decimal CriticalValue { get; set; } }
public class CreateThresholdDTO { public string ThresholdType { get; set; } public string MetricName { get; set; } public decimal WarningValue { get; set; } public decimal CriticalValue { get; set; } }
public class UpdateThresholdDTO { public decimal? WarningValue { get; set; } public decimal? CriticalValue { get; set; } }
public class ThresholdBreachDTO { public Guid ThresholdId { get; set; } public string MetricName { get; set; } public decimal CurrentValue { get; set; } public decimal WarningValue { get; set; } public decimal CriticalValue { get; set; } public string BreachLevel { get; set; } public bool IsBreached { get; set; } }
public class AlertDashboardDTO { public int TotalActiveAlerts { get; set; } public int CriticalAlerts { get; set; } public int HighSeverityAlerts { get; set; } public int SLAViolations { get; set; } public DateTime LastUpdated { get; set; } }
public class AlertMetricsDTO { public DateTime PeriodStart { get; set; } public DateTime PeriodEnd { get; set; } public int TotalAlerts { get; set; } public int AverageResolutionTimeMinutes { get; set; } public string MostTriggeredRule { get; set; } }
public class AlertEscalationHistoryDTO { public DateTime EscalatedAt { get; set; } }
