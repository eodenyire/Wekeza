namespace Wekeza.Core.Domain.Enums;

/// <summary>
/// Defines the authorization roles in Wekeza Bank
/// </summary>
public enum UserRole
{
    Customer,           // Regular customer - can view own accounts, make transactions
    Teller,            // Branch teller - can process deposits, withdrawals
    LoanOfficer,       // Can approve/disburse loans
    RiskOfficer,       // Can verify business accounts, freeze accounts
    Administrator,     // Full system access
    SystemService      // For automated processes and integrations
}
