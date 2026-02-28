using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Admin User";
        public string UserInitials { get; set; } = "AU";
        public int NotificationCount { get; set; } = 5;
        
        // Dashboard Metrics
        public string TotalUsers { get; set; } = "1,245";
        public string ActiveAccounts { get; set; } = "3,892";
        public string TotalTransactions { get; set; } = "15.2K";
        public string ActiveLoans { get; set; } = "487";
        
        public List<ActivityLog> RecentActivities { get; set; } = new();

        public void OnGet()
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

            // Load recent activities (sample data)
            RecentActivities = new List<ActivityLog>
            {
                new ActivityLog { Description = "New user created", UserName = "system", TimeAgo = "5 mins ago" },
                new ActivityLog { Description = "System settings updated", UserName = "admin", TimeAgo = "1 hour ago" },
                new ActivityLog { Description = "Database backup completed", UserName = "system", TimeAgo = "2 hours ago" },
                new ActivityLog { Description = "Security policy updated", UserName = "admin", TimeAgo = "3 hours ago" },
                new ActivityLog { Description = "User permissions modified", UserName = "admin", TimeAgo = "4 hours ago" }
            };
        }

        public class ActivityLog
        {
            public string Description { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string TimeAgo { get; set; } = string.Empty;
        }
    }
}
