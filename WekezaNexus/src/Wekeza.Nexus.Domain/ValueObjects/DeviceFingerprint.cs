namespace Wekeza.Nexus.Domain.ValueObjects;

/// <summary>
/// Captures device-specific fingerprinting data
/// Inspired by BioCatch and ThreatMark device intelligence
/// </summary>
public record DeviceFingerprint
{
    /// <summary>
    /// Unique device identifier (hashed)
    /// </summary>
    public string DeviceId { get; init; } = string.Empty;
    
    /// <summary>
    /// Device type (Mobile, Desktop, Tablet)
    /// </summary>
    public string DeviceType { get; init; } = string.Empty;
    
    /// <summary>
    /// Operating system (iOS, Android, Windows, macOS, Linux)
    /// </summary>
    public string OperatingSystem { get; init; } = string.Empty;
    
    /// <summary>
    /// Browser type and version
    /// </summary>
    public string Browser { get; init; } = string.Empty;
    
    /// <summary>
    /// Screen resolution
    /// </summary>
    public string ScreenResolution { get; init; } = string.Empty;
    
    /// <summary>
    /// User agent string
    /// </summary>
    public string UserAgent { get; init; } = string.Empty;
    
    /// <summary>
    /// Whether this is a recognized device for this user
    /// </summary>
    public bool IsRecognizedDevice { get; init; }
    
    /// <summary>
    /// IP address of the device
    /// </summary>
    public string IpAddress { get; init; } = string.Empty;
    
    /// <summary>
    /// Geographic location derived from IP
    /// </summary>
    public string Location { get; init; } = string.Empty;
    
    /// <summary>
    /// ISP/Network provider
    /// </summary>
    public string NetworkProvider { get; init; } = string.Empty;
    
    /// <summary>
    /// Whether the connection is through a VPN/Proxy
    /// </summary>
    public bool IsVpnOrProxy { get; init; }
    
    /// <summary>
    /// Timestamp when fingerprint was captured
    /// </summary>
    public DateTime CapturedAt { get; init; } = DateTime.UtcNow;
}
