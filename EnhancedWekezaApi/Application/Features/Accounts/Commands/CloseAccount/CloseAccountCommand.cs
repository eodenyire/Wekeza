using EnhancedWekezaApi.Domain.Common;

namespace EnhancedWekezaApi.Application.Features.Accounts.Commands.CloseAccount;

/// <summary>
/// Intent to permanently close an account. 
/// Requires a zero balance and zero outstanding liabilities.
/// </summary>
public record CloseAccountCommand : ICommand<bool>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public string ClosureReason { get; init; } = string.Empty;
}