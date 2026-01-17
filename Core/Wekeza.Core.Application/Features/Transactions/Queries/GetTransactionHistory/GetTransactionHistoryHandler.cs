using MediatR;
using AutoMapper;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Exceptions;
///
/// 3. The Performance Handler: GetTransactionHistoryHandler.cs
/// In a Tier-1 system, the "Read" side often bypasses the heavy Domain Aggregates and goes straight to the database using an optimized repository or Dapper. For our Core, we use the ITransactionRepository.

namespace Wekeza.Core.Application.Features.Transactions.Queries.GetTransactionHistory;

public class GetTransactionHistoryHandler : IRequestHandler<GetTransactionHistoryQuery, PaginatedList<TransactionDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionHistoryHandler(
        IAccountRepository accountRepository, 
        ITransactionRepository transactionRepository, 
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TransactionDto>> Handle(GetTransactionHistoryQuery request, CancellationToken ct)
    {
        // 1. Validate Account Existence
        var account = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber), ct)
            ?? throw new NotFoundException("Account", request.AccountNumber);

        // 2. Fetch Paginated Transactions
        // In 'The Beast', this repository call uses an Index on (AccountId, Timestamp DESC)
        var transactions = await _transactionRepository.GetPaginatedByAccountIdAsync(
            account.Id, 
            request.PageNumber, 
            request.PageSize, 
            request.FromDate, 
            request.ToDate, 
            ct);

        // 3. Map to DTOs
        var dtos = _mapper.Map<List<TransactionDto>>(transactions.Items);

        return new PaginatedList<TransactionDto>(dtos, transactions.TotalCount, request.PageNumber, request.PageSize);
    }
}
