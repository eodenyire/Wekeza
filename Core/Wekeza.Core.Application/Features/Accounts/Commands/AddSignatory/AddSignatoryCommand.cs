using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Accounts.Commands.AddSignatory;

public record AddSignatoryCommand : ICommand<bool>
{
    public Guid AccountId { get; init; }
    public string SignatoryName { get; init; } = string.Empty;
    public string IdNumber { get; init; } = string.Empty;
    public string Role { get; init; } = "Approver"; // Initiator, Approver, Viewer
    public decimal SignatureLimit { get; init; } // Max amount this person can authorize alone
}
