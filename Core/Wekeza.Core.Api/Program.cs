using Wekeza.Core.Api.Extensions;
using Wekeza.Core.Api.Middleware;
using Wekeza.Core.Application;
using Wekeza.Core.Infrastructure;
using Serilog;

// Configure Serilog early
var builder = WebApplication.CreateBuilder(args);
builder.AddWekezaSerilog();

try
{
    Log.Information("🏦 Starting Wekeza Core Banking System...");

    // --- 1. SERVICE REGISTRATION (DI Container) ---

    // Add the layers (These extension methods are defined in their respective projects)
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Add Authentication & Authorization
    builder.Services.AddWekezaAuthentication(builder.Configuration);

    // Add Observability (Logging, Metrics, Health Checks)
    builder.Services.AddWekezaObservability(builder.Configuration);

    // Add Rate Limiting
    builder.Services.AddWekezaRateLimiting();

    // Add API Specifics
    builder.Services.AddControllers();
    builder.Services.AddWekezaHealthChecks(builder.Configuration);
    builder.Services.AddWekezaSwagger();

    // Enable static files for Swagger custom CSS
    builder.Services.AddHttpContextAccessor();

    // Add CORS for frontend applications
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("WekezaPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // --- 2. THE MIDDLEWARE GAUNTLET (The Pipeline) ---

    // Serilog request logging
    app.UseSerilogRequestLogging();

    // Serve static files (for Swagger custom CSS)
    app.UseStaticFiles();

    // Order matters here! Security first, then Logging, then the logic.
    if (app.Environment.IsDevelopment())
    {
        app.UseWekezaSwagger();
    }

    // Global Error Shield
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // The Audit Guard: Records every request attempt
    app.UseMiddleware<TransactionLoggingMiddleware>();

    // The Performance Watchtower: Catches slow requests (>500ms)
    app.UseMiddleware<PerformanceMiddleware>();

    app.UseHttpsRedirection();
    app.UseCors("WekezaPolicy");

    // Rate Limiting
    app.UseRateLimiter();

    // Authentication & Authorization (MUST be in this order)
    app.UseAuthentication();
    app.UseAuthorization();

    // Map Health Check Endpoints for the Load Balancer
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready");
    app.MapHealthChecks("/health/live");
    
    // Welcome endpoint with system information
    app.MapGet("/", () => new
    {
        Service = "Wekeza Core Banking System",
        Version = "1.0.0",
        Environment = app.Environment.EnvironmentName,
        Timestamp = DateTime.UtcNow,
        Status = "Running",
        Features = new[]
        {
            "Complete Banking Operations",
            "Maker-Checker Workflow",
            "Multi-Role Access Control",
            "Real-time Analytics Dashboard",
            "Digital Channels (Internet, Mobile, USSD)",
            "Comprehensive Loan Management",
            "Trade Finance Operations",
            "Treasury & FX Management",
            "Risk & Compliance Management",
            "Advanced Reporting & MIS"
        },
        Portals = new[]
        {
            "Administrator Portal - /admin (Web Interface)",
            "Administrator API - /api/administrator",
            "Teller Portal - /api/teller", 
            "Customer Self-Service Portal - /api/customer-portal",
            "Analytics Dashboard - /api/dashboard",
            "Loan Officer Portal - /api/loans",
            "Compliance Portal - /api/compliance"
        },
        Documentation = "/swagger"
    });

    // API overview endpoint
    app.MapGet("/api", () => new
    {
        Title = "Wekeza Core Banking API",
        Description = "Enterprise-grade banking system with comprehensive operations",
        Version = "1.0.0",
        Documentation = "/swagger",
        Modules = new
        {
            Administration = "/api/administrator - User, role, and system management",
            TellerOperations = "/api/teller - Branch teller operations",
            CustomerPortal = "/api/customer-portal - Customer self-service", 
            Dashboard = "/api/dashboard - Real-time analytics and KPIs",
            Accounts = "/api/accounts - Account management and operations",
            CIF = "/api/cif - Customer Information File management",
            Loans = "/api/loans - Loan origination and management",
            Payments = "/api/payments - Payment processing",
            Transactions = "/api/transactions - Transaction processing",
            Cards = "/api/cards - Card management (physical & virtual)",
            DigitalChannels = "/api/digitalchannels - Channel enrollment",
            BranchOperations = "/api/branchoperations - Branch daily operations",
            Compliance = "/api/compliance - AML and compliance management",
            TradeFinance = "/api/tradefinance - Letters of credit and guarantees",
            Treasury = "/api/treasury - FX and money market operations",
            Reporting = "/api/reporting - Regulatory and MIS reports",
            Workflows = "/api/workflows - Maker-checker approval workflows"
        }
    });

    // Map our Controllers (All banking modules)
    app.MapControllers();

    // --- 3. THE IGNITION ---
    Log.Information("✅ Wekeza Core Banking System started successfully");
    Log.Information("🌐 API Documentation available at: /swagger");
    Log.Information("📊 System Status available at: /");
    Log.Information("🏥 Health Checks available at: /health");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Wekeza Core Banking System failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
