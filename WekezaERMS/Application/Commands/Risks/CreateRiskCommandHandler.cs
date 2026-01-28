using MediatR;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Commands.Risks;

public class CreateRiskCommandHandler : IRequestHandler<CreateRiskCommand, RiskDto>
{
    private readonly IRiskRepository _riskRepository;

    public CreateRiskCommandHandler(IRiskRepository riskRepository)
    {
        _riskRepository = riskRepository;
    }

    public async Task<RiskDto> Handle(CreateRiskCommand request, CancellationToken cancellationToken)
    {
        // Generate risk code
        var riskCode = await GenerateRiskCode();

        // Create risk entity
        var risk = Risk.Create(
            riskCode,
            request.RiskData.Title,
            request.RiskData.Description,
            request.RiskData.Category,
            request.RiskData.InherentLikelihood,
            request.RiskData.InherentImpact,
            request.RiskData.OwnerId,
            request.RiskData.Department,
            request.RiskData.TreatmentStrategy,
            request.RiskData.RiskAppetite,
            request.CreatedBy
        );

        // Save to repository
        await _riskRepository.AddAsync(risk);
        await _riskRepository.SaveChangesAsync();

        // Return DTO
        return MapToDto(risk);
    }

    private async Task<string> GenerateRiskCode()
    {
        var count = await _riskRepository.GetCountAsync();
        return $"RISK-{DateTime.UtcNow.Year}-{(count + 1):D4}";
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
