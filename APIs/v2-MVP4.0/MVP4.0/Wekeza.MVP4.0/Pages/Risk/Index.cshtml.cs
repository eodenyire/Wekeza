using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Risk
{
    [Authorize(Roles = "RiskOfficer")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Risk Officer";
        public string UserInitials { get; set; } = "RO";
        public int NotificationCount { get; set; } = 14;
        
        public string CreditRiskExposure { get; set; } = "$45.2M";
        public string NPLRatio { get; set; } = "2.3%";
        public string HighRiskAccounts { get; set; } = "28";
        public string ConcentrationRisk { get; set; } = "35%";

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Risk Officer";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "RO";
            }
        }
    }
}
