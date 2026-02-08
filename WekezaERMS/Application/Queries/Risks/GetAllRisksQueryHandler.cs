using AutoMapper;
using MediatR;
using WekezaERMS.Application.Commands.Risks;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Queries.Risks;

public class GetAllRisksQueryHandler : IRequestHandler<GetAllRisksQuery, List<RiskDto>>
{
    private readonly IRiskRepository _riskRepository;
    private readonly IMapper _mapper;

    public GetAllRisksQueryHandler(IRiskRepository riskRepository, IMapper mapper)
    {
        _riskRepository = riskRepository;
        _mapper = mapper;
    }

    public async Task<List<RiskDto>> Handle(GetAllRisksQuery request, CancellationToken cancellationToken)
    {
        var risks = await _riskRepository.GetAllAsync();
        return _mapper.Map<List<RiskDto>>(risks);
    }
}
