///📂 Wekeza.Core.Application/Features/Accounts/Commands/Management/
///1. FreezeAccountHandler.cs (The Security Brake)
///This handler is used when the Risk Engine or a Compliance Officer detects suspicious activity. It pulls the account, calls the Freeze method on the Domain Aggregate (which we built in the Soul), and persists the change.
///
///
///
using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Accounts.Commands.Management;

public record FreezeAccountCommand(string AccountNumber, string Reason) : IRequest<bool>;

public class FreezeAccountHandler : IRequestHandler<FreezeAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FreezeAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(FreezeAccountCommand request, CancellationToken ct)
    {
        var accountNumber = new AccountNumber(request.AccountNumber);
        var account = await _accountRepository.GetByAccountNumberAsync(accountNumber, ct)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        // Domain Logic: The Soul decides if this is allowed
        account.Freeze(request.Reason, _currentUserService.Username ?? "System");

        await _unitOfWork.SaveChangesAsync(ct);
        
        // This will trigger the 'AccountFrozenEvent' which we built earlier
        return true;
    }
}
