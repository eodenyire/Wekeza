using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessChequeDeposit;

public record ProcessChequeDepositCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string ChequeNumber { get; init; } = string.Empty;
    public string DrawerBank { get; init; } = string.Empty;
}
