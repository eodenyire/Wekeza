namespace WekezaERMS.Domain.Enums;

/// <summary>
/// User roles for RBAC in the ERMS system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Risk Manager - Full access to create, update, and manage all risks
    /// </summary>
    RiskManager = 1,

    /// <summary>
    /// Risk Officer - Can create and update risks, manage mitigation actions
    /// </summary>
    RiskOfficer = 2,

    /// <summary>
    /// Risk Viewer - Read-only access to risk data
    /// </summary>
    RiskViewer = 3,

    /// <summary>
    /// Auditor - Read-only access with audit trail visibility
    /// </summary>
    Auditor = 4,

    /// <summary>
    /// Executive - High-level dashboard access and reporting
    /// </summary>
    Executive = 5,

    /// <summary>
    /// Administrator - Full system access including user management
    /// </summary>
    Administrator = 6
}
