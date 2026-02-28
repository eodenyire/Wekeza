using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Administration.Commands.CreateRole;

public record CreateRoleCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string RoleName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> Permissions { get; init; } = new();
}
