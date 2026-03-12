using Microsoft.Extensions.DependencyInjection;
using Wekeza.Nexus.Application.Services;
using Wekeza.Nexus.Domain.Interfaces;

namespace Wekeza.Nexus.Application;

/// <summary>
/// Dependency injection configuration for Wekeza Nexus
/// 
/// Usage in Program.cs or Startup.cs:
/// builder.Services.AddWekezaNexus();
/// 
/// Note: Repository implementation should be registered in the Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWekezaNexus(this IServiceCollection services)
    {
        // Register Domain Services
        services.AddScoped<IFraudEvaluationService, FraudEvaluationService>();
        services.AddScoped<ITransactionVelocityService, TransactionVelocityService>();
        
        // Register Client
        services.AddScoped<WekezaNexusClient>();
        
        return services;
    }
}
