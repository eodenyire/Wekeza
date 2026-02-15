using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Api.Authentication;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Authentication endpoints for Wekeza Bank
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationController(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // TODO: Validate credentials against database
        // This is a simplified example - in production, validate against user store
        
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Mock user validation - replace with actual authentication
        var userId = Guid.NewGuid();
        var roles = DetermineUserRoles(request.Username);

        var token = _jwtTokenGenerator.GenerateToken(
            userId,
            request.Username,
            request.Email ?? $"{request.Username}@wekeza.com",
            roles
        );

        return Ok(new LoginResponse
        {
            Token = token,
            UserId = userId,
            Username = request.Username,
            Roles = roles.Select(r => r.ToString()).ToList(),
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        });
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

        return Ok(new UserInfoResponse
        {
            UserId = Guid.Parse(userId!),
            Username = username!,
            Email = email!,
            Roles = roles
        });
    }

    private static IEnumerable<UserRole> DetermineUserRoles(string username)
    {
        // Mock role assignment - replace with database lookup
        return username.ToLower() switch
        {
            "admin" => new[] { UserRole.Administrator },
            "teller" => new[] { UserRole.Teller },
            "loanofficer" => new[] { UserRole.LoanOfficer },
            "riskofficer" => new[] { UserRole.RiskOfficer },
            _ => new[] { UserRole.Customer }
        };
    }
}

public record LoginRequest
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? Email { get; init; }
}

public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
    public DateTime ExpiresAt { get; init; }
}

public record UserInfoResponse
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
}
