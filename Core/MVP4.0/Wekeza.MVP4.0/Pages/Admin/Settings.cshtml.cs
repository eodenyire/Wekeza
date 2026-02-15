using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class SettingsModel : PageModel
    {
        private readonly IAdminService _adminService;

        public SettingsModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public string UserName { get; set; } = "Admin User";
        public string UserInitials { get; set; } = "AU";
        public int NotificationCount { get; set; } = 5;

        public Dictionary<string, string> SystemSettings { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Admin User";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "AU";
            }

            await LoadSystemSettingsAsync();
        }

        public async Task<IActionResult> OnPostUpdateSettingsAsync()
        {
            try
            {
                var form = Request.Form;
                var updatedSettings = new List<string>();

                foreach (var key in form.Keys)
                {
                    if (key.StartsWith("__") || key == "handler") continue; // Skip system fields
                    
                    var value = form[key].ToString();
                    
                    // Handle checkboxes (they send "true,false" when checked)
                    if (value.Contains(","))
                    {
                        value = value.Split(',')[0]; // Take the first value (true/false)
                    }
                    
                    await _adminService.UpdateSystemSettingAsync(key, value);
                    updatedSettings.Add(key);
                }

                TempData["SuccessMessage"] = $"Updated {updatedSettings.Count} system settings successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating settings: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task LoadSystemSettingsAsync()
        {
            try
            {
                SystemSettings = await _adminService.GetSystemSettingsAsync();
            }
            catch (Exception)
            {
                SystemSettings = new Dictionary<string, string>();
            }
        }
    }
}