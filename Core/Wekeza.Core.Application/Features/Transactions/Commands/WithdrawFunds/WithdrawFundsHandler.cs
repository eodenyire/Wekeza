using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Exceptions;
///
/// 3. The Executioner: WithdrawFundsHandler.cs
/// This is where we implement the Principal-Grade Withdrawal Logic. It checks the Balance (via the Domain Entity) and would realistically check a Daily Limit Service.
namespace Wekeza.Core.Application.Features.Transactions.Commands.WithdrawFunds;

public class WithdrawFundsHandler : IRequestHandler<WithdrawFundsCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawFundsHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(WithdrawFundsCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Aggregate
        var account = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        // 2. Risk Check: Daily Limit (Future-proofing: Injected ILimitService)
        // For now, let's assume a hard limit of 100,000 KES for ATMs
        if (request.Channel == "ATM" && request.Amount > 100000)
        {
            throw new GenericDomainException("Withdrawal exceeds daily ATM limit.", "LIMIT_EXCEEDED");
        }

        // 3. Domain Logic: Debit
        // This internal call checks if balance is sufficient and if account is frozen
        var withdrawalAmount = new Money(request.Amount, Currency.FromCode(request.Currency));
        account.Debit(withdrawalAmount);

        // 4. Update Ledger
        _accountRepository.Update(account);

        // 5. Commit
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return request.CorrelationId;
    }
}
