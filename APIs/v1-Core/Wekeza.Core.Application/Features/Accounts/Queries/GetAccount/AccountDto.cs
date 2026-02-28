using AutoMapper;
using Wekeza.Core.Application.Common.Mappings;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Application.Features.Accounts.Queries.GetAccount;

/// <summary>
/// 📂 Wekeza.Core.Application/Features/Accounts/Queries/GetAccount
/// Even though we are currently "Writing" (Opening an Account), the response we send back is a "Read" model. We store these in the Queries sub-folder of the feature to maintain CQRS discipline.
/// 1. AccountDto.cs (The Data Contract)
/// We document this heavily because this is what the Mobile App developers will see. We use simple types (strings, decimals) because the frontend shouldn't have to understand our complex C# Money or AccountNumber objects.
/// The public-facing representation of a Wekeza Bank Account.
/// Designed for high-speed serialization and zero data leakage.
/// </summary>
public record AccountDto : IMapFrom<Account>
{
    public Guid Id { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Custom mapping logic to flatten the complex Domain Aggregate into a simple DTO.
    /// This is where we transform our Value Objects (Money, AccountNumber) into primitives.
    /// </summary>
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Account, AccountDto>()
            .ForMember(d => d.AccountNumber, opt => opt.MapFrom(s => s.AccountNumber.Value))
            .ForMember(d => d.Balance, opt => opt.MapFrom(s => s.Balance.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Balance.Currency.Code))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsFrozen ? "Frozen" : "Active"));
            // Note: CreatedAt will be mapped automatically if the names match or via a BaseEntity property.
    }
}
