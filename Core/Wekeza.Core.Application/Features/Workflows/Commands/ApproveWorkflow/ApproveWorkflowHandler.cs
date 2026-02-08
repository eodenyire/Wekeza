using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Workflows.Commands.ApproveWorkflow;

public class ApproveWorkflowHandler : IRequestHandler<ApproveWorkflowCommand, bool>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ApproveWorkflowHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(ApproveWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetByIdAsync(request.WorkflowId, cancellationToken);
        
        if (workflow == null)
        {
            throw new NotFoundException($"Workflow with ID {request.WorkflowId} not found.", request.WorkflowId);
        }

        var approverId = _currentUserService.UserId?.ToString() ?? "System";
        var approverRole = _currentUserService.Roles.FirstOrDefault();

        // Approve the workflow
        workflow.Approve(approverId, request.Comments, (Wekeza.Core.Domain.Aggregates.UserRole)approverRole);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
