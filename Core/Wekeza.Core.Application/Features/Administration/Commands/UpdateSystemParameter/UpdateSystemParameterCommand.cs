using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Administration.Commands.UpdateSystemParameter;

public record UpdateSystemParameterCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string ParameterKey { get; init; } = string.Empty;
    public string ParameterValue { get; init; } = string.Empty;
}
