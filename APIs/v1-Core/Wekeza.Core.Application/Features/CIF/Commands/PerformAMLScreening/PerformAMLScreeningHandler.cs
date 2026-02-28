using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CIF.Commands.PerformAMLScreening;

public class PerformAMLScreeningHandler : IRequestHandler<PerformAMLScreeningCommand, AMLScreeningResult>
{
    private readonly IPartyRepository _partyRepository;
    private readonly IAMLScreeningService _amlService;
    private readonly IUnitOfWork _unitOfWork;

    public PerformAMLScreeningHandler(
        IPartyRepository partyRepository,
        IAMLScreeningService amlService,
        IUnitOfWork unitOfWork)
    {
        _partyRepository = partyRepository;
        _amlService = amlService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AMLScreeningResult> Handle(PerformAMLScreeningCommand request, CancellationToken ct)
    {
        // 1. Get party
        var party = await _partyRepository.GetByPartyNumberAsync(request.PartyNumber, ct)
            ?? throw new NotFoundException("Party", request.PartyNumber);

        // 2. Perform screening
        var result = await _amlService.ScreenPartyAsync(party, request, ct);

        // 3. Update party based on results
        if (result.IsSanctioned)
        {
            party.MarkAsSanctioned();
        }

        if (result.IsPEP)
        {
            party.MarkAsPEP();
        }

        // 4. Update risk rating
        party.UpdateRiskRating(result.RecommendedRiskRating);

        // 5. Save changes
        _partyRepository.Update(party);
        await _unitOfWork.SaveChangesAsync(ct);

        return result;
    }
}
