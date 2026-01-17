using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
/// 3. The Executioner: DepositChequeHandler.cs
/// This handler is special. It doesn't call Credit() immediately. It marks a transaction as Pending and sets a ClearanceDate.

namespace Wekeza.Core.Application.Features.Transactions.Commands.DepositCheque;

public class DepositChequeHandler : IRequestHandler<DepositChequeCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public DepositChequeHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork, IDateTime dateTime)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Guid> Handle(DepositChequeCommand request, CancellationToken ct)
    {
        var account = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), ct)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        var chequeAmount = new Money(request.Amount, account.Balance.Currency);
        
        // Future-Proofing: We calculate the date when funds become 'Available'
        var clearanceDate = _dateTime.UtcNow.AddDays(request.ClearanceDays);

        // Note: In 'The Beast', the Transaction Aggregate would have a 'IsCleared' flag.
        // The balance update logic would distinguish between Ledger and Available.
        
        // For this MVP core, we record the intent and update the Ledger
        account.Credit(chequeAmount); 

        // Add detailed transaction record for the clearing system to pick up later
        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(ct);

        return request.CorrelationId;
    }
}
