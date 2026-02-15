namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Types of parties that can interact with the bank
/// </summary>
public enum PartyType
{
    Individual,          // Retail customers
    Corporate,           // Companies, businesses
    Government,          // Government entities
    FinancialInstitution, // Banks, NBFCs
    Trust,               // Trusts
    Partnership,         // Partnerships
    Cooperative,         // Cooperatives, Saccos
    NGO                  // Non-profit organizations
}

/// <summary>
/// Party status in the system
/// </summary>
public enum PartyStatus
{
    Active,
    Inactive,
    Blocked,
    Suspended,
    Deceased,
    Merged,
    Closed
}

/// <summary>
/// Risk rating for AML/CFT
/// </summary>
public enum RiskRating
{
    Low,
    Medium,
    High,
    VeryHigh,
    Prohibited
}

/// <summary>
/// Customer segmentation
/// </summary>
public enum CustomerSegment
{
    Retail,              // Individual customers
    Affluent,            // High net worth individuals
    SME,                 // Small & Medium Enterprises
    Corporate,           // Large corporations
    Government,          // Government entities
    FinancialInstitution // Banks, NBFCs
}
