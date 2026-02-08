using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Risk Template Entity - Represents pre-configured risk templates
/// Aligned with Riskonnect out-of-the-box templates and workflows
/// </summary>
public class RiskTemplate
{
    /// <summary>
    /// Unique identifier for the template
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Template code (e.g., TMPL-CYBER-001)
    /// </summary>
    public string TemplateCode { get; private set; }

    /// <summary>
    /// Template name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Template description
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Risk category this template applies to
    /// </summary>
    public RiskCategory Category { get; private set; }

    /// <summary>
    /// Industry this template is designed for (e.g., Banking, Healthcare, Finance)
    /// </summary>
    public string? Industry { get; private set; }

    /// <summary>
    /// Framework alignment (e.g., ISO 31000, COSO, SOX)
    /// </summary>
    public FrameworkType? Framework { get; private set; }

    /// <summary>
    /// Pre-configured risk title template
    /// </summary>
    public string RiskTitleTemplate { get; private set; }

    /// <summary>
    /// Pre-configured risk description template
    /// </summary>
    public string RiskDescriptionTemplate { get; private set; }

    /// <summary>
    /// Suggested inherent likelihood
    /// </summary>
    public RiskLikelihood SuggestedLikelihood { get; private set; }

    /// <summary>
    /// Suggested inherent impact
    /// </summary>
    public RiskImpact SuggestedImpact { get; private set; }

    /// <summary>
    /// Suggested treatment strategy
    /// </summary>
    public RiskTreatmentStrategy SuggestedTreatmentStrategy { get; private set; }

    /// <summary>
    /// Pre-configured controls
    /// </summary>
    public string? PreConfiguredControls { get; private set; }

    /// <summary>
    /// Pre-configured mitigation actions
    /// </summary>
    public string? PreConfiguredMitigations { get; private set; }

    /// <summary>
    /// Key risk indicators suggestions
    /// </summary>
    public string? SuggestedKRIs { get; private set; }

    /// <summary>
    /// Assessment frequency in months
    /// </summary>
    public int AssessmentFrequencyMonths { get; private set; }

    /// <summary>
    /// Compliance requirements
    /// </summary>
    public string? ComplianceRequirements { get; private set; }

    /// <summary>
    /// Whether this is a standard template
    /// </summary>
    public bool IsStandardTemplate { get; private set; }

    /// <summary>
    /// Whether this template is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Number of times this template has been used
    /// </summary>
    public int UsageCount { get; private set; }

    /// <summary>
    /// Template version
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private RiskTemplate() { }

    public static RiskTemplate Create(
        string templateCode,
        string name,
        string description,
        RiskCategory category,
        string riskTitleTemplate,
        string riskDescriptionTemplate,
        RiskLikelihood suggestedLikelihood,
        RiskImpact suggestedImpact,
        RiskTreatmentStrategy suggestedTreatmentStrategy,
        int assessmentFrequencyMonths,
        bool isStandardTemplate,
        string? industry,
        FrameworkType? framework,
        Guid createdBy)
    {
        return new RiskTemplate
        {
            Id = Guid.NewGuid(),
            TemplateCode = templateCode,
            Name = name,
            Description = description,
            Category = category,
            RiskTitleTemplate = riskTitleTemplate,
            RiskDescriptionTemplate = riskDescriptionTemplate,
            SuggestedLikelihood = suggestedLikelihood,
            SuggestedImpact = suggestedImpact,
            SuggestedTreatmentStrategy = suggestedTreatmentStrategy,
            AssessmentFrequencyMonths = assessmentFrequencyMonths,
            IsStandardTemplate = isStandardTemplate,
            Industry = industry,
            Framework = framework,
            IsActive = true,
            UsageCount = 0,
            Version = "1.0",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void IncrementUsage()
    {
        UsageCount++;
    }

    public void UpdateTemplate(
        string name,
        string description,
        string version,
        Guid updatedBy)
    {
        Name = name;
        Description = description;
        Version = version;
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

    public Risk CreateRiskFromTemplate(
        string riskCode,
        Guid ownerId,
        string department,
        int riskAppetite,
        Guid createdBy)
    {
        IncrementUsage();
        
        return Risk.Create(
            riskCode,
            RiskTitleTemplate,
            RiskDescriptionTemplate,
            Category,
            SuggestedLikelihood,
            SuggestedImpact,
            ownerId,
            department,
            SuggestedTreatmentStrategy,
            riskAppetite,
            createdBy
        );
    }
}
