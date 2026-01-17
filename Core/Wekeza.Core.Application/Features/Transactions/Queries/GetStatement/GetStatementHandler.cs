public class GetStatementHandler : IRequestHandler<GetStatementQuery, List<TransactionHistoryDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public async Task<List<TransactionHistoryDto>> Handle(GetStatementQuery request, CancellationToken ct)
    {
        var transactions = await _transactionRepository.GetByAccountAsync(request.AccountId, request.FromDate, request.ToDate, ct);
        return _mapper.Map<List<TransactionHistoryDto>>(transactions);
    }
}
