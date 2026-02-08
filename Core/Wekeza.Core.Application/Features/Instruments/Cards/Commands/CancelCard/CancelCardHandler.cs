using MediatR;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.CancelCard;

public class CancelCardHandler : IRequestHandler<CancelCardCommand, bool>
{
    private readonly ICardRepository _cardRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelCardHandler(ICardRepository cardRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(CancelCardCommand request, CancellationToken ct)
    {
        // 1. Locate the Card
        var card = await _cardRepository.GetByIdAsync(request.CardId, ct)
            ?? throw new NotFoundException("Card", request.CardId);

        // 2. State Transition: Set Status to 'Cancelled' or 'Hotlisted'
        card.Cancel(request.Reason, _currentUserService.Username ?? "System");

        // 3. Persist changes
        _cardRepository.Update(card);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return true;
    }
}
