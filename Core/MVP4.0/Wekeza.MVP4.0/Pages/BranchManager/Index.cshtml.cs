using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Pages.BranchManager
{
    [Authorize(Roles = "BranchManager")]
    public class IndexModel : PageModel
    {
        private readonly IBranchManagerService _branchManagerService;

        public IndexModel(IBranchManagerService branchManagerService)
        {
            _branchManagerService = branchManagerService;
        }

        public string UserName { get; set; } = "Branch Manager";
        public string UserInitials { get; set; } = "BM";
        public int NotificationCount { get; set; } = 7;
        
        // Core BM Dashboard Properties
        public string PendingAuthorizations { get; set; } = "0";
        public string CashPosition { get; set; } = "$0";
        public string RiskAlerts { get; set; } = "0";
        public string BranchPerformance { get; set; } = "0%";

        // Real data lists
        public List<Authorization> PendingAuthorizationsList { get; set; } = new();
        public List<RiskAlert> RiskAlertsList { get; set; } = new();

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

            // Load real data from database
            await LoadDashboardDataAsync();
        }

        public async Task<IActionResult> OnPostApproveAuthorizationAsync(int authorizationId)
        {
            try
            {
                var success = await _branchManagerService.ApproveAuthorizationAsync(authorizationId, UserName, "Approved by Branch Manager");
                if (success)
                {
                    TempData["SuccessMessage"] = "Authorization approved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to approve authorization.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving authorization: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAuthorizationAsync(int authorizationId)
        {
            try
            {
                var success = await _branchManagerService.RejectAuthorizationAsync(authorizationId, UserName, "Rejected by Branch Manager");
                if (success)
                {
                    TempData["SuccessMessage"] = "Authorization rejected successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to reject authorization.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting authorization: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateRiskAlertAsync(int alertId, string status)
        {
            try
            {
                var success = await _branchManagerService.UpdateRiskAlertStatusAsync(alertId, status, UserName);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Risk alert {status.ToLower()} successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update risk alert.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating risk alert: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task LoadDashboardDataAsync()
        {
            try
            {
                // Load pending authorizations
                PendingAuthorizationsList = await _branchManagerService.GetPendingAuthorizationsAsync();
                
                // Count all items that need BM attention
                var pendingAuthCount = PendingAuthorizationsList.Count;
                
                // Add cash oversight items (simulated - in real system would come from cash operations table)
                var cashOversightCount = 1; // Till opening approval
                
                // Add customer disputes (simulated - in real system would come from disputes table)  
                var customerDisputesCount = 2; // Transaction dispute + KYC update
                
                // Total pending items requiring BM authorization
                var totalPendingItems = pendingAuthCount + cashOversightCount + customerDisputesCount;
                PendingAuthorizations = totalPendingItems.ToString();

                // Load risk alerts
                RiskAlertsList = await _branchManagerService.GetActiveRiskAlertsAsync();
                RiskAlerts = RiskAlertsList.Count.ToString();

                // Load cash position
                var cashPosition = await _branchManagerService.GetCurrentCashPositionAsync();
                var totalCash = cashPosition.VaultCash + cashPosition.TellerCash + cashPosition.ATMCash;
                CashPosition = $"${totalCash:N0}";

                // Get performance metrics
                var metrics = await _branchManagerService.GetBranchPerformanceMetricsAsync();
                BranchPerformance = metrics.ContainsKey("BranchPerformance") ? metrics["BranchPerformance"].ToString() : "94%";
            }
            catch (Exception ex)
            {
                // Log error and use default values
                PendingAuthorizations = "0";
                CashPosition = "$0";
                RiskAlerts = "0";
                BranchPerformance = "0%";
                
                // Log the actual error for debugging
                Console.WriteLine($"Dashboard loading error: {ex.Message}");
            }
        }

        public string GetRiskLevelClass(string riskLevel)
        {
            return riskLevel?.ToLower() switch
            {
                "critical" => "danger",
                "high" => "danger",
                "medium" => "warning",
                "low" => "info",
                _ => "secondary"
            };
        }
    }
}
