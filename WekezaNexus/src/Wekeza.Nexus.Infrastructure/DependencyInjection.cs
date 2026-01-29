using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Wekeza.Nexus.Domain.Interfaces;
using Wekeza.Nexus.Infrastructure.Data;
using Wekeza.Nexus.Infrastructure.Repositories;
using Wekeza.Nexus.Infrastructure.Services;

namespace Wekeza.Nexus.Infrastructure;

/// <summary>
/// Dependency injection for Wekeza Nexus Infrastructure layer
/// Integrated with MVP4.0 PostgreSQL and Redis infrastructure
/// 
/// Usage in Program.cs:
/// builder.Services.AddWekezaNexusInfrastructure(builder.Configuration);
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add Wekeza Nexus Infrastructure services with PostgreSQL and Redis support
    /// </summary>
    public static IServiceCollection AddWekezaNexusInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool usePostgreSql = true,
        bool useRedis = true)
    {
        if (usePostgreSql)
        {
            // Register PostgreSQL DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Database=wekeza_mvp4;Username=postgres;Password=postgres";
            
            services.AddDbContext<NexusDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            // Register PostgreSQL-backed repository
            services.AddScoped<IFraudEvaluationRepository, PostgreSqlFraudEvaluationRepository>();
        }
        else
        {
            // Fallback to in-memory implementation
            services.AddScoped<IFraudEvaluationRepository, InMemoryFraudEvaluationRepository>();
        }
        
        if (useRedis)
        {
            // Register Redis connection (reuse from existing MVP4.0 configuration if available)
            var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString")
                ?? "localhost:6379";
            
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configOptions = ConfigurationOptions.Parse(redisConnectionString);
                configOptions.AbortOnConnectFail = false;
                configOptions.ConnectTimeout = 5000;
                configOptions.SyncTimeout = 5000;
                return ConnectionMultiplexer.Connect(configOptions);
            });
            
            // Register Redis-backed velocity service
            services.AddScoped<ITransactionVelocityService, RedisTransactionVelocityService>();
        }
        else
        {
            // Fallback to stub implementation (from Application layer)
            // Already registered in AddWekezaNexus()
        }
        
        return services;
    }
    
    /// <summary>
    /// Add Wekeza Nexus Infrastructure with in-memory implementations (for testing)
    /// </summary>
    public static IServiceCollection AddWekezaNexusInMemory(this IServiceCollection services)
    {
        services.AddScoped<IFraudEvaluationRepository, InMemoryFraudEvaluationRepository>();
        return services;
    }
}
