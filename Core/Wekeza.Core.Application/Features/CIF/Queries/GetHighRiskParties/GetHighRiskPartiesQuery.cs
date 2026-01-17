using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.CIF.Queries.GetHighRiskParties;

[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record GetHighRiskPartiesQuery : IQuery<List<HighRiskPartyDto>>;

public record HighRiskPartyDto
{
    public string PartyNumber { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string PartyType { get; init; } = string.Empty;
    public string RiskRating { get; init; } = string.Empty;
    public bool IsPEP { get; init; }
    public bool IsSanctioned { get; init; }
    public string KYCStatus { get; init; } = string.Empty;
    public List<string> RiskFlags { get; init; } = new();
}
