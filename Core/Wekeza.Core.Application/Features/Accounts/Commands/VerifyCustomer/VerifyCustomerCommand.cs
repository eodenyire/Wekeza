using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Accounts.Commands.VerifyCustomer;

/// <summary>
/// 📂 Wekeza.Core.Application/Features/Accounts/Commands/VerifyCustomer
/// 1. The Intent: VerifyCustomerCommand.cs
/// We capture the VerifierId (the staff member or AI bot) and the VerificationSource (e.g., "IPRS", "Manual Document Review").
/// Transition a customer to 'Verified' status. 
/// Necessary for unlocking full banking capabilities and compliance.
/// </summary>
public record VerifyCustomerCommand : ICommand<bool>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public string VerificationSource { get; init; } = "IPRS"; // Internal/External source
    public string VerifiedBy { get; init; } = string.Empty; // Staff/System ID
    public string Remarks { get; init; } = "ID and Bio-data matched.";
}
