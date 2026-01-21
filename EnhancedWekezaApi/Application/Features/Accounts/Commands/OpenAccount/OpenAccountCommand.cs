using EnhancedWekezaApi.Domain.Common;
using EnhancedWekezaApi.Application.Features.Accounts.Queries.GetAccount;

namespace EnhancedWekezaApi.Application.Features.Accounts.Commands.OpenAccount;

/// <summary>
/// The intent to onboard a new customer and open their first account.
/// </summary>
public record OpenAccountCommand : ICommand<AccountDto>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string IdentificationNumber { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = "KES"; // Default to home currency
    public decimal InitialDeposit { get; init; }
}