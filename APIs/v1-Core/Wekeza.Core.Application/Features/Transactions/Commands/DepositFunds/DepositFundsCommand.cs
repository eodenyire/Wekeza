using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Transactions.Commands.DepositFunds;

/// <summary>
/// ðŸ“‚ Wekeza.Core.Application/Features/Transactions/Commands/DepositFunds
/// 1. The Intent: DepositFundsCommand.cs
/// We include a Source and Reference. For 2026, we ensure we can handle third-party transaction IDs (like an M-Pesa SQR... code) to prevent duplicate processing.
/// Command to deposit funds into a Wekeza account.
/// Supports multiple channels: Cash, MobileMoney, WireTransfer.
/// </summary>
public record DepositFundsCommand : ICommand<Guid>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string Channel { get; init; } = "Cash"; // Cash, M-Pesa, Swift
    public string ExternalReference { get; init; } = string.Empty; // e.g., M-Pesa Receipt Number
    public string Description { get; init; } = "Deposit";
}
