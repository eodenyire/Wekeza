using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class UsersModel : PageModel
    {
        private readonly IAdminService _adminService;

        public UsersModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public string UserName { get; set; } = "Admin User";
        public string UserInitials { get; set; } = "AU";
        public int NotificationCount { get; set; } = 5;
        
        public string TotalUsers { get; set; } = "0";
        public string ActiveUsers { get; set; } = "0";
        public string PendingUsers { get; set; } = "0";
        public string LockedUsers { get; set; } = "0";

        public List<ApplicationUser> SystemUsers { get; set; } = new();

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

            await LoadUsersDataAsync();
        }

        public async Task<IActionResult> OnPostCreateUserAsync(string fullName, string email, string username, string password, string role)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage();
            }

            try
            {
                if (Enum.TryParse<UserRole>(role, out var userRole))
                {
                    var user = new ApplicationUser
                    {
                        Username = username,
                        Email = email,
                        FullName = fullName,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                        Role = userRole,
                        IsActive = true
                    };

                    var success = await _adminService.CreateUserAsync(user);
                    if (success)
                    {
                        TempData["SuccessMessage"] = $"User {fullName} created successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create user.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid role selected.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating user: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(Guid userId, string newPassword)
        {
            if (userId == Guid.Empty || string.IsNullOrEmpty(newPassword))
            {
                TempData["ErrorMessage"] = "Invalid user or password.";
                return RedirectToPage();
            }

            try
            {
                var success = await _adminService.ResetPasswordAsync(userId, newPassword);
                if (success)
                {
                    TempData["SuccessMessage"] = "Password reset successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to reset password.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error resetting password: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task LoadUsersDataAsync()
        {
            try
            {
                SystemUsers = await _adminService.GetAllUsersAsync();
                
                TotalUsers = SystemUsers.Count.ToString();
                ActiveUsers = SystemUsers.Count(u => u.IsActive).ToString();
                LockedUsers = SystemUsers.Count(u => !u.IsActive).ToString();
                PendingUsers = "0"; // In a real system, this would be users awaiting approval
            }
            catch (Exception)
            {
                SystemUsers = new List<ApplicationUser>();
                TotalUsers = "0";
                ActiveUsers = "0";
                PendingUsers = "0";
                LockedUsers = "0";
            }
        }

        public string GetRoleClass(UserRole role)
        {
            return role switch
            {
                UserRole.Administrator => "danger",
                UserRole.BranchManager => "primary",
                UserRole.Supervisor => "info",
                UserRole.ComplianceOfficer or UserRole.RiskOfficer or UserRole.Auditor => "warning",
                UserRole.ITAdministrator => "dark",
                _ => "secondary"
            };
        }

        public string GetUserInitials(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts.Length > 0 ? parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper() : "??";
        }
    }
}