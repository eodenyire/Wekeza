using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class GLAccountRepository : IGLAccountRepository
{
    private readonly ApplicationDbContext _context;

    public GLAccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GLAccount?> GetByGLCodeAsync(string glCode, CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .FirstOrDefaultAsync(g => g.GLCode == glCode, ct);
    }

    public async Task<GLAccount?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .FirstOrDefaultAsync(g => g.Id == id, ct);
    }

    public async Task<IEnumerable<GLAccount>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .OrderBy(g => g.GLCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<GLAccount>> GetByTypeAsync(GLAccountType accountType, CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .Where(g => g.AccountType == accountType)
            .OrderBy(g => g.GLCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<GLAccount>> GetByCategoryAsync(GLAccountCategory category, CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .Where(g => g.Category == category)
            .OrderBy(g => g.GLCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<GLAccount>> GetByParentAsync(string parentGLCode, CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .Where(g => g.ParentGLCode == parentGLCode)
            .OrderBy(g => g.GLCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<GLAccount>> GetLeafAccountsAsync(CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .Where(g => g.IsLeaf)
            .OrderBy(g => g.GLCode)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<GLAccount>> GetChartOfAccountsAsync(CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .OrderBy(g => g.AccountType)
            .ThenBy(g => g.GLCode)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(string glCode, CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .AnyAsync(g => g.GLCode == glCode, ct);
    }

    public async Task<string> GenerateGLCodeAsync(GLAccountType accountType, GLAccountCategory category, CancellationToken ct = default)
    {
        // Generate GL Code based on account type and category
        var prefix = accountType switch
        {
            GLAccountType.Asset => "1",
            GLAccountType.Liability => "2",
            GLAccountType.Equity => "3",
            GLAccountType.Income => "4",
            GLAccountType.Expense => "5",
            _ => "9"
        };

        var categoryCode = ((int)category).ToString("00");
        
        // Find the next available number in this category
        var existingCodes = await _context.GLAccounts
            .Where(g => g.GLCode.StartsWith(prefix + categoryCode))
            .Select(g => g.GLCode)
            .ToListAsync(ct);

        var nextNumber = 1;
        string newCode;
        
        do
        {
            newCode = $"{prefix}{categoryCode}{nextNumber:000}";
            nextNumber++;
        } while (existingCodes.Contains(newCode));

        return newCode;
    }

    public async Task<GLAccount?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .FirstOrDefaultAsync(g => g.GLCode == code, ct);
    }

    public async Task<bool> ExistsByGLCodeAsync(string glCode, CancellationToken ct = default)
    {
        return await _context.GLAccounts
            .AnyAsync(g => g.GLCode == glCode, ct);
    }

    public async Task AddAsync(GLAccount glAccount, CancellationToken ct = default)
    {
        await _context.GLAccounts.AddAsync(glAccount, ct);
    }

    public void Add(GLAccount glAccount)
    {
        _context.GLAccounts.Add(glAccount);
    }

    public void Update(GLAccount glAccount)
    {
        _context.GLAccounts.Update(glAccount);
    }

    public void Remove(GLAccount glAccount)
    {
        _context.GLAccounts.Remove(glAccount);
    }
}