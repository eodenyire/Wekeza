using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Exceptions;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.IssueCard;

public class IssueCardHandler : IRequestHandler<IssueCardCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IssueCardHandler(
        IAccountRepository accountRepository,
        ICardRepository cardRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(IssueCardCommand request, CancellationToken ct)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, ct)
            ?? throw new NotFoundException("Account", request.AccountId);

        // Domain Rule: Cannot issue cards to Frozen accounts
        if (account.IsFrozen)
            throw new GenericDomainException("Cannot issue a card to a frozen account.");

        // Determine daily withdrawal limit based on card type
        decimal dailyLimit = request.CardType.ToLower() switch
        {
            "debit" => 50_000,
            "credit" => 100_000,
            "prepaid" => 20_000,
            _ => 50_000
        };

        // Create the card
        var card = new Card(
            Guid.NewGuid(),
            account.Id,
            request.CardType,
            request.NameOnCard,
            dailyLimit
        );

        await _cardRepository.AddAsync(card, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return card.Id;
    }
}
