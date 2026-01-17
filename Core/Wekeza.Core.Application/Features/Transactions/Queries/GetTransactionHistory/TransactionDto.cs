using AutoMapper;
using Wekeza.Core.Application.Common.Mappings;
using Wekeza.Core.Domain.Aggregates;
///
/// 📂 Wekeza.Core.Application/Features/Transactions/Queries/GetTransactionHistory
/// This is our first major Query slice. We implement pagination because "The Beast" will eventually handle millions of rows per account.
/// 1. The Data Contract: TransactionDto.cs
///We flatten the data. The UI doesn't care about our internal aggregate IDs; it wants dates, descriptions, and amounts.
///
namespace Wekeza.Core.Application.Features.Transactions.Queries.GetTransactionHistory;

public record TransactionDto : IMapFrom<Transaction>
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // Deposit, Withdrawal, etc.
    public string Description { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string Status { get; init; } = "Completed"; // Default for now

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Transaction, TransactionDto>()
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Amount.Currency.Code))
            .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
    }
}
