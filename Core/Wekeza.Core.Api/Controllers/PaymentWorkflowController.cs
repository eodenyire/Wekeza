using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Payment Workflow Controller - Maker-Checker-Approver Pattern
/// </summary>
[ApiController]
[Route("api/public-sector/payments")]
[Authorize]
public class PaymentWorkflowController : ControllerBase
{
    private readonly IDbConnection _dbConnection;

    public PaymentWorkflowController(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    /// <summary>
    /// Initiate a new payment request (Maker role)
    /// </summary>
    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest request)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            // Get current user ID from token (simplified - in production, extract from JWT claims)
            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Admin user

            // Validate account balance
            var accountQuery = @"
                SELECT ""BalanceAmount"" 
                FROM ""Accounts"" 
                WHERE ""Id"" = @AccountId AND ""Status"" = 'Active'";
            
            var balance = await _dbConnection.QueryFirstOrDefaultAsync<decimal>(accountQuery, new { request.AccountId });
            
            if (balance < request.Amount)
            {
                return BadRequest(new { success = false, message = "Insufficient account balance" });
            }

            // Check budget availability if budget allocation is specified
            if (request.BudgetAllocationId.HasValue)
            {
                var budgetQuery = @"
                    SELECT ""AvailableAmount"" 
                    FROM ""BudgetAllocations"" 
                    WHERE ""Id"" = @BudgetAllocationId AND ""Status"" = 'Active'";
                
                var availableBudget = await _dbConnection.QueryFirstOrDefaultAsync<decimal>(
                    budgetQuery, 
                    new { BudgetAllocationId = request.BudgetAllocationId.Value });
                
                if (availableBudget < request.Amount)
                {
                    return BadRequest(new { success = false, message = "Insufficient budget allocation" });
                }
            }

            // Determine required approval levels based on amount
            var requiredLevels = request.Amount switch
            {
                <= 10000000 => 1,      // Up to 10M: 1 approval
                <= 100000000 => 2,     // Up to 100M: 2 approvals
                _ => 3                  // Above 100M: 3 approvals
            };

            // Create payment request
            var paymentId = Guid.NewGuid();
            var requestNumber = $"PAY-{DateTime.Now:yyyyMMdd}-{paymentId.ToString().Substring(0, 8).ToUpper()}";

            var insertQuery = @"
                INSERT INTO ""PaymentRequests"" 
                (""Id"", ""RequestNumber"", ""InitiatorId"", ""CustomerId"", ""AccountId"", ""PaymentType"", 
                 ""Amount"", ""Currency"", ""BeneficiaryName"", ""BeneficiaryAccount"", ""BeneficiaryBank"", 
                 ""Purpose"", ""Reference"", ""Status"", ""CurrentApprovalLevel"", ""RequiredApprovalLevels"")
                VALUES 
                (@Id, @RequestNumber, @InitiatorId, @CustomerId, @AccountId, @PaymentType, 
                 @Amount, @Currency, @BeneficiaryName, @BeneficiaryAccount, @BeneficiaryBank, 
                 @Purpose, @Reference, @Status, @CurrentApprovalLevel, @RequiredApprovalLevels)";

            await _dbConnection.ExecuteAsync(insertQuery, new
            {
                Id = paymentId,
                RequestNumber = requestNumber,
                InitiatorId = userId,
                request.CustomerId,
                request.AccountId,
                request.PaymentType,
                request.Amount,
                request.Currency,
                request.BeneficiaryName,
                request.BeneficiaryAccount,
                request.BeneficiaryBank,
                request.Purpose,
                request.Reference,
                Status = "Pending",
                CurrentApprovalLevel = 1,
                RequiredApprovalLevels = requiredLevels
            });

            // Log audit trail
            await LogAudit(userId, "PAYMENT_INITIATED", "PaymentRequest", paymentId, null, requestNumber);

            return Ok(new
            {
                success = true,
                message = "Payment request initiated successfully",
                data = new
                {
                    paymentId,
                    requestNumber,
                    requiredApprovals = requiredLevels,
                    status = "Pending"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error initiating payment: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get pending approvals for current user
    /// </summary>
    [HttpGet("pending-approval")]
    public async Task<IActionResult> GetPendingApprovals([FromQuery] int? level = null)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    pr.""Id"" as id,
                    pr.""RequestNumber"" as requestnumber,
                    pr.""PaymentType"" as paymenttype,
                    pr.""Amount"" as amount,
                    pr.""Currency"" as currency,
                    pr.""BeneficiaryName"" as beneficiaryname,
                    pr.""BeneficiaryAccount"" as beneficiaryaccount,
                    pr.""Purpose"" as purpose,
                    pr.""CurrentApprovalLevel"" as currentapprovallevel,
                    pr.""RequiredApprovalLevels"" as requiredapprovallevels,
                    pr.""CreatedAt"" as createdat,
                    u.""Username"" as initiator,
                    c.""Name"" as customername
                FROM ""PaymentRequests"" pr
                JOIN ""Users"" u ON pr.""InitiatorId"" = u.""Id""
                JOIN ""Customers"" c ON pr.""CustomerId"" = c.""Id""
                WHERE pr.""Status"" = 'Pending'
                AND (@Level IS NULL OR pr.""CurrentApprovalLevel"" = @Level)
                ORDER BY pr.""CreatedAt"" DESC";

            var pendingPayments = await _dbConnection.QueryAsync<PendingPaymentDto>(query, new { Level = level });

            return Ok(new { success = true, data = pendingPayments });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving pending approvals: {ex.Message}" });
        }
    }

    /// <summary>
    /// Approve a payment request
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApprovePayment(Guid id, [FromBody] ApprovalRequest request)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Get payment request details
            var paymentQuery = @"
                SELECT 
                    ""CurrentApprovalLevel"" as currentapprovallevel,
                    ""RequiredApprovalLevels"" as requiredapprovallevels,
                    ""Amount"" as amount,
                    ""Status"" as status
                FROM ""PaymentRequests""
                WHERE ""Id"" = @Id";

            var payment = await _dbConnection.QueryFirstOrDefaultAsync<PaymentApprovalStatusDto>(paymentQuery, new { Id = id });

            if (payment == null)
            {
                return NotFound(new { success = false, message = "Payment request not found" });
            }

            if (payment.Status != "Pending")
            {
                return BadRequest(new { success = false, message = $"Payment is already {payment.Status}" });
            }

            // Create approval record
            var approvalId = Guid.NewGuid();
            var approvalQuery = @"
                INSERT INTO ""PaymentApprovals""
                (""Id"", ""PaymentRequestId"", ""ApproverId"", ""ApprovalLevel"", ""Action"", ""Comments"")
                VALUES
                (@Id, @PaymentRequestId, @ApproverId, @ApprovalLevel, @Action, @Comments)";

            await _dbConnection.ExecuteAsync(approvalQuery, new
            {
                Id = approvalId,
                PaymentRequestId = id,
                ApproverId = userId,
                ApprovalLevel = payment.CurrentApprovalLevel,
                Action = "APPROVED",
                request.Comments
            });

            // Update payment request
            string updateQuery;
            string newStatus;

            if (payment.CurrentApprovalLevel >= payment.RequiredApprovalLevels)
            {
                // Final approval - execute payment
                newStatus = "Approved";
                updateQuery = @"
                    UPDATE ""PaymentRequests""
                    SET ""Status"" = @Status,
                        ""CurrentApprovalLevel"" = @CurrentApprovalLevel,
                        ""UpdatedAt"" = CURRENT_TIMESTAMP,
                        ""ExecutedAt"" = CURRENT_TIMESTAMP
                    WHERE ""Id"" = @Id";
            }
            else
            {
                // Move to next approval level
                newStatus = "Pending";
                updateQuery = @"
                    UPDATE ""PaymentRequests""
                    SET ""CurrentApprovalLevel"" = @CurrentApprovalLevel,
                        ""UpdatedAt"" = CURRENT_TIMESTAMP
                    WHERE ""Id"" = @Id";
            }

            await _dbConnection.ExecuteAsync(updateQuery, new
            {
                Id = id,
                Status = newStatus,
                CurrentApprovalLevel = payment.CurrentApprovalLevel + 1
            });

            // Log audit trail
            await LogAudit(userId, "PAYMENT_APPROVED", "PaymentRequest", id, 
                $"Level {payment.CurrentApprovalLevel}", 
                $"Approved at level {payment.CurrentApprovalLevel}");

            return Ok(new
            {
                success = true,
                message = payment.CurrentApprovalLevel >= payment.RequiredApprovalLevels
                    ? "Payment approved and executed"
                    : $"Payment approved at level {payment.CurrentApprovalLevel}. Pending level {payment.CurrentApprovalLevel + 1} approval.",
                data = new
                {
                    paymentId = id,
                    currentLevel = payment.CurrentApprovalLevel + 1,
                    requiredLevels = payment.RequiredApprovalLevels,
                    status = newStatus
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error approving payment: {ex.Message}" });
        }
    }

    /// <summary>
    /// Reject a payment request
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectPayment(Guid id, [FromBody] RejectionRequest request)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Get payment request
            var paymentQuery = @"
                SELECT ""Status"" as status, ""CurrentApprovalLevel"" as currentapprovallevel
                FROM ""PaymentRequests""
                WHERE ""Id"" = @Id";

            var payment = await _dbConnection.QueryFirstOrDefaultAsync<PaymentApprovalStatusDto>(paymentQuery, new { Id = id });

            if (payment == null)
            {
                return NotFound(new { success = false, message = "Payment request not found" });
            }

            if (payment.Status != "Pending")
            {
                return BadRequest(new { success = false, message = $"Payment is already {payment.Status}" });
            }

            // Create rejection record
            var approvalId = Guid.NewGuid();
            var approvalQuery = @"
                INSERT INTO ""PaymentApprovals""
                (""Id"", ""PaymentRequestId"", ""ApproverId"", ""ApprovalLevel"", ""Action"", ""Comments"")
                VALUES
                (@Id, @PaymentRequestId, @ApproverId, @ApprovalLevel, @Action, @Comments)";

            await _dbConnection.ExecuteAsync(approvalQuery, new
            {
                Id = approvalId,
                PaymentRequestId = id,
                ApproverId = userId,
                ApprovalLevel = payment.CurrentApprovalLevel,
                Action = "REJECTED",
                Comments = request.Reason
            });

            // Update payment request
            var updateQuery = @"
                UPDATE ""PaymentRequests""
                SET ""Status"" = 'Rejected',
                    ""RejectionReason"" = @RejectionReason,
                    ""UpdatedAt"" = CURRENT_TIMESTAMP
                WHERE ""Id"" = @Id";

            await _dbConnection.ExecuteAsync(updateQuery, new
            {
                Id = id,
                RejectionReason = request.Reason
            });

            // Log audit trail
            await LogAudit(userId, "PAYMENT_REJECTED", "PaymentRequest", id, 
                "Pending", 
                $"Rejected: {request.Reason}");

            return Ok(new
            {
                success = true,
                message = "Payment request rejected",
                data = new
                {
                    paymentId = id,
                    status = "Rejected",
                    reason = request.Reason
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error rejecting payment: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get approval history for a payment
    /// </summary>
    [HttpGet("{id}/approval-history")]
    public async Task<IActionResult> GetApprovalHistory(Guid id)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    pa.""Id"" as id,
                    pa.""ApprovalLevel"" as approvallevel,
                    pa.""Action"" as action,
                    pa.""Comments"" as comments,
                    pa.""ApprovedAt"" as approvedat,
                    u.""Username"" as approvername,
                    u.""Email"" as approveremail
                FROM ""PaymentApprovals"" pa
                JOIN ""Users"" u ON pa.""ApproverId"" = u.""Id""
                WHERE pa.""PaymentRequestId"" = @PaymentRequestId
                ORDER BY pa.""ApprovedAt"" ASC";

            var history = await _dbConnection.QueryAsync<ApprovalHistoryDto>(query, new { PaymentRequestId = id });

            return Ok(new { success = true, data = history });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving approval history: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get payment request details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentRequest(Guid id)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    pr.""Id"" as id,
                    pr.""RequestNumber"" as requestnumber,
                    pr.""PaymentType"" as paymenttype,
                    pr.""Amount"" as amount,
                    pr.""Currency"" as currency,
                    pr.""BeneficiaryName"" as beneficiaryname,
                    pr.""BeneficiaryAccount"" as beneficiaryaccount,
                    pr.""BeneficiaryBank"" as beneficiarybank,
                    pr.""Purpose"" as purpose,
                    pr.""Reference"" as reference,
                    pr.""Status"" as status,
                    pr.""CurrentApprovalLevel"" as currentapprovallevel,
                    pr.""RequiredApprovalLevels"" as requiredapprovallevels,
                    pr.""CreatedAt"" as createdat,
                    pr.""ExecutedAt"" as executedat,
                    pr.""RejectionReason"" as rejectionreason,
                    u.""Username"" as initiator,
                    c.""Name"" as customername
                FROM ""PaymentRequests"" pr
                JOIN ""Users"" u ON pr.""InitiatorId"" = u.""Id""
                JOIN ""Customers"" c ON pr.""CustomerId"" = c.""Id""
                WHERE pr.""Id"" = @Id";

            var payment = await _dbConnection.QueryFirstOrDefaultAsync<PaymentRequestDetailDto>(query, new { Id = id });

            if (payment == null)
            {
                return NotFound(new { success = false, message = "Payment request not found" });
            }

            return Ok(new { success = true, data = payment });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving payment request: {ex.Message}" });
        }
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
public class InitiatePaymentRequest
{
    public Guid CustomerId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? BudgetAllocationId { get; set; }
    public string PaymentType { get; set; } = "SINGLE";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "KES";
    public string BeneficiaryName { get; set; } = string.Empty;
    public string BeneficiaryAccount { get; set; } = string.Empty;
    public string? BeneficiaryBank { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Reference { get; set; }
}

public class ApprovalRequest
{
    public string? Comments { get; set; }
}

public class RejectionRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class PendingPaymentDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string BeneficiaryName { get; set; } = string.Empty;
    public string BeneficiaryAccount { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public int CurrentApprovalLevel { get; set; }
    public int RequiredApprovalLevels { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Initiator { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}

public class PaymentApprovalStatusDto
{
    public int CurrentApprovalLevel { get; set; }
    public int RequiredApprovalLevels { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ApprovalHistoryDto
{
    public Guid Id { get; set; }
    public int ApprovalLevel { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime ApprovedAt { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public string ApproverEmail { get; set; } = string.Empty;
}

public class PaymentRequestDetailDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string BeneficiaryName { get; set; } = string.Empty;
    public string BeneficiaryAccount { get; set; } = string.Empty;
    public string? BeneficiaryBank { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CurrentApprovalLevel { get; set; }
    public int RequiredApprovalLevels { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string Initiator { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}
