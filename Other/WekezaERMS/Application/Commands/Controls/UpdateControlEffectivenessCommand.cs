using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Controls;

public class UpdateControlEffectivenessCommand : IRequest<ControlDto?>
{
    public Guid Id { get; set; }
    public ControlEffectivenessDto EffectivenessData { get; set; } = null!;
    public Guid UpdatedBy { get; set; }
}
