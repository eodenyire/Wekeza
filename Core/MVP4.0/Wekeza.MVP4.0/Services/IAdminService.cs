using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services
{
    public interface IAdminService
    {
        // User Management
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
        Task<bool> CreateUserAsync(ApplicationUser user);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> ResetPasswordAsync(Guid userId, string newPassword);
        Task<bool> LockUserAsync(Guid userId);
        Task<bool> UnlockUserAsync(Guid userId);
        
        // System Statistics
        Task<Dictionary<string, object>> GetSystemStatisticsAsync();
        Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10);
        Task<List<SystemHealthCheck>> GetSystemHealthAsync();
        
        // Role Management
        Task<List<UserRole>> GetAllRolesAsync();
        Task<bool> AssignRoleToUserAsync(Guid userId, UserRole role);
        
        // Audit Logs
        Task<List<AuditLog>> GetAuditLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 50);
        Task<bool> LogActivityAsync(string activity, string userName, string details = "");
        
        // System Configuration
        Task<Dictionary<string, string>> GetSystemSettingsAsync();
        Task<bool> UpdateSystemSettingAsync(string key, string value);
        
        // Database Operations
        Task<bool> BackupDatabaseAsync();
        Task<List<string>> GetBackupHistoryAsync();
    }
    
    public class ActivityLog
    {
        public int Id { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string TimeAgo => GetTimeAgo();
        
        private string GetTimeAgo()
        {
            var timeSpan = DateTime.UtcNow - CreatedAt;
            if (timeSpan.TotalMinutes < 1) return "Just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} mins ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            return $"{(int)timeSpan.TotalDays} days ago";
        }
    }
    
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string OldValues { get; set; } = string.Empty;
        public string NewValues { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; } = string.Empty;
    }
    
    public class SystemHealthCheck
    {
        public string Component { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusClass { get; set; } = string.Empty;
        public DateTime LastCheck { get; set; } = DateTime.UtcNow;
        public string Details { get; set; } = string.Empty;
    }
}