using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Workflows.Commands.InitiateWorkflow;

/// <summary>
/// Command to initiate a new workflow for approval
/// Implements Maker-Checker pattern
/// </summary>
public record InitiateWorkflowCommand : ICommand<Guid>
{
    public string WorkflowCode { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string EntityReference { get; init; } = string.Empty;
    public string RequestData { get; init; } = string.Empty; // JSON
    public decimal? Amount { get; init; } // For amount-based approval rules
    public string? Operation { get; init; } // Create, Update, Delete, etc.
}
