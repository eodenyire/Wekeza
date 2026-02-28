namespace Wekeza.Nexus.Domain.Enums;

/// <summary>
/// Specific reasons why a transaction was flagged as fraudulent
/// </summary>
public enum FraudReason
{
    None = 0,
    
    // Velocity-based reasons
    HighTransactionVelocity = 1,
    HighAmountVelocity = 2,
    UnusualTransactionPattern = 3,
    
    // Behavioral reasons
    AnomalousBehavior = 10,
    DeviceMismatch = 11,
    LocationAnomaly = 12,
    TimeAnomaly = 13,
    
    // Relationship-based reasons
    FirstTimeBeneficiary = 20,
    NewAccountBeneficiary = 21,
    CircularTransactionDetected = 22,
    MuleAccountPattern = 23,
    
    // Amount-based reasons
    UnusuallyHighAmount = 30,
    RoundAmountPattern = 31,
    
    // Account-based reasons
    AccountCompromised = 40,
    SuspiciousIPAddress = 41,
    MultipleFailedAttempts = 42,
    
    // External signals
    ExternalWatchlistMatch = 50,
    SanctionedEntity = 51
}
