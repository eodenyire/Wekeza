namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Product category - high-level grouping
/// </summary>
public enum ProductCategory
{
    Deposits,           // CASA, FD, RD
    Loans,              // Personal, Home, Auto, Business
    Cards,              // Debit, Credit, Prepaid
    Investments,        // Mutual Funds, Bonds
    TradeFinance,       // LC, BG
    Treasury            // FX, Money Market
}

/// <summary>
/// Specific product types within each category
/// </summary>
public enum ProductType
{
    // Deposit Products
    SavingsAccount,
    CurrentAccount,
    FixedDeposit,
    RecurringDeposit,
    CallDeposit,
    
    // Loan Products
    PersonalLoan,
    HomeLoan,
    AutoLoan,
    EducationLoan,
    BusinessLoan,
    Overdraft,
    CreditLine,
    MortgageLoan,
    
    // Card Products
    DebitCard,
    CreditCard,
    PrepaidCard,
    
    // Investment Products
    MutualFund,
    Bond,
    Equity,
    
    // Trade Finance
    LetterOfCredit,
    BankGuarantee,
    
    // Treasury
    ForeignExchange,
    MoneyMarket
}

/// <summary>
/// Product lifecycle status
/// </summary>
public enum ProductStatus
{
    Draft,              // Being configured
    Active,             // Available for customers
    Inactive,           // Temporarily disabled
    Expired,            // Past expiry date
    Withdrawn           // Permanently removed
}

/// <summary>
/// Interest calculation types
/// </summary>
public enum InterestType
{
    Credit,             // Interest paid to customer (deposits)
    Debit               // Interest charged to customer (loans)
}

/// <summary>
/// Interest calculation methods
/// </summary>
/// <summary>
/// Interest posting frequency
/// </summary>
public enum InterestPostingFrequency
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    HalfYearly,
    Yearly,
    OnMaturity
}

/// <summary>
/// Fee calculation types
/// </summary>
public enum FeeCalculationType
{
    Flat,               // Fixed amount
    Percentage,         // Percentage of transaction
    Tiered,             // Based on tiers
    Slab                // Slab-based
}

/// <summary>
/// Account types (enhanced from existing)
/// </summary>
public enum AccountType
{
    Savings,
    Current,
    FixedDeposit,
    RecurringDeposit,
    Overdraft,
    Loan
}

/// <summary>
/// Loan types (enhanced from existing)
/// </summary>
public enum LoanType
{
    Personal,
    Home,
    Auto,
    Education,
    Business,
    Agriculture,
    SME,
    Corporate,
    Overdraft
}

