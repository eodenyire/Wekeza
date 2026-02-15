using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.ITAdmin
{
    [Authorize(Roles = "ITAdministrator")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "IT Administrator";
        public string UserInitials { get; set; } = "IT";
        public int NotificationCount { get; set; } = 8;
        
        public string SystemUptime { get; set; } = "99.98%";
        public string DatabaseHealth { get; set; } = "100%";
        public string ActiveUsers { get; set; } = "247";
        public string ErrorLogs { get; set; } = "12";
        
        public List<ErrorLog> RecentErrors { get; set; } = new();

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "IT Administrator";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "IT";
            }

            RecentErrors = new List<ErrorLog>
            {
                new ErrorLog { Time = "10:45 AM", Type = "Database", Message = "Connection timeout", Severity = "Warning", SeverityClass = "warning" },
                new ErrorLog { Time = "09:30 AM", Type = "Application", Message = "Null reference exception", Severity = "Error", SeverityClass = "danger" },
                new ErrorLog { Time = "08:15 AM", Type = "Network", Message = "High latency detected", Severity = "Info", SeverityClass = "info" }
            };
        }

        public class ErrorLog
        {
            public string Time { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string Severity { get; set; } = string.Empty;
            public string SeverityClass { get; set; } = string.Empty;
        }
    }
}
