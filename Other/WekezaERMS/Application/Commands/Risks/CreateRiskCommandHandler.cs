using AutoMapper;
using MediatR;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Commands.Risks;

public class CreateRiskCommandHandler : IRequestHandler<CreateRiskCommand, RiskDto>
{
    private readonly IRiskRepository _riskRepository;
    private readonly IMapper _mapper;

    public CreateRiskCommandHandler(IRiskRepository riskRepository, IMapper mapper)
    {
        _riskRepository = riskRepository;
        _mapper = mapper;
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

        // Return DTO using AutoMapper
        return _mapper.Map<RiskDto>(risk);
    }

    private async Task<string> GenerateRiskCode()
    {
        var count = await _riskRepository.GetCountAsync();
        return $"RISK-{DateTime.UtcNow.Year}-{(count + 1):D4}";
    }
}
