using Serilog;

namespace Wekeza.Core.Api.Extensions;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddWekezaSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/wekeza-.txt", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Wekeza.Core.Api");
        });

        return builder;
    }

    public static IServiceCollection AddWekezaObservability(this IServiceCollection services, IConfiguration configuration)
    {
        // Add logging
        services.AddLogging();

        // Add metrics (if needed)
        services.AddSingleton<IMetricsLogger, MetricsLogger>();

        return services;
    }
}

public interface IMetricsLogger
{
    void LogMetric(string name, double value, Dictionary<string, string>? tags = null);
}

public class MetricsLogger : IMetricsLogger
{
    private readonly ILogger<MetricsLogger> _logger;

    public MetricsLogger(ILogger<MetricsLogger> logger)
    {
        _logger = logger;
    }

    public void LogMetric(string name, double value, Dictionary<string, string>? tags = null)
    {
        _logger.LogInformation("Metric: {MetricName} = {Value} {@Tags}", name, value, tags);
    }
}