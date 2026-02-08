using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Administration.Commands.CreateBranch;

public record CreateBranchCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string BranchCode { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string? City { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
}
