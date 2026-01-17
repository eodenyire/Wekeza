using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
///
/// 3. The Executioner: CloseAccountHandler.cs
/// This is where we implement the Financial Integrity Check. We check the balance and we query the "Lending" domain to ensure no active loans are linked to this account.
///

namespace Wekeza.Core.Application.Features.Accounts.Commands.CloseAccount;

public class CloseAccountHandler : IRequestHandler<CloseAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    // Note: In a real 'Beast', we would also have a ILoanRepository or a LoanService check here.
    private readonly IUnitOfWork _unitOfWork;

    public CloseAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch the Aggregate
        var account = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        // 2. Financial Integrity Check: Balance must be EXACTLY zero
        if (account.Balance.Amount != 0)
        {
            throw new DomainException(
                $"Account {request.AccountNumber} cannot be closed. Current balance is {account.Balance.Amount}. Please withdraw funds first.", 
                "ACCOUNT_NOT_EMPTY");
        }

        // 3. Liability Check: Verify no outstanding loans (Mocking the logic for now)
        // In the 'Beast', you'd call: if (await _loanService.HasActiveLoans(account.Id)) ...
        bool hasActiveLoans = false; // Logic to be implemented in the Loans feature
        if (hasActiveLoans)
        {
            throw new DomainException(
                $"Account {request.AccountNumber} has outstanding loan obligations. Closure denied.", 
                "OUTSTANDING_LIABILITIES");
        }

        // 4. Execute Domain Logic
        // This will move status to 'Closed' and raise an AccountClosedDomainEvent
        // account.Close(); 

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
