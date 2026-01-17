using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Accounts.Commands.FreezeAccount;

/// <summary>
/// 📂 Wekeza.Core.Application/Features/Accounts/Commands/FreezeAccount
/// This vertical slice ensures that once the Model Risk Manager (you) or the Fraud Engine detects an anomaly, the account is locked instantly.
/// 1. The Intent: FreezeAccountCommand.cs
/// We use a record to ensure the command is immutable. We include the Reason for the freeze, which is a regulatory requirement for KYC/AML audits.
/// Command to administrative-lock an account. 
/// Prevents all outgoing transactions while allowing incoming credits (optional by policy).
/// </summary>
public record FreezeAccountCommand : ICommand<bool>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public string Reason { get; init; } = "Suspicious Activity Detected";
    public string AuthorizedBy { get; init; } = string.Empty; // Staff ID or System Service Name
}
