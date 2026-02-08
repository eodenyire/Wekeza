using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface IGLAccountRepository
{
    Task<GLAccount?> GetByGLCodeAsync(string glCode);
    Task<GLAccount?> GetByCodeAsync(string code); // Alias for compatibility
    Task<GLAccount?> GetByIdAsync(Guid id);
    Task<IEnumerable<GLAccount>> GetAllAsync();
    Task<IEnumerable<GLAccount>> GetByTypeAsync(GLAccountType accountType);
    Task<IEnumerable<GLAccount>> GetByCategoryAsync(GLAccountCategory category);
    Task<IEnumerable<GLAccount>> GetByParentAsync(string parentGLCode);
    Task<IEnumerable<GLAccount>> GetLeafAccountsAsync();
    Task<IEnumerable<GLAccount>> GetChartOfAccountsAsync();
    Task<bool> ExistsAsync(string glCode);
    Task<bool> ExistsByGLCodeAsync(string glCode, CancellationToken cancellationToken = default);
    Task<string> GenerateGLCodeAsync(GLAccountType accountType, GLAccountCategory category);
    Task AddAsync(GLAccount glAccount, CancellationToken cancellationToken = default);
    void Add(GLAccount glAccount);
    void Update(GLAccount glAccount);
    void Remove(GLAccount glAccount);
}