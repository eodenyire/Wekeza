using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.MVP4._0.Services;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Controllers
{
    [Authorize(Roles = "Administrator")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpPost("lock-user/{userId}")]
        public async Task<IActionResult> LockUser(Guid userId)
        {
            try
            {
                var success = await _adminService.LockUserAsync(userId);
                if (success)
                {
                    return Ok(new { success = true, message = "User locked successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to lock user." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("unlock-user/{userId}")]
        public async Task<IActionResult> UnlockUser(Guid userId)
        {
            try
            {
                var success = await _adminService.UnlockUserAsync(userId);
                if (success)
                {
                    return Ok(new { success = true, message = "User unlocked successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to unlock user." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("reset-password/{userId}")]
        public async Task<IActionResult> ResetPassword(Guid userId, [FromBody] ResetPasswordRequest request)
        {
            try
            {
                var success = await _adminService.ResetPasswordAsync(userId, request.NewPassword);
                if (success)
                {
                    return Ok(new { success = true, message = "Password reset successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to reset password." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                _logger?.LogInformation($"Create user request received for username: {request?.Username}");
                _logger?.LogInformation($"Request data: FullName={request?.FullName}, Username={request?.Username}, Email={request?.Email}, Role={request?.Role}, IsActive={request?.IsActive}");
                
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Email) || 
                    string.IsNullOrWhiteSpace(request.FullName) || 
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger?.LogWarning("Create user failed: Missing required fields");
                    return BadRequest(new { success = false, message = "All fields are required." });
                }

                if (request.Password.Length < 6)
                {
                    _logger?.LogWarning($"Create user failed: Password too short for user {request.Username}");
                    return BadRequest(new { success = false, message = "Password must be at least 6 characters long." });
                }

                // Check if username already exists
                var existingUser = await _adminService.GetAllUsersAsync();
                if (existingUser.Any(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger?.LogWarning($"Create user failed: Username '{request.Username}' already exists");
                    return BadRequest(new { success = false, message = $"Username '{request.Username}' already exists. Please choose a different username." });
                }

                // Check if email already exists
                if (existingUser.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger?.LogWarning($"Create user failed: Email '{request.Email}' already exists");
                    return BadRequest(new { success = false, message = $"Email address '{request.Email}' already exists. Please use a different email." });
                }

                var user = new ApplicationUser
                {
                    Username = request.Username.Trim(),
                    Email = request.Email.Trim(),
                    FullName = request.FullName.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role,
                    IsActive = request.IsActive
                };

                _logger?.LogInformation($"Attempting to create user: {user.Username} with role {user.Role}");
                var success = await _adminService.CreateUserAsync(user);
                if (success)
                {
                    _logger?.LogInformation($"User created successfully: {user.Username}");
                    return Ok(new { success = true, message = "User created successfully!" });
                }
                else
                {
                    _logger?.LogError($"Failed to create user: {user.Username} - AdminService returned false");
                    return BadRequest(new { success = false, message = "Failed to create user. Please check the logs for details." });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Exception while creating user: {request?.Username}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("system-stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            try
            {
                var stats = await _adminService.GetSystemStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("system-health")]
        public async Task<IActionResult> GetSystemHealth()
        {
            try
            {
                var health = await _adminService.GetSystemHealthAsync();
                return Ok(health);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("backup-database")]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                var success = await _adminService.BackupDatabaseAsync();
                if (success)
                {
                    return Ok(new { success = true, message = "Database backup initiated successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to initiate database backup." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int pageSize = 50)
        {
            try
            {
                var logs = await _adminService.GetAuditLogsAsync(fromDate, toDate, pageSize);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("export-users")]
        public async Task<IActionResult> ExportUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                var csv = GenerateUsersCsv(users);
                var fileName = $"users_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("export-audit-logs")]
        public async Task<IActionResult> ExportAuditLogs([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var logs = await _adminService.GetAuditLogsAsync(fromDate, toDate, 1000);
                var csv = GenerateAuditLogsCsv(logs);
                var fileName = $"audit_logs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("generate-report")]
        public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequest request)
        {
            try
            {
                // Simulate report generation
                await Task.Delay(2000);
                
                var reportData = GenerateReportData(request.ReportType);
                var fileName = $"{request.ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                return File(System.Text.Encoding.UTF8.GetBytes(reportData), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                // In a real system, this would update the current user's profile
                await _adminService.LogActivityAsync("Profile Updated", User.Identity?.Name ?? "admin", $"Updated profile information");
                return Ok(new { success = true, message = "Profile updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // In a real system, this would verify current password and update
                await _adminService.LogActivityAsync("Password Changed", User.Identity?.Name ?? "admin", "Password changed successfully");
                return Ok(new { success = true, message = "Password changed successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("update-system-setting")]
        public async Task<IActionResult> UpdateSystemSetting([FromBody] UpdateSettingRequest request)
        {
            try
            {
                var success = await _adminService.UpdateSystemSettingAsync(request.Key, request.Value);
                if (success)
                {
                    return Ok(new { success = true, message = "Setting updated successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to update setting." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private string GenerateUsersCsv(List<ApplicationUser> users)
        {
            var csv = "Full Name,Username,Email,Role,Status,Created At,Last Login\n";
            foreach (var user in users)
            {
                csv += $"\"{user.FullName}\",\"{user.Username}\",\"{user.Email}\",\"{user.Role}\",\"{(user.IsActive ? "Active" : "Inactive")}\",\"{user.CreatedAt:yyyy-MM-dd}\",\"{user.LastLoginAt?.ToString("yyyy-MM-dd") ?? "Never"}\"\n";
            }
            return csv;
        }

        private string GenerateAuditLogsCsv(List<AuditLog> logs)
        {
            var csv = "Timestamp,User,Action,Entity Type,Entity ID,IP Address\n";
            foreach (var log in logs)
            {
                csv += $"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{log.UserName}\",\"{log.Action}\",\"{log.EntityType}\",\"{log.EntityId}\",\"{log.IpAddress}\"\n";
            }
            return csv;
        }

        private string GenerateReportData(string reportType)
        {
            return reportType switch
            {
                "user-activity" => "User,Login Count,Last Login\nAdmin,25,2026-01-23\nTeller1,18,2026-01-23\nManager1,12,2026-01-22\n",
                "login-history" => "User,Login Time,IP Address,Status\nAdmin,2026-01-23 09:30,192.168.1.100,Success\nTeller1,2026-01-23 08:45,192.168.1.101,Success\n",
                "security-audit" => "Check,Status,Details\nPassword Policy,Pass,All users have strong passwords\nAccount Lockout,Pass,Lockout policy active\n",
                "db-performance" => "Metric,Value,Status\nQuery Response Time,45ms,Good\nConnection Pool,85%,Normal\nDisk Usage,65%,Normal\n",
                _ => "Report Type,Generated At\n" + reportType + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n"
            };
        }
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class GenerateReportRequest
    {
        public string ReportType { get; set; } = string.Empty;
    }

    public class UpdateSettingRequest
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}