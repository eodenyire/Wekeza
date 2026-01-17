using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Products.Queries.GetProductCatalog;

/// <summary>
/// Query to get product catalog (all active products)
/// Similar to Finacle Product Catalog and T24 ARRANGEMENT catalog
/// </summary>
[Authorize(UserRole.Teller, UserRole.RiskOfficer, UserRole.Administrator)]
public record GetProductCatalogQuery : IQuery<ProductCatalogDto>
{
    public ProductCategory? Category { get; init; }
    public bool ActiveOnly { get; init; } = true;
}

public record ProductCatalogDto
{
    public List<ProductSummaryDto> Products { get; init; } = new();
    public int TotalCount { get; init; }
    public Dictionary<string, int> CountByCategory { get; init; } = new();
}

public record ProductSummaryDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal? InterestRate { get; init; }
    public string? InterestType { get; init; }
    public List<string> Features { get; init; } = new();
}
