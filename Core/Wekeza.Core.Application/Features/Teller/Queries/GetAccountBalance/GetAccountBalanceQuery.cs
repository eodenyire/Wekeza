using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Queries.GetAccountBalance;

public record GetAccountBalanceQuery : IRequest<Result<AccountBalanceDto>>
{
    public string AccountNumber { get; init; } = string.Empty;
    public Guid AccountId { get; init; }
}

public class AccountBalanceDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal AvailableBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal HoldAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime LastTransactionDate { get; set; }
}

public class GetAccountBalanceHandler : IRequestHandler<GetAccountBalanceQuery, Result<AccountBalanceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAccountBalanceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AccountBalanceDto>> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var balance = new AccountBalanceDto
            {
                AccountNumber = request.AccountNumber,
                AvailableBalance = 5000,
                CurrentBalance = 5000,
                HoldAmount = 0,
                Currency = "USD",
                LastTransactionDate = DateTime.UtcNow
            };

            await Task.CompletedTask;
            return Result<AccountBalanceDto>.Success(balance);
        }
        catch (Exception ex)
        {
            return Result<AccountBalanceDto>.Failure($"Failed to get account balance: {ex.Message}");
        }
    }
}
