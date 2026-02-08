using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetCustomerStatistics;

public record GetCustomerStatisticsQuery : IRequest<Result<CustomerStatisticsDto>>
{
}

public class CustomerStatisticsDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public int IndividualCustomers { get; set; }
    public int CorporateCustomers { get; set; }
}

public class GetCustomerStatisticsHandler : IRequestHandler<GetCustomerStatisticsQuery, Result<CustomerStatisticsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerStatisticsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CustomerStatisticsDto>> Handle(GetCustomerStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var stats = new CustomerStatisticsDto();
            return Result<CustomerStatisticsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<CustomerStatisticsDto>.Failure($"Failed to get customer statistics: {ex.Message}");
        }
    }
}
