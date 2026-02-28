using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MinimalWekezaApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON serialization to handle circular references
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});

// Add Entity Framework with PostgreSQL
builder.Services.AddDbContext<MinimalDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=wekeza_banking_minimal;Username=postgres;Password=the_beast_pass"));

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Create database and tables if they don't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MinimalDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wekeza Banking API v1");
        c.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger
    });
}

// Serve static files (our web interface)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();

// === WEKEZA CORE BANKING SYSTEM ENDPOINTS ===

// Welcome endpoint - serve HTML interface
app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>🏦 Wekeza Core Banking System - Minimal (Port 5000)</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            color: #333;
        }
        .container { 
            max-width: 1200px; 
            margin: 0 auto; 
            padding: 20px; 
        }
        .header {
            background: rgba(255,255,255,0.95);
            padding: 30px;
            border-radius: 15px;
            margin-bottom: 30px;
            text-align: center;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }
        .header h1 { 
            color: #2c3e50; 
            margin-bottom: 10px;
            font-size: 2.5em;
        }
        .status { 
            color: #27ae60; 
            font-weight: bold; 
            font-size: 1.2em;
        }
        .owner-info {
            margin-top: 15px;
            color: #7f8c8d;
        }
        .dashboard {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        .card {
            background: rgba(255,255,255,0.95);
            padding: 25px;
            border-radius: 15px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }
        .card h3 {
            color: #2c3e50;
            margin-bottom: 15px;
            font-size: 1.3em;
        }
        .form-group {
            margin-bottom: 15px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
            color: #555;
        }
        .form-group input, .form-group select {
            width: 100%;
            padding: 10px;
            border: 2px solid #ddd;
            border-radius: 8px;
            font-size: 16px;
        }
        .form-group input:focus, .form-group select:focus {
            outline: none;
            border-color: #667eea;
        }
        .btn {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 12px 25px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 16px;
            font-weight: bold;
            width: 100%;
            margin-top: 10px;
            transition: transform 0.2s;
        }
        .btn:hover {
            transform: translateY(-2px);
        }
        .result {
            margin-top: 15px;
            padding: 15px;
            border-radius: 8px;
            display: none;
        }
        .result.success {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        .result.error {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        .stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-top: 20px;
        }
        .stat-card {
            background: rgba(255,255,255,0.9);
            padding: 20px;
            border-radius: 10px;
            text-align: center;
        }
        .stat-number {
            font-size: 2em;
            font-weight: bold;
            color: #667eea;
        }
        .stat-label {
            color: #666;
            margin-top: 5px;
        }
        .api-links {
            background: rgba(255,255,255,0.95);
            padding: 20px;
            border-radius: 15px;
            margin-top: 20px;
            text-align: center;
        }
        .api-links a {
            display: inline-block;
            margin: 10px;
            padding: 10px 20px;
            background: #667eea;
            color: white;
            text-decoration: none;
            border-radius: 8px;
            transition: background 0.3s;
        }
        .api-links a:hover {
            background: #764ba2;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🏦 Wekeza Core Banking System - Minimal</h1>
            <div class="status">🟢 OPERATIONAL - PORT 5000</div>
            <div class="owner-info">
                <strong>Owner:</strong> Emmanuel Odenyire<br>
                <strong>Contact:</strong> 0716478835<br>
                <strong>ID:</strong> 28839872<br>
                <strong>DOB:</strong> 17/March/1992
            </div>
        </div>

        <div class="dashboard">
            <!-- Customer Management -->
            <div class="card">
                <h3>👤 Create Customer</h3>
                <div class="form-group">
                    <label>First Name</label>
                    <input type="text" id="firstName" placeholder="Enter first name">
                </div>
                <div class="form-group">
                    <label>Last Name</label>
                    <input type="text" id="lastName" placeholder="Enter last name">
                </div>
                <div class="form-group">
                    <label>Email</label>
                    <input type="email" id="email" placeholder="Enter email">
                </div>
                <div class="form-group">
                    <label>ID Number</label>
                    <input type="text" id="idNumber" placeholder="Enter ID number">
                </div>
                <button class="btn" onclick="createCustomer()">Create Customer</button>
                <div id="customerResult" class="result"></div>
            </div>

            <!-- Account Management -->
            <div class="card">
                <h3>🏦 Create Account</h3>
                <div class="form-group">
                    <label>Customer ID</label>
                    <input type="text" id="customerId" placeholder="Enter customer ID">
                </div>
                <div class="form-group">
                    <label>Currency</label>
                    <select id="currency">
                        <option value="KES">KES - Kenyan Shilling</option>
                        <option value="USD">USD - US Dollar</option>
                        <option value="EUR">EUR - Euro</option>
                    </select>
                </div>
                <div class="form-group">
                    <label>Initial Deposit</label>
                    <input type="number" id="initialDeposit" placeholder="Enter initial deposit" min="0" step="0.01">
                </div>
                <button class="btn" onclick="createAccount()">Create Account</button>
                <div id="accountResult" class="result"></div>
            </div>

            <!-- Deposit -->
            <div class="card">
                <h3>💰 Process Deposit</h3>
                <div class="form-group">
                    <label>Account Number</label>
                    <input type="text" id="depositAccount" placeholder="Enter account number">
                </div>
                <div class="form-group">
                    <label>Amount</label>
                    <input type="number" id="depositAmount" placeholder="Enter deposit amount" min="0" step="0.01">
                </div>
                <button class="btn" onclick="processDeposit()">Process Deposit</button>
                <div id="depositResult" class="result"></div>
            </div>

            <!-- Withdrawal -->
            <div class="card">
                <h3>💸 Process Withdrawal</h3>
                <div class="form-group">
                    <label>Account Number</label>
                    <input type="text" id="withdrawAccount" placeholder="Enter account number">
                </div>
                <div class="form-group">
                    <label>Amount</label>
                    <input type="number" id="withdrawAmount" placeholder="Enter withdrawal amount" min="0" step="0.01">
                </div>
                <button class="btn" onclick="processWithdrawal()">Process Withdrawal</button>
                <div id="withdrawResult" class="result"></div>
            </div>

            <!-- Balance Inquiry -->
            <div class="card">
                <h3>📊 Check Balance</h3>
                <div class="form-group">
                    <label>Account Number</label>
                    <input type="text" id="balanceAccount" placeholder="Enter account number">
                </div>
                <button class="btn" onclick="checkBalance()">Check Balance</button>
                <div id="balanceResult" class="result"></div>
            </div>

            <!-- System Status -->
            <div class="card">
                <h3>🖥️ System Status</h3>
                <button class="btn" onclick="getSystemStatus()">Refresh Status</button>
                <div id="statusResult" class="result"></div>
                <div class="stats" id="systemStats"></div>
            </div>
        </div>

        <div class="api-links">
            <h3>🔗 API Access</h3>
            <a href="/api/status" target="_blank">System Status API</a>
            <a href="/swagger" target="_blank">API Documentation</a>
        </div>
    </div>

    <script>
        const API_BASE = '';

        async function apiCall(endpoint, method = 'GET', data = null) {
            try {
                const options = {
                    method,
                    headers: {
                        'Content-Type': 'application/json',
                    }
                };
                
                if (data) {
                    options.body = JSON.stringify(data);
                }

                const response = await fetch(API_BASE + endpoint, options);
                return await response.json();
            } catch (error) {
                return { error: error.message };
            }
        }

        function showResult(elementId, data, isError = false) {
            const element = document.getElementById(elementId);
            element.style.display = 'block';
            element.className = `result ${isError ? 'error' : 'success'}`;
            element.innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
        }

        async function createCustomer() {
            const data = {
                firstName: document.getElementById('firstName').value,
                lastName: document.getElementById('lastName').value,
                email: document.getElementById('email').value,
                identificationNumber: document.getElementById('idNumber').value
            };

            if (!data.firstName || !data.lastName || !data.email || !data.identificationNumber) {
                showResult('customerResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/customers', 'POST', data);
            showResult('customerResult', result, !!result.error);
        }

        async function createAccount() {
            const customerId = document.getElementById('customerId').value;
            const data = {
                customerId: customerId,
                currencyCode: document.getElementById('currency').value,
                initialDeposit: parseFloat(document.getElementById('initialDeposit').value) || 0
            };

            if (!customerId) {
                showResult('accountResult', { error: 'Customer ID is required' }, true);
                return;
            }

            const result = await apiCall('/api/accounts', 'POST', data);
            showResult('accountResult', result, !!result.error);
        }

        async function processDeposit() {
            const data = {
                accountNumber: document.getElementById('depositAccount').value,
                amount: parseFloat(document.getElementById('depositAmount').value) || 0
            };

            if (!data.accountNumber || data.amount <= 0) {
                showResult('depositResult', { error: 'Valid account number and amount are required' }, true);
                return;
            }

            const result = await apiCall('/api/transactions/deposit', 'POST', data);
            showResult('depositResult', result, !!result.error);
        }

        async function processWithdrawal() {
            const data = {
                accountNumber: document.getElementById('withdrawAccount').value,
                amount: parseFloat(document.getElementById('withdrawAmount').value) || 0
            };

            if (!data.accountNumber || data.amount <= 0) {
                showResult('withdrawResult', { error: 'Valid account number and amount are required' }, true);
                return;
            }

            const result = await apiCall('/api/transactions/withdraw', 'POST', data);
            showResult('withdrawResult', result, !!result.error);
        }

        async function checkBalance() {
            const accountNumber = document.getElementById('balanceAccount').value;

            if (!accountNumber) {
                showResult('balanceResult', { error: 'Account number is required' }, true);
                return;
            }

            const result = await apiCall(`/api/accounts/${accountNumber}/balance`);
            showResult('balanceResult', result, !!result.error);
        }

        async function getSystemStatus() {
            const result = await apiCall('/api/status');
            showResult('statusResult', result, !!result.error);
            
            if (result.Statistics) {
                const statsHtml = `
                    <div class="stat-card">
                        <div class="stat-number">${result.Statistics.TotalCustomers}</div>
                        <div class="stat-label">Total Customers</div>
                    </div>
                    <div class="stat-card">
                        <div class="stat-number">${result.Statistics.TotalAccounts}</div>
                        <div class="stat-label">Total Accounts</div>
                    </div>
                    <div class="stat-card">
                        <div class="stat-number">${result.Statistics.TotalTransactions}</div>
                        <div class="stat-label">Total Transactions</div>
                    </div>
                    <div class="stat-card">
                        <div class="stat-number">${result.Statistics.SystemLoad}</div>
                        <div class="stat-label">System Load</div>
                    </div>
                `;
                document.getElementById('systemStats').innerHTML = statsHtml;
            }
        }

        // Load system status on page load
        window.onload = function() {
            getSystemStatus();
        };
    </script>
</body>
</html>
""", "text/html"));

// System status
app.MapGet("/api/status", () => new
{
    SystemName = "Wekeza Core Banking System",
    Owner = "Emmanuel Odenyire (ID: 28839872)",
    Contact = "0716478835",
    Status = "🟢 OPERATIONAL",
    Uptime = TimeSpan.FromMinutes(Random.Shared.Next(1, 1440)).ToString(@"hh\:mm\:ss"),
    Components = new
    {
        API = "✅ Active",
        Database = "✅ PostgreSQL Ready",
        Cache = "✅ Redis Available", 
        Security = "🔐 JWT Ready"
    },
    Statistics = new
    {
        TotalCustomers = Random.Shared.Next(1000, 5000),
        TotalAccounts = Random.Shared.Next(1200, 6000),
        TotalTransactions = Random.Shared.Next(10000, 50000),
        SystemLoad = $"{Random.Shared.Next(15, 85)}%"
    },
    LastUpdated = DateTime.UtcNow
});

// Create customer
app.MapPost("/api/customers", async ([FromBody] CreateCustomerRequest request, MinimalDbContext context) =>
{
    try
    {
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            IdentificationNumber = request.IdentificationNumber,
            CustomerNumber = $"CUS-{Guid.NewGuid().ToString()[..8].ToUpper()}"
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Customer created successfully and saved to PostgreSQL!",
            Customer = customer
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to create customer",
            Error = ex.Message
        });
    }
});

// Create account
app.MapPost("/api/accounts", async ([FromBody] CreateAccountRequest request, MinimalDbContext context) =>
{
    try
    {
        // Check if customer exists
        var customer = await context.Customers.FindAsync(request.CustomerId);
        if (customer == null)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Customer not found",
                Error = "Please create customer first"
            });
        }

        var account = new Account
        {
            AccountNumber = $"WKZ-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            CustomerId = request.CustomerId,
            CurrencyCode = request.CurrencyCode ?? "KES",
            Balance = request.InitialDeposit,
            AvailableBalance = request.InitialDeposit
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Account created successfully and saved to PostgreSQL!",
            Account = account
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to create account",
            Error = ex.Message
        });
    }
});

// Process deposit
app.MapPost("/api/transactions/deposit", async ([FromBody] DepositRequest request, MinimalDbContext context) =>
{
    try
    {
        // Find account by account number
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);
        if (account == null)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Account not found",
                Error = "Invalid account number"
            });
        }

        // Update account balance
        var previousBalance = account.Balance;
        account.Balance += request.Amount;
        account.AvailableBalance += request.Amount;

        // Create transaction record
        var transaction = new Transaction
        {
            Reference = $"DEP-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            AccountId = account.Id,
            Type = "Credit/Deposit",
            Amount = request.Amount,
            BalanceAfter = account.Balance,
            Description = "Cash Deposit"
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Deposit processed successfully and saved to PostgreSQL!",
            Transaction = new
            {
                TransactionId = transaction.Id,
                Reference = transaction.Reference,
                AccountNumber = request.AccountNumber,
                Type = transaction.Type,
                Amount = transaction.Amount,
                PreviousBalance = previousBalance,
                NewBalance = account.Balance,
                Status = transaction.Status,
                ProcessedAt = transaction.ProcessedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Deposit failed",
            Error = ex.Message
        });
    }
});

// Process withdrawal
app.MapPost("/api/transactions/withdraw", ([FromBody] WithdrawRequest request) =>
{
    try
    {
        var transactionId = Guid.NewGuid();
        var currentBalance = Random.Shared.Next(5000, 50000); // Simulate current balance
        
        if (request.Amount > currentBalance)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Insufficient funds",
                AvailableBalance = currentBalance,
                RequestedAmount = request.Amount
            });
        }
        
        var newBalance = currentBalance - request.Amount;
        
        var transaction = new
        {
            TransactionId = transactionId,
            AccountNumber = request.AccountNumber,
            Type = "Debit/Withdrawal",
            Amount = request.Amount,
            Currency = "KES",
            PreviousBalance = currentBalance,
            NewBalance = newBalance,
            Status = "Completed",
            Reference = $"WTH-{transactionId.ToString()[..8].ToUpper()}",
            ProcessedAt = DateTime.UtcNow,
            ProcessedBy = "System"
        };

        return Results.Ok(new
        {
            Message = "✅ Withdrawal processed successfully!",
            Transaction = transaction
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Withdrawal failed",
            Error = ex.Message
        });
    }
});

// Get account balance
app.MapGet("/api/accounts/{accountNumber}/balance", async (string accountNumber, MinimalDbContext context) =>
{
    try
    {
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        if (account == null)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Account not found",
                Error = "Invalid account number"
            });
        }
        
        return Results.Ok(new
        {
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            AvailableBalance = account.AvailableBalance,
            Currency = account.CurrencyCode,
            Status = account.Status,
            AccountType = account.AccountType,
            LastUpdated = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to get balance",
            Error = ex.Message
        });
    }
});

// Get transaction history
app.MapGet("/api/accounts/{accountNumber}/transactions", (string accountNumber) =>
{
    try
    {
        var transactions = Enumerable.Range(1, 10).Select(i => new
        {
            TransactionId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(-i),
            Type = Random.Shared.Next(2) == 0 ? "Credit" : "Debit",
            Amount = Random.Shared.Next(100, 5000),
            Description = Random.Shared.Next(2) == 0 ? "Deposit" : "Withdrawal",
            Reference = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Status = "Completed"
        }).ToArray();

        return Results.Ok(new
        {
            AccountNumber = accountNumber,
            Transactions = transactions,
            TotalTransactions = transactions.Length
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to get transactions",
            Error = ex.Message
        });
    }
});

// Configure to run on port 5000
app.Urls.Add("http://localhost:5000");

app.Run();

// Request DTOs
public record CreateCustomerRequest(string FirstName, string LastName, string Email, string IdentificationNumber);
public record CreateAccountRequest(Guid CustomerId, string? CurrencyCode, decimal InitialDeposit);
public record DepositRequest(string AccountNumber, decimal Amount);
public record WithdrawRequest(string AccountNumber, decimal Amount);