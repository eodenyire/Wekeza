using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class ProfileModel : PageModel
    {
        private readonly IAdminService _adminService;

        public ProfileModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public string UserName { get; set; } = "Admin User";
        public string UserInitials { get; set; } = "AU";
        public int NotificationCount { get; set; } = 5;
        
        // Profile Information
        public string FullName { get; set; } = "System Administrator";
        public string Email { get; set; } = "admin@wekeza.com";
        public string Username { get; set; } = "admin";
        public string LastLogin { get; set; } = "Today, 09:30 AM";
        public string AccountCreated { get; set; } = "Jan 15, 2024";
        public int TotalSessions { get; set; } = 127;

        public async Task OnGetAsync()
        {
            // Get user info from claims
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Admin User";
                Username = UserName.ToLower();
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "AU";
                
                FullName = UserName;
                Email = $"{Username}@wekeza.com";
            }

            // Load additional profile data
            await LoadProfileDataAsync();
        }

        private async Task LoadProfileDataAsync()
        {
            try
            {
                // In a real system, this would load actual user profile data
                LastLogin = DateTime.Now.ToString("MMM dd, yyyy HH:mm");
                AccountCreated = "Jan 15, 2024";
                TotalSessions = 127;
            }
            catch (Exception)
            {
                // Fall back to default values if there's an error
                LastLogin = "Unknown";
                AccountCreated = "Unknown";
                TotalSessions = 0;
            }
        }
    }
}