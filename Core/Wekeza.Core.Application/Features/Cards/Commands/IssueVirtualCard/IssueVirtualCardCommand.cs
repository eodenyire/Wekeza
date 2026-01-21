using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Cards.Commands.IssueVirtualCard;

/// <summary>
/// Command to issue virtual debit card for online purchases
/// Virtual cards are instantly available and can be used for online transactions
/// </summary>
[Authorize(UserRole.Teller, UserRole.CustomerService, UserRole.Customer)]
public record IssueVirtualCardCommand : ICommand<Result<VirtualCardResult>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    public Guid CustomerId { get; init; }
    public Guid AccountId { get; init; }
    public string CardType { get; init; } = "VIRTUAL_DEBIT";
    public string CardBrand { get; init; } = "VISA";
    public decimal DailyLimit { get; init; } = 50000;
    public decimal MonthlyLimit { get; init; } = 200000;
    public decimal TransactionLimit { get; init; } = 10000;
    public List<string> AllowedMerchantCategories { get; init; } = new();
    public List<string> BlockedMerchantCategories { get; init; } = new();
    public bool AllowInternationalTransactions { get; init; } = true;
    public bool AllowOnlineTransactions { get; init; } = true;
    public bool AllowContactlessPayments { get; init; } = false; // Virtual cards don't support contactless
    public DateTime? ExpiryDate { get; init; } // If not provided, defaults to 3 years
    public string? CardholderName { get; init; }
    public string? DeliveryAddress { get; init; } // Not applicable for virtual cards
    public bool RequiresApproval { get; init; } = false; // Virtual cards can be instant
}

public record VirtualCardResult
{
    public Guid CardId { get; init; }
    public string CardNumber { get; init; } = string.Empty;
    public string MaskedCardNumber { get; init; } = string.Empty;
    public string ExpiryDate { get; init; } = string.Empty;
    public string CVV { get; init; } = string.Empty;
    public string CardholderName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime IssuedDate { get; init; }
    public DateTime ExpiryDateTime { get; init; }
    public decimal DailyLimit { get; init; }
    public decimal MonthlyLimit { get; init; }
    public decimal TransactionLimit { get; init; }
    public bool IsActive { get; init; }
    public string Message { get; init; } = string.Empty;
}