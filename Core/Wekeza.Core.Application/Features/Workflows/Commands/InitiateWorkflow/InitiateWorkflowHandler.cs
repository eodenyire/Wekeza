using MediatR;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Workflows.Commands.InitiateWorkflow;

public class InitiateWorkflowHandler : IRequestHandler<InitiateWorkflowCommand, Guid>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IApprovalMatrixRepository _matrixRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public InitiateWorkflowHandler(
        IWorkflowRepository workflowRepository,
        IApprovalMatrixRepository matrixRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _workflowRepository = workflowRepository;
        _matrixRepository = matrixRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(InitiateWorkflowCommand request, CancellationToken cancellationToken)
    {
        // Get approval matrix for entity type
        var matrix = await _matrixRepository.GetByEntityTypeAsync(request.EntityType, cancellationToken);
        if (matrix == null)
        {
            throw new InvalidOperationException($"No approval matrix found for entity type {request.EntityType}");
        }

        // Determine required approval levels based on amount and operation
        var amount = request.Amount ?? 0;
        var operation = request.Operation ?? "Create";
        var requiredLevels = matrix.GetRequiredLevels(amount, operation);

        // Get workflow name from matrix
        var workflowName = $"{request.EntityType} {operation} Approval";

        // Create workflow instance
        var workflow = WorkflowInstance.Create(
            workflowCode: request.WorkflowCode,
            workflowName: workflowName,
            type: requiredLevels > 1 ? WorkflowType.MultiLevel : WorkflowType.MakerChecker,
            entityType: request.EntityType,
            entityId: request.EntityId,
            entityReference: request.EntityReference,
            requiredLevels: requiredLevels,
            initiatedBy: _currentUserService.UserId ?? "System",
            requestData: request.RequestData,
            slaHours: 24);

        // Save workflow
        await _workflowRepository.AddAsync(workflow, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return workflow.Id;
    }
}
