using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.BranchManager
{
    [Authorize(Roles = "BranchManager")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Branch Manager";
        public string UserInitials { get; set; } = "BM";
        public int NotificationCount { get; set; } = 7;
        
        public string BranchRevenue { get; set; } = "$245K";
        public string TotalDeposits { get; set; } = "$1.8M";
        public string ActiveStaff { get; set; } = "28";
        public string CustomerSatisfaction { get; set; } = "4.8/5";

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
