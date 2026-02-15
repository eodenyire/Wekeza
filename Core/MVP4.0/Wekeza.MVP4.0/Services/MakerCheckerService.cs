using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

public class MakerCheckerService : IMakerCheckerService
{
    private readonly MVP4DbContext _context;
    private readonly IRBACService _rbacService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<MakerCheckerService> _logger;

    public MakerCheckerService(
        MVP4DbContext context,
        IRBACService rbacService,
        INotificationService notificationService,
        ILogger<MakerCheckerService> logger)
    {
        _context = context;
        _rbacService = rbacService;
        _notificationService = notificationService;
        _logger = logger;
    }

    #region Transaction Helper Methods

    private async Task<T> ExecuteWithTransactionAsync<T>(Func<Task<T>> operation)
    {
        // Skip transactions for in-memory database (used in tests)
        var useTransaction = !_context.Database.IsInMemory();
        
        if (useTransaction)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await operation();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
            return await operation();
        }
    }

    private async Task ExecuteWithTransactionAsync(Func<Task> operation)
    {
        // Skip transactions for in-memory database (used in tests)
        var useTransaction = !_context.Database.IsInMemory();
        
        if (useTransaction)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
            await operation();
        }
    }

    #endregion

    #region Workflow Initiation

    public async Task<WorkflowInstance> InitiateMakerActionAsync(MakerAction action)
    {
        return await ExecuteWithTransactionAsync(async () =>
        {
            return await InitiateMakerActionInternalAsync(action);
        });
    }

    private async Task<WorkflowInstance> InitiateMakerActionInternalAsync(MakerAction action)
    {
        // Validate the maker action
        var validation = await ValidateMakerActionAsync(action);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Invalid maker action: {validation.Message}");
        }

        // Determine approval requirements
        var approvalRequirement = await DetermineApprovalRequirementsAsync(action);

        // Create workflow instance
        var workflow = new WorkflowInstance
        {
            WorkflowId = Guid.NewGuid(),
            WorkflowType = action.ActionType,
            ResourceId = action.ResourceId,
            ResourceType = action.ResourceType,
            Status = WorkflowStatus.Pending.ToString(),
            InitiatedBy = action.MakerId,
            InitiatedAt = DateTime.UtcNow,
            Data = action.Data,
            BusinessJustification = action.BusinessJustification,
            Priority = action.Priority,
            Amount = action.Amount,
            ApprovalDeadline = CalculateApprovalDeadline(action.Priority, action.RequestedCompletionDate)
        };

        _context.WorkflowInstances.Add(workflow);
        await _context.SaveChangesAsync();

        // Create approval steps
        await CreateApprovalStepsAsync(workflow.WorkflowId, approvalRequirement);

        // Send approval notifications
        foreach (var approverRole in approvalRequirement)
        {
            await _notificationService.SendApprovalNotificationAsync(
                workflow.WorkflowId, 
                approverRole, 
                $"New {action.ActionType} workflow requires your approval. Amount: {action.Amount:C}, Justification: {action.BusinessJustification}");
        }

        // Log the workflow initiation
        await _rbacService.LogUserActionAsync(
            action.MakerId,
            $"Initiated workflow: {action.ActionType}",
            "Workflow",
            workflow.WorkflowId,
            newValues: JsonSerializer.Serialize(action)
        );

        _logger.LogInformation("Workflow {WorkflowId} initiated by user {MakerId} for action {ActionType}",
            workflow.WorkflowId, action.MakerId, action.ActionType);

        return workflow;
    }

    public async Task<WorkflowInstance?> GetWorkflowInstanceAsync(Guid workflowId)
    {
        return await _context.WorkflowInstances
            .FirstOrDefaultAsync(w => w.WorkflowId == workflowId);
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstancesByUserAsync(Guid userId)
    {
        return await _context.WorkflowInstances
            .Where(w => w.InitiatedBy == userId)
            .OrderByDescending(w => w.InitiatedAt)
            .ToListAsync();
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstancesByStatusAsync(WorkflowStatus status)
    {
        return await _context.WorkflowInstances
            .Where(w => w.Status == status.ToString())
            .OrderByDescending(w => w.InitiatedAt)
            .ToListAsync();
    }

    #endregion

    #region Approval Processing

    public async Task<ApprovalResult> SubmitForApprovalAsync(Guid workflowId, Guid checkerId, string? comments = null)
    {
        return await ExecuteWithTransactionAsync(async () =>
        {
            return await SubmitForApprovalInternalAsync(workflowId, checkerId, comments);
        });
    }

    private async Task<ApprovalResult> SubmitForApprovalInternalAsync(Guid workflowId, Guid checkerId, string? comments = null)
    {
        var workflow = await GetWorkflowInstanceAsync(workflowId);
        if (workflow == null)
        {
            return new ApprovalResult
            {
                IsSuccess = false,
                Message = "Workflow not found",
                ErrorCode = "WORKFLOW_NOT_FOUND"
            };
        }

        // Validate user can approve this workflow
        var canApprove = await CanUserApproveWorkflowAsync(checkerId, workflowId);
        if (!canApprove)
        {
            return new ApprovalResult
            {
                IsSuccess = false,
                Message = "User is not authorized to approve this workflow",
                ErrorCode = "UNAUTHORIZED_APPROVAL"
            };
        }

        // Get current approval step
        var currentStep = await _context.ApprovalSteps
            .Where(s => s.WorkflowId == workflowId && s.Status == ApprovalStepStatus.Pending.ToString())
            .OrderBy(s => s.StepOrder)
            .FirstOrDefaultAsync();

        if (currentStep == null)
        {
            return new ApprovalResult
            {
                IsSuccess = false,
                Message = "No pending approval step found",
                ErrorCode = "NO_PENDING_STEP"
            };
        }

        // Update approval step
        currentStep.Status = ApprovalStepStatus.Approved.ToString();
        currentStep.ApprovedBy = checkerId;
        currentStep.ApprovedAt = DateTime.UtcNow;
        currentStep.Comments = comments;

        // Check if there are more steps
        var nextStep = await _context.ApprovalSteps
            .Where(s => s.WorkflowId == workflowId && s.StepOrder > currentStep.StepOrder)
            .OrderBy(s => s.StepOrder)
            .FirstOrDefaultAsync();

        if (nextStep == null)
        {
            // All steps completed, mark workflow as approved
            workflow.Status = WorkflowStatus.Approved.ToString();
            workflow.CompletedAt = DateTime.UtcNow;

            // Send completion notification to initiator
            await _notificationService.SendWorkflowCompletedNotificationAsync(
                workflowId, workflow.InitiatedBy, "Approved");
        }

        await _context.SaveChangesAsync();

        // Log the approval
        await _rbacService.LogUserActionAsync(
            checkerId,
            $"Approved workflow step: {workflow.WorkflowType}",
            "Workflow",
            workflowId,
            newValues: JsonSerializer.Serialize(new { StepId = currentStep.StepId, Comments = comments })
        );

        _logger.LogInformation("Workflow {WorkflowId} step approved by user {CheckerId}",
            workflowId, checkerId);

        return new ApprovalResult
        {
            IsSuccess = true,
            Message = nextStep == null ? "Workflow completed successfully" : "Approval step completed, awaiting next approval",
            WorkflowInstance = workflow
        };
    }

    public async Task<ApprovalResult> RejectWorkflowAsync(Guid workflowId, Guid checkerId, string reason)
    {
        return await ExecuteWithTransactionAsync(async () =>
        {
            return await RejectWorkflowInternalAsync(workflowId, checkerId, reason);
        });
    }

    private async Task<ApprovalResult> RejectWorkflowInternalAsync(Guid workflowId, Guid checkerId, string reason)
    {
        var workflow = await GetWorkflowInstanceAsync(workflowId);
        if (workflow == null)
        {
            return new ApprovalResult
            {
                IsSuccess = false,
                Message = "Workflow not found",
                ErrorCode = "WORKFLOW_NOT_FOUND"
            };
        }

        // Validate user can reject this workflow
        var canApprove = await CanUserApproveWorkflowAsync(checkerId, workflowId);
        if (!canApprove)
        {
            return new ApprovalResult
            {
                IsSuccess = false,
                Message = "User is not authorized to reject this workflow",
                ErrorCode = "UNAUTHORIZED_REJECTION"
            };
        }

        // Update workflow status
        workflow.Status = WorkflowStatus.Rejected.ToString();
        workflow.RejectionReason = reason;
        workflow.CompletedAt = DateTime.UtcNow;

        // Send rejection notification to initiator
        await _notificationService.SendWorkflowCompletedNotificationAsync(
            workflowId, workflow.InitiatedBy, "Rejected");

        // Update current approval step
        var currentStep = await _context.ApprovalSteps
            .Where(s => s.WorkflowId == workflowId && s.Status == ApprovalStepStatus.Pending.ToString())
            .OrderBy(s => s.StepOrder)
            .FirstOrDefaultAsync();

        if (currentStep != null)
        {
            currentStep.Status = ApprovalStepStatus.Rejected.ToString();
            currentStep.ApprovedBy = checkerId;
            currentStep.ApprovedAt = DateTime.UtcNow;
            currentStep.Comments = reason;
        }

        await _context.SaveChangesAsync();

        // Log the rejection
        await _rbacService.LogUserActionAsync(
            checkerId,
            $"Rejected workflow: {workflow.WorkflowType}",
            "Workflow",
            workflowId,
            newValues: JsonSerializer.Serialize(new { Reason = reason })
        );

        _logger.LogInformation("Workflow {WorkflowId} rejected by user {CheckerId} with reason: {Reason}",
            workflowId, checkerId, reason);

        return new ApprovalResult
        {
            IsSuccess = true,
            Message = "Workflow rejected successfully",
            WorkflowInstance = workflow
        };
    }

    public async Task<List<PendingApproval>> GetApprovalQueueAsync(Guid checkerId)
    {
        var userRoles = await _rbacService.GetUserRolesAsync(checkerId);
        var roleNames = userRoles.Select(r => r.RoleName).ToList();

        return await GetApprovalQueueByRolesAsync(roleNames);
    }

    public async Task<List<PendingApproval>> GetApprovalQueueByRoleAsync(string roleName)
    {
        return await GetApprovalQueueByRolesAsync(new List<string> { roleName });
    }

    private async Task<List<PendingApproval>> GetApprovalQueueByRolesAsync(List<string> roleNames)
    {
        var pendingApprovals = await (from step in _context.ApprovalSteps
                                     join workflow in _context.WorkflowInstances on step.WorkflowId equals workflow.WorkflowId
                                     join user in _context.Users on workflow.InitiatedBy equals user.Id
                                     where step.Status == ApprovalStepStatus.Pending.ToString()
                                           && roleNames.Contains(step.ApproverRole)
                                           && workflow.Status == WorkflowStatus.Pending.ToString()
                                     select new PendingApproval
                                     {
                                         WorkflowId = workflow.WorkflowId,
                                         ActionType = workflow.WorkflowType,
                                         ResourceType = workflow.ResourceType,
                                         ResourceId = workflow.ResourceId,
                                         BusinessJustification = workflow.BusinessJustification,
                                         Amount = workflow.Amount,
                                         Priority = workflow.Priority,
                                         InitiatedAt = workflow.InitiatedAt,
                                         ApprovalDeadline = workflow.ApprovalDeadline,
                                         InitiatedByName = user.FullName,
                                         CurrentApproverRole = step.ApproverRole
                                     }).ToListAsync();

        // Calculate days remaining and overdue status
        var now = DateTime.UtcNow;
        foreach (var approval in pendingApprovals)
        {
            if (approval.ApprovalDeadline.HasValue)
            {
                var timeRemaining = approval.ApprovalDeadline.Value - now;
                approval.DaysRemaining = Math.Max(0, (int)timeRemaining.TotalDays);
                approval.IsOverdue = timeRemaining.TotalDays < 0;
            }
        }

        return pendingApprovals.OrderBy(p => p.ApprovalDeadline).ToList();
    }

    #endregion

    #region Escalation Management

    public async Task<EscalationResult> EscalateApprovalAsync(Guid workflowId, string reason)
    {
        return await ExecuteWithTransactionAsync(async () =>
        {
            return await EscalateApprovalInternalAsync(workflowId, reason);
        });
    }

    private async Task<EscalationResult> EscalateApprovalInternalAsync(Guid workflowId, string reason)
    {
        var workflow = await GetWorkflowInstanceAsync(workflowId);
        if (workflow == null)
        {
            return new EscalationResult
            {
                IsSuccess = false,
                Message = "Workflow not found"
            };
        }

        // Determine escalation target
        var escalationTarget = DetermineEscalationTarget(workflow.WorkflowType, workflow.Amount);

        // Update workflow status
        workflow.Status = WorkflowStatus.Escalated.ToString();
        workflow.EscalatedAt = DateTime.UtcNow;
        workflow.EscalationReason = reason;

        // Create new approval step for escalated approval
        var newStep = new ApprovalStep
        {
            StepId = Guid.NewGuid(),
            WorkflowId = workflowId,
            StepOrder = await GetNextStepOrderAsync(workflowId),
            ApproverRole = escalationTarget,
            Status = ApprovalStepStatus.Pending.ToString(),
            CreatedAt = DateTime.UtcNow,
            IsEscalated = true
        };

        _context.ApprovalSteps.Add(newStep);
        await _context.SaveChangesAsync();

        // Send escalation notification
        await _notificationService.SendEscalationNotificationAsync(
            workflowId, escalationTarget, reason);

        _logger.LogInformation("Workflow {WorkflowId} escalated to {EscalationTarget} with reason: {Reason}",
            workflowId, escalationTarget, reason);

        return new EscalationResult
        {
            IsSuccess = true,
            Message = $"Workflow escalated to {escalationTarget}",
            EscalatedWorkflowIds = new List<Guid> { workflowId },
            NewApproverRole = escalationTarget
        };
    }

    public async Task<EscalationResult> AutoEscalateExpiredWorkflowsAsync()
    {
        var expiredWorkflows = await GetExpiredWorkflowsAsync();
        var escalatedWorkflowIds = new List<Guid>();

        foreach (var workflow in expiredWorkflows)
        {
            var result = await EscalateApprovalAsync(workflow.WorkflowId, "Automatic escalation due to approval deadline exceeded");
            if (result.IsSuccess)
            {
                escalatedWorkflowIds.AddRange(result.EscalatedWorkflowIds);
            }
        }

        return new EscalationResult
        {
            IsSuccess = true,
            Message = $"Auto-escalated {escalatedWorkflowIds.Count} expired workflows",
            EscalatedWorkflowIds = escalatedWorkflowIds
        };
    }

    public async Task<List<WorkflowInstance>> GetExpiredWorkflowsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.WorkflowInstances
            .Where(w => w.Status == WorkflowStatus.Pending.ToString()
                       && w.ApprovalDeadline.HasValue
                       && w.ApprovalDeadline.Value < now)
            .ToListAsync();
    }

    #endregion

    #region Workflow Management

    public async Task<bool> CancelWorkflowAsync(Guid workflowId, Guid cancelledBy, string reason)
    {
        try
        {
            var workflow = await GetWorkflowInstanceAsync(workflowId);
            if (workflow == null) return false;

            workflow.Status = WorkflowStatus.Cancelled.ToString();
            workflow.CancellationReason = reason;
            workflow.CancelledBy = cancelledBy;
            workflow.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Workflow {WorkflowId} cancelled by user {CancelledBy} with reason: {Reason}",
                workflowId, cancelledBy, reason);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling workflow {WorkflowId}", workflowId);
            return false;
        }
    }

    public async Task<bool> CompleteWorkflowAsync(Guid workflowId)
    {
        try
        {
            var workflow = await GetWorkflowInstanceAsync(workflowId);
            if (workflow == null) return false;

            workflow.Status = WorkflowStatus.Completed.ToString();
            workflow.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Workflow {WorkflowId} completed", workflowId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing workflow {WorkflowId}", workflowId);
            return false;
        }
    }

    public async Task<List<ApprovalStep>> GetWorkflowStepsAsync(Guid workflowId)
    {
        return await _context.ApprovalSteps
            .Where(s => s.WorkflowId == workflowId)
            .OrderBy(s => s.StepOrder)
            .ToListAsync();
    }

    #endregion

    #region Validation

    public async Task<ValidationResult> ValidateMakerCheckerRulesAsync(Guid makerId, Guid checkerId, string actionType)
    {
        var result = new ValidationResult { IsValid = true };

        // Rule 1: Maker and checker cannot be the same user
        if (makerId == checkerId)
        {
            result.IsValid = false;
            result.Message = "Maker and checker cannot be the same user";
            result.ValidationErrors.Add("SAME_USER_VIOLATION");
            return result;
        }

        // Rule 2: Checker must have appropriate role for the action type
        var checkerRoles = await _rbacService.GetUserRolesAsync(checkerId);
        var requiredRoles = GetRequiredApproverRoles(actionType);

        if (!checkerRoles.Any(r => requiredRoles.Contains(r.RoleName)))
        {
            result.IsValid = false;
            result.Message = $"Checker does not have required role for action type: {actionType}";
            result.ValidationErrors.Add("INSUFFICIENT_ROLE");
            return result;
        }

        result.Message = "Maker-checker rules validation passed";
        return result;
    }

    public async Task<bool> CanUserApproveWorkflowAsync(Guid userId, Guid workflowId)
    {
        // Get user roles
        var userRoles = await _rbacService.GetUserRolesAsync(userId);
        var userRoleNames = userRoles.Select(r => r.RoleName).ToList();

        // Get current pending approval step
        var currentStep = await _context.ApprovalSteps
            .Where(s => s.WorkflowId == workflowId && s.Status == ApprovalStepStatus.Pending.ToString())
            .OrderBy(s => s.StepOrder)
            .FirstOrDefaultAsync();

        if (currentStep == null) return false;

        // Check if user has the required role for this step
        return userRoleNames.Contains(currentStep.ApproverRole);
    }

    #endregion

    #region Reporting and Analytics

    public async Task<WorkflowMetrics> GetWorkflowMetricsAsync(DateTime fromDate, DateTime toDate)
    {
        var workflows = await _context.WorkflowInstances
            .Where(w => w.InitiatedAt >= fromDate && w.InitiatedAt <= toDate)
            .ToListAsync();

        var metrics = new WorkflowMetrics
        {
            TotalWorkflows = workflows.Count,
            PendingWorkflows = workflows.Count(w => w.Status == WorkflowStatus.Pending.ToString()),
            ApprovedWorkflows = workflows.Count(w => w.Status == WorkflowStatus.Approved.ToString()),
            RejectedWorkflows = workflows.Count(w => w.Status == WorkflowStatus.Rejected.ToString()),
            EscalatedWorkflows = workflows.Count(w => w.Status == WorkflowStatus.Escalated.ToString()),
            OverdueWorkflows = workflows.Count(w => w.ApprovalDeadline.HasValue && w.ApprovalDeadline.Value < DateTime.UtcNow && w.Status == WorkflowStatus.Pending.ToString())
        };

        // Calculate average processing time for completed workflows
        var completedWorkflows = workflows.Where(w => w.CompletedAt.HasValue).ToList();
        if (completedWorkflows.Any())
        {
            var totalHours = completedWorkflows.Sum(w => (w.CompletedAt!.Value - w.InitiatedAt).TotalHours);
            metrics.AverageProcessingTimeHours = totalHours / completedWorkflows.Count;
        }

        // Group by type and priority
        metrics.WorkflowsByType = workflows.GroupBy(w => w.WorkflowType)
            .ToDictionary(g => g.Key, g => g.Count());

        metrics.WorkflowsByPriority = workflows.GroupBy(w => w.Priority)
            .ToDictionary(g => g.Key, g => g.Count());

        return metrics;
    }

    public async Task<List<WorkflowInstance>> GetWorkflowHistoryAsync(Guid userId, int pageSize = 50, int pageNumber = 1)
    {
        return await _context.WorkflowInstances
            .Where(w => w.InitiatedBy == userId)
            .OrderByDescending(w => w.InitiatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    #endregion

    #region Private Helper Methods

    private async Task<ValidationResult> ValidateMakerActionAsync(MakerAction action)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrEmpty(action.ActionType))
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Action type is required");
        }

        if (action.ResourceId == Guid.Empty)
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Resource ID is required");
        }

        if (string.IsNullOrEmpty(action.ResourceType))
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Resource type is required");
        }

        if (action.MakerId == Guid.Empty)
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Maker ID is required");
        }

        if (string.IsNullOrEmpty(action.BusinessJustification))
        {
            result.IsValid = false;
            result.ValidationErrors.Add("Business justification is required");
        }

        if (result.ValidationErrors.Any())
        {
            result.Message = string.Join("; ", result.ValidationErrors);
        }

        return result;
    }

    private async Task<List<string>> DetermineApprovalRequirementsAsync(MakerAction action)
    {
        var approverRoles = new List<string>();

        switch (action.ActionType.ToLower())
        {
            case "account_creation":
                approverRoles.Add("BackOfficeOfficer");
                if (action.Amount > 100000) // High value accounts
                {
                    approverRoles.Add("BranchManager");
                }
                break;

            case "loan_approval":
                approverRoles.Add("LoanOfficer");
                if (action.Amount > 50000) // High value loans
                {
                    approverRoles.Add("BranchManager");
                }
                break;

            case "policy_creation":
                approverRoles.Add("BancassuranceOfficer");
                if (action.Amount > 25000) // High value policies
                {
                    approverRoles.Add("BranchManager");
                }
                break;

            case "transaction_reversal":
                approverRoles.Add("BackOfficeOfficer");
                approverRoles.Add("BranchManager"); // Always requires manager approval
                break;

            default:
                approverRoles.Add("BackOfficeOfficer"); // Default approval
                break;
        }

        return approverRoles;
    }

    private async Task CreateApprovalStepsAsync(Guid workflowId, List<string> approverRoles)
    {
        for (int i = 0; i < approverRoles.Count; i++)
        {
            var step = new ApprovalStep
            {
                StepId = Guid.NewGuid(),
                WorkflowId = workflowId,
                StepOrder = i + 1,
                ApproverRole = approverRoles[i],
                Status = ApprovalStepStatus.Pending.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ApprovalSteps.Add(step);
        }

        await _context.SaveChangesAsync();
    }

    private DateTime? CalculateApprovalDeadline(string priority, DateTime? requestedDate)
    {
        var baseDate = requestedDate ?? DateTime.UtcNow;

        return priority.ToLower() switch
        {
            "critical" => baseDate.AddHours(4),
            "high" => baseDate.AddHours(24),
            "normal" => baseDate.AddDays(3),
            "low" => baseDate.AddDays(7),
            _ => baseDate.AddDays(3)
        };
    }

    private string DetermineEscalationTarget(string workflowType, decimal? amount)
    {
        // Always escalate to Branch Manager for high-value or complex workflows
        if (amount > 100000 || workflowType.Contains("loan") || workflowType.Contains("reversal"))
        {
            return "BranchManager";
        }

        return "BranchManager"; // Default escalation target
    }

    private async Task<int> GetNextStepOrderAsync(Guid workflowId)
    {
        var maxOrder = await _context.ApprovalSteps
            .Where(s => s.WorkflowId == workflowId)
            .MaxAsync(s => (int?)s.StepOrder) ?? 0;

        return maxOrder + 1;
    }

    private List<string> GetRequiredApproverRoles(string actionType)
    {
        return actionType.ToLower() switch
        {
            "account_creation" => new List<string> { "BackOfficeOfficer", "BranchManager" },
            "loan_approval" => new List<string> { "LoanOfficer", "BranchManager" },
            "policy_creation" => new List<string> { "BancassuranceOfficer", "BranchManager" },
            "transaction_reversal" => new List<string> { "BackOfficeOfficer", "BranchManager" },
            _ => new List<string> { "BackOfficeOfficer", "BranchManager" }
        };
    }

    #endregion
}