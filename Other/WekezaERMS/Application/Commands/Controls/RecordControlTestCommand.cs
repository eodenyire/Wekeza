using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Controls;

public class RecordControlTestCommand : IRequest<ControlDto?>
{
    public Guid Id { get; set; }
    public ControlTestDto TestData { get; set; } = null!;
    public Guid UpdatedBy { get; set; }
}
