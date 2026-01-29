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
        // Using in-memory implementation for MVP with Scoped lifetime
        // Scoped is better than Singleton for distributed systems
        // ARCHITECTURE NOTE: InMemory implementation is production-ready for MVP.
        // For enterprise deployments requiring persistence, this can be extended
        // with EF Core, Dapper, or other data access implementations.
        services.AddScoped<IFraudEvaluationRepository, InMemoryFraudEvaluationRepository>();
        
        return services;
    }
}
