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
    Log.Information("🏦 Starting Wekeza Bank API...");

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

    var app = builder.Build();

    // --- 2. THE MIDDLEWARE GAUNTLET (The Pipeline) ---

    // Serilog request logging
    app.UseSerilogRequestLogging();

    // Order matters here! Security first, then Logging, then the logic.
    if (app.Environment.IsDevelopment())
    {
        app.UseWekezaSwagger();
    }

    // Serve static files (for Swagger custom CSS)
    app.UseStaticFiles();

    // Global Error Shield
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // The Audit Guard: Records every request attempt
    app.UseMiddleware<TransactionLoggingMiddleware>();

    // The Performance Watchtower: Catches slow requests (>500ms)
    app.UseMiddleware<PerformanceMiddleware>();

    app.UseHttpsRedirection();

    // Rate Limiting
    app.UseRateLimiter();

    // Authentication & Authorization (MUST be in this order)
    app.UseAuthentication();
    app.UseAuthorization();

    // Map Health Check Endpoints for the Load Balancer
    app.MapHealthChecks("/health");

    // Map our Controllers (Accounts, Transactions, Loans, Cards)
    app.MapControllers();

    // --- 3. THE IGNITION ---
    Log.Information("✅ Wekeza Bank API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Wekeza Bank API failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
