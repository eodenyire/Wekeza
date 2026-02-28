using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Queries.GetRoles;

public record GetRolesQuery : IRequest<Result<List<RoleDto>>>
{
}

public class RoleDto
{
    public Guid Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class GetRolesHandler : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetRolesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var roles = new List<RoleDto>();
            return Result<List<RoleDto>>.Success(roles);
        }
        catch (Exception ex)
        {
            return Result<List<RoleDto>>.Failure($"Failed to get roles: {ex.Message}");
        }
    }
}
