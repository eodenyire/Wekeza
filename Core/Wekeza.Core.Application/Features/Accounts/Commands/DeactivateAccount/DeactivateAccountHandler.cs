using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using MediatR;

namespace Wekeza.Core.Application.Features.Accounts.Commands.DeactivateAccount;

public class DeactivateAccountHandler : IRequestHandler<DeactivateAccountCommand, bool>
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateAccountHandler(IAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeactivateAccountCommand request, CancellationToken ct)
    {
        var account = await _repository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), ct)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        account.Close(request.Reason, "System"); // Domain logic sets IsActive = false

        _repository.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
