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
        var cardType = request.CardType.ToLower() switch
        {
            "debit" => Domain.Aggregates.CardType.Debit,
            "credit" => Domain.Aggregates.CardType.Credit,
            "prepaid" => Domain.Aggregates.CardType.Prepaid,
            _ => Domain.Aggregates.CardType.Debit
        };

        var currency = account.Currency;
        var dailyWithdrawalLimit = new Domain.ValueObjects.Money(50_000, currency);
        var dailyPurchaseLimit = new Domain.ValueObjects.Money(100_000, currency);
        var monthlyLimit = new Domain.ValueObjects.Money(500_000, currency);

        // Create the card using factory method
        var card = Card.IssueCard(
            account.CustomerId,
            account.Id,
            cardType,
            request.NameOnCard,
            "Default Address", // TODO: Get from customer
            dailyWithdrawalLimit,
            dailyPurchaseLimit,
            monthlyLimit
        );

        await _cardRepository.AddAsync(card, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return card.Id;
    }
}
