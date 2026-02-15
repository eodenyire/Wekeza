using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Bancassurance
{
    [Authorize(Roles = "BancassuranceAgent")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Bancassurance Agent";
        public string UserInitials { get; set; } = "BA";
        public int NotificationCount { get; set; } = 6;
        
        public string ActivePolicies { get; set; } = "342";
        public string SalesThisMonth { get; set; } = "$185K";
        public string RenewalsDue { get; set; } = "47";
        public string PendingClaims { get; set; } = "12";

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Bancassurance Agent";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "BA";
            }
        }
    }
}
