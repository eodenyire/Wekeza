using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Events;

// AML Case Events
public record AMLCaseCreatedDomainEvent(
    Guid CaseId, 
    string CaseNumber, 
    AMLAlertType AlertType, 
    RiskScore RiskScore) : IDomainEvent;

public record AMLCaseAssignedDomainEvent(
    Guid CaseId, 
    string CaseNumber, 
    string InvestigatorId) : IDomainEvent;

public record AMLEvidenceAddedDomainEvent(
    Guid CaseId, 
    string CaseNumber, 
    string EvidenceType, 
    string Description) : IDomainEvent;

public record AMLCaseEscalatedDomainEvent(
    Guid CaseId, 
    string CaseNumber, 
    string Reason) : IDomainEvent;

public record SARFiledDomainEvent(
    Guid CaseId, 
    string CaseNumber, 
    string SARReference, 
    string Reason) : IDomainEvent;

public record AMLCaseClosedDomainEvent(
    Guid CaseId, 
    string CaseNumber, 
    AMLResolution Resolution, 
    string Reason) : IDomainEvent;

public record AMLCaseReopenedDomainEvent(
    Guid CaseId, 
    string CaseNumber, 
    string Reason) : IDomainEvent;

public record AMLRiskScoreUpdatedDomainEvent(
    Guid CaseId, 
    string CaseNumber, 
    RiskScore OldScore, 
    RiskScore NewScore) : IDomainEvent;

// Transaction Monitoring Events
public record TransactionMonitoringCompletedDomainEvent(
    Guid MonitoringId, 
    Guid TransactionId, 
    ScreeningResult Result, 
    AlertSeverity Severity) : IDomainEvent;

public record TransactionMonitoringReviewedDomainEvent(
    Guid MonitoringId, 
    Guid TransactionId, 
    MonitoringDecision Decision, 
    string ReviewedBy) : IDomainEvent;

public record MonitoringAlertGeneratedDomainEvent(
    Guid MonitoringId, 
    Guid TransactionId, 
    string AlertType, 
    AlertSeverity Severity) : IDomainEvent;

public record TransactionRiskScoreUpdatedDomainEvent(
    Guid MonitoringId, 
    Guid TransactionId, 
    RiskScore? OldScore, 
    RiskScore NewScore, 
    string Reason) : IDomainEvent;

public record TransactionBlockedDomainEvent(
    Guid MonitoringId, 
    Guid TransactionId, 
    string Reason) : IDomainEvent;

public record TransactionClearedDomainEvent(
    Guid MonitoringId, 
    Guid TransactionId, 
    string Reason) : IDomainEvent;

// Sanctions Screening Events
public record SanctionsScreeningInitiatedDomainEvent(
    Guid ScreeningId, 
    EntityType EntityType, 
    Guid EntityId, 
    List<string> Watchlists) : IDomainEvent;

public record SanctionsMatchFoundDomainEvent(
    Guid ScreeningId, 
    EntityType EntityType, 
    Guid EntityId, 
    string WatchlistName, 
    string MatchedName, 
    decimal MatchScore) : IDomainEvent;

public record SanctionsScreeningCompletedDomainEvent(
    Guid ScreeningId, 
    EntityType EntityType, 
    Guid EntityId, 
    ScreeningStatus Status, 
    decimal HighestMatchScore, 
    int MatchCount) : IDomainEvent;

public record SanctionsScreeningReviewedDomainEvent(
    Guid ScreeningId, 
    EntityType EntityType, 
    Guid EntityId, 
    ScreeningDecision Decision, 
    string ReviewedBy) : IDomainEvent;

public record EntityWhitelistedDomainEvent(
    EntityType EntityType, 
    Guid EntityId, 
    string AddedBy, 
    string Reason) : IDomainEvent;

public record EntityBlockedDomainEvent(
    EntityType EntityType, 
    Guid EntityId, 
    string BlockedBy, 
    string Reason) : IDomainEvent;

public record SanctionsScreeningEscalatedDomainEvent(
    Guid ScreeningId, 
    EntityType EntityType, 
    Guid EntityId, 
    string Reason) : IDomainEvent;

// Risk Management Events
public record RiskLimitBreachedDomainEvent(
    Guid LimitId, 
    string LimitType, 
    Guid EntityId, 
    Money LimitAmount, 
    Money UtilizedAmount, 
    decimal UtilizationPercentage) : IDomainEvent;

public record RiskLimitUpdatedDomainEvent(
    Guid LimitId, 
    string LimitType, 
    Guid EntityId, 
    Money OldLimit, 
    Money NewLimit, 
    string UpdatedBy) : IDomainEvent;

public record RiskLimitSuspendedDomainEvent(
    Guid LimitId, 
    string LimitType, 
    Guid EntityId, 
    string SuspendedBy, 
    string Reason) : IDomainEvent;

public record RiskLimitReinstatedDomainEvent(
    Guid LimitId, 
    string LimitType, 
    Guid EntityId, 
    string ReinstatedBy, 
    string Reason) : IDomainEvent;

// Fraud Detection Events
public record FraudAlertGeneratedDomainEvent(
    Guid AlertId, 
    Guid TransactionId, 
    string FraudType, 
    RiskScore RiskScore, 
    string Description) : IDomainEvent;

public record FraudInvestigationInitiatedDomainEvent(
    Guid AlertId, 
    Guid TransactionId, 
    string InvestigatorId, 
    string Reason) : IDomainEvent;

public record FraudAlertResolvedDomainEvent(
    Guid AlertId, 
    Guid TransactionId, 
    string Resolution, 
    string ResolvedBy) : IDomainEvent;

// Compliance Events
public record ComplianceViolationDetectedDomainEvent(
    Guid ViolationId, 
    string ViolationType, 
    Guid EntityId, 
    string Description, 
    RiskScore Severity) : IDomainEvent;

public record RegulatoryReportGeneratedDomainEvent(
    Guid ReportId, 
    string ReportType, 
    DateTime ReportingPeriod, 
    string GeneratedBy) : IDomainEvent;

public record ComplianceAuditInitiatedDomainEvent(
    Guid AuditId, 
    string AuditType, 
    DateTime AuditDate, 
    string AuditorId) : IDomainEvent;

// General Risk Events
public record AMLCaseRequiredDomainEvent(
    Guid? TransactionId, 
    Guid? PartyId, 
    AMLAlertType AlertType, 
    RiskScore RiskScore) : IDomainEvent;

public record HighRiskActivityDetectedDomainEvent(
    Guid EntityId, 
    EntityType EntityType, 
    string ActivityType, 
    RiskScore RiskScore, 
    string Description) : IDomainEvent;

public record RegulatoryThresholdExceededDomainEvent(
    Guid TransactionId, 
    string ThresholdType, 
    Money Amount, 
    Money ThresholdLimit) : IDomainEvent;

public record SuspiciousPatternDetectedDomainEvent(
    Guid PatternId, 
    string PatternType, 
    List<Guid> RelatedTransactions, 
    RiskScore RiskScore, 
    string Description) : IDomainEvent;