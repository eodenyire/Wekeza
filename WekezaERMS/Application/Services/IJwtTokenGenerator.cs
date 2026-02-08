using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Services;

/// <summary>
/// Interface for JWT token generation
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
