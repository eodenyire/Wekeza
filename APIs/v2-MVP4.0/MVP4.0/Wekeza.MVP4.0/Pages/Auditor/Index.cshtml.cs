using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Auditor
{
    [Authorize(Roles = "Auditor")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Auditor";
        public string UserInitials { get; set; } = "AD";
        public int NotificationCount { get; set; } = 10;
        
        public string TotalAuditEntries { get; set; } = "152.4K";
        public string TodayActivities { get; set; } = "3,247";
        public string FlaggedActivities { get; set; } = "12";
        public string CriticalChanges { get; set; } = "8";
        
        public List<ChangeLog> RecentChanges { get; set; } = new();

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Auditor";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "AD";
            }

            RecentChanges = new List<ChangeLog>
            {
                new ChangeLog { Time = "10:45 AM", User = "admin", Action = "Update", Entity = "User Settings" },
                new ChangeLog { Time = "10:30 AM", User = "teller1", Action = "Create", Entity = "Transaction" },
                new ChangeLog { Time = "10:15 AM", User = "manager", Action = "Approve", Entity = "Loan Application" },
                new ChangeLog { Time = "10:00 AM", User = "system", Action = "Backup", Entity = "Database" },
                new ChangeLog { Time = "09:45 AM", User = "admin", Action = "Delete", Entity = "Old Logs" }
            };
        }

        public class ChangeLog
        {
            public string Time { get; set; } = string.Empty;
            public string User { get; set; } = string.Empty;
            public string Action { get; set; } = string.Empty;
            public string Entity { get; set; } = string.Empty;
        }
    }
}
