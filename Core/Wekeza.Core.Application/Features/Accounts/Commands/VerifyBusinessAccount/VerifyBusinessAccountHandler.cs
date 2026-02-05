using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using MediatR;

namespace Wekeza.Core.Application.Features.Accounts.Commands.VerifyBusinessAccount;

public class VerifyBusinessAccountHandler : IRequestHandler<VerifyBusinessAccountCommand, bool>
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyBusinessAccountHandler(IAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(VerifyBusinessAccountCommand request, CancellationToken ct)
    {
        var account = await _repository.GetByIdAsync(request.AccountId, ct)
            ?? throw new NotFoundException("Account", request.AccountId.ToString(), request.AccountId);

        // Business Rule: Update status and set the 'Corporate Mandate'
        account.Verify(request.VerifiedBy); 
        account.UpdateTransactionLimit(request.DailyLimit);

        _repository.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
