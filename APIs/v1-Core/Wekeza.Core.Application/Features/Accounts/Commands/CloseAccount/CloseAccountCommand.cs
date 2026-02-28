using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Accounts.Commands.CloseAccount;

/// <summary>
/// ðŸ“‚ Wekeza.Core.Application/Features/Accounts/Commands/CloseAccount
/// This feature ensures the bank's ledger remains balanced and no liabilities are left hanging in the air.
/// 1. The Intent: CloseAccountCommand.cs
/// We need the account number and a reason (e.g., "Customer Request", "Regulatory Order").
/// Intent to permanently close an account. 
/// Requires a zero balance and zero outstanding liabilities.
/// </summary>
public record CloseAccountCommand : ICommand<bool>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public string ClosureReason { get; init; } = string.Empty;
}
