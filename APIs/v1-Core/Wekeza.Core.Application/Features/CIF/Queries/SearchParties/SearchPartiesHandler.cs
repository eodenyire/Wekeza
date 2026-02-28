using MediatR;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CIF.Queries.SearchParties;

public class SearchPartiesHandler : IRequestHandler<SearchPartiesQuery, List<PartySearchResultDto>>
{
    private readonly IPartyRepository _partyRepository;

    public SearchPartiesHandler(IPartyRepository partyRepository)
    {
        _partyRepository = partyRepository;
    }

    public async Task<List<PartySearchResultDto>> Handle(SearchPartiesQuery request, CancellationToken cancellationToken)
    {
        var parties = await _partyRepository.SearchByNameAsync(request.SearchTerm, cancellationToken);

        return parties.Select(p => new PartySearchResultDto
        {
            PartyNumber = p.PartyNumber,
            PartyType = p.PartyType.ToString(),
            FullName = p.PartyType == Domain.Enums.PartyType.Individual
                ? $"{p.FirstName} {p.LastName}"
                : p.CompanyName ?? string.Empty,
            Email = p.PrimaryEmail,
            Phone = p.PrimaryPhone,
            Status = p.Status.ToString(),
            KYCStatus = p.KYCStatus.ToString(),
            RiskRating = p.RiskRating.ToString()
        }).ToList();
    }
}
