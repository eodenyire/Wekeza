namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Frequency of interest payment for deposits
/// </summary>
public enum InterestPaymentFrequency
{
    Monthly = 1,
    Quarterly = 2,
    HalfYearly = 3,
    Yearly = 4,
    OnMaturity = 5
}

/// <summary>
/// Types of deposit products
/// </summary>
public enum DepositType
{
    FixedDeposit = 1,
    RecurringDeposit = 2,
    TermDeposit = 3,
    CallDeposit = 4,
    CertificateOfDeposit = 5,
    CumulativeDeposit = 6,
    NonCumulativeDeposit = 7
}

/// <summary>
/// Deposit renewal options
/// </summary>
public enum RenewalOption
{
    AutoRenewal = 1,
    ManualRenewal = 2,
    NoRenewal = 3,
    PartialRenewal = 4
}

/// <summary>
/// Deposit closure reasons
/// </summary>
public enum ClosureReason
{
    Maturity = 1,
    CustomerRequest = 2,
    EmergencyWithdrawal = 3,
    LoanAdjustment = 4,
    DeathOfDepositor = 5,
    BankInitiated = 6,
    RegulatoryCompliance = 7,
    AccountClosure = 8
}

/// <summary>
/// Deposit nomination types
/// </summary>
public enum NominationType
{
    Single = 1,
    Multiple = 2,
    Successive = 3
}

/// <summary>
/// Deposit maturity instruction types
/// </summary>
public enum MaturityInstruction
{
    CreditToAccount = 1,
    IssueCheque = 2,
    AutoRenewal = 3,
    PartialWithdrawal = 4,
    TransferToAnother = 5
}

/// <summary>
/// Interest rate types for deposits
/// </summary>
public enum InterestRateType
{
    Fixed = 1,
    Variable = 2,
    Floating = 3,
    Annual = 4,
    Monthly = 5,
    Daily = 6,
    Compound = 7,
    Simple = 8
}