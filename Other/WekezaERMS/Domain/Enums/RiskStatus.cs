namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Status of a risk entry in its lifecycle
/// </summary>
public enum RiskStatus
{
    /// <summary>
    /// Identified - Risk has been identified but not yet assessed
    /// </summary>
    Identified = 1,

    /// <summary>
    /// Under Assessment - Risk is being analyzed and evaluated
    /// </summary>
    UnderAssessment = 2,

    /// <summary>
    /// Active - Risk is assessed and being monitored
    /// </summary>
    Active = 3,

    /// <summary>
    /// Mitigating - Actions are being taken to reduce the risk
    /// </summary>
    Mitigating = 4,

    /// <summary>
    /// Escalated - Risk has been escalated to senior management
    /// </summary>
    Escalated = 5,

    /// <summary>
    /// Closed - Risk has been resolved or accepted
    /// </summary>
    Closed = 6,

    /// <summary>
    /// Archived - Historical risk for reference
    /// </summary>
    Archived = 7
}
