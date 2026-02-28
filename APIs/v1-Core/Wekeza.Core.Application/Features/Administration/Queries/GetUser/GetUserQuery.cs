using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Queries.GetUser;

public record GetUserQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; init; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
}

public class GetUserHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = new UserDto { Id = request.UserId };
            return Result<UserDto>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Failed to get user: {ex.Message}");
        }
    }
}
