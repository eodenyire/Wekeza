using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.BackOffice
{
    [Authorize(Roles = "BackOfficeStaff")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Back Office Staff";
        public string UserInitials { get; set; } = "BO";
        public int NotificationCount { get; set; } = 11;
        
        public string PendingTasks { get; set; } = "18";
        public string ProcessedToday { get; set; } = "247";
        public string ReconciliationStatus { get; set; } = "85%";
        public string Exceptions { get; set; } = "5";

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Back Office Staff";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "BO";
            }
        }
    }
}
