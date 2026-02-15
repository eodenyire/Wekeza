using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class AuditModel : PageModel
    {
        private readonly IAdminService _adminService;

        public AuditModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public string UserName { get; set; } = "Admin User";
        public string UserInitials { get; set; } = "AU";
        public int NotificationCount { get; set; } = 5;

        public List<AuditLog> AuditLogs { get; set; } = new();
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? UserNameFilter { get; set; }
        public string? ActionFilter { get; set; }

        public async Task OnGetAsync(DateTime? fromDate, DateTime? toDate, string? userName, string? action)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Admin User";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "AU";
            }

            // Set filter values
            FromDate = fromDate;
            ToDate = toDate;
            UserNameFilter = userName;
            ActionFilter = action;

            await LoadAuditLogsAsync();
        }

        private async Task LoadAuditLogsAsync()
        {
            try
            {
                var logs = await _adminService.GetAuditLogsAsync(FromDate, ToDate, 100);
                
                // Apply additional filters
                if (!string.IsNullOrEmpty(UserNameFilter))
                {
                    logs = logs.Where(l => l.UserName.Contains(UserNameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                if (!string.IsNullOrEmpty(ActionFilter))
                {
                    logs = logs.Where(l => l.Action.Contains(ActionFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                AuditLogs = logs;
            }
            catch (Exception)
            {
                AuditLogs = new List<AuditLog>();
            }
        }
    }
}