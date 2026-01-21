using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.ValueObjects;

/// <summary>
/// Permission Value Object - Represents a specific permission with resource and action details
/// Supports fine-grained authorization and access control
/// </summary>
public class Permission : ValueObject
{
    public string Code { get; }
    public string Name { get; }
    public string Resource { get; }
    public string Action { get; }
    public AccessLevel Level { get; }
    public List<string> Conditions { get; }
    public bool IsSystemPermission { get; }
    public string Description { get; }

    public Permission(
        string code,
        string name,
        string resource,
        string action,
        AccessLevel level = AccessLevel.Read,
        List<string>? conditions = null,
        bool isSystemPermission = false,
        string description = "")
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Permission code cannot be empty", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(resource))
            throw new ArgumentException("Resource cannot be empty", nameof(resource));
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));

        Code = code;
        Name = name;
        Resource = resource;
        Action = action;
        Level = level;
        Conditions = conditions ?? new List<string>();
        IsSystemPermission = isSystemPermission;
        Description = description;
    }

    // Factory Methods for Common Permissions
    public static Permission CreateReadPermission(string resource, string? description = null)
    {
        return new Permission(
            $"{resource}.Read",
            $"Read {resource}",
            resource,
            "Read",
            AccessLevel.Read,
            description: description ?? $"Allows reading {resource} data");
    }

    public static Permission CreateWritePermission(string resource, string? description = null)
    {
        return new Permission(
            $"{resource}.Write",
            $"Write {resource}",
            resource,
            "Write",
            AccessLevel.Write,
            description: description ?? $"Allows creating and updating {resource} data");
    }

    public static Permission CreateDeletePermission(string resource, string? description = null)
    {
        return new Permission(
            $"{resource}.Delete",
            $"Delete {resource}",
            resource,
            "Delete",
            AccessLevel.Delete,
            description: description ?? $"Allows deleting {resource} data");
    }

    public static Permission CreateAdminPermission(string resource, string? description = null)
    {
        return new Permission(
            $"{resource}.Admin",
            $"Administer {resource}",
            resource,
            "Admin",
            AccessLevel.Admin,
            isSystemPermission: true,
            description: description ?? $"Full administrative access to {resource}");
    }

    public static Permission CreateExecutePermission(string resource, string action, string? description = null)
    {
        return new Permission(
            $"{resource}.{action}",
            $"{action} {resource}",
            resource,
            action,
            AccessLevel.Execute,
            description: description ?? $"Allows executing {action} on {resource}");
    }

    // Business Methods
    public bool Allows(string resource, string action)
    {
        if (Resource != "*" && Resource != resource)
            return false;

        if (Action != "*" && Action != action)
            return false;

        return true;
    }

    public bool IsMoreRestrictiveThan(Permission other)
    {
        if (other == null)
            return false;

        // Same resource and action
        if (Resource == other.Resource && Action == other.Action)
        {
            return Level < other.Level;
        }

        // Different resources or actions - compare by level
        return Level < other.Level;
    }

    public Permission CombineWith(Permission other)
    {
        if (other == null)
            return this;

        if (Resource != other.Resource || Action != other.Action)
            throw new ArgumentException("Cannot combine permissions for different resources or actions");

        // Take the higher access level
        var combinedLevel = (AccessLevel)Math.Max((int)Level, (int)other.Level);
        
        // Combine conditions
        var combinedConditions = Conditions.Union(other.Conditions).ToList();
        
        // System permission if either is system
        var isSystem = IsSystemPermission || other.IsSystemPermission;

        return new Permission(
            Code,
            Name,
            Resource,
            Action,
            combinedLevel,
            combinedConditions,
            isSystem,
            Description);
    }

    public bool HasCondition(string condition)
    {
        return Conditions.Contains(condition);
    }

    public bool CanRead()
    {
        return Level >= AccessLevel.Read;
    }

    public bool CanWrite()
    {
        return Level >= AccessLevel.Write;
    }

    public bool CanExecute()
    {
        return Level >= AccessLevel.Execute;
    }

    public bool CanDelete()
    {
        return Level >= AccessLevel.Delete;
    }

    public bool IsAdmin()
    {
        return Level == AccessLevel.Admin;
    }

    public string GetDisplayName()
    {
        return $"{Name} ({Resource}.{Action})";
    }

    public Dictionary<string, object> ToMetadata()
    {
        return new Dictionary<string, object>
        {
            ["Code"] = Code,
            ["Name"] = Name,
            ["Resource"] = Resource,
            ["Action"] = Action,
            ["Level"] = Level.ToString(),
            ["IsSystemPermission"] = IsSystemPermission,
            ["Conditions"] = Conditions,
            ["Description"] = Description
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
        yield return Resource;
        yield return Action;
        yield return Level;
        yield return IsSystemPermission;
        
        foreach (var condition in Conditions.OrderBy(c => c))
        {
            yield return condition;
        }
    }

    public override string ToString()
    {
        var conditions = Conditions.Any() ? $" [{string.Join(", ", Conditions)}]" : "";
        return $"{Code}: {Resource}.{Action} ({Level}){conditions}";
    }
}

// Common Banking Permissions Factory
public static class BankingPermissions
{
    // Account Management
    public static Permission AccountRead => Permission.CreateReadPermission("Account", "View account details and balances");
    public static Permission AccountWrite => Permission.CreateWritePermission("Account", "Create and update accounts");
    public static Permission AccountDelete => Permission.CreateDeletePermission("Account", "Close and delete accounts");
    public static Permission AccountFreeze => Permission.CreateExecutePermission("Account", "Freeze", "Freeze and unfreeze accounts");
    public static Permission AccountAdmin => Permission.CreateAdminPermission("Account", "Full account management");

    // Transaction Management
    public static Permission TransactionRead => Permission.CreateReadPermission("Transaction", "View transaction history");
    public static Permission TransactionWrite => Permission.CreateWritePermission("Transaction", "Process transactions");
    public static Permission TransactionReverse => Permission.CreateExecutePermission("Transaction", "Reverse", "Reverse transactions");
    public static Permission TransactionAdmin => Permission.CreateAdminPermission("Transaction", "Full transaction management");

    // Loan Management
    public static Permission LoanRead => Permission.CreateReadPermission("Loan", "View loan details and schedules");
    public static Permission LoanWrite => Permission.CreateWritePermission("Loan", "Create and update loans");
    public static Permission LoanApprove => Permission.CreateExecutePermission("Loan", "Approve", "Approve loan applications");
    public static Permission LoanDisburse => Permission.CreateExecutePermission("Loan", "Disburse", "Disburse approved loans");
    public static Permission LoanAdmin => Permission.CreateAdminPermission("Loan", "Full loan management");

    // Customer Management
    public static Permission CustomerRead => Permission.CreateReadPermission("Customer", "View customer information");
    public static Permission CustomerWrite => Permission.CreateWritePermission("Customer", "Create and update customers");
    public static Permission CustomerKYC => Permission.CreateExecutePermission("Customer", "KYC", "Perform KYC verification");
    public static Permission CustomerAdmin => Permission.CreateAdminPermission("Customer", "Full customer management");

    // Card Management
    public static Permission CardRead => Permission.CreateReadPermission("Card", "View card details");
    public static Permission CardWrite => Permission.CreateWritePermission("Card", "Issue and update cards");
    public static Permission CardBlock => Permission.CreateExecutePermission("Card", "Block", "Block and unblock cards");
    public static Permission CardAdmin => Permission.CreateAdminPermission("Card", "Full card management");

    // System Administration
    public static Permission UserRead => Permission.CreateReadPermission("User", "View user accounts");
    public static Permission UserWrite => Permission.CreateWritePermission("User", "Create and update users");
    public static Permission UserAdmin => Permission.CreateAdminPermission("User", "Full user management");
    public static Permission RoleAdmin => Permission.CreateAdminPermission("Role", "Full role management");
    public static Permission SystemAdmin => Permission.CreateAdminPermission("System", "Full system administration");

    // Reporting
    public static Permission ReportRead => Permission.CreateReadPermission("Report", "View reports");
    public static Permission ReportGenerate => Permission.CreateExecutePermission("Report", "Generate", "Generate reports");
    public static Permission ReportAdmin => Permission.CreateAdminPermission("Report", "Full report management");

    // Audit
    public static Permission AuditRead => Permission.CreateReadPermission("Audit", "View audit logs");
    public static Permission AuditAdmin => Permission.CreateAdminPermission("Audit", "Full audit management");

    // Get all banking permissions
    public static List<Permission> GetAllPermissions()
    {
        return new List<Permission>
        {
            // Account
            AccountRead, AccountWrite, AccountDelete, AccountFreeze, AccountAdmin,
            
            // Transaction
            TransactionRead, TransactionWrite, TransactionReverse, TransactionAdmin,
            
            // Loan
            LoanRead, LoanWrite, LoanApprove, LoanDisburse, LoanAdmin,
            
            // Customer
            CustomerRead, CustomerWrite, CustomerKYC, CustomerAdmin,
            
            // Card
            CardRead, CardWrite, CardBlock, CardAdmin,
            
            // System
            UserRead, UserWrite, UserAdmin, RoleAdmin, SystemAdmin,
            
            // Reporting
            ReportRead, ReportGenerate, ReportAdmin,
            
            // Audit
            AuditRead, AuditAdmin
        };
    }

    // Get permissions by category
    public static List<Permission> GetAccountPermissions()
    {
        return new List<Permission> { AccountRead, AccountWrite, AccountDelete, AccountFreeze, AccountAdmin };
    }

    public static List<Permission> GetTransactionPermissions()
    {
        return new List<Permission> { TransactionRead, TransactionWrite, TransactionReverse, TransactionAdmin };
    }

    public static List<Permission> GetLoanPermissions()
    {
        return new List<Permission> { LoanRead, LoanWrite, LoanApprove, LoanDisburse, LoanAdmin };
    }

    public static List<Permission> GetCustomerPermissions()
    {
        return new List<Permission> { CustomerRead, CustomerWrite, CustomerKYC, CustomerAdmin };
    }

    public static List<Permission> GetCardPermissions()
    {
        return new List<Permission> { CardRead, CardWrite, CardBlock, CardAdmin };
    }

    public static List<Permission> GetSystemPermissions()
    {
        return new List<Permission> { UserRead, UserWrite, UserAdmin, RoleAdmin, SystemAdmin };
    }

    public static List<Permission> GetReportingPermissions()
    {
        return new List<Permission> { ReportRead, ReportGenerate, ReportAdmin };
    }

    public static List<Permission> GetAuditPermissions()
    {
        return new List<Permission> { AuditRead, AuditAdmin };
    }
}