///2. CloseAccountHandler.cs (The Final Exit)
///Closing an account is a permanent action. In 2026 banking standards, you cannot close an account if it still has money in it. This handler ensures the balance is zero before the database record is "soft-deleted" or marked as closed.
///
///
using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Accounts.Commands.Management;

public record CloseAccountCommand(string AccountNumber) : IRequest<bool>;

public class CloseAccountHandler : IRequestHandler<CloseAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CloseAccountCommand request, CancellationToken ct)
    {
        var accountNumber = new AccountNumber(request.AccountNumber);
        var account = await _accountRepository.GetByAccountNumberAsync(accountNumber, ct)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        // Domain Logic: Will throw DomainException if Balance > 0
        account.Close("Account closed by user request", _currentUserService.Username ?? "System");

        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
