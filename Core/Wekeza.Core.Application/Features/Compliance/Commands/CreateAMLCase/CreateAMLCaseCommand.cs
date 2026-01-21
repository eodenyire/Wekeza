using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Compliance.Commands.CreateAMLCase;

public record CreateAMLCaseCommand : ICommand<CreateAMLCaseResponse>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string CaseNumber { get; init; } = string.Empty;
    public Guid? PartyId { get; init; }
    public Guid? TransactionId { get; init; }
    public AMLAlertType AlertType { get; init; }
    public decimal RiskScore { get; init; }
    public string RiskMethodology { get; init; } = "STANDARD";
    public string? Description { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public Dictionary<string, decimal> RiskFactors { get; init; } = new();
}

public record CreateAMLCaseResponse
{
    public Guid CaseId { get; init; }
    public string CaseNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal RiskScore { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }
}