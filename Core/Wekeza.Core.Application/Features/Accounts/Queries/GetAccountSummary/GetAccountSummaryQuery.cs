using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Accounts.Queries.GetAccountSummary;

public record GetAccountSummaryQuery : IRequest<Result<AccountSummaryDto>>
{
    public string AccountNumber { get; init; } = string.Empty;
}

public class AccountSummaryDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "KES";
    public string Status { get; set; } = string.Empty;
    public DateTime OpenedDate { get; set; }
    public decimal AvailableBalance { get; set; }
}

public class GetAccountSummaryHandler : IRequestHandler<GetAccountSummaryQuery, Result<AccountSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAccountSummaryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AccountSummaryDto>> Handle(GetAccountSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var summary = new AccountSummaryDto
            {
                AccountNumber = request.AccountNumber,
                AccountName = "Sample Account",
                Balance = 0
            };
            return Result<AccountSummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result<AccountSummaryDto>.Failure($"Failed to get account summary: {ex.Message}");
        }
    }
}
