using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

public class GetStatementHandler : IRequestHandler<GetStatementQuery, Result<StatementDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetStatementHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<Result<StatementDto>> Handle(GetStatementQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = await _transactionRepository.GetByAccountAsync(
                request.AccountId, 
                request.FromDate, 
                request.ToDate, 
                cancellationToken);

            var transactionDtos = _mapper.Map<List<TransactionHistoryDto>>(transactions);
            
            var statement = new StatementDto
            {
                AccountId = request.AccountId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Transactions = transactionDtos,
                TotalTransactions = transactionDtos.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(transactionDtos.Count / (double)request.PageSize)
            };

            return Result<StatementDto>.Success(statement);
        }
        catch (Exception ex)
        {
            return Result<StatementDto>.Failure($"Failed to get statement: {ex.Message}");
        }
    }
}
