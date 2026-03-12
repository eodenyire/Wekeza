namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Status of mitigation actions
/// </summary>
public enum MitigationStatus
{
    /// <summary>
    /// Planned - Action has been planned but not started
    /// </summary>
    Planned = 1,

    /// <summary>
    /// In Progress - Action is currently being executed
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Completed - Action has been completed
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Delayed - Action is behind schedule
    /// </summary>
    Delayed = 4,

    /// <summary>
    /// Cancelled - Action has been cancelled
    /// </summary>
    Cancelled = 5
}
