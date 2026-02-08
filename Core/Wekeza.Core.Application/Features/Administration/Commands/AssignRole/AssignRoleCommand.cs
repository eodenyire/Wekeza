using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Administration.Commands.AssignRole;

public record AssignRoleCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public Guid RoleId { get; init; }
}
