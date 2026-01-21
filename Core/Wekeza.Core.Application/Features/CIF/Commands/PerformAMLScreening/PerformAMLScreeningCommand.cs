using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.CIF.Commands.PerformAMLScreening;

/// <summary>
/// Command to perform AML/CFT screening on a party
/// Checks against sanctions lists (OFAC, UN, EU), PEP databases
/// </summary>
[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record PerformAMLScreeningCommand : ICommand<AMLScreeningResult>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string PartyNumber { get; init; } = string.Empty;
    public bool CheckSanctions { get; init; } = true;
    public bool CheckPEP { get; init; } = true;
    public bool CheckAdverseMedia { get; init; } = true;
}

public record AMLScreeningResult
{
    public bool IsClear { get; init; }
    public bool IsSanctioned { get; init; }
    public bool IsPEP { get; init; }
    public bool HasAdverseMedia { get; init; }
    public RiskRating RecommendedRiskRating { get; init; }
    public List<ScreeningMatch> Matches { get; init; } = new();
    public string Summary { get; init; } = string.Empty;
}

public record ScreeningMatch
{
    public string MatchType { get; init; } = string.Empty; // Sanctions, PEP, AdverseMedia
    public string ListName { get; init; } = string.Empty; // OFAC, UN, EU, etc.
    public string MatchedName { get; init; } = string.Empty;
    public int ConfidenceScore { get; init; } // 0-100
    public string Details { get; init; } = string.Empty;
}
