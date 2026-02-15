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

            var transactionList = transactions.ToList();
            
            // Map transactions to DTOs
            var transactionDtos = transactionList.Select(t => new TransactionHistoryDto
            {
                TransactionId = t.Id,
                TransactionDate = t.Timestamp,
                ValueDate = t.Timestamp,
                TransactionType = t.Type.ToString(),
                Description = t.Description,
                Reference = t.CorrelationId.ToString(),
                DebitAmount = t.Type == Wekeza.Core.Domain.Aggregates.TransactionType.Withdrawal || t.Type == Wekeza.Core.Domain.Aggregates.TransactionType.Fee ? t.Amount.Amount : 0,
                CreditAmount = t.Type == Wekeza.Core.Domain.Aggregates.TransactionType.Deposit || t.Type == Wekeza.Core.Domain.Aggregates.TransactionType.Interest ? t.Amount.Amount : 0,
                Balance = 0, // Will be calculated separately if needed
                Currency = t.Amount.Currency.Code,
                Channel = "Unknown",
                Status = "Completed",
                Remarks = null
            }).ToList();
            
            var statement = new StatementDto
            {
                AccountId = request.AccountId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Transactions = transactionDtos,
                TotalTransactions = transactionDtos.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)transactionDtos.Count / request.PageSize)
            };

            return Result<StatementDto>.Success(statement);
        }
        catch (Exception ex)
        {
            return Result<StatementDto>.Failure($"Failed to get statement: {ex.Message}");
        }
    }
}
