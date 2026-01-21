using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Accounts.Commands.AddSignatory;

public record AddSignatoryCommand : ICommand<bool>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid AccountId { get; init; }
    public string SignatoryName { get; init; } = string.Empty;
    public string IdNumber { get; init; } = string.Empty;
    public string Role { get; init; } = "Approver"; // Initiator, Approver, Viewer
    public decimal SignatureLimit { get; init; } // Max amount this person can authorize alone
}
