using AutoMapper;
using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Controls;

public class UpdateControlCommandHandler : IRequestHandler<UpdateControlCommand, ControlDto?>
{
    private readonly IControlRepository _controlRepository;
    private readonly IMapper _mapper;

    public UpdateControlCommandHandler(IControlRepository controlRepository, IMapper mapper)
    {
        _controlRepository = controlRepository;
        _mapper = mapper;
    }

    public async Task<ControlDto?> Handle(UpdateControlCommand request, CancellationToken cancellationToken)
    {
        var control = await _controlRepository.GetByIdAsync(request.Id);
        if (control == null)
        {
            return null;
        }

        // Use domain method to update
        control.Update(
            request.ControlData.ControlName,
            request.ControlData.Description,
            request.ControlData.ControlType,
            request.ControlData.OwnerId,
            request.ControlData.TestingFrequency,
            request.UpdatedBy
        );

        await _controlRepository.UpdateAsync(control);
        await _controlRepository.SaveChangesAsync();

        return _mapper.Map<ControlDto>(control);
    }
}
