using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CIF.Commands.UpdateKYCStatus;

public class UpdateKYCStatusHandler : IRequestHandler<UpdateKYCStatusCommand, bool>
{
    private readonly IPartyRepository _partyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateKYCStatusHandler(
        IPartyRepository partyRepository,
        IUnitOfWork unitOfWork)
    {
        _partyRepository = partyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateKYCStatusCommand request, CancellationToken cancellationToken)
    {
        var party = await _partyRepository.GetByPartyNumberAsync(request.PartyNumber, cancellationToken);
        
        if (party == null)
        {
            throw new NotFoundException($"Party with number {request.PartyNumber} not found.");
        }

        // Update KYC status
        party.UpdateKYCStatus(request.NewStatus, request.ExpiryDate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
