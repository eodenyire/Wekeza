using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessAccountOpening;

public record ProcessAccountOpeningCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public decimal InitialDeposit { get; init; }
}
