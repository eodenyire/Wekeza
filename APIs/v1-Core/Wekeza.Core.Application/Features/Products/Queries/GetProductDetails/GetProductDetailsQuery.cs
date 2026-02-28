using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Application.Features.Products.Commands.CreateProduct;

namespace Wekeza.Core.Application.Features.Products.Queries.GetProductDetails;

[Authorize(UserRole.Teller, UserRole.RiskOfficer, UserRole.Administrator)]
public record GetProductDetailsQuery : IQuery<ProductDetailsDto>
{
    public string ProductCode { get; init; } = string.Empty;
}

public record ProductDetailsDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? MarketingDescription { get; init; }
    public DateTime EffectiveDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    
    public InterestConfigDto? InterestConfig { get; init; }
    public List<FeeConfigDto> Fees { get; init; } = new();
    public LimitConfigDto? Limits { get; init; }
    public List<EligibilityRuleDto> EligibilityRules { get; init; } = new();
    public AccountingConfigDto? AccountingConfig { get; init; }
    public Dictionary<string, string> Attributes { get; init; } = new();
}
