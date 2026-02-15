using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Controllers
{
    [Authorize(Roles = "BranchManager")]
    [ApiController]
    [Route("api/[controller]")]
    public class BranchManagerController : ControllerBase
    {
        private readonly IBranchManagerService _branchManagerService;

        public BranchManagerController(IBranchManagerService branchManagerService)
        {
            _branchManagerService = branchManagerService;
        }

        [HttpPost("authorize/{authorizationId}/approve")]
        public async Task<IActionResult> ApproveAuthorization(int authorizationId)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Branch Manager";
                var success = await _branchManagerService.ApproveAuthorizationAsync(authorizationId, userName, "Approved by Branch Manager");
                
                if (success)
                {
                    return Ok(new { success = true, message = "Authorization approved successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to approve authorization." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("authorize/{authorizationId}/reject")]
        public async Task<IActionResult> RejectAuthorization(int authorizationId)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Branch Manager";
                var success = await _branchManagerService.RejectAuthorizationAsync(authorizationId, userName, "Rejected by Branch Manager");
                
                if (success)
                {
                    return Ok(new { success = true, message = "Authorization rejected successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to reject authorization." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("risk-alert/{alertId}/update")]
        public async Task<IActionResult> UpdateRiskAlert(int alertId, [FromBody] UpdateRiskAlertRequest request)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Branch Manager";
                var success = await _branchManagerService.UpdateRiskAlertStatusAsync(alertId, request.Status, userName, request.Resolution);
                
                if (success)
                {
                    return Ok(new { success = true, message = $"Risk alert {request.Status.ToLower()} successfully!" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to update risk alert." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("dashboard-data")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var pendingAuthorizations = await _branchManagerService.GetPendingAuthorizationsAsync();
                var riskAlerts = await _branchManagerService.GetActiveRiskAlertsAsync();
                var cashPosition = await _branchManagerService.GetCurrentCashPositionAsync();
                var metrics = await _branchManagerService.GetBranchPerformanceMetricsAsync();

                var totalCash = cashPosition.VaultCash + cashPosition.TellerCash + cashPosition.ATMCash;
                
                // Count all items needing BM attention
                var totalPendingItems = pendingAuthorizations.Count + 1 + 2; // auths + cash oversight + disputes

                return Ok(new
                {
                    pendingAuthorizations = totalPendingItems,
                    riskAlerts = riskAlerts.Count,
                    cashPosition = $"${totalCash:N0}",
                    branchPerformance = metrics.ContainsKey("BranchPerformance") ? metrics["BranchPerformance"].ToString() : "94%"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("generate-report")]
        public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequest request)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Branch Manager";
                var report = await _branchManagerService.GenerateReportAsync(request.ReportType, userName);
                
                return Ok(new { success = true, message = $"{request.ReportType.ToUpper()} report generated successfully!", reportId = report.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error generating report: {ex.Message}" });
            }
        }

        [HttpGet("download-report/{reportId}")]
        public async Task<IActionResult> DownloadReport(int reportId)
        {
            try
            {
                var reportFile = await _branchManagerService.GetReportFileAsync(reportId);
                if (reportFile == null)
                {
                    return NotFound(new { success = false, message = "Report file not found." });
                }

                var reports = await _branchManagerService.GetRecentReportsAsync(50);
                var report = reports.FirstOrDefault(r => r.Id == reportId);
                if (report == null)
                {
                    return NotFound(new { success = false, message = "Report not found." });
                }

                var fileName = Path.GetFileName(report.FilePath) ?? $"report_{reportId}.csv";
                return File(reportFile, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error downloading report: {ex.Message}" });
            }
        }
    }

    public class UpdateRiskAlertRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Resolution { get; set; }
    }
}