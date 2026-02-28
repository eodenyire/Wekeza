using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Risk Management Service - Limits, Thresholds, Anomaly Detection, Exposure Management
/// Risk management portal for controlling exposure, setting limits, detecting anomalies, and managing risk policies
/// </summary>
public interface IRiskManagementService
{
    // ===== Limits Management =====
    Task<LimitDefinitionDTO> GetLimitAsync(Guid limitId);
    Task<List<LimitDefinitionDTO>> SearchLimitsAsync(string? limitType = null, string? status = null, int page = 1, int pageSize = 50);
    Task<LimitDefinitionDTO> CreateLimitAsync(CreateLimitRequest request, Guid createdByUserId);
    Task<LimitDefinitionDTO> UpdateLimitAsync(Guid limitId, UpdateLimitRequest request, Guid updatedByUserId);
    Task ApproveLimitAsync(Guid limitId, Guid approverUserId);
    Task RevokeLimitAsync(Guid limitId, string reason, Guid revokedByUserId);
    Task<HierarchicalLimitDTO> GetHierarchicalLimitsAsync(string entityType, string entityId);

    // ===== Threshold Configuration =====
    Task<ThresholdConfigDTO> GetThresholdAsync(Guid thresholdId);
    Task<List<ThresholdConfigDTO>> SearchThresholdsAsync(string? thresholdType = null, int page = 1, int pageSize = 50);
    Task<ThresholdConfigDTO> CreateThresholdAsync(CreateThresholdRequest request, Guid createdByUserId);
    Task<ThresholdConfigDTO> UpdateThresholdAsync(Guid thresholdId, UpdateThresholdRequest request, Guid updatedByUserId);
    Task ActivateThresholdAsync(Guid thresholdId, Guid activatedByUserId);
    Task<ThresholdBreachDTO> CheckThresholdBreachAsync(string entityType, string entityId, string metricCode);

    // ===== Limit Utilization Tracking =====
    Task<LimitUtilizationDTO> GetLimitUtilizationAsync(string entityId, string limitType);
    Task<List<LimitUtilizationDTO>> GetAllLimitUtilizationsAsync(string entityId);
    Task<List<LimitApproachingDTO>> GetApproachingLimitsAsync(decimal warningThreshold = 80);
    Task<List<LimitBreachedDTO>> GetBreachedLimitsAsync();
    Task<LimitHistoryDTO> GetLimitUsageHistoryAsync(string entityId, string limitType, DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Anomaly Detection =====
    Task<AnomalyDetectionResultDTO> DetectAnomaliesAsync(AnomalyDetectionRequest request);
    Task<List<DetectedAnomalyDTO>> SearchAnomaliesAsync(string? severity = null, string? status = null, int page = 1, int pageSize = 50);
    Task InvestigateAnomalyAsync(Guid anomalyId, string investigationNotes, Guid investigatedByUserId);
    Task ResolveAnomalyAsync(Guid anomalyId, string resolution, Guid resolvedByUserId);
    Task ConfigureAnomalyRuleAsync(CreateAnomalyRuleRequest request, Guid createdByUserId);
    Task<List<AnomalyRuleDTO>> GetAnomalyRulesAsync();

    // ===== Exposure Management =====
    Task<ExposureAnalysisDTO> AnalyzeExposureAsync(string entityType, string entityId);
    Task<PartyExposureDTO> GetPartyExposureAsync(Guid partyId);
    Task<GeographicExposureDTO> GetGeographicExposureAsync();
    Task<SectorExposureDTO> GetSectorExposureAsync();
    Task<ConcentrationRiskDTO> AnalyzeConcentrationRiskAsync(string concentrationType);

    // ===== Regulatory Limits =====
    Task<RegulatoryLimitDTO> GetRegulatoryLimitAsync(Guid limitId);
    Task<List<RegulatoryLimitDTO>> GetApplicableRegulatoryLimitsAsync();
    Task<RegulatoryComplianceDTO> CheckRegulatoryComplianceAsync(DateTime? asOfDate = null);
    Task<RegulatoryReportDTO> GenerateRegulatoryReportAsync(DateTime fromDate, DateTime toDate, Guid generatedByUserId);

    // ===== Escalation & Approval Workflows =====
    Task<EscalationConfigDTO> GetEscalationConfigAsync(Guid configId);
    Task<EscalationConfigDTO> CreateEscalationConfigAsync(CreateEscalationConfigRequest request, Guid createdByUserId);
    Task TriggerManualEscalationAsync(Guid limitId, string reason, Guid triggeredByUserId);
    Task<List<PendingApprovalDTO>> GetPendingLimitApprovalsAsync();
    Task ApproveLimitOverrideAsync(Guid overrideId, Guid approverUserId);
    Task RejectLimitOverrideAsync(Guid overrideId, string reason, Guid rejectorUserId);

    // ===== Risk Dashboard =====
    Task<RiskDashboardDTO> GetRiskDashboardAsync(DateTime? asOfDate = null);
    Task<RiskScoreDTO> CalculateRiskScoreAsync(string entityType, string entityId);
    Task<List<RiskAlertDTO>> GetRiskAlertsAsync();
    Task<ComplianceMetricsDTO> GetComplianceMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null);
}

// DTOs
public class LimitDefinitionDTO
{
    public Guid Id { get; set; }
    public string LimitType { get; set; }
    public string EntityType { get; set; }
    public decimal LimitAmount { get; set; }
    public string CurrencyCode { get; set; }
    public string Status { get; set; }
    public string LimitHierarchy { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLimitRequest
{
    public string LimitType { get; set; }
    public string EntityType { get; set; }
    public decimal LimitAmount { get; set; }
    public string CurrencyCode { get; set; }
    public string LimitHierarchy { get; set; }
    public Dictionary<string, object> LimitRules { get; set; }
}

public class UpdateLimitRequest
{
    public decimal LimitAmount { get; set; }
    public Dictionary<string, object> LimitRules { get; set; }
}

public class HierarchicalLimitDTO
{
    public string EntityId { get; set; }
    public List<LimitLevelDTO> LimitLevels { get; set; }
    public decimal TotalUtilization { get; set; }
}

public class LimitLevelDTO
{
    public int Level { get; set; }
    public string EntityType { get; set; }
    public decimal LimitAmount { get; set; }
    public decimal UtilizedAmount { get; set; }
    public decimal AvailableAmount { get; set; }
}

public class ThresholdConfigDTO
{
    public Guid Id { get; set; }
    public string ThresholdType { get; set; }
    public string MetricCode { get; set; }
    public decimal WarningLevel { get; set; }
    public decimal CriticalLevel { get; set; }
    public string Status { get; set; }
}

public class CreateThresholdRequest
{
    public string ThresholdType { get; set; }
    public string MetricCode { get; set; }
    public decimal WarningLevel { get; set; }
    public decimal CriticalLevel { get; set; }
}

public class UpdateThresholdRequest
{
    public decimal WarningLevel { get; set; }
    public decimal CriticalLevel { get; set; }
}

public class ThresholdBreachDTO
{
    public Guid ThresholdId { get; set; }
    public string ThresholdType { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal ThresholdValue { get; set; }
    public string BreachLevel { get; set; }
    public DateTime BreachedAt { get; set; }
}

public class LimitUtilizationDTO
{
    public string EntityId { get; set; }
    public string LimitType { get; set; }
    public decimal LimitAmount { get; set; }
    public decimal UtilizedAmount { get; set; }
    public decimal AvailableAmount { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

public class LimitApproachingDTO
{
    public Guid LimitId { get; set; }
    public string EntityId { get; set; }
    public string LimitType { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public string Alert { get; set; }
}

public class LimitBreachedDTO
{
    public Guid LimitId { get; set; }
    public string EntityId { get; set; }
    public string LimitType { get; set; }
    public decimal ExcessAmount { get; set; }
    public DateTime BreachedAt { get; set; }
    public string Status { get; set; }
}

public class LimitHistoryDTO
{
    public string EntityId { get; set; }
    public string LimitType { get; set; }
    public List<LimitHistoryEntryDTO> Entries { get; set; }
}

public class LimitHistoryEntryDTO
{
    public DateTime RecordedAt { get; set; }
    public decimal LimitAmount { get; set; }
    public decimal UtilizedAmount { get; set; }
    public decimal AvailableAmount { get; set; }
}

public class AnomalyDetectionResultDTO
{
    public Guid DetectionId { get; set; }
    public List<DetectedAnomalyDTO> AnomaliesDetected { get; set; }
    public int TotalAnomalies { get; set; }
    public DateTime DetectedAt { get; set; }
}

public class DetectedAnomalyDTO
{
    public Guid AnomalyId { get; set; }
    public string AnomalyType { get; set; }
    public string Description { get; set; }
    public string Severity { get; set; }
    public double AnomalyScore { get; set; }
    public string Status { get; set; }
    public DateTime DetectedAt { get; set; }
}

public class AnomalyDetectionRequest
{
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class AnomalyRuleDTO
{
    public Guid RuleId { get; set; }
    public string RuleName { get; set; }
    public string RuleCondition { get; set; }
    public int Threshold { get; set; }
    public string Status { get; set; }
}

public class CreateAnomalyRuleRequest
{
    public string RuleName { get; set; }
    public string RuleCondition { get; set; }
    public int Threshold { get; set; }
    public string AlertSeverity { get; set; }
}

public class ExposureAnalysisDTO
{
    public string EntityId { get; set; }
    public decimal TotalExposure { get; set; }
    public decimal OnBalanceExposure { get; set; }
    public decimal OffBalanceExposure { get; set; }
    public double RiskConcentration { get; set; }
    public List<ExposureBreakdownDTO> ExposureBreakdown { get; set; }
}

public class ExposureBreakdownDTO
{
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
}

public class PartyExposureDTO
{
    public Guid PartyId { get; set; }
    public string PartyName { get; set; }
    public decimal TotalExposure { get; set; }
    public List<ExposureProductDTO> ExposureByProduct { get; set; }
}

public class ExposureProductDTO
{
    public string ProductCode { get; set; }
    public decimal Exposure { get; set; }
}

public class GeographicExposureDTO
{
    public List<GeographicExposureLineDTO> ExposureByCountry { get; set; }
    public decimal TotalExposure { get; set; }
}

public class GeographicExposureLineDTO
{
    public string CountryCode { get; set; }
    public decimal Exposure { get; set; }
    public double PercentageOfTotal { get; set; }
}

public class SectorExposureDTO
{
    public List<SectorExposureLineDTO> ExposureBySector { get; set; }
    public decimal TotalExposure { get; set; }
}

public class SectorExposureLineDTO
{
    public string SectorCode { get; set; }
    public decimal Exposure { get; set; }
    public double PercentageOfTotal { get; set; }
}

public class ConcentrationRiskDTO
{
    public string ConcentrationType { get; set; }
    public double ConcentrationRatio { get; set; }
    public string RiskLevel { get; set; }
    public List<TopExposuresDTO> TopExposures { get; set; }
}

public class TopExposuresDTO
{
    public string EntityName { get; set; }
    public decimal Exposure { get; set; }
    public double PercentageOfTotal { get; set; }
}

public class RegulatoryLimitDTO
{
    public Guid Id { get; set; }
    public string LimitCode { get; set; }
    public string Regulation { get; set; }
    public decimal LimitAmount { get; set; }
    public string Status { get; set; }
}

public class RegulatoryComplianceDTO
{
    public DateTime AsOfDate { get; set; }
    public int TotalRegulatoryLimits { get; set; }
    public int CompliantLimits { get; set; }
    public int ViolatedLimits { get; set; }
    public double CompliancePercentage { get; set; }
    public List<RegulatoryViolationDTO> Violations { get; set; }
}

public class RegulatoryViolationDTO
{
    public string LimitCode { get; set; }
    public string Regulation { get; set; }
    public decimal LimitAmount { get; set; }
    public decimal CurrentExposure { get; set; }
    public decimal ExcessAmount { get; set; }
}

public class RegulatoryReportDTO
{
    public Guid ReportId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public RegulatoryComplianceDTO ComplianceStatus { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class EscalationConfigDTO
{
    public Guid Id { get; set; }
    public string EscalationName { get; set; }
    public List<EscalationLevelDTO> EscalationLevels { get; set; }
}

public class EscalationLevelDTO
{
    public int Level { get; set; }
    public List<string> EscalateTo { get; set; }
    public int TimeoutMinutes { get; set; }
}

public class CreateEscalationConfigRequest
{
    public string EscalationName { get; set; }
    public List<EscalationLevelDTO> EscalationLevels { get; set; }
}

public class PendingApprovalDTO
{
    public Guid ApprovalId { get; set; }
    public Guid LimitId { get; set; }
    public string LimitType { get; set; }
    public decimal RequestedAmount { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; }
}

public class RiskDashboardDTO
{
    public DateTime AsOfDate { get; set; }
    public double OverallRiskScore { get; set; }
    public int LimitBreaches { get; set; }
    public int AnomaliesDetected { get; set; }
    public RegulatoryComplianceDTO RegulatoryStatus { get; set; }
    public List<RiskAlertDTO> CriticalAlerts { get; set; }
}

public class RiskScoreDTO
{
    public string EntityId { get; set; }
    public double RiskScore { get; set; }
    public string RiskLevel { get; set; }
    public List<RiskFactorDTO> RiskFactors { get; set; }
}

public class RiskFactorDTO
{
    public string FactorName { get; set; }
    public double WeightedScore { get; set; }
}

public class RiskAlertDTO
{
    public Guid AlertId { get; set; }
    public string AlertType { get; set; }
    public string Message { get; set; }
    public string Severity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ComplianceMetricsDTO
{
    public int TotalLimits { get; set; }
    public int CompliantLimits { get; set; }
    public double CompliancePercentage { get; set; }
    public int BreachesThisPeriod { get; set; }
    public int AnomaliesDetected { get; set; }
}
