using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccount;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;
/// <summary>
/// 3. The Logic: Commands/OpenAccount/OpenAccountHandler.cs
/// This is the "Brain." It orchestrates the Domain Aggregates (Customer and Account) and uses the Repositories.
/// </summary>

namespace Wekeza.Core.Application.Features.Accounts.Commands.OpenAccount;

public class OpenAccountHandler : IRequestHandler<OpenAccountCommand, Result<OpenAccountResult>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly AutoMapper.IMapper _mapper;

    public OpenAccountHandler(
        ICustomerRepository customerRepository, 
        IAccountRepository accountRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        AutoMapper.IMapper mapper)
    {
        _customerRepository = customerRepository;
        _accountRepository = accountRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<OpenAccountResult>> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Logic: Check if customer already exists by ID Number
        var existingCustomer = await _customerRepository.GetByIdentificationNumberAsync(request.IdentificationNumber, cancellationToken);
        
        var customer = existingCustomer ?? new Customer(
            Guid.NewGuid(), 
            request.FirstName ?? "Unknown", 
            request.LastName ?? "Unknown", 
            request.Email ?? "unknown@example.com", 
            request.PhoneNumber ?? "000-000-0000",
            request.IdentificationNumber ?? "UNKNOWN");

        if (existingCustomer == null)
            await _customerRepository.AddAsync(customer, cancellationToken);

        // 2. Logic: Create the Account Aggregate
        var currency = Currency.FromCode(request.CurrencyCode);
        var accountNumber = new AccountNumber($"WKZ-{Guid.NewGuid().ToString()[..8].ToUpper()}"); // Simplified for now
        
        // Get product for account configuration
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
            throw new NotFoundException("Product", request.ProductId);
        
        var account = Account.OpenAccount(
            customer.Id, 
            request.ProductId,
            accountNumber, 
            currency,
            $"GL-{accountNumber.Value}", // GL code
            _currentUserService.Username ?? "System",
            product,
            request.AccountType);

        // 3. Handle Initial Deposit if any
        if (request.InitialDeposit > 0)
        {
            account.Credit(new Money(request.InitialDeposit, currency), Guid.NewGuid().ToString(), "Initial deposit");
        }

        await _accountRepository.AddAsync(account, cancellationToken);

        // 4. Return the DTO (Mapping handled by our Common/Mappings engine)
        return Result<OpenAccountResult>.Success(new OpenAccountResult
        {
            AccountId = account.Id,
            AccountNumber = account.AccountNumber.Value,
            CustomerId = customer.Id,
            CIFNumber = customer.CustomerNumber ?? "CIF-" + customer.Id.ToString()[..8],
            Status = "PENDING_APPROVAL",
            Message = "Account opening request submitted successfully",
            RequiresApproval = request.RequiresApproval
        });
    }
}
