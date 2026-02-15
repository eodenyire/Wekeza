using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LogoutController : ControllerBase
    {
        private readonly IRBACService _rbacService;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(IRBACService rbacService, ILogger<LogoutController> logger)
        {
            _rbacService = rbacService;
            _logger = logger;
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                
                // Log the logout action in RBAC service
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    await _rbacService.LogoutUserAsync(userId);
                }
                
                // Clear authentication cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Clear session
                HttpContext.Session.Clear();
                
                _logger.LogInformation($"User {userName} logged out successfully");
                
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "Logout failed" });
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogoutGet()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                
                // Log the logout action in RBAC service
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    await _rbacService.LogoutUserAsync(userId);
                }
                
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                return Redirect("/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return Redirect("/");
            }
        }
    }
}
