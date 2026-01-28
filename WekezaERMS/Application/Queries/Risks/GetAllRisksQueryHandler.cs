using MediatR;
using WekezaERMS.Application.Commands.Risks;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Queries.Risks;

public class GetAllRisksQueryHandler : IRequestHandler<GetAllRisksQuery, List<RiskDto>>
{
    private readonly IRiskRepository _riskRepository;

    public GetAllRisksQueryHandler(IRiskRepository riskRepository)
    {
        _riskRepository = riskRepository;
    }

    public async Task<List<RiskDto>> Handle(GetAllRisksQuery request, CancellationToken cancellationToken)
    {
        var risks = await _riskRepository.GetAllAsync();
        return risks.Select(MapToDto).ToList();
    }

    private RiskDto MapToDto(Risk risk)
    {
        return new RiskDto
        {
            Id = risk.Id,
            RiskCode = risk.RiskCode,
            Title = risk.Title,
            Description = risk.Description,
            Category = risk.Category,
            Status = risk.Status,
            InherentLikelihood = risk.InherentLikelihood,
            InherentImpact = risk.InherentImpact,
            InherentRiskScore = risk.InherentRiskScore,
            InherentRiskLevel = risk.InherentRiskLevel,
            ResidualLikelihood = risk.ResidualLikelihood,
            ResidualImpact = risk.ResidualImpact,
            ResidualRiskScore = risk.ResidualRiskScore,
            ResidualRiskLevel = risk.ResidualRiskLevel,
            TreatmentStrategy = risk.TreatmentStrategy,
            OwnerId = risk.OwnerId,
            Department = risk.Department,
            IdentifiedDate = risk.IdentifiedDate,
            LastAssessmentDate = risk.LastAssessmentDate,
            NextReviewDate = risk.NextReviewDate,
            RiskAppetite = risk.RiskAppetite,
            CreatedAt = risk.CreatedAt
        };
    }
}
