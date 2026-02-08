using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Accounts.Commands.FreezeAccount;

public class FreezeAccountHandler : IRequestHandler<FreezeAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public FreezeAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(FreezeAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        account.Freeze(request.Reason, _currentUserService.Username ?? "System");

        _accountRepository.Update(account);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
