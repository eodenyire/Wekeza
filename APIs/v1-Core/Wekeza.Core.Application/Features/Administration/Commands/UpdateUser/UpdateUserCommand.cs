using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Administration.Commands.UpdateUser;

public record UpdateUserCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Department { get; init; }
    public string? JobTitle { get; init; }
    public Guid? BranchId { get; init; }
    public bool? IsActive { get; init; }
}
