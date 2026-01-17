using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
///
/// 3. The Executioner: DepositFundsHandler.cs
/// This handler is clean because the Account aggregate handles the math. It marks the transaction as a Credit.
///

namespace Wekeza.Core.Application.Features.Transactions.Commands.DepositFunds;

public class DepositFundsHandler : IRequestHandler<DepositFundsCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepositFundsHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DepositFundsCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch the Target Account
        var account = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        // 2. Wrap Amount into Value Object
        var depositAmount = new Money(request.Amount, Currency.FromCode(request.Currency));

        // 3. Domain Logic: Execute Credit
        // This validates if the account is frozen and updates the balance
        account.Credit(depositAmount);

        // 4. Record the specific Transaction entity (The Ledger entry)
        // This is what the 'GetStatement' query will read later
        var transaction = new Transaction(
            Guid.NewGuid(),
            request.CorrelationId,
            account.Id,
            depositAmount,
            TransactionType.Deposit,
            $"[{request.Channel}] {request.Description} - Ref: {request.ExternalReference}"
        );

        // Note: In 'The Beast', the repository would handle adding this transaction record
        _accountRepository.Update(account);

        // 5. Atomic Save
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return request.CorrelationId;
    }
}
