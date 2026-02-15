using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly IAdminService _adminService;

        public IndexModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public string UserName { get; set; } = "Admin User";
        public string UserInitials { get; set; } = "AU";
        public int NotificationCount { get; set; } = 5;
        
        // Dashboard Metrics
        public string TotalUsers { get; set; } = "0";
        public string ActiveAccounts { get; set; } = "0";
        public string TotalTransactions { get; set; } = "0";
        public string ActiveLoans { get; set; } = "0";
        
        public List<ActivityLog> RecentActivities { get; set; } = new();
        public List<SystemHealthCheck> SystemHealth { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Get user info from claims
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Admin User";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "AU";
            }

            // Load real dashboard data
            await LoadDashboardDataAsync();
        }

        private async Task LoadDashboardDataAsync()
        {
            try
            {
                // Get system statistics
                var stats = await _adminService.GetSystemStatisticsAsync();
                
                TotalUsers = stats.ContainsKey("TotalUsers") ? stats["TotalUsers"].ToString() : "0";
                ActiveAccounts = stats.ContainsKey("ActiveAccounts") ? FormatNumber((int)stats["ActiveAccounts"]) : "0";
                TotalTransactions = stats.ContainsKey("TotalTransactions") ? FormatNumber((int)stats["TotalTransactions"]) : "0";
                ActiveLoans = stats.ContainsKey("ActiveLoans") ? stats["ActiveLoans"].ToString() : "0";

                // Load recent activities
                RecentActivities = await _adminService.GetRecentActivitiesAsync(5);
                
                // Load system health
                SystemHealth = await _adminService.GetSystemHealthAsync();
            }
            catch (Exception)
            {
                // Fall back to default values if there's an error
                TotalUsers = "0";
                ActiveAccounts = "0";
                TotalTransactions = "0";
                ActiveLoans = "0";
                RecentActivities = new List<ActivityLog>();
                SystemHealth = new List<SystemHealthCheck>();
            }
        }

        private string FormatNumber(int number)
        {
            if (number >= 1000000)
                return $"{number / 1000000.0:F1}M";
            if (number >= 1000)
                return $"{number / 1000.0:F1}K";
            return number.ToString();
        }
    }
}
