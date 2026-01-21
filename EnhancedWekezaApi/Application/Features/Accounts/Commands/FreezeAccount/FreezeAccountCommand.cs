using EnhancedWekezaApi.Domain.Common;

namespace EnhancedWekezaApi.Application.Features.Accounts.Commands.FreezeAccount;

/// <summary>
/// Intent to freeze an account (prevent all transactions).
/// Used for compliance, security, or regulatory purposes.
/// </summary>
public record FreezeAccountCommand : ICommand<bool>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public string FreezeReason { get; init; } = string.Empty;
}