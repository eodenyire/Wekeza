using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Services;
using Wekeza.Nexus.Application;
using Wekeza.Nexus.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/mvp4-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Configure PostgreSQL Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=wekeza_mvp4;Username=postgres;Password=postgres";

builder.Services.AddDbContext<MVP4DbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Wekeza Nexus Fraud Detection System
// Integrated with MVP4.0 PostgreSQL and Redis infrastructure
builder.Services.AddWekezaNexus();
builder.Services.AddWekezaNexusInfrastructure(builder.Configuration, usePostgreSql: true, useRedis: true);

Log.Information("Wekeza Nexus fraud detection system registered successfully");

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "WekeezaMVP4SecretKeyThatIsAtLeast32CharactersLong123456";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WekeezaMVP4";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WekeezaMVP4Users";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("Teller", policy => policy.RequireRole("Teller"));
    options.AddPolicy("Supervisor", policy => policy.RequireRole("Supervisor"));
    options.AddPolicy("BranchManager", policy => policy.RequireRole("BranchManager"));
    options.AddPolicy("LoanOfficer", policy => policy.RequireRole("LoanOfficer"));
    options.AddPolicy("CashOfficer", policy => policy.RequireRole("CashOfficer"));
    options.AddPolicy("BackOfficeStaff", policy => policy.RequireRole("BackOfficeStaff"));
    options.AddPolicy("ComplianceOfficer", policy => policy.RequireRole("ComplianceOfficer"));
    options.AddPolicy("RiskOfficer", policy => policy.RequireRole("RiskOfficer"));
    options.AddPolicy("Auditor", policy => policy.RequireRole("Auditor"));
    options.AddPolicy("CustomerCareOfficer", policy => policy.RequireRole("CustomerCareOfficer"));
    options.AddPolicy("BancassuranceAgent", policy => policy.RequireRole("BancassuranceAgent"));
    options.AddPolicy("ITAdministrator", policy => policy.RequireRole("ITAdministrator"));
});

// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpClient();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Create database and apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Apply MVP4.0 migrations
        var context = services.GetRequiredService<MVP4DbContext>();
        context.Database.Migrate();
        Log.Information("MVP4.0 database created and migrations applied successfully");
        
        // Apply Wekeza Nexus migrations
        var nexusContext = services.GetRequiredService<Wekeza.Nexus.Infrastructure.Data.NexusDbContext>();
        nexusContext.Database.Migrate();
        Log.Information("Wekeza Nexus database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while creating the database");
        // Try to ensure created if migration fails
        try
        {
            var context = services.GetRequiredService<MVP4DbContext>();
            context.Database.EnsureCreated();
            Log.Information("MVP4.0 database ensured created");
            
            var nexusContext = services.GetRequiredService<Wekeza.Nexus.Infrastructure.Data.NexusDbContext>();
            nexusContext.Database.EnsureCreated();
            Log.Information("Wekeza Nexus database ensured created");
        }
        catch (Exception innerEx)
        {
            Log.Error(innerEx, "Failed to ensure database creation");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

Log.Information("Wekeza MVP4.0 Core Banking System started successfully on port 5004");
Log.Information("Navigate to https://localhost:5004 to access the application");

app.Run();
