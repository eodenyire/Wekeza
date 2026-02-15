using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using MediatR;

namespace Wekeza.Core.Application.Features.Accounts.Commands.AddSignatory;

public class AddSignatoryHandler : IRequestHandler<AddSignatoryCommand, bool>
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddSignatoryHandler(IAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddSignatoryCommand request, CancellationToken ct)
    {
        var account = await _repository.GetByIdAsync(request.AccountId, ct)
            ?? throw new NotFoundException("Account", request.AccountId);

        // TODO: Domain Logic: Add the signatory to the account's internal list
        // This requires extending the Account aggregate with signatory management
        // account.AddSignatory(request.SignatoryName, request.IdNumber, request.Role, request.SignatureLimit);

        _repository.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
