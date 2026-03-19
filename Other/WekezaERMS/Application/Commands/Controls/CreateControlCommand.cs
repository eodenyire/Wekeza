using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Controls;

public class CreateControlCommand : IRequest<ControlDto>
{
    public Guid RiskId { get; set; }
    public CreateControlDto ControlData { get; set; } = null!;
    public Guid CreatedBy { get; set; }
}
