using MediatR;
using EnhancedWekezaApi.Domain.Interfaces;

namespace EnhancedWekezaApi.Application.Features.Accounts.Commands.CloseAccount;

public class CloseAccountHandler : IRequestHandler<CloseAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;

    public CloseAccountHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<bool> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber, cancellationToken);
        
        if (account == null)
            throw new InvalidOperationException("Account not found");

        // Business rule: Account must have zero balance to close
        if (account.BalanceAmount != 0)
            throw new InvalidOperationException("Cannot close account with non-zero balance");

        account.Close();
        await _accountRepository.UpdateAsync(account, cancellationToken);

        return true;
    }
}