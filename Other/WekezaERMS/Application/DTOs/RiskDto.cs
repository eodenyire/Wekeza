using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Application.DTOs;

public class RiskDto
{
    public Guid Id { get; set; }
    public string RiskCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RiskCategory Category { get; set; }
    public RiskStatus Status { get; set; }
    public RiskLikelihood InherentLikelihood { get; set; }
    public RiskImpact InherentImpact { get; set; }
    public int InherentRiskScore { get; set; }
    public RiskLevel InherentRiskLevel { get; set; }
    public RiskLikelihood? ResidualLikelihood { get; set; }
    public RiskImpact? ResidualImpact { get; set; }
    public int? ResidualRiskScore { get; set; }
    public RiskLevel? ResidualRiskLevel { get; set; }
    public RiskTreatmentStrategy TreatmentStrategy { get; set; }
    public Guid OwnerId { get; set; }
    public string Department { get; set; } = string.Empty;
    public DateTime IdentifiedDate { get; set; }
    public DateTime? LastAssessmentDate { get; set; }
    public DateTime NextReviewDate { get; set; }
    public int RiskAppetite { get; set; }
    public DateTime CreatedAt { get; set; }
}
