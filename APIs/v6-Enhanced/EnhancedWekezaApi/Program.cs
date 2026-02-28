using Microsoft.EntityFrameworkCore;
using MediatR;
using EnhancedWekezaApi.Infrastructure.Data;
using EnhancedWekezaApi.Domain.Interfaces;
using EnhancedWekezaApi.Infrastructure.Repositories;
using EnhancedWekezaApi.Application.Features.Accounts.Commands.OpenAccount;
using EnhancedWekezaApi.Application.Features.Accounts.Commands.CloseAccount;
using EnhancedWekezaApi.Application.Features.Accounts.Commands.FreezeAccount;
using EnhancedWekezaApi.Application.Features.Accounts.Queries.GetBalance;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Wekeza Enhanced Banking API", 
        Version = "v1",
        Description = "Enhanced Core Banking System with Full Accounts Feature Implementation"
    });
});

// Configure JSON serialization
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});

// Add Entity Framework with PostgreSQL
builder.Services.AddDbContext<WekezaDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=wekeza_banking_enhanced;Username=postgres;Password=the_beast_pass"));

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

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
    var context = scope.ServiceProvider.GetRequiredService<WekezaDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wekeza Enhanced Banking API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();

// === WEKEZA ENHANCED CORE BANKING SYSTEM - PORT 5001 ===

// Welcome endpoint - serve HTML interface
app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>🏦 Wekeza Enhanced Banking System (Port 5002)</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            background: linear-gradient(135deg, #2c3e50 0%, #3498db 100%);
            min-height: 100vh;
            color: #333;
        }
        .container { 
            max-width: 1400px; 
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
            font-size: 2.8em;
        }
        .status { 
            color: #27ae60; 
            font-weight: bold; 
            font-size: 1.3em;
        }
        .enhanced-status {
            background: linear-gradient(135deg, #e74c3c, #f39c12);
            color: white;
            padding: 10px;
            border-radius: 8px;
            margin-top: 10px;
            font-weight: bold;
        }
        .owner-info {
            margin-top: 15px;
            color: #7f8c8d;
        }
        .dashboard {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
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
            font-size: 1.4em;
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
        .form-group input, .form-group select, .form-group textarea {
            width: 100%;
            padding: 12px;
            border: 2px solid #ddd;
            border-radius: 8px;
            font-size: 16px;
        }
        .form-group input:focus, .form-group select:focus, .form-group textarea:focus {
            outline: none;
            border-color: #3498db;
        }
        .btn {
            background: linear-gradient(135deg, #2c3e50 0%, #3498db 100%);
            color: white;
            padding: 14px 28px;
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
        .btn.danger {
            background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%);
        }
        .btn.warning {
            background: linear-gradient(135deg, #f39c12 0%, #e67e22 100%);
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
        .features-list {
            background: rgba(255,255,255,0.95);
            padding: 20px;
            border-radius: 15px;
            margin-top: 20px;
        }
        .features-list h3 {
            color: #2c3e50;
            margin-bottom: 15px;
        }
        .features-list ul {
            list-style: none;
            padding: 0;
        }
        .features-list li {
            padding: 8px 0;
            border-bottom: 1px solid #eee;
        }
        .features-list li:before {
            content: "✅ ";
            margin-right: 10px;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🏦 Wekeza Enhanced Banking System</h1>
            <div class="status">🟢 OPERATIONAL - PORT 5002</div>
            <div class="enhanced-status">🚀 ENHANCED MVP - Full Accounts Feature Implementation</div>
            <div class="owner-info">
                <strong>Owner:</strong> Emmanuel Odenyire<br>
                <strong>Contact:</strong> 0716478835<br>
                <strong>ID:</strong> 28839872<br>
                <strong>DOB:</strong> 17/March/1992
            </div>
        </div>

        <div class="dashboard">
            <!-- Open Account (Enhanced) -->
            <div class="card">
                <h3>🏦 Open Account (Enhanced)</h3>
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
                <div class="form-group">
                    <label>Currency</label>
                    <select id="currency">
                        <option value="KES">KES - Kenyan Shilling</option>
                        <option value="USD">USD - US Dollar</option>
                        <option value="EUR">EUR - Euro</option>
                        <option value="GBP">GBP - British Pound</option>
                    </select>
                </div>
                <div class="form-group">
                    <label>Initial Deposit</label>
                    <input type="number" id="initialDeposit" placeholder="Enter initial deposit" min="0" step="0.01">
                </div>
                <button class="btn" onclick="openAccount()">Open Account</button>
                <div id="openAccountResult" class="result"></div>
            </div>

            <!-- Get Balance -->
            <div class="card">
                <h3>📊 Get Account Balance</h3>
                <div class="form-group">
                    <label>Account Number</label>
                    <input type="text" id="balanceAccount" placeholder="Enter account number">
                </div>
                <button class="btn" onclick="getBalance()">Get Balance</button>
                <div id="balanceResult" class="result"></div>
            </div>

            <!-- Freeze Account -->
            <div class="card">
                <h3>🧊 Freeze Account</h3>
                <div class="form-group">
                    <label>Account Number</label>
                    <input type="text" id="freezeAccount" placeholder="Enter account number">
                </div>
                <div class="form-group">
                    <label>Freeze Reason</label>
                    <textarea id="freezeReason" placeholder="Enter reason for freezing account" rows="3"></textarea>
                </div>
                <button class="btn warning" onclick="freezeAccount()">Freeze Account</button>
                <div id="freezeResult" class="result"></div>
            </div>

            <!-- Close Account -->
            <div class="card">
                <h3>🔒 Close Account</h3>
                <div class="form-group">
                    <label>Account Number</label>
                    <input type="text" id="closeAccount" placeholder="Enter account number">
                </div>
                <div class="form-group">
                    <label>Closure Reason</label>
                    <textarea id="closeReason" placeholder="Enter reason for closing account" rows="3"></textarea>
                </div>
                <button class="btn danger" onclick="closeAccount()">Close Account</button>
                <div id="closeResult" class="result"></div>
            </div>
        </div>

        <div class="features-list">
            <h3>🚀 Enhanced Features Implemented</h3>
            <ul>
                <li>Complete CQRS Architecture with MediatR</li>
                <li>Domain-Driven Design with Rich Domain Models</li>
                <li>Value Objects (Money, AccountNumber, Currency)</li>
                <li>Repository Pattern with Entity Framework</li>
                <li>Command and Query Separation</li>
                <li>Business Rule Enforcement in Domain Layer</li>
                <li>Account Opening with Customer Creation</li>
                <li>Account Balance Inquiries</li>
                <li>Account Freezing with Reason Tracking</li>
                <li>Account Closure with Balance Validation</li>
                <li>Comprehensive Error Handling</li>
                <li>Database Persistence with PostgreSQL</li>
            </ul>
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
                const result = await response.json();
                
                if (!response.ok) {
                    return { error: result.message || result.title || 'Request failed' };
                }
                
                return result;
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

        async function openAccount() {
            const data = {
                firstName: document.getElementById('firstName').value,
                lastName: document.getElementById('lastName').value,
                email: document.getElementById('email').value,
                identificationNumber: document.getElementById('idNumber').value,
                currencyCode: document.getElementById('currency').value,
                initialDeposit: parseFloat(document.getElementById('initialDeposit').value) || 0
            };

            if (!data.firstName || !data.lastName || !data.email || !data.identificationNumber) {
                showResult('openAccountResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/accounts/open', 'POST', data);
            showResult('openAccountResult', result, !!result.error);
        }

        async function getBalance() {
            const accountNumber = document.getElementById('balanceAccount').value;

            if (!accountNumber) {
                showResult('balanceResult', { error: 'Account number is required' }, true);
                return;
            }

            const result = await apiCall(`/api/accounts/${accountNumber}/balance`);
            showResult('balanceResult', result, !!result.error);
        }

        async function freezeAccount() {
            const data = {
                accountNumber: document.getElementById('freezeAccount').value,
                freezeReason: document.getElementById('freezeReason').value
            };

            if (!data.accountNumber || !data.freezeReason) {
                showResult('freezeResult', { error: 'Account number and reason are required' }, true);
                return;
            }

            const result = await apiCall('/api/accounts/freeze', 'POST', data);
            showResult('freezeResult', result, !!result.error);
        }

        async function closeAccount() {
            const data = {
                accountNumber: document.getElementById('closeAccount').value,
                closureReason: document.getElementById('closeReason').value
            };

            if (!data.accountNumber || !data.closureReason) {
                showResult('closeResult', { error: 'Account number and reason are required' }, true);
                return;
            }

            const result = await apiCall('/api/accounts/close', 'POST', data);
            showResult('closeResult', result, !!result.error);
        }
    </script>
</body>
</html>
""", "text/html"));

// === ENHANCED ACCOUNTS API ENDPOINTS ===

// Open Account (Enhanced with CQRS)
app.MapPost("/api/accounts/open", async ([FromBody] OpenAccountCommand command, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(command);
        return Results.Ok(new
        {
            Message = "✅ Account opened successfully using CQRS pattern!",
            Account = result
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to open account",
            Error = ex.Message
        });
    }
});

// Get Balance (Enhanced with CQRS)
app.MapGet("/api/accounts/{accountNumber}/balance", async (string accountNumber, IMediator mediator) =>
{
    try
    {
        var query = new GetBalanceQuery(accountNumber);
        var result = await mediator.Send(query);
        return Results.Ok(new
        {
            Message = "✅ Balance retrieved successfully using CQRS pattern!",
            Account = result
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

// Freeze Account (Enhanced with CQRS)
app.MapPost("/api/accounts/freeze", async ([FromBody] FreezeAccountCommand command, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(command);
        return Results.Ok(new
        {
            Message = "✅ Account frozen successfully using CQRS pattern!",
            Success = result
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to freeze account",
            Error = ex.Message
        });
    }
});

// Close Account (Enhanced with CQRS)
app.MapPost("/api/accounts/close", async ([FromBody] CloseAccountCommand command, IMediator mediator) =>
{
    try
    {
        var result = await mediator.Send(command);
        return Results.Ok(new
        {
            Message = "✅ Account closed successfully using CQRS pattern!",
            Success = result
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to close account",
            Error = ex.Message
        });
    }
});

// Configure to run on port 5002
app.Urls.Add("http://localhost:5002");

app.Run();

// Request DTOs for API endpoints
public record OpenAccountRequest(string FirstName, string LastName, string Email, string IdentificationNumber, string CurrencyCode, decimal InitialDeposit);
public record FreezeAccountRequest(string AccountNumber, string FreezeReason);
public record CloseAccountRequest(string AccountNumber, string ClosureReason);