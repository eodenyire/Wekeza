using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Main Risk Entity - Represents a risk in the risk register
/// Core aggregate for the Enterprise Risk Management System
/// </summary>
public class Risk
{
    /// <summary>
    /// Unique identifier for the risk
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Human-readable risk ID (e.g., RISK-2024-001)
    /// </summary>
    public string RiskCode { get; private set; }

    /// <summary>
    /// Risk title/name
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Detailed description of the risk
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Risk category classification
    /// </summary>
    public RiskCategory Category { get; private set; }

    /// <summary>
    /// Current status of the risk
    /// </summary>
    public RiskStatus Status { get; private set; }

    /// <summary>
    /// Likelihood of the risk occurring (inherent)
    /// </summary>
    public RiskLikelihood InherentLikelihood { get; private set; }

    /// <summary>
    /// Impact if the risk occurs (inherent)
    /// </summary>
    public RiskImpact InherentImpact { get; private set; }

    /// <summary>
    /// Inherent risk score (likelihood × impact)
    /// </summary>
    public int InherentRiskScore { get; private set; }

    /// <summary>
    /// Inherent risk level derived from score
    /// </summary>
    public RiskLevel InherentRiskLevel { get; private set; }

    /// <summary>
    /// Likelihood after controls (residual)
    /// </summary>
    public RiskLikelihood? ResidualLikelihood { get; private set; }

    /// <summary>
    /// Impact after controls (residual)
    /// </summary>
    public RiskImpact? ResidualImpact { get; private set; }

    /// <summary>
    /// Residual risk score
    /// </summary>
    public int? ResidualRiskScore { get; private set; }

    /// <summary>
    /// Residual risk level
    /// </summary>
    public RiskLevel? ResidualRiskLevel { get; private set; }

    /// <summary>
    /// Treatment strategy for this risk
    /// </summary>
    public RiskTreatmentStrategy TreatmentStrategy { get; private set; }

    /// <summary>
    /// Risk owner (user ID from Wekeza Core)
    /// </summary>
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// Department or business unit affected
    /// </summary>
    public string Department { get; private set; }

    /// <summary>
    /// Date risk was identified
    /// </summary>
    public DateTime IdentifiedDate { get; private set; }

    /// <summary>
    /// Last assessment date
    /// </summary>
    public DateTime? LastAssessmentDate { get; private set; }

    /// <summary>
    /// Next review date
    /// </summary>
    public DateTime NextReviewDate { get; private set; }

    /// <summary>
    /// Risk appetite threshold for this risk
    /// </summary>
    public int RiskAppetite { get; private set; }

    /// <summary>
    /// Associated controls
    /// </summary>
    public List<RiskControl> Controls { get; private set; }

    /// <summary>
    /// Mitigation actions
    /// </summary>
    public List<MitigationAction> MitigationActions { get; private set; }

    /// <summary>
    /// Key Risk Indicators
    /// </summary>
    public List<KeyRiskIndicator> KeyRiskIndicators { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private Risk() 
    { 
        Controls = new List<RiskControl>();
        MitigationActions = new List<MitigationAction>();
        KeyRiskIndicators = new List<KeyRiskIndicator>();
    }

    public static Risk Create(
        string riskCode,
        string title,
        string description,
        RiskCategory category,
        RiskLikelihood inherentLikelihood,
        RiskImpact inherentImpact,
        Guid ownerId,
        string department,
        RiskTreatmentStrategy treatmentStrategy,
        int riskAppetite,
        Guid createdBy)
    {
        var risk = new Risk
        {
            Id = Guid.NewGuid(),
            RiskCode = riskCode,
            Title = title,
            Description = description,
            Category = category,
            Status = RiskStatus.Identified,
            InherentLikelihood = inherentLikelihood,
            InherentImpact = inherentImpact,
            TreatmentStrategy = treatmentStrategy,
            OwnerId = ownerId,
            Department = department,
            IdentifiedDate = DateTime.UtcNow,
            NextReviewDate = DateTime.UtcNow.AddMonths(3), // Default 3 months
            RiskAppetite = riskAppetite,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        risk.CalculateInherentRisk();
        return risk;
    }

    public void Assess(
        RiskLikelihood likelihood,
        RiskImpact impact,
        Guid assessedBy)
    {
        InherentLikelihood = likelihood;
        InherentImpact = impact;
        CalculateInherentRisk();
        LastAssessmentDate = DateTime.UtcNow;
        Status = RiskStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = assessedBy;
    }

    public void UpdateResidualRisk(
        RiskLikelihood residualLikelihood,
        RiskImpact residualImpact,
        Guid updatedBy)
    {
        ResidualLikelihood = residualLikelihood;
        ResidualImpact = residualImpact;
        CalculateResidualRisk();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void AddControl(RiskControl control)
    {
        Controls.Add(control);
        // Recalculate residual risk if we have effectiveness data
        if (Controls.Any(c => c.Effectiveness != null))
        {
            RecalculateResidualRiskFromControls();
        }
    }

    public void AddMitigationAction(MitigationAction action)
    {
        MitigationActions.Add(action);
        if (MitigationActions.Any(a => a.Status == MitigationStatus.InProgress))
        {
            Status = RiskStatus.Mitigating;
        }
    }

    public void AddKeyRiskIndicator(KeyRiskIndicator kri)
    {
        KeyRiskIndicators.Add(kri);
    }

    public void Escalate(Guid escalatedBy)
    {
        Status = RiskStatus.Escalated;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = escalatedBy;
    }

    public void Close(string closureReason, Guid closedBy)
    {
        Status = RiskStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = closedBy;
    }

    public void Update(
        string title,
        string description,
        RiskCategory category,
        string department,
        RiskTreatmentStrategy treatmentStrategy,
        Guid ownerId,
        int riskAppetite,
        DateTime nextReviewDate,
        Guid updatedBy)
    {
        Title = title;
        Description = description;
        Category = category;
        Department = department;
        TreatmentStrategy = treatmentStrategy;
        OwnerId = ownerId;
        RiskAppetite = riskAppetite;
        NextReviewDate = nextReviewDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    private void CalculateInherentRisk()
    {
        InherentRiskScore = (int)InherentLikelihood * (int)InherentImpact;
        InherentRiskLevel = DetermineRiskLevel(InherentRiskScore);
    }

    private void CalculateResidualRisk()
    {
        if (ResidualLikelihood.HasValue && ResidualImpact.HasValue)
        {
            ResidualRiskScore = (int)ResidualLikelihood.Value * (int)ResidualImpact.Value;
            ResidualRiskLevel = DetermineRiskLevel(ResidualRiskScore.Value);
        }
    }

    private void RecalculateResidualRiskFromControls()
    {
        // Simplified calculation - reduce likelihood based on control effectiveness
        var effectiveControls = Controls.Count(c => 
            c.Effectiveness == ControlEffectiveness.Effective || 
            c.Effectiveness == ControlEffectiveness.HighlyEffective);

        var reductionFactor = Math.Min(effectiveControls, 2); // Max 2 levels reduction
        var newLikelihood = Math.Max(1, (int)InherentLikelihood - reductionFactor);
        
        ResidualLikelihood = (RiskLikelihood)newLikelihood;
        ResidualImpact = InherentImpact; // Impact typically doesn't change
        CalculateResidualRisk();
    }

    private static RiskLevel DetermineRiskLevel(int score)
    {
        return score switch
        {
            <= 4 => RiskLevel.Low,
            <= 9 => RiskLevel.Medium,
            <= 15 => RiskLevel.High,
            <= 20 => RiskLevel.VeryHigh,
            _ => RiskLevel.Critical
        };
    }
}
