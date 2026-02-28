using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Domain.ValueObjects;
///<summary>
/// 3. The Orchestrator: TransferFundsHandler.cs
/// This is the Principal-Grade implementation. Notice how it uses the Domain Service to perform the logic. It doesn't "touch" the balance directly; it delegates to the experts.
///</summary>
namespace Wekeza.Core.Application.Features.Transactions.Commands.TransferFunds;

public class TransferFundsHandler : IRequestHandler<TransferFundsCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TransferService _transferService;

    public TransferFundsHandler(
        IAccountRepository accountRepository, 
        IUnitOfWork unitOfWork,
        TransferService transferService)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _transferService = transferService;
    }

    public async Task<Guid> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Aggregates (Infrastructure handles Row-Level Locking via the repository)
        var sourceAccount = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.FromAccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.FromAccountNumber);

        var destinationAccount = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.ToAccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.ToAccountNumber);

        // 2. Map to Value Object
        var transferAmount = new Money(request.Amount, Currency.FromCode(request.Currency));

        // 3. Delegate to Domain Service (Encapsulated Business Logic)
        // This method performs the Debit, Credit, and Currency Checks.
        _transferService.Transfer(sourceAccount, destinationAccount, transferAmount);

        // 4. Update Repositories
        _accountRepository.Update(sourceAccount);
        _accountRepository.Update(destinationAccount);

        // 5. Commit (The TransactionBehavior will handle the physical SQL Transaction & Outbox)
        // We return the CorrelationId so the UI can track the status.
        return request.CorrelationId;
    }
}
