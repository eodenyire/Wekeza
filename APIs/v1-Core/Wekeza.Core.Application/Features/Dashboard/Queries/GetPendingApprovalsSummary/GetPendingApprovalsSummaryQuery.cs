using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetPendingApprovalsSummary;

public record GetPendingApprovalsSummaryQuery : IRequest<Result<PendingApprovalsSummaryDto>>
{
}

public class PendingApprovalsSummaryDto
{
    public int TotalPendingApprovals { get; set; }
    public int PendingLoans { get; set; }
    public int PendingAccounts { get; set; }
    public int PendingTransactions { get; set; }
    public int PendingCustomers { get; set; }
}

public class GetPendingApprovalsSummaryHandler : IRequestHandler<GetPendingApprovalsSummaryQuery, Result<PendingApprovalsSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingApprovalsSummaryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PendingApprovalsSummaryDto>> Handle(GetPendingApprovalsSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var summary = new PendingApprovalsSummaryDto
            {
                TotalPendingApprovals = 0,
                PendingLoans = 0,
                PendingAccounts = 0,
                PendingTransactions = 0,
                PendingCustomers = 0
            };

            await Task.CompletedTask;
            return Result<PendingApprovalsSummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result<PendingApprovalsSummaryDto>.Failure($"Failed to get pending approvals summary: {ex.Message}");
        }
    }
}
