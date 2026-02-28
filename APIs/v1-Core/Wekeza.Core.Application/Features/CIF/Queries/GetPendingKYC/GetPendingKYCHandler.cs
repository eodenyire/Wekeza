using MediatR;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CIF.Queries.GetPendingKYC;

public class GetPendingKYCHandler : IRequestHandler<GetPendingKYCQuery, List<PendingKYCDto>>
{
    private readonly IPartyRepository _partyRepository;

    public GetPendingKYCHandler(IPartyRepository partyRepository)
    {
        _partyRepository = partyRepository;
    }

    public async Task<List<PendingKYCDto>> Handle(GetPendingKYCQuery request, CancellationToken cancellationToken)
    {
        var parties = await _partyRepository.GetPendingKYCAsync(cancellationToken);

        return parties.Select(p => new PendingKYCDto
        {
            PartyNumber = p.PartyNumber,
            FullName = p.PartyType == Domain.Enums.PartyType.Individual
                ? $"{p.FirstName} {p.LastName}"
                : p.CompanyName ?? string.Empty,
            PartyType = p.PartyType.ToString(),
            KYCStatus = p.KYCStatus.ToString(),
            CreatedDate = p.CreatedDate,
            DaysPending = (DateTime.UtcNow - p.CreatedDate).Days
        }).ToList();
    }
}
