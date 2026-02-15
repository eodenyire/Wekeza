using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Budget Control & Commitments Controller
/// Manages budget allocations, commitments, and utilization tracking
/// </summary>
[ApiController]
[Route("api/public-sector/budget")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<BudgetController> _logger;

    public BudgetController(IDbConnection dbConnection, ILogger<BudgetController> logger)
    {
        _dbConnection = dbConnection;
        _logger = logger;
    }

    /// <summary>
    /// Get budget allocations for a fiscal year
    /// </summary>
    [HttpGet("allocations")]
    public async Task<IActionResult> GetBudgetAllocations([FromQuery] int fiscalYear = 2026)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    ""Id"" as id,
                    ""DepartmentName"" as departmentname,
                    ""Category"" as category,
                    ""FiscalYear"" as fiscalyear,
                    ""AllocatedAmount"" as allocatedamount,
                    ""SpentAmount"" as spentamount,
                    ""CommittedAmount"" as committedamount,
                    ""AvailableAmount"" as availableamount,
                    ""Status"" as status,
                    ""CreatedAt"" as createdat
                FROM ""BudgetAllocations""
                WHERE ""FiscalYear"" = @FiscalYear
                ORDER BY ""DepartmentName"", ""Category""";

            var allocations = await _dbConnection.QueryAsync<BudgetAllocationDto>(query, new { FiscalYear = fiscalYear });

            // Calculate utilization percentages
            var result = allocations.Select(a => new
            {
                a.Id,
                a.DepartmentName,
                a.Category,
                a.FiscalYear,
                a.AllocatedAmount,
                a.SpentAmount,
                a.CommittedAmount,
                a.AvailableAmount,
                a.Status,
                UtilizationPercentage = a.AllocatedAmount > 0 ? (a.SpentAmount / a.AllocatedAmount) * 100 : 0,
                CommitmentPercentage = a.AllocatedAmount > 0 ? (a.CommittedAmount / a.AllocatedAmount) * 100 : 0,
                Alert = GetBudgetAlert(a.AvailableAmount, a.AllocatedAmount)
            });

            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget allocations");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Create budget commitment
    /// </summary>
    [HttpPost("commitments")]
    public async Task<IActionResult> CreateCommitment([FromBody] CreateCommitmentRequest request)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Check budget availability
            var allocationQuery = @"
                SELECT 
                    ""AvailableAmount"" as availableamount,
                    ""Status"" as status
                FROM ""BudgetAllocations""
                WHERE ""Id"" = @AllocationId";

            var allocation = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(
                allocationQuery, 
                new { AllocationId = request.AllocationId });

            if (allocation == null)
            {
                return NotFound(new { success = false, message = "Budget allocation not found" });
            }

            if (allocation.status != "Active")
            {
                return BadRequest(new { success = false, message = "Budget allocation is not active" });
            }

            if (allocation.availableamount < request.Amount)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Insufficient budget. Available: KES {allocation.availableamount:N2}, Requested: KES {request.Amount:N2}" 
                });
            }

            // Create commitment
            var commitmentId = Guid.NewGuid();
            var commitmentNumber = $"CMT-{DateTime.Now:yyyyMMdd}-{commitmentId.ToString().Substring(0, 8).ToUpper()}";

            var insertQuery = @"
                INSERT INTO ""BudgetCommitments""
                (""Id"", ""CommitmentNumber"", ""AllocationId"", ""Amount"", ""Purpose"", 
                 ""Reference"", ""CreatedBy"", ""Status"")
                VALUES
                (@Id, @CommitmentNumber, @AllocationId, @Amount, @Purpose, 
                 @Reference, @CreatedBy, @Status)";

            await _dbConnection.ExecuteAsync(insertQuery, new
            {
                Id = commitmentId,
                CommitmentNumber = commitmentNumber,
                request.AllocationId,
                request.Amount,
                request.Purpose,
                request.Reference,
                CreatedBy = userId,
                Status = "ACTIVE"
            });

            // Update budget allocation
            var updateQuery = @"
                UPDATE ""BudgetAllocations""
                SET ""CommittedAmount"" = ""CommittedAmount"" + @Amount,
                    ""AvailableAmount"" = ""AvailableAmount"" - @Amount,
                    ""UpdatedAt"" = CURRENT_TIMESTAMP
                WHERE ""Id"" = @AllocationId";

            await _dbConnection.ExecuteAsync(updateQuery, new
            {
                request.Amount,
                request.AllocationId
            });

            // Log audit
            await LogAudit(userId, "BUDGET_COMMITMENT_CREATED", "BudgetCommitment", commitmentId, 
                null, $"{commitmentNumber}: KES {request.Amount:N2}");

            return Ok(new
            {
                success = true,
                message = "Budget commitment created successfully",
                data = new
                {
                    commitmentId,
                    commitmentNumber,
                    amount = request.Amount,
                    status = "ACTIVE"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating commitment");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get budget utilization report
    /// </summary>
    [HttpGet("utilization")]
    public async Task<IActionResult> GetBudgetUtilization([FromQuery] Guid? departmentId = null, [FromQuery] int fiscalYear = 2026)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    ""DepartmentName"" as departmentname,
                    ""Category"" as category,
                    SUM(""AllocatedAmount"") as totalallocated,
                    SUM(""SpentAmount"") as totalspent,
                    SUM(""CommittedAmount"") as totalcommitted,
                    SUM(""AvailableAmount"") as totalavailable
                FROM ""BudgetAllocations""
                WHERE ""FiscalYear"" = @FiscalYear
                GROUP BY ""DepartmentName"", ""Category""
                ORDER BY ""DepartmentName"", ""Category""";

            var utilization = await _dbConnection.QueryAsync<dynamic>(query, new { FiscalYear = fiscalYear });

            var result = utilization.Select(u => new
            {
                u.departmentname,
                u.category,
                u.totalallocated,
                u.totalspent,
                u.totalcommitted,
                u.totalavailable,
                utilizationRate = u.totalallocated > 0 ? (u.totalspent / u.totalallocated) * 100 : 0,
                commitmentRate = u.totalallocated > 0 ? (u.totalcommitted / u.totalallocated) * 100 : 0,
                availabilityRate = u.totalallocated > 0 ? (u.totalavailable / u.totalallocated) * 100 : 0
            });

            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget utilization");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Check budget availability
    /// </summary>
    [HttpPost("check-availability")]
    public async Task<IActionResult> CheckBudgetAvailability([FromBody] CheckAvailabilityRequest request)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    ""AvailableAmount"" as availableamount,
                    ""AllocatedAmount"" as allocatedamount,
                    ""SpentAmount"" as spentamount,
                    ""CommittedAmount"" as committedamount,
                    ""Status"" as status
                FROM ""BudgetAllocations""
                WHERE ""Id"" = @AllocationId";

            var allocation = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(
                query, 
                new { AllocationId = request.AllocationId });

            if (allocation == null)
            {
                return NotFound(new { success = false, message = "Budget allocation not found" });
            }

            var isAvailable = allocation.availableamount >= request.Amount && allocation.status == "Active";

            return Ok(new
            {
                success = true,
                data = new
                {
                    isAvailable,
                    requestedAmount = request.Amount,
                    availableAmount = allocation.availableamount,
                    allocatedAmount = allocation.allocatedamount,
                    spentAmount = allocation.spentamount,
                    committedAmount = allocation.committedamount,
                    status = allocation.status,
                    message = isAvailable 
                        ? "Budget is available" 
                        : $"Insufficient budget. Available: KES {allocation.availableamount:N2}, Requested: KES {request.Amount:N2}"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking budget availability");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get budget commitments
    /// </summary>
    [HttpGet("commitments")]
    public async Task<IActionResult> GetCommitments([FromQuery] Guid? allocationId = null, [FromQuery] string? status = null)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    c.""Id"" as id,
                    c.""CommitmentNumber"" as commitmentnumber,
                    c.""Amount"" as amount,
                    c.""Purpose"" as purpose,
                    c.""Reference"" as reference,
                    c.""Status"" as status,
                    c.""CreatedAt"" as createdat,
                    c.""ReleasedAt"" as releasedat,
                    u.""Username"" as createdby,
                    ba.""DepartmentName"" as departmentname,
                    ba.""Category"" as category
                FROM ""BudgetCommitments"" c
                JOIN ""Users"" u ON c.""CreatedBy"" = u.""Id""
                JOIN ""BudgetAllocations"" ba ON c.""AllocationId"" = ba.""Id""
                WHERE (@AllocationId IS NULL OR c.""AllocationId"" = @AllocationId)
                AND (@Status IS NULL OR c.""Status"" = @Status)
                ORDER BY c.""CreatedAt"" DESC";

            var commitments = await _dbConnection.QueryAsync<dynamic>(query, new
            {
                AllocationId = allocationId,
                Status = status
            });

            return Ok(new { success = true, data = commitments });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting commitments");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Release budget commitment
    /// </summary>
    [HttpPost("commitments/{commitmentId}/release")]
    public async Task<IActionResult> ReleaseCommitment(Guid commitmentId)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Get commitment
            var commitmentQuery = @"
                SELECT 
                    ""AllocationId"" as allocationid,
                    ""Amount"" as amount,
                    ""Status"" as status
                FROM ""BudgetCommitments""
                WHERE ""Id"" = @CommitmentId";

            var commitment = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(
                commitmentQuery, 
                new { CommitmentId = commitmentId });

            if (commitment == null)
            {
                return NotFound(new { success = false, message = "Commitment not found" });
            }

            if (commitment.status != "ACTIVE")
            {
                return BadRequest(new { success = false, message = $"Commitment is not active. Current status: {commitment.status}" });
            }

            // Release commitment
            var updateCommitmentQuery = @"
                UPDATE ""BudgetCommitments""
                SET ""Status"" = 'RELEASED', ""ReleasedAt"" = CURRENT_TIMESTAMP
                WHERE ""Id"" = @CommitmentId";

            await _dbConnection.ExecuteAsync(updateCommitmentQuery, new { CommitmentId = commitmentId });

            // Update budget allocation
            var updateAllocationQuery = @"
                UPDATE ""BudgetAllocations""
                SET ""CommittedAmount"" = ""CommittedAmount"" - @Amount,
                    ""AvailableAmount"" = ""AvailableAmount"" + @Amount,
                    ""UpdatedAt"" = CURRENT_TIMESTAMP
                WHERE ""Id"" = @AllocationId";

            await _dbConnection.ExecuteAsync(updateAllocationQuery, new
            {
                Amount = commitment.amount,
                AllocationId = commitment.allocationid
            });

            // Log audit
            await LogAudit(userId, "BUDGET_COMMITMENT_RELEASED", "BudgetCommitment", commitmentId, 
                "ACTIVE", "RELEASED");

            return Ok(new
            {
                success = true,
                message = "Budget commitment released successfully",
                data = new
                {
                    commitmentId,
                    amount = commitment.amount,
                    status = "RELEASED"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing commitment");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get budget alerts
    /// </summary>
    [HttpGet("alerts")]
    public async Task<IActionResult> GetBudgetAlerts([FromQuery] int fiscalYear = 2026)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    ""Id"" as id,
                    ""DepartmentName"" as departmentname,
                    ""Category"" as category,
                    ""AllocatedAmount"" as allocatedamount,
                    ""AvailableAmount"" as availableamount
                FROM ""BudgetAllocations""
                WHERE ""FiscalYear"" = @FiscalYear
                AND ""Status"" = 'Active'";

            var allocations = await _dbConnection.QueryAsync<dynamic>(query, new { FiscalYear = fiscalYear });

            var alerts = allocations
                .Select(a => new
                {
                    a.id,
                    a.departmentname,
                    a.category,
                    a.allocatedamount,
                    a.availableamount,
                    utilizationPercentage = a.allocatedamount > 0 ? ((a.allocatedamount - a.availableamount) / a.allocatedamount) * 100 : 0,
                    alertLevel = GetBudgetAlert(a.availableamount, a.allocatedamount)
                })
                .Where(a => a.alertLevel != "NORMAL")
                .OrderByDescending(a => a.utilizationPercentage);

            return Ok(new { success = true, data = alerts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget alerts");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    private string GetBudgetAlert(decimal available, decimal allocated)
    {
        if (allocated == 0) return "NORMAL";
        
        var percentage = (available / allocated) * 100;
        
        if (percentage <= 0) return "CRITICAL";
        if (percentage <= 10) return "HIGH";
        if (percentage <= 20) return "MEDIUM";
        return "NORMAL";
    }

    private async Task LogAudit(Guid userId, string action, string entityType, Guid entityId, string? oldValue, string? newValue)
    {
        var auditQuery = @"
            INSERT INTO ""AuditTrail""
            (""Id"", ""UserId"", ""Action"", ""EntityType"", ""EntityId"", ""OldValue"", ""NewValue"")
            VALUES
            (@Id, @UserId, @Action, @EntityType, @EntityId, @OldValue, @NewValue)";

        await _dbConnection.ExecuteAsync(auditQuery, new
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValue = oldValue,
            NewValue = newValue
        });
    }
}

// DTOs
public class BudgetAllocationDto
{
    public Guid Id { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int FiscalYear { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public decimal AvailableAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateCommitmentRequest
{
    public Guid AllocationId { get; set; }
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Reference { get; set; }
}

public class CheckAvailabilityRequest
{
    public Guid AllocationId { get; set; }
    public decimal Amount { get; set; }
}
