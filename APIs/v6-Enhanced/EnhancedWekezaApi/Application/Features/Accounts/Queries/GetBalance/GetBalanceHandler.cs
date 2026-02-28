using MediatR;
using EnhancedWekezaApi.Domain.Interfaces;
using EnhancedWekezaApi.Application.Features.Accounts.Queries.GetAccount;

namespace EnhancedWekezaApi.Application.Features.Accounts.Queries.GetBalance;

public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetBalanceHandler(IAccountRepository accountRepository, ICustomerRepository customerRepository)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    public async Task<AccountDto> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber, cancellationToken);
        
        if (account == null)
            throw new InvalidOperationException("Account not found");

        var customer = await _customerRepository.GetByIdAsync(account.CustomerId, cancellationToken);

        return new AccountDto
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            Balance = account.BalanceAmount,
            Currency = account.CurrencyCode,
            Status = account.Status,
            IsFrozen = account.IsFrozen,
            AccountType = account.AccountType,
            CreatedAt = account.CreatedAt,
            CustomerName = customer?.FullName ?? "Unknown",
            CustomerNumber = customer?.CustomerNumber ?? "Unknown"
        };
    }
}