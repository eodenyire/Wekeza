using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly MVP4DbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        MVP4DbContext context,
        IConfiguration configuration,
        ILogger<AuthenticationService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Check if role matches
            if (user.Role != request.Role)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Invalid credentials for {request.Role} role"
                };
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Success = true,
                Token = token,
                Message = "Login successful",
                User = user
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication");
            return new LoginResponse
            {
                Success = false,
                Message = "An error occurred during authentication"
            };
        }
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public string GenerateJwtToken(ApplicationUser user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "WekeezaMVP4SecretKeyThatIsAtLeast32CharactersLong123456";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "WekeezaMVP4";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "WekeezaMVP4Users";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("FullName", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
