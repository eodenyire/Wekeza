using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Wekeza.MVP4._0.Models;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRBACService _rbacService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IRBACService rbacService,
        ILogger<AuthController> logger)
    {
        _rbacService = rbacService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Username and password are required."
                });
            }

            var credentials = new UserCredentials
            {
                Username = request.Username,
                Password = request.Password
            };

            var authResult = await _rbacService.AuthenticateUserAsync(credentials);

            if (!authResult.IsSuccess)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = authResult.ErrorMessage ?? "Authentication failed"
                });
            }

            // Check if role matches (for backward compatibility)
            if (authResult.User != null && authResult.User.Role != request.Role)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = $"Invalid credentials for {request.Role} role"
                });
            }

            // Create cookie authentication for web sessions
            if (authResult.User != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, authResult.User.Id.ToString()),
                    new Claim(ClaimTypes.Name, authResult.User.Username),
                    new Claim(ClaimTypes.Email, authResult.User.Email),
                    new Claim(ClaimTypes.Role, authResult.User.Role.ToString()),
                    new Claim("FullName", authResult.User.FullName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = authResult.ExpiresAt
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Store session information
                HttpContext.Session.SetString("UserId", authResult.User.Id.ToString());
                HttpContext.Session.SetString("Username", authResult.User.Username);
                HttpContext.Session.SetString("Role", authResult.User.Role.ToString());
            }

            return Ok(new LoginResponse
            {
                Success = true,
                Token = authResult.Token,
                Message = "Login successful",
                User = authResult.User
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return StatusCode(500, new LoginResponse
            {
                Success = false,
                Message = "An error occurred during login. Please try again."
            });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Get user ID from session or claims
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                await _rbacService.LogoutUserAsync(userId);
            }

            // Clear cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session
            HttpContext.Session.Clear();

            return Ok(new { Success = true, Message = "Logout successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { Success = false, Message = "An error occurred during logout" });
        }
    }

    [HttpGet("validate-session")]
    public async Task<IActionResult> ValidateSession()
    {
        try
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Success = false, Message = "Invalid session" });
            }

            var isValid = await _rbacService.ValidateSessionAsync(userId);
            if (!isValid)
            {
                return Unauthorized(new { Success = false, Message = "Session expired" });
            }

            return Ok(new { Success = true, Message = "Session valid", UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating session");
            return StatusCode(500, new { Success = false, Message = "An error occurred validating session" });
        }
    }

    [HttpGet("user-permissions")]
    public async Task<IActionResult> GetUserPermissions()
    {
        try
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { Success = false, Message = "Invalid session" });
            }

            var permissions = await _rbacService.GetUserPermissionsAsync(userId);
            return Ok(new { Success = true, Permissions = permissions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions");
            return StatusCode(500, new { Success = false, Message = "An error occurred getting permissions" });
        }
    }
}
