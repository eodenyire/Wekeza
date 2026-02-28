using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Branch Manager Portal Controller
/// Handles branch-level operations, reporting, and staff management
/// </summary>
[ApiController]
[Route("api/branch-manager")]
[Authorize(Roles = "BranchManager")]
public class BranchManagerPortalController : BaseApiController
{
    public BranchManagerPortalController(IMediator mediator) : base(mediator) { }

    #region Dashboard & Reporting

    /// <summary>
    /// Get branch dashboard summary
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(BranchDashboardDto), 200)]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var dashboard = new BranchDashboardDto
            {
                TotalCustomers = 1250,
                TotalAccounts = 2100,
                DailyTransactions = 450,
                DailyTransactionValue = 12500000m,
                CashOnHand = 8500000m,
                PendingApprovals = 15,
                ActiveTellers = 8,
                BranchHealth = "Excellent"
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get branch staff list with performance metrics
    /// </summary>
    [HttpGet("staff")]
    public async Task<IActionResult> GetBranchStaff(
        [FromQuery] string? role = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var staff = new List<StaffPerformanceDto>
            {
                new StaffPerformanceDto
                {
                    StaffId = "EMP001",
                    Name = "Jane Kariuki",
                    Role = "Teller",
                    Status = "Active",
                    TransactionsProcessed = 245,
                    ErrorRate = 0.2m,
                    AverageTransactionTime = 2.5m,
                    CustomerSatisfactionRating = 4.8m
                },
                new StaffPerformanceDto
                {
                    StaffId = "EMP002",
                    Name = "John Musyoka",
                    Role = "Supervisor",
                    Status = "Active",
                    TransactionsProcessed = 89,
                    ErrorRate = 0.1m,
                    AverageTransactionTime = 3.2m,
                    CustomerSatisfactionRating = 4.9m
                }
            };

            if (!string.IsNullOrEmpty(role))
                staff = staff.Where(s => s.Role == role).ToList();

            if (!string.IsNullOrEmpty(status))
                staff = staff.Where(s => s.Status == status).ToList();

            return Ok(new { success = true, data = staff, count = staff.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get daily transaction summary
    /// </summary>
    [HttpGet("transactions/daily")]
    public async Task<IActionResult> GetDailyTransactionSummary([FromQuery] DateTime? date = null)
    {
        try
        {
            var targetDate = date ?? DateTime.Today;
            var summary = new
            {
                Date = targetDate,
                TotalTransactions = 450,
                TotalValue = 12500000m,
                Deposits = new { Count = 250, Value = 8500000m },
                Withdrawals = new { Count = 120, Value = 3200000m },
                Transfers = new { Count = 80, Value = 800000m },
                FailedTransactions = 5,
                AverageTransactionValue = 27777.78m
            };

            return Ok(new { success = true, data = summary });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Staff Management

    /// <summary>
    /// Get pending staff requests (approvals, assignments)
    /// </summary>
    [HttpGet("pending-requests")]
    public async Task<IActionResult> GetPendingRequests()
    {
        try
        {
            var requests = new List<object>
            {
                new
                {
                    RequestId = "REQ001",
                    Type = "OverLimitTransaction",
                    Staff = "Jane Kariuki",
                    Amount = 500000m,
                    Reason = "Customer account upgrade",
                    SubmittedAt = DateTime.UtcNow.AddHours(-2),
                    Status = "Pending"
                },
                new
                {
                    RequestId = "REQ002",
                    Type = "CashWithdrawal",
                    Staff = "John Musyoka",
                    Amount = 1000000m,
                    Reason = "Customer withdrawal",
                    SubmittedAt = DateTime.UtcNow.AddHours(-4),
                    Status = "Pending"
                }
            };

            return Ok(new { success = true, data = requests, count = requests.Count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Approve or reject a pending request
    /// </summary>
    [HttpPost("pending-requests/{requestId}/approve")]
    public async Task<IActionResult> ApproveRequest(string requestId, [FromBody] ApprovalRequestDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(requestId))
                return BadRequest(new { error = "Request ID is required" });

            return Ok(new
            {
                success = true,
                message = "Request approved successfully",
                requestId = requestId,
                approvedBy = User.Identity?.Name,
                approvedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Reject a pending request
    /// </summary>
    [HttpPost("pending-requests/{requestId}/reject")]
    public async Task<IActionResult> RejectRequest(string requestId, [FromBody] RejectionRequestDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(requestId))
                return BadRequest(new { error = "Request ID is required" });

            return Ok(new
            {
                success = true,
                message = "Request rejected",
                requestId = requestId,
                rejectedBy = User.Identity?.Name,
                reason = dto.Reason,
                rejectedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Compliance & Audit

    /// <summary>
    /// Get branch compliance status
    /// </summary>
    [HttpGet("compliance")]
    public async Task<IActionResult> GetComplianceStatus()
    {
        try
        {
            var compliance = new
            {
                AMLScreeningStatus = "Compliant",
                KYCCoverage = 98.5m,
                TransactionReporting = "Up to date",
                LastAuditDate = DateTime.UtcNow.AddMonths(-1),
                PendingIssues = 2,
                IssueDetails = new[]
                {
                    new { Code = "AML001", Description = "3 customers pending AML review", Severity = "Low" },
                    new { Code = "KYC002", Description = "5 KYC documents need verification", Severity = "Medium" }
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
    /// Get audit trail for branch activities
    /// </summary>
    [HttpGet("audit-trail")]
    public async Task<IActionResult> GetAuditTrail(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? action = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var trail = new List<AuditEntryDto>
            {
                new AuditEntryDto
                {
                    AuditId = "AUD001",
                    Action = "CashDeposit",
                    Staff = "Jane Kariuki",
                    Actor = "John Musyoka (Supervisor)",
                    Amount = 250000m,
                    Timestamp = DateTime.UtcNow.AddHours(-1),
                    IpAddress = "192.168.1.100"
                },
                new AuditEntryDto
                {
                    AuditId = "AUD002",
                    Action = "ApprovedTransaction",
                    Staff = "Branch Manager",
                    Actor = "Peter Ochieng",
                    Amount = 500000m,
                    Timestamp = DateTime.UtcNow.AddHours(-2),
                    IpAddress = "192.168.1.50"
                }
            };

            return Ok(new
            {
                success = true,
                data = trail.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                pagination = new { page, pageSize, total = trail.Count }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Cash Management

    /// <summary>
    /// Get branch cash position
    /// </summary>
    [HttpGet("cash-position")]
    public async Task<IActionResult> GetCashPosition()
    {
        try
        {
            var position = new
            {
                CashOnHand = 8500000m,
                SafeVaultCash = 15000000m,
                TotalCashAvailable = 23500000m,
                CashInTransit = 2000000m,
                CashExpectedIncoming = 5000000m,
                CashAtCentralBank = 20000000m,
                LastUpdated = DateTime.UtcNow
            };

            return Ok(new { success = true, data = position });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Request cash replenishment
    /// </summary>
    [HttpPost("cash-replenishment/request")]
    public async Task<IActionResult> RequestCashReplenishment([FromBody] CashReplenishmentRequestDto dto)
    {
        try
        {
            if (dto.Amount <= 0)
                return BadRequest(new { error = "Amount must be greater than 0" });

            return Ok(new
            {
                success = true,
                message = "Cash replenishment request submitted",
                requestId = Guid.NewGuid().ToString(),
                amount = dto.Amount,
                currency = "KES",
                requestedAt = DateTime.UtcNow,
                expectedDelivery = DateTime.UtcNow.AddDays(1)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion
}

#region DTOs

public class BranchDashboardDto
{
    public int TotalCustomers { get; set; }
    public int TotalAccounts { get; set; }
    public int DailyTransactions { get; set; }
    public decimal DailyTransactionValue { get; set; }
    public decimal CashOnHand { get; set; }
    public int PendingApprovals { get; set; }
    public int ActiveTellers { get; set; }
    public string BranchHealth { get; set; } = string.Empty;
}

public class StaffPerformanceDto
{
    public string StaffId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TransactionsProcessed { get; set; }
    public decimal ErrorRate { get; set; }
    public decimal AverageTransactionTime { get; set; }
    public decimal CustomerSatisfactionRating { get; set; }
}

public class ApprovalRequestDto
{
    public string Reason { get; set; } = string.Empty;
}

public class RejectionRequestDto
{
    public string Reason { get; set; } = string.Empty;
}

public class AuditEntryDto
{
    public string AuditId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Staff { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}

public class CashReplenishmentRequestDto
{
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
}

#endregion
