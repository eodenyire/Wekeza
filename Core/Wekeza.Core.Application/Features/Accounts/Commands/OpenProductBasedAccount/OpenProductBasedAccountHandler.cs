using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Accounts.Commands.OpenProductBasedAccount;

public class OpenProductBasedAccountHandler : IRequestHandler<OpenProductBasedAccountCommand, OpenProductBasedAccountResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountRepository _accountRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly IGLAccountRepository _glAccountRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly ICurrentUserService _currentUserService;

    public OpenProductBasedAccountHandler(
        IUnitOfWork unitOfWork,
        IAccountRepository accountRepository,
        IProductRepository productRepository,
        IPartyRepository partyRepository,
        IGLAccountRepository glAccountRepository,
        IJournalEntryRepository journalEntryRepository,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _accountRepository = accountRepository;
        _productRepository = productRepository;
        _partyRepository = partyRepository;
        _glAccountRepository = glAccountRepository;
        _journalEntryRepository = journalEntryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<OpenProductBasedAccountResult> Handle(OpenProductBasedAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate product exists and is active
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
            throw new ArgumentException("Product not found");

        if (product.Status != ProductStatus.Active)
            throw new InvalidOperationException("Product is not active");

        // 2. Validate customer exists
        var customer = await _partyRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
            throw new ArgumentException("Customer not found");

        // 3. Check product eligibility
        var customerAge = DateTime.UtcNow.Year - customer.DateOfBirth?.Year ?? 25; // Default age if not available
        var customerSegment = CustomerSegment.Retail; // This would come from customer classification

        if (!product.IsEligible(request.CustomerId, request.InitialDeposit, customerSegment, customerAge))
            throw new InvalidOperationException("Customer is not eligible for this product");

        // 4. Generate account number
        var accountNumber = await GenerateAccountNumber(product);

        // 5. Create customer-specific GL account
        var customerGLCode = await CreateCustomerGLAccount(customer, product, accountNumber);

        // 6. Create the account with product configuration
        var currency = new Currency(request.Currency);
        var account = Account.OpenAccount(
            request.CustomerId,
            request.ProductId,
            accountNumber,
            currency,
            customerGLCode,
            _currentUserService.UserId,
            product);

        _accountRepository.Add(account);

        // 7. Handle initial deposit if provided
        string? journalNumber = null;
        if (request.InitialDeposit > 0)
        {
            var depositAmount = new Money(request.InitialDeposit, currency);
            
            // Credit the account
            account.Credit(depositAmount, $"INITIAL-{accountNumber}", "Initial deposit");

            // Create GL entry for initial deposit
            journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Standard);
            var cashGLCode = await GetCashGLCode(request.Currency);
            
            var journalEntry = GLPostingService.CreateDepositEntry(
                account,
                depositAmount,
                $"INITIAL-{accountNumber}",
                "Initial deposit on account opening",
                cashGLCode,
                journalNumber,
                _currentUserService.UserId);

            journalEntry.Post(_currentUserService.UserId);
            _journalEntryRepository.Add(journalEntry);

            // Update GL account balances
            await UpdateGLAccountBalances(journalEntry);
        }

        // 8. Save all changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OpenProductBasedAccountResult
        {
            AccountId = account.Id,
            AccountNumber = accountNumber.Value,
            CustomerGLCode = customerGLCode,
            JournalNumber = journalNumber ?? string.Empty,
            Message = $"Account {accountNumber.Value} opened successfully with product {product.ProductName}"
        };
    }

    private async Task<AccountNumber> GenerateAccountNumber(Product product)
    {
        // Generate account number based on product type
        var prefix = product.Type switch
        {
            ProductType.SavingsAccount => "SAV",
            ProductType.CurrentAccount => "CUR",
            ProductType.FixedDeposit => "FD",
            ProductType.CallDeposit => "CD",
            _ => "ACC"
        };

        var sequence = await _accountRepository.GetNextAccountSequenceAsync(prefix);
        var accountNumber = $"{prefix}{DateTime.UtcNow:yyyyMM}{sequence:000000}";
        
        return new AccountNumber(accountNumber);
    }

    private async Task<string> CreateCustomerGLAccount(Party customer, Product product, AccountNumber accountNumber)
    {
        // Create a customer-specific GL account for this account
        var glCode = await _glAccountRepository.GenerateGLCodeAsync(GLAccountType.Liability, GLAccountCategory.CustomerDeposits);
        
        var customerGLAccount = GLAccount.Create(
            glCode,
            $"{customer.FullName} - {accountNumber.Value}",
            GLAccountType.Liability,
            GLAccountCategory.CustomerDeposits,
            product.Currency,
            3, // Detail level
            true, // Is leaf account
            _currentUserService.UserId,
            product.AccountingConfig?.LiabilityGLCode); // Parent GL code from product

        _glAccountRepository.Add(customerGLAccount);
        
        return glCode;
    }

    private async Task<string> GetCashGLCode(string currency)
    {
        // Get the cash GL account for the currency
        var cashAccounts = await _glAccountRepository.GetByCategoryAsync(GLAccountCategory.Cash);
        var cashAccount = cashAccounts.FirstOrDefault(c => c.Currency == currency);
        
        if (cashAccount == null)
        {
            // Create default cash account if not exists
            var glCode = await _glAccountRepository.GenerateGLCodeAsync(GLAccountType.Asset, GLAccountCategory.Cash);
            var newCashAccount = GLAccount.Create(
                glCode,
                $"Cash in Hand - {currency}",
                GLAccountType.Asset,
                GLAccountCategory.Cash,
                currency,
                2,
                true,
                _currentUserService.UserId);
            
            _glAccountRepository.Add(newCashAccount);
            return glCode;
        }

        return cashAccount.GLCode;
    }

    private async Task UpdateGLAccountBalances(JournalEntry journalEntry)
    {
        foreach (var line in journalEntry.Lines)
        {
            var glAccount = await _glAccountRepository.GetByGLCodeAsync(line.GLCode);
            if (glAccount != null)
            {
                if (line.DebitAmount > 0)
                    glAccount.PostDebit(line.DebitAmount);
                if (line.CreditAmount > 0)
                    glAccount.PostCredit(line.CreditAmount);
                
                _glAccountRepository.Update(glAccount);
            }
        }
    }
}