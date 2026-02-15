using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Pages.BranchManager
{
    [Authorize(Roles = "BranchManager")]
    public class ReportsModel : PageModel
    {
        private readonly IBranchManagerService _branchManagerService;

        public ReportsModel(IBranchManagerService branchManagerService)
        {
            _branchManagerService = branchManagerService;
        }

        public string UserName { get; set; } = "Branch Manager";
        public string UserInitials { get; set; } = "BM";
        public int NotificationCount { get; set; } = 7;

        public List<Report> RecentReports { get; set; } = new();

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

            // Load real reports from database
            var dbReports = await _branchManagerService.GetRecentReportsAsync(10);
            RecentReports = dbReports.Select(r => new Report
            {
                Id = r.Id,
                Name = r.Name,
                Type = r.Type,
                Generated = r.GeneratedAt.ToString("MMM dd, yyyy HH:mm"),
                Status = r.Status,
                StatusClass = r.Status.ToLower() switch
                {
                    "ready" => "success",
                    "generating" => "warning",
                    "failed" => "danger",
                    _ => "secondary"
                },
                FilePath = r.FilePath,
                FileSize = r.FileSize
            }).ToList();
        }

        public async Task<IActionResult> OnPostGenerateReportAsync(string reportType)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Branch Manager";
                var report = await _branchManagerService.GenerateReportAsync(reportType, userName);
                
                TempData["SuccessMessage"] = $"{reportType.ToUpper()} report generated successfully!";
                return new JsonResult(new { success = true, message = $"{reportType.ToUpper()} report generated successfully!", reportId = report.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
                return new JsonResult(new { success = false, message = $"Error generating report: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnGetDownloadReportAsync(int reportId)
        {
            try
            {
                var reportFile = await _branchManagerService.GetReportFileAsync(reportId);
                if (reportFile == null)
                {
                    TempData["ErrorMessage"] = "Report file not found.";
                    return RedirectToPage();
                }

                var reports = await _branchManagerService.GetRecentReportsAsync(50);
                var report = reports.FirstOrDefault(r => r.Id == reportId);
                if (report == null)
                {
                    TempData["ErrorMessage"] = "Report not found.";
                    return RedirectToPage();
                }

                var fileName = Path.GetFileName(report.FilePath) ?? $"report_{reportId}.csv";
                return File(reportFile, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error downloading report: {ex.Message}";
                return RedirectToPage();
            }
        }

        public class Report
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Generated { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string StatusClass { get; set; } = string.Empty;
            public string? FilePath { get; set; }
            public long FileSize { get; set; }
        }
    }
}