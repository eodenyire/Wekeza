using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// AuditLog Aggregate - Comprehensive audit trail for compliance and forensics
/// Supports enterprise-grade logging, monitoring, and regulatory compliance
/// </summary>
public class AuditLog : AggregateRoot
{
    // Core Properties
    public string EventType { get; private set; }
    public string EventCategory { get; private set; }
    public AuditLevel Level { get; private set; }
    public DateTime Timestamp { get; private set; }
    
    // User & Session
    public string? UserId { get; private set; }
    public string? Username { get; private set; }
    public string? SessionId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    
    // Action Details
    public string Action { get; private set; }
    public string? Resource { get; private set; }
    public string? ResourceId { get; private set; }
    public Dictionary<string, object> OldValues { get; private set; }
    public Dictionary<string, object> NewValues { get; private set; }
    
    // Request & Response
    public string? RequestMethod { get; private set; }
    public string? RequestPath { get; private set; }
    public Dictionary<string, object> RequestData { get; private set; }
    public int? ResponseStatusCode { get; private set; }
    public string? ResponseMessage { get; private set; }
    
    // Result & Impact
    public AuditResult Result { get; private set; }
    public string? ResultMessage { get; private set; }
    public RiskLevel RiskLevel { get; private set; }
    public bool RequiresReview { get; private set; }
    
    // Context & Metadata
    public string ApplicationName { get; private set; }
    public string? ModuleName { get; private set; }
    public string? CorrelationId { get; private set; }
    public Dictionary<string, object> AdditionalData { get; private set; }
    
    // Compliance & Retention
    public List<string> ComplianceFlags { get; private set; }
    public DateTime RetentionUntil { get; private set; }
    public bool IsArchived { get; private set; }
    public string? ArchiveLocation { get; private set; }

    private AuditLog()
    {
        OldValues = new Dictionary<string, object>();
        NewValues = new Dictionary<string, object>();
        RequestData = new Dictionary<string, object>();
        AdditionalData = new Dictionary<string, object>();
        ComplianceFlags = new List<string>();
    }

    public AuditLog(
        string eventType,
        string eventCategory,
        string action,
        AuditLevel level = AuditLevel.Information,
        string applicationName = "Wekeza.Core") : this()
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));

        Id = Guid.NewGuid();
        EventType = eventType;
        EventCategory = eventCategory;
        Action = action;
        Level = level;
        ApplicationName = applicationName;
        Timestamp = DateTime.UtcNow;
        Result = AuditResult.Success;
        RiskLevel = RiskLevel.Low;
        
        // Default retention: 7 years for compliance
        RetentionUntil = DateTime.UtcNow.AddYears(7);
    }

    // Factory Methods
    public static AuditLog CreateUserAction(
        string userId,
        string username,
        string action,
        string resource,
        string? resourceId = null,
        string? sessionId = null,
        string? ipAddress = null)
    {
        var auditLog = new AuditLog("UserAction", "Security", action, AuditLevel.Information)
        {
            UserId = userId,
            Username = username,
            Resource = resource,
            ResourceId = resourceId,
            SessionId = sessionId,
            IpAddress = ipAddress
        };

        auditLog.AddDomainEvent(new AuditLogCreatedDomainEvent(auditLog.Id, "UserAction", userId, action));
        return auditLog;
    }

    public static AuditLog CreateSystemEvent(
        string eventType,
        string details,
        AuditLevel level = AuditLevel.Information,
        RiskLevel riskLevel = RiskLevel.Low)
    {
        var auditLog = new AuditLog(eventType, "System", "SystemEvent", level)
        {
            ResultMessage = details,
            RiskLevel = riskLevel
        };

        auditLog.AddDomainEvent(new AuditLogCreatedDomainEvent(auditLog.Id, eventType, "System", "SystemEvent"));
        return auditLog;
    }

    public static AuditLog CreateSecurityEvent(
        string eventType,
        string? userId,
        string action,
        RiskLevel riskLevel,
        string? ipAddress = null,
        string? details = null)
    {
        var auditLog = new AuditLog(eventType, "Security", action, AuditLevel.Security)
        {
            UserId = userId,
            IpAddress = ipAddress,
            ResultMessage = details,
            RiskLevel = riskLevel,
            RequiresReview = riskLevel >= RiskLevel.High
        };

        // Add compliance flags for security events
        auditLog.ComplianceFlags.Add("SECURITY_EVENT");
        if (riskLevel >= RiskLevel.High)
        {
            auditLog.ComplianceFlags.Add("HIGH_RISK");
        }

        auditLog.AddDomainEvent(new SecurityAuditLogCreatedDomainEvent(auditLog.Id, eventType, userId, riskLevel));
        return auditLog;
    }

    public static AuditLog CreateTransactionEvent(
        string userId,
        string username,
        string transactionType,
        decimal amount,
        string currency,
        string? accountId = null,
        string? sessionId = null,
        string? ipAddress = null)
    {
        var auditLog = new AuditLog("Transaction", "Financial", transactionType, AuditLevel.Information)
        {
            UserId = userId,
            Username = username,
            Resource = "Transaction",
            ResourceId = accountId,
            SessionId = sessionId,
            IpAddress = ipAddress
        };

        auditLog.AdditionalData["Amount"] = amount;
        auditLog.AdditionalData["Currency"] = currency;
        auditLog.AdditionalData["TransactionType"] = transactionType;

        // Add compliance flags for large transactions
        if (amount >= 10000) // Configurable threshold
        {
            auditLog.ComplianceFlags.Add("LARGE_TRANSACTION");
            auditLog.RequiresReview = true;
        }

        auditLog.AddDomainEvent(new TransactionAuditLogCreatedDomainEvent(auditLog.Id, userId, transactionType, amount));
        return auditLog;
    }

    // Business Methods
    public void SetUserContext(string userId, string username, string? sessionId = null, string? ipAddress = null, string? userAgent = null)
    {
        UserId = userId;
        Username = username;
        SessionId = sessionId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public void SetRequestContext(string method, string path, Dictionary<string, object>? requestData = null)
    {
        RequestMethod = method;
        RequestPath = path;
        if (requestData != null)
        {
            RequestData = requestData;
        }
    }

    public void SetResponseContext(int statusCode, string? message = null)
    {
        ResponseStatusCode = statusCode;
        ResponseMessage = message;
        
        // Determine result based on status code
        Result = statusCode >= 200 && statusCode < 300 ? AuditResult.Success :
                statusCode >= 400 && statusCode < 500 ? AuditResult.Unauthorized :
                AuditResult.Failure;
    }

    public void SetResourceContext(string resource, string? resourceId = null, Dictionary<string, object>? oldValues = null, Dictionary<string, object>? newValues = null)
    {
        Resource = resource;
        ResourceId = resourceId;
        if (oldValues != null)
        {
            OldValues = oldValues;
        }
        if (newValues != null)
        {
            NewValues = newValues;
        }
    }

    public void SetResult(AuditResult result, string? message = null)
    {
        Result = result;
        ResultMessage = message;
    }

    public void SetRiskLevel(RiskLevel riskLevel)
    {
        RiskLevel = riskLevel;
        RequiresReview = riskLevel >= RiskLevel.High;
        
        if (riskLevel >= RiskLevel.High && !ComplianceFlags.Contains("HIGH_RISK"))
        {
            ComplianceFlags.Add("HIGH_RISK");
        }
    }

    public void MarkForReview(string reason)
    {
        RequiresReview = true;
        AdditionalData["ReviewReason"] = reason;
        AdditionalData["MarkedForReviewAt"] = DateTime.UtcNow;

        AddDomainEvent(new AuditLogMarkedForReviewDomainEvent(Id, EventType, reason));
    }

    public void Archive(string location)
    {
        if (IsArchived)
            return;

        IsArchived = true;
        ArchiveLocation = location;
        AdditionalData["ArchivedAt"] = DateTime.UtcNow;

        AddDomainEvent(new AuditLogArchivedDomainEvent(Id, EventType, location));
    }

    public void AddComplianceFlag(string flag)
    {
        if (!ComplianceFlags.Contains(flag))
        {
            ComplianceFlags.Add(flag);
            AdditionalData[$"ComplianceFlag_{flag}_AddedAt"] = DateTime.UtcNow;
        }
    }

    public void RemoveComplianceFlag(string flag)
    {
        if (ComplianceFlags.Remove(flag))
        {
            AdditionalData[$"ComplianceFlag_{flag}_RemovedAt"] = DateTime.UtcNow;
        }
    }

    public void SetModuleContext(string moduleName, string? correlationId = null)
    {
        ModuleName = moduleName;
        CorrelationId = correlationId;
    }

    public void AddAdditionalData(string key, object value)
    {
        AdditionalData[key] = value;
    }

    public void AddAdditionalData(Dictionary<string, object> data)
    {
        foreach (var kvp in data)
        {
            AdditionalData[kvp.Key] = kvp.Value;
        }
    }

    public void ExtendRetention(TimeSpan extension, string reason)
    {
        RetentionUntil = RetentionUntil.Add(extension);
        AdditionalData["RetentionExtendedAt"] = DateTime.UtcNow;
        AdditionalData["RetentionExtensionReason"] = reason;

        AddDomainEvent(new AuditLogRetentionExtendedDomainEvent(Id, EventType, RetentionUntil, reason));
    }

    // Query Methods
    public bool IsExpired()
    {
        return DateTime.UtcNow > RetentionUntil;
    }

    public bool HasComplianceFlag(string flag)
    {
        return ComplianceFlags.Contains(flag);
    }

    public TimeSpan GetAge()
    {
        return DateTime.UtcNow - Timestamp;
    }

    public bool IsHighRisk()
    {
        return RiskLevel >= RiskLevel.High;
    }

    public bool IsSecurityEvent()
    {
        return EventCategory == "Security" || Level == AuditLevel.Security;
    }

    public Dictionary<string, object> GetSummary()
    {
        return new Dictionary<string, object>
        {
            ["Id"] = Id,
            ["EventType"] = EventType,
            ["Action"] = Action,
            ["Timestamp"] = Timestamp,
            ["UserId"] = UserId,
            ["Username"] = Username,
            ["Resource"] = Resource,
            ["Result"] = Result.ToString(),
            ["RiskLevel"] = RiskLevel.ToString(),
            ["RequiresReview"] = RequiresReview
        };
    }
}

// Enumerations
public enum AuditLevel
{
    Information,
    Warning,
    Error,
    Critical,
    Security
}

public enum AuditResult
{
    Success,
    Failure,
    Partial,
    Blocked,
    Unauthorized
}

public enum RiskLevel
{
    Low,
    Medium,
    High,
    Critical
}