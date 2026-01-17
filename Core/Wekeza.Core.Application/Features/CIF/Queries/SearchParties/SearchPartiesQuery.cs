using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.CIF.Queries.SearchParties;

[Authorize(UserRole.Teller, UserRole.RiskOfficer, UserRole.Administrator)]
public record SearchPartiesQuery : IQuery<List<PartySearchResultDto>>
{
    public string SearchTerm { get; init; } = string.Empty;
}

public record PartySearchResultDto
{
    public string PartyNumber { get; init; } = string.Empty;
    public string PartyType { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Status { get; init; } = string.Empty;
    public string KYCStatus { get; init; } = string.Empty;
    public string RiskRating { get; init; } = string.Empty;
}
