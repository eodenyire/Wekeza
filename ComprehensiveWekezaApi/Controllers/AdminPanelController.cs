using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ComprehensiveWekezaApi.Services;
using DatabaseWekezaApi.Data;
using DatabaseWekezaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ComprehensiveWekezaApi.Controllers;

/// <summary>
/// Admin Panel Controller for Comprehensive Wekeza API
/// Provides role-based access to banking operations
/// </summary>
[ApiController]
[Route("admin")]
public class AdminPanelController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly WekezaDbContext _context;

    public AdminPanelController(IStaffService staffService, WekezaDbContext context)
    {
        _staffService = staffService;
        _context = context;
    }

    /// <summary>
    /// Main admin panel dashboard - serves the HTML interface
    /// </summary>
    [HttpGet("")]
    public async Task<IActionResult> Dashboard([FromServices] IWebHostEnvironment env)
    {
        // Serve the HTML file for the admin panel interface
        var filePath = Path.Combine(env.WebRootPath, "admin", "index.html");
        if (System.IO.File.Exists(filePath))
        {
            var content = await System.IO.File.ReadAllTextAsync(filePath);
            return Content(content, "text/html");
        }
        return NotFound("Admin panel not found");
    }

    /// <summary>
    /// Dashboard API endpoint - returns dashboard data
    /// </summary>
    [HttpGet("api")]
    public IActionResult DashboardApi()
    {
        var userRoles = GetUserRoles();
        var dashboardData = GetDashboardData(userRoles);
        
        return Ok(new
        {
            Title = "Wekeza Bank - Administrator Panel (Port 5003)",
            User = GetCurrentUserInfo(),
            Roles = userRoles,
            Dashboard = dashboardData,
            Navigation = GetNavigationMenu(userRoles),
            SystemInfo = new
            {
                Version = "2.0.0-Comprehensive",
                Port = 5003,
                Modules = 18,
                Endpoints = 85,
                Database = "PostgreSQL",
                Status = "Fully Operational"
            }
        });
    }

    /// <summary>
    /// Authentication endpoint for admin panel
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] AdminLoginRequest request)
    {
        // Mock authentication - replace with actual authentication
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var userId = Guid.NewGuid();
        var roles = DetermineUserRoles(request.Username);
        var token = GenerateToken(userId, request.Username, roles);

        return Ok(new
        {
            Token = token,
            UserId = userId,
            Username = request.Username,
            Roles = roles,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            SystemAccess = GetSystemAccess(roles)
        });
    }

    /// <summary>
    /// Get user management interface
    /// </summary>
    [HttpGet("users")]
    public IActionResult UserManagement()
    {
        return Ok(new
        {
            Title = "User Management - Comprehensive Banking System",
            AvailableRoles = GetAvailableRoles(),
            Permissions = GetUserPermissions(),
            SystemModules = GetSystemModules()
        });
    }

    /// <summary>
    /// Get staff creation interface
    /// </summary>
    [HttpGet("staff/create")]
    public IActionResult CreateStaffInterface()
    {
        return Ok(new
        {
            Title = "Create Banking Staff - Comprehensive System",
            StaffTypes = GetStaffTypes(),
            Branches = GetBranches(),
            Departments = GetDepartments(),
            SystemModules = GetSystemModules()
        });
    }

    /// <summary>
    /// Create a new banking staff member - REAL DATABASE PERSISTENCE
    /// </summary>
    [HttpPost("staff/create")]
    public async Task<IActionResult> CreateStaffMember([FromBody] CreateStaffRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = GetCurrentUserInfo();
            var createdBy = "admin"; // For demo purposes

            var staff = await _staffService.CreateStaffAsync(request, createdBy);

            return Ok(new
            {
                Success = true,
                Message = "✅ Banking staff member created successfully and saved to database!",
                Data = new
                {
                    Id = staff.Id,
                    EmployeeId = staff.EmployeeId,
                    FullName = staff.FullName,
                    Email = staff.Email,
                    Phone = staff.Phone,
                    Role = staff.Role,
                    BranchName = staff.BranchName,
                    DepartmentName = staff.DepartmentName,
                    Status = staff.Status,
                    CreatedAt = staff.CreatedAt,
                    CreatedBy = staff.CreatedBy
                },
                DatabasePersistence = true,
                SystemInfo = "Data has been permanently saved to PostgreSQL database"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message,
                DatabasePersistence = false
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to create staff member",
                Error = ex.Message,
                DatabasePersistence = false
            });
        }
    }

    /// <summary>
    /// Get all staff members from database
    /// </summary>
    [HttpGet("staff")]
    public async Task<IActionResult> GetAllStaff()
    {
        try
        {
            var staff = await _staffService.GetAllStaffAsync();
            
            return Ok(new
            {
                Success = true,
                Message = "Staff data retrieved from database",
                Data = staff.Select(s => new
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    FullName = s.FullName,
                    Email = s.Email,
                    Phone = s.Phone,
                    Role = s.Role,
                    BranchName = s.BranchName,
                    DepartmentName = s.DepartmentName,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    LastLogin = s.LastLogin
                }).ToList(),
                TotalCount = staff.Count,
                DatabasePersistence = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to retrieve staff data",
                Error = ex.Message
            });
        }
    }
    /// <summary>
    /// CIF (Customer Information File) Operations
    /// </summary>
    [HttpGet("cif")]
    public IActionResult CIFOperations()
    {
        return Ok(new
        {
            Title = "CIF - Customer Information File Management",
            Operations = new[]
            {
                "Create Individual Party",
                "Create Corporate Party", 
                "Update KYC Status",
                "Customer 360 View",
                "AML Screening",
                "Risk Assessment",
                "Customer Onboarding"
            },
            Endpoints = new[]
            {
                "POST /admin/cif/individual",
                "POST /admin/cif/corporate",
                "PUT /admin/cif/kyc-status",
                "GET /admin/cif/customer360/{id}",
                "POST /admin/cif/aml-screening"
            }
        });
    }

    /// <summary>
    /// Create Individual Customer - REAL IMPLEMENTATION
    /// </summary>
    [HttpPost("cif/individual")]
    public async Task<IActionResult> CreateIndividualCustomer([FromBody] CreateIndividualCustomerRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                CustomerNumber = await GenerateCustomerNumberAsync("IND"),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                IdentificationNumber = request.IdentificationNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Nationality = request.Nationality,
                Status = "Active",
                KYCStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "✅ Individual customer created successfully!",
                Data = new
                {
                    Id = customer.Id,
                    CustomerNumber = customer.CustomerNumber,
                    FullName = $"{customer.FirstName} {customer.LastName}",
                    Email = customer.Email,
                    Phone = customer.PrimaryPhone,
                    IdentificationNumber = customer.IdentificationNumber,
                    Status = customer.Status,
                    KYCStatus = customer.KYCStatus,
                    CreatedAt = customer.CreatedAt
                },
                DatabasePersistence = true,
                SystemInfo = "Customer data saved to PostgreSQL database"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to create customer",
                Error = ex.Message,
                DatabasePersistence = false
            });
        }
    }

    /// <summary>
    /// Create Corporate Customer - REAL IMPLEMENTATION
    /// </summary>
    [HttpPost("cif/corporate")]
    public async Task<IActionResult> CreateCorporateCustomer([FromBody] CreateCorporateCustomerRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var business = new Business
            {
                Id = Guid.NewGuid(),
                BusinessName = request.BusinessName,
                CompanyName = request.CompanyName,
                RegistrationNumber = request.RegistrationNumber,
                IncorporationDate = request.IncorporationDate,
                CompanyType = request.CompanyType,
                Industry = request.Industry,
                Email = request.Email,
                Phone = request.Phone,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "✅ Corporate customer created successfully!",
                Data = new
                {
                    Id = business.Id,
                    BusinessName = business.BusinessName,
                    CompanyName = business.CompanyName,
                    RegistrationNumber = business.RegistrationNumber,
                    Industry = business.Industry,
                    Email = business.Email,
                    Phone = business.Phone,
                    Status = business.Status,
                    CreatedAt = business.CreatedAt
                },
                DatabasePersistence = true,
                SystemInfo = "Corporate customer data saved to PostgreSQL database"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to create corporate customer",
                Error = ex.Message,
                DatabasePersistence = false
            });
        }
    }

    /// <summary>
    /// Get All Customers - REAL IMPLEMENTATION
    /// </summary>
    [HttpGet("cif/customers")]
    public async Task<IActionResult> GetAllCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var customers = await _context.Customers
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Customers.CountAsync();

            return Ok(new
            {
                Success = true,
                Data = customers.Select(c => new
                {
                    Id = c.Id,
                    CustomerNumber = c.CustomerNumber,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Email = c.Email,
                    Phone = c.PrimaryPhone,
                    Status = c.Status,
                    KYCStatus = c.KYCStatus,
                    CreatedAt = c.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                DatabasePersistence = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to retrieve customers",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Account Management Operations
    /// </summary>
    [HttpGet("accounts")]
    public IActionResult AccountOperations()
    {
        return Ok(new
        {
            Title = "Account Management Operations",
            Operations = new[]
            {
                "Open Product-Based Account",
                "Register Business Account",
                "Freeze/Unfreeze Account",
                "Close Account",
                "Add Signatories",
                "Account Maintenance"
            },
            Endpoints = new[]
            {
                "POST /admin/accounts/product-based",
                "POST /admin/accounts/business",
                "PUT /admin/accounts/{id}/freeze",
                "PUT /admin/accounts/{id}/close",
                "POST /admin/accounts/{id}/signatories"
            }
        });
    }

    /// <summary>
    /// Open Product-Based Account - REAL IMPLEMENTATION
    /// </summary>
    [HttpPost("accounts/product-based")]
    public async Task<IActionResult> OpenProductBasedAccount([FromBody] OpenAccountRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify customer exists
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
                return BadRequest("Customer not found");

            var account = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = await GenerateAccountNumberAsync(),
                CustomerId = request.CustomerId,
                BusinessId = request.BusinessId,
                AccountType = request.AccountType,
                Currency = request.Currency,
                Balance = request.InitialDeposit,
                AvailableBalance = request.InitialDeposit,
                Status = "Active",
                InterestRate = GetInterestRateForAccountType(request.AccountType),
                MinimumBalance = GetMinimumBalanceForAccountType(request.AccountType),
                CreatedAt = DateTime.UtcNow
            };

            _context.Accounts.Add(account);

            // Create initial deposit transaction if amount > 0
            if (request.InitialDeposit > 0)
            {
                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    Reference = await GenerateTransactionReferenceAsync("DEP"),
                    Type = "Deposit",
                    Amount = request.InitialDeposit,
                    Currency = request.Currency,
                    Description = "Initial deposit - Account opening",
                    Status = "Completed",
                    ProcessedAt = DateTime.UtcNow,
                    ProcessedBy = "admin" // Since GetCurrentUserInfo returns anonymous object
                };

                _context.Transactions.Add(transaction);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "✅ Account opened successfully!",
                Data = new
                {
                    Id = account.Id,
                    AccountNumber = account.AccountNumber,
                    CustomerId = account.CustomerId,
                    CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
                    AccountType = account.AccountType,
                    Currency = account.Currency,
                    Balance = account.Balance,
                    AvailableBalance = account.AvailableBalance,
                    Status = account.Status,
                    InterestRate = account.InterestRate,
                    MinimumBalance = account.MinimumBalance,
                    CreatedAt = account.CreatedAt
                },
                DatabasePersistence = true,
                SystemInfo = "Account data saved to PostgreSQL database"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to open account",
                Error = ex.Message,
                DatabasePersistence = false
            });
        }
    }

    /// <summary>
    /// Get All Accounts - REAL IMPLEMENTATION
    /// </summary>
    [HttpGet("accounts/list")]
    public async Task<IActionResult> GetAllAccounts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var accounts = await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Business)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await _context.Accounts.CountAsync();

            return Ok(new
            {
                Success = true,
                Data = accounts.Select(a => new
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    CustomerName = a.Customer != null ? $"{a.Customer.FirstName} {a.Customer.LastName}" : a.Business?.BusinessName ?? "Unknown",
                    AccountType = a.AccountType,
                    Currency = a.Currency,
                    Balance = a.Balance,
                    AvailableBalance = a.AvailableBalance,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                DatabasePersistence = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to retrieve accounts",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Freeze/Unfreeze Account - REAL IMPLEMENTATION
    /// </summary>
    [HttpPut("accounts/{accountId:guid}/freeze")]
    public async Task<IActionResult> FreezeAccount(Guid accountId, [FromBody] FreezeAccountRequest request)
    {
        try
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
                return NotFound("Account not found");

            account.Status = request.Freeze ? "Frozen" : "Active";
            account.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = $"✅ Account {(request.Freeze ? "frozen" : "unfrozen")} successfully!",
                Data = new
                {
                    AccountId = accountId,
                    AccountNumber = account.AccountNumber,
                    Status = account.Status,
                    Reason = request.Reason,
                    UpdatedAt = account.UpdatedAt,
                    UpdatedBy = "admin" // Since Account model doesn't have UpdatedBy field
                },
                DatabasePersistence = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to update account status",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Transaction Processing Operations
    /// </summary>
    [HttpGet("transactions")]
    public IActionResult TransactionOperations()
    {
        return Ok(new
        {
            Title = "Transaction Processing Operations",
            Operations = new[]
            {
                "Deposit Funds",
                "Withdraw Funds",
                "Transfer Funds",
                "Cheque Deposits",
                "Account Statements",
                "Transaction History"
            },
            Endpoints = new[]
            {
                "POST /api/transactions/deposit",
                "POST /api/transactions/withdraw", 
                "POST /api/transactions/transfer",
                "POST /api/transactions/cheque",
                "GET /api/transactions/statement"
            }
        });
    }

    /// <summary>
    /// Loan Management Operations
    /// </summary>
    [HttpGet("loans")]
    public IActionResult LoanOperations()
    {
        return Ok(new
        {
            Title = "Loan Management Operations",
            Operations = new[]
            {
                "Loan Applications",
                "Loan Approval",
                "Loan Disbursement",
                "Repayment Processing",
                "Repayment Schedule",
                "Loan Portfolio Management"
            },
            Endpoints = new[]
            {
                "POST /api/loans/apply",
                "PUT /api/loans/{id}/approve",
                "POST /api/loans/{id}/disburse",
                "POST /api/loans/{id}/repayment",
                "GET /api/loans/{id}/schedule"
            }
        });
    }

    /// <summary>
    /// Fixed Deposits & Investments Operations
    /// </summary>
    [HttpGet("deposits")]
    public IActionResult DepositOperations()
    {
        return Ok(new
        {
            Title = "Fixed Deposits & Investment Operations",
            Operations = new[]
            {
                "Book Fixed Deposit",
                "Create Call Deposit",
                "Setup Recurring Deposit",
                "Interest Accrual Processing",
                "Maturity Management"
            },
            Endpoints = new[]
            {
                "POST /api/deposits/fixed",
                "POST /api/deposits/call",
                "POST /api/deposits/recurring",
                "POST /api/deposits/interest-accrual",
                "GET /api/deposits/{id}/maturity"
            }
        });
    }

    /// <summary>
    /// Teller Operations
    /// </summary>
    [HttpGet("teller")]
    public IActionResult TellerOperations()
    {
        return Ok(new
        {
            Title = "Teller Operations - Branch Banking",
            Operations = new[]
            {
                "Start Teller Session",
                "Cash Deposits",
                "Cash Withdrawals",
                "End Teller Session",
                "Cash Position Management",
                "Daily Reconciliation"
            },
            Endpoints = new[]
            {
                "POST /api/teller/session/start",
                "POST /api/teller/cash-deposit",
                "POST /api/teller/cash-withdrawal",
                "PUT /api/teller/session/end",
                "GET /api/teller/cash-position"
            }
        });
    }
    /// <summary>
    /// Branch Operations
    /// </summary>
    [HttpGet("branches")]
    public IActionResult BranchOperations()
    {
        return Ok(new
        {
            Title = "Branch Operations Management",
            Operations = new[]
            {
                "Create Branch",
                "Branch Performance",
                "Cash Drawer Management",
                "Branch Transactions",
                "Multi-Branch Hierarchy"
            },
            Endpoints = new[]
            {
                "POST /api/branches",
                "GET /api/branches",
                "GET /api/branches/{id}/performance",
                "POST /api/branches/{id}/cash-drawer",
                "GET /api/branches/{id}/transactions"
            }
        });
    }

    /// <summary>
    /// Cards & Instruments Operations
    /// </summary>
    [HttpGet("cards")]
    public IActionResult CardOperations()
    {
        return Ok(new
        {
            Title = "Cards & Instruments Management",
            Operations = new[]
            {
                "Issue Cards (Debit/Credit/Prepaid)",
                "ATM Transaction Processing",
                "POS Transaction Processing",
                "Update Card Limits",
                "Card Status Management"
            },
            Endpoints = new[]
            {
                "POST /api/cards/issue",
                "POST /api/cards/atm-transaction",
                "POST /api/cards/pos-transaction",
                "PUT /api/cards/{id}/limits",
                "PUT /api/cards/{id}/status"
            }
        });
    }

    /// <summary>
    /// General Ledger Operations
    /// </summary>
    [HttpGet("general-ledger")]
    public IActionResult GeneralLedgerOperations()
    {
        return Ok(new
        {
            Title = "General Ledger - Financial Accounting",
            Operations = new[]
            {
                "Create GL Accounts",
                "Post Journal Entries",
                "Chart of Accounts",
                "Trial Balance",
                "Balance Sheet Generation"
            },
            Endpoints = new[]
            {
                "POST /api/gl/accounts",
                "POST /api/gl/journal-entry",
                "GET /api/gl/chart-of-accounts",
                "GET /api/gl/trial-balance",
                "GET /api/gl/balance-sheet"
            }
        });
    }

    /// <summary>
    /// Treasury & Markets Operations
    /// </summary>
    [HttpGet("treasury")]
    public IActionResult TreasuryOperations()
    {
        return Ok(new
        {
            Title = "Treasury & Markets Operations",
            Operations = new[]
            {
                "FX Deal Creation",
                "Money Market Deals",
                "Securities Trading",
                "Treasury Positions",
                "Risk Metrics Management"
            },
            Endpoints = new[]
            {
                "POST /api/treasury/fx-deal",
                "POST /api/treasury/money-market",
                "POST /api/treasury/securities",
                "GET /api/treasury/positions",
                "GET /api/treasury/risk-metrics"
            }
        });
    }

    /// <summary>
    /// Trade Finance Operations
    /// </summary>
    [HttpGet("trade-finance")]
    public IActionResult TradeFinanceOperations()
    {
        return Ok(new
        {
            Title = "Trade Finance Operations",
            Operations = new[]
            {
                "Issue Letter of Credit",
                "Issue Bank Guarantee",
                "Documentary Collection",
                "LC Amendment",
                "Trade Finance Portfolio"
            },
            Endpoints = new[]
            {
                "POST /api/trade-finance/letter-of-credit",
                "POST /api/trade-finance/bank-guarantee",
                "POST /api/trade-finance/documentary-collection",
                "GET /api/trade-finance/lc/{id}/details",
                "PUT /api/trade-finance/lc/{id}/amend"
            }
        });
    }

    /// <summary>
    /// Payment Processing Operations
    /// </summary>
    [HttpGet("payments")]
    public IActionResult PaymentOperations()
    {
        return Ok(new
        {
            Title = "Payment Processing Operations",
            Operations = new[]
            {
                "Create Payment Orders",
                "RTGS Payments",
                "SWIFT Payments",
                "Payment Status Tracking",
                "Payment Cancellation"
            },
            Endpoints = new[]
            {
                "POST /api/payments/create",
                "POST /api/payments/rtgs",
                "POST /api/payments/swift",
                "GET /api/payments/{id}/status",
                "POST /api/payments/{id}/cancel"
            }
        });
    }
    /// <summary>
    /// Compliance & Risk Operations
    /// </summary>
    [HttpGet("compliance")]
    public IActionResult ComplianceOperations()
    {
        return Ok(new
        {
            Title = "Compliance & Risk Management",
            Operations = new[]
            {
                "Transaction Screening",
                "AML Case Management",
                "Sanctions Screening",
                "Risk Assessment",
                "Suspicious Activity Reporting"
            },
            Endpoints = new[]
            {
                "POST /api/compliance/screen-transaction",
                "POST /api/compliance/aml-case",
                "GET /api/compliance/sanctions-screening",
                "GET /api/compliance/risk-assessment",
                "POST /api/compliance/suspicious-activity"
            }
        });
    }

    /// <summary>
    /// Reporting & Analytics Operations
    /// </summary>
    [HttpGet("reporting")]
    public IActionResult ReportingOperations()
    {
        return Ok(new
        {
            Title = "Reporting & Analytics",
            Operations = new[]
            {
                "Regulatory Reports",
                "MIS Reports",
                "Executive Dashboard",
                "Business Analytics",
                "Custom Report Builder"
            },
            Endpoints = new[]
            {
                "POST /api/reports/regulatory",
                "GET /api/reports/mis",
                "GET /api/reports/dashboard",
                "GET /api/reports/analytics",
                "POST /api/reports/custom"
            }
        });
    }

    /// <summary>
    /// Workflow Engine Operations
    /// </summary>
    [HttpGet("workflows")]
    public IActionResult WorkflowOperations()
    {
        return Ok(new
        {
            Title = "Workflow Engine - Maker-Checker",
            Operations = new[]
            {
                "Initiate Workflow",
                "Approve Workflow",
                "Reject Workflow",
                "Pending Approvals",
                "Approval Matrix Setup"
            },
            Endpoints = new[]
            {
                "POST /api/workflows/initiate",
                "PUT /api/workflows/{id}/approve",
                "PUT /api/workflows/{id}/reject",
                "GET /api/workflows/pending",
                "POST /api/workflows/approval-matrix"
            }
        });
    }

    /// <summary>
    /// Product Factory Operations
    /// </summary>
    [HttpGet("products")]
    public IActionResult ProductOperations()
    {
        return Ok(new
        {
            Title = "Product Factory - Banking Products",
            Operations = new[]
            {
                "Create Products",
                "Product Management",
                "Activate Products",
                "Update Pricing",
                "Product Analytics"
            },
            Endpoints = new[]
            {
                "POST /api/products/create",
                "GET /api/products",
                "PUT /api/products/{id}/activate",
                "PUT /api/products/{id}/pricing",
                "GET /api/products/{id}/details"
            }
        });
    }

    #region Helper Methods

    private List<string> GetUserRoles()
    {
        // Mock roles - in real implementation, get from JWT token or session
        return new List<string> { "Administrator", "BranchManager" };
    }

    private object GetCurrentUserInfo()
    {
        return new
        {
            UserId = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@wekeza.com",
            Roles = GetUserRoles(),
            LastLogin = DateTime.UtcNow.AddHours(-2)
        };
    }
    private object GetDashboardData(List<string> userRoles)
    {
        var dashboardItems = new List<object>();

        // Core Banking Operations (Available to most roles)
        dashboardItems.AddRange(new[]
        {
            new { Title = "CIF Management", Icon = "users", Url = "/admin/cif", Description = "Customer Information File operations", Module = "CIF" },
            new { Title = "Account Management", Icon = "credit-card", Url = "/admin/accounts", Description = "Account operations and management", Module = "Accounts" },
            new { Title = "Transaction Processing", Icon = "exchange-alt", Url = "/admin/transactions", Description = "Transaction processing operations", Module = "Transactions" },
            new { Title = "Teller Operations", Icon = "cash-register", Url = "/admin/teller", Description = "Branch teller operations", Module = "Teller" }
        });

        // Specialized Operations
        if (userRoles.Any(r => new[] { "LoanOfficer", "BranchManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Loan Management", Icon = "file-contract", Url = "/admin/loans", Description = "Loan processing and management", Module = "Loans" });
        }

        if (userRoles.Any(r => new[] { "InvestmentOfficer", "BranchManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Deposits & Investments", Icon = "piggy-bank", Url = "/admin/deposits", Description = "Fixed deposits and investments", Module = "Deposits" });
        }

        if (userRoles.Any(r => new[] { "BranchManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.AddRange(new[]
            {
                new { Title = "Branch Operations", Icon = "building", Url = "/admin/branches", Description = "Multi-branch management", Module = "Branches" },
                new { Title = "Cards Management", Icon = "credit-card", Url = "/admin/cards", Description = "Card issuance and management", Module = "Cards" }
            });
        }

        if (userRoles.Any(r => new[] { "AccountingOfficer", "FinanceManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "General Ledger", Icon = "book", Url = "/admin/general-ledger", Description = "Financial accounting operations", Module = "GL" });
        }

        if (userRoles.Any(r => new[] { "TreasuryOfficer", "FinanceManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Treasury & Markets", Icon = "chart-line", Url = "/admin/treasury", Description = "Treasury and FX operations", Module = "Treasury" });
        }

        if (userRoles.Any(r => new[] { "TradeFinanceOfficer", "BranchManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Trade Finance", Icon = "globe", Url = "/admin/trade-finance", Description = "International trade finance", Module = "TradeFinance" });
        }

        if (userRoles.Any(r => new[] { "PaymentOfficer", "BranchManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Payment Processing", Icon = "money-bill-wave", Url = "/admin/payments", Description = "Payment processing operations", Module = "Payments" });
        }

        if (userRoles.Any(r => new[] { "ComplianceOfficer", "RiskOfficer", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Compliance & Risk", Icon = "shield-alt", Url = "/admin/compliance", Description = "AML and risk management", Module = "Compliance" });
        }

        if (userRoles.Any(r => new[] { "ReportingOfficer", "Manager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Reporting & Analytics", Icon = "chart-bar", Url = "/admin/reporting", Description = "Reports and business analytics", Module = "Reporting" });
        }

        if (userRoles.Any(r => new[] { "Supervisor", "BranchManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Workflow Management", Icon = "tasks", Url = "/admin/workflows", Description = "Maker-checker workflows", Module = "Workflows" });
        }

        if (userRoles.Any(r => new[] { "ProductManager", "Administrator" }.Contains(r)))
        {
            dashboardItems.Add(new { Title = "Product Factory", Icon = "cogs", Url = "/admin/products", Description = "Banking product management", Module = "Products" });
        }

        // Admin-only items
        if (userRoles.Contains("Administrator"))
        {
            dashboardItems.AddRange(new[]
            {
                new { Title = "User Management", Icon = "users-cog", Url = "/admin/users", Description = "System user management", Module = "Admin" },
                new { Title = "Staff Management", Icon = "user-plus", Url = "/admin/staff/create", Description = "Banking staff management", Module = "Admin" }
            });
        }

        return dashboardItems;
    }

    private object GetNavigationMenu(List<string> userRoles)
    {
        var menuItems = new List<object>();

        // Always available
        menuItems.Add(new { Title = "Dashboard", Icon = "home", Url = "/admin", Active = true });

        // Core Banking Operations
        var coreOperations = new List<object>
        {
            new { Title = "CIF Management", Url = "/admin/cif" },
            new { Title = "Account Management", Url = "/admin/accounts" },
            new { Title = "Transaction Processing", Url = "/admin/transactions" },
            new { Title = "Teller Operations", Url = "/admin/teller" }
        };

        menuItems.Add(new { Title = "Core Banking", Icon = "university", Submenu = coreOperations, Active = false });

        // Lending & Investments
        var lendingMenu = new List<object>();
        if (userRoles.Any(r => new[] { "LoanOfficer", "BranchManager", "Administrator" }.Contains(r)))
            lendingMenu.Add(new { Title = "Loan Management", Url = "/admin/loans" });
        if (userRoles.Any(r => new[] { "InvestmentOfficer", "BranchManager", "Administrator" }.Contains(r)))
            lendingMenu.Add(new { Title = "Deposits & Investments", Url = "/admin/deposits" });

        if (lendingMenu.Any())
            menuItems.Add(new { Title = "Lending & Investments", Icon = "hand-holding-usd", Submenu = lendingMenu, Active = false });

        // Operations Management
        var operationsMenu = new List<object>();
        if (userRoles.Any(r => new[] { "BranchManager", "Administrator" }.Contains(r)))
        {
            operationsMenu.Add(new { Title = "Branch Operations", Url = "/admin/branches" });
            operationsMenu.Add(new { Title = "Cards Management", Url = "/admin/cards" });
        }
        if (userRoles.Any(r => new[] { "PaymentOfficer", "BranchManager", "Administrator" }.Contains(r)))
            operationsMenu.Add(new { Title = "Payment Processing", Url = "/admin/payments" });

        if (operationsMenu.Any())
            menuItems.Add(new { Title = "Operations", Icon = "cogs", Submenu = operationsMenu, Active = false });

        // Financial Management
        var financialMenu = new List<object>();
        if (userRoles.Any(r => new[] { "AccountingOfficer", "FinanceManager", "Administrator" }.Contains(r)))
            financialMenu.Add(new { Title = "General Ledger", Url = "/admin/general-ledger" });
        if (userRoles.Any(r => new[] { "TreasuryOfficer", "FinanceManager", "Administrator" }.Contains(r)))
            financialMenu.Add(new { Title = "Treasury & Markets", Url = "/admin/treasury" });
        if (userRoles.Any(r => new[] { "TradeFinanceOfficer", "BranchManager", "Administrator" }.Contains(r)))
            financialMenu.Add(new { Title = "Trade Finance", Url = "/admin/trade-finance" });

        if (financialMenu.Any())
            menuItems.Add(new { Title = "Financial Management", Icon = "chart-line", Submenu = financialMenu, Active = false });

        // Risk & Compliance
        if (userRoles.Any(r => new[] { "ComplianceOfficer", "RiskOfficer", "Administrator" }.Contains(r)))
        {
            menuItems.Add(new { Title = "Compliance & Risk", Icon = "shield-alt", Url = "/admin/compliance", Active = false });
        }

        // Reporting & Analytics
        if (userRoles.Any(r => new[] { "ReportingOfficer", "Manager", "Administrator" }.Contains(r)))
        {
            menuItems.Add(new { Title = "Reporting & Analytics", Icon = "chart-bar", Url = "/admin/reporting", Active = false });
        }

        // System Management
        var systemMenu = new List<object>();
        if (userRoles.Any(r => new[] { "Supervisor", "BranchManager", "Administrator" }.Contains(r)))
            systemMenu.Add(new { Title = "Workflow Management", Url = "/admin/workflows" });
        if (userRoles.Any(r => new[] { "ProductManager", "Administrator" }.Contains(r)))
            systemMenu.Add(new { Title = "Product Factory", Url = "/admin/products" });
        if (userRoles.Contains("Administrator"))
        {
            systemMenu.Add(new { Title = "User Management", Url = "/admin/users" });
            systemMenu.Add(new { Title = "Staff Management", Url = "/admin/staff/create" });
        }

        if (systemMenu.Any())
            menuItems.Add(new { Title = "System Management", Icon = "tools", Submenu = systemMenu, Active = false });

        return menuItems;
    }
    private object GetAvailableRoles()
    {
        return new
        {
            CoreBanking = new[]
            {
                new { Role = "Teller", Description = "Branch teller operations", Permissions = "CIF, Accounts, Transactions, Teller Operations" },
                new { Role = "CustomerService", Description = "Customer support and onboarding", Permissions = "CIF, Account inquiries, Customer support" },
                new { Role = "BackOfficeStaff", Description = "Back office operations", Permissions = "Transaction processing, Reconciliation" }
            },
            Specialized = new[]
            {
                new { Role = "LoanOfficer", Description = "Loan processing and management", Permissions = "Loans, Credit assessment, Disbursements" },
                new { Role = "InvestmentOfficer", Description = "Investment and deposit products", Permissions = "Fixed deposits, Call deposits, Investment products" },
                new { Role = "PaymentOfficer", Description = "Payment processing", Permissions = "RTGS, SWIFT, Payment orders" },
                new { Role = "TradeFinanceOfficer", Description = "Trade finance operations", Permissions = "Letters of credit, Bank guarantees, Documentary collections" },
                new { Role = "TreasuryOfficer", Description = "Treasury operations", Permissions = "FX deals, Money market, Securities trading" },
                new { Role = "ComplianceOfficer", Description = "Compliance and AML", Permissions = "AML screening, Sanctions screening, Risk assessment" },
                new { Role = "RiskOfficer", Description = "Risk management", Permissions = "Risk assessment, Limit management, Portfolio analysis" }
            },
            Management = new[]
            {
                new { Role = "Supervisor", Description = "Operations supervisor", Permissions = "Approve transactions, Staff oversight, Workflows" },
                new { Role = "BranchManager", Description = "Branch management", Permissions = "All branch operations, Staff management, Approvals" },
                new { Role = "FinanceManager", Description = "Financial management", Permissions = "General ledger, Treasury, Financial reporting" },
                new { Role = "ProductManager", Description = "Product management", Permissions = "Product factory, Product configuration, Pricing" }
            },
            Administrative = new[]
            {
                new { Role = "Administrator", Description = "System administrator", Permissions = "Full system access, User management, System configuration" },
                new { Role = "ReportingOfficer", Description = "Reporting and analytics", Permissions = "All reports, Analytics, Dashboard management" },
                new { Role = "AccountingOfficer", Description = "Accounting operations", Permissions = "General ledger, Journal entries, Financial statements" }
            }
        };
    }

    private object GetUserPermissions()
    {
        var userRoles = GetUserRoles();
        var permissions = new List<string>();

        if (userRoles.Contains("Administrator"))
        {
            permissions.AddRange(new[] { 
                "FULL_SYSTEM_ACCESS", "USER_MANAGEMENT", "SYSTEM_CONFIG", 
                "ALL_MODULES", "AUDIT_LOGS", "SYSTEM_MONITORING" 
            });
        }

        if (userRoles.Contains("BranchManager"))
        {
            permissions.AddRange(new[] { 
                "BRANCH_OPERATIONS", "STAFF_MANAGEMENT", "TRANSACTION_APPROVALS",
                "BRANCH_REPORTS", "CUSTOMER_MANAGEMENT", "PRODUCT_OPERATIONS" 
            });
        }

        // Add module-specific permissions based on roles
        foreach (var role in userRoles)
        {
            permissions.AddRange(GetModulePermissions(role));
        }

        return permissions.Distinct().ToList();
    }

    private List<string> GetModulePermissions(string role)
    {
        return role switch
        {
            "Teller" => new[] { "CIF_READ", "ACCOUNTS_READ", "TRANSACTIONS_CREATE", "TELLER_OPERATIONS" }.ToList(),
            "LoanOfficer" => new[] { "LOANS_FULL", "CREDIT_ASSESSMENT", "DISBURSEMENTS" }.ToList(),
            "ComplianceOfficer" => new[] { "COMPLIANCE_FULL", "AML_SCREENING", "RISK_ASSESSMENT" }.ToList(),
            "TreasuryOfficer" => new[] { "TREASURY_FULL", "FX_DEALS", "MONEY_MARKET" }.ToList(),
            "PaymentOfficer" => new[] { "PAYMENTS_FULL", "RTGS_OPERATIONS", "SWIFT_OPERATIONS" }.ToList(),
            _ => new List<string>()
        };
    }

    private object GetStaffTypes()
    {
        return new[]
        {
            new { 
                Category = "Core Banking",
                Roles = new[]
                {
                    new { Role = "Teller", Description = "Branch teller operations", RequiredSkills = "Cash handling, Customer service, Transaction processing", Modules = "CIF, Accounts, Transactions, Teller" },
                    new { Role = "CustomerService", Description = "Customer support and onboarding", RequiredSkills = "Communication, Problem solving, CIF management", Modules = "CIF, Customer Portal" },
                    new { Role = "BackOfficeStaff", Description = "Back office operations", RequiredSkills = "Data processing, Reconciliation, Attention to detail", Modules = "Transactions, Reconciliation" }
                }
            },
            new {
                Category = "Specialized Banking",
                Roles = new[]
                {
                    new { Role = "LoanOfficer", Description = "Loan processing and management", RequiredSkills = "Credit analysis, Risk assessment, Financial analysis", Modules = "Loans, Credit Assessment" },
                    new { Role = "InvestmentOfficer", Description = "Investment products", RequiredSkills = "Investment knowledge, Product sales, Financial planning", Modules = "Deposits, Investments" },
                    new { Role = "PaymentOfficer", Description = "Payment processing", RequiredSkills = "Payment systems, SWIFT, RTGS operations", Modules = "Payments, RTGS, SWIFT" },
                    new { Role = "TradeFinanceOfficer", Description = "Trade finance operations", RequiredSkills = "International trade, Letters of credit, Documentary collections", Modules = "Trade Finance, LC, Guarantees" },
                    new { Role = "TreasuryOfficer", Description = "Treasury operations", RequiredSkills = "FX trading, Money markets, Risk management", Modules = "Treasury, FX, Money Markets" }
                }
            },
            new {
                Category = "Risk & Compliance",
                Roles = new[]
                {
                    new { Role = "ComplianceOfficer", Description = "Compliance and AML", RequiredSkills = "Regulatory knowledge, AML procedures, Risk assessment", Modules = "Compliance, AML, Sanctions Screening" },
                    new { Role = "RiskOfficer", Description = "Risk management", RequiredSkills = "Risk analysis, Portfolio management, Financial modeling", Modules = "Risk Assessment, Portfolio Analysis" }
                }
            },
            new {
                Category = "Management",
                Roles = new[]
                {
                    new { Role = "Supervisor", Description = "Operations supervision", RequiredSkills = "Leadership, Operations management, Approval workflows", Modules = "Workflows, Approvals, Staff Management" },
                    new { Role = "BranchManager", Description = "Branch management", RequiredSkills = "Management, Strategic planning, Customer relationship", Modules = "All Branch Operations" },
                    new { Role = "FinanceManager", Description = "Financial management", RequiredSkills = "Financial management, Accounting, Reporting", Modules = "General Ledger, Treasury, Reporting" }
                }
            }
        };
    }

    private object GetBranches()
    {
        return new[]
        {
            new { Id = 1, Name = "Main Branch", Code = "001", Location = "Downtown Nairobi", Status = "Active", Staff = 25 },
            new { Id = 2, Name = "Westlands Branch", Code = "002", Location = "Westlands", Status = "Active", Staff = 18 },
            new { Id = 3, Name = "Eastleigh Branch", Code = "003", Location = "Eastleigh", Status = "Active", Staff = 15 },
            new { Id = 4, Name = "Mombasa Branch", Code = "004", Location = "Mombasa", Status = "Active", Staff = 20 },
            new { Id = 5, Name = "Kisumu Branch", Code = "005", Location = "Kisumu", Status = "Active", Staff = 12 }
        };
    }

    private object GetDepartments()
    {
        return new[]
        {
            new { Id = 1, Name = "Customer Service", Description = "Customer facing operations", Modules = "CIF, Customer Portal" },
            new { Id = 2, Name = "Teller Operations", Description = "Branch teller operations", Modules = "Teller, Transactions, Cash Management" },
            new { Id = 3, Name = "Loans & Credit", Description = "Lending operations", Modules = "Loans, Credit Assessment, Disbursements" },
            new { Id = 4, Name = "Treasury & Investments", Description = "Treasury and investment operations", Modules = "Treasury, FX, Deposits, Investments" },
            new { Id = 5, Name = "Trade Finance", Description = "International trade finance", Modules = "Trade Finance, LC, Guarantees" },
            new { Id = 6, Name = "Payments", Description = "Payment processing", Modules = "Payments, RTGS, SWIFT" },
            new { Id = 7, Name = "Compliance & Risk", Description = "Risk and compliance management", Modules = "Compliance, AML, Risk Assessment" },
            new { Id = 8, Name = "Finance & Accounting", Description = "Financial accounting", Modules = "General Ledger, Financial Reporting" },
            new { Id = 9, Name = "IT & Systems", Description = "Information technology support", Modules = "System Administration, User Management" },
            new { Id = 10, Name = "Product Management", Description = "Banking product management", Modules = "Product Factory, Product Configuration" }
        };
    }

    private object GetSystemModules()
    {
        return new[]
        {
            new { Id = 1, Name = "CIF", Description = "Customer Information File", Endpoints = 5, Status = "Active" },
            new { Id = 2, Name = "Accounts", Description = "Account Management", Endpoints = 5, Status = "Active" },
            new { Id = 3, Name = "Transactions", Description = "Transaction Processing", Endpoints = 5, Status = "Active" },
            new { Id = 4, Name = "Loans", Description = "Loan Management", Endpoints = 5, Status = "Active" },
            new { Id = 5, Name = "Deposits", Description = "Fixed Deposits & Investments", Endpoints = 5, Status = "Active" },
            new { Id = 6, Name = "Teller", Description = "Teller Operations", Endpoints = 5, Status = "Active" },
            new { Id = 7, Name = "Branches", Description = "Branch Operations", Endpoints = 5, Status = "Active" },
            new { Id = 8, Name = "Cards", Description = "Cards & Instruments", Endpoints = 5, Status = "Active" },
            new { Id = 9, Name = "General Ledger", Description = "Financial Accounting", Endpoints = 5, Status = "Active" },
            new { Id = 10, Name = "Treasury", Description = "Treasury & Markets", Endpoints = 5, Status = "Active" },
            new { Id = 11, Name = "Trade Finance", Description = "Trade Finance Operations", Endpoints = 5, Status = "Active" },
            new { Id = 12, Name = "Payments", Description = "Payment Processing", Endpoints = 5, Status = "Active" },
            new { Id = 13, Name = "Compliance", Description = "Compliance & Risk", Endpoints = 5, Status = "Active" },
            new { Id = 14, Name = "Reporting", Description = "Reporting & Analytics", Endpoints = 5, Status = "Active" },
            new { Id = 15, Name = "Workflows", Description = "Workflow Engine", Endpoints = 5, Status = "Active" },
            new { Id = 16, Name = "Products", Description = "Product Factory", Endpoints = 5, Status = "Active" }
        };
    }

    private List<string> DetermineUserRoles(string username)
    {
        return username.ToLower() switch
        {
            "admin" => new[] { "Administrator" }.ToList(),
            "manager" => new[] { "BranchManager" }.ToList(),
            "teller" => new[] { "Teller" }.ToList(),
            "loanofficer" => new[] { "LoanOfficer" }.ToList(),
            "compliance" => new[] { "ComplianceOfficer" }.ToList(),
            "treasury" => new[] { "TreasuryOfficer" }.ToList(),
            "payments" => new[] { "PaymentOfficer" }.ToList(),
            "tradefinance" => new[] { "TradeFinanceOfficer" }.ToList(),
            _ => new[] { "Teller" }.ToList()
        };
    }

    private string GenerateToken(Guid userId, string username, List<string> roles)
    {
        // Mock token generation - replace with actual JWT implementation
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userId}:{username}:{string.Join(",", roles)}:{DateTime.UtcNow.Ticks}"));
    }

    private object GetSystemAccess(List<string> roles)
    {
        var moduleAccess = new Dictionary<string, bool>();
        var allModules = new[] { "CIF", "Accounts", "Transactions", "Loans", "Deposits", "Teller", "Branches", "Cards", "GeneralLedger", "Treasury", "TradeFinance", "Payments", "Compliance", "Reporting", "Workflows", "Products" };

        foreach (var module in allModules)
        {
            moduleAccess[module] = HasModuleAccess(roles, module);
        }

        return new
        {
            ModuleAccess = moduleAccess,
            TotalModules = allModules.Length,
            AccessibleModules = moduleAccess.Count(m => m.Value),
            IsFullAccess = roles.Contains("Administrator")
        };
    }

    private bool HasModuleAccess(List<string> roles, string module)
    {
        if (roles.Contains("Administrator")) return true;
        if (roles.Contains("BranchManager")) return true;

        return module switch
        {
            "CIF" => roles.Any(r => new[] { "Teller", "CustomerService" }.Contains(r)),
            "Accounts" => roles.Any(r => new[] { "Teller", "CustomerService", "AccountingOfficer" }.Contains(r)),
            "Transactions" => roles.Any(r => new[] { "Teller", "BackOfficeStaff" }.Contains(r)),
            "Loans" => roles.Contains("LoanOfficer"),
            "Deposits" => roles.Contains("InvestmentOfficer"),
            "Treasury" => roles.Contains("TreasuryOfficer"),
            "TradeFinance" => roles.Contains("TradeFinanceOfficer"),
            "Payments" => roles.Contains("PaymentOfficer"),
            "Compliance" => roles.Any(r => new[] { "ComplianceOfficer", "RiskOfficer" }.Contains(r)),
            "Reporting" => roles.Contains("ReportingOfficer"),
            "Products" => roles.Contains("ProductManager"),
            _ => false
        };
    }

    private async Task<string> GenerateAccountNumberAsync()
    {
        var lastAccount = await _context.Accounts
            .OrderByDescending(a => a.AccountNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastAccount != null && lastAccount.AccountNumber.StartsWith("WKZ"))
        {
            var lastSequence = lastAccount.AccountNumber.Substring(3);
            if (int.TryParse(lastSequence, out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"WKZ{sequence:D8}";
    }

    private async Task<string> GenerateTransactionReferenceAsync(string prefix)
    {
        var lastTransaction = await _context.Transactions
            .Where(t => t.Reference.StartsWith(prefix))
            .OrderByDescending(t => t.Reference)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastTransaction != null)
        {
            var lastSequence = lastTransaction.Reference.Substring(prefix.Length);
            if (int.TryParse(lastSequence, out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D8}";
    }

    private decimal GetInterestRateForAccountType(string accountType)
    {
        return accountType switch
        {
            "Savings" => 6.5m,
            "Current" => 0.0m,
            "Fixed Deposit" => 8.5m,
            "Call Deposit" => 5.0m,
            _ => 0.0m
        };
    }

    private decimal GetMinimumBalanceForAccountType(string accountType)
    {
        return accountType switch
        {
            "Savings" => 1000.00m,
            "Current" => 5000.00m,
            "Fixed Deposit" => 10000.00m,
            "Call Deposit" => 50000.00m,
            _ => 0.00m
        };
    }

    private async Task<string> GenerateCustomerNumberAsync(string prefix)
    {
        var lastCustomer = await _context.Customers
            .Where(c => c.CustomerNumber.StartsWith(prefix))
            .OrderByDescending(c => c.CustomerNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastCustomer != null)
        {
            var lastSequence = lastCustomer.CustomerNumber.Substring(prefix.Length);
            if (int.TryParse(lastSequence, out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D6}";
    }

    #endregion
}

// Request DTOs
public record AdminLoginRequest(string Username, string Password);

// CIF Request DTOs
public record CreateIndividualCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string IdentificationNumber,
    DateTime DateOfBirth,
    string Gender,
    string Nationality
);

public record CreateCorporateCustomerRequest(
    string BusinessName,
    string CompanyName,
    string RegistrationNumber,
    DateTime IncorporationDate,
    string CompanyType,
    string Industry,
    string Email,
    string Phone
);

// Account Request DTOs
public record OpenAccountRequest(
    Guid CustomerId,
    Guid? BusinessId,
    string AccountType,
    string Currency,
    decimal InitialDeposit
);

public record FreezeAccountRequest(
    bool Freeze,
    string Reason
);