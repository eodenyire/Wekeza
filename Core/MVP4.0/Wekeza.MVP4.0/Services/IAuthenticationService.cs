using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public interface IAuthenticationService
{
    Task<LoginResponse> AuthenticateAsync(LoginRequest request);
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    Task<ApplicationUser?> GetUserByUsernameAsync(string username);
    string GenerateJwtToken(ApplicationUser user);
}
