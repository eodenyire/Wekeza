using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface IGLAccountRepository
{
    Task<GLAccount?> GetByGLCodeAsync(string glCode, CancellationToken cancellationToken = default);
    Task<GLAccount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default); // Alias for compatibility
    Task<GLAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetByTypeAsync(GLAccountType accountType, CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetByCategoryAsync(GLAccountCategory category, CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetByParentAsync(string parentGLCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetLeafAccountsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<GLAccount>> GetChartOfAccountsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string glCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsByGLCodeAsync(string glCode, CancellationToken cancellationToken = default);
    Task<string> GenerateGLCodeAsync(GLAccountType accountType, GLAccountCategory category, CancellationToken cancellationToken = default);
    Task AddAsync(GLAccount glAccount, CancellationToken cancellationToken = default);
    void Add(GLAccount glAccount);
    void Update(GLAccount glAccount);
    void Remove(GLAccount glAccount);
}