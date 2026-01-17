using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Role Aggregate - Comprehensive role-based access control with hierarchical permissions
/// Supports enterprise-grade authorization, delegation, and security policies
/// </summary>
public class Role : AggregateRoot
{
    // Core Properties
    public string RoleCode { get; private set; }
    public string RoleName { get; private set; }
    public string Description { get; private set; }
    public RoleType Type { get; private set; }
    public RoleStatus Status { get; private set; }
    
    // Hierarchy & Inheritance
    public Guid? ParentRoleId { get; private set; }
    public List<Guid> ChildRoleIds { get; private set; }
    public bool InheritsPermissions { get; private set; }
    
    // Permissions & Access
    public List<Permission> Permissions { get; private set; }
    public List<string> AllowedModules { get; private set; }
    public List<string> RestrictedModules { get; private set; }
    public Dictionary<string, AccessLevel> ModuleAccess { get; private set; }
    
    // Constraints & Limits
    public List<string> IpRestrictions { get; private set; }
    public List<TimeWindow> TimeRestrictions { get; private set; }
    public decimal? TransactionLimit { get; private set; }
    public decimal? DailyLimit { get; private set; }
    public bool RequiresMfa { get; private set; }
    public SecurityClearanceLevel RequiredClearance { get; private set; }
    
    // Approval & Workflow
    public bool RequiresApproval { get; private set; }
    public List<string> ApprovalWorkflow { get; private set; }
    public int MaxConcurrentUsers { get; private set; }
    public TimeSpan SessionTimeout { get; private set; }
    
    // Audit & Compliance
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private Role()
    {
        ChildRoleIds = new List<Guid>();
        Permissions = new List<Permission>();
        AllowedModules = new List<string>();
        RestrictedModules = new List<string>();
        ModuleAccess = new Dictionary<string, AccessLevel>();
        IpRestrictions = new List<string>();
        TimeRestrictions = new List<TimeWindow>();
        ApprovalWorkflow = new List<string>();
        Metadata = new Dictionary<string, object>();
    }

    public Role(
        string roleCode,
        string roleName,
        string description,
        RoleType type,
        string createdBy,
        SecurityClearanceLevel requiredClearance = SecurityClearanceLevel.Internal) : this()
    {
        if (string.IsNullOrWhiteSpace(roleCode))
            throw new ArgumentException("Role code cannot be empty", nameof(roleCode));
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty", nameof(roleName));

        Id = Guid.NewGuid();
        RoleCode = roleCode;
        RoleName = roleName;
        Description = description;
        Type = type;
        Status = RoleStatus.Active;
        RequiredClearance = requiredClearance;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        InheritsPermissions = true;
        MaxConcurrentUsers = 100;
        SessionTimeout = TimeSpan.FromHours(8);

        AddDomainEvent(new RoleCreatedDomainEvent(Id, RoleCode, RoleName, Type, CreatedBy));
    }

    // Business Methods
    public void AddPermission(Permission permission, string addedBy)
    {
        if (Permissions.Any(p => p.Code == permission.Code))
            return;

        Permissions.Add(permission);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = addedBy;

        AddDomainEvent(new RolePermissionAddedDomainEvent(Id, RoleCode, permission.Code, addedBy));
    }

    public void RemovePermission(string permissionCode, string removedBy)
    {
        var permission = Permissions.FirstOrDefault(p => p.Code == permissionCode);
        if (permission == null)
            return;

        Permissions.Remove(permission);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = removedBy;

        AddDomainEvent(new RolePermissionRemovedDomainEvent(Id, RoleCode, permissionCode, removedBy));
    }

    public void UpdateModuleAccess(string module, AccessLevel level, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(module))
            throw new ArgumentException("Module cannot be empty", nameof(module));

        ModuleAccess[module] = level;
        
        // Update allowed/restricted modules based on access level
        if (level == AccessLevel.None)
        {
            AllowedModules.Remove(module);
            if (!RestrictedModules.Contains(module))
                RestrictedModules.Add(module);
        }
        else
        {
            RestrictedModules.Remove(module);
            if (!AllowedModules.Contains(module))
                AllowedModules.Add(module);
        }

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new RoleModuleAccessUpdatedDomainEvent(Id, RoleCode, module, level, updatedBy));
    }

    public void AddIpRestriction(string ipAddress, string addedBy)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be empty", nameof(ipAddress));

        if (!IpRestrictions.Contains(ipAddress))
        {
            IpRestrictions.Add(ipAddress);
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = addedBy;

            AddDomainEvent(new RoleIpRestrictionAddedDomainEvent(Id, RoleCode, ipAddress, addedBy));
        }
    }

    public void RemoveIpRestriction(string ipAddress, string removedBy)
    {
        if (IpRestrictions.Remove(ipAddress))
        {
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = removedBy;

            AddDomainEvent(new RoleIpRestrictionRemovedDomainEvent(Id, RoleCode, ipAddress, removedBy));
        }
    }

    public void AddTimeRestriction(TimeWindow window, string addedBy)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        TimeRestrictions.Add(window);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = addedBy;

        AddDomainEvent(new RoleTimeRestrictionAddedDomainEvent(Id, RoleCode, window, addedBy));
    }

    public void UpdateTransactionLimits(decimal? transactionLimit, decimal? dailyLimit, string updatedBy)
    {
        TransactionLimit = transactionLimit;
        DailyLimit = dailyLimit;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new RoleTransactionLimitsUpdatedDomainEvent(Id, RoleCode, transactionLimit, dailyLimit, updatedBy));
    }

    public void SetApprovalWorkflow(List<string> workflow, string updatedBy)
    {
        ApprovalWorkflow = workflow ?? new List<string>();
        RequiresApproval = ApprovalWorkflow.Any();
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new RoleApprovalWorkflowUpdatedDomainEvent(Id, RoleCode, workflow, updatedBy));
    }

    public void SetParentRole(Guid parentRoleId, string updatedBy)
    {
        ParentRoleId = parentRoleId;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new RoleHierarchyUpdatedDomainEvent(Id, RoleCode, parentRoleId, updatedBy));
    }

    public void AddChildRole(Guid childRoleId, string updatedBy)
    {
        if (!ChildRoleIds.Contains(childRoleId))
        {
            ChildRoleIds.Add(childRoleId);
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = updatedBy;

            AddDomainEvent(new RoleChildAddedDomainEvent(Id, RoleCode, childRoleId, updatedBy));
        }
    }

    public void RemoveChildRole(Guid childRoleId, string updatedBy)
    {
        if (ChildRoleIds.Remove(childRoleId))
        {
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = updatedBy;

            AddDomainEvent(new RoleChildRemovedDomainEvent(Id, RoleCode, childRoleId, updatedBy));
        }
    }

    public void EnableMfaRequirement(string updatedBy)
    {
        if (RequiresMfa)
            return;

        RequiresMfa = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new RoleMfaRequirementEnabledDomainEvent(Id, RoleCode, updatedBy));
    }

    public void DisableMfaRequirement(string updatedBy)
    {
        if (!RequiresMfa)
            return;

        RequiresMfa = false;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new RoleMfaRequirementDisabledDomainEvent(Id, RoleCode, updatedBy));
    }

    public void Activate(string activatedBy)
    {
        if (Status == RoleStatus.Active)
            return;

        Status = RoleStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = activatedBy;

        AddDomainEvent(new RoleActivatedDomainEvent(Id, RoleCode, activatedBy));
    }

    public void Deactivate(string deactivatedBy)
    {
        if (Status == RoleStatus.Inactive)
            return;

        Status = RoleStatus.Inactive;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = deactivatedBy;

        AddDomainEvent(new RoleDeactivatedDomainEvent(Id, RoleCode, deactivatedBy));
    }

    // Query Methods
    public bool HasPermission(string permissionCode)
    {
        return Permissions.Any(p => p.Code == permissionCode);
    }

    public bool CanAccessModule(string module)
    {
        if (RestrictedModules.Contains(module))
            return false;

        return AllowedModules.Contains(module) || 
               ModuleAccess.ContainsKey(module) && ModuleAccess[module] != AccessLevel.None;
    }

    public bool IsAccessAllowed(string ipAddress, DateTime accessTime)
    {
        // Check IP restrictions
        if (IpRestrictions.Any() && !IpRestrictions.Contains(ipAddress))
            return false;

        // Check time restrictions
        if (TimeRestrictions.Any())
        {
            var currentTime = accessTime.TimeOfDay;
            var currentDay = accessTime.DayOfWeek;
            
            return TimeRestrictions.Any(tr => tr.IsAllowed(currentDay, currentTime));
        }

        return true;
    }

    public AccessLevel GetModuleAccessLevel(string module)
    {
        return ModuleAccess.TryGetValue(module, out var level) ? level : AccessLevel.None;
    }

    public List<string> GetEffectivePermissions()
    {
        return Permissions.Select(p => p.Code).ToList();
    }
}

// Supporting classes
public class TimeWindow
{
    public DayOfWeek StartDay { get; set; }
    public DayOfWeek EndDay { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Description { get; set; }

    public TimeWindow(DayOfWeek startDay, DayOfWeek endDay, TimeSpan startTime, TimeSpan endTime, string description = "")
    {
        StartDay = startDay;
        EndDay = endDay;
        StartTime = startTime;
        EndTime = endTime;
        Description = description;
    }

    public bool IsAllowed(DayOfWeek day, TimeSpan time)
    {
        // Check if day is within range
        bool dayAllowed = StartDay <= EndDay 
            ? day >= StartDay && day <= EndDay
            : day >= StartDay || day <= EndDay;

        if (!dayAllowed)
            return false;

        // Check if time is within range
        return StartTime <= EndTime
            ? time >= StartTime && time <= EndTime
            : time >= StartTime || time <= EndTime;
    }
}

// Enumerations
public enum RoleType
{
    System,
    Functional,
    Departmental,
    Custom,
    Temporary
}

public enum RoleStatus
{
    Active,
    Inactive,
    Suspended,
    Deprecated
}

public enum AccessLevel
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4,
    Delete = 8,
    Admin = 15 // All permissions
}