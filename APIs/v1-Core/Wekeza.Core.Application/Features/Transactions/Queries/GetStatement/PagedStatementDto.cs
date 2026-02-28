///ðŸ“‚ Wekeza.Core.Application/Features/Transactions/Queries/GetStatement/
///1. PagedStatementDto.cs (The Data Contract)
///We don't just return a list. We return a "Beast-grade" package that includes metadata like total records and current page so the frontend (Mobile or Web) can build a smooth scrolling experience.
///
///
namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

public record PagedStatementDto
{
    public string AccountNumber { get; init; } = default!;
    public decimal CurrentBalance { get; init; }
    public List<StatementItemDto> Items { get; init; } = new();
    
    // Pagination Metadata
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalRecords { get; init; }
}

public record StatementItemDto(
    Guid Id, 
    decimal Amount, 
    string Type, // Debit/Credit
    string Description, 
    DateTime Timestamp, 
    string Reference
);
