using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.WithdrawFromAtm;

public record WithdrawFromAtmCommand : ICommand<Guid>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CardId { get; init; }
    public decimal Amount { get; init; }
    public string TerminalId { get; init; } = string.Empty; // Location of the ATM
    public string Currency { get; init; } = "KES";
}
