using MediatR;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Workflows.Queries.GetPendingApprovals;

public class GetPendingApprovalsHandler : IRequestHandler<GetPendingApprovalsQuery, List<PendingApprovalDto>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPendingApprovalsHandler(
        IWorkflowRepository workflowRepository,
        ICurrentUserService currentUserService)
    {
        _workflowRepository = workflowRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<PendingApprovalDto>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? "System";
        
        var workflows = await _workflowRepository.GetPendingForApproverAsync(userId, cancellationToken);

        return workflows.Select(w => new PendingApprovalDto
        {
            WorkflowId = w.Id,
            WorkflowName = w.WorkflowName,
            EntityType = w.EntityType,
            EntityReference = w.EntityReference,
            CurrentLevel = w.CurrentLevel,
            RequiredLevels = w.RequiredLevels,
            InitiatedDate = w.InitiatedDate,
            InitiatedBy = w.InitiatedBy,
            DueDate = w.DueDate,
            IsOverdue = w.IsOverdue,
            IsEscalated = w.IsEscalated,
            RequestSummary = BuildRequestSummary(w)
        }).ToList();
    }

    private string BuildRequestSummary(Domain.Aggregates.WorkflowInstance workflow)
    {
        return $"{workflow.EntityType} - {workflow.EntityReference} (Level {workflow.CurrentLevel + 1}/{workflow.RequiredLevels})";
    }
}
