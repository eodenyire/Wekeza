using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Product Repository Implementation
/// High-performance repository for Product Factory management
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Product?> GetByProductCodeAsync(string productCode, CancellationToken ct = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.ProductCode == productCode, ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(product, ct);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, CancellationToken ct = default)
    {
        return await _context.Products
            .Where(p => p.Category == category)
            .OrderBy(p => p.ProductName)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetByTypeAsync(ProductType type, CancellationToken ct = default)
    {
        return await _context.Products
            .Where(p => p.Type == type)
            .OrderBy(p => p.ProductName)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, CancellationToken ct = default)
    {
        return await _context.Products
            .Where(p => p.Status == status)
            .OrderBy(p => p.ProductName)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Products
            .Where(p => p.Status == ProductStatus.Active &&
                       p.EffectiveDate <= now &&
                       (p.ExpiryDate == null || p.ExpiryDate > now))
            .OrderBy(p => p.Category)
            .ThenBy(p => p.ProductName)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetActiveDepositProductsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Products
            .Where(p => p.Status == ProductStatus.Active &&
                       p.Category == ProductCategory.Deposits &&
                       p.EffectiveDate <= now &&
                       (p.ExpiryDate == null || p.ExpiryDate > now))
            .OrderBy(p => p.ProductName)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetActiveLoanProductsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Products
            .Where(p => p.Status == ProductStatus.Active &&
                       p.Category == ProductCategory.Loans &&
                       p.EffectiveDate <= now &&
                       (p.ExpiryDate == null || p.ExpiryDate > now))
            .OrderBy(p => p.ProductName)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetEligibleProductsAsync(
        CustomerSegment segment,
        decimal amount,
        int customerAge,
        CancellationToken ct = default)
    {
        var activeProducts = await GetActiveProductsAsync(ct);
        
        return activeProducts.Where(p => p.IsEligible(Guid.Empty, amount, segment, customerAge));
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name, CancellationToken ct = default)
    {
        var searchTerm = name.ToLower();
        return await _context.Products
            .Where(p => p.ProductName.ToLower().Contains(searchTerm) ||
                       p.ProductCode.ToLower().Contains(searchTerm))
            .Take(100)
            .ToListAsync(ct);
    }

    public async Task<int> GetTotalProductsCountAsync(CancellationToken ct = default)
    {
        return await _context.Products.CountAsync(ct);
    }

    public async Task<Dictionary<ProductCategory, int>> GetProductsByCategoryCountAsync(CancellationToken ct = default)
    {
        return await _context.Products
            .GroupBy(p => p.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count, ct);
    }

    public async Task<Dictionary<ProductStatus, int>> GetProductsByStatusCountAsync(CancellationToken ct = default)
    {
        return await _context.Products
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, ct);
    }

    public async Task<bool> ExistsByProductCodeAsync(string productCode, CancellationToken ct = default)
    {
        return await _context.Products.AnyAsync(p => p.ProductCode == productCode, ct);
    }
}
