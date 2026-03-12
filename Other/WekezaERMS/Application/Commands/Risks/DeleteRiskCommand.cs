using MediatR;

namespace WekezaERMS.Application.Commands.Risks;

public class DeleteRiskCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid DeletedBy { get; set; }
}
