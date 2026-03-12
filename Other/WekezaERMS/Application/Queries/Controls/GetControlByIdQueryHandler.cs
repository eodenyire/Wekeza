using AutoMapper;
using MediatR;
using WekezaERMS.Application.Commands.Controls;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Queries.Controls;

public class GetControlByIdQueryHandler : IRequestHandler<GetControlByIdQuery, ControlDto?>
{
    private readonly IControlRepository _controlRepository;
    private readonly IMapper _mapper;

    public GetControlByIdQueryHandler(IControlRepository controlRepository, IMapper mapper)
    {
        _controlRepository = controlRepository;
        _mapper = mapper;
    }

    public async Task<ControlDto?> Handle(GetControlByIdQuery request, CancellationToken cancellationToken)
    {
        var control = await _controlRepository.GetByIdAsync(request.Id);
        return control == null ? null : _mapper.Map<ControlDto>(control);
    }
}
