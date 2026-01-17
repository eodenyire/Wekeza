using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository interface for TaskAssignment aggregate
/// </summary>
public interface ITaskAssignmentRepository
{
    Task<TaskAssignment?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskAssignment>> GetByAssigneeAsync(string assigneeId);
    Task<IEnumerable<TaskAssignment>> GetByStatusAsync(TaskStatus status);
    Task<IEnumerable<TaskAssignment>> GetByPriorityAsync(Priority priority);
    Task<IEnumerable<TaskAssignment>> GetOverdueAsync();
    Task<IEnumerable<TaskAssignment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task AddAsync(TaskAssignment task);
    Task UpdateAsync(TaskAssignment task);
    Task DeleteAsync(Guid id);
}