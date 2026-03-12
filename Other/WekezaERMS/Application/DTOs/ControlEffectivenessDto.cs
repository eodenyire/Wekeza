using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Application.DTOs;

public class ControlEffectivenessDto
{
    public ControlEffectiveness Effectiveness { get; set; }
    public string TestingEvidence { get; set; } = string.Empty;
}
