using MediatR;

namespace WekezaERMS.Application.Commands.Controls;

public class DeleteControlCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid DeletedBy { get; set; }
}
