using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Application.DTOs;

public class ControlTestDto
{
    public ControlEffectiveness Effectiveness { get; set; }
    public string TestingEvidence { get; set; } = string.Empty;
    public DateTime TestDate { get; set; }
}
