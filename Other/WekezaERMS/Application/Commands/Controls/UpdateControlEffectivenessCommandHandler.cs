using AutoMapper;
using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Controls;

public class UpdateControlEffectivenessCommandHandler : IRequestHandler<UpdateControlEffectivenessCommand, ControlDto?>
{
    private readonly IControlRepository _controlRepository;
    private readonly IMapper _mapper;

    public UpdateControlEffectivenessCommandHandler(IControlRepository controlRepository, IMapper mapper)
    {
        _controlRepository = controlRepository;
        _mapper = mapper;
    }

    public async Task<ControlDto?> Handle(UpdateControlEffectivenessCommand request, CancellationToken cancellationToken)
    {
        var control = await _controlRepository.GetByIdAsync(request.Id);
        if (control == null)
        {
            return null;
        }

        // Update effectiveness using domain method
        control.UpdateEffectiveness(
            request.EffectivenessData.Effectiveness,
            request.EffectivenessData.TestingEvidence,
            request.UpdatedBy
        );

        await _controlRepository.UpdateAsync(control);
        await _controlRepository.SaveChangesAsync();

        return _mapper.Map<ControlDto>(control);
    }
}
