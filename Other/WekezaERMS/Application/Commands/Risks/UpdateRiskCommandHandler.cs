using AutoMapper;
using MediatR;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Commands.Risks;

public class UpdateRiskCommandHandler : IRequestHandler<UpdateRiskCommand, RiskDto?>
{
    private readonly IRiskRepository _riskRepository;
    private readonly IMapper _mapper;

    public UpdateRiskCommandHandler(IRiskRepository riskRepository, IMapper mapper)
    {
        _riskRepository = riskRepository;
        _mapper = mapper;
    }

    public async Task<RiskDto?> Handle(UpdateRiskCommand request, CancellationToken cancellationToken)
    {
        var risk = await _riskRepository.GetByIdAsync(request.Id);
        
        if (risk == null)
        {
            return null;
        }

        // Update risk using domain methods
        risk.Update(
            request.RiskData.Title,
            request.RiskData.Description,
            request.RiskData.Category,
            request.RiskData.Department,
            request.RiskData.TreatmentStrategy,
            request.RiskData.OwnerId,
            request.RiskData.RiskAppetite,
            request.RiskData.NextReviewDate,
            request.UpdatedBy
        );

        risk.Assess(
            request.RiskData.InherentLikelihood,
            request.RiskData.InherentImpact,
            request.UpdatedBy
        );

        await _riskRepository.UpdateAsync(risk);
        await _riskRepository.SaveChangesAsync();

        return _mapper.Map<RiskDto>(risk);
    }
}
