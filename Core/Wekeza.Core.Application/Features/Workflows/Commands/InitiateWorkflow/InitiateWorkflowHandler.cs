using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Services;

namespace Wekeza.Core.Application.Features.Workflows.Commands.InitiateWorkflow;

public class InitiateWorkflowHandler : IRequestHandler<InitiateWorkflowCommand, Result<Guid>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IApprovalMatrixRepository _approvalMatrixRepository;
    private readonly IUserRepository _userRepository;
    private readonly ApprovalRoutingService _approvalRoutingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public InitiateWorkflowHandler(
        IWorkflowRepository workflowRepository,
        IApprovalMatrixRepository approvalMatrixRepository,
        IUserRepository userRepository,
        ApprovalRoutingService approvalRoutingService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _workflowRepository = workflowRepository;
        _approvalMatrixRepository = approvalMatrixRepository;
        _userRepository = userRepository;
        _approvalRoutingService = approvalRoutingService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(InitiateWorkflowCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate initiator permissions
            var initiator = await _userRepository.GetByIdAsync(_currentUserService.UserId ?? Guid.Empty, cancellationToken);
            if (initiator == null)
            {
                return Result<Guid>.Failure("User not found");
            }

            // 2. Check if user can initiate this type of workflow
            if (!CanInitiateWorkflow(initiator, request.WorkflowCode))
            {
                return Result<Guid>.Failure("Insufficient permissions to initiate this workflow");
            }

            // 3. Get or create approval matrix for this workflow
            ApprovalMatrix approvalMatrix;
            if (request.UseAutoRouting)
            {
                approvalMatrix = await _approvalMatrixRepository.GetByWorkflowCodeAsync(
                    request.WorkflowCode, cancellationToken);
                
                if (approvalMatrix == null)
                {
                    // Create default approval matrix based on workflow type and amount
                    approvalMatrix = await CreateDefaultApprovalMatrix(request, cancellationToken);
                }
            }
            else
            {
                // Create custom approval matrix from request
                approvalMatrix = CreateCustomApprovalMatrix(request);
            }

            // 4. Determine approval routing
            var approvalSteps = await _approvalRoutingService.DetermineApprovalStepsAsync(
                approvalMatrix, request.Amount, request.BranchCode, request.Department, cancellationToken);

            if (!approvalSteps.Any())
            {
                return Result<Guid>.Failure("No approval steps could be determined for this workflow");
            }

            // 5. Create workflow instance
            var workflow = WorkflowInstance.Create(
                workflowCode: request.WorkflowCode,
                workflowName: request.WorkflowName,
                type: (Domain.Aggregates.WorkflowType)request.WorkflowType,
                entityType: request.EntityType,
                entityId: request.EntityId,
                entityReference: request.EntityReference,
                requiredLevels: approvalSteps.Count,
                initiatedBy: _currentUserService.Username ?? "Unknown",
                requestData: request.RequestData,
                slaHours: request.SLAHours
            );

            // 6. Add approval steps to workflow
            foreach (var step in approvalSteps)
            {
                var approvalStep = new Wekeza.Core.Domain.Aggregates.ApprovalStep(
                    id: Guid.NewGuid(),
                    workflowId: workflow.Id,
                    level: step.Level,
                    approverRole: step.RequiredRole.ToString(),
                    specificApprover: step.SpecificApprover,
                    isRequired: step.IsRequired,
                    minimumAmount: null,
                    maximumAmount: null,
                    createdAt: DateTime.UtcNow
                );
                workflow.AddApprovalStep(approvalStep);
            }

            // 7. Add initiator comments if provided
            if (!string.IsNullOrEmpty(request.InitiatorComments))
            {
                workflow.AddComment(_currentUserService.Username ?? "Unknown", request.InitiatorComments);
            }

            // 8. Set priority and due date
            workflow.SetPriority((Domain.Aggregates.Priority)request.Priority);
            workflow.SetDueDate(DateTime.UtcNow.AddHours(request.SLAHours));

            // 9. Save workflow
            await _workflowRepository.AddAsync(workflow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 10. Notify first level approvers
            await NotifyApprovers(workflow, 1, cancellationToken);

            return Result<Guid>.Success(workflow.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to initiate workflow: {ex.Message}");
        }
    }

    private bool CanInitiateWorkflow(User user, string workflowCode)
    {
        // Check if user has permission to initiate this type of workflow
        // This would be based on user roles and workflow configuration
        
        return workflowCode switch
        {
            "ACCOUNT_OPENING" => user.HasRole("Teller") || user.HasRole("CustomerService"),
            "LOAN_APPROVAL" => user.HasRole("LoanOfficer"),
            "LARGE_TRANSACTION" => user.HasRole("Teller") || user.HasRole("CustomerService"),
            "CARD_ISSUANCE" => user.HasRole("Teller") || user.HasRole("CustomerService"),
            "CUSTOMER_ONBOARDING" => user.HasRole("Teller") || user.HasRole("CustomerService"),
            _ => user.HasRole("Administrator") || user.HasRole("Supervisor")
        };
    }

    private async Task<ApprovalMatrix> CreateDefaultApprovalMatrix(
        InitiateWorkflowCommand request, CancellationToken cancellationToken)
    {
        // Create default approval matrix based on workflow type and amount
        var matrix = ApprovalMatrix.Create(
            matrixCode: request.WorkflowCode,
            matrixName: request.WorkflowName,
            entityType: request.EntityType,
            createdBy: _currentUserService.Username ?? "System"
        );

        // Add default approval levels based on amount thresholds
        if (request.Amount.HasValue)
        {
            var amount = request.Amount.Value;
            
            if (amount <= 10000) // Small amounts - single approval
            {
                matrix.AddRule(new ApprovalRule(1, new List<Domain.Enums.UserRole> { Domain.Enums.UserRole.Supervisor }, amount, amount, null, 24));
            }
            else if (amount <= 100000) // Medium amounts - supervisor approval
            {
                matrix.AddRule(new ApprovalRule(1, new List<Domain.Enums.UserRole> { Domain.Enums.UserRole.Supervisor }, 10001, 100000, null, 24));
            }
            else if (amount <= 1000000) // Large amounts - manager approval
            {
                matrix.AddRule(new ApprovalRule(1, new List<Domain.Enums.UserRole> { Domain.Enums.UserRole.BranchManager }, 100001, 1000000, null, 48));
            }
            else // Very large amounts - multiple approvals
            {
                matrix.AddRule(new ApprovalRule(1, new List<Domain.Enums.UserRole> { Domain.Enums.UserRole.BranchManager }, 1000001, null, null, 48));
                matrix.AddRule(new ApprovalRule(2, new List<Domain.Enums.UserRole> { Domain.Enums.UserRole.Administrator }, 1000001, null, null, 72));
            }
        }
        else
        {
            // Default single approval for non-monetary workflows
            matrix.AddRule(new ApprovalRule(1, new List<Domain.Enums.UserRole> { Domain.Enums.UserRole.Supervisor }, null, null, null, 24));
        }

        await _approvalMatrixRepository.AddAsync(matrix, cancellationToken);
        return matrix;
    }

    private ApprovalMatrix CreateCustomApprovalMatrix(InitiateWorkflowCommand request)
    {
        var matrix = ApprovalMatrix.Create(
            matrixCode: request.WorkflowCode,
            matrixName: request.WorkflowName,
            entityType: request.EntityType,
            createdBy: _currentUserService.Username ?? "System"
        );

        foreach (var level in request.ApprovalLevels)
        {
            foreach (var role in level.RequiredRoles)
            {
                matrix.AddRule(new ApprovalRule(
                    level.Level,
                    new List<Domain.Enums.UserRole> { role },
                    level.MinAmount,
                    level.MaxAmount,
                    null,
                    level.TimeoutHours
                ));
            }
        }

        return matrix;
    }

    private async Task NotifyApprovers(WorkflowInstance workflow, int level, CancellationToken cancellationToken)
    {
        // This would send notifications to approvers
        // Implementation would depend on notification service
        var approvers = await _approvalRoutingService.GetApproversForLevelAsync(
            workflow.Id, level, cancellationToken);

        // Note: Domain events are raised internally by the WorkflowInstance aggregate
        // Notifications will be handled by domain event handlers
    }
}