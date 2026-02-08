using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Workflows.Commands.RejectWorkflow;

public class RejectWorkflowHandler : IRequestHandler<RejectWorkflowCommand, bool>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RejectWorkflowHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(RejectWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetByIdAsync(request.WorkflowId, cancellationToken);
        
        if (workflow == null)
        {
            throw new NotFoundException("Workflow", request.WorkflowId);
        }

        var rejectedBy = (_currentUserService.UserId ?? Guid.Empty).ToString();

        workflow.Reject(rejectedBy, request.Reason);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
