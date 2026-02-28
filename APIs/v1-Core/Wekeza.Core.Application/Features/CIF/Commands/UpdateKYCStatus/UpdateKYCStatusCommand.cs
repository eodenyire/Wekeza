using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.CIF.Commands.UpdateKYCStatus;

/// <summary>
/// Command to update KYC status after verification
/// </summary>
[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record UpdateKYCStatusCommand : ICommand<bool>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string PartyNumber { get; init; } = string.Empty;
    public KYCStatus NewStatus { get; init; }
    public string? Remarks { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public List<string>? VerifiedDocuments { get; init; }
}
