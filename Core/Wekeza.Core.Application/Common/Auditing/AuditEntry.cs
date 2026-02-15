namespace Wekeza.Core.Application.Common.Auditing;
///
///📂 Wekeza.Core.Application/Common/Auditing
///In a Principal-grade system, auditing is automated. We don't manually write "Log this" in every file. We use our MediatR Pipeline to intercept every command.
/// 1. AuditEntry.cs (The Data Structure)
/// This captures the "Who, What, When, and Where" of every administrative action.
///

public record AuditEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string? UserId { get; init; }     // The Staff/System ID
    public string Action { get; init; } = string.Empty; // e.g., "ApproveLoan"
    public string EntityName { get; init; } = string.Empty; // e.g., "Loan"
    public string EntityId { get; init; } = string.Empty;
    public string Changes { get; init; } = string.Empty; // JSON of what changed
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string RemoteIp { get; init; } = string.Empty;
    public string UserAgent { get; init; } = string.Empty;
}
