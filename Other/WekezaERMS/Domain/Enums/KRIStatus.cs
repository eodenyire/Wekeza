namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Status of KRI based on threshold comparison
/// </summary>
public enum KRIStatus
{
    /// <summary>
    /// Normal - KRI value is within acceptable range
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Warning - KRI value has exceeded warning threshold
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Critical - KRI value has exceeded critical threshold
    /// </summary>
    Critical = 3
}
