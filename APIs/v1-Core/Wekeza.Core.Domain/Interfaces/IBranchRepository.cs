using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for Branch aggregate
/// </summary>
public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(Guid id);
    Task<Branch?> GetByCodeAsync(string branchCode);
    Task<IEnumerable<Branch>> GetAllAsync();
    Task<IEnumerable<Branch>> GetByStatusAsync(BranchStatus status);
    Task<IEnumerable<Branch>> GetByTypeAsync(BranchType branchType);
    Task<IEnumerable<Branch>> GetByManagerAsync(string managerId);
    Task<IEnumerable<Branch>> GetOperationalAsync();
    Task<bool> ExistsByCodeAsync(string branchCode);
    Task AddAsync(Branch branch);
    Task UpdateAsync(Branch branch);
    Task DeleteAsync(Guid id);
}