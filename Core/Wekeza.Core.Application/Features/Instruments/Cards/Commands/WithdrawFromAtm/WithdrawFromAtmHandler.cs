using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Aggregates;
using MediatR;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.WithdrawFromAtm;

public class WithdrawFromAtmHandler : IRequestHandler<WithdrawFromAtmCommand, Guid>
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawFromAtmHandler(
        ICardRepository cardRepository,
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(WithdrawFromAtmCommand request, CancellationToken ct)
    {
        // 1. Fetch and Validate Card
        var card = await _cardRepository.GetByIdAsync(request.CardId, ct)
            ?? throw new NotFoundException("Card", request.CardId);

        if (card.IsCancelled) 
            throw new ForbiddenAccessException("This card has been hot-listed.");

        // 2. Risk Check: Validate Daily ATM Limit
        if (!card.CanWithdraw(request.Amount))
            throw new DomainException("Daily withdrawal limit exceeded.", "LIMIT_EXCEEDED");

        // 3. Fetch Linked Account and Debit
        var account = await _accountRepository.GetByIdAsync(card.AccountId, ct)
            ?? throw new NotFoundException("Linked Account", card.AccountId);

        var withdrawalAmount = new Money(request.Amount, Currency.FromCode(request.Currency));
        
        // Debit the account (This checks for Insufficient Funds)
        account.Debit(withdrawalAmount);

        // 4. Record withdrawal on card
        card.RecordWithdrawal(request.Amount);

        // 5. Record Transaction with ATM Metadata
        var transaction = new Transaction(
            Guid.NewGuid(),
            request.CorrelationId,
            account.Id,
            withdrawalAmount,
            TransactionType.Withdrawal,
            $"ATM Withdrawal | Terminal: {request.TerminalId}"
        );

        await _transactionRepository.AddAsync(transaction, ct);
        _cardRepository.Update(card);
        _accountRepository.Update(account);
        
        await _unitOfWork.SaveChangesAsync(ct);
        return transaction.Id;
    }
}
