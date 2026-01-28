using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WekezaERMS.Application.Behaviors;
using WekezaERMS.Application.Commands.Risks;
using WekezaERMS.Application.Mappings;
using WekezaERMS.Infrastructure.Persistence;
using WekezaERMS.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Wekeza ERMS API", 
        Version = "v1",
        Description = "Enterprise Risk Management System API for Wekeza Bank"
    });
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
app.UseAuthorization();
app.MapControllers();

app.Run();
