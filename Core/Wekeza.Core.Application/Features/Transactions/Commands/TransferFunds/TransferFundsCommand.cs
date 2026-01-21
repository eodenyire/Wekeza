using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Transactions.Commands.TransferFunds;

/// <summary>
/// ðŸ“‚ Wekeza.Core.Application/Features/Transactions/Commands/TransferFunds
/// This vertical slice handles the core movement of value within the bank.
/// 1. The Intent: TransferFundsCommand.cs
/// We include the CorrelationId and Description. Future-proofing: We include a ScheduledFor property (optional) for standing orders, making the system ready for 2026's advanced banking features.
/// Represents the intent to move value between two accounts within Wekeza Bank.
/// </summary>
public record TransferFundsCommand : ICommand<Guid>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string FromAccountNumber { get; init; } = string.Empty;
    public string ToAccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string Description { get; init; } = "Internal Transfer";
}
