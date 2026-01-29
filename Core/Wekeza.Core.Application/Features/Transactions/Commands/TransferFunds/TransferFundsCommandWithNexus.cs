using Wekeza.Core.Application.Common;
using Wekeza.Nexus.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Transactions.Commands.TransferFunds;

/// <summary>
/// Enhanced TransferFundsCommand with Wekeza Nexus integration support
/// 
/// This extends the original command to include fraud detection context:
/// - UserId for velocity tracking
/// - Device fingerprint for device intelligence
/// - Behavioral metrics for biometric analysis
/// - Session and channel information
/// 
/// Backward compatible: All new properties have defaults
/// </summary>
public record TransferFundsCommandWithNexus : ICommand<Guid>
{
    // Original properties
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string FromAccountNumber { get; init; } = string.Empty;
    public string ToAccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string Description { get; init; } = "Internal Transfer";
    
    // NEW: Fraud detection properties
    
    /// <summary>
    /// User ID initiating the transaction (for velocity tracking)
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Device fingerprint captured from client
    /// </summary>
    public DeviceFingerprint? DeviceInfo { get; init; }
    
    /// <summary>
    /// Behavioral biometric data captured from client
    /// </summary>
    public BehavioralMetrics? BehavioralData { get; init; }
    
    /// <summary>
    /// Channel through which transaction was initiated
    /// </summary>
    public string? Channel { get; init; }
    
    /// <summary>
    /// Session ID for tracking user session
    /// </summary>
    public string? SessionId { get; init; }
}
