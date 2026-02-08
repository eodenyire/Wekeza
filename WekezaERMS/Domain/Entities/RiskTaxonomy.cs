using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Risk Taxonomy Entry - Provides comprehensive risk classification
/// Aligned with Riskonnect's comprehensive risk taxonomy capabilities
/// </summary>
public class RiskTaxonomy
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Taxonomy code (e.g., TAX-001)
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Risk category
    /// </summary>
    public RiskCategory Category { get; private set; }

    /// <summary>
    /// Sub-category name
    /// </summary>
    public string SubCategory { get; private set; }

    /// <summary>
    /// Risk type within sub-category
    /// </summary>
    public string RiskType { get; private set; }

    /// <summary>
    /// Detailed description
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Common indicators
    /// </summary>
    public string? CommonIndicators { get; private set; }

    /// <summary>
    /// Typical controls
    /// </summary>
    public string? TypicalControls { get; private set; }

    /// <summary>
    /// Framework alignment
    /// </summary>
    public FrameworkType? FrameworkAlignment { get; private set; }

    /// <summary>
    /// Industry applicability
    /// </summary>
    public string? IndustryApplicability { get; private set; }

    /// <summary>
    /// Parent taxonomy ID for hierarchical structure
    /// </summary>
    public Guid? ParentTaxonomyId { get; private set; }

    /// <summary>
    /// Level in hierarchy (1 = top level)
    /// </summary>
    public int HierarchyLevel { get; private set; }

    /// <summary>
    /// Whether this is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private RiskTaxonomy() { }

    public static RiskTaxonomy Create(
        string code,
        RiskCategory category,
        string subCategory,
        string riskType,
        string description,
        int hierarchyLevel,
        Guid? parentTaxonomyId,
        Guid createdBy)
    {
        return new RiskTaxonomy
        {
            Id = Guid.NewGuid(),
            Code = code,
            Category = category,
            SubCategory = subCategory,
            RiskType = riskType,
            Description = description,
            HierarchyLevel = hierarchyLevel,
            ParentTaxonomyId = parentTaxonomyId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void UpdateIndicatorsAndControls(
        string commonIndicators,
        string typicalControls,
        Guid updatedBy)
    {
        CommonIndicators = commonIndicators;
        TypicalControls = typicalControls;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetFrameworkAlignment(FrameworkType framework, Guid updatedBy)
    {
        FrameworkAlignment = framework;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
