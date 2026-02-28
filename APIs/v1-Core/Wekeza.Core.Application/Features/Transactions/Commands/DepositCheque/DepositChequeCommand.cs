using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Transactions.Commands.DepositCheque;

/// <summary>
/// 1. The Intent: DepositChequeCommand.cs
/// We include the ChequeNumber, DrawerBank (who issued it), and ClearanceDays.
/// Intent to deposit a physical cheque. 
/// Funds are held in a 'Pending' state until the clearance period expires.
/// </summary>
public record DepositChequeCommand : ICommand<Guid>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string ChequeNumber { get; init; } = string.Empty;
    public string DrawerBank { get; init; } = string.Empty;
    public int ClearanceDays { get; init; } = 3; // Standard CBK clearing cycle
}
