using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Controls;

public class UpdateControlCommand : IRequest<ControlDto?>
{
    public Guid Id { get; set; }
    public UpdateControlDto ControlData { get; set; } = null!;
    public Guid UpdatedBy { get; set; }
}
