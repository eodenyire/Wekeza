using MediatR;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Products.Queries.GetProductCatalog;

public class GetProductCatalogHandler : IRequestHandler<GetProductCatalogQuery, ProductCatalogDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductCatalogHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductCatalogDto> Handle(GetProductCatalogQuery request, CancellationToken cancellationToken)
    {
        var products = request.ActiveOnly
            ? await _productRepository.GetActiveProductsAsync(cancellationToken)
            : request.Category.HasValue
                ? await _productRepository.GetByCategoryAsync(request.Category.Value, cancellationToken)
                : (await _productRepository.GetActiveProductsAsync(cancellationToken)).ToList();

        var productSummaries = products.Select(p => new ProductSummaryDto
        {
            ProductCode = p.ProductCode,
            ProductName = p.ProductName,
            Category = p.Category.ToString(),
            Type = p.Type.ToString(),
            Status = p.Status.ToString(),
            Currency = p.Currency,
            Description = p.Description,
            InterestRate = p.InterestConfig?.Rate,
            InterestType = p.InterestConfig?.Type.ToString(),
            Features = BuildFeatureList(p)
        }).ToList();

        var countByCategory = products
            .GroupBy(p => p.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new ProductCatalogDto
        {
            Products = productSummaries,
            TotalCount = productSummaries.Count,
            CountByCategory = countByCategory
        };
    }

    private List<string> BuildFeatureList(Domain.Aggregates.Product product)
    {
        var features = new List<string>();

        if (product.InterestConfig != null)
        {
            features.Add($"Interest: {product.InterestConfig.Rate}% {product.InterestConfig.Type}");
        }

        if (product.Fees.Any())
        {
            features.Add($"{product.Fees.Count} fee(s) applicable");
        }

        if (product.Limits != null)
        {
            if (product.Limits.MinBalance.HasValue)
                features.Add($"Min Balance: {product.Currency} {product.Limits.MinBalance}");
            if (product.Limits.MaxBalance.HasValue)
                features.Add($"Max Balance: {product.Currency} {product.Limits.MaxBalance}");
        }

        return features;
    }
}
