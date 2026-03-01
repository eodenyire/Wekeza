using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Admin Panel Controller - Serves the administrative web interface
/// Provides role-based access to different banking operations
/// </summary>
[ApiController]
[Route("admin")]
[Authorize]
public class AdminPanelController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AdminPanelController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Get admin dashboard statistics with real system metrics
    /// </summary>
    [HttpGet("dashboard/stats")]
    [Authorize(Roles = "Administrator,ITAdministrator,BranchManager")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Get active users count
            var activeUsers = await ExecuteScalarAsync<int>(
                connection,
                @"SELECT COUNT(*) FROM ""Users"" WHERE ""IsActive"" = true");

            // Get total customers count  
            var totalCustomers = await ExecuteScalarAsync<int>(
                connection,
                @"SELECT COUNT(*) FROM ""Customers"" WHERE ""IsActive"" = true");

            // Get total accounts count
            var totalAccounts = await ExecuteScalarAsync<int>(
                connection,
                @"SELECT COUNT(*) FROM ""Accounts"" WHERE ""Status"" = 'Active'");

            // Get total balance across all accounts
            var totalBalance = await ExecuteScalarAsync<decimal>(
                connection,
                @"SELECT COALESCE(SUM(""Balance""), 0) FROM ""Accounts"" WHERE ""Status"" = 'Active'");

            // Get transactions today
            var transactionsToday = await ExecuteScalarAsync<int>(
                connection,
                @"SELECT COUNT(*) FROM ""Transactions"" WHERE DATE(""CreatedAt"") = CURRENT_DATE");

            // Get pending approvals (mock for now - replace with actual approval table query)
            var pendingApprovals = 0;

            // Calculate system health percentage (based on successful transactions ratio)
            var failedTransactionsToday = await ExecuteScalarAsync<int>(
                connection,
                @"SELECT COUNT(*) FROM ""Transactions"" WHERE DATE(""CreatedAt"") = CURRENT_DATE AND ""Status"" = 'Failed'");
            
            var systemHealth = transactionsToday > 0 
                ? Math.Round(((decimal)(transactionsToday - failedTransactionsToday) / transactionsToday) * 100, 1)
                : 99.9m;

            // Get recent incidents/errors from audit logs (simplified)
            var incidents = new List<object>();
            try
            {
                var incidentQuery = @"
                    SELECT 
                        ""Id"",
                        ""Action"",
                        ""Details"",
                        ""CreatedAt""
                    FROM ""AuditLogs""
                    WHERE ""Action"" LIKE '%Error%' OR ""Action"" LIKE '%Failed%'
                    ORDER BY ""CreatedAt"" DESC
                    LIMIT 5";

                await using var command = new NpgsqlCommand(incidentQuery, connection);
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    incidents.Add(new
                    {
                        Id = reader.GetGuid(0).ToString().Substring(0, 8),
                        Severity = "Medium",
                        Service = reader.IsDBNull(1) ? "System" : reader.GetString(1),
                        Status = "Open",
                        Timestamp = reader.GetDateTime(3)
                    });
                }
            }
            catch
            {
                // If AuditLogs table doesn't exist, use mock data
                incidents.Add(new { Id = "SYS-001", Severity = "Low", Service = "System", Status = "Resolved", Timestamp = DateTime.UtcNow });
            }

            return Ok(new
            {
                activeUsers = activeUsers,
                totalCustomers = totalCustomers,
                totalAccounts = totalAccounts,
                totalBalance = totalBalance,
                transactionsToday = transactionsToday,
                pendingApprovals = pendingApprovals,
                systemHealth = systemHealth,
                incidents = incidents,
                lastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve dashboard stats", details = ex.Message });
        }
    }
    
    /// <summary>
    /// Main admin panel dashboard
    /// </summary>
    [HttpGet("")]
    public IActionResult Dashboard()
    {
        var userRoles = GetUserRoles();
        var dashboardData = GetDashboardData(userRoles);
        
        return Ok(new
        {
            Title = "Wekeza Bank - Administrator Panel",
            User = GetCurrentUserInfo(),
            Roles = userRoles,
            Dashboard = dashboardData,
            Navigation = GetNavigationMenu(userRoles)
        });
    }

    /// <summary>
    /// Get user management interface
    /// </summary>
    [HttpGet("users")]
    [Authorize(Roles = "Administrator,ITAdministrator,BranchManager")]
    public IActionResult UserManagement()
    {
        return Ok(new
        {
            Title = "User Management",
            AvailableRoles = GetAvailableRoles(),
            Permissions = GetUserPermissions()
        });
    }

    /// <summary>
    /// Get staff creation interface
    /// </summary>
    [HttpGet("staff/create")]
    [Authorize(Roles = "Administrator,ITAdministrator,BranchManager")]
    public IActionResult CreateStaff()
    {
        return Ok(new
        {
            Title = "Create Banking Staff",
            StaffTypes = GetStaffTypes(),
            Branches = GetBranches(),
            Departments = GetDepartments()
        });
    }
    /// <summary>
    /// Get teller operations interface
    /// </summary>
    [HttpGet("teller")]
    [Authorize(Roles = "Teller,Supervisor,BranchManager,Administrator")]
    public IActionResult TellerOperations()
    {
        return Ok(new
        {
            Title = "Teller Operations",
            Operations = new[]
            {
                "Cash Deposits",
                "Cash Withdrawals", 
                "Cheque Deposits",
                "Fund Transfers",
                "Account Balance Inquiry",
                "Account Statement Generation"
            }
        });
    }

    /// <summary>
    /// Get customer care interface
    /// </summary>
    [HttpGet("customer-care")]
    [Authorize(Roles = "CustomerService,Supervisor,BranchManager,Administrator")]
    public IActionResult CustomerCare()
    {
        return Ok(new
        {
            Title = "Customer Care Operations",
            Operations = new[]
            {
                "Customer Onboarding",
                "Balance Inquiries",
                "Account Status Updates",
                "Customer Support Tickets",
                "Account Maintenance"
            }
        });
    }

    /// <summary>
    /// Get back office operations interface
    /// </summary>
    [HttpGet("back-office")]
    [Authorize(Roles = "BackOfficeStaff,Supervisor,BranchManager,Administrator")]
    public IActionResult BackOfficeOperations()
    {
        return Ok(new
        {
            Title = "Back Office Operations",
            Operations = new[]
            {
                "Transaction Processing",
                "Account Reconciliation",
                "Batch Processing",
                "Report Generation",
                "Data Management"
            }
        });
    }

    /// <summary>
    /// Get treasury operations interface
    /// </summary>
    [HttpGet("treasury")]
    [Authorize(Roles = "CashOfficer,BranchManager,Administrator")]
    public IActionResult TreasuryOperations()
    {
        return Ok(new
        {
            Title = "Treasury Operations",
            Operations = new[]
            {
                "CSDC Account Management",
                "Foreign Exchange",
                "Money Market Operations",
                "Liquidity Management",
                "Investment Management"
            }
        });
    }
    /// <summary>
    /// Get loan management interface
    /// </summary>
    [HttpGet("loans")]
    [Authorize(Roles = "LoanOfficer,Supervisor,BranchManager,Administrator")]
    public IActionResult LoanManagement()
    {
        return Ok(new
        {
            Title = "Loan Management",
            Operations = new[]
            {
                "Loan Applications",
                "Credit Assessment",
                "Loan Disbursement",
                "Repayment Processing",
                "Loan Portfolio Management"
            }
        });
    }

    /// <summary>
    /// Get compliance interface
    /// </summary>
    [HttpGet("compliance")]
    [Authorize(Roles = "ComplianceOfficer,RiskOfficer,Administrator")]
    public IActionResult ComplianceOperations()
    {
        return Ok(new
        {
            Title = "Compliance & Risk Management",
            Operations = new[]
            {
                "AML Monitoring",
                "Sanctions Screening",
                "Risk Assessment",
                "Regulatory Reporting",
                "Audit Management"
            }
        });
    }

    /// <summary>
    /// Get product management interface
    /// </summary>
    [HttpGet("products")]
    [Authorize(Roles = "Administrator,BranchManager,Supervisor")]
    public IActionResult ProductManagement()
    {
        return Ok(new
        {
            Title = "Product Management",
            Operations = new[]
            {
                "Create Banking Products",
                "Manage Interest Rates",
                "Configure Fees",
                "Product Lifecycle Management",
                "Pricing Management"
            }
        });
    }

    #region Helper Methods

    private List<string> GetUserRoles()
    {
        return User.FindAll(System.Security.Claims.ClaimTypes.Role)
                  .Select(c => c.Value)
                  .ToList();
    }

    private object GetCurrentUserInfo()
    {
        return new
        {
            UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            Roles = GetUserRoles()
        };
    }
    private object GetDashboardData(List<string> userRoles)
    {
        var dashboardItems = new List<object>();

        // Admin-only items
        if (userRoles.Contains("Administrator") || userRoles.Contains("ITAdministrator"))
        {
            dashboardItems.AddRange(new[]
            {
                new { Title = "User Management", Icon = "users", Url = "/admin/users", Description = "Manage banking staff and users" },
                new { Title = "System Configuration", Icon = "settings", Url = "/admin/system", Description = "Configure system parameters" },
                new { Title = "Audit Logs", Icon = "file-text", Url = "/admin/audit", Description = "View system audit logs" }
            });
        }

        // Branch Manager items
        if (userRoles.Contains("BranchManager") || userRoles.Contains("Administrator"))
        {
            dashboardItems.AddRange(new[]
            {
                new { Title = "Branch Operations", Icon = "building", Url = "/admin/branch", Description = "Manage branch operations" },
                new { Title = "Staff Management", Icon = "user-plus", Url = "/admin/staff", Description = "Manage branch staff" }
            });
        }

        // Teller items
        if (userRoles.Contains("Teller") || userRoles.Contains("Supervisor") || userRoles.Contains("BranchManager") || userRoles.Contains("Administrator"))
        {
            dashboardItems.Add(new { Title = "Teller Operations", Icon = "credit-card", Url = "/admin/teller", Description = "Process customer transactions" });
        }

        // Customer Service items
        if (userRoles.Contains("CustomerService") || userRoles.Contains("Supervisor") || userRoles.Contains("BranchManager") || userRoles.Contains("Administrator"))
        {
            dashboardItems.Add(new { Title = "Customer Care", Icon = "headphones", Url = "/admin/customer-care", Description = "Customer support operations" });
        }

        // Back Office items
        if (userRoles.Contains("BackOfficeStaff") || userRoles.Contains("Supervisor") || userRoles.Contains("BranchManager") || userRoles.Contains("Administrator"))
        {
            dashboardItems.Add(new { Title = "Back Office", Icon = "database", Url = "/admin/back-office", Description = "Back office operations" });
        }

        // Treasury items
        if (userRoles.Contains("CashOfficer") || userRoles.Contains("BranchManager") || userRoles.Contains("Administrator"))
        {
            dashboardItems.Add(new { Title = "Treasury", Icon = "dollar-sign", Url = "/admin/treasury", Description = "Treasury and cash management" });
        }

        // Loan Officer items
        if (userRoles.Contains("LoanOfficer") || userRoles.Contains("Supervisor") || userRoles.Contains("BranchManager") || userRoles.Contains("Administrator"))
        {
            dashboardItems.Add(new { Title = "Loan Management", Icon = "file-text", Url = "/admin/loans", Description = "Loan processing and management" });
        }

        // Compliance items
        if (userRoles.Contains("ComplianceOfficer") || userRoles.Contains("RiskOfficer") || userRoles.Contains("Administrator"))
        {
            dashboardItems.Add(new { Title = "Compliance", Icon = "shield", Url = "/admin/compliance", Description = "Risk and compliance management" });
        }

        return dashboardItems;
    }
    private object GetNavigationMenu(List<string> userRoles)
    {
        var menuItems = new List<object>();

        // Always available
        menuItems.Add(new { Title = "Dashboard", Icon = "home", Url = "/admin", Active = true });

        // Admin navigation
        if (userRoles.Contains("Administrator") || userRoles.Contains("ITAdministrator"))
        {
            menuItems.AddRange(new[]
            {
                new { Title = "User Management", Icon = "users", Url = "/admin/users", Active = false },
                new { Title = "System Config", Icon = "settings", Url = "/admin/system", Active = false },
                new { Title = "Audit Logs", Icon = "file-text", Url = "/admin/audit", Active = false }
            });
        }

        // Operations navigation
        if (userRoles.Any(r => new[] { "Teller", "CustomerService", "BackOfficeStaff", "LoanOfficer", "Supervisor", "BranchManager", "Administrator" }.Contains(r)))
        {
            var operationsMenu = new List<object>();
            
            if (userRoles.Any(r => new[] { "Teller", "Supervisor", "BranchManager", "Administrator" }.Contains(r)))
                operationsMenu.Add(new { Title = "Teller Operations", Url = "/admin/teller" });
                
            if (userRoles.Any(r => new[] { "CustomerService", "Supervisor", "BranchManager", "Administrator" }.Contains(r)))
                operationsMenu.Add(new { Title = "Customer Care", Url = "/admin/customer-care" });
                
            if (userRoles.Any(r => new[] { "BackOfficeStaff", "Supervisor", "BranchManager", "Administrator" }.Contains(r)))
                operationsMenu.Add(new { Title = "Back Office", Url = "/admin/back-office" });
                
            if (userRoles.Any(r => new[] { "LoanOfficer", "Supervisor", "BranchManager", "Administrator" }.Contains(r)))
                operationsMenu.Add(new { Title = "Loans", Url = "/admin/loans" });

            menuItems.Add(new { Title = "Operations", Icon = "briefcase", Submenu = operationsMenu, Active = false });
        }

        // Management navigation
        if (userRoles.Any(r => new[] { "BranchManager", "Administrator", "CashOfficer" }.Contains(r)))
        {
            var managementMenu = new List<object>();
            
            if (userRoles.Any(r => new[] { "CashOfficer", "BranchManager", "Administrator" }.Contains(r)))
                managementMenu.Add(new { Title = "Treasury", Url = "/admin/treasury" });
                
            if (userRoles.Any(r => new[] { "BranchManager", "Administrator" }.Contains(r)))
                managementMenu.Add(new { Title = "Products", Url = "/admin/products" });

            menuItems.Add(new { Title = "Management", Icon = "trending-up", Submenu = managementMenu, Active = false });
        }

        // Compliance navigation
        if (userRoles.Any(r => new[] { "ComplianceOfficer", "RiskOfficer", "Administrator" }.Contains(r)))
        {
            menuItems.Add(new { Title = "Compliance", Icon = "shield", Url = "/admin/compliance", Active = false });
        }

        return menuItems;
    }
    private object GetAvailableRoles()
    {
        return new
        {
            CustomerFacing = new[]
            {
                new { Role = "Teller", Description = "Branch teller operations", Permissions = "Deposits, Withdrawals, Transfers" },
                new { Role = "CustomerService", Description = "Customer support", Permissions = "Account inquiries, Customer onboarding" }
            },
            BackOffice = new[]
            {
                new { Role = "BackOfficeStaff", Description = "Back office operations", Permissions = "Transaction processing, Reconciliation" },
                new { Role = "LoanOfficer", Description = "Loan processing", Permissions = "Loan applications, Credit assessment" },
                new { Role = "CashOfficer", Description = "Treasury operations", Permissions = "Cash management, CSDC operations" }
            },
            Management = new[]
            {
                new { Role = "Supervisor", Description = "Operations supervisor", Permissions = "Approve transactions, Staff oversight" },
                new { Role = "BranchManager", Description = "Branch management", Permissions = "Branch operations, Staff management" }
            },
            Specialized = new[]
            {
                new { Role = "ComplianceOfficer", Description = "Compliance management", Permissions = "AML monitoring, Risk assessment" },
                new { Role = "RiskOfficer", Description = "Risk management", Permissions = "Risk assessment, Limit management" },
                new { Role = "InsuranceOfficer", Description = "Insurance products", Permissions = "Insurance processing, Claims" }
            },
            Administrative = new[]
            {
                new { Role = "Administrator", Description = "System administrator", Permissions = "Full system access" },
                new { Role = "ITAdministrator", Description = "Technical administrator", Permissions = "System configuration, User management" }
            }
        };
    }

    private object GetUserPermissions()
    {
        var userRoles = GetUserRoles();
        var permissions = new List<string>();

        if (userRoles.Contains("Administrator") || userRoles.Contains("ITAdministrator"))
        {
            permissions.AddRange(new[] { "CREATE_USERS", "MANAGE_ROLES", "SYSTEM_CONFIG", "VIEW_AUDIT_LOGS" });
        }

        if (userRoles.Contains("BranchManager"))
        {
            permissions.AddRange(new[] { "MANAGE_BRANCH_STAFF", "APPROVE_TRANSACTIONS", "BRANCH_REPORTS" });
        }

        if (userRoles.Contains("Supervisor"))
        {
            permissions.AddRange(new[] { "APPROVE_BASIC_TRANSACTIONS", "STAFF_OVERSIGHT" });
        }

        return permissions.Distinct().ToList();
    }
    private object GetStaffTypes()
    {
        return new[]
        {
            new { 
                Category = "Customer Facing",
                Roles = new[]
                {
                    new { Role = "Teller", Description = "Process customer transactions", RequiredSkills = "Cash handling, Customer service" },
                    new { Role = "CustomerService", Description = "Customer support and onboarding", RequiredSkills = "Communication, Problem solving" }
                }
            },
            new {
                Category = "Back Office",
                Roles = new[]
                {
                    new { Role = "BackOfficeStaff", Description = "Transaction processing and reconciliation", RequiredSkills = "Data processing, Attention to detail" },
                    new { Role = "LoanOfficer", Description = "Loan processing and credit assessment", RequiredSkills = "Credit analysis, Risk assessment" },
                    new { Role = "CashOfficer", Description = "Treasury and cash management", RequiredSkills = "Cash management, Financial markets" }
                }
            },
            new {
                Category = "Management",
                Roles = new[]
                {
                    new { Role = "Supervisor", Description = "Operations supervision", RequiredSkills = "Leadership, Operations management" },
                    new { Role = "BranchManager", Description = "Branch management", RequiredSkills = "Management, Strategic planning" }
                }
            },
            new {
                Category = "Specialized",
                Roles = new[]
                {
                    new { Role = "ComplianceOfficer", Description = "Compliance and AML", RequiredSkills = "Regulatory knowledge, Risk management" },
                    new { Role = "RiskOfficer", Description = "Risk assessment", RequiredSkills = "Risk analysis, Financial modeling" },
                    new { Role = "InsuranceOfficer", Description = "Insurance products", RequiredSkills = "Insurance knowledge, Sales" }
                }
            }
        };
    }

    private object GetBranches()
    {
        // Mock data - replace with actual branch data from database
        return new[]
        {
            new { Id = 1, Name = "Main Branch", Code = "001", Location = "Downtown", Status = "Active" },
            new { Id = 2, Name = "Westlands Branch", Code = "002", Location = "Westlands", Status = "Active" },
            new { Id = 3, Name = "Eastleigh Branch", Code = "003", Location = "Eastleigh", Status = "Active" }
        };
    }

    private object GetDepartments()
    {
        return new[]
        {
            new { Id = 1, Name = "Customer Service", Description = "Customer facing operations" },
            new { Id = 2, Name = "Back Office", Description = "Transaction processing and support" },
            new { Id = 3, Name = "Loans", Description = "Credit and loan operations" },
            new { Id = 4, Name = "Treasury", Description = "Cash and treasury management" },
            new { Id = 5, Name = "Compliance", Description = "Risk and compliance management" },
            new { Id = 6, Name = "IT", Description = "Information technology support" }
        };
    }

    #region Helper Methods

    /// <summary>
    /// Execute a scalar query and return typed result
    /// </summary>
    private static async Task<T> ExecuteScalarAsync<T>(NpgsqlConnection connection, string query)
    {
        await using var command = new NpgsqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();
        return result == null || result is DBNull ? default(T)! : (T)Convert.ChangeType(result, typeof(T))!;
    }

    /// <summary>
    /// Execute a scalar query with parameters and return typed result
    /// </summary>
    private static async Task<T> ExecuteScalarAsync<T>(NpgsqlConnection connection, string query, params (string, object)[] parameters)
    {
        await using var command = new NpgsqlCommand(query, connection);
        foreach (var (paramName, paramValue) in parameters)
        {
            command.Parameters.AddWithValue(paramName, paramValue ?? DBNull.Value);
        }
        var result = await command.ExecuteScalarAsync();
        return result == null || result is DBNull ? default(T)! : (T)Convert.ChangeType(result, typeof(T))!;
    }

    #endregion

    #endregion
}