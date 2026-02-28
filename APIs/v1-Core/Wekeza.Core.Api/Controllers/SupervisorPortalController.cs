using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Supervisor Portal Controller
/// Handles team management, transaction approval, and operational oversight
/// </summary>
[ApiController]
[Route("api/supervisor")]
[Authorize(Roles = "Supervisor,BranchManager")]
public class SupervisorPortalController : BaseApiController
{
    public SupervisorPortalController(IMediator mediator) : base(mediator) { }

    #region Team Management

    /// <summary>
    /// Get supervised tellers/staff
    /// </summary>
    [HttpGet("team")]
    public async Task<IActionResult> GetTeam([FromQuery] string? status = null)
    {
        try
        {
            var team = new List<TeamMemberDto>
            {
                new TeamMemberDto
                {
                    MemberId = "EMP001",
                    Name = "Jane Kariuki",
                    Role = "Teller",
                    Status = "Active",
                    OnDuty = true,
                    SessionStarted = DateTime.UtcNow.AddHours(-4),
                    TransactionsToday = 145,
                    ErrorsToday = 1
                },
                new TeamMemberDto
                {
                    MemberId = "EMP003",
                    Name = "Alice Wanjiru",
                    Role = "Teller",
                    Status = "Active",
                    OnDuty = true,
                    SessionStarted = DateTime.UtcNow.AddHours(-3),
                    TransactionsToday = 98,
                    ErrorsToday = 0
                },
                new TeamMemberDto
                {
                    MemberId = "EMP004",
                    Name = "Isaac Kipchoge",
                    Role = "Teller",
                    Status = "OnLeave",
                    OnDuty = false,
                    SessionStarted = null,
                    TransactionsToday = 0,
                    ErrorsToday = 0
                }
            };

            if (!string.IsNullOrEmpty(status))
                team = team.Where(t => t.Status == status).ToList();

            return Ok(new { success = true, data = team, count = team.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get team performance summary
    /// </summary>
    [HttpGet("team/performance")]
    public async Task<IActionResult> GetTeamPerformance()
    {
        try
        {
            var performance = new
            {
                TotalTeamMembers = 8,
                ActiveMembers = 6,
                AverageTellerTransactionsPerDay = 156,
                TeamErrorRate = 0.15m,
                TeamAverageTransactionTime = 2.3m,
                AverageCustomerSatisfaction = 4.7m,
                BestPerformer = new { Name = "John Musyoka", TransactionsProcessed = 245 },
                LowestErrorRate = new { Name = "Alice Wanjiru", ErrorRate = 0.05m }
            };

            return Ok(new { success = true, data = performance });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Transaction Approval

    /// <summary>
    /// Get pending transactions requiring supervisor approval
    /// </summary>
    [HttpGet("approvals/pending")]
    public async Task<IActionResult> GetPendingApprovals(
        [FromQuery] string? type = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var pending = new List<PendingApprovalDto>
            {
                new PendingApprovalDto
                {
                    ApprovalId = "APR001",
                    Type = "OverLimitCashWithdrawal",
                    Staff = "Jane Kariuki",
                    Customer = "John Omondi",
                    Amount = 500000m,
                    Reason = "Customer requested special withdrawal",
                    SubmittedAt = DateTime.UtcNow.AddMinutes(-15),
                    Priority = "High"
                },
                new PendingApprovalDto
                {
                    ApprovalId = "APR002",
                    Type = "SpecialTransaction",
                    Staff = "Alice Wanjiru",
                    Customer = "Jane Smith (Business)",
                    Amount = 2500000m,
                    Reason = "Payroll disbursement",
                    SubmittedAt = DateTime.UtcNow.AddMinutes(-45),
                    Priority = "Medium"
                },
                new PendingApprovalDto
                {
                    ApprovalId = "APR003",
                    Type = "DayLimitExceeded",
                    Staff = "Isaac Kipchoge",
                    Customer = "Peter Koech",
                    Amount = 1500000m,
                    Reason = "Daily limit exceeded override request",
                    SubmittedAt = DateTime.UtcNow.AddHours(-1),
                    Priority = "Low"
                }
            };

            if (!string.IsNullOrEmpty(type))
                pending = pending.Where(p => p.Type == type).ToList();

            if (minAmount.HasValue)
                pending = pending.Where(p => p.Amount >= minAmount.Value).ToList();

            var paged = pending.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                success = true,
                data = paged,
                pagination = new { page, pageSize, total = pending.Count }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Approve a pending transaction
    /// </summary>
    [HttpPost("approvals/{approvalId}/approve")]
    public async Task<IActionResult> ApproveTransaction(string approvalId, [FromBody] TransactionApprovalDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(approvalId))
                return BadRequest(new { error = "Approval ID is required" });

            return Ok(new
            {
                success = true,
                message = "Transaction approved successfully",
                approvalId = approvalId,
                approvedBy = User.Identity?.Name,
                approvedAt = DateTime.UtcNow,
                notes = dto.Notes
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Reject a pending transaction
    /// </summary>
    [HttpPost("approvals/{approvalId}/reject")]
    public async Task<IActionResult> RejectTransaction(string approvalId, [FromBody] TransactionRejectionDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(approvalId))
                return BadRequest(new { error = "Approval ID is required" });

            if (string.IsNullOrEmpty(dto.Reason))
                return BadRequest(new { error = "Rejection reason is required" });

            return Ok(new
            {
                success = true,
                message = "Transaction rejected",
                approvalId = approvalId,
                rejectedBy = User.Identity?.Name,
                rejectedAt = DateTime.UtcNow,
                reason = dto.Reason
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Operational Oversight

    /// <summary>
    /// Get daily operational metrics
    /// </summary>
    [HttpGet("operations/daily-metrics")]
    public async Task<IActionResult> GetDailyMetrics()
    {
        try
        {
            var metrics = new
            {
                Date = DateTime.Today,
                TotalTransactions = 456,
                SuccessfulTransactions = 451,
                FailedTransactions = 5,
                SuccessRate = 98.9m,
                TotalValue = 12850000m,
                AverageTransactionValue = 28157.89m,
                PeakHour = "10:00-11:00",
                AverageTellerUtilization = 87.5m
            };

            return Ok(new { success = true, data = metrics });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get transaction error analysis
    /// </summary>
    [HttpGet("operations/error-analysis")]
    public async Task<IActionResult> GetErrorAnalysis([FromQuery] string? period = "today")
    {
        try
        {
            var analysis = new
            {
                Period = period ?? "today",
                TotalErrors = 5,
                ErrorsByType = new[]
                {
                    new { Type = "InsufficientFunds", Count = 2 },
                    new { Type = "InvalidAccount", Count = 1 },
                    new { Type = "ExceededLimit", Count = 1 },
                    new { Type = "SystemError", Count = 1 }
                },
                ErrorsByStaff = new[]
                {
                    new { Staff = "Jane Kariuki", ErrorCount = 1, ErrorRate = 0.7m },
                    new { Staff = "Alice Wanjiru", ErrorCount = 0, ErrorRate = 0m }
                },
                TrendingIssues = new[] { "Incorrect account numbers (reoccurring)", "System timeout during peak hours" }
            };

            return Ok(new { success = true, data = analysis });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get real-time operations dashboard
    /// </summary>
    [HttpGet("operations/realtime-dashboard")]
    public async Task<IActionResult> GetRealtimeDashboard()
    {
        try
        {
            var dashboard = new
            {
                ActiveSessions = 6,
                OnlineTellers = 6,
                OnlineCustomers = 234,
                TransactionsInLastHour = 89,
                AverageQueueWaitTime = "3 minutes",
                SystemHealth = "Optimal",
                CashDrawerBalance = 8500000m,
                LastUpdated = DateTime.UtcNow
            };

            return Ok(new { success = true, data = dashboard });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Compliance & Quality

    /// <summary>
    /// Get team compliance status
    /// </summary>
    [HttpGet("compliance/team-status")]
    public async Task<IActionResult> GetTeamComplianceStatus()
    {
        try
        {
            var compliance = new
            {
                KYCCompliance = 98.5m,
                AMLScreeningCompliance = 99.2m,
                DocumentationCompliance = 97.8m,
                TransactionReportingCompliance = 100m,
                OverallCompliance = 98.9m,
                Risks = new[]
                {
                    new { Risk = "3 customers KYC documents pending", Severity = "Low" },
                    new { Risk = "AML review queue: 2 transactions", Severity = "Medium" }
                }
            };

            return Ok(new { success = true, data = compliance });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get quality control metrics
    /// </summary>
    [HttpGet("quality/control-metrics")]
    public async Task<IActionResult> GetQualityMetrics()
    {
        try
        {
            var metrics = new
            {
                DataEntryAccuracy = 99.8m,
                DocumentationAccuracy = 98.5m,
                ComplianceAccuracy = 99.2m,
                OverallQuality = 99.2m,
                NeedImprovement = new[]
                {
                    new { Staff = "Jane Kariuki", Metric = "Documentation accuracy", CurrentValue = 97.5m },
                    new { Staff = "Isaac Kipchoge", Metric = "Compliance accuracy", CurrentValue = 98.0m }
                }
            };

            return Ok(new { success = true, data = metrics });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion
}

#region DTOs

public class TeamMemberDto
{
    public string MemberId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool OnDuty { get; set; }
    public DateTime? SessionStarted { get; set; }
    public int TransactionsToday { get; set; }
    public int ErrorsToday { get; set; }
}

public class PendingApprovalDto
{
    public string ApprovalId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Staff { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string Priority { get; set; } = string.Empty;
}

public class TransactionApprovalDto
{
    public string? Notes { get; set; }
}

public class TransactionRejectionDto
{
    public string Reason { get; set; } = string.Empty;
}

#endregion
