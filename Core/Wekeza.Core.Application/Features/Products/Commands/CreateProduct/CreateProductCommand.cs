using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Command to create a new banking product
/// Similar to Finacle Product Factory and T24 ARRANGEMENT creation
/// </summary>
[Authorize(UserRole.Administrator)]
public record CreateProductCommand : ICommand<string>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public ProductCategory Category { get; init; }
    public ProductType Type { get; init; }
    public string Currency { get; init; } = "KES";
    public string Description { get; init; } = string.Empty;
    public string? MarketingDescription { get; init; }
    
    // Interest Configuration (optional)
    public InterestConfigDto? InterestConfig { get; init; }
    
    // Fee Configuration (optional)
    public List<FeeConfigDto>? Fees { get; init; }
    
    // Limit Configuration (optional)
    public LimitConfigDto? Limits { get; init; }
    
    // Eligibility Rules (optional)
    public List<EligibilityRuleDto>? EligibilityRules { get; init; }
    
    // Accounting Configuration (optional)
    public AccountingConfigDto? AccountingConfig { get; init; }
}

public record InterestConfigDto
{
    public InterestType Type { get; init; }
    public decimal Rate { get; init; }
    public InterestCalculationMethod CalculationMethod { get; init; }
    public InterestPostingFrequency PostingFrequency { get; init; }
    public bool IsTiered { get; init; }
    public List<InterestTierDto>? Tiers { get; init; }
}

public record InterestTierDto
{
    public decimal MinBalance { get; init; }
    public decimal MaxBalance { get; init; }
    public decimal Rate { get; init; }
}

public record FeeConfigDto
{
    public string FeeCode { get; init; } = string.Empty;
    public string FeeType { get; init; } = string.Empty;
    public string FeeName { get; init; } = string.Empty;
    public FeeCalculationType CalculationType { get; init; }
    public decimal Amount { get; init; }
    public decimal? Percentage { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
    public bool IsWaivable { get; init; } = true;
}

public record LimitConfigDto
{
    public decimal? MinBalance { get; init; }
    public decimal? MaxBalance { get; init; }
    public decimal? MinTransactionAmount { get; init; }
    public decimal? MaxTransactionAmount { get; init; }
    public decimal? DailyTransactionLimit { get; init; }
    public int? MaxTransactionsPerDay { get; init; }
    public decimal? MonthlyTransactionLimit { get; init; }
}

public record EligibilityRuleDto
{
    public string RuleType { get; init; } = string.Empty;
    public string Operator { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

public record AccountingConfigDto
{
    public string AssetGLCode { get; init; } = string.Empty;
    public string LiabilityGLCode { get; init; } = string.Empty;
    public string IncomeGLCode { get; init; } = string.Empty;
    public string ExpenseGLCode { get; init; } = string.Empty;
    public string InterestPayableGLCode { get; init; } = string.Empty;
    public string InterestReceivableGLCode { get; init; } = string.Empty;
}
