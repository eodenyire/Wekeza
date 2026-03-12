using AutoMapper;
using MediatR;
using WekezaERMS.Application.Commands.Risks;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Queries.Risks;

public class GetRiskByIdQueryHandler : IRequestHandler<GetRiskByIdQuery, RiskDto?>
{
    private readonly IRiskRepository _riskRepository;
    private readonly IMapper _mapper;

    public GetRiskByIdQueryHandler(IRiskRepository riskRepository, IMapper mapper)
    {
        _riskRepository = riskRepository;
        _mapper = mapper;
    }

    public async Task<RiskDto?> Handle(GetRiskByIdQuery request, CancellationToken cancellationToken)
    {
        var risk = await _riskRepository.GetByIdAsync(request.Id);
        
        if (risk == null)
        {
            return null;
        }

        return _mapper.Map<RiskDto>(risk);
    }
}
