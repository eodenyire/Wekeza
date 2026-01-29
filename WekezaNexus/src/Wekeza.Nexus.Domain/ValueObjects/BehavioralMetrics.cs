namespace Wekeza.Nexus.Domain.ValueObjects;

/// <summary>
/// Captures behavioral biometric data from user interactions
/// Inspired by BioCatch behavioral analytics
/// </summary>
public record BehavioralMetrics
{
    /// <summary>
    /// Average typing speed in milliseconds between keystrokes
    /// </summary>
    public double? TypingSpeedMs { get; init; }
    
    /// <summary>
    /// Mouse movement velocity (pixels per second)
    /// </summary>
    public double? MouseVelocity { get; init; }
    
    /// <summary>
    /// Average hesitation time on critical fields (seconds)
    /// </summary>
    public double? HesitationTime { get; init; }
    
    /// <summary>
    /// Number of copy-paste actions detected
    /// High copy-paste may indicate social engineering
    /// </summary>
    public int CopyPasteCount { get; init; }
    
    /// <summary>
    /// Whether user is on an active voice/video call during transaction
    /// Indicates possible social engineering/grandparent scam
    /// </summary>
    public bool IsOnActiveCall { get; init; }
    
    /// <summary>
    /// Device orientation changes during session
    /// </summary>
    public int OrientationChanges { get; init; }
    
    /// <summary>
    /// Whether screen was shared/mirrored during transaction
    /// </summary>
    public bool IsScreenShared { get; init; }
    
    /// <summary>
    /// Session duration before transaction (seconds)
    /// Very short sessions may indicate automation
    /// </summary>
    public double SessionDuration { get; init; }
    
    /// <summary>
    /// Number of field errors/corrections made
    /// </summary>
    public int FieldErrors { get; init; }
    
    /// <summary>
    /// Whether biometric authentication was used (fingerprint, face)
    /// </summary>
    public bool BiometricAuthUsed { get; init; }
    
    /// <summary>
    /// Anomaly score compared to user's historical behavior (0-1)
    /// </summary>
    public double BehaviorAnomalyScore { get; init; }
}
