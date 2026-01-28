using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WekezaERMS.Application.Behaviors;
using WekezaERMS.Application.Commands.Risks;
using WekezaERMS.Application.Commands.Users;
using WekezaERMS.Application.Mappings;
using WekezaERMS.Application.Services;
using WekezaERMS.Infrastructure.Persistence;
using WekezaERMS.Infrastructure.Persistence.Repositories;
using WekezaERMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;
using WekezaERMS.Domain.Entities;
using WekezaERMS.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure JWT Settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Wekeza ERMS API", 
        Version = "v1",
        Description = "Enterprise Risk Management System API for Wekeza Bank - JWT Auth Required"
    });
    
    // Note: JWT Bearer auth setup - use Authorization header with format: Bearer {token}
    c.AddServer(new() { Url = "https://localhost:7000", Description = "Development Server" });
    c.AddServer(new() { Url = "http://localhost:5000", Description = "Development HTTP" });
});

// Configure Database - Use In-Memory for demo, PostgreSQL for production
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);
if (useInMemory)
{
    builder.Services.AddDbContext<ERMSDbContext>(options =>
        options.UseInMemoryDatabase("WekezaERMS"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("ERMSConnection") 
        ?? "Host=localhost;Database=WekezaERMS;Username=postgres;Password=postgres";
    builder.Services.AddDbContext<ERMSDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Register AutoMapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<RiskMappingProfile>();
});

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateRiskCommand>();

// Register MediatR with validation pipeline
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateRiskCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// Register Repositories
builder.Services.AddScoped<IRiskRepository, RiskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Services
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RiskManager", policy => 
        policy.RequireRole("RiskManager", "Administrator"));
    
    options.AddPolicy("RiskOfficer", policy => 
        policy.RequireRole("RiskOfficer", "RiskManager", "Administrator"));
    
    options.AddPolicy("RiskViewer", policy => 
        policy.RequireRole("RiskViewer", "RiskOfficer", "RiskManager", "Auditor", "Executive", "Administrator"));
    
    options.AddPolicy("Auditor", policy => 
        policy.RequireRole("Auditor", "Administrator"));
    
    options.AddPolicy("Executive", policy => 
        policy.RequireRole("Executive", "Administrator"));
    
    options.AddPolicy("Administrator", policy => 
        policy.RequireRole("Administrator"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Seed initial admin user
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ERMSDbContext>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    
    // Ensure database is created
    context.Database.EnsureCreated();
    
    // Check if admin user exists
    var adminExists = await userRepository.GetByUsernameAsync("admin");
    if (adminExists == null)
    {
        var adminUser = WekezaERMS.Domain.Entities.User.Create(
            username: "admin",
            email: "admin@wekeza.com",
            passwordHash: BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            role: UserRole.Administrator,
            fullName: "System Administrator"
        );
        await userRepository.CreateAsync(adminUser);
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wekeza ERMS API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
