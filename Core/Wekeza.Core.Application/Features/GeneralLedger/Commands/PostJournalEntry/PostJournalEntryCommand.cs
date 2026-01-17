using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.GeneralLedger.Commands.PostJournalEntry;

/// <summary>
/// Command to post a journal entry to GL
/// Implements double-entry bookkeeping
/// </summary>
[Authorize(UserRole.Administrator, UserRole.SystemService)]
public record PostJournalEntryCommand : ICommand<string>
{
    public DateTime PostingDate { get; init; }
    public DateTime ValueDate { get; init; }
    public JournalType Type { get; init; }
    public string SourceType { get; init; } = string.Empty;
    public Guid SourceId { get; init; }
    public string SourceReference { get; init; } = string.Empty;
    public string Currency { get; init; } = "KES";
    public string? Description { get; init; }
    public List<JournalLineDto> Lines { get; init; } = new();
}

public record JournalLineDto
{
    public string GLCode { get; init; } = string.Empty;
    public decimal DebitAmount { get; init; }
    public decimal CreditAmount { get; init; }
    public string? CostCenter { get; init; }
    public string? ProfitCenter { get; init; }
    public string? Description { get; init; }
}
