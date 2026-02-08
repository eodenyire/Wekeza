using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Compliance
{
    [Authorize(Roles = "ComplianceOfficer")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Compliance Officer";
        public string UserInitials { get; set; } = "CO";
        public int NotificationCount { get; set; } = 18;
        
        public string AMLAlerts { get; set; } = "24";
        public string KYCPending { get; set; } = "37";
        public string SanctionsHits { get; set; } = "3";
        public string HighRiskAccounts { get; set; } = "15";

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Compliance Officer";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "CO";
            }
        }
    }
}
