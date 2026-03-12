using AutoMapper;
using MediatR;
using WekezaERMS.Application.Commands.Controls;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Queries.Controls;

public class GetControlsByRiskIdQueryHandler : IRequestHandler<GetControlsByRiskIdQuery, List<ControlDto>>
{
    private readonly IControlRepository _controlRepository;
    private readonly IMapper _mapper;

    public GetControlsByRiskIdQueryHandler(IControlRepository controlRepository, IMapper mapper)
    {
        _controlRepository = controlRepository;
        _mapper = mapper;
    }

    public async Task<List<ControlDto>> Handle(GetControlsByRiskIdQuery request, CancellationToken cancellationToken)
    {
        var controls = await _controlRepository.GetByRiskIdAsync(request.RiskId);
        return _mapper.Map<List<ControlDto>>(controls);
    }
}
