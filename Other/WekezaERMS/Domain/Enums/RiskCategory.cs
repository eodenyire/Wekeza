namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Represents the main categories of risks in the banking system
/// Aligned with Basel III, Riskonnect taxonomy, and industry standards
/// </summary>
public enum RiskCategory
{
    /// <summary>
    /// Credit Risk - Risk of loss due to counterparty default
    /// </summary>
    Credit = 1,

    /// <summary>
    /// Operational Risk - Risk from inadequate or failed processes, people, systems
    /// </summary>
    Operational = 2,

    /// <summary>
    /// Market Risk - Risk of losses from market price movements
    /// </summary>
    Market = 3,

    /// <summary>
    /// Liquidity Risk - Risk of inability to meet obligations when they come due
    /// </summary>
    Liquidity = 4,

    /// <summary>
    /// Strategic Risk - Risk from adverse business decisions
    /// </summary>
    Strategic = 5,

    /// <summary>
    /// Compliance Risk - Risk of legal or regulatory sanctions
    /// </summary>
    Compliance = 6,

    /// <summary>
    /// Reputation Risk - Risk of damage to the bank's reputation
    /// </summary>
    Reputation = 7,

    /// <summary>
    /// Technology Risk - Risk from IT systems, cybersecurity
    /// </summary>
    Technology = 8,

    /// <summary>
    /// Cyber and IT Risk - Cybersecurity threats, data breaches, IT infrastructure risks
    /// Includes third-party technology risk and digital transformation risk
    /// </summary>
    CyberAndIT = 9,

    /// <summary>
    /// Third-Party Risk - Risk from vendors, suppliers, and external service providers
    /// Includes vendor risk assessment, contract compliance, SLA tracking
    /// </summary>
    ThirdParty = 10,

    /// <summary>
    /// Insurable Risk - Risk that can be transferred through insurance
    /// Includes claims tracking, coverage gap analysis, premium optimization
    /// </summary>
    InsurableRisk = 11,

    /// <summary>
    /// Environmental Risk (ESG) - Environmental impact and sustainability risks
    /// Part of ESG risk management framework
    /// </summary>
    Environmental = 12,

    /// <summary>
    /// Social Risk (ESG) - Social responsibility and community impact risks
    /// Part of ESG risk management framework
    /// </summary>
    Social = 13,

    /// <summary>
    /// Governance Risk (ESG) - Corporate governance and ethics risks
    /// Part of ESG risk management framework
    /// </summary>
    Governance = 14,

    /// <summary>
    /// AI and Algorithm Risk - Risks from AI systems, machine learning models
    /// Includes algorithm bias, AI ethics, and automated decision-making risks
    /// </summary>
    AIAndAlgorithm = 15
}
