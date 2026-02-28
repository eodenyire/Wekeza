namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// GL Account types based on accounting equation
/// Assets = Liabilities + Equity
/// </summary>
public enum GLAccountType
{
    Asset,              // Debit balance (Cash, Loans, Fixed Assets)
    Liability,          // Credit balance (Deposits, Payables)
    Equity,             // Credit balance (Capital, Retained Earnings)
    Income,             // Credit balance (Interest Income, Fee Income)
    Expense             // Debit balance (Interest Expense, Operating Expense)
}

/// <summary>
/// GL Account categories for reporting
/// </summary>
public enum GLAccountCategory
{
    // Assets
    Cash,
    BankBalances,
    LoansAndAdvances,
    Investments,
    FixedAssets,
    OtherAssets,
    
    // Liabilities
    CustomerDeposits,
    BorrowedFunds,
    OtherLiabilities,
    
    // Equity
    ShareCapital,
    Reserves,
    RetainedEarnings,
    
    // Income
    InterestIncome,
    FeeIncome,
    OtherIncome,
    
    // Expenses
    InterestExpense,
    OperatingExpense,
    Provisions,
    OtherExpenses
}

/// <summary>
/// GL Account status
/// </summary>
public enum GLAccountStatus
{
    Active,             // Can post transactions
    Suspended,          // Temporarily disabled
    Closed              // Permanently closed
}

/// <summary>
/// Journal entry type
/// </summary>
public enum JournalType
{
    Standard,           // Regular posting
    Adjustment,         // Manual adjustment
    Reversal,           // Reversal entry
    Opening,            // Opening balance
    Closing,            // Closing entry
    Accrual,            // Accrual entry
    Provision           // Provision entry
}

/// <summary>
/// Journal entry status
/// </summary>
public enum JournalStatus
{
    Draft,              // Being prepared
    Posted,             // Posted to GL
    Reversed            // Reversed
}

/// <summary>
/// Financial statement type
/// </summary>
public enum FinancialStatementType
{
    BalanceSheet,
    IncomeStatement,
    CashFlow,
    TrialBalance
}

/// <summary>
/// Posting period status
/// </summary>
public enum PeriodStatus
{
    Open,               // Can post
    Closed,             // Cannot post
    Locked              // Permanently locked
}
