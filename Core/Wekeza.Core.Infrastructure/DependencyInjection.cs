using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Npgsql;
using System.Data;
using Wekeza.Core.Infrastructure.Persistence;
using Wekeza.Core.Infrastructure.Persistence.Repositories;
using Wekeza.Core.Infrastructure.Services;
using Wekeza.Core.Infrastructure.Caching;
using Wekeza.Core.Infrastructure.Monitoring;
using Wekeza.Core.Infrastructure.Notifications;
using Wekeza.Core.Infrastructure.ApiGateway;
using Wekeza.Core.Infrastructure.HealthChecks;
using Wekeza.Core.Infrastructure.BackgroundServices;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Register IDbConnection for Dapper-based repositories
        services.AddScoped<IDbConnection>(provider =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            return new NpgsqlConnection(connectionString);
        });

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IApprovalMatrixRepository, ApprovalMatrixRepository>();
        services.AddScoped<IGLAccountRepository, GLAccountRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<IPaymentOrderRepository, PaymentOrderRepository>();
        services.AddScoped<ITellerSessionRepository, TellerSessionRepository>();
        services.AddScoped<ICashDrawerRepository, CashDrawerRepository>();
        services.AddScoped<ITellerTransactionRepository, TellerTransactionRepository>();
        
        // Week 8: Cards & Channels Management Repositories
        services.AddScoped<IATMTransactionRepository, ATMTransactionRepository>();
        services.AddScoped<IPOSTransactionRepository, POSTransactionRepository>();
        services.AddScoped<ICardApplicationRepository, CardApplicationRepository>();

        // Week 9: Trade Finance Repositories
        services.AddScoped<ILetterOfCreditRepository, LetterOfCreditRepository>();
        services.AddScoped<IBankGuaranteeRepository, BankGuaranteeRepository>();
        services.AddScoped<IDocumentaryCollectionRepository, DocumentaryCollectionRepository>();

        // New Repositories for 200% Completion
        services.AddScoped<IApprovalWorkflowRepository, ApprovalWorkflowRepository>();
        services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IDigitalChannelRepository, DigitalChannelRepository>();

        // Domain Services
        services.AddScoped<PaymentProcessingService>();
        services.AddScoped<CreditScoringService>();
        services.AddScoped<LoanServicingService>();
        services.AddScoped<TellerOperationsService>();
        
        // Week 8: Cards & Channels Management Services
        services.AddScoped<CardManagementService>();
        services.AddScoped<ATMProcessingService>();
        services.AddScoped<POSProcessingService>();

        // Application Services
        services.AddScoped<IAMLScreeningService, AMLScreeningService>();
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailService, EmailService>();

        // Week 14: Advanced Features & Optimization Services
        AddWeek14Services(services, configuration);

        return services;
    }

    /// <summary>
    /// Add Week 14 Advanced Features & Optimization services
    /// </summary>
    private static void AddWeek14Services(IServiceCollection services, IConfiguration configuration)
    {
        // Redis Caching - Configure options from configuration section
        var redisSection = configuration.GetSection("Redis");
        services.Configure<RedisCacheOptions>(options =>
        {
            options.ConnectionString = redisSection["ConnectionString"] ?? "localhost:6379";
            options.Database = int.TryParse(redisSection["Database"], out var db) ? db : 0;
            options.KeyPrefix = redisSection["KeyPrefix"] ?? "wekeza:";
            if (TimeSpan.TryParse(redisSection["DefaultExpiration"], out var expiration))
                options.DefaultExpiration = expiration;
        });
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var connectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            try
            {
                return ConnectionMultiplexer.Connect(connectionString);
            }
            catch
            {
                // If Redis is not available, return null and services will gracefully degrade
                return null!;
            }
        });
        services.AddScoped<ICacheService, RedisCacheService>();

        // Performance Monitoring
        services.AddScoped<IPerformanceMonitoringService, PerformanceMonitoringService>();

        // Real-time Notifications - temporarily disabled due to SignalR dependency issues
        // TODO: Update SignalR package and re-enable
        // services.AddScoped<INotificationService, NotificationService>();
        // services.AddSignalR(options =>
        // {
        //     options.EnableDetailedErrors = true;
        //     options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        //     options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        // });

        // API Gateway
        services.AddScoped<IApiGatewayService, ApiGatewayService>();

        // Memory Caching (In-Memory Cache for frequently accessed data)
        services.AddMemoryCache();

        // Health Checks
        services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("redis")
            .AddCheck<DatabaseHealthCheck>("database")
            .AddCheck<ApiGatewayHealthCheck>("api-gateway");

        // Background Services for Week 14 - Commented out temporarily to resolve DI issues
        // TODO: Fix background services to properly use scoped services via IServiceScopeFactory
        // services.AddHostedService<PerformanceMonitoringBackgroundService>();
        // services.AddHostedService<CacheWarmupBackgroundService>();
        // services.AddHostedService<HealthCheckBackgroundService>();
    }
}
