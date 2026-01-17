using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Digital Channel aggregate - Complete digital banking platform
/// Supports Internet Banking, Mobile Banking, USSD, and API channels
/// </summary>
public class DigitalChannel : AggregateRoot
{
    public string ChannelCode { get; private set; }
    public string ChannelName { get; private set; }
    public ChannelType ChannelType { get; private set; }
    public ChannelStatus Status { get; private set; }
    public string Description { get; private set; }
    public string BaseUrl { get; private set; }
    public string ApiVersion { get; private set; }
    public bool IsSecure { get; private set; }
    public bool RequiresAuthentication { get; private set; }
    public bool SupportsMFA { get; private set; }
    public int MaxConcurrentSessions { get; private set; }
    public TimeSpan SessionTimeout { get; private set; }
    public Money DailyTransactionLimit { get; private set; }
    public Money SingleTransactionLimit { get; private set; }
    public int MaxDailyTransactions { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    private readonly List<ChannelService> _services = new();
    public IReadOnlyList<ChannelService> Services => _services.AsReadOnly();

    private readonly List<ChannelSession> _sessions = new();
    public IReadOnlyList<ChannelSession> Sessions => _sessions.AsReadOnly();

    private readonly List<ChannelTransaction> _transactions = new();
    public IReadOnlyList<ChannelTransaction> Transactions => _transactions.AsReadOnly();

    private readonly List<ChannelAlert> _alerts = new();
    public IReadOnlyList<ChannelAlert> Alerts => _alerts.AsReadOnly();

    private DigitalChannel() { } // EF Core

    public DigitalChannel(
        Guid id,
        string channelCode,
        string channelName,
        ChannelType channelType,
        string description,
        string baseUrl,
        string apiVersion,
        bool isSecure,
        bool requiresAuthentication,
        bool supportsMFA,
        int maxConcurrentSessions,
        TimeSpan sessionTimeout,
        Money dailyTransactionLimit,
        Money singleTransactionLimit,
        int maxDailyTransactions,
        string createdBy)
    {
        Id = id;
        ChannelCode = channelCode;
        ChannelName = channelName;
        ChannelType = channelType;
        Description = description;
        BaseUrl = baseUrl;
        ApiVersion = apiVersion;
        IsSecure = isSecure;
        RequiresAuthentication = requiresAuthentication;
        SupportsMFA = supportsMFA;
        MaxConcurrentSessions = maxConcurrentSessions;
        SessionTimeout = sessionTimeout;
        DailyTransactionLimit = dailyTransactionLimit;
        SingleTransactionLimit = singleTransactionLimit;
        MaxDailyTransactions = maxDailyTransactions;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        
        Status = ChannelStatus.Active;

        // Add default services based on channel type
        AddDefaultServices();

        AddDomainEvent(new DigitalChannelCreatedDomainEvent(Id, ChannelCode, ChannelType, createdBy));
    }

    private void AddDefaultServices()
    {
        var defaultServices = ChannelType switch
        {
            ChannelType.InternetBanking => new[]
            {
                ("BALANCE_INQUIRY", "Balance Inquiry", true),
                ("FUND_TRANSFER", "Fund Transfer", true),
                ("BILL_PAYMENT", "Bill Payment", true),
                ("STATEMENT_DOWNLOAD", "Statement Download", true),
                ("CHEQUE_BOOK_REQUEST", "Cheque Book Request", true),
                ("ACCOUNT_MANAGEMENT", "Account Management", true)
            },
            ChannelType.MobileBanking => new[]
            {
                ("BALANCE_INQUIRY", "Balance Inquiry", true),
                ("MINI_STATEMENT", "Mini Statement", true),
                ("FUND_TRANSFER", "Fund Transfer", true),
                ("AIRTIME_PURCHASE", "Airtime Purchase", true),
                ("BILL_PAYMENT", "Bill Payment", true),
                ("QR_PAYMENT", "QR Code Payment", true)
            },
            ChannelType.USSD => new[]
            {
                ("BALANCE_INQUIRY", "Balance Inquiry", true),
                ("MINI_STATEMENT", "Mini Statement", true),
                ("FUND_TRANSFER", "Fund Transfer", true),
                ("AIRTIME_PURCHASE", "Airtime Purchase", true),
                ("PIN_CHANGE", "PIN Change", true)
            },
            ChannelType.API => new[]
            {
                ("ACCOUNT_INFO", "Account Information", true),
                ("TRANSACTION_HISTORY", "Transaction History", true),
                ("PAYMENT_INITIATION", "Payment Initiation", true),
                ("BALANCE_CHECK", "Balance Check", true)
            },
            _ => Array.Empty<(string, string, bool)>()
        };

        foreach (var (serviceCode, serviceName, isEnabled) in defaultServices)
        {
            var service = new ChannelService(
                Guid.NewGuid(),
                Id,
                serviceCode,
                serviceName,
                isEnabled,
                CreatedBy,
                DateTime.UtcNow);

            _services.Add(service);
        }
    }

    public void AddService(string serviceCode, string serviceName, bool isEnabled, string addedBy)
    {
        if (_services.Any(s => s.ServiceCode == serviceCode))
            throw new InvalidOperationException("Service with this code already exists");

        var service = new ChannelService(
            Guid.NewGuid(),
            Id,
            serviceCode,
            serviceName,
            isEnabled,
            addedBy,
            DateTime.UtcNow);

        _services.Add(service);

        AddDomainEvent(new ChannelServiceAddedDomainEvent(Id, ChannelCode, serviceCode, serviceName));
    }

    public void EnableService(string serviceCode, string enabledBy)
    {
        var service = _services.FirstOrDefault(s => s.ServiceCode == serviceCode);
        if (service == null)
            throw new InvalidOperationException("Service not found");

        service.Enable(enabledBy);

        AddDomainEvent(new ChannelServiceEnabledDomainEvent(Id, ChannelCode, serviceCode));
    }

    public void DisableService(string serviceCode, string disabledBy, string? reason = null)
    {
        var service = _services.FirstOrDefault(s => s.ServiceCode == serviceCode);
        if (service == null)
            throw new InvalidOperationException("Service not found");

        service.Disable(disabledBy, reason);

        AddDomainEvent(new ChannelServiceDisabledDomainEvent(Id, ChannelCode, serviceCode, reason));
    }

    public void StartSession(string sessionId, string userId, string deviceId, string ipAddress, string userAgent)
    {
        if (!IsOperational())
            throw new InvalidOperationException("Channel is not operational");

        var activeSessions = _sessions.Count(s => s.UserId == userId && s.Status == SessionStatus.Active);
        if (activeSessions >= MaxConcurrentSessions)
            throw new InvalidOperationException("Maximum concurrent sessions exceeded");

        var session = new ChannelSession(
            Guid.NewGuid(),
            Id,
            sessionId,
            userId,
            deviceId,
            ipAddress,
            userAgent,
            DateTime.UtcNow,
            DateTime.UtcNow.Add(SessionTimeout),
            SessionStatus.Active);

        _sessions.Add(session);

        AddDomainEvent(new ChannelSessionStartedDomainEvent(Id, ChannelCode, sessionId, userId));
    }

    public void EndSession(string sessionId, string reason)
    {
        var session = _sessions.FirstOrDefault(s => s.SessionId == sessionId);
        if (session == null)
            throw new InvalidOperationException("Session not found");

        session.End(reason);

        AddDomainEvent(new ChannelSessionEndedDomainEvent(Id, ChannelCode, sessionId, reason));
    }

    public void ProcessTransaction(string transactionId, string sessionId, string serviceCode, Money amount, string description, string processedBy)
    {
        var session = _sessions.FirstOrDefault(s => s.SessionId == sessionId && s.Status == SessionStatus.Active);
        if (session == null)
            throw new InvalidOperationException("Invalid or expired session");

        var service = _services.FirstOrDefault(s => s.ServiceCode == serviceCode && s.IsEnabled);
        if (service == null)
            throw new InvalidOperationException("Service not available");

        // Validate transaction limits
        ValidateTransactionLimits(session.UserId, amount);

        var transaction = new ChannelTransaction(
            Guid.NewGuid(),
            Id,
            transactionId,
            sessionId,
            serviceCode,
            amount,
            description,
            TransactionStatus.Initiated,
            processedBy,
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new ChannelTransactionInitiatedDomainEvent(Id, ChannelCode, transactionId, serviceCode, amount));
    }

    private void ValidateTransactionLimits(string userId, Money amount)
    {
        // Check single transaction limit
        if (amount.Amount > SingleTransactionLimit.Amount)
            throw new InvalidOperationException("Transaction amount exceeds single transaction limit");

        // Check daily transaction limit
        var today = DateTime.UtcNow.Date;
        var todayTransactions = _transactions.Where(t => 
            t.ProcessedAt.Date == today && 
            t.Status == TransactionStatus.Completed &&
            _sessions.Any(s => s.Id == t.SessionId && s.UserId == userId));

        var dailyAmount = todayTransactions.Sum(t => t.Amount.Amount);
        if (dailyAmount + amount.Amount > DailyTransactionLimit.Amount)
            throw new InvalidOperationException("Daily transaction limit exceeded");

        // Check daily transaction count
        var dailyCount = todayTransactions.Count();
        if (dailyCount >= MaxDailyTransactions)
            throw new InvalidOperationException("Maximum daily transactions exceeded");
    }

    public void CompleteTransaction(string transactionId, string completedBy)
    {
        var transaction = _transactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (transaction == null)
            throw new InvalidOperationException("Transaction not found");

        transaction.Complete(completedBy);

        AddDomainEvent(new ChannelTransactionCompletedDomainEvent(Id, ChannelCode, transactionId, transaction.Amount));
    }

    public void FailTransaction(string transactionId, string failureReason, string failedBy)
    {
        var transaction = _transactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (transaction == null)
            throw new InvalidOperationException("Transaction not found");

        transaction.Fail(failureReason, failedBy);

        AddDomainEvent(new ChannelTransactionFailedDomainEvent(Id, ChannelCode, transactionId, failureReason));
    }

    public void CreateAlert(AlertType alertType, string title, string message, AlertSeverity severity, string createdBy)
    {
        var alert = new ChannelAlert(
            Guid.NewGuid(),
            Id,
            alertType,
            title,
            message,
            severity,
            AlertStatus.Active,
            createdBy,
            DateTime.UtcNow);

        _alerts.Add(alert);

        AddDomainEvent(new ChannelAlertCreatedDomainEvent(Id, ChannelCode, alertType, severity));
    }

    public void UpdateStatus(ChannelStatus newStatus, string updatedBy, string? reason = null)
    {
        if (Status == newStatus)
            return;

        var previousStatus = Status;
        Status = newStatus;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;

        // End all active sessions if channel is being disabled
        if (newStatus == ChannelStatus.Inactive || newStatus == ChannelStatus.Maintenance)
        {
            foreach (var session in _sessions.Where(s => s.Status == SessionStatus.Active))
            {
                session.End($"Channel status changed to {newStatus}");
            }
        }

        AddDomainEvent(new ChannelStatusUpdatedDomainEvent(Id, ChannelCode, previousStatus, newStatus, reason));
    }

    public bool IsOperational()
    {
        return Status == ChannelStatus.Active;
    }

    public bool IsServiceAvailable(string serviceCode)
    {
        return IsOperational() && _services.Any(s => s.ServiceCode == serviceCode && s.IsEnabled);
    }

    public int GetActiveSessionCount()
    {
        return _sessions.Count(s => s.Status == SessionStatus.Active);
    }

    public int GetActiveSessionCount(string userId)
    {
        return _sessions.Count(s => s.UserId == userId && s.Status == SessionStatus.Active);
    }

    public Money GetDailyTransactionVolume(DateTime date)
    {
        var dayTransactions = _transactions.Where(t => 
            t.ProcessedAt.Date == date.Date && 
            t.Status == TransactionStatus.Completed);

        var totalAmount = dayTransactions.Sum(t => t.Amount.Amount);
        return new Money(totalAmount, DailyTransactionLimit.Currency);
    }

    public int GetDailyTransactionCount(DateTime date)
    {
        return _transactions.Count(t => 
            t.ProcessedAt.Date == date.Date && 
            t.Status == TransactionStatus.Completed);
    }

    public IEnumerable<ChannelSession> GetActiveSessions()
    {
        return _sessions.Where(s => s.Status == SessionStatus.Active);
    }

    public IEnumerable<ChannelTransaction> GetRecentTransactions(int count = 100)
    {
        return _transactions.OrderByDescending(t => t.ProcessedAt).Take(count);
    }

    public IEnumerable<ChannelAlert> GetActiveAlerts()
    {
        return _alerts.Where(a => a.Status == AlertStatus.Active);
    }

    public decimal GetSuccessRate(DateTime fromDate, DateTime toDate)
    {
        var totalTransactions = _transactions.Count(t => t.ProcessedAt >= fromDate && t.ProcessedAt <= toDate);
        if (totalTransactions == 0)
            return 100;

        var successfulTransactions = _transactions.Count(t => 
            t.ProcessedAt >= fromDate && 
            t.ProcessedAt <= toDate && 
            t.Status == TransactionStatus.Completed);

        return (decimal)successfulTransactions / totalTransactions * 100;
    }
}

/// <summary>
/// Channel Service configuration
/// </summary>
public class ChannelService
{
    public Guid Id { get; private set; }
    public Guid ChannelId { get; private set; }
    public string ServiceCode { get; private set; }
    public string ServiceName { get; private set; }
    public bool IsEnabled { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    public string? DisabledReason { get; private set; }

    private ChannelService() { } // EF Core

    public ChannelService(Guid id, Guid channelId, string serviceCode, string serviceName, bool isEnabled, string createdBy, DateTime createdAt)
    {
        Id = id;
        ChannelId = channelId;
        ServiceCode = serviceCode;
        ServiceName = serviceName;
        IsEnabled = isEnabled;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public void Enable(string enabledBy)
    {
        IsEnabled = true;
        DisabledReason = null;
        ModifiedBy = enabledBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void Disable(string disabledBy, string? reason = null)
    {
        IsEnabled = false;
        DisabledReason = reason;
        ModifiedBy = disabledBy;
        ModifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Channel Session tracking
/// </summary>
public class ChannelSession
{
    public Guid Id { get; private set; }
    public Guid ChannelId { get; private set; }
    public string SessionId { get; private set; }
    public string UserId { get; private set; }
    public string DeviceId { get; private set; }
    public string IpAddress { get; private set; }
    public string UserAgent { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime ExpiryTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public SessionStatus Status { get; private set; }
    public string? EndReason { get; private set; }

    private ChannelSession() { } // EF Core

    public ChannelSession(Guid id, Guid channelId, string sessionId, string userId, string deviceId, string ipAddress, string userAgent, DateTime startTime, DateTime expiryTime, SessionStatus status)
    {
        Id = id;
        ChannelId = channelId;
        SessionId = sessionId;
        UserId = userId;
        DeviceId = deviceId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        StartTime = startTime;
        ExpiryTime = expiryTime;
        Status = status;
    }

    public void End(string reason)
    {
        Status = SessionStatus.Ended;
        EndTime = DateTime.UtcNow;
        EndReason = reason;
    }

    public void Expire()
    {
        if (DateTime.UtcNow > ExpiryTime && Status == SessionStatus.Active)
        {
            End("Session expired");
        }
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiryTime;
    }

    public TimeSpan GetDuration()
    {
        var endTime = EndTime ?? DateTime.UtcNow;
        return endTime - StartTime;
    }
}

/// <summary>
/// Channel Transaction tracking
/// </summary>
public class ChannelTransaction
{
    public Guid Id { get; private set; }
    public Guid ChannelId { get; private set; }
    public string TransactionId { get; private set; }
    public string SessionId { get; private set; }
    public string ServiceCode { get; private set; }
    public Money Amount { get; private set; }
    public string Description { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string ProcessedBy { get; private set; }
    public DateTime ProcessedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }

    private ChannelTransaction() { } // EF Core

    public ChannelTransaction(Guid id, Guid channelId, string transactionId, string sessionId, string serviceCode, Money amount, string description, TransactionStatus status, string processedBy, DateTime processedAt)
    {
        Id = id;
        ChannelId = channelId;
        TransactionId = transactionId;
        SessionId = sessionId;
        ServiceCode = serviceCode;
        Amount = amount;
        Description = description;
        Status = status;
        ProcessedBy = processedBy;
        ProcessedAt = processedAt;
    }

    public void Complete(string completedBy)
    {
        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail(string failureReason, string failedBy)
    {
        Status = TransactionStatus.Failed;
        FailureReason = failureReason;
        CompletedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Channel Alert for notifications
/// </summary>
public class ChannelAlert
{
    public Guid Id { get; private set; }
    public Guid ChannelId { get; private set; }
    public AlertType AlertType { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public AlertStatus Status { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedBy { get; private set; }

    private ChannelAlert() { } // EF Core

    public ChannelAlert(Guid id, Guid channelId, AlertType alertType, string title, string message, AlertSeverity severity, AlertStatus status, string createdBy, DateTime createdAt)
    {
        Id = id;
        ChannelId = channelId;
        AlertType = alertType;
        Title = title;
        Message = message;
        Severity = severity;
        Status = status;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public void Resolve(string resolvedBy)
    {
        Status = AlertStatus.Resolved;
        ResolvedBy = resolvedBy;
        ResolvedAt = DateTime.UtcNow;
    }
}

// Enums
public enum ChannelType
{
    InternetBanking = 1,
    MobileBanking = 2,
    USSD = 3,
    API = 4,
    ATM = 5,
    POS = 6,
    CallCenter = 7,
    Branch = 8
}

public enum ChannelStatus
{
    Active = 1,
    Inactive = 2,
    Maintenance = 3,
    Suspended = 4
}

public enum SessionStatus
{
    Active = 1,
    Ended = 2,
    Expired = 3,
    Terminated = 4
}

public enum TransactionStatus
{
    Initiated = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}

public enum AlertType
{
    System = 1,
    Security = 2,
    Performance = 3,
    Maintenance = 4,
    Business = 5
}

public enum AlertSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum AlertStatus
{
    Active = 1,
    Resolved = 2,
    Dismissed = 3
}

// Domain Events
public record DigitalChannelCreatedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    ChannelType ChannelType,
    string CreatedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelServiceAddedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    string ServiceCode,
    string ServiceName) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelServiceEnabledDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    string ServiceCode) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelServiceDisabledDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    string ServiceCode,
    string? Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelSessionStartedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    string SessionId,
    string UserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelSessionEndedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    string SessionId,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelTransactionInitiatedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    string TransactionId,
    string ServiceCode,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelTransactionCompletedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    string TransactionId,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelTransactionFailedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    string TransactionId,
    string FailureReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelAlertCreatedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    AlertType AlertType,
    AlertSeverity Severity) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ChannelStatusUpdatedDomainEvent(
    Guid ChannelId,
    string ChannelCode,
    ChannelStatus PreviousStatus,
    ChannelStatus NewStatus,
    string? Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}