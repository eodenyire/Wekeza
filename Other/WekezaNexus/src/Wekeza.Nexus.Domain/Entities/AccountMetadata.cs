namespace Wekeza.Nexus.Domain.Entities;

/// <summary>
/// Stores metadata about bank accounts for fraud analysis
/// Tracks account creation date, status, and risk indicators
/// </summary>
public class AccountMetadata
{
    /// <summary>
    /// Account number
    /// </summary>
    public string AccountNumber { get; init; } = string.Empty;
    
    /// <summary>
    /// When the account was created
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// Account holder's user ID
    /// </summary>
    public Guid? UserId { get; init; }
    
    /// <summary>
    /// Account status (Active, Dormant, Closed, etc.)
    /// </summary>
    public string Status { get; init; } = "Active";
    
    /// <summary>
    /// Age of the account in days
    /// </summary>
    public int AgeDays => (DateTime.UtcNow - CreatedAt).Days;
    
    /// <summary>
    /// Whether this account has been flagged for suspicious activity
    /// </summary>
    public bool IsFlagged { get; set; }
    
    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
