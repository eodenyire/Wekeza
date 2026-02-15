using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.CIF.Queries.GetPendingKYC;

[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record GetPendingKYCQuery : IQuery<List<PendingKYCDto>>;

public record PendingKYCDto
{
    public string PartyNumber { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string PartyType { get; init; } = string.Empty;
    public string KYCStatus { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
    public int DaysPending { get; init; }
}
