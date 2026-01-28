namespace WekezaERMS.Domain.Enums;

/// <summary>
/// Represents the main categories of risks in the banking system
/// Aligned with Basel III and industry standards
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
    Technology = 8
}
