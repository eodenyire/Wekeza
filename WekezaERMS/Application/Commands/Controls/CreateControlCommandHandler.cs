using AutoMapper;
using MediatR;
using WekezaERMS.Application.Commands.Risks;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Commands.Controls;

public class CreateControlCommandHandler : IRequestHandler<CreateControlCommand, ControlDto>
{
    private readonly IControlRepository _controlRepository;
    private readonly IRiskRepository _riskRepository;
    private readonly IMapper _mapper;

    public CreateControlCommandHandler(
        IControlRepository controlRepository,
        IRiskRepository riskRepository,
        IMapper mapper)
    {
        _controlRepository = controlRepository;
        _riskRepository = riskRepository;
        _mapper = mapper;
    }

    public async Task<ControlDto> Handle(CreateControlCommand request, CancellationToken cancellationToken)
    {
        // Verify risk exists
        var risk = await _riskRepository.GetByIdAsync(request.RiskId);
        if (risk == null)
        {
            throw new InvalidOperationException($"Risk with ID {request.RiskId} not found");
        }

        // Create control entity
        var control = RiskControl.Create(
            request.RiskId,
            request.ControlData.ControlName,
            request.ControlData.Description,
            request.ControlData.ControlType,
            request.ControlData.OwnerId,
            request.ControlData.TestingFrequency,
            request.CreatedBy
        );

        // Save to repository
        await _controlRepository.AddAsync(control);
        await _controlRepository.SaveChangesAsync();

        // Return DTO using AutoMapper
        return _mapper.Map<ControlDto>(control);
    }
}
