using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Workflows.Queries.GetWorkflowDetails;

public class GetWorkflowDetailsHandler : IRequestHandler<GetWorkflowDetailsQuery, WorkflowDetailsDto>
{
    private readonly IWorkflowRepository _workflowRepository;

    public GetWorkflowDetailsHandler(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    public async Task<WorkflowDetailsDto> Handle(GetWorkflowDetailsQuery request, CancellationToken cancellationToken)
    {
        var workflow = await _workflowRepository.GetByIdAsync(request.WorkflowId, cancellationToken);
        
        if (workflow == null)
        {
            throw new NotFoundException("Workflow", request.WorkflowId);
        }

        return new WorkflowDetailsDto
        {
            WorkflowId = workflow.Id,
            WorkflowCode = workflow.WorkflowCode,
            WorkflowName = workflow.WorkflowName,
            Type = workflow.Type.ToString(),
            Status = workflow.Status.ToString(),
            EntityType = workflow.EntityType,
            EntityReference = workflow.EntityReference,
            CurrentLevel = workflow.CurrentLevel,
            RequiredLevels = workflow.RequiredLevels,
            InitiatedDate = workflow.InitiatedDate,
            InitiatedBy = workflow.InitiatedBy,
            CompletedDate = workflow.CompletedDate,
            CompletedBy = workflow.CompletedBy,
            DueDate = workflow.DueDate,
            IsOverdue = workflow.IsOverdue,
            IsEscalated = workflow.IsEscalated,
            ApprovalSteps = workflow.ApprovalSteps.Select(s => new ApprovalStepDto
            {
                Level = s.Level,
                AssignedTo = s.AssignedTo,
                AssignedDate = s.AssignedDate,
                Status = s.Status.ToString(),
                ApprovedBy = s.ApprovedBy,
                ApprovedDate = s.ApprovedDate,
                Comments = s.Comments,
                ApproverRole = s.ApproverRole
            }).ToList(),
            Comments = workflow.Comments.Select(c => new CommentDto
            {
                CommentBy = c.CommentBy,
                Comment = c.Comment,
                CommentDate = c.CommentDate
            }).ToList(),
            RequestData = workflow.RequestData
        };
    }
}
