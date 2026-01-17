using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TaskAssignment aggregate
/// </summary>
public class TaskAssignmentRepository : ITaskAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public TaskAssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskAssignment?> GetByIdAsync(Guid id)
    {
        return await _context.TaskAssignments
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Include(t => t.Dependencies)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskAssignment>> GetByAssigneeAsync(string assigneeId)
    {
        return await _context.TaskAssignments
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Include(t => t.Dependencies)
            .Where(t => t.AssignedTo == assigneeId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskAssignment>> GetByStatusAsync(TaskStatus status)
    {
        return await _context.TaskAssignments
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskAssignment>> GetByPriorityAsync(Priority priority)
    {
        return await _context.TaskAssignments
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Where(t => t.Priority == priority)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskAssignment>> GetOverdueAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.TaskAssignments
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Where(t => t.DueDate.HasValue && 
                       t.DueDate.Value < now && 
                       t.Status != TaskStatus.Completed && 
                       t.Status != TaskStatus.Cancelled)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskAssignment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.TaskAssignments
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Where(t => t.CreatedAt >= fromDate && t.CreatedAt <= toDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(TaskAssignment task)
    {
        await _context.TaskAssignments.AddAsync(task);
    }

    public async Task UpdateAsync(TaskAssignment task)
    {
        _context.TaskAssignments.Update(task);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _context.TaskAssignments.FindAsync(id);
        if (task != null)
        {
            _context.TaskAssignments.Remove(task);
        }
    }
}