namespace Wekeza.Nexus.Domain.Enums;

/// <summary>
/// Risk level classification for fraud scoring
/// </summary>
public enum RiskLevel
{
    /// <summary>
    /// Very Low Risk (Score 0-200)
    /// </summary>
    VeryLow = 0,
    
    /// <summary>
    /// Low Risk (Score 201-400)
    /// </summary>
    Low = 1,
    
    /// <summary>
    /// Medium Risk (Score 401-600)
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// High Risk (Score 601-800)
    /// </summary>
    High = 3,
    
    /// <summary>
    /// Critical Risk (Score 801-1000)
    /// </summary>
    Critical = 4
}
