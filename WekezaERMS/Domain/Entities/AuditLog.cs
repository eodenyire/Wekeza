using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Audit Log Entry - Comprehensive audit trail for all risk activities
/// Aligned with Riskonnect's audit trail and documentation capabilities
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Entity type that was modified (Risk, Incident, Vendor, etc.)
    /// </summary>
    public string EntityType { get; private set; }

    /// <summary>
    /// Entity ID
    /// </summary>
    public Guid EntityId { get; private set; }

    /// <summary>
    /// Action performed (Created, Updated, Deleted, Assessed, etc.)
    /// </summary>
    public string Action { get; private set; }

    /// <summary>
    /// User who performed the action
    /// </summary>
    public Guid PerformedBy { get; private set; }

    /// <summary>
    /// User name (denormalized for reporting)
    /// </summary>
    public string PerformedByName { get; private set; }

    /// <summary>
    /// Timestamp of the action
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Previous value (for updates)
    /// </summary>
    public string? OldValue { get; private set; }

    /// <summary>
    /// New value (for updates)
    /// </summary>
    public string? NewValue { get; private set; }

    /// <summary>
    /// Field or property that was changed
    /// </summary>
    public string? FieldChanged { get; private set; }

    /// <summary>
    /// Additional context or notes
    /// </summary>
    public string? Context { get; private set; }

    /// <summary>
    /// IP address of the user
    /// </summary>
    public string? IPAddress { get; private set; }

    /// <summary>
    /// User agent information
    /// </summary>
    public string? UserAgent { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        string entityType,
        Guid entityId,
        string action,
        Guid performedBy,
        string performedByName,
        string? oldValue = null,
        string? newValue = null,
        string? fieldChanged = null,
        string? context = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            PerformedBy = performedBy,
            PerformedByName = performedByName,
            Timestamp = DateTime.UtcNow,
            OldValue = oldValue,
            NewValue = newValue,
            FieldChanged = fieldChanged,
            Context = context,
            IPAddress = ipAddress,
            UserAgent = userAgent
        };
    }
}
