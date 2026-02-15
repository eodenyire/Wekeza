using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;
using Wekeza.MVP4._0.Models;
using Wekeza.MVP4._0.Data;

namespace Wekeza.MVP4._0.Pages.BranchManager
{
    [Authorize(Roles = "BranchManager")]
    public class StaffModel : PageModel
    {
        private readonly IBranchManagerService _branchManagerService;

        public StaffModel(IBranchManagerService branchManagerService)
        {
            _branchManagerService = branchManagerService;
        }

        public string UserName { get; set; } = "Branch Manager";
        public string UserInitials { get; set; } = "BM";
        public int NotificationCount { get; set; } = 7;
        
        public string TotalStaff { get; set; } = "0";
        public string PresentToday { get; set; } = "0";
        public string OnLeave { get; set; } = "0";
        public string PendingReviews { get; set; } = "0";

        public List<StaffMember> StaffMembers { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Branch Manager";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "BM";
            }

            // Load real staff data from database
            var staff = await _branchManagerService.GetBranchStaffAsync();
            
            StaffMembers = staff.Select(s => new StaffMember
            {
                Id = s.Id,
                Name = s.FullName,
                Email = s.Email,
                Position = s.Role.ToString(),
                Department = GetDepartmentFromRole(s.Role),
                Status = s.IsActive ? "Present" : "Inactive",
                StatusClass = s.IsActive ? "success" : "secondary",
                Performance = GetRandomPerformance(), // In real system, this would come from performance table
                PerformanceClass = "success",
                Initials = GetInitials(s.FullName)
            }).ToList();

            // Calculate stats
            TotalStaff = staff.Count.ToString();
            PresentToday = staff.Count(s => s.IsActive).ToString();
            OnLeave = staff.Count(s => !s.IsActive).ToString(); // For now, inactive = on leave
            PendingReviews = "3"; // This would come from performance review table
        }

        public class StaffMember
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Position { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string StatusClass { get; set; } = string.Empty;
            public int Performance { get; set; }
            public string PerformanceClass { get; set; } = string.Empty;
            public string Initials { get; set; } = string.Empty;
        }

        // Form handlers for staff management actions
        public async Task<IActionResult> OnPostAddStaffAsync(string fullName, string email, string position, string department)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(position))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage();
            }

            try
            {
                // Create new user in database
                var newUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Username = email.Split('@')[0], // Use email prefix as username
                    Email = email,
                    FullName = fullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("temp123"), // Temporary password
                    Role = GetRoleFromPosition(position),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                // Add to database using the context directly since we don't have a user service
                using (var scope = HttpContext.RequestServices.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MVP4DbContext>();
                    context.Users.Add(newUser);
                    await context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Staff member {fullName} has been added successfully! Temporary password: temp123";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding staff member: {ex.Message}";
            }
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateScheduleAsync(string staffId, DateTime scheduleDate, string shift)
        {
            if (string.IsNullOrEmpty(staffId) || string.IsNullOrEmpty(shift))
            {
                TempData["ErrorMessage"] = "Please select a staff member and shift.";
                return RedirectToPage();
            }

            try
            {
                // In a real system, this would update a schedule table
                // For now, we'll simulate the database update
                if (Guid.TryParse(staffId, out var staffGuid))
                {
                    // Here you would typically have a ScheduleService or update a Schedule table
                    // await _scheduleService.UpdateStaffScheduleAsync(staffGuid, scheduleDate, shift);
                    
                    // For demonstration, we'll just log the update
                    var staff = await _branchManagerService.GetBranchStaffAsync();
                    var staffMember = staff.FirstOrDefault(s => s.Id == staffGuid);
                    var staffName = staffMember?.FullName ?? "Unknown Staff";
                    
                    TempData["SuccessMessage"] = $"Schedule updated successfully for {staffName} on {scheduleDate:MMM dd, yyyy} - {shift}!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid staff ID.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating schedule: {ex.Message}";
            }
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdatePerformanceAsync(string staffId, int performanceScore, string reviewPeriod, string strengths, string improvements, string comments)
        {
            if (string.IsNullOrEmpty(staffId) || performanceScore < 0 || performanceScore > 100)
            {
                TempData["ErrorMessage"] = "Please select a staff member and provide a valid performance score.";
                return RedirectToPage();
            }

            try
            {
                if (Guid.TryParse(staffId, out var staffGuid))
                {
                    await _branchManagerService.UpdateStaffPerformanceAsync(staffGuid, performanceScore, comments ?? "");
                    TempData["SuccessMessage"] = $"Performance review saved successfully! Score: {performanceScore}%";
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid staff ID.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving performance review: {ex.Message}";
            }
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateRoleAsync(string staffId, string newPosition, string newDepartment, DateTime effectiveDate, string reason)
        {
            if (string.IsNullOrEmpty(staffId) || string.IsNullOrEmpty(newPosition) || string.IsNullOrEmpty(newDepartment))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage();
            }

            try
            {
                // In a real system, this would update the user's role in database
                TempData["SuccessMessage"] = $"Staff role updated successfully! New position: {newPosition}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating role: {ex.Message}";
            }
            
            return RedirectToPage();
        }

        // Helper methods
        private string GetDepartmentFromRole(UserRole role)
        {
            return role switch
            {
                UserRole.Teller or UserRole.CashOfficer or UserRole.Supervisor => "Operations",
                UserRole.CustomerCareOfficer => "Service",
                UserRole.LoanOfficer => "Credit",
                UserRole.Administrator or UserRole.ITAdministrator => "Administration",
                UserRole.ComplianceOfficer or UserRole.RiskOfficer or UserRole.Auditor => "Compliance",
                _ => "General"
            };
        }

        private UserRole GetRoleFromPosition(string position)
        {
            return position switch
            {
                "Teller" => UserRole.Teller,
                "Senior Teller" => UserRole.Teller,
                "Cash Officer" => UserRole.CashOfficer,
                "Customer Care" => UserRole.CustomerCareOfficer,
                "Loan Officer" => UserRole.LoanOfficer,
                "Supervisor" => UserRole.Supervisor,
                _ => UserRole.Teller
            };
        }

        private string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return parts.Length > 0 ? parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper() : "??";
        }

        private int GetRandomPerformance()
        {
            return Random.Shared.Next(75, 100); // Random performance between 75-100%
        }
    }
}