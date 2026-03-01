using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Infrastructure.Repositories.Admin;

public class ProductAdminRepository
{
    private readonly ApplicationDbContext _context;

    public ProductAdminRepository(ApplicationDbContext context) => _context = context;

    public async Task<ProductTemplate> GetProductTemplateByIdAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductTemplates.AsNoTracking().FirstOrDefaultAsync(p => p.Id == templateId, cancellationToken);
    }

    public async Task<List<ProductTemplate>> SearchProductTemplatesAsync(string productType, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ProductTemplates.AsNoTracking();
        if (!string.IsNullOrEmpty(productType)) query = query.Where(p => p.ProductType == productType);
        if (!string.IsNullOrEmpty(status)) query = query.Where(p => p.Status == status);
        return await query.OrderByDescending(p => p.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<ProductTemplate> AddProductTemplateAsync(ProductTemplate template, CancellationToken cancellationToken = default)
    {
        await _context.ProductTemplates.AddAsync(template, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return template;
    }

    public async Task<ProductTemplate> UpdateProductTemplateAsync(ProductTemplate template, CancellationToken cancellationToken = default)
    {
        _context.ProductTemplates.Update(template);
        await _context.SaveChangesAsync(cancellationToken);
        return template;
    }

    public async Task<FeeStructure> GetFeeStructureByIdAsync(Guid feeStructureId, CancellationToken cancellationToken = default)
    {
        return await _context.FeeStructures.AsNoTracking().FirstOrDefaultAsync(f => f.Id == feeStructureId, cancellationToken);
    }

    // NOTE: ProductCode property not yet implemented in FeeStructure domain aggregate (uses ProductId instead)
    // SearchFeeStructuresAsync temporarily disabled - use GetFeeStructureByIdAsync instead
    public async Task<List<FeeStructure>> GetAllFeeStructuresAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.FeeStructures.AsNoTracking();
        return await query.OrderBy(f => f.FeeName).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<FeeStructure> AddFeeStructureAsync(FeeStructure feeStructure, CancellationToken cancellationToken = default)
    {
        await _context.FeeStructures.AddAsync(feeStructure, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return feeStructure;
    }

    public async Task<FeeStructure> UpdateFeeStructureAsync(FeeStructure feeStructure, CancellationToken cancellationToken = default)
    {
        _context.FeeStructures.Update(feeStructure);
        await _context.SaveChangesAsync(cancellationToken);
        return feeStructure;
    }

    public async Task<InterestRateTable> GetInterestRateTableByIdAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        return await _context.InterestRateTables.AsNoTracking().FirstOrDefaultAsync(i => i.Id == tableId, cancellationToken);
    }

    // NOTE: ProductCode property not yet implemented in InterestRateTable domain aggregate (uses ProductId instead)
    // SearchInterestRateTablesAsync temporarily disabled - use GetInterestRateTableByIdAsync instead
    public async Task<List<InterestRateTable>> GetAllInterestRateTablesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.InterestRateTables.AsNoTracking();
        return await query.OrderByDescending(i => i.EffectiveDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<InterestRateTable> AddInterestRateTableAsync(InterestRateTable table, CancellationToken cancellationToken = default)
    {
        await _context.InterestRateTables.AddAsync(table, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return table;
    }

    public async Task<InterestRateTable> UpdateInterestRateTableAsync(InterestRateTable table, CancellationToken cancellationToken = default)
    {
        _context.InterestRateTables.Update(table);
        await _context.SaveChangesAsync(cancellationToken);
        return table;
    }

    public async Task<PostingRule> GetPostingRuleByIdAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        return await _context.PostingRules.AsNoTracking().FirstOrDefaultAsync(p => p.Id == ruleId, cancellationToken);
    }

    // NOTE: ProductCode property not yet implemented in PostingRule domain aggregate (uses ProductId instead)
    // SearchPostingRulesAsync temporarily disabled - use SearchPostingRulesByTransactionTypeAsync instead
    public async Task<List<PostingRule>> SearchPostingRulesByTransactionTypeAsync(string transactionType, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.PostingRules.AsNoTracking();
        if (!string.IsNullOrEmpty(transactionType)) 
            query = query.Where(p => p.TransactionType == transactionType);
        return await query.OrderBy(p => p.RuleCode).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task<PostingRule> AddPostingRuleAsync(PostingRule rule, CancellationToken cancellationToken = default)
    {
        await _context.PostingRules.AddAsync(rule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return rule;
    }

    public async Task<PostingRule> UpdatePostingRuleAsync(PostingRule rule, CancellationToken cancellationToken = default)
    {
        _context.PostingRules.Update(rule);
        await _context.SaveChangesAsync(cancellationToken);
        return rule;
    }
}
