using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Business Continuity Plan Entity - Represents business continuity and resilience plans
/// Aligned with Riskonnect business continuity management capabilities
/// </summary>
public class BusinessContinuityPlan
{
    /// <summary>
    /// Unique identifier for the BCP
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Human-readable BCP ID (e.g., BCP-2024-001)
    /// </summary>
    public string PlanCode { get; private set; }

    /// <summary>
    /// Plan name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Detailed description of the plan
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Business function or process covered
    /// </summary>
    public string BusinessFunction { get; private set; }

    /// <summary>
    /// Department responsible for the plan
    /// </summary>
    public string Department { get; private set; }

    /// <summary>
    /// Plan owner
    /// </summary>
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// Recovery Time Objective (RTO) in hours
    /// Maximum acceptable downtime
    /// </summary>
    public int RecoveryTimeObjectiveHours { get; private set; }

    /// <summary>
    /// Recovery Point Objective (RPO) in hours
    /// Maximum acceptable data loss
    /// </summary>
    public int RecoveryPointObjectiveHours { get; private set; }

    /// <summary>
    /// Business criticality (1-5, with 5 being most critical)
    /// </summary>
    public int CriticalityLevel { get; private set; }

    /// <summary>
    /// Potential financial impact per hour of disruption
    /// </summary>
    public decimal FinancialImpactPerHour { get; private set; }

    /// <summary>
    /// Business impact analysis summary
    /// </summary>
    public string? BusinessImpactAnalysis { get; private set; }

    /// <summary>
    /// Recovery strategies and procedures
    /// </summary>
    public string RecoveryStrategies { get; private set; }

    /// <summary>
    /// Alternative site or location for operations
    /// </summary>
    public string? AlternativeLocation { get; private set; }

    /// <summary>
    /// Key personnel and contact information
    /// </summary>
    public string KeyPersonnel { get; private set; }

    /// <summary>
    /// Resource requirements for recovery
    /// </summary>
    public string? ResourceRequirements { get; private set; }

    /// <summary>
    /// Dependencies on other systems or functions
    /// </summary>
    public string? Dependencies { get; private set; }

    /// <summary>
    /// Communication plan during disruption
    /// </summary>
    public string? CommunicationPlan { get; private set; }

    /// <summary>
    /// Crisis management procedures
    /// </summary>
    public string? CrisisManagementProcedures { get; private set; }

    /// <summary>
    /// Date of last test
    /// </summary>
    public DateTime? LastTestDate { get; private set; }

    /// <summary>
    /// Result of last test
    /// </summary>
    public string? LastTestResults { get; private set; }

    /// <summary>
    /// Date of next scheduled test
    /// </summary>
    public DateTime NextTestDate { get; private set; }

    /// <summary>
    /// Date of last review
    /// </summary>
    public DateTime? LastReviewDate { get; private set; }

    /// <summary>
    /// Date of next scheduled review
    /// </summary>
    public DateTime NextReviewDate { get; private set; }

    /// <summary>
    /// Plan version
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// Whether the plan is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private BusinessContinuityPlan() { }

    public static BusinessContinuityPlan Create(
        string planCode,
        string name,
        string description,
        string businessFunction,
        string department,
        Guid ownerId,
        int rtoHours,
        int rpoHours,
        int criticalityLevel,
        decimal financialImpactPerHour,
        string recoveryStrategies,
        string keyPersonnel,
        Guid createdBy)
    {
        return new BusinessContinuityPlan
        {
            Id = Guid.NewGuid(),
            PlanCode = planCode,
            Name = name,
            Description = description,
            BusinessFunction = businessFunction,
            Department = department,
            OwnerId = ownerId,
            RecoveryTimeObjectiveHours = rtoHours,
            RecoveryPointObjectiveHours = rpoHours,
            CriticalityLevel = criticalityLevel,
            FinancialImpactPerHour = financialImpactPerHour,
            RecoveryStrategies = recoveryStrategies,
            KeyPersonnel = keyPersonnel,
            NextTestDate = DateTime.UtcNow.AddMonths(6), // Default 6 months
            NextReviewDate = DateTime.UtcNow.AddMonths(12), // Default annual review
            Version = "1.0",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void RecordTest(string testResults, bool successful, Guid testedBy)
    {
        LastTestDate = DateTime.UtcNow;
        LastTestResults = testResults;
        NextTestDate = DateTime.UtcNow.AddMonths(6);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = testedBy;
    }

    public void Review(string version, Guid reviewedBy)
    {
        LastReviewDate = DateTime.UtcNow;
        NextReviewDate = DateTime.UtcNow.AddMonths(12);
        Version = version;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = reviewedBy;
    }

    public void UpdateBusinessImpactAnalysis(string analysis, Guid updatedBy)
    {
        BusinessImpactAnalysis = analysis;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateRTO(int newRtoHours, Guid updatedBy)
    {
        RecoveryTimeObjectiveHours = newRtoHours;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateRPO(int newRpoHours, Guid updatedBy)
    {
        RecoveryPointObjectiveHours = newRpoHours;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate(Guid activatedBy)
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = activatedBy;
    }

    public void Deactivate(Guid deactivatedBy)
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;
    }
}
