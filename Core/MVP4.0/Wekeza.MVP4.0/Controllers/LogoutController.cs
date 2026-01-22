using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Wekeza.MVP4._0.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LogoutController : ControllerBase
    {
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(ILogger<LogoutController> logger)
        {
            _logger = logger;
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                
                // Clear authentication cookie
                await HttpContext.SignOutAsync();
                
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
                await HttpContext.SignOutAsync();
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
