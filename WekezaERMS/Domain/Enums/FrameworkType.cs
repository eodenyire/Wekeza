namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Regulatory and compliance frameworks supported
/// Aligned with Riskonnect framework alignment capabilities
/// </summary>
public enum FrameworkType
{
    /// <summary>
    /// ISO 31000 - Risk Management
    /// </summary>
    ISO31000 = 1,

    /// <summary>
    /// ISO 27001 - Information Security Management
    /// </summary>
    ISO27001 = 2,

    /// <summary>
    /// ISO 22301 - Business Continuity Management
    /// </summary>
    ISO22301 = 3,

    /// <summary>
    /// COSO ERM - Enterprise Risk Management Framework
    /// </summary>
    COSO_ERM = 4,

    /// <summary>
    /// SOX - Sarbanes-Oxley Act
    /// </summary>
    SOX = 5,

    /// <summary>
    /// DORA - Digital Operational Resilience Act
    /// </summary>
    DORA = 6,

    /// <summary>
    /// APRA CPS 230 - Operational Risk Management
    /// </summary>
    APRA_CPS_230 = 7,

    /// <summary>
    /// GDPR - General Data Protection Regulation
    /// </summary>
    GDPR = 8,

    /// <summary>
    /// NIS2 - Network and Information Security Directive
    /// </summary>
    NIS2 = 9,

    /// <summary>
    /// NIST - Cybersecurity Framework
    /// </summary>
    NIST = 10,

    /// <summary>
    /// HIPAA - Health Insurance Portability and Accountability Act
    /// </summary>
    HIPAA = 11,

    /// <summary>
    /// Basel III - Banking Supervision Framework
    /// </summary>
    Basel_III = 12
}
