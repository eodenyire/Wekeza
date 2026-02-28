using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Role aggregate
/// </summary>
public class RoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Role?> GetByRoleCodeAsync(string roleCode, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.RoleCode == roleCode, cancellationToken);
    }

    public async Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Role>> SearchAsync(string? searchTerm, RoleType? type, RoleStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(r => r.RoleName.Contains(searchTerm) || r.RoleCode.Contains(searchTerm) || r.Description.Contains(searchTerm));
        }

        if (type.HasValue)
        {
            query = query.Where(r => r.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.Permissions)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null) return new List<Role>();

        // Get Role entities based on user's role codes
        var roleCodes = user.Roles.Select(ur => ur.RoleCode).ToList();
        return await _context.Roles
            .Where(r => roleCodes.Contains(r.RoleCode))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _context.Roles.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await GetByIdAsync(id, cancellationToken);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetRoleUserCountAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await GetByIdAsync(roleId, cancellationToken);
        if (role == null) return 0;
        
        // Count users with this role via User aggregate
        return await _context.Users
            .CountAsync(u => u.Roles.Any(r => r.RoleCode == role.RoleCode), cancellationToken);
    }
}
