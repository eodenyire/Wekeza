using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository for Product Factory management
/// Inspired by Finacle Product Catalog and T24 ARRANGEMENT
/// </summary>
public interface IProductRepository
{
    // Basic CRUD
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetByProductCodeAsync(string productCode, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Delete(Product product);
    
    // Query by Category & Type
    Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByTypeAsync(ProductType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);
    
    // Active Products
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveDepositProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveLoanProductsAsync(CancellationToken cancellationToken = default);
    
    // Eligibility
    Task<IEnumerable<Product>> GetEligibleProductsAsync(
        CustomerSegment segment,
        decimal amount,
        int customerAge,
        CancellationToken cancellationToken = default);
    
    // Search
    Task<IEnumerable<Product>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    
    // Analytics
    Task<int> GetTotalProductsCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<ProductCategory, int>> GetProductsByCategoryCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<ProductStatus, int>> GetProductsByStatusCountAsync(CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> ExistsByProductCodeAsync(string productCode, CancellationToken cancellationToken = default);
}
