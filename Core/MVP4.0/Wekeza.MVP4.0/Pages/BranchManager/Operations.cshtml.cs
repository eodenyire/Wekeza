using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.BranchManager
{
    [Authorize(Roles = "BranchManager")]
    public class OperationsModel : PageModel
    {
        public string UserName { get; set; } = "Branch Manager";
        public string UserInitials { get; set; } = "BM";
        public int NotificationCount { get; set; } = 7;
        
        // Risk & Compliance Properties
        public string AMLAlerts { get; set; } = "4";
        public string SuspiciousTransactions { get; set; } = "7";
        public string DormantAccounts { get; set; } = "23";
        public string ComplianceScore { get; set; } = "96%";

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Branch Manager";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "BM";
            }
        }
    }
}