using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetRestrictedAccounts;

public record GetRestrictedAccountsQuery : IRequest<Result<List<RestrictedAccountDto>>>
{
}

public class RestrictedAccountDto
{
    public Guid AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string RestrictionType { get; set; } = string.Empty;
    public string RestrictionReason { get; set; } = string.Empty;
    public DateTime RestrictionDate { get; set; }
}

public class GetRestrictedAccountsHandler : IRequestHandler<GetRestrictedAccountsQuery, Result<List<RestrictedAccountDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetRestrictedAccountsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<RestrictedAccountDto>>> Handle(GetRestrictedAccountsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var accounts = new List<RestrictedAccountDto>();
            return Result<List<RestrictedAccountDto>>.Success(accounts);
        }
        catch (Exception ex)
        {
            return Result<List<RestrictedAccountDto>>.Failure($"Failed to get restricted accounts: {ex.Message}");
        }
    }
}
