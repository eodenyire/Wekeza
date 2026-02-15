using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Workflows.Commands.CreateApprovalWorkflow;

/// <summary>
/// Handler for creating approval workflows
/// </summary>
public class CreateApprovalWorkflowHandler : IRequestHandler<CreateApprovalWorkflowCommand, Result<Guid>>
{
    private readonly IApprovalWorkflowRepository _approvalWorkflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApprovalWorkflowHandler(
        IApprovalWorkflowRepository approvalWorkflowRepository,
        IUnitOfWork unitOfWork)
    {
        _approvalWorkflowRepository = approvalWorkflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateApprovalWorkflowCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if workflow already exists for this entity
            if (await _approvalWorkflowRepository.ExistsForEntityAsync(request.EntityType, request.EntityId))
                return Result<Guid>.Failure("Workflow already exists for this entity");

            // Create approval workflow
            var workflow = new ApprovalWorkflow(
                request.WorkflowId,
                request.WorkflowCode,
                request.WorkflowName,
                (Wekeza.Core.Domain.Aggregates.WorkflowType)request.WorkflowType,
                request.EntityType,
                request.EntityId,
                request.Amount.HasValue ? new Money(request.Amount.Value, new Currency(request.Currency)) : null,
                (Wekeza.Core.Domain.Aggregates.Priority)request.Priority,
                request.InitiatedBy,
                request.BranchCode,
                request.Department,
                request.RequiresMakerChecker);

            // Add approval steps
            foreach (var stepRequest in request.ApprovalSteps)
            {
                workflow.AddApprovalStep(
                    stepRequest.Level,
                    stepRequest.ApproverRole,
                    stepRequest.SpecificApprover,
                    stepRequest.IsRequired,
                    stepRequest.MinimumAmount.HasValue ? new Money(stepRequest.MinimumAmount.Value, new Currency(request.Currency)) : null,
                    stepRequest.MaximumAmount.HasValue ? new Money(stepRequest.MaximumAmount.Value, new Currency(request.Currency)) : null);
            }

            // Start the workflow
            workflow.StartWorkflow();

            // Save workflow
            await _approvalWorkflowRepository.AddAsync(workflow);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(workflow.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create approval workflow: {ex.Message}");
        }
    }
}