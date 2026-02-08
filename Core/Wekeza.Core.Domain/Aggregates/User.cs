using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// User Aggregate - Comprehensive user management with authentication, MFA, and security features
/// Supports enterprise-grade user lifecycle management, security policies, and audit trails
/// </summary>
public class User : AggregateRoot
{
    // Core Properties
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmployeeId { get; private set; }
    public UserStatus Status { get; private set; }
    
    // Authentication
    public string PasswordHash { get; private set; }
    public DateTime? PasswordExpiresAt { get; private set; }
    public bool MustChangePassword { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? LastLoginIp { get; private set; }
    
    // Multi-Factor Authentication
    public bool MfaEnabled { get; private set; }
    public MfaMethod MfaMethod { get; private set; }
    public string? MfaSecret { get; private set; }
    public List<string> BackupCodes { get; private set; }
    
    // Profile & Preferences
    public string? PhoneNumber { get; private set; }
    public string? Department { get; private set; }
    public string? JobTitle { get; private set; }
    public string? ManagerId { get; private set; }
    public Dictionary<string, object> Preferences { get; private set; }
    public string TimeZone { get; private set; }
    public string Language { get; private set; }
    
    // Security & Compliance
    public List<UserRole> Roles { get; private set; }
    public List<string> Permissions { get; private set; }
    public DateTime? LastPasswordChange { get; private set; }
    public List<UserSession> ActiveSessions { get; private set; }
    public SecurityClearanceLevel SecurityClearance { get; private set; }
    
    // Audit & Tracking
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private User() : base(Guid.NewGuid()) {
        BackupCodes = new List<string>();
        Preferences = new Dictionary<string, object>();
        Roles = new List<UserRole>();
        Permissions = new List<string>();
        ActiveSessions = new List<UserSession>();
        Metadata = new Dictionary<string, object>();
    }

    public User(
        string username,
        string email,
        string firstName,
        string lastName,
        string employeeId,
        string createdBy,
        SecurityClearanceLevel securityClearance = SecurityClearanceLevel.Basic,
        string timeZone = "UTC",
        string language = "en-US") : this()
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        EmployeeId = employeeId;
        Status = UserStatus.PendingActivation;
        SecurityClearance = securityClearance;
        TimeZone = timeZone;
        Language = language;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        MustChangePassword = true;
        MfaMethod = MfaMethod.None;

        AddDomainEvent(new UserCreatedDomainEvent(Id, Username, Email, CreatedBy));
    }

    // Business Methods
    public void ChangePassword(string newPasswordHash, string changedBy)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        var oldPasswordHash = PasswordHash;
        PasswordHash = newPasswordHash;
        LastPasswordChange = DateTime.UtcNow;
        MustChangePassword = false;
        FailedLoginAttempts = 0;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = changedBy;

        AddDomainEvent(new UserPasswordChangedDomainEvent(Id, Username, changedBy));
    }

    public void LockAccount(TimeSpan lockDuration, string reason, string lockedBy)
    {
        if (Status == UserStatus.Locked)
            return;

        Status = UserStatus.Locked;
        LockedUntil = DateTime.UtcNow.Add(lockDuration);
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = lockedBy;

        // End all active sessions
        foreach (var session in ActiveSessions.ToList())
        {
            EndSession(session.SessionId, "Account locked");
        }

        AddDomainEvent(new UserAccountLockedDomainEvent(Id, Username, reason, LockedUntil.Value, lockedBy));
    }

    public void UnlockAccount(string unlockedBy)
    {
        if (Status != UserStatus.Locked)
            return;

        Status = UserStatus.Active;
        LockedUntil = null;
        FailedLoginAttempts = 0;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = unlockedBy;

        AddDomainEvent(new UserAccountUnlockedDomainEvent(Id, Username, unlockedBy));
    }

    public void EnableMfa(MfaMethod method, string secret, string enabledBy)
    {
        if (method == MfaMethod.None)
            throw new ArgumentException("Invalid MFA method", nameof(method));

        MfaEnabled = true;
        MfaMethod = method;
        MfaSecret = secret;
        BackupCodes = GenerateBackupCodes();
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = enabledBy;

        AddDomainEvent(new UserMfaEnabledDomainEvent(Id, Username, method, enabledBy));
    }

    public void DisableMfa(string disabledBy)
    {
        if (!MfaEnabled)
            return;

        MfaEnabled = false;
        MfaMethod = MfaMethod.None;
        MfaSecret = null;
        BackupCodes.Clear();
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = disabledBy;

        AddDomainEvent(new UserMfaDisabledDomainEvent(Id, Username, disabledBy));
    }

    public void AddRole(UserRole role, string addedBy)
    {
        if (Roles.Any(r => r.RoleCode == role.RoleCode))
            return;

        Roles.Add(role);
        RefreshPermissions();
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = addedBy;

        AddDomainEvent(new UserRoleAddedDomainEvent(Id, Username, role.RoleCode, addedBy));
    }

    public void RemoveRole(string roleCode, string removedBy)
    {
        var role = Roles.FirstOrDefault(r => r.RoleCode == roleCode);
        if (role == null)
            return;

        Roles.Remove(role);
        RefreshPermissions();
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = removedBy;

        AddDomainEvent(new UserRoleRemovedDomainEvent(Id, Username, roleCode, removedBy));
    }

    public void UpdateProfile(UserProfile profile, string updatedBy)
    {
        FirstName = profile.FirstName ?? FirstName;
        LastName = profile.LastName ?? LastName;
        PhoneNumber = profile.PhoneNumber;
        Department = profile.Department;
        JobTitle = profile.JobTitle;
        ManagerId = profile.ManagerId;
        TimeZone = profile.TimeZone ?? TimeZone;
        Language = profile.Language ?? Language;
        
        if (profile.Preferences != null)
        {
            foreach (var pref in profile.Preferences)
            {
                Preferences[pref.Key] = pref.Value;
            }
        }

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new UserProfileUpdatedDomainEvent(Id, Username, updatedBy));
    }

    public void RecordLogin(string ipAddress, string sessionId)
    {
        LastLoginAt = DateTime.UtcNow;
        LastLoginIp = ipAddress;
        FailedLoginAttempts = 0;

        var session = new UserSession(sessionId, ipAddress, DateTime.UtcNow);
        ActiveSessions.Add(session);

        AddDomainEvent(new UserLoggedInDomainEvent(Id, Username, ipAddress, sessionId));
    }

    public void RecordFailedLogin(string ipAddress)
    {
        FailedLoginAttempts++;
        
        // Auto-lock after 5 failed attempts
        if (FailedLoginAttempts >= 5)
        {
            LockAccount(TimeSpan.FromMinutes(30), "Too many failed login attempts", "System");
        }

        AddDomainEvent(new UserLoginFailedDomainEvent(Id, Username, ipAddress, FailedLoginAttempts));
    }

    public void StartSession(UserSession session)
    {
        // Remove expired sessions
        ActiveSessions.RemoveAll(s => s.ExpiresAt < DateTime.UtcNow);
        
        ActiveSessions.Add(session);
        AddDomainEvent(new UserSessionStartedDomainEvent(Id, Username, session.SessionId, session.IpAddress));
    }

    public void EndSession(string sessionId, string reason = "User logout")
    {
        var session = ActiveSessions.FirstOrDefault(s => s.SessionId == sessionId);
        if (session == null)
            return;

        ActiveSessions.Remove(session);
        AddDomainEvent(new UserSessionEndedDomainEvent(Id, Username, sessionId, reason));
    }

    public void UpdateSecurityClearance(SecurityClearanceLevel level, string updatedBy)
    {
        var oldLevel = SecurityClearance;
        SecurityClearance = level;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;

        AddDomainEvent(new UserSecurityClearanceUpdatedDomainEvent(Id, Username, oldLevel, level, updatedBy));
    }

    public void Activate(string activatedBy)
    {
        if (Status == UserStatus.Active)
            return;

        Status = UserStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = activatedBy;

        AddDomainEvent(new UserActivatedDomainEvent(Id, Username, activatedBy));
    }

    public void Deactivate(string deactivatedBy)
    {
        if (Status == UserStatus.Inactive)
            return;

        Status = UserStatus.Inactive;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = deactivatedBy;

        // End all active sessions
        foreach (var session in ActiveSessions.ToList())
        {
            EndSession(session.SessionId, "Account deactivated");
        }

        AddDomainEvent(new UserDeactivatedDomainEvent(Id, Username, deactivatedBy));
    }

    public bool HasPermission(string permission)
    {
        return Permissions.Contains(permission);
    }

    public bool IsSessionActive(string sessionId)
    {
        return ActiveSessions.Any(s => s.SessionId == sessionId && s.ExpiresAt > DateTime.UtcNow);
    }

    public bool CanLogin()
    {
        return Status == UserStatus.Active && 
               (LockedUntil == null || LockedUntil < DateTime.UtcNow);
    }

    public void SetPassword(string passwordHash, string setBy)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
        LastPasswordChange = DateTime.UtcNow;
        MustChangePassword = false;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = setBy;
    }

    public void AssignRole(string roleCode, string assignedBy)
    {
        var role = new UserRole(roleCode, roleCode, new List<string>(), assignedBy);
        AddRole(role, assignedBy);
    }

    public bool HasRole(string roleCode)
    {
        return Roles.Any(r => r.RoleCode == roleCode);
    }

    public void UpdatePhoneNumber(string phoneNumber, string updatedBy)
    {
        PhoneNumber = phoneNumber;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;
    }

    public void UpdateDepartment(string department, string updatedBy)
    {
        Department = department;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;
    }

    public void UpdateJobTitle(string jobTitle, string updatedBy)
    {
        JobTitle = jobTitle;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = updatedBy;
    }

    public void AssignToBranch(string branchId, string assignedBy)
    {
        Metadata["BranchId"] = branchId;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = assignedBy;
    }

    private void RefreshPermissions()
    {
        Permissions.Clear();
        foreach (var role in Roles)
        {
            Permissions.AddRange(role.Permissions);
        }
        Permissions = Permissions.Distinct().ToList();
    }

    private List<string> GenerateBackupCodes()
    {
        var codes = new List<string>();
        var random = new Random();
        
        for (int i = 0; i < 10; i++)
        {
            codes.Add(random.Next(100000, 999999).ToString());
        }
        
        return codes;
    }
}

// Supporting classes
public class UserRole
{
    public string RoleCode { get; set; }
    public string RoleName { get; set; }
    public List<string> Permissions { get; set; }
    public DateTime AssignedAt { get; set; }
    public string AssignedBy { get; set; }

    public UserRole(string roleCode, string roleName, List<string> permissions, string assignedBy)
    {
        RoleCode = roleCode;
        RoleName = roleName;
        Permissions = permissions ?? new List<string>();
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }
}

public class UserSession
{
    public string SessionId { get; set; }
    public string IpAddress { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string UserAgent { get; set; }
    public Dictionary<string, object> Properties { get; set; }

    public UserSession(string sessionId, string ipAddress, DateTime startedAt, TimeSpan? duration = null)
    {
        SessionId = sessionId;
        IpAddress = ipAddress;
        StartedAt = startedAt;
        ExpiresAt = startedAt.Add(duration ?? TimeSpan.FromHours(8));
        Properties = new Dictionary<string, object>();
    }
}

public class UserProfile
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public string? ManagerId { get; set; }
    public string? TimeZone { get; set; }
    public string? Language { get; set; }
    public Dictionary<string, object>? Preferences { get; set; }
}

// Enumerations






