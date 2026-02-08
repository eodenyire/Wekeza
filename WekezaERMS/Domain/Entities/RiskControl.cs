using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Represents a control mechanism to mitigate a risk
/// </summary>
public class RiskControl
{
    public Guid Id { get; private set; }
    public Guid RiskId { get; private set; }
    public string ControlName { get; private set; }
    public string Description { get; private set; }
    public string ControlType { get; private set; } // Preventive, Detective, Corrective
    public ControlEffectiveness? Effectiveness { get; private set; }
    public DateTime? LastTestedDate { get; private set; }
    public DateTime? NextTestDate { get; private set; }
    public Guid OwnerId { get; private set; }
    public string TestingFrequency { get; private set; } // Monthly, Quarterly, Annually
    public string TestingEvidence { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private RiskControl() { }

    public static RiskControl Create(
        Guid riskId,
        string controlName,
        string description,
        string controlType,
        Guid ownerId,
        string testingFrequency,
        Guid createdBy)
    {
        return new RiskControl
        {
            Id = Guid.NewGuid(),
            RiskId = riskId,
            ControlName = controlName,
            Description = description,
            ControlType = controlType,
            OwnerId = ownerId,
            TestingFrequency = testingFrequency,
            TestingEvidence = string.Empty,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void UpdateEffectiveness(
        ControlEffectiveness effectiveness,
        string testingEvidence,
        Guid updatedBy)
    {
        Effectiveness = effectiveness;
        TestingEvidence = testingEvidence;
        LastTestedDate = DateTime.UtcNow;
        NextTestDate = CalculateNextTestDate();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Update(
        string controlName,
        string description,
        string controlType,
        Guid ownerId,
        string testingFrequency,
        Guid updatedBy)
    {
        ControlName = controlName;
        Description = description;
        ControlType = controlType;
        OwnerId = ownerId;
        TestingFrequency = testingFrequency;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    private DateTime CalculateNextTestDate()
    {
        return TestingFrequency?.ToLower() switch
        {
            "monthly" => DateTime.UtcNow.AddMonths(1),
            "quarterly" => DateTime.UtcNow.AddMonths(3),
            "annually" => DateTime.UtcNow.AddYears(1),
            _ => DateTime.UtcNow.AddMonths(3) // Default to quarterly
        };
    }
}
