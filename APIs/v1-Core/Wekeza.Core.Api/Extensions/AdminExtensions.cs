using Wekeza.Core.Application.Admin;

namespace Wekeza.Core.Api.Extensions;

/// <summary>
/// Dependency Injection configuration for Admin Portal services
/// Registers System Admin and Operations Admin services
/// </summary>
public static class AdminExtensions
{
    public static IServiceCollection AddAdminServices(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<ISystemAdminService, SystemAdminService>();
        services.AddScoped<IOpsAdminService, OpsAdminService>();

        // Note: Repository implementations should be registered in 
        // Infrastructure layer DI extension (InfrastructureServiceCollectionExtension)
        // This includes:
        // - IUserRepository
        // - IRoleRepository
        // - IAdminSessionRepository
        // - ISystemConfigurationRepository
        // - IAuditLogRepository
        // - IAccountRepository
        // - ITransactionRepository
        // - IBatchJobRepository
        // - IExceptionCaseRepository
        // - IApprovalRepository
        // - INotificationService

        return services;
    }

    /// <summary>
    /// This method should be called as part of Program.cs setup
    /// Example:
    /// builder.Services.AddApplicationServices();
    /// builder.Services.AddInfrastructureServices(builder.Configuration);
    /// builder.Services.AddAdminServices(); // Add this line
    /// </summary>
    public static WebApplicationBuilder AddAdminPortal(this WebApplicationBuilder builder)
    {
        builder.Services.AddAdminServices();
        return builder;
    }
}
