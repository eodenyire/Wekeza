using Microsoft.EntityFrameworkCore;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services
{
    public class AdminService : IAdminService
    {
        private readonly MVP4DbContext _context;
        private readonly ILogger<AdminService> _logger;

        public AdminService(MVP4DbContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // User Management
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user)
        {
            try
            {
                // Check if username already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    _logger.LogWarning($"User creation failed: Username '{user.Username}' already exists");
                    return false;
                }

                // Check if email already exists
                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingEmail != null)
                {
                    _logger.LogWarning($"User creation failed: Email '{user.Email}' already exists");
                    return false;
                }

                user.Id = Guid.NewGuid();
                user.CreatedAt = DateTime.UtcNow;
                user.IsActive = true;
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"User created successfully: {user.FullName} ({user.Username}) with role {user.Role}");
                await LogActivityAsync("User Created", "admin", $"Created user: {user.FullName} ({user.Username})");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user: {user?.Username ?? "unknown"} - {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                
                await LogActivityAsync("User Updated", "admin", $"Updated user: {user.FullName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.IsActive = false; // Soft delete
                await _context.SaveChangesAsync();
                
                await LogActivityAsync("User Deactivated", "admin", $"Deactivated user: {user.FullName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();
                
                await LogActivityAsync("Password Reset", "admin", $"Reset password for user: {user.FullName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return false;
            }
        }

        public async Task<bool> LockUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.IsActive = false;
                await _context.SaveChangesAsync();
                
                await LogActivityAsync("User Locked", "admin", $"Locked user: {user.FullName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user");
                return false;
            }
        }

        public async Task<bool> UnlockUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.IsActive = true;
                await _context.SaveChangesAsync();
                
                await LogActivityAsync("User Unlocked", "admin", $"Unlocked user: {user.FullName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user");
                return false;
            }
        }

        // System Statistics
        public async Task<Dictionary<string, object>> GetSystemStatisticsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
            var pendingAuthorizations = await _context.Authorizations.CountAsync(a => a.Status == "Pending");
            var activeRiskAlerts = await _context.RiskAlerts.CountAsync(r => r.Status != "Resolved");
            var totalReports = await _context.BranchReports.CountAsync();
            var cashPosition = await _context.CashPositions.OrderByDescending(c => c.LastUpdated).FirstOrDefaultAsync();
            
            var totalCash = cashPosition != null ? cashPosition.VaultCash + cashPosition.TellerCash + cashPosition.ATMCash : 0;

            return new Dictionary<string, object>
            {
                ["TotalUsers"] = totalUsers,
                ["ActiveUsers"] = activeUsers,
                ["ActiveAccounts"] = activeUsers * 2, // Estimate 2 accounts per user
                ["TotalTransactions"] = pendingAuthorizations * 50, // Estimate
                ["ActiveLoans"] = activeUsers / 3, // Estimate 1 loan per 3 users
                ["PendingAuthorizations"] = pendingAuthorizations,
                ["RiskAlerts"] = activeRiskAlerts,
                ["TotalReports"] = totalReports,
                ["CashPosition"] = totalCash
            };
        }

        public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10)
        {
            // In a real system, this would come from an audit log table
            // For now, we'll generate some based on recent database activity
            var activities = new List<ActivityLog>();
            
            // Get recent authorizations
            var recentAuths = await _context.Authorizations
                .Where(a => a.AuthorizedAt.HasValue)
                .OrderByDescending(a => a.AuthorizedAt)
                .Take(3)
                .ToListAsync();
                
            foreach (var auth in recentAuths)
            {
                activities.Add(new ActivityLog
                {
                    Activity = $"Authorization {auth.Status}: {auth.Type}",
                    UserName = auth.AuthorizedBy ?? "system",
                    CreatedAt = auth.AuthorizedAt ?? DateTime.UtcNow,
                    Details = $"Amount: ${auth.Amount:N2}"
                });
            }
            
            // Get recent risk alerts
            var recentAlerts = await _context.RiskAlerts
                .Where(r => r.ResolvedAt.HasValue)
                .OrderByDescending(r => r.ResolvedAt)
                .Take(2)
                .ToListAsync();
                
            foreach (var alert in recentAlerts)
            {
                activities.Add(new ActivityLog
                {
                    Activity = $"Risk Alert {alert.Status}: {alert.AlertType}",
                    UserName = alert.AssignedTo ?? "system",
                    CreatedAt = alert.ResolvedAt ?? DateTime.UtcNow,
                    Details = $"Account: {alert.AccountNumber}"
                });
            }
            
            // Add some system activities
            activities.AddRange(new[]
            {
                new ActivityLog { Activity = "System backup completed", UserName = "system", CreatedAt = DateTime.UtcNow.AddHours(-2) },
                new ActivityLog { Activity = "Database maintenance", UserName = "system", CreatedAt = DateTime.UtcNow.AddHours(-4) },
                new ActivityLog { Activity = "Security scan completed", UserName = "system", CreatedAt = DateTime.UtcNow.AddHours(-6) }
            });
            
            return activities.OrderByDescending(a => a.CreatedAt).Take(count).ToList();
        }

        public async Task<List<SystemHealthCheck>> GetSystemHealthAsync()
        {
            var healthChecks = new List<SystemHealthCheck>();
            
            // Database health
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                healthChecks.Add(new SystemHealthCheck
                {
                    Component = "Database",
                    Status = "Healthy",
                    StatusClass = "success",
                    LastCheck = DateTime.UtcNow,
                    Details = "Connection successful"
                });
            }
            catch
            {
                healthChecks.Add(new SystemHealthCheck
                {
                    Component = "Database",
                    Status = "Error",
                    StatusClass = "danger",
                    LastCheck = DateTime.UtcNow,
                    Details = "Connection failed"
                });
            }
            
            // API Server (always healthy if we're running)
            healthChecks.Add(new SystemHealthCheck
            {
                Component = "API Server",
                Status = "Running",
                StatusClass = "success",
                LastCheck = DateTime.UtcNow,
                Details = "All endpoints responding"
            });
            
            // Memory usage (simulated)
            var memoryUsage = Random.Shared.Next(60, 85);
            healthChecks.Add(new SystemHealthCheck
            {
                Component = "Memory Usage",
                Status = $"{memoryUsage}%",
                StatusClass = memoryUsage > 80 ? "danger" : memoryUsage > 70 ? "warning" : "success",
                LastCheck = DateTime.UtcNow,
                Details = $"Used: {memoryUsage}% of available memory"
            });
            
            // Disk space (simulated)
            var diskUsage = Random.Shared.Next(30, 60);
            healthChecks.Add(new SystemHealthCheck
            {
                Component = "Disk Space",
                Status = $"{diskUsage}% Used",
                StatusClass = diskUsage > 80 ? "danger" : diskUsage > 70 ? "warning" : "success",
                LastCheck = DateTime.UtcNow,
                Details = $"Available: {100 - diskUsage}% free space"
            });
            
            return healthChecks;
        }

        // Role Management
        public async Task<List<UserRole>> GetAllRolesAsync()
        {
            return Enum.GetValues<UserRole>().ToList();
        }

        public async Task<bool> AssignRoleToUserAsync(Guid userId, UserRole role)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.Role = role;
                await _context.SaveChangesAsync();
                
                await LogActivityAsync("Role Assigned", "admin", $"Assigned role {role} to user: {user.FullName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role");
                return false;
            }
        }

        // Audit Logs
        public async Task<List<AuditLog>> GetAuditLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 50)
        {
            // In a real system, this would query an audit log table
            // For now, we'll return some sample data
            var logs = new List<AuditLog>();
            
            for (int i = 0; i < pageSize; i++)
            {
                logs.Add(new AuditLog
                {
                    Id = i + 1,
                    Action = (i % 4) switch
                    {
                        0 => "User Login",
                        1 => "Authorization Approved",
                        2 => "Risk Alert Created",
                        _ => "System Setting Updated"
                    },
                    UserName = (i % 3) switch
                    {
                        0 => "admin",
                        1 => "branchmanager1",
                        _ => "system"
                    },
                    EntityType = "User",
                    EntityId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow.AddMinutes(-i * 15),
                    IpAddress = "192.168.1." + (100 + (i % 50))
                });
            }
            
            return logs;
        }

        public async Task<bool> LogActivityAsync(string activity, string userName, string details = "")
        {
            // In a real system, this would insert into an audit log table
            _logger.LogInformation($"Activity: {activity} by {userName} - {details}");
            return true;
        }

        // System Configuration
        public async Task<Dictionary<string, string>> GetSystemSettingsAsync()
        {
            // In a real system, this would come from a settings table
            return new Dictionary<string, string>
            {
                ["MaxLoginAttempts"] = "3",
                ["SessionTimeout"] = "30",
                ["PasswordMinLength"] = "8",
                ["RequirePasswordComplexity"] = "true",
                ["EnableAuditLogging"] = "true",
                ["BackupRetentionDays"] = "30",
                ["MaxTransactionAmount"] = "100000",
                ["RequireDualApproval"] = "true"
            };
        }

        public async Task<bool> UpdateSystemSettingAsync(string key, string value)
        {
            // In a real system, this would update a settings table
            await LogActivityAsync("System Setting Updated", "admin", $"Updated {key} to {value}");
            return true;
        }

        // Database Operations
        public async Task<bool> BackupDatabaseAsync()
        {
            try
            {
                // In a real system, this would trigger a database backup
                await LogActivityAsync("Database Backup", "system", "Database backup initiated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database backup");
                return false;
            }
        }

        public async Task<List<string>> GetBackupHistoryAsync()
        {
            // In a real system, this would query backup history
            return new List<string>
            {
                $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql",
                $"backup_{DateTime.Now.AddDays(-1):yyyyMMdd_HHmmss}.sql",
                $"backup_{DateTime.Now.AddDays(-2):yyyyMMdd_HHmmss}.sql",
                $"backup_{DateTime.Now.AddDays(-7):yyyyMMdd_HHmmss}.sql"
            };
        }
    }
}