using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccount;

namespace Wekeza.Core.Application.Features.Accounts.Commands.OpenAccount;

/// <summary>
/// 📂 Wekeza.Core.Application/Features/Accounts
/// We are starting with the OpenAccount vertical slice. This is our first end-to-end mission.
/// 1. The Request Structure: Commands/OpenAccount/OpenAccountCommand.cs
/// This defines the "Intent." We use a record for immutability. Every request must carry the CorrelationId we defined in our ICommand interface.
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
