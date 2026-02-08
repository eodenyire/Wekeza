using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Application.DTOs;

public class ControlDto
{
    public Guid Id { get; set; }
    public Guid RiskId { get; set; }
    public string ControlName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ControlType { get; set; } = string.Empty;
    public ControlEffectiveness? Effectiveness { get; set; }
    public DateTime? LastTestedDate { get; set; }
    public DateTime? NextTestDate { get; set; }
    public Guid OwnerId { get; set; }
    public string TestingFrequency { get; set; } = string.Empty;
    public string TestingEvidence { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
