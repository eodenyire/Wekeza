using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Wekeza.Core.Api.Extensions;

/// <summary>
/// Extension methods for configuring observability (logging, metrics, tracing)
/// </summary>
public static class ObservabilityExtensions
{
    public static IServiceCollection AddWekezaObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add Application Insights (optional)
        services.AddApplicationInsightsTelemetry();

        // Add Health Checks
        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "database",
                tags: new[] { "db", "sql", "postgres" })
            .AddRedis(
                configuration["Redis:ConnectionString"] ?? "localhost:6379",
                name: "redis",
                tags: new[] { "cache", "redis" });

        return services;
    }

    public static WebApplicationBuilder AddWekezaSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "Wekeza.Bank")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code)
            .WriteTo.File(
                path: "logs/wekeza-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Seq(
                serverUrl: context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341",
                apiKey: context.Configuration["Seq:ApiKey"])
        );

        return builder;
    }
}
