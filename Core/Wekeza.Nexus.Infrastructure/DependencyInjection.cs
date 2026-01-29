using Microsoft.Extensions.DependencyInjection;
using Wekeza.Nexus.Domain.Interfaces;
using Wekeza.Nexus.Infrastructure.Repositories;

namespace Wekeza.Nexus.Infrastructure;

/// <summary>
/// Dependency injection for Wekeza Nexus Infrastructure layer
/// 
/// Usage in Program.cs:
/// builder.Services.AddWekezaNexusInfrastructure();
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWekezaNexusInfrastructure(this IServiceCollection services)
    {
        // Register Repositories
        // Using in-memory implementation for MVP
        // In production, replace with EF Core implementation
        services.AddSingleton<IFraudEvaluationRepository, InMemoryFraudEvaluationRepository>();
        
        return services;
    }
}
