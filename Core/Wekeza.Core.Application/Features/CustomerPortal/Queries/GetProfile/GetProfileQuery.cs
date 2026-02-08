using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Queries.GetProfile;

public record GetProfileQuery : IRequest<Result<CustomerProfileDto>>
{
    public Guid CustomerId { get; init; }
}

public class CustomerProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
}

public class GetProfileHandler : IRequestHandler<GetProfileQuery, Result<CustomerProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProfileHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CustomerProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var profile = new CustomerProfileDto();
            return Result<CustomerProfileDto>.Success(profile);
        }
        catch (Exception ex)
        {
            return Result<CustomerProfileDto>.Failure($"Failed to get profile: {ex.Message}");
        }
    }
}
