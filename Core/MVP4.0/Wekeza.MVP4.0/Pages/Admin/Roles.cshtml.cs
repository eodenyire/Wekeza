using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class RolesModel : PageModel
    {
        private readonly IAdminService _adminService;

        public RolesModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public string UserName { get; set; } = "Admin User";
        public string UserInitials { get; set; } = "AU";
        public int NotificationCount { get; set; } = 5;

        public List<ApplicationUser> AllUsers { get; set; } = new();
        public List<UserRole> AvailableRoles { get; set; } = new();
        public Dictionary<string, int> RoleStatistics { get; set; } = new();

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

            await LoadRoleDataAsync();
        }

        public async Task<IActionResult> OnPostAssignRoleAsync(Guid userId, string newRole, string reason)
        {
            if (userId == Guid.Empty || string.IsNullOrEmpty(newRole))
            {
                TempData["ErrorMessage"] = "Please select a user and role.";
                return RedirectToPage();
            }

            try
            {
                if (Enum.TryParse<UserRole>(newRole, out var role))
                {
                    var success = await _adminService.AssignRoleToUserAsync(userId, role);
                    if (success)
                    {
                        TempData["SuccessMessage"] = $"Role {newRole} assigned successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to assign role.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid role selected.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error assigning role: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task LoadRoleDataAsync()
        {
            try
            {
                // Load all users
                AllUsers = await _adminService.GetAllUsersAsync();
                
                // Load available roles
                AvailableRoles = await _adminService.GetAllRolesAsync();
                
                // Calculate role statistics
                RoleStatistics = AllUsers
                    .GroupBy(u => u.Role.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());
                
                // Ensure all roles are represented
                foreach (var role in AvailableRoles)
                {
                    if (!RoleStatistics.ContainsKey(role.ToString()))
                    {
                        RoleStatistics[role.ToString()] = 0;
                    }
                }
            }
            catch (Exception)
            {
                AllUsers = new List<ApplicationUser>();
                AvailableRoles = new List<UserRole>();
                RoleStatistics = new Dictionary<string, int>();
            }
        }
    }
}