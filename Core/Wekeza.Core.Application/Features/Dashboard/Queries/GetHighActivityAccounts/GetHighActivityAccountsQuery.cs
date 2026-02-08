using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetHighActivityAccounts;

public record GetHighActivityAccountsQuery : IRequest<Result<List<HighActivityAccountDto>>>
{
    public int Limit { get; init; } = 10;
}

public class HighActivityAccountDto
{
    public Guid AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalVolume { get; set; }
}

public class GetHighActivityAccountsHandler : IRequestHandler<GetHighActivityAccountsQuery, Result<List<HighActivityAccountDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetHighActivityAccountsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<HighActivityAccountDto>>> Handle(GetHighActivityAccountsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var accounts = new List<HighActivityAccountDto>();
            return Result<List<HighActivityAccountDto>>.Success(accounts);
        }
        catch (Exception ex)
        {
            return Result<List<HighActivityAccountDto>>.Failure($"Failed to get high activity accounts: {ex.Message}");
        }
    }
}
