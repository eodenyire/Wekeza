using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Transactions.Commands.WithdrawFunds;

/// <summary>
/// 1. The Intent: WithdrawFundsCommand.cs
/// We include Channel and TerminalId (ATM ID or Branch ID). This is crucial for tracking where the money exited the bank.
/// Intent to withdraw physical cash from an account.
/// Subject to balance checks and daily transaction limits.
/// </summary>
public record WithdrawFundsCommand : ICommand<Guid>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string Channel { get; init; } = "ATM"; // ATM, Branch, Agent
    public string TerminalId { get; init; } = string.Empty; // Unique ID of the ATM or POS
}
