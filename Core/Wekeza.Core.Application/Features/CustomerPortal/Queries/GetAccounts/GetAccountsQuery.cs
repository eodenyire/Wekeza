using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Queries.GetAccounts;

public record GetAccountsQuery : IRequest<Result<List<CustomerAccountDto>>>
{
    public Guid CustomerId { get; init; }
}

public class CustomerAccountDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "KES";
    public string Status { get; set; } = string.Empty;
}

public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, Result<List<CustomerAccountDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAccountsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CustomerAccountDto>>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var accounts = new List<CustomerAccountDto>();
            return Result<List<CustomerAccountDto>>.Success(accounts);
        }
        catch (Exception ex)
        {
            return Result<List<CustomerAccountDto>>.Failure($"Failed to get accounts: {ex.Message}");
        }
    }
}
