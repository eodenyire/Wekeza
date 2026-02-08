using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Queries.GetBranches;

public record GetBranchesQuery : IRequest<Result<List<BranchDto>>>
{
}

public class BranchDto
{
    public Guid Id { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

public class GetBranchesHandler : IRequestHandler<GetBranchesQuery, Result<List<BranchDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetBranchesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BranchDto>>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var branches = new List<BranchDto>();
            return Result<List<BranchDto>>.Success(branches);
        }
        catch (Exception ex)
        {
            return Result<List<BranchDto>>.Failure($"Failed to get branches: {ex.Message}");
        }
    }
}
