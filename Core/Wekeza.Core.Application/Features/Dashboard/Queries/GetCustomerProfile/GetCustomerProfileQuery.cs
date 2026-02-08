using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetCustomerProfile;

public record GetCustomerProfileQuery : IRequest<Result<CustomerProfileDto>>
{
    public Guid CustomerId { get; init; }
}

public class CustomerProfileDto
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CIFNumber { get; set; } = string.Empty;
    public string CustomerType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class GetCustomerProfileHandler : IRequestHandler<GetCustomerProfileQuery, Result<CustomerProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerProfileHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CustomerProfileDto>> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var profile = new CustomerProfileDto { CustomerId = request.CustomerId };
            return Result<CustomerProfileDto>.Success(profile);
        }
        catch (Exception ex)
        {
            return Result<CustomerProfileDto>.Failure($"Failed to get customer profile: {ex.Message}");
        }
    }
}
