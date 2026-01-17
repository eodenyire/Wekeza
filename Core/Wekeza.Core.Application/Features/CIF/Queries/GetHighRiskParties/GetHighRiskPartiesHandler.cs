using MediatR;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CIF.Queries.GetHighRiskParties;

public class GetHighRiskPartiesHandler : IRequestHandler<GetHighRiskPartiesQuery, List<HighRiskPartyDto>>
{
    private readonly IPartyRepository _partyRepository;

    public GetHighRiskPartiesHandler(IPartyRepository partyRepository)
    {
        _partyRepository = partyRepository;
    }

    public async Task<List<HighRiskPartyDto>> Handle(GetHighRiskPartiesQuery request, CancellationToken cancellationToken)
    {
        var parties = await _partyRepository.GetHighRiskPartiesAsync(cancellationToken);

        return parties.Select(p =>
        {
            var riskFlags = new List<string>();
            if (p.IsPEP) riskFlags.Add("PEP");
            if (p.IsSanctioned) riskFlags.Add("Sanctioned");
            if (p.KYCStatus == Domain.Enums.KYCStatus.Expired) riskFlags.Add("KYC Expired");
            if (p.RiskRating == Domain.Enums.RiskRating.VeryHigh) riskFlags.Add("Very High Risk");

            return new HighRiskPartyDto
            {
                PartyNumber = p.PartyNumber,
                FullName = p.PartyType == Domain.Enums.PartyType.Individual
                    ? $"{p.FirstName} {p.LastName}"
                    : p.CompanyName ?? string.Empty,
                PartyType = p.PartyType.ToString(),
                RiskRating = p.RiskRating.ToString(),
                IsPEP = p.IsPEP,
                IsSanctioned = p.IsSanctioned,
                KYCStatus = p.KYCStatus.ToString(),
                RiskFlags = riskFlags
            };
        }).ToList();
    }
}
