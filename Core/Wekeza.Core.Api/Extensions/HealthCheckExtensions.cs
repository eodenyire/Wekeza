///📂 Wekeza.Core.Api/Extensions/
///1. HealthCheckExtensions.cs (The Heartbeat)
///In a production environment, load balancers (like Nginx or AWS ALB) need to know if the bank is healthy. If the database connection drops, this extension tells the load balancer to stop sending traffic to this specific instance.
///
///
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Wekeza.Core.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddWekezaHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            // 1. Check if the PostgreSQL Database is reachable
            .AddNpgSql(
                connectionString: configuration.GetConnectionString("DefaultConnection")!,
                name: "PostgreSQL",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "sql", "beast" });
            // 2. Check if the Identity Provider/Auth system is reachable
            // .AddUrlGroup(new Uri(configuration["ExternalServices:AuthUrl"]!), "IdentityServer");

        return services;
    }
}
