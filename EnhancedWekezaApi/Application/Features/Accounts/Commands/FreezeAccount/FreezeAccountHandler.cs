using MediatR;
using EnhancedWekezaApi.Domain.Interfaces;

namespace EnhancedWekezaApi.Application.Features.Accounts.Commands.FreezeAccount;

public class FreezeAccountHandler : IRequestHandler<FreezeAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;

    public FreezeAccountHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<bool> Handle(FreezeAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber, cancellationToken);
        
        if (account == null)
            throw new InvalidOperationException("Account not found");

        account.Freeze(request.FreezeReason);
        await _accountRepository.UpdateAsync(account, cancellationToken);

        return true;
    }
}