using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.TradeFinance.Queries.GetLCDetails;

public record GetLCDetailsQuery : IQuery<LCDetailsDto>
{
    public Guid LCId { get; init; }
    public string? LCNumber { get; init; }
}

public record LCDetailsDto
{
    public Guid Id { get; init; }
    public string LCNumber { get; init; } = string.Empty;
    public string ApplicantName { get; init; } = string.Empty;
    public string BeneficiaryName { get; init; } = string.Empty;
    public string IssuingBankName { get; init; } = string.Empty;
    public string? AdvisingBankName { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public DateTime ExpiryDate { get; init; }
    public DateTime? LastShipmentDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Terms { get; init; } = string.Empty;
    public string GoodsDescription { get; init; } = string.Empty;
    public bool IsTransferable { get; init; }
    public bool IsConfirmed { get; init; }
    public bool IsExpired { get; init; }
    public int DaysToExpiry { get; init; }
    public List<LCAmendmentDto> Amendments { get; init; } = new();
    public List<TradeDocumentDto> Documents { get; init; } = new();
}

public record LCAmendmentDto
{
    public Guid Id { get; init; }
    public int AmendmentNumber { get; init; }
    public string AmendmentDetails { get; init; } = string.Empty;
    public decimal PreviousAmount { get; init; }
    public decimal NewAmount { get; init; }
    public DateTime PreviousExpiryDate { get; init; }
    public DateTime NewExpiryDate { get; init; }
    public DateTime AmendmentDate { get; init; }
    public string Status { get; init; } = string.Empty;
}

public record TradeDocumentDto
{
    public Guid Id { get; init; }
    public string DocumentType { get; init; } = string.Empty;
    public string DocumentNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime UploadedAt { get; init; }
    public string UploadedBy { get; init; } = string.Empty;
    public string Comments { get; init; } = string.Empty;
}