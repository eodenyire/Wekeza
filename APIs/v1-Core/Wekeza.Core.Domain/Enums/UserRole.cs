namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Defines the authorization roles in Wekeza Bank
/// Implements comprehensive role-based access control with maker-checker hierarchy
/// </summary>
public enum UserRole
{
    // Customer Roles
    Customer,           // Regular customer - can view own accounts, make transactions

    // Maker Roles (Can initiate but need approval)
    Teller,            // Branch teller - can process deposits, withdrawals, account opening
    LoanOfficer,       // Can process loan applications, disbursements
    InsuranceOfficer,  // Can process insurance products and claims
    CashOfficer,       // Can manage cash operations, ATM loading
    BackOfficeStaff,   // Can process back office operations
    CustomerService,   // Can handle customer inquiries and basic operations

    // Checker Roles (Can approve maker transactions)
    Supervisor,        // Can approve teller and basic operations
    BranchManager,     // Can approve branch operations and higher limits
    Administrator,     // Full system access and user management
    ITAdministrator,   // System administration and technical operations

    // Specialized Portal Roles
    ComplianceOfficer,      // AML, sanctions screening, regulatory compliance
    RiskOfficer,            // Risk assessment, account verification, limits management
    Auditor,                // Audit functions, read-only access to all modules
    SystemService,          // For automated processes and integrations
    TreasuryDealer,         // FX, money market, and liquidity operations
    TradeFinanceOfficer,    // Letters of credit, guarantees, and collections
    PaymentsOfficer,        // Payment processing and clearing
    ClearingOfficer,        // Clearing and settlement operations
    ProductManager,         // Product lifecycle and GL management
    FinanceController,      // Financial controls and reporting (maps from LoanOfficer)
    VaultOfficer,           // Vault operations, cash controls, and EOD/BOD
    CorporateBankingOfficer,// Corporate banking products and services

    // Executive Roles
    CEO,                    // Chief Executive Officer — board & executive portal
    RegionalManager,        // Multi-branch oversight
    HeadOfOperations,       // Operations oversight
    ChiefRiskOfficer,       // Enterprise risk management
    ChiefComplianceOfficer  // Enterprise compliance oversight
}
