using MediatR;
using AutoMapper;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccount;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Accounts.Queries.GetBalance;

public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, AccountDto>
{
    private readonly IAccountRepository _repository;
    private readonly IMapper _mapper;

    public GetBalanceHandler(IAccountRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        return _mapper.Map<AccountDto>(account);
    }
}
