using MediatR;
using AutoMapper;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccount;
/// <summary>
/// 3. The Logic: Commands/OpenAccount/OpenAccountHandler.cs
/// This is the "Brain." It orchestrates the Domain Aggregates (Customer and Account) and uses the Repositories.
/// </summary>

namespace Wekeza.Core.Application.Features.Accounts.Commands.OpenAccount;

public class OpenAccountHandler : IRequestHandler<OpenAccountCommand, AccountDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public OpenAccountHandler(
        ICustomerRepository customerRepository, 
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Logic: Check if customer already exists by ID Number
        var existingCustomer = await _customerRepository.GetByIdentificationNumberAsync(request.IdentificationNumber, cancellationToken);
        
        var customer = existingCustomer ?? new Customer(
            Guid.NewGuid(), 
            request.FirstName, 
            request.LastName, 
            request.Email, 
            request.IdentificationNumber);

        if (existingCustomer == null)
            await _customerRepository.AddAsync(customer, cancellationToken);

        // 2. Logic: Create the Account Aggregate
        var currency = Currency.FromCode(request.CurrencyCode);
        var accountNumber = new AccountNumber($"WKZ-{Guid.NewGuid().ToString()[..8].ToUpper()}"); // Simplified for now
        
        var account = new Account(Guid.NewGuid(), customer.Id, accountNumber, currency);

        // 3. Handle Initial Deposit if any
        if (request.InitialDeposit > 0)
        {
            account.Credit(new Money(request.InitialDeposit, currency));
        }

        await _accountRepository.AddAsync(account, cancellationToken);

        // 4. Return the DTO (Mapping handled by our Common/Mappings engine)
        return _mapper.Map<AccountDto>(account);
    }
}
