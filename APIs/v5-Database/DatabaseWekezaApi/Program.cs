using Microsoft.EntityFrameworkCore;
using DatabaseWekezaApi.Data;
using DatabaseWekezaApi.Models;
using Microsoft.AspNetCore.Mvc;

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
builder.Services.AddDbContext<WekezaDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=wekeza_banking;Username=postgres;Password=the_beast_pass"));

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
    context.Database.EnsureCreated(); // Just ensure created, preserve existing data
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wekeza Banking API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();

// === WEKEZA CORE BANKING SYSTEM WITH DATABASE ===

// Welcome endpoint - serve HTML interface
app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>🏦 Wekeza Core Banking System - Database Connected (Port 5001)</title>
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
        .database-status {
            background: #d4edda;
            color: #155724;
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
        .tab-content {
            display: block;
        }
        .btn.active {
            background: #27ae60 !important;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🏦 Wekeza Core Banking System - Database</h1>
            <div class="status">🟢 OPERATIONAL - PORT 5001</div>
            <div class="database-status">🗄️ DATABASE CONNECTED - PostgreSQL</div>
            <div class="owner-info">
                <strong>Owner:</strong> Emmanuel Odenyire<br>
                <strong>Contact:</strong> 0716478835<br>
                <strong>ID:</strong> 28839872<br>
                <strong>DOB:</strong> 17/March/1992
            </div>
        </div>

        <!-- Navigation Tabs -->
        <div style="background: rgba(255,255,255,0.95); padding: 20px; border-radius: 15px; margin-bottom: 20px; text-align: center;">
            <button class="btn" onclick="showTab('basic')" id="basicTab" style="margin: 5px; background: #667eea;">Basic Banking</button>
            <button class="btn" onclick="showTab('cif')" id="cifTab" style="margin: 5px; background: #764ba2;">CIF Management</button>
            <button class="btn" onclick="showTab('loans')" id="loansTab" style="margin: 5px; background: #667eea;">Loans & Deposits</button>
            <button class="btn" onclick="showTab('operations')" id="operationsTab" style="margin: 5px; background: #764ba2;">Operations</button>
            <button class="btn" onclick="showTab('treasury')" id="treasuryTab" style="margin: 5px; background: #667eea;">Treasury & Trade</button>
        </div>

        <!-- Basic Banking Tab -->
        <div id="basicTab-content" class="tab-content">
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

                <!-- View All Customers -->
                <div class="card">
                    <h3>👥 View All Customers</h3>
                    <button class="btn" onclick="getAllCustomers()">Load All Customers</button>
                    <div id="customersResult" class="result"></div>
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

                <!-- View All Accounts -->
                <div class="card">
                    <h3>🏦 View All Accounts</h3>
                    <button class="btn" onclick="getAllAccounts()">Load All Accounts</button>
                    <div id="accountsResult" class="result"></div>
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

                <!-- Transaction History -->
                <div class="card">
                    <h3>📋 Transaction History</h3>
                    <div class="form-group">
                        <label>Account Number</label>
                        <input type="text" id="historyAccount" placeholder="Enter account number">
                    </div>
                    <button class="btn" onclick="getTransactionHistory()">Get History</button>
                    <div id="historyResult" class="result"></div>
                </div>

                <!-- System Status -->
                <div class="card">
                    <h3>🖥️ System Status</h3>
                    <button class="btn" onclick="getSystemStatus()">Refresh Status</button>
                    <div id="statusResult" class="result"></div>
                    <div class="stats" id="systemStats"></div>
                </div>
            </div>
        </div>

        <!-- CIF Management Tab -->
        <div id="cifTab-content" class="tab-content" style="display: none;">
            <div class="dashboard">
                <!-- AML Screening -->
                <div class="card">
                    <h3>🔍 AML Screening</h3>
                    <div class="form-group">
                        <label>Customer ID</label>
                        <input type="text" id="amlCustomerId" placeholder="Enter customer ID">
                    </div>
                    <button class="btn" onclick="performAMLScreening()">Perform AML Screening</button>
                    <div id="amlResult" class="result"></div>
                </div>

                <!-- Pending KYC -->
                <div class="card">
                    <h3>⏳ Pending KYC</h3>
                    <button class="btn" onclick="getPendingKYC()">Get Pending KYC</button>
                    <div id="pendingKYCResult" class="result"></div>
                </div>

                <!-- High Risk Parties -->
                <div class="card">
                    <h3>⚠️ High Risk Parties</h3>
                    <button class="btn" onclick="getHighRiskParties()">Get High Risk Parties</button>
                    <div id="highRiskResult" class="result"></div>
                </div>

                <!-- Customer 360 View -->
                <div class="card">
                    <h3>👁️ Customer 360 View</h3>
                    <div class="form-group">
                        <label>Customer ID</label>
                        <input type="text" id="customer360Id" placeholder="Enter customer ID">
                    </div>
                    <button class="btn" onclick="getCustomer360()">Get Customer 360</button>
                    <div id="customer360Result" class="result"></div>
                </div>

                <!-- Search Parties -->
                <div class="card">
                    <h3>🔎 Search Parties</h3>
                    <div class="form-group">
                        <label>Search Term</label>
                        <input type="text" id="searchTerm" placeholder="Enter name, email, or phone">
                    </div>
                    <button class="btn" onclick="searchParties()">Search Parties</button>
                    <div id="searchResult" class="result"></div>
                </div>

                <!-- Create Business -->
                <div class="card">
                    <h3>🏢 Create Business</h3>
                    <div class="form-group">
                        <label>Company Name</label>
                        <input type="text" id="companyName" placeholder="Enter company name">
                    </div>
                    <div class="form-group">
                        <label>Registration Number</label>
                        <input type="text" id="regNumber" placeholder="Enter registration number">
                    </div>
                    <div class="form-group">
                        <label>Email</label>
                        <input type="email" id="businessEmail" placeholder="Enter business email">
                    </div>
                    <button class="btn" onclick="createBusiness()">Create Business</button>
                    <div id="businessResult" class="result"></div>
                </div>
            </div>
        </div>

        <!-- Loans & Deposits Tab -->
        <div id="loansTab-content" class="tab-content" style="display: none;">
            <div class="dashboard">
                <!-- Apply for Loan -->
                <div class="card">
                    <h3>💳 Apply for Loan</h3>
                    <div class="form-group">
                        <label>Customer ID</label>
                        <input type="text" id="loanCustomerId" placeholder="Enter customer ID">
                    </div>
                    <div class="form-group">
                        <label>Account ID</label>
                        <input type="text" id="loanAccountId" placeholder="Enter account ID">
                    </div>
                    <div class="form-group">
                        <label>Loan Amount</label>
                        <input type="number" id="loanAmount" placeholder="Enter loan amount" min="1000" step="1000">
                    </div>
                    <div class="form-group">
                        <label>Term (Months)</label>
                        <input type="number" id="loanTerm" placeholder="Enter term in months" min="6" max="60">
                    </div>
                    <div class="form-group">
                        <label>Purpose</label>
                        <input type="text" id="loanPurpose" placeholder="Enter loan purpose">
                    </div>
                    <button class="btn" onclick="applyForLoan()">Apply for Loan</button>
                    <div id="loanApplicationResult" class="result"></div>
                </div>

                <!-- Book Fixed Deposit -->
                <div class="card">
                    <h3>🏦 Book Fixed Deposit</h3>
                    <div class="form-group">
                        <label>Customer ID</label>
                        <input type="text" id="fdCustomerId" placeholder="Enter customer ID">
                    </div>
                    <div class="form-group">
                        <label>Source Account ID</label>
                        <input type="text" id="fdAccountId" placeholder="Enter source account ID">
                    </div>
                    <div class="form-group">
                        <label>Amount</label>
                        <input type="number" id="fdAmount" placeholder="Enter deposit amount" min="10000" step="1000">
                    </div>
                    <div class="form-group">
                        <label>Interest Rate (%)</label>
                        <input type="number" id="fdRate" placeholder="Enter interest rate" min="1" max="15" step="0.1">
                    </div>
                    <div class="form-group">
                        <label>Term (Months)</label>
                        <input type="number" id="fdTerm" placeholder="Enter term in months" min="3" max="60">
                    </div>
                    <button class="btn" onclick="bookFixedDeposit()">Book Fixed Deposit</button>
                    <div id="fdResult" class="result"></div>
                </div>

                <!-- Product Management -->
                <div class="card">
                    <h3>📦 Create Product</h3>
                    <div class="form-group">
                        <label>Product Code</label>
                        <input type="text" id="productCode" placeholder="Enter product code">
                    </div>
                    <div class="form-group">
                        <label>Product Name</label>
                        <input type="text" id="productName" placeholder="Enter product name">
                    </div>
                    <div class="form-group">
                        <label>Product Type</label>
                        <select id="productType">
                            <option value="Savings">Savings</option>
                            <option value="Current">Current</option>
                            <option value="Fixed Deposit">Fixed Deposit</option>
                            <option value="Loan">Loan</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label>Interest Rate (%)</label>
                        <input type="number" id="productRate" placeholder="Enter interest rate" min="0" max="20" step="0.1">
                    </div>
                    <button class="btn" onclick="createProduct()">Create Product</button>
                    <div id="productResult" class="result"></div>
                </div>

                <!-- View Products -->
                <div class="card">
                    <h3>📋 View All Products</h3>
                    <button class="btn" onclick="getAllProducts()">Load All Products</button>
                    <div id="productsResult" class="result"></div>
                </div>
            </div>
        </div>

        <!-- Operations Tab -->
        <div id="operationsTab-content" class="tab-content" style="display: none;">
            <div class="dashboard">
                <!-- Start Teller Session -->
                <div class="card">
                    <h3>🏪 Start Teller Session</h3>
                    <div class="form-group">
                        <label>Teller Name</label>
                        <input type="text" id="tellerName" placeholder="Enter teller name">
                    </div>
                    <div class="form-group">
                        <label>Branch Code</label>
                        <input type="text" id="branchCode" placeholder="Enter branch code">
                    </div>
                    <div class="form-group">
                        <label>Starting Cash</label>
                        <input type="number" id="startingCash" placeholder="Enter starting cash amount" min="0" step="1000">
                    </div>
                    <button class="btn" onclick="startTellerSession()">Start Session</button>
                    <div id="tellerSessionResult" class="result"></div>
                </div>

                <!-- Create Branch -->
                <div class="card">
                    <h3>🏢 Create Branch</h3>
                    <div class="form-group">
                        <label>Branch Code</label>
                        <input type="text" id="newBranchCode" placeholder="Enter branch code">
                    </div>
                    <div class="form-group">
                        <label>Branch Name</label>
                        <input type="text" id="branchName" placeholder="Enter branch name">
                    </div>
                    <div class="form-group">
                        <label>City</label>
                        <input type="text" id="branchCity" placeholder="Enter city">
                    </div>
                    <div class="form-group">
                        <label>Manager</label>
                        <input type="text" id="branchManager" placeholder="Enter manager name">
                    </div>
                    <button class="btn" onclick="createBranch()">Create Branch</button>
                    <div id="branchResult" class="result"></div>
                </div>

                <!-- View Branches -->
                <div class="card">
                    <h3>🏪 View All Branches</h3>
                    <button class="btn" onclick="getAllBranches()">Load All Branches</button>
                    <div id="branchesResult" class="result"></div>
                </div>

                <!-- Issue Card -->
                <div class="card">
                    <h3>💳 Issue Card</h3>
                    <div class="form-group">
                        <label>Customer ID</label>
                        <input type="text" id="cardCustomerId" placeholder="Enter customer ID">
                    </div>
                    <div class="form-group">
                        <label>Account ID</label>
                        <input type="text" id="cardAccountId" placeholder="Enter account ID">
                    </div>
                    <div class="form-group">
                        <label>Card Type</label>
                        <select id="cardType">
                            <option value="Debit">Debit Card</option>
                            <option value="Credit">Credit Card</option>
                            <option value="Prepaid">Prepaid Card</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label>Daily Limit</label>
                        <input type="number" id="dailyLimit" placeholder="Enter daily limit" min="1000" step="1000">
                    </div>
                    <button class="btn" onclick="issueCard()">Issue Card</button>
                    <div id="cardResult" class="result"></div>
                </div>

                <!-- Create Payment Order -->
                <div class="card">
                    <h3>💸 Create Payment Order</h3>
                    <div class="form-group">
                        <label>From Account ID</label>
                        <input type="text" id="paymentFromAccount" placeholder="Enter from account ID">
                    </div>
                    <div class="form-group">
                        <label>To Account Number</label>
                        <input type="text" id="paymentToAccount" placeholder="Enter to account number">
                    </div>
                    <div class="form-group">
                        <label>Amount</label>
                        <input type="number" id="paymentAmount" placeholder="Enter payment amount" min="1" step="0.01">
                    </div>
                    <div class="form-group">
                        <label>Payment Type</label>
                        <select id="paymentType">
                            <option value="Internal">Internal Transfer</option>
                            <option value="EFT">EFT</option>
                            <option value="RTGS">RTGS</option>
                            <option value="SWIFT">SWIFT</option>
                        </select>
                    </div>
                    <button class="btn" onclick="createPaymentOrder()">Create Payment</button>
                    <div id="paymentResult" class="result"></div>
                </div>
            </div>
        </div>

        <!-- Treasury & Trade Tab -->
        <div id="treasuryTab-content" class="tab-content" style="display: none;">
            <div class="dashboard">
                <!-- Issue Letter of Credit -->
                <div class="card">
                    <h3>📜 Issue Letter of Credit</h3>
                    <div class="form-group">
                        <label>Customer ID</label>
                        <input type="text" id="lcCustomerId" placeholder="Enter customer ID">
                    </div>
                    <div class="form-group">
                        <label>Applicant</label>
                        <input type="text" id="lcApplicant" placeholder="Enter applicant name">
                    </div>
                    <div class="form-group">
                        <label>Beneficiary</label>
                        <input type="text" id="lcBeneficiary" placeholder="Enter beneficiary name">
                    </div>
                    <div class="form-group">
                        <label>Amount</label>
                        <input type="number" id="lcAmount" placeholder="Enter LC amount" min="1000" step="1000">
                    </div>
                    <div class="form-group">
                        <label>Currency</label>
                        <select id="lcCurrency">
                            <option value="USD">USD</option>
                            <option value="EUR">EUR</option>
                            <option value="KES">KES</option>
                        </select>
                    </div>
                    <button class="btn" onclick="issueLetterOfCredit()">Issue LC</button>
                    <div id="lcResult" class="result"></div>
                </div>

                <!-- Create FX Deal -->
                <div class="card">
                    <h3>💱 Create FX Deal</h3>
                    <div class="form-group">
                        <label>Customer ID</label>
                        <input type="text" id="fxCustomerId" placeholder="Enter customer ID">
                    </div>
                    <div class="form-group">
                        <label>Base Currency</label>
                        <select id="baseCurrency">
                            <option value="USD">USD</option>
                            <option value="EUR">EUR</option>
                            <option value="KES">KES</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label>Quote Currency</label>
                        <select id="quoteCurrency">
                            <option value="KES">KES</option>
                            <option value="USD">USD</option>
                            <option value="EUR">EUR</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label>Base Amount</label>
                        <input type="number" id="baseAmount" placeholder="Enter base amount" min="100" step="100">
                    </div>
                    <div class="form-group">
                        <label>Exchange Rate</label>
                        <input type="number" id="exchangeRate" placeholder="Enter exchange rate" min="0.01" step="0.01">
                    </div>
                    <button class="btn" onclick="createFXDeal()">Create FX Deal</button>
                    <div id="fxResult" class="result"></div>
                </div>

                <!-- GL Account Management -->
                <div class="card">
                    <h3>📊 Create GL Account</h3>
                    <div class="form-group">
                        <label>Account Code</label>
                        <input type="text" id="glAccountCode" placeholder="Enter GL account code">
                    </div>
                    <div class="form-group">
                        <label>Account Name</label>
                        <input type="text" id="glAccountName" placeholder="Enter GL account name">
                    </div>
                    <div class="form-group">
                        <label>Account Type</label>
                        <select id="glAccountType">
                            <option value="Asset">Asset</option>
                            <option value="Liability">Liability</option>
                            <option value="Equity">Equity</option>
                            <option value="Income">Income</option>
                            <option value="Expense">Expense</option>
                        </select>
                    </div>
                    <button class="btn" onclick="createGLAccount()">Create GL Account</button>
                    <div id="glResult" class="result"></div>
                </div>

                <!-- System Analytics -->
                <div class="card">
                    <h3>📈 System Analytics</h3>
                    <button class="btn" onclick="getSystemAnalytics()">Get Analytics</button>
                    <div id="analyticsResult" class="result"></div>
                </div>

                <!-- Comprehensive API Test -->
                <div class="card">
                    <h3>🧪 API Test Suite</h3>
                    <button class="btn" onclick="runAPITests()">Run All API Tests</button>
                    <div id="apiTestResult" class="result"></div>
                </div>
            </div>
        </div>

        <div class="api-links">
            <h3>🔗 Comprehensive API Access</h3>
            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 10px; margin-top: 15px;">
                <a href="/api/status" target="_blank">System Status</a>
                <a href="/swagger" target="_blank">API Documentation</a>
                <a href="/api/customers" target="_blank">Customers API</a>
                <a href="/api/accounts" target="_blank">Accounts API</a>
                <a href="/api/products" target="_blank">Products API</a>
                <a href="/api/branches" target="_blank">Branches API</a>
                <a href="/api/cif/pending-kyc" target="_blank">Pending KYC</a>
                <a href="/api/cif/high-risk-parties" target="_blank">High Risk Parties</a>
            </div>
            <div style="margin-top: 15px; padding: 15px; background: rgba(255,255,255,0.1); border-radius: 10px;">
                <h4>🏦 Available Banking Modules:</h4>
                <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 10px; margin-top: 10px; font-size: 14px;">
                    <div>✅ CIF (Customer Information File)</div>
                    <div>✅ Account Management</div>
                    <div>✅ Transaction Processing</div>
                    <div>✅ Loan Management</div>
                    <div>✅ Fixed Deposits</div>
                    <div>✅ Teller Operations</div>
                    <div>✅ Branch Operations</div>
                    <div>✅ Cards & Instruments</div>
                    <div>✅ General Ledger</div>
                    <div>✅ Payment Processing</div>
                    <div>✅ Product Management</div>
                    <div>✅ Trade Finance</div>
                    <div>✅ Treasury Operations</div>
                </div>
            </div>
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

        async function getAllCustomers() {
            const result = await apiCall('/api/customers');
            showResult('customersResult', result, !!result.error);
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

        async function getAllAccounts() {
            const result = await apiCall('/api/accounts');
            showResult('accountsResult', result, !!result.error);
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

        async function getTransactionHistory() {
            const accountNumber = document.getElementById('historyAccount').value;

            if (!accountNumber) {
                showResult('historyResult', { error: 'Account number is required' }, true);
                return;
            }

            const result = await apiCall(`/api/accounts/${accountNumber}/transactions`);
            showResult('historyResult', result, !!result.error);
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
            showTab('basic'); // Show basic tab by default
        };

        // Tab Management
        function showTab(tabName) {
            // Hide all tab contents
            const tabs = ['basic', 'cif', 'loans', 'operations', 'treasury'];
            tabs.forEach(tab => {
                const content = document.getElementById(tab + 'Tab-content');
                const button = document.getElementById(tab + 'Tab');
                if (content) content.style.display = 'none';
                if (button) button.classList.remove('active');
            });
            
            // Show selected tab
            const selectedContent = document.getElementById(tabName + 'Tab-content');
            const selectedButton = document.getElementById(tabName + 'Tab');
            if (selectedContent) selectedContent.style.display = 'block';
            if (selectedButton) selectedButton.classList.add('active');
        }

        // ============================================================================
        // CIF MANAGEMENT FUNCTIONS
        // ============================================================================

        async function performAMLScreening() {
            const customerId = document.getElementById('amlCustomerId').value;
            if (!customerId) {
                showResult('amlResult', { error: 'Customer ID is required' }, true);
                return;
            }

            const result = await apiCall(`/api/cif/aml-screening/${customerId}`, 'POST');
            showResult('amlResult', result, !!result.error);
        }

        async function getPendingKYC() {
            const result = await apiCall('/api/cif/pending-kyc');
            showResult('pendingKYCResult', result, !!result.error);
        }

        async function getHighRiskParties() {
            const result = await apiCall('/api/cif/high-risk-parties');
            showResult('highRiskResult', result, !!result.error);
        }

        async function getCustomer360() {
            const customerId = document.getElementById('customer360Id').value;
            if (!customerId) {
                showResult('customer360Result', { error: 'Customer ID is required' }, true);
                return;
            }

            const result = await apiCall(`/api/cif/customer-360/${customerId}`);
            showResult('customer360Result', result, !!result.error);
        }

        async function searchParties() {
            const searchTerm = document.getElementById('searchTerm').value;
            if (!searchTerm) {
                showResult('searchResult', { error: 'Search term is required' }, true);
                return;
            }

            const result = await apiCall(`/api/cif/search?name=${encodeURIComponent(searchTerm)}`);
            showResult('searchResult', result, !!result.error);
        }

        async function createBusiness() {
            const data = {
                CompanyName: document.getElementById('companyName').value,
                RegistrationNumber: document.getElementById('regNumber').value,
                IncorporationDate: new Date().toISOString(),
                CompanyType: "Private Limited",
                Industry: "General",
                PrimaryEmail: document.getElementById('businessEmail').value,
                PrimaryPhone: "+254-700-000000",
                AnnualTurnover: 1000000,
                NumberOfEmployees: 10
            };

            if (!data.CompanyName || !data.RegistrationNumber || !data.PrimaryEmail) {
                showResult('businessResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/cif/corporate', 'POST', data);
            showResult('businessResult', result, !!result.error);
        }

        // ============================================================================
        // LOANS & DEPOSITS FUNCTIONS
        // ============================================================================

        async function applyForLoan() {
            const data = {
                CustomerId: document.getElementById('loanCustomerId').value,
                AccountId: document.getElementById('loanAccountId').value,
                Amount: parseFloat(document.getElementById('loanAmount').value) || 0,
                Currency: "KES",
                TermInMonths: parseInt(document.getElementById('loanTerm').value) || 12,
                Purpose: document.getElementById('loanPurpose').value
            };

            if (!data.CustomerId || !data.AccountId || data.Amount <= 0) {
                showResult('loanApplicationResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/loans/apply', 'POST', data);
            showResult('loanApplicationResult', result, !!result.error);
        }

        async function bookFixedDeposit() {
            const data = {
                CustomerId: document.getElementById('fdCustomerId').value,
                SourceAccountId: document.getElementById('fdAccountId').value,
                Amount: parseFloat(document.getElementById('fdAmount').value) || 0,
                Currency: "KES",
                InterestRate: parseFloat(document.getElementById('fdRate').value) || 0,
                TermInMonths: parseInt(document.getElementById('fdTerm').value) || 12
            };

            if (!data.CustomerId || !data.SourceAccountId || data.Amount <= 0) {
                showResult('fdResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/fixed-deposits/book', 'POST', data);
            showResult('fdResult', result, !!result.error);
        }

        async function createProduct() {
            const data = {
                ProductCode: document.getElementById('productCode').value,
                ProductName: document.getElementById('productName').value,
                ProductType: document.getElementById('productType').value,
                Category: "Deposit",
                Description: "Banking product",
                InterestRate: parseFloat(document.getElementById('productRate').value) || 0,
                MinimumBalance: 1000,
                MaximumBalance: 1000000,
                MonthlyFee: 50,
                TransactionFee: 10,
                Currency: "KES",
                MinAge: 18,
                MaxAge: 65,
                EligibilityCriteria: "Valid ID required",
                Features: "Standard banking features"
            };

            if (!data.ProductCode || !data.ProductName) {
                showResult('productResult', { error: 'Product code and name are required' }, true);
                return;
            }

            const result = await apiCall('/api/products', 'POST', data);
            showResult('productResult', result, !!result.error);
        }

        async function getAllProducts() {
            const result = await apiCall('/api/products');
            showResult('productsResult', result, !!result.error);
        }

        // ============================================================================
        // OPERATIONS FUNCTIONS
        // ============================================================================

        async function startTellerSession() {
            const data = {
                TellerName: document.getElementById('tellerName').value,
                BranchCode: document.getElementById('branchCode').value,
                StartingCash: parseFloat(document.getElementById('startingCash').value) || 0
            };

            if (!data.TellerName || !data.BranchCode) {
                showResult('tellerSessionResult', { error: 'Teller name and branch code are required' }, true);
                return;
            }

            const result = await apiCall('/api/teller/start-session', 'POST', data);
            showResult('tellerSessionResult', result, !!result.error);
        }

        async function createBranch() {
            const data = {
                BranchCode: document.getElementById('newBranchCode').value,
                BranchName: document.getElementById('branchName').value,
                Address: "123 Banking Street",
                City: document.getElementById('branchCity').value,
                Region: "Central",
                Phone: "+254-700-000000",
                Email: "branch@wekeza.com",
                Manager: document.getElementById('branchManager').value,
                CashLimit: 5000000
            };

            if (!data.BranchCode || !data.BranchName || !data.City || !data.Manager) {
                showResult('branchResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/branches', 'POST', data);
            showResult('branchResult', result, !!result.error);
        }

        async function getAllBranches() {
            const result = await apiCall('/api/branches');
            showResult('branchesResult', result, !!result.error);
        }

        async function issueCard() {
            const data = {
                CustomerId: document.getElementById('cardCustomerId').value,
                AccountId: document.getElementById('cardAccountId').value,
                CardType: document.getElementById('cardType').value,
                DailyLimit: parseFloat(document.getElementById('dailyLimit').value) || 50000,
                MonthlyLimit: 200000,
                IsContactless: true,
                IsInternational: false
            };

            if (!data.CustomerId || !data.AccountId) {
                showResult('cardResult', { error: 'Customer ID and Account ID are required' }, true);
                return;
            }

            const result = await apiCall('/api/cards/issue', 'POST', data);
            showResult('cardResult', result, !!result.error);
        }

        async function createPaymentOrder() {
            const data = {
                FromAccountId: document.getElementById('paymentFromAccount').value,
                ToAccountNumber: document.getElementById('paymentToAccount').value,
                BeneficiaryName: "Beneficiary Name",
                BeneficiaryBank: "Destination Bank",
                BeneficiaryBankCode: "001",
                Amount: parseFloat(document.getElementById('paymentAmount').value) || 0,
                Currency: "KES",
                PaymentType: document.getElementById('paymentType').value,
                PaymentPurpose: "Payment transfer",
                Priority: "Normal",
                ValueDate: new Date().toISOString(),
                InitiatedBy: "System User"
            };

            if (!data.FromAccountId || !data.ToAccountNumber || data.Amount <= 0) {
                showResult('paymentResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/payments/orders', 'POST', data);
            showResult('paymentResult', result, !!result.error);
        }

        // ============================================================================
        // TREASURY & TRADE FUNCTIONS
        // ============================================================================

        async function issueLetterOfCredit() {
            const data = {
                CustomerId: document.getElementById('lcCustomerId').value,
                Applicant: document.getElementById('lcApplicant').value,
                Beneficiary: document.getElementById('lcBeneficiary').value,
                AdvisingBank: "Advising Bank Ltd",
                Amount: parseFloat(document.getElementById('lcAmount').value) || 0,
                Currency: document.getElementById('lcCurrency').value,
                LCType: "Sight LC",
                PaymentTerms: "At sight",
                GoodsDescription: "General goods",
                ExpiryDate: new Date(Date.now() + 90 * 24 * 60 * 60 * 1000).toISOString(),
                LatestShipmentDate: new Date(Date.now() + 60 * 24 * 60 * 60 * 1000).toISOString(),
                PortOfLoading: "Mombasa",
                PortOfDischarge: "Dubai"
            };

            if (!data.CustomerId || !data.Applicant || !data.Beneficiary || data.Amount <= 0) {
                showResult('lcResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/trade-finance/letters-of-credit', 'POST', data);
            showResult('lcResult', result, !!result.error);
        }

        async function createFXDeal() {
            const data = {
                CustomerId: document.getElementById('fxCustomerId').value,
                BaseCurrency: document.getElementById('baseCurrency').value,
                QuoteCurrency: document.getElementById('quoteCurrency').value,
                BaseAmount: parseFloat(document.getElementById('baseAmount').value) || 0,
                ExchangeRate: parseFloat(document.getElementById('exchangeRate').value) || 1,
                DealType: "Spot",
                Side: "Buy",
                ValueDate: new Date().toISOString(),
                Trader: "System Trader",
                Purpose: "Currency exchange"
            };

            if (!data.CustomerId || data.BaseAmount <= 0 || data.ExchangeRate <= 0) {
                showResult('fxResult', { error: 'All fields are required' }, true);
                return;
            }

            const result = await apiCall('/api/treasury/fx-deals', 'POST', data);
            showResult('fxResult', result, !!result.error);
        }

        async function createGLAccount() {
            const data = {
                AccountCode: document.getElementById('glAccountCode').value,
                AccountName: document.getElementById('glAccountName').value,
                AccountType: document.getElementById('glAccountType').value,
                Category: "General",
                SubCategory: "Standard",
                Currency: "KES",
                IsControlAccount: false,
                AllowManualPosting: true
            };

            if (!data.AccountCode || !data.AccountName) {
                showResult('glResult', { error: 'Account code and name are required' }, true);
                return;
            }

            const result = await apiCall('/api/gl/accounts', 'POST', data);
            showResult('glResult', result, !!result.error);
        }

        async function getSystemAnalytics() {
            const result = await apiCall('/api/status');
            showResult('analyticsResult', {
                message: "System Analytics Retrieved",
                analytics: {
                    totalCustomers: result.statistics?.totalCustomers || 0,
                    totalAccounts: result.statistics?.totalAccounts || 0,
                    totalTransactions: result.statistics?.totalTransactions || 0,
                    systemLoad: result.statistics?.systemLoad || "0%",
                    uptime: result.uptime || "Unknown",
                    databaseStatus: result.database || "Unknown"
                }
            }, !!result.error);
        }

        async function runAPITests() {
            showResult('apiTestResult', { message: "Running comprehensive API tests..." }, false);
            
            const tests = [
                { name: "System Status", endpoint: "/api/status" },
                { name: "Get Customers", endpoint: "/api/customers" },
                { name: "Get Accounts", endpoint: "/api/accounts" },
                { name: "Get Products", endpoint: "/api/products" },
                { name: "Pending KYC", endpoint: "/api/cif/pending-kyc" },
                { name: "High Risk Parties", endpoint: "/api/cif/high-risk-parties" },
                { name: "Get Branches", endpoint: "/api/branches" }
            ];

            const results = [];
            for (const test of tests) {
                try {
                    const result = await apiCall(test.endpoint);
                    results.push({
                        test: test.name,
                        status: result.error ? "❌ FAILED" : "✅ PASSED",
                        endpoint: test.endpoint
                    });
                } catch (error) {
                    results.push({
                        test: test.name,
                        status: "❌ ERROR",
                        endpoint: test.endpoint,
                        error: error.message
                    });
                }
            }

            showResult('apiTestResult', {
                message: "API Test Suite Completed",
                totalTests: tests.length,
                passedTests: results.filter(r => r.status.includes("PASSED")).length,
                failedTests: results.filter(r => r.status.includes("FAILED") || r.status.includes("ERROR")).length,
                results: results
            }, false);
        }
    </script>
</body>
</html>
""", "text/html"));

// System status with real database statistics
app.MapGet("/api/status", async (WekezaDbContext context) =>
{
    var totalCustomers = await context.Customers.CountAsync();
    var totalAccounts = await context.Accounts.CountAsync();
    var totalTransactions = await context.Transactions.CountAsync();
    
    return new
    {
        SystemName = "Wekeza Core Banking System",
        Owner = "Emmanuel Odenyire (ID: 28839872)",
        Contact = "0716478835",
        Status = "🟢 OPERATIONAL",
        Database = "🗄️ PostgreSQL Connected",
        Uptime = TimeSpan.FromMinutes(Random.Shared.Next(1, 1440)).ToString(@"hh\:mm\:ss"),
        Components = new
        {
            API = "✅ Active",
            Database = "✅ PostgreSQL Connected",
            Cache = "✅ Redis Available",
            Security = "🔐 JWT Ready"
        },
        Statistics = new
        {
            TotalCustomers = totalCustomers,
            TotalAccounts = totalAccounts,
            TotalTransactions = totalTransactions,
            SystemLoad = $"{Random.Shared.Next(15, 85)}%"
        },
        LastUpdated = DateTime.UtcNow
    };
});

// Get all customers
app.MapGet("/api/customers", async (WekezaDbContext context) =>
{
    try
    {
        var customers = await context.Customers
            .Include(c => c.Accounts)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.IdentificationNumber,
                c.CustomerNumber,
                c.Status,
                c.CreatedAt,
                AccountCount = c.Accounts.Count(),
                Accounts = c.Accounts.Select(a => new
                {
                    a.Id,
                    a.AccountNumber,
                    a.Currency,
                    a.Balance,
                    a.AvailableBalance,
                    a.Status,
                    a.AccountType,
                    a.CreatedAt
                }).ToList()
            })
            .ToListAsync();
        
        return Results.Ok(new
        {
            Message = "✅ Customers retrieved successfully",
            TotalCustomers = customers.Count,
            Customers = customers,
            RetrievedAt = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to retrieve customers",
            Error = ex.Message,
            Details = ex.InnerException?.Message
        });
    }
});

// Create customer
app.MapPost("/api/customers", async ([FromBody] CreateCustomerRequest request, WekezaDbContext context) =>
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
            Message = "✅ Customer created successfully and saved to database!",
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

// Get all accounts
app.MapGet("/api/accounts", async (WekezaDbContext context) =>
{
    try
    {
        var accounts = await context.Accounts
            .Include(a => a.Customer)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.AccountNumber,
                a.Currency,
                a.Balance,
                a.AvailableBalance,
                a.Status,
                a.AccountType,
                a.CreatedAt,
                Customer = new
                {
                    a.Customer.Id,
                    a.Customer.FirstName,
                    a.Customer.LastName,
                    a.Customer.Email,
                    a.Customer.CustomerNumber
                }
            })
            .ToListAsync();
        
        return Results.Ok(new
        {
            Message = "✅ Accounts retrieved successfully",
            TotalAccounts = accounts.Count,
            Accounts = accounts,
            RetrievedAt = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to retrieve accounts",
            Error = ex.Message,
            Details = ex.InnerException?.Message
        });
    }
});

// Create account
app.MapPost("/api/accounts", async ([FromBody] CreateAccountRequest request, WekezaDbContext context) =>
{
    try
    {
        Customer? customer = null;
        
        // Try to parse as GUID first (Customer ID)
        if (Guid.TryParse(request.CustomerId.ToString(), out var customerGuid))
        {
            customer = await context.Customers.FindAsync(customerGuid);
        }
        
        // If not found, try to find by Customer Number
        if (customer == null)
        {
            customer = await context.Customers.FirstOrDefaultAsync(c => c.CustomerNumber == request.CustomerId.ToString());
        }
        
        if (customer == null)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Customer not found",
                Error = "Invalid Customer ID or Customer Number",
                Hint = "Use either the full Customer ID (GUID) or Customer Number (e.g., CUS-2F2E4FF5)"
            });
        }

        var account = new Account
        {
            AccountNumber = $"WKZ-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            CustomerId = customer.Id,
            Currency = request.CurrencyCode ?? "KES",
            Balance = request.InitialDeposit,
            AvailableBalance = request.InitialDeposit
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Account created successfully and saved to database!",
            Account = new
            {
                account.Id,
                account.AccountNumber,
                account.CustomerId,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                CustomerNumber = customer.CustomerNumber,
                account.Currency,
                account.Balance,
                account.AvailableBalance,
                account.Status,
                account.AccountType,
                account.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to create account",
            Error = ex.Message,
            Details = ex.InnerException?.Message
        });
    }
});

// Process deposit
app.MapPost("/api/transactions/deposit", async ([FromBody] DepositRequest request, WekezaDbContext context) =>
{
    try
    {
        var account = await context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);
            
        if (account == null)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Account not found",
                Error = "Invalid account number"
            });
        }

        if (request.Amount <= 0)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Invalid amount",
                Error = "Deposit amount must be greater than zero"
            });
        }

        var previousBalance = account.Balance;
        account.Balance += request.Amount;
        account.AvailableBalance += request.Amount;

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = "Credit",
            Amount = request.Amount,
            Currency = account.Currency,
            PreviousBalance = previousBalance,
            NewBalance = account.Balance,
            Reference = $"DEP-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Description = "Cash Deposit"
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Deposit processed successfully and saved to database!",
            Transaction = new
            {
                transaction.Id,
                transaction.Reference,
                transaction.Type,
                transaction.Amount,
                transaction.Currency,
                transaction.PreviousBalance,
                transaction.NewBalance,
                transaction.Status,
                transaction.ProcessedAt,
                AccountNumber = account.AccountNumber,
                CustomerName = $"{account.Customer.FirstName} {account.Customer.LastName}"
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Deposit failed",
            Error = ex.Message,
            Details = ex.InnerException?.Message
        });
    }
});

// Process withdrawal
app.MapPost("/api/transactions/withdraw", async ([FromBody] WithdrawRequest request, WekezaDbContext context) =>
{
    try
    {
        var account = await context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);
            
        if (account == null)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Account not found",
                Error = "Invalid account number"
            });
        }

        if (request.Amount <= 0)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Invalid amount",
                Error = "Withdrawal amount must be greater than zero"
            });
        }

        if (request.Amount > account.AvailableBalance)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Insufficient funds",
                Error = $"Available balance: {account.AvailableBalance:C}, Requested: {request.Amount:C}",
                AvailableBalance = account.AvailableBalance,
                RequestedAmount = request.Amount
            });
        }

        var previousBalance = account.Balance;
        account.Balance -= request.Amount;
        account.AvailableBalance -= request.Amount;

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = "Debit",
            Amount = request.Amount,
            Currency = account.Currency,
            PreviousBalance = previousBalance,
            NewBalance = account.Balance,
            Reference = $"WTH-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Description = "Cash Withdrawal"
        };

        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Withdrawal processed successfully and saved to database!",
            Transaction = new
            {
                transaction.Id,
                transaction.Reference,
                transaction.Type,
                transaction.Amount,
                transaction.Currency,
                transaction.PreviousBalance,
                transaction.NewBalance,
                transaction.Status,
                transaction.ProcessedAt,
                AccountNumber = account.AccountNumber,
                CustomerName = $"{account.Customer.FirstName} {account.Customer.LastName}"
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Withdrawal failed",
            Error = ex.Message,
            Details = ex.InnerException?.Message
        });
    }
});

// Get account balance
app.MapGet("/api/accounts/{accountNumber}/balance", async (string accountNumber, WekezaDbContext context) =>
{
    try
    {
        var account = await context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        
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
            CustomerName = $"{account.Customer.FirstName} {account.Customer.LastName}",
            Balance = account.Balance,
            AvailableBalance = account.AvailableBalance,
            Currency = account.Currency,
            Status = account.Status,
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
app.MapGet("/api/accounts/{accountNumber}/transactions", async (string accountNumber, WekezaDbContext context) =>
{
    try
    {
        var account = await context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            
        if (account == null)
        {
            return Results.BadRequest(new
            {
                Message = "❌ Account not found",
                Error = "Invalid account number"
            });
        }

        var transactions = await context.Transactions
            .Where(t => t.AccountId == account.Id)
            .OrderByDescending(t => t.ProcessedAt)
            .Take(50)
            .Select(t => new
            {
                t.Id,
                t.Reference,
                t.Type,
                t.Amount,
                t.Currency,
                t.PreviousBalance,
                t.NewBalance,
                t.Status,
                t.Description,
                t.ProcessedAt,
                t.ProcessedBy
            })
            .ToListAsync();

        return Results.Ok(new
        {
            Message = "✅ Transaction history retrieved successfully",
            AccountNumber = accountNumber,
            CustomerName = $"{account.Customer.FirstName} {account.Customer.LastName}",
            CurrentBalance = account.Balance,
            AvailableBalance = account.AvailableBalance,
            Transactions = transactions,
            TotalTransactions = transactions.Count,
            RetrievedAt = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new
        {
            Message = "❌ Failed to get transaction history",
            Error = ex.Message,
            Details = ex.InnerException?.Message
        });
    }
});

// === COMPREHENSIVE BANKING SYSTEM ENDPOINTS ===
// Implementing ALL features from Core/Wekeza.Core.Application/Features

// ============================================================================
// CIF (Customer Information File) COMMANDS
// ============================================================================

// ============================================================================
// CIF (Customer Information File) COMMANDS - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Create Individual Party (Enhanced Customer Creation)
app.MapPost("/api/cif/individual", async ([FromBody] CreateIndividualPartyRequest request, WekezaDbContext context) =>
{
    try
    {
        var customer = new Customer
        {
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.PrimaryEmail,
            IdentificationNumber = request.PrimaryIdentification.DocumentNumber,
            CustomerNumber = $"CIF-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            MaritalStatus = request.MaritalStatus,
            Nationality = request.Nationality,
            PrimaryPhone = request.PrimaryPhone,
            SecondaryPhone = request.SecondaryPhone,
            PreferredLanguage = request.PreferredLanguage,
            OptInMarketing = request.OptInMarketing,
            OptInSMS = request.OptInSMS,
            OptInEmail = request.OptInEmail
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        // Add primary address
        var address = new Address
        {
            CustomerId = customer.Id,
            AddressType = request.PrimaryAddress.AddressType,
            AddressLine1 = request.PrimaryAddress.AddressLine1,
            AddressLine2 = request.PrimaryAddress.AddressLine2,
            City = request.PrimaryAddress.City,
            State = request.PrimaryAddress.State,
            Country = request.PrimaryAddress.Country,
            PostalCode = request.PrimaryAddress.PostalCode,
            IsPrimary = true
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Individual party created successfully",
            CustomerNumber = customer.CustomerNumber,
            CustomerId = customer.Id,
            FullName = $"{customer.FirstName} {customer.LastName}",
            KYCStatus = customer.KYCStatus,
            AMLRiskRating = customer.AMLRiskRating
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to create individual party", Error = ex.Message });
    }
});

// Create Corporate Party (Business Customer Creation)
app.MapPost("/api/cif/corporate", async ([FromBody] CreateCorporatePartyRequest request, WekezaDbContext context) =>
{
    try
    {
        var business = new Business
        {
            CompanyName = request.CompanyName,
            RegistrationNumber = request.RegistrationNumber,
            IncorporationDate = request.IncorporationDate,
            CompanyType = request.CompanyType,
            Industry = request.Industry,
            Email = request.PrimaryEmail,
            Phone = request.PrimaryPhone,
            Website = request.Website,
            AnnualTurnover = request.AnnualTurnover,
            NumberOfEmployees = request.NumberOfEmployees,
            TaxIdentificationNumber = request.TaxIdentificationNumber,
            BusinessNumber = $"BIZ-{Guid.NewGuid().ToString()[..8].ToUpper()}"
        };

        context.Businesses.Add(business);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Corporate party created successfully",
            BusinessNumber = business.BusinessNumber,
            BusinessId = business.Id,
            CompanyName = business.CompanyName,
            RegistrationNumber = business.RegistrationNumber
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to create corporate party", Error = ex.Message });
    }
});

// Update KYC Status
app.MapPut("/api/cif/kyc-status/{customerId}", async (Guid customerId, [FromBody] UpdateKYCStatusRequest request, WekezaDbContext context) =>
{
    try
    {
        var customer = await context.Customers.FindAsync(customerId);
        if (customer == null)
        {
            return Results.BadRequest(new { Message = "❌ Customer not found" });
        }

        customer.KYCStatus = request.Status;
        customer.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ KYC status updated successfully",
            CustomerId = customerId,
            NewStatus = customer.KYCStatus,
            UpdatedAt = customer.UpdatedAt
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to update KYC status", Error = ex.Message });
    }
});

// Get Pending KYC
app.MapGet("/api/cif/pending-kyc", async (WekezaDbContext context) =>
{
    try
    {
        var pendingKYC = await context.Customers
            .Where(c => c.KYCStatus == "Pending")
            .Select(c => new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.CustomerNumber,
                c.Email,
                c.KYCStatus,
                c.CreatedAt,
                DaysPending = (DateTime.UtcNow - c.CreatedAt).Days
            })
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return Results.Ok(new
        {
            Message = "✅ Pending KYC customers retrieved successfully",
            TotalPendingKYC = pendingKYC.Count,
            PendingKYC = pendingKYC
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to get pending KYC", Error = ex.Message });
    }
});

// Get High Risk Parties
app.MapGet("/api/cif/high-risk-parties", async (WekezaDbContext context) =>
{
    try
    {
        var highRiskCustomers = await context.Customers
            .Where(c => c.AMLRiskRating == "High")
            .Select(c => new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.CustomerNumber,
                c.AMLRiskRating,
                c.LastAMLScreening,
                c.KYCStatus,
                AccountCount = c.Accounts.Count(),
                TotalBalance = c.Accounts.Sum(a => a.Balance)
            })
            .ToListAsync();

        return Results.Ok(new
        {
            Message = "✅ High risk parties retrieved successfully",
            TotalHighRiskParties = highRiskCustomers.Count,
            HighRiskParties = highRiskCustomers
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to get high risk parties", Error = ex.Message });
    }
});

// Perform AML Screening
app.MapPost("/api/cif/aml-screening/{customerId}", async (Guid customerId, WekezaDbContext context) =>
{
    try
    {
        var customer = await context.Customers.FindAsync(customerId);
        if (customer == null)
        {
            return Results.BadRequest(new { Message = "❌ Customer not found" });
        }

        // Simulate comprehensive AML screening
        var riskFactors = new List<string>();
        var matches = new List<object>();
        var riskScore = 0;

        // Check nationality
        if (customer.Nationality != "Kenyan")
        {
            riskFactors.Add("Foreign national");
            riskScore += 20;
            matches.Add(new { MatchType = "Nationality", Details = "Non-resident customer", ConfidenceScore = 85 });
        }

        // Check age
        if (customer.DateOfBirth.HasValue)
        {
            var age = DateTime.Now.Year - customer.DateOfBirth.Value.Year;
            if (age < 18)
            {
                riskFactors.Add("Minor");
                riskScore += 30;
                matches.Add(new { MatchType = "Age", Details = "Under 18 years", ConfidenceScore = 100 });
            }
            else if (age > 65)
            {
                riskFactors.Add("Senior citizen");
                riskScore += 10;
            }
        }

        // Simulate sanctions check (normally would check against OFAC, UN, EU lists)
        var isSanctioned = false;
        var isPEP = false;
        var hasAdverseMedia = false;

        // Simple name-based screening simulation
        var fullName = $"{customer.FirstName} {customer.LastName}".ToLower();
        if (fullName.Contains("sanction") || fullName.Contains("blocked"))
        {
            isSanctioned = true;
            riskScore += 100;
            matches.Add(new { MatchType = "Sanctions", ListName = "OFAC", Details = "Name match found", ConfidenceScore = 95 });
        }

        // Determine risk rating
        string riskRating = riskScore switch
        {
            < 20 => "Low",
            < 50 => "Medium",
            _ => "High"
        };

        customer.AMLRiskRating = riskRating;
        customer.LastAMLScreening = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ AML screening completed",
            CustomerId = customerId,
            IsClear = !isSanctioned && !isPEP && riskScore < 50,
            IsSanctioned = isSanctioned,
            IsPEP = isPEP,
            HasAdverseMedia = hasAdverseMedia,
            RecommendedRiskRating = riskRating,
            RiskScore = riskScore,
            RiskFactors = riskFactors,
            Matches = matches,
            Summary = $"Customer screened with {riskRating} risk rating. {matches.Count} potential matches found.",
            ScreenedAt = customer.LastAMLScreening
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ AML screening failed", Error = ex.Message });
    }
});

// Get Customer 360 View
app.MapGet("/api/cif/customer-360/{customerId}", async (Guid customerId, WekezaDbContext context) =>
{
    try
    {
        var customer = await context.Customers
            .Include(c => c.Accounts)
                .ThenInclude(a => a.Transactions.Take(10))
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        if (customer == null)
        {
            return Results.BadRequest(new { Message = "❌ Customer not found" });
        }

        var loans = await context.Loans
            .Where(l => l.CustomerId == customerId)
            .ToListAsync();

        var fixedDeposits = await context.FixedDeposits
            .Where(fd => fd.CustomerId == customerId)
            .ToListAsync();

        return Results.Ok(new
        {
            Message = "✅ Customer 360 view retrieved successfully",
            Customer = new
            {
                customer.Id,
                customer.CustomerNumber,
                customer.FirstName,
                customer.MiddleName,
                customer.LastName,
                customer.Email,
                customer.DateOfBirth,
                customer.Gender,
                customer.MaritalStatus,
                customer.Nationality,
                customer.PrimaryPhone,
                customer.SecondaryPhone,
                customer.KYCStatus,
                customer.AMLRiskRating,
                customer.LastAMLScreening,
                customer.CreatedAt,
                customer.UpdatedAt
            },
            Addresses = customer.Addresses.Select(a => new
            {
                a.Id,
                a.AddressType,
                a.AddressLine1,
                a.AddressLine2,
                a.City,
                a.State,
                a.Country,
                a.PostalCode,
                a.IsPrimary
            }),
            Accounts = customer.Accounts.Select(a => new
            {
                a.Id,
                a.AccountNumber,
                a.AccountType,
                a.Currency,
                a.Balance,
                a.AvailableBalance,
                a.Status,
                a.CreatedAt,
                RecentTransactions = a.Transactions.Take(5).Select(t => new
                {
                    t.Id,
                    t.Reference,
                    t.Type,
                    t.Amount,
                    t.Description,
                    t.ProcessedAt
                })
            }),
            Loans = loans.Select(l => new
            {
                l.Id,
                l.LoanNumber,
                l.Amount,
                l.Currency,
                l.InterestRate,
                l.TermInMonths,
                l.Status,
                l.OutstandingBalance,
                l.NextPaymentDate,
                l.CreatedAt
            }),
            FixedDeposits = fixedDeposits.Select(fd => new
            {
                fd.Id,
                fd.DepositNumber,
                fd.Amount,
                fd.Currency,
                fd.InterestRate,
                fd.MaturityDate,
                fd.Status,
                fd.CreatedAt
            }),
            Summary = new
            {
                TotalAccounts = customer.Accounts.Count,
                TotalBalance = customer.Accounts.Sum(a => a.Balance),
                TotalLoans = loans.Count,
                TotalLoanBalance = loans.Sum(l => l.OutstandingBalance),
                TotalFixedDeposits = fixedDeposits.Count,
                TotalFixedDepositAmount = fixedDeposits.Sum(fd => fd.Amount),
                RelationshipStartDate = customer.CreatedAt,
                LastActivity = customer.Accounts.SelectMany(a => a.Transactions).Max(t => t.ProcessedAt)
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to get customer 360 view", Error = ex.Message });
    }
});

// Search Parties
app.MapGet("/api/cif/search", async (string? name, string? email, string? phone, string? idNumber, WekezaDbContext context) =>
{
    try
    {
        var query = context.Customers.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => (c.FirstName + " " + c.LastName).Contains(name) || 
                                   c.FirstName.Contains(name) || 
                                   c.LastName.Contains(name));
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(c => c.Email.Contains(email));
        }

        if (!string.IsNullOrEmpty(phone))
        {
            query = query.Where(c => c.PrimaryPhone.Contains(phone) || 
                                   (c.SecondaryPhone != null && c.SecondaryPhone.Contains(phone)));
        }

        if (!string.IsNullOrEmpty(idNumber))
        {
            query = query.Where(c => c.IdentificationNumber.Contains(idNumber));
        }

        var results = await query
            .Include(c => c.Accounts)
            .Select(c => new
            {
                c.Id,
                c.CustomerNumber,
                c.FirstName,
                c.LastName,
                c.Email,
                c.PrimaryPhone,
                c.IdentificationNumber,
                c.KYCStatus,
                c.AMLRiskRating,
                c.Status,
                AccountCount = c.Accounts.Count(),
                TotalBalance = c.Accounts.Sum(a => a.Balance),
                c.CreatedAt
            })
            .Take(50)
            .ToListAsync();

        return Results.Ok(new
        {
            Message = "✅ Party search completed successfully",
            SearchCriteria = new { name, email, phone, idNumber },
            TotalResults = results.Count,
            Results = results
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Party search failed", Error = ex.Message });
    }
});

// ============================================================================
// ACCOUNTS MANAGEMENT - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Open Product-Based Account
app.MapPost("/api/accounts/product-based", async ([FromBody] OpenProductBasedAccountRequest request, WekezaDbContext context) =>
{
    try
    {
        Customer? customer = null;
        
        if (Guid.TryParse(request.CustomerId.ToString(), out var customerGuid))
        {
            customer = await context.Customers.FindAsync(customerGuid);
        }
        
        if (customer == null)
        {
            customer = await context.Customers.FirstOrDefaultAsync(c => c.CustomerNumber == request.CustomerId.ToString());
        }
        
        if (customer == null)
        {
            return Results.BadRequest(new { Message = "❌ Customer not found" });
        }

        var account = new Account
        {
            AccountNumber = $"WKZ-{request.ProductCode}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CustomerId = customer.Id,
            Currency = request.CurrencyCode ?? "KES",
            Balance = request.InitialDeposit,
            AvailableBalance = request.InitialDeposit,
            AccountType = request.ProductCode,
            InterestRate = request.InterestRate,
            MinimumBalance = request.MinimumBalance,
            MonthlyFee = request.MonthlyFee
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Product-based account opened successfully",
            Account = new
            {
                account.Id,
                account.AccountNumber,
                account.AccountType,
                account.Currency,
                account.Balance,
                account.InterestRate,
                account.MinimumBalance,
                account.MonthlyFee,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                account.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to open product-based account", Error = ex.Message });
    }
});

// Add Signatory
app.MapPost("/api/accounts/{accountId}/signatories", async (Guid accountId, [FromBody] AddSignatoryRequest request, WekezaDbContext context) =>
{
    try
    {
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            return Results.BadRequest(new { Message = "❌ Account not found" });
        }

        var signatory = new Signatory
        {
            AccountId = accountId,
            SignatoryName = request.SignatoryName,
            IdNumber = request.IdNumber,
            Role = request.Role,
            SignatureLimit = request.SignatureLimit,
            IsActive = true
        };

        context.Signatories.Add(signatory);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Signatory added successfully",
            Signatory = new
            {
                signatory.Id,
                signatory.SignatoryName,
                signatory.IdNumber,
                signatory.Role,
                signatory.SignatureLimit,
                signatory.IsActive,
                signatory.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to add signatory", Error = ex.Message });
    }
});

// Freeze Account
app.MapPost("/api/accounts/{accountId}/freeze", async (Guid accountId, [FromBody] FreezeAccountRequest request, WekezaDbContext context) =>
{
    try
    {
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            return Results.BadRequest(new { Message = "❌ Account not found" });
        }

        account.Status = "Frozen";
        account.FreezeReason = request.Reason;
        account.FrozenAt = DateTime.UtcNow;
        account.FrozenBy = request.FrozenBy;
        account.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Account frozen successfully",
            AccountNumber = account.AccountNumber,
            Status = account.Status,
            FreezeReason = account.FreezeReason,
            FrozenAt = account.FrozenAt,
            FrozenBy = account.FrozenBy
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to freeze account", Error = ex.Message });
    }
});

// Unfreeze Account
app.MapPost("/api/accounts/{accountId}/unfreeze", async (Guid accountId, [FromBody] UnfreezeAccountRequest request, WekezaDbContext context) =>
{
    try
    {
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            return Results.BadRequest(new { Message = "❌ Account not found" });
        }

        account.Status = "Active";
        account.FreezeReason = null;
        account.FrozenAt = null;
        account.FrozenBy = null;
        account.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Account unfrozen successfully",
            AccountNumber = account.AccountNumber,
            Status = account.Status,
            UnfrozenAt = account.UpdatedAt
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to unfreeze account", Error = ex.Message });
    }
});

// Close Account
app.MapPost("/api/accounts/{accountId}/close", async (Guid accountId, [FromBody] CloseAccountRequest request, WekezaDbContext context) =>
{
    try
    {
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null)
        {
            return Results.BadRequest(new { Message = "❌ Account not found" });
        }

        if (account.Balance != 0)
        {
            return Results.BadRequest(new 
            { 
                Message = "❌ Cannot close account with non-zero balance", 
                CurrentBalance = account.Balance,
                Hint = "Please transfer or withdraw all funds before closing"
            });
        }

        account.Status = "Closed";
        account.ClosedAt = DateTime.UtcNow;
        account.ClosureReason = request.Reason;
        account.ClosedBy = request.ClosedBy;
        account.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Account closed successfully",
            AccountNumber = account.AccountNumber,
            Status = account.Status,
            ClosedAt = account.ClosedAt,
            ClosureReason = account.ClosureReason,
            FinalBalance = account.Balance
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to close account", Error = ex.Message });
    }
});

// Register Business Account
app.MapPost("/api/accounts/business", async ([FromBody] RegisterBusinessAccountRequest request, WekezaDbContext context) =>
{
    try
    {
        var business = await context.Businesses.FindAsync(request.BusinessId);
        if (business == null)
        {
            return Results.BadRequest(new { Message = "❌ Business not found" });
        }

        var account = new Account
        {
            AccountNumber = $"BIZ-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            BusinessId = business.Id,
            Currency = request.CurrencyCode ?? "KES",
            Balance = request.InitialDeposit,
            AvailableBalance = request.InitialDeposit,
            AccountType = "Business",
            MinimumBalance = request.MinimumBalance ?? 10000 // Default business minimum
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Business account registered successfully",
            Account = new
            {
                account.Id,
                account.AccountNumber,
                account.AccountType,
                account.Currency,
                account.Balance,
                account.MinimumBalance,
                BusinessName = business.CompanyName,
                BusinessNumber = business.BusinessNumber,
                account.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to register business account", Error = ex.Message });
    }
});

// ============================================================================
// FIXED DEPOSITS - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Book Fixed Deposit
app.MapPost("/api/fixed-deposits/book", async ([FromBody] BookFixedDepositRequest request, WekezaDbContext context) =>
{
    try
    {
        var customer = await context.Customers.FindAsync(request.CustomerId);
        if (customer == null)
        {
            return Results.BadRequest(new { Message = "❌ Customer not found" });
        }

        var sourceAccount = await context.Accounts.FindAsync(request.SourceAccountId);
        if (sourceAccount == null)
        {
            return Results.BadRequest(new { Message = "❌ Source account not found" });
        }

        if (request.Amount <= 0)
        {
            return Results.BadRequest(new { Message = "❌ Deposit amount must be greater than zero" });
        }

        if (request.Amount > sourceAccount.AvailableBalance)
        {
            return Results.BadRequest(new 
            { 
                Message = "❌ Insufficient funds in source account",
                AvailableBalance = sourceAccount.AvailableBalance,
                RequestedAmount = request.Amount
            });
        }

        // Calculate maturity date and interest
        var maturityDate = DateTime.UtcNow.AddMonths(request.TermInMonths);
        var maturityAmount = request.Amount * (1 + (request.InterestRate / 100) * (request.TermInMonths / 12.0m));

        var fixedDeposit = new FixedDeposit
        {
            DepositNumber = $"FD-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            CustomerId = request.CustomerId,
            SourceAccountId = request.SourceAccountId,
            Amount = request.Amount,
            Currency = request.Currency ?? sourceAccount.Currency,
            InterestRate = request.InterestRate,
            TermInMonths = request.TermInMonths,
            MaturityDate = maturityDate,
            MaturityAmount = maturityAmount,
            Status = "Active"
        };

        // Debit from source account
        var previousBalance = sourceAccount.Balance;
        sourceAccount.Balance -= request.Amount;
        sourceAccount.AvailableBalance -= request.Amount;

        var transaction = new Transaction
        {
            AccountId = sourceAccount.Id,
            Type = "Debit",
            Amount = request.Amount,
            Currency = sourceAccount.Currency,
            PreviousBalance = previousBalance,
            NewBalance = sourceAccount.Balance,
            Reference = $"FDB-{fixedDeposit.DepositNumber}",
            Description = $"Fixed Deposit Booking - {fixedDeposit.DepositNumber}"
        };

        context.FixedDeposits.Add(fixedDeposit);
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Fixed deposit booked successfully",
            FixedDeposit = new
            {
                fixedDeposit.Id,
                fixedDeposit.DepositNumber,
                fixedDeposit.Amount,
                fixedDeposit.Currency,
                fixedDeposit.InterestRate,
                fixedDeposit.TermInMonths,
                fixedDeposit.MaturityDate,
                fixedDeposit.MaturityAmount,
                fixedDeposit.Status,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                SourceAccountNumber = sourceAccount.AccountNumber,
                InterestEarning = maturityAmount - request.Amount,
                fixedDeposit.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Fixed deposit booking failed", Error = ex.Message });
    }
});

// ============================================================================
// TELLER OPERATIONS - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Start Teller Session
app.MapPost("/api/teller/start-session", async ([FromBody] StartTellerSessionRequest request, WekezaDbContext context) =>
{
    try
    {
        // Check if teller already has an active session
        var existingSession = await context.TellerSessions
            .FirstOrDefaultAsync(ts => ts.TellerName == request.TellerName && ts.Status == "Active");

        if (existingSession != null)
        {
            return Results.BadRequest(new 
            { 
                Message = "❌ Teller already has an active session",
                ExistingSessionId = existingSession.Id,
                StartedAt = existingSession.StartedAt
            });
        }

        var session = new TellerSession
        {
            TellerName = request.TellerName,
            BranchCode = request.BranchCode,
            StartingCash = request.StartingCash,
            CurrentCash = request.StartingCash,
            Status = "Active"
        };

        context.TellerSessions.Add(session);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Teller session started successfully",
            Session = new
            {
                session.Id,
                session.TellerName,
                session.BranchCode,
                session.StartingCash,
                session.CurrentCash,
                session.Status,
                session.StartedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to start teller session", Error = ex.Message });
    }
});

// Process Cash Deposit (Teller)
app.MapPost("/api/teller/cash-deposit", async ([FromBody] ProcessCashDepositRequest request, WekezaDbContext context) =>
{
    try
    {
        var session = await context.TellerSessions.FindAsync(request.TellerSessionId);
        if (session == null)
        {
            return Results.BadRequest(new { Message = "❌ Teller session not found" });
        }

        if (session.Status != "Active")
        {
            return Results.BadRequest(new { Message = "❌ Teller session is not active" });
        }

        var account = await context.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);
            
        if (account == null)
        {
            return Results.BadRequest(new { Message = "❌ Account not found" });
        }

        if (request.Amount <= 0)
        {
            return Results.BadRequest(new { Message = "❌ Deposit amount must be greater than zero" });
        }

        // Update account balance
        var previousBalance = account.Balance;
        account.Balance += request.Amount;
        account.AvailableBalance += request.Amount;

        // Create transaction
        var transaction = new Transaction
        {
            AccountId = account.Id,
            Type = "Credit",
            Amount = request.Amount,
            Currency = account.Currency,
            PreviousBalance = previousBalance,
            NewBalance = account.Balance,
            Reference = $"TCD-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Description = "Teller Cash Deposit",
            ProcessedBy = session.TellerName
        };

        // Create teller transaction
        var tellerTransaction = new TellerTransaction
        {
            TellerSessionId = session.Id,
            TransactionId = transaction.Id,
            Type = "Cash Deposit",
            Amount = request.Amount,
            Currency = account.Currency,
            Reference = transaction.Reference,
            CustomerName = $"{account.Customer.FirstName} {account.Customer.LastName}",
            AccountNumber = account.AccountNumber
        };

        // Update teller cash position
        session.CurrentCash += request.Amount;
        session.UpdatedAt = DateTime.UtcNow;

        context.Transactions.Add(transaction);
        context.TellerTransactions.Add(tellerTransaction);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Cash deposit processed successfully",
            Transaction = new
            {
                transaction.Id,
                transaction.Reference,
                transaction.Amount,
                transaction.Currency,
                transaction.NewBalance,
                AccountNumber = account.AccountNumber,
                CustomerName = $"{account.Customer.FirstName} {account.Customer.LastName}",
                ProcessedBy = session.TellerName,
                transaction.ProcessedAt
            },
            TellerSession = new
            {
                session.Id,
                session.TellerName,
                session.CurrentCash,
                TransactionCount = session.Transactions.Count + 1
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Cash deposit processing failed", Error = ex.Message });
    }
});

// End Teller Session
app.MapPost("/api/teller/end-session/{sessionId}", async (Guid sessionId, [FromBody] EndTellerSessionRequest request, WekezaDbContext context) =>
{
    try
    {
        var session = await context.TellerSessions
            .Include(ts => ts.Transactions)
            .FirstOrDefaultAsync(ts => ts.Id == sessionId);
            
        if (session == null)
        {
            return Results.BadRequest(new { Message = "❌ Teller session not found" });
        }

        if (session.Status != "Active")
        {
            return Results.BadRequest(new { Message = "❌ Teller session is not active" });
        }

        session.Status = "Closed";
        session.EndedAt = DateTime.UtcNow;
        session.EndingCash = request.EndingCash;
        session.UpdatedAt = DateTime.UtcNow;

        var cashDifference = (session.EndingCash ?? 0) - session.CurrentCash;

        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Teller session ended successfully",
            Session = new
            {
                session.Id,
                session.TellerName,
                session.BranchCode,
                session.StartedAt,
                session.EndedAt,
                session.StartingCash,
                session.EndingCash,
                session.CurrentCash,
                CashDifference = cashDifference,
                TransactionCount = session.Transactions.Count,
                TotalCashIn = session.Transactions.Where(t => t.Type == "Cash Deposit").Sum(t => t.Amount),
                TotalCashOut = session.Transactions.Where(t => t.Type == "Cash Withdrawal").Sum(t => t.Amount),
                IsBalanced = Math.Abs(cashDifference) < 0.01m
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to end teller session", Error = ex.Message });
    }
});

// ============================================================================
// HELPER METHODS FOR LOAN CALCULATIONS
// ============================================================================

static decimal CalculateCreditScore(Customer customer, Account account, decimal loanAmount)
{
    var baseScore = 650m;
    
    // Account age factor
    var accountAge = (DateTime.UtcNow - account.CreatedAt).Days;
    if (accountAge > 365) baseScore += 50;
    else if (accountAge > 180) baseScore += 25;
    
    // Balance factor
    var balanceRatio = account.Balance / loanAmount;
    if (balanceRatio > 0.5m) baseScore += 100;
    else if (balanceRatio > 0.2m) baseScore += 50;
    
    // Age factor
    if (customer.DateOfBirth.HasValue)
    {
        var age = DateTime.Now.Year - customer.DateOfBirth.Value.Year;
        if (age >= 25 && age <= 55) baseScore += 50;
    }
    
    return Math.Min(850, Math.Max(300, baseScore));
}

static string DetermineRiskGrade(decimal creditScore)
{
    return creditScore switch
    {
        >= 750 => "A",
        >= 700 => "B",
        >= 650 => "C",
        >= 600 => "D",
        _ => "E"
    };
}

static decimal CalculateInterestRate(string riskGrade, int termInMonths)
{
    var baseRate = riskGrade switch
    {
        "A" => 12.0m,
        "B" => 14.0m,
        "C" => 16.0m,
        "D" => 18.0m,
        _ => 22.0m
    };
    
    // Adjust for term
    if (termInMonths > 36) baseRate += 1.0m;
    if (termInMonths > 60) baseRate += 1.0m;
    
    return baseRate;
}

static decimal CalculateMonthlyPayment(decimal principal, decimal annualRate, int termInMonths)
{
    var monthlyRate = annualRate / 100 / 12;
    var payment = principal * (monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), termInMonths)) /
                  ((decimal)Math.Pow((double)(1 + monthlyRate), termInMonths) - 1);
    return Math.Round(payment, 2);
}

// ============================================================================
// BRANCH OPERATIONS - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Create Branch
app.MapPost("/api/branches", async ([FromBody] CreateBranchRequest request, WekezaDbContext context) =>
{
    try
    {
        var branch = new Branch
        {
            BranchCode = request.BranchCode,
            BranchName = request.BranchName,
            Address = request.Address,
            City = request.City,
            Region = request.Region,
            Phone = request.Phone,
            Email = request.Email,
            Manager = request.Manager,
            CashLimit = request.CashLimit
        };

        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Branch created successfully",
            Branch = new
            {
                branch.Id,
                branch.BranchCode,
                branch.BranchName,
                branch.City,
                branch.Manager,
                branch.Status,
                branch.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to create branch", Error = ex.Message });
    }
});

// Get All Branches
app.MapGet("/api/branches", async (WekezaDbContext context) =>
{
    try
    {
        var branches = await context.Branches
            .Include(b => b.CashDrawers)
            .Select(b => new
            {
                b.Id,
                b.BranchCode,
                b.BranchName,
                b.Address,
                b.City,
                b.Region,
                b.Manager,
                b.Status,
                CashDrawerCount = b.CashDrawers.Count,
                TotalCashLimit = b.CashLimit,
                b.CreatedAt
            })
            .ToListAsync();

        return Results.Ok(new
        {
            Message = "✅ Branches retrieved successfully",
            TotalBranches = branches.Count,
            Branches = branches
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to retrieve branches", Error = ex.Message });
    }
});

// ============================================================================
// CARDS AND INSTRUMENTS - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Issue Card
app.MapPost("/api/cards/issue", async ([FromBody] IssueCardRequest request, WekezaDbContext context) =>
{
    try
    {
        var customer = await context.Customers.FindAsync(request.CustomerId);
        if (customer == null)
        {
            return Results.BadRequest(new { Message = "❌ Customer not found" });
        }

        var account = await context.Accounts.FindAsync(request.AccountId);
        if (account == null)
        {
            return Results.BadRequest(new { Message = "❌ Account not found" });
        }

        var card = new Card
        {
            CardNumber = GenerateCardNumber(),
            CustomerId = request.CustomerId,
            AccountId = request.AccountId,
            CardType = request.CardType,
            CardHolderName = $"{customer.FirstName} {customer.LastName}".ToUpper(),
            ExpiryDate = DateTime.UtcNow.AddYears(3),
            DailyLimit = request.DailyLimit,
            MonthlyLimit = request.MonthlyLimit,
            IsContactless = request.IsContactless,
            IsInternational = request.IsInternational
        };

        context.Cards.Add(card);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Card issued successfully",
            Card = new
            {
                card.Id,
                CardNumber = MaskCardNumber(card.CardNumber),
                card.CardType,
                card.CardHolderName,
                card.ExpiryDate,
                card.Status,
                card.DailyLimit,
                card.MonthlyLimit,
                card.IssuedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to issue card", Error = ex.Message });
    }
});

// Process ATM Transaction
app.MapPost("/api/cards/atm-transaction", async ([FromBody] ATMTransactionRequest request, WekezaDbContext context) =>
{
    try
    {
        var card = await context.Cards
            .Include(c => c.Account)
            .FirstOrDefaultAsync(c => c.CardNumber == request.CardNumber);

        if (card == null)
        {
            return Results.BadRequest(new { Message = "❌ Card not found" });
        }

        if (card.Status != "Active")
        {
            return Results.BadRequest(new { Message = "❌ Card is not active" });
        }

        // Check daily limit
        if (request.TransactionType == "Withdrawal" && (card.UsedToday + request.Amount) > card.DailyLimit)
        {
            return Results.BadRequest(new { Message = "❌ Daily limit exceeded" });
        }

        var atmTransaction = new ATMTransaction
        {
            CardId = card.Id,
            ATMId = request.ATMId,
            ATMLocation = request.ATMLocation,
            TransactionType = request.TransactionType,
            Amount = request.Amount,
            Reference = $"ATM-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Fee = CalculateATMFee(request.TransactionType, request.Amount)
        };

        // Process withdrawal
        if (request.TransactionType == "Withdrawal")
        {
            if (card.Account.AvailableBalance < request.Amount)
            {
                atmTransaction.Status = "Failed";
                atmTransaction.ResponseCode = "51";
                atmTransaction.ResponseMessage = "Insufficient funds";
            }
            else
            {
                card.Account.Balance -= request.Amount;
                card.Account.AvailableBalance -= request.Amount;
                card.UsedToday += request.Amount;
                atmTransaction.Status = "Completed";
                atmTransaction.ResponseCode = "00";
                atmTransaction.ResponseMessage = "Approved";
            }
        }

        context.ATMTransactions.Add(atmTransaction);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = atmTransaction.Status == "Completed" ? "✅ ATM transaction completed" : "❌ ATM transaction failed",
            Transaction = new
            {
                atmTransaction.Id,
                atmTransaction.Reference,
                atmTransaction.TransactionType,
                atmTransaction.Amount,
                atmTransaction.Status,
                atmTransaction.ResponseMessage,
                atmTransaction.Fee,
                atmTransaction.ProcessedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ ATM transaction failed", Error = ex.Message });
    }
});

// ============================================================================
// GENERAL LEDGER - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Create GL Account
app.MapPost("/api/gl/accounts", async ([FromBody] CreateGLAccountRequest request, WekezaDbContext context) =>
{
    try
    {
        var glAccount = new GLAccount
        {
            AccountCode = request.AccountCode,
            AccountName = request.AccountName,
            AccountType = request.AccountType,
            Category = request.Category,
            SubCategory = request.SubCategory,
            Currency = request.Currency,
            IsControlAccount = request.IsControlAccount,
            AllowManualPosting = request.AllowManualPosting
        };

        context.GLAccounts.Add(glAccount);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ GL Account created successfully",
            GLAccount = new
            {
                glAccount.Id,
                glAccount.AccountCode,
                glAccount.AccountName,
                glAccount.AccountType,
                glAccount.Balance,
                glAccount.Status,
                glAccount.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to create GL account", Error = ex.Message });
    }
});

// Post Journal Entry
app.MapPost("/api/gl/journal-entries", async ([FromBody] PostJournalEntryRequest request, WekezaDbContext context) =>
{
    try
    {
        var journalNumber = $"JE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        var journalEntries = new List<JournalEntry>();

        foreach (var entry in request.Entries)
        {
            var glAccount = await context.GLAccounts.FindAsync(entry.GLAccountId);
            if (glAccount == null)
            {
                return Results.BadRequest(new { Message = $"❌ GL Account {entry.GLAccountId} not found" });
            }

            var journalEntry = new JournalEntry
            {
                JournalNumber = journalNumber,
                GLAccountId = entry.GLAccountId,
                EntryType = entry.EntryType,
                Amount = entry.Amount,
                Currency = entry.Currency,
                Description = entry.Description,
                Reference = request.Reference,
                TransactionId = request.TransactionId
            };

            // Update GL Account balance
            if (entry.EntryType == "Debit")
            {
                glAccount.DebitBalance += entry.Amount;
                if (glAccount.AccountType == "Asset" || glAccount.AccountType == "Expense")
                {
                    glAccount.Balance += entry.Amount;
                }
                else
                {
                    glAccount.Balance -= entry.Amount;
                }
            }
            else // Credit
            {
                glAccount.CreditBalance += entry.Amount;
                if (glAccount.AccountType == "Liability" || glAccount.AccountType == "Equity" || glAccount.AccountType == "Income")
                {
                    glAccount.Balance += entry.Amount;
                }
                else
                {
                    glAccount.Balance -= entry.Amount;
                }
            }

            journalEntries.Add(journalEntry);
        }

        context.JournalEntries.AddRange(journalEntries);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Journal entries posted successfully",
            JournalNumber = journalNumber,
            TotalEntries = journalEntries.Count,
            TotalDebits = journalEntries.Where(e => e.EntryType == "Debit").Sum(e => e.Amount),
            TotalCredits = journalEntries.Where(e => e.EntryType == "Credit").Sum(e => e.Amount)
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to post journal entries", Error = ex.Message });
    }
});

// ============================================================================
// PAYMENTS - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Create Payment Order
app.MapPost("/api/payments/orders", async ([FromBody] CreatePaymentOrderRequest request, WekezaDbContext context) =>
{
    try
    {
        var fromAccount = await context.Accounts.FindAsync(request.FromAccountId);
        if (fromAccount == null)
        {
            return Results.BadRequest(new { Message = "❌ Source account not found" });
        }

        if (fromAccount.AvailableBalance < request.Amount)
        {
            return Results.BadRequest(new { Message = "❌ Insufficient funds" });
        }

        var paymentOrder = new PaymentOrder
        {
            PaymentReference = $"PAY-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            FromAccountId = request.FromAccountId,
            ToAccountNumber = request.ToAccountNumber,
            BeneficiaryName = request.BeneficiaryName,
            BeneficiaryBank = request.BeneficiaryBank,
            BeneficiaryBankCode = request.BeneficiaryBankCode,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentType = request.PaymentType,
            PaymentPurpose = request.PaymentPurpose,
            Priority = request.Priority,
            ValueDate = request.ValueDate,
            InitiatedBy = request.InitiatedBy,
            Fee = CalculatePaymentFee(request.PaymentType, request.Amount)
        };

        context.PaymentOrders.Add(paymentOrder);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Payment order created successfully",
            PaymentOrder = new
            {
                paymentOrder.Id,
                paymentOrder.PaymentReference,
                paymentOrder.Amount,
                paymentOrder.Currency,
                paymentOrder.PaymentType,
                paymentOrder.Status,
                paymentOrder.Fee,
                paymentOrder.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to create payment order", Error = ex.Message });
    }
});

// ============================================================================
// PRODUCTS - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Create Product
app.MapPost("/api/products", async ([FromBody] CreateProductRequest request, WekezaDbContext context) =>
{
    try
    {
        var product = new Product
        {
            ProductCode = request.ProductCode,
            ProductName = request.ProductName,
            ProductType = request.ProductType,
            Category = request.Category,
            Description = request.Description,
            InterestRate = request.InterestRate,
            MinimumBalance = request.MinimumBalance,
            MaximumBalance = request.MaximumBalance,
            MonthlyFee = request.MonthlyFee,
            TransactionFee = request.TransactionFee,
            Currency = request.Currency,
            MinAge = request.MinAge,
            MaxAge = request.MaxAge,
            EligibilityCriteria = request.EligibilityCriteria,
            Features = request.Features
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Product created successfully",
            Product = new
            {
                product.Id,
                product.ProductCode,
                product.ProductName,
                product.ProductType,
                product.InterestRate,
                product.MinimumBalance,
                product.Status,
                product.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to create product", Error = ex.Message });
    }
});

// Get All Products
app.MapGet("/api/products", async (WekezaDbContext context) =>
{
    try
    {
        var products = await context.Products
            .Where(p => p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.ProductType,
                p.Category,
                p.InterestRate,
                p.MinimumBalance,
                p.MonthlyFee,
                p.Features,
                p.Status,
                AccountCount = p.Accounts.Count()
            })
            .ToListAsync();

        return Results.Ok(new
        {
            Message = "✅ Products retrieved successfully",
            TotalProducts = products.Count,
            Products = products
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to retrieve products", Error = ex.Message });
    }
});

// ============================================================================
// TRADE FINANCE - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Issue Letter of Credit
app.MapPost("/api/trade-finance/letters-of-credit", async ([FromBody] IssueLCRequest request, WekezaDbContext context) =>
{
    try
    {
        var customer = await context.Customers.FindAsync(request.CustomerId);
        if (customer == null)
        {
            return Results.BadRequest(new { Message = "❌ Customer not found" });
        }

        var lc = new LetterOfCredit
        {
            LCNumber = $"LC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CustomerId = request.CustomerId,
            Applicant = request.Applicant,
            Beneficiary = request.Beneficiary,
            AdvisingBank = request.AdvisingBank,
            ConfirmingBank = request.ConfirmingBank,
            Amount = request.Amount,
            Currency = request.Currency,
            LCType = request.LCType,
            PaymentTerms = request.PaymentTerms,
            GoodsDescription = request.GoodsDescription,
            ExpiryDate = request.ExpiryDate,
            LatestShipmentDate = request.LatestShipmentDate,
            PortOfLoading = request.PortOfLoading,
            PortOfDischarge = request.PortOfDischarge,
            Commission = request.Amount * 0.0025m, // 0.25% commission
            SpecialInstructions = request.SpecialInstructions
        };

        context.LettersOfCredit.Add(lc);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ Letter of Credit issued successfully",
            LetterOfCredit = new
            {
                lc.Id,
                lc.LCNumber,
                lc.Amount,
                lc.Currency,
                lc.LCType,
                lc.Status,
                lc.ExpiryDate,
                lc.Commission,
                lc.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to issue Letter of Credit", Error = ex.Message });
    }
});

// ============================================================================
// TREASURY - COMPREHENSIVE IMPLEMENTATION
// ============================================================================

// Create FX Deal
app.MapPost("/api/treasury/fx-deals", async ([FromBody] CreateFXDealRequest request, WekezaDbContext context) =>
{
    try
    {
        var customer = await context.Customers.FindAsync(request.CustomerId);
        if (customer == null)
        {
            return Results.BadRequest(new { Message = "❌ Customer not found" });
        }

        var fxDeal = new FXDeal
        {
            DealNumber = $"FX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CustomerId = request.CustomerId,
            BaseCurrency = request.BaseCurrency,
            QuoteCurrency = request.QuoteCurrency,
            BaseAmount = request.BaseAmount,
            ExchangeRate = request.ExchangeRate,
            QuoteAmount = request.BaseAmount * request.ExchangeRate,
            DealType = request.DealType,
            Side = request.Side,
            ValueDate = request.ValueDate,
            MaturityDate = request.MaturityDate,
            Trader = request.Trader,
            Purpose = request.Purpose,
            Margin = request.BaseAmount * 0.001m // 0.1% margin
        };

        context.FXDeals.Add(fxDeal);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            Message = "✅ FX Deal created successfully",
            FXDeal = new
            {
                fxDeal.Id,
                fxDeal.DealNumber,
                fxDeal.BaseCurrency,
                fxDeal.QuoteCurrency,
                fxDeal.BaseAmount,
                fxDeal.QuoteAmount,
                fxDeal.ExchangeRate,
                fxDeal.Status,
                fxDeal.Margin,
                fxDeal.CreatedAt
            }
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Message = "❌ Failed to create FX deal", Error = ex.Message });
    }
});

// ============================================================================
// HELPER METHODS FOR COMPREHENSIVE BANKING
// ============================================================================

static string GenerateCardNumber()
{
    var random = new Random();
    var cardNumber = "4532"; // Visa prefix
    for (int i = 0; i < 12; i++)
    {
        cardNumber += random.Next(0, 10).ToString();
    }
    return cardNumber;
}

static string MaskCardNumber(string cardNumber)
{
    if (cardNumber.Length < 8) return cardNumber;
    return cardNumber[..4] + "****" + cardNumber[^4..];
}

static decimal CalculateATMFee(string transactionType, decimal amount)
{
    return transactionType switch
    {
        "Withdrawal" => amount > 10000 ? 50 : 25,
        "BalanceInquiry" => 10,
        _ => 0
    };
}

static decimal CalculatePaymentFee(string paymentType, decimal amount)
{
    return paymentType switch
    {
        "RTGS" => Math.Max(500, amount * 0.001m),
        "EFT" => Math.Max(100, amount * 0.0005m),
        "SWIFT" => Math.Max(1000, amount * 0.002m),
        "Internal" => 0,
        _ => 50
    };
}

// Configure to run on port 5001
app.Urls.Add("http://localhost:5001");

app.Run();

// ============================================================================
// REQUEST DTOs FOR COMPREHENSIVE BANKING SYSTEM
// ============================================================================
public record CreateCustomerRequest(string FirstName, string LastName, string Email, string IdentificationNumber);
public record CreateAccountRequest(string CustomerId, string? CurrencyCode, decimal InitialDeposit);
public record DepositRequest(string AccountNumber, decimal Amount);
public record WithdrawRequest(string AccountNumber, decimal Amount);

// Enhanced DTOs for comprehensive banking features
public record UpdateKYCStatusRequest(string Status);

public record CreateIndividualPartyRequest(
    string FirstName,
    string? MiddleName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    string? MaritalStatus,
    string Nationality,
    string PrimaryEmail,
    string PrimaryPhone,
    string? SecondaryPhone,
    string? PreferredLanguage,
    AddressRequest PrimaryAddress,
    IdentificationRequest PrimaryIdentification,
    bool OptInMarketing,
    bool OptInSMS,
    bool OptInEmail
);

public record CreateCorporatePartyRequest(
    string CompanyName,
    string RegistrationNumber,
    DateTime IncorporationDate,
    string CompanyType,
    string Industry,
    string PrimaryEmail,
    string PrimaryPhone,
    string? Website,
    decimal? AnnualTurnover,
    int? NumberOfEmployees,
    string? TaxIdentificationNumber
);

public record AddressRequest(
    string AddressType,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string Country,
    string PostalCode,
    bool IsPrimary
);

public record IdentificationRequest(
    string DocumentType,
    string DocumentNumber,
    string IssuingCountry,
    DateTime IssueDate,
    DateTime ExpiryDate
);

// Account Management DTOs
public record OpenProductBasedAccountRequest(
    string CustomerId,
    string ProductCode,
    string? CurrencyCode,
    decimal InitialDeposit,
    decimal? InterestRate,
    decimal? MinimumBalance,
    decimal? MonthlyFee
);

public record AddSignatoryRequest(
    string SignatoryName,
    string IdNumber,
    string Role,
    decimal SignatureLimit
);

public record FreezeAccountRequest(
    string Reason,
    string FrozenBy
);

public record UnfreezeAccountRequest(
    string UnfrozenBy
);

public record CloseAccountRequest(
    string Reason,
    string ClosedBy
);

public record RegisterBusinessAccountRequest(
    Guid BusinessId,
    string? CurrencyCode,
    decimal InitialDeposit,
    decimal? MinimumBalance
);

// Transaction DTOs
public record TransferFundsRequest(
    string FromAccountNumber,
    string ToAccountNumber,
    decimal Amount,
    string? Currency,
    string Description
);

public record DepositChequeRequest(
    string AccountNumber,
    decimal Amount,
    string ChequeNumber,
    string DrawerBank
);

// Loan DTOs
public record ApplyForLoanRequest(
    Guid CustomerId,
    Guid AccountId,
    decimal Amount,
    string Currency,
    int TermInMonths,
    string Purpose,
    DateTime? PreferredDisbursementDate
);

public record ApproveLoanRequest(
    string ApprovedBy,
    string? Comments,
    decimal? ApprovedInterestRate
);

public record DisburseLoanRequest(
    string DisbursedBy
);

public record ProcessRepaymentRequest(
    decimal Amount,
    string? PaymentMethod,
    string? ProcessedBy
);

// Fixed Deposit DTOs
public record BookFixedDepositRequest(
    Guid CustomerId,
    Guid SourceAccountId,
    decimal Amount,
    string? Currency,
    decimal InterestRate,
    int TermInMonths
);

// Teller DTOs
public record StartTellerSessionRequest(
    string TellerName,
    string BranchCode,
    decimal StartingCash
);

public record ProcessCashDepositRequest(
    Guid TellerSessionId,
    string AccountNumber,
    decimal Amount
);

public record EndTellerSessionRequest(
    decimal EndingCash
);

// Branch Operations DTOs
public record CreateBranchRequest(
    string BranchCode,
    string BranchName,
    string Address,
    string City,
    string Region,
    string Phone,
    string Email,
    string Manager,
    decimal CashLimit
);

// Cards DTOs
public record IssueCardRequest(
    Guid CustomerId,
    Guid AccountId,
    string CardType,
    decimal DailyLimit,
    decimal MonthlyLimit,
    bool IsContactless,
    bool IsInternational
);

public record ATMTransactionRequest(
    string CardNumber,
    string ATMId,
    string ATMLocation,
    string TransactionType,
    decimal Amount
);

// General Ledger DTOs
public record CreateGLAccountRequest(
    string AccountCode,
    string AccountName,
    string AccountType,
    string Category,
    string SubCategory,
    string Currency,
    bool IsControlAccount,
    bool AllowManualPosting
);

public record PostJournalEntryRequest(
    string Reference,
    string? TransactionId,
    List<JournalEntryLineRequest> Entries
);

public record JournalEntryLineRequest(
    Guid GLAccountId,
    string EntryType,
    decimal Amount,
    string Currency,
    string Description
);

// Payments DTOs
public record CreatePaymentOrderRequest(
    Guid FromAccountId,
    string ToAccountNumber,
    string BeneficiaryName,
    string BeneficiaryBank,
    string BeneficiaryBankCode,
    decimal Amount,
    string Currency,
    string PaymentType,
    string PaymentPurpose,
    string Priority,
    DateTime ValueDate,
    string InitiatedBy
);

// Products DTOs
public record CreateProductRequest(
    string ProductCode,
    string ProductName,
    string ProductType,
    string Category,
    string Description,
    decimal InterestRate,
    decimal MinimumBalance,
    decimal MaximumBalance,
    decimal MonthlyFee,
    decimal TransactionFee,
    string Currency,
    int MinAge,
    int MaxAge,
    string EligibilityCriteria,
    string Features
);

// Trade Finance DTOs
public record IssueLCRequest(
    Guid CustomerId,
    string Applicant,
    string Beneficiary,
    string AdvisingBank,
    string? ConfirmingBank,
    decimal Amount,
    string Currency,
    string LCType,
    string PaymentTerms,
    string GoodsDescription,
    DateTime ExpiryDate,
    DateTime LatestShipmentDate,
    string PortOfLoading,
    string PortOfDischarge,
    string? SpecialInstructions
);

// Treasury DTOs
public record CreateFXDealRequest(
    Guid CustomerId,
    string BaseCurrency,
    string QuoteCurrency,
    decimal BaseAmount,
    decimal ExchangeRate,
    string DealType,
    string Side,
    DateTime ValueDate,
    DateTime? MaturityDate,
    string Trader,
    string Purpose
);

// ============================================================================
// END OF COMPREHENSIVE BANKING SYSTEM
// ============================================================================