using Wekeza.Core.Application.Common.Mappings;
using Wekeza.Core.Domain.Aggregates;
///<summary>
/// 2. StatementResponseDto.cs (The Data Contract)
/// We don't just return a list. We return metadata—balance at the start of the period, balance at the end, and total counts. This is the Tier-1 Bank standard.
///</summary>
namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

public record StatementResponseDto
{
    public string AccountNumber { get; init; } = string.Empty;
    public decimal OpeningBalance { get; init; }
    public decimal ClosingBalance { get; init; }
    public List<TransactionItemDto> Transactions { get; init; } = new();
    
    // Pagination Metadata
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int TotalRecords { get; init; }
}

public record TransactionItemDto : IMapFrom<Transaction>
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string ReferenceId { get; init; } = string.Empty;
}
