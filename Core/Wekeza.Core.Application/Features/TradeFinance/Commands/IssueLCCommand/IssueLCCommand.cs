using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.TradeFinance.Commands.IssueLCCommand;

public record IssueLCCommand : ICommand<IssueLCResponse>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string LCNumber { get; init; } = string.Empty;
    public Guid ApplicantId { get; init; }
    public Guid BeneficiaryId { get; init; }
    public Guid IssuingBankId { get; init; }
    public Guid? AdvisingBankId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public DateTime ExpiryDate { get; init; }
    public DateTime? LastShipmentDate { get; init; }
    public LCType Type { get; init; } = LCType.Commercial;
    public string Terms { get; init; } = string.Empty;
    public string GoodsDescription { get; init; } = string.Empty;
    public bool IsTransferable { get; init; } = false;
    public List<DocumentRequirement> DocumentRequirements { get; init; } = new();
}

public record DocumentRequirement
{
    public string DocumentType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsRequired { get; init; } = true;
    public int Copies { get; init; } = 1;
}

public record IssueLCResponse
{
    public Guid LCId { get; init; }
    public string LCNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public string SwiftMessage { get; init; } = string.Empty;
}