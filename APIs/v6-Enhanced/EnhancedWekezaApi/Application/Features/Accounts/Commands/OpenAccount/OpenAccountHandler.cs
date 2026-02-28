using MediatR;
using EnhancedWekezaApi.Domain.Entities;
using EnhancedWekezaApi.Domain.Interfaces;
using EnhancedWekezaApi.Domain.ValueObjects;
using EnhancedWekezaApi.Application.Features.Accounts.Queries.GetAccount;

namespace EnhancedWekezaApi.Application.Features.Accounts.Commands.OpenAccount;

public class OpenAccountHandler : IRequestHandler<OpenAccountCommand, AccountDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAccountRepository _accountRepository;

    public OpenAccountHandler(
        ICustomerRepository customerRepository, 
        IAccountRepository accountRepository)
    {
        _customerRepository = customerRepository;
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if customer already exists by ID Number
        var existingCustomer = await _customerRepository.GetByIdentificationNumberAsync(request.IdentificationNumber, cancellationToken);
        
        var customer = existingCustomer ?? new Customer(
            Guid.NewGuid(), 
            request.FirstName, 
            request.LastName, 
            request.Email, 
            request.IdentificationNumber);

        if (existingCustomer == null)
            await _customerRepository.AddAsync(customer, cancellationToken);

        // 2. Create the Account
        var currency = Currency.FromCode(request.CurrencyCode);
        var accountNumber = AccountNumber.Generate();
        
        var account = new Account(Guid.NewGuid(), customer.Id, accountNumber, currency);

        // 3. Handle Initial Deposit if any
        if (request.InitialDeposit > 0)
        {
            account.Credit(new Money(request.InitialDeposit, currency));
        }

        await _accountRepository.AddAsync(account, cancellationToken);

        // 4. Return the DTO
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
            CustomerName = customer.FullName,
            CustomerNumber = customer.CustomerNumber
        };
    }
}