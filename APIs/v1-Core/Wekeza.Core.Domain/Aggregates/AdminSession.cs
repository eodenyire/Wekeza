using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Admin Session - Tracks admin user sessions with detailed audit trail
/// Supports session monitoring, anomaly detection, and compliance requirements
/// </summary>
public class AdminSession : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Username { get; private set; }
    public string SessionToken { get; private set; }
    public string AdminRole { get; private set; }
    
    // Session Details
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public string Hostname { get; private set; }
    public string? DeviceIdFingernprint { get; private set; }
    
    // Timeline
    public DateTime LoginAt { get; private set; }
    public DateTime? LastActivityAt { get; private set; }
    public DateTime? LogoutAt { get; private set; }
    public TimeSpan? SessionDuration => LogoutAt.HasValue ? LogoutAt.Value - LoginAt : DateTime.UtcNow - LoginAt;
    
    // Activity Tracking
    public List<AdminAction> Actions { get; private set; }
    public int ActionCount { get; private set; }
    public List<string> AccessedModules { get; private set; }
    
    // Security
    public AdminSessionStatus Status { get; private set; }
    public bool MfaVerified { get; private set; }
    public string? AuthenticationMethod { get; private set; }
    public List<string> SecurityEvents { get; private set; }
    
    // Risk Assessment
    public string RiskLevel { get; private set; } // Low, Medium, High, Critical
    public List<string> AnomaliesDetected { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private AdminSession() : base(Guid.NewGuid())
    {
        Actions = new List<AdminAction>();
        AccessedModules = new List<string>();
        SecurityEvents = new List<string>();
        AnomaliesDetected = new List<string>();
        Metadata = new Dictionary<string, object>();
    }

    public AdminSession(
        Guid userId,
        string username,
        string sessionToken,
        string adminRole,
        string ipAddress,
        string userAgent,
        string hostname,
        bool mfaVerified = false) : this()
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        Id = Guid.NewGuid();
        UserId = userId;
        Username = username;
        SessionToken = sessionToken;
        AdminRole = adminRole;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Hostname = hostname;
        LoginAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
        Status = AdminSessionStatus.Active;
        MfaVerified = mfaVerified;
        RiskLevel = "Low";
    }

    public void RecordAction(AdminAction action)
    {
        if (Status != AdminSessionStatus.Active)
            throw new InvalidOperationException("Cannot record action on inactive session");

        Actions.Add(action);
        ActionCount++;
        LastActivityAt = DateTime.UtcNow;

        if (!AccessedModules.Contains(action.Module))
            AccessedModules.Add(action.Module);
    }

    public void DetectAnomaly(string anomalyDescription, string severity = "Medium")
    {
        AnomaliesDetected.Add($"[{severity}] {anomalyDescription} at {DateTime.UtcNow:O}");
        
        // Escalate risk level if critical anomalies
        if (severity == "Critical")
            RiskLevel = "Critical";
        else if (severity == "High" && RiskLevel != "Critical")
            RiskLevel = "High";
    }

    public void AddSecurityEvent(string eventDescription)
    {
        SecurityEvents.Add($"{eventDescription} at {DateTime.UtcNow:O}");
    }

    public void Logout(string reason = "User initiated")
    {
        Status = AdminSessionStatus.Closed;
        LogoutAt = DateTime.UtcNow;
        AddSecurityEvent($"Session closed: {reason}");
    }

    public void Timeout()
    {
        Status = AdminSessionStatus.TimedOut;
        LogoutAt = DateTime.UtcNow;
        AddSecurityEvent("Session timed out due to inactivity");
    }

    public void Terminate(string reason)
    {
        Status = AdminSessionStatus.Terminated;
        LogoutAt = DateTime.UtcNow;
        AddSecurityEvent($"Session terminated: {reason}");
    }

    public bool IsActive => Status == AdminSessionStatus.Active && LogoutAt == null;
    public bool HasExpired(TimeSpan timeout) => LastActivityAt.HasValue && 
        (DateTime.UtcNow - LastActivityAt.Value) > timeout;
}

public class AdminAction
{
    public Guid Id { get; set; }
    public string Module { get; set; }
    public string Action { get; set; }
    public string Resource { get; set; }
    public string ResourceId { get; set; }
    public string OperationType { get; set; } // Create, Read, Update, Delete, Execute
    public DateTime PerformedAt { get; set; }
    public string Status { get; set; } // Success, Failure
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Details { get; set; }
    public Dictionary<string, object>? ChangedFields { get; set; } // For audit trail
}

public enum AdminSessionStatus
{
    Active = 1,
    Inactive = 2,
    Closed = 3,
    TimedOut = 4,
    Terminated = 5,
    Suspicious = 6
}
