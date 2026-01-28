using AutoMapper;
using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Controls;

public class RecordControlTestCommandHandler : IRequestHandler<RecordControlTestCommand, ControlDto?>
{
    private readonly IControlRepository _controlRepository;
    private readonly IMapper _mapper;

    public RecordControlTestCommandHandler(IControlRepository controlRepository, IMapper mapper)
    {
        _controlRepository = controlRepository;
        _mapper = mapper;
    }

    public async Task<ControlDto?> Handle(RecordControlTestCommand request, CancellationToken cancellationToken)
    {
        var control = await _controlRepository.GetByIdAsync(request.Id);
        if (control == null)
        {
            return null;
        }

        // Record test using domain method
        control.UpdateEffectiveness(
            request.TestData.Effectiveness,
            request.TestData.TestingEvidence,
            request.UpdatedBy
        );

        await _controlRepository.UpdateAsync(control);
        await _controlRepository.SaveChangesAsync();

        return _mapper.Map<ControlDto>(control);
    }
}
