using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Pages.Admin.Users
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly IAdminService _adminService;

        public CreateModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public string UserName { get; set; } = "Admin User";
        public string UserInitials { get; set; } = "AU";
        public int NotificationCount { get; set; } = 5;

        public async Task OnGetAsync()
        {
            // Get user info from claims
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Admin User";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "AU";
            }
        }
    }
}