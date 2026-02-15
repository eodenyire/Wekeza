using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.TradeFinance.Commands.IssueBGCommand;

public record IssueBGCommand : ICommand<IssueBGResponse>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string BGNumber { get; init; } = string.Empty;
    public Guid PrincipalId { get; init; }
    public Guid BeneficiaryId { get; init; }
    public Guid IssuingBankId { get; init; }
    public Guid? CounterGuaranteeId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public DateTime ExpiryDate { get; init; }
    public GuaranteeType Type { get; init; } = GuaranteeType.Performance;
    public string Terms { get; init; } = string.Empty;
    public string Purpose { get; init; } = string.Empty;
    public bool IsRevocable { get; init; } = false;
    public List<BGCondition> Conditions { get; init; } = new();
}

public record BGCondition
{
    public string ConditionType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsMandatory { get; init; } = true;
}

public record IssueBGResponse
{
    public Guid BGId { get; init; }
    public string BGNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public string SwiftMessage { get; init; } = string.Empty;
}