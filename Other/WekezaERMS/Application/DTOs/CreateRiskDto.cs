using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Application.DTOs;

public class CreateRiskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RiskCategory Category { get; set; }
    public RiskLikelihood InherentLikelihood { get; set; }
    public RiskImpact InherentImpact { get; set; }
    public Guid OwnerId { get; set; }
    public string Department { get; set; } = string.Empty;
    public RiskTreatmentStrategy TreatmentStrategy { get; set; }
    public int RiskAppetite { get; set; }
}
