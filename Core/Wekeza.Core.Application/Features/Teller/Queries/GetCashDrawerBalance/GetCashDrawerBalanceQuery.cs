using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Queries.GetCashDrawerBalance;

public record GetCashDrawerBalanceQuery : IRequest<Result<CashDrawerBalanceDto>>
{
    public Guid SessionId { get; init; }
}

public class CashDrawerBalanceDto
{
    public Guid SessionId { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public int TransactionCount { get; set; }
}

public class GetCashDrawerBalanceHandler : IRequestHandler<GetCashDrawerBalanceQuery, Result<CashDrawerBalanceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCashDrawerBalanceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CashDrawerBalanceDto>> Handle(GetCashDrawerBalanceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var balance = new CashDrawerBalanceDto
            {
                SessionId = request.SessionId,
                OpeningBalance = 10000,
                CurrentBalance = 12500,
                TotalDeposits = 5000,
                TotalWithdrawals = 2500,
                TransactionCount = 10
            };

            await Task.CompletedTask;
            return Result<CashDrawerBalanceDto>.Success(balance);
        }
        catch (Exception ex)
        {
            return Result<CashDrawerBalanceDto>.Failure($"Failed to get cash drawer balance: {ex.Message}");
        }
    }
}
