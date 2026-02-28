using Microsoft.Extensions.Logging;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Application.Features.CIF.Commands.PerformAMLScreening;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Infrastructure.Services;

/// <summary>
/// AML Screening Service Implementation
/// In production, this would integrate with providers like:
/// - Dow Jones Risk & Compliance
/// - Refinitiv World-Check
/// - LexisNexis Bridger
/// - ComplyAdvantage
/// </summary>
public class AMLScreeningService : IAMLScreeningService
{
    private readonly ILogger<AMLScreeningService> _logger;

    public AMLScreeningService(ILogger<AMLScreeningService> logger)
    {
        _logger = logger;
    }

    public async Task<AMLScreeningResult> ScreenPartyAsync(
        Party party, 
        PerformAMLScreeningCommand request, 
        CancellationToken ct = default)
    {
        _logger.LogInformation("Performing AML screening for party: {PartyNumber}", party.PartyNumber);

        var matches = new List<ScreeningMatch>();
        var isSanctioned = false;
        var isPEP = false;
        var hasAdverseMedia = false;

        // Get party name
        var partyName = party.PartyType == PartyType.Individual
            ? $"{party.FirstName} {party.LastName}"
            : party.CompanyName;

        // 1. Sanctions Screening
        if (request.CheckSanctions)
        {
            isSanctioned = await CheckSanctionsAsync(partyName!, party.Nationality, ct);
            if (isSanctioned)
            {
                matches.Add(new ScreeningMatch
                {
                    MatchType = "Sanctions",
                    ListName = "OFAC SDN List",
                    MatchedName = partyName!,
                    ConfidenceScore = 95,
                    Details = "Matched against OFAC Specially Designated Nationals list"
                });
            }
        }

        // 2. PEP Screening
        if (request.CheckPEP && party.PartyType == PartyType.Individual)
        {
            isPEP = await CheckPEPAsync(partyName!, party.Nationality, ct);
            if (isPEP)
            {
                matches.Add(new ScreeningMatch
                {
                    MatchType = "PEP",
                    ListName = "Global PEP Database",
                    MatchedName = partyName!,
                    ConfidenceScore = 85,
                    Details = "Identified as Politically Exposed Person"
                });
            }
        }

        // 3. Adverse Media Screening
        if (request.CheckAdverseMedia)
        {
            hasAdverseMedia = await CheckAdverseMediaAsync(partyName!, ct);
            if (hasAdverseMedia)
            {
                matches.Add(new ScreeningMatch
                {
                    MatchType = "AdverseMedia",
                    ListName = "Global News Sources",
                    MatchedName = partyName!,
                    ConfidenceScore = 70,
                    Details = "Found adverse media mentions related to financial crimes"
                });
            }
        }

        // 4. Determine risk rating
        var riskRating = DetermineRiskRating(isSanctioned, isPEP, hasAdverseMedia, party);

        // 5. Build result
        var result = new AMLScreeningResult
        {
            IsClear = !isSanctioned && !isPEP && !hasAdverseMedia,
            IsSanctioned = isSanctioned,
            IsPEP = isPEP,
            HasAdverseMedia = hasAdverseMedia,
            RecommendedRiskRating = riskRating,
            Matches = matches,
            Summary = BuildSummary(isSanctioned, isPEP, hasAdverseMedia)
        };

        _logger.LogInformation(
            "AML screening completed for {PartyNumber}. Clear: {IsClear}, Risk: {RiskRating}",
            party.PartyNumber, result.IsClear, riskRating);

        return result;
    }

    public async Task<bool> CheckSanctionsAsync(string name, string? nationality = null, CancellationToken ct = default)
    {
        // TODO: Integrate with actual sanctions screening API
        // For now, simulate with mock data
        await Task.Delay(100, ct); // Simulate API call

        // Mock: Check against known sanctioned names
        var sanctionedNames = new[] { "sanctioned", "blocked", "prohibited" };
        return sanctionedNames.Any(s => name.ToLower().Contains(s));
    }

    public async Task<bool> CheckPEPAsync(string name, string? nationality = null, CancellationToken ct = default)
    {
        // TODO: Integrate with PEP database API
        await Task.Delay(100, ct);

        // Mock: Check against known PEP indicators
        var pepIndicators = new[] { "minister", "senator", "governor", "president" };
        return pepIndicators.Any(p => name.ToLower().Contains(p));
    }

    public async Task<bool> CheckAdverseMediaAsync(string name, CancellationToken ct = default)
    {
        // TODO: Integrate with adverse media screening API
        await Task.Delay(100, ct);

        // Mock: Random adverse media check
        return false; // Most parties won't have adverse media
    }

    public async Task PerformOngoingMonitoringAsync(CancellationToken ct = default)
    {
        // TODO: Implement ongoing monitoring
        // This would run as a background job to re-screen existing parties
        _logger.LogInformation("Performing ongoing AML monitoring");
        await Task.CompletedTask;
    }

    private RiskRating DetermineRiskRating(bool isSanctioned, bool isPEP, bool hasAdverseMedia, Party party)
    {
        if (isSanctioned)
            return RiskRating.Prohibited;

        if (isPEP && hasAdverseMedia)
            return RiskRating.VeryHigh;

        if (isPEP || hasAdverseMedia)
            return RiskRating.High;

        // Consider other factors
        if (party.PartyType == PartyType.Corporate)
            return RiskRating.Medium;

        return RiskRating.Low;
    }

    private string BuildSummary(bool isSanctioned, bool isPEP, bool hasAdverseMedia)
    {
        if (isSanctioned)
            return "CRITICAL: Party appears on sanctions list. Account opening prohibited.";

        if (isPEP && hasAdverseMedia)
            return "HIGH RISK: Party is a PEP with adverse media mentions. Enhanced due diligence required.";

        if (isPEP)
            return "HIGH RISK: Party is a Politically Exposed Person. Enhanced due diligence required.";

        if (hasAdverseMedia)
            return "MEDIUM RISK: Adverse media mentions found. Additional verification recommended.";

        return "CLEAR: No adverse findings. Standard due diligence applies.";
    }
}
