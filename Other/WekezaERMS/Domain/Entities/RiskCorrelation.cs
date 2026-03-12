using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Risk Correlation - Represents relationships and correlations between risks
/// Aligned with Riskonnect's risk correlation analysis capabilities
/// </summary>
public class RiskCorrelation
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// First risk ID
    /// </summary>
    public Guid Risk1Id { get; private set; }

    /// <summary>
    /// Second risk ID
    /// </summary>
    public Guid Risk2Id { get; private set; }

    /// <summary>
    /// Correlation strength (0-100)
    /// </summary>
    public decimal CorrelationStrength { get; private set; }

    /// <summary>
    /// Type of correlation
    /// </summary>
    public string CorrelationType { get; private set; }

    /// <summary>
    /// Description of the relationship
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Whether one risk triggers the other
    /// </summary>
    public bool IsCausalRelationship { get; private set; }

    /// <summary>
    /// Combined impact if both risks materialize
    /// </summary>
    public RiskImpact? CombinedImpact { get; private set; }

    /// <summary>
    /// Analysis date
    /// </summary>
    public DateTime AnalysisDate { get; private set; }

    /// <summary>
    /// Analyst notes
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private RiskCorrelation() { }

    public static RiskCorrelation Create(
        Guid risk1Id,
        Guid risk2Id,
        decimal correlationStrength,
        string correlationType,
        string description,
        bool isCausalRelationship,
        Guid createdBy)
    {
        if (risk1Id == risk2Id)
        {
            throw new ArgumentException("Cannot create correlation between the same risk");
        }

        if (correlationStrength < 0 || correlationStrength > 100)
        {
            throw new ArgumentException("Correlation strength must be between 0 and 100");
        }

        return new RiskCorrelation
        {
            Id = Guid.NewGuid(),
            Risk1Id = risk1Id,
            Risk2Id = risk2Id,
            CorrelationStrength = correlationStrength,
            CorrelationType = correlationType,
            Description = description,
            IsCausalRelationship = isCausalRelationship,
            AnalysisDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void UpdateCorrelation(
        decimal newStrength,
        string description,
        Guid updatedBy)
    {
        CorrelationStrength = newStrength;
        Description = description;
        AnalysisDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetCombinedImpact(RiskImpact combinedImpact, Guid updatedBy)
    {
        CombinedImpact = combinedImpact;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
