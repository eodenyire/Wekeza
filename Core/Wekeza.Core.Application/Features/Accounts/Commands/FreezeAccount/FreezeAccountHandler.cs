using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
///
/// 3. The Executioner: FreezeAccountHandler.cs
/// This handler uses our Aggregate Root logic. It doesn't just change a flag in the DB; it calls the Freeze() method on the Account aggregate, which will subsequently raise the AccountFrozenEvent to signal the Card and Mobile systems.

namespace Wekeza.Core.Application.Features.Accounts.Commands.FreezeAccount;

public class FreezeAccountHandler : IRequestHandler<FreezeAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FreezeAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(FreezeAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch the Aggregate
        var account = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        // 2. Execute Domain Logic
        // The .Freeze() method inside the Domain Aggregate handles state change and Event generation.
        account.Freeze();

        // 3. Update Repository
        _accountRepository.Update(account);

        // 4. Atomic Save
        // TransactionBehavior ensures this is wrapped in a DB transaction.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
