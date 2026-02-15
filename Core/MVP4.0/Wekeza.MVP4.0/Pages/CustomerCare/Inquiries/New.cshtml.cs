using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.CustomerCare.Inquiries
{
    [Authorize(Roles = "CustomerCareOfficer")]
    public class NewModel : PageModel
    {
        public string UserName { get; set; } = "Customer Care Officer";
        public string UserInitials { get; set; } = "CC";
        public int NotificationCount { get; set; } = 15;

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Customer Care Officer";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "CC";
            }
        }
    }
}