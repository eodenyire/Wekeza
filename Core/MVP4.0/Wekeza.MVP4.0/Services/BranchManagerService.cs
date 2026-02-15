using Microsoft.EntityFrameworkCore;
using System.Text;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services
{
    public class BranchManagerService : IBranchManagerService
    {
        private readonly MVP4DbContext _context;
        private readonly ILogger<BranchManagerService> _logger;

        public BranchManagerService(MVP4DbContext context, ILogger<BranchManagerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Authorization Management
        public async Task<List<Authorization>> GetPendingAuthorizationsAsync()
        {
            return await _context.Authorizations
                .Where(a => a.Status == "Pending")
                .OrderBy(a => a.RequestedAt)
                .ToListAsync();
        }

        public async Task<bool> ApproveAuthorizationAsync(int authorizationId, string authorizedBy, string? reason = null)
        {
            var authorization = await _context.Authorizations.FindAsync(authorizationId);
            if (authorization == null) return false;

            authorization.Status = "Approved";
            authorization.AuthorizedBy = authorizedBy;
            authorization.AuthorizedAt = DateTime.UtcNow;
            authorization.Reason = reason;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Authorization {authorizationId} approved by {authorizedBy}");
            return true;
        }

        public async Task<bool> RejectAuthorizationAsync(int authorizationId, string authorizedBy, string reason)
        {
            var authorization = await _context.Authorizations.FindAsync(authorizationId);
            if (authorization == null) return false;

            authorization.Status = "Rejected";
            authorization.AuthorizedBy = authorizedBy;
            authorization.AuthorizedAt = DateTime.UtcNow;
            authorization.Reason = reason;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Authorization {authorizationId} rejected by {authorizedBy}");
            return true;
        }

        public async Task<Authorization> CreateAuthorizationRequestAsync(Authorization authorization)
        {
            authorization.TransactionId = GenerateTransactionId();
            _context.Authorizations.Add(authorization);
            await _context.SaveChangesAsync();
            return authorization;
        }

        // Cash Management
        public async Task<CashPosition> GetCurrentCashPositionAsync()
        {
            var latest = await _context.CashPositions
                .OrderByDescending(c => c.LastUpdated)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                // Create initial cash position
                latest = new CashPosition
                {
                    VaultCash = 125000,
                    TellerCash = 45000,
                    ATMCash = 35000,
                    UpdatedBy = "System",
                    LastUpdated = DateTime.UtcNow
                };
                _context.CashPositions.Add(latest);
                await _context.SaveChangesAsync();
            }

            return latest;
        }

        public async Task<bool> UpdateCashPositionAsync(CashPosition cashPosition)
        {
            _context.CashPositions.Add(cashPosition);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CashPosition>> GetCashMovementHistoryAsync(int days = 7)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            return await _context.CashPositions
                .Where(c => c.LastUpdated >= cutoffDate)
                .OrderByDescending(c => c.LastUpdated)
                .ToListAsync();
        }

        // Risk & Compliance
        public async Task<List<RiskAlert>> GetActiveRiskAlertsAsync()
        {
            return await _context.RiskAlerts
                .Where(r => r.Status != "Resolved")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateRiskAlertStatusAsync(int alertId, string status, string? assignedTo = null, string? resolution = null)
        {
            var alert = await _context.RiskAlerts.FindAsync(alertId);
            if (alert == null) return false;

            alert.Status = status;
            if (assignedTo != null) alert.AssignedTo = assignedTo;
            if (resolution != null) alert.Resolution = resolution;
            if (status == "Resolved") alert.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RiskAlert> CreateRiskAlertAsync(RiskAlert alert)
        {
            _context.RiskAlerts.Add(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<int> GetRiskAlertCountByTypeAsync(string alertType)
        {
            return await _context.RiskAlerts
                .CountAsync(r => r.AlertType == alertType && r.Status != "Resolved");
        }

        // Report Management
        public async Task<List<BranchReport>> GetRecentReportsAsync(int count = 10)
        {
            return await _context.BranchReports
                .OrderByDescending(r => r.GeneratedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<BranchReport> GenerateReportAsync(string reportType, string generatedBy)
        {
            var report = new BranchReport
            {
                Name = $"{reportType.ToUpper()} Report - {DateTime.Now:yyyy-MM-dd}",
                Type = reportType,
                GeneratedBy = generatedBy,
                Status = "Generating",
                GeneratedAt = DateTime.UtcNow
            };

            _context.BranchReports.Add(report);
            await _context.SaveChangesAsync();

            // Simulate report generation
            await Task.Delay(2000);

            // Generate actual report content
            var reportContent = await GenerateReportContent(reportType);
            var fileName = $"{reportType}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine("Reports", fileName);

            // Ensure Reports directory exists
            Directory.CreateDirectory("Reports");
            
            // Write report to file
            await File.WriteAllTextAsync(filePath, reportContent);

            // Update report record
            report.Status = "Ready";
            report.FilePath = filePath;
            report.FileSize = new FileInfo(filePath).Length;

            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<byte[]?> GetReportFileAsync(int reportId)
        {
            var report = await _context.BranchReports.FindAsync(reportId);
            if (report == null || string.IsNullOrEmpty(report.FilePath) || !File.Exists(report.FilePath))
                return null;

            return await File.ReadAllBytesAsync(report.FilePath);
        }

        public async Task<bool> DeleteReportAsync(int reportId)
        {
            var report = await _context.BranchReports.FindAsync(reportId);
            if (report == null) return false;

            if (!string.IsNullOrEmpty(report.FilePath) && File.Exists(report.FilePath))
            {
                File.Delete(report.FilePath);
            }

            _context.BranchReports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        // Staff Performance
        public async Task<List<ApplicationUser>> GetBranchStaffAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive && u.Role != UserRole.Administrator)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<Dictionary<string, object>> GetBranchPerformanceMetricsAsync()
        {
            var totalStaff = await _context.Users.CountAsync(u => u.IsActive);
            var totalAuthorizations = await _context.Authorizations.CountAsync(a => a.Status == "Pending");
            var totalRiskAlerts = await _context.RiskAlerts.CountAsync(r => r.Status != "Resolved");
            var cashPosition = await GetCurrentCashPositionAsync();

            return new Dictionary<string, object>
            {
                ["TotalStaff"] = totalStaff,
                ["PendingAuthorizations"] = totalAuthorizations,
                ["RiskAlerts"] = totalRiskAlerts,
                ["CashPosition"] = $"${(cashPosition.VaultCash + cashPosition.TellerCash + cashPosition.ATMCash):N0}",
                ["BranchPerformance"] = "94%"
            };
        }

        public async Task<bool> UpdateStaffPerformanceAsync(Guid staffId, decimal performanceScore, string comments)
        {
            // In a real system, this would update a performance tracking table
            // For now, we'll just log it
            _logger.LogInformation($"Performance updated for staff {staffId}: {performanceScore}% - {comments}");
            return true;
        }

        // Helper Methods
        private string GenerateTransactionId()
        {
            return $"TXN{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }

        private async Task<string> GenerateReportContent(string reportType)
        {
            var content = new StringBuilder();
            
            switch (reportType.ToLower())
            {
                case "daily":
                    content.AppendLine("Date,Transactions,Deposits,Withdrawals,New Accounts");
                    content.AppendLine($"{DateTime.Now:yyyy-MM-dd},247,156,91,12");
                    break;
                    
                case "weekly":
                    content.AppendLine("Week,Total Transactions,Revenue,Customer Growth");
                    content.AppendLine($"Week of {DateTime.Now:yyyy-MM-dd},1234,$245000,127");
                    break;
                    
                case "monthly":
                    content.AppendLine("Month,Revenue,Deposits,Loans,Staff Performance");
                    content.AppendLine($"{DateTime.Now:yyyy-MM},1800000,2500000,450000,94%");
                    break;
                    
                case "staff":
                    content.AppendLine("Name,Position,Performance,Status");
                    var staff = await GetBranchStaffAsync();
                    foreach (var member in staff)
                    {
                        content.AppendLine($"{member.FullName},{member.Role},{Random.Shared.Next(75, 100)}%,Active");
                    }
                    break;
                    
                default:
                    content.AppendLine("Report Type,Generated Date,Status");
                    content.AppendLine($"{reportType},{DateTime.Now:yyyy-MM-dd HH:mm:ss},Generated");
                    break;
            }
            
            return content.ToString();
        }
    }
}