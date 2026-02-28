using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Queries.GetCustomerAccounts;

public record GetCustomerAccountsQuery : IRequest<Result<List<CustomerAccountDto>>>
{
    public Guid CustomerId { get; init; }
}

public class CustomerAccountDto
{
    public Guid AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class GetCustomerAccountsHandler : IRequestHandler<GetCustomerAccountsQuery, Result<List<CustomerAccountDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerAccountsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CustomerAccountDto>>> Handle(GetCustomerAccountsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var accounts = new List<CustomerAccountDto>();
            await Task.CompletedTask;
            return Result<List<CustomerAccountDto>>.Success(accounts);
        }
        catch (Exception ex)
        {
            return Result<List<CustomerAccountDto>>.Failure($"Failed to get customer accounts: {ex.Message}");
        }
    }
}
