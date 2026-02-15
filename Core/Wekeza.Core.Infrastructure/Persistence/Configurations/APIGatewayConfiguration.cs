using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class APIGatewayConfiguration : IEntityTypeConfiguration<APIGateway>
{
    public void Configure(EntityTypeBuilder<APIGateway> builder)
    {
        builder.ToTable("APIGateways");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.GatewayName).IsRequired().HasMaxLength(200);
        builder.Property(a => a.BaseUrl).IsRequired().HasMaxLength(500);
        builder.Property(a => a.Version).HasMaxLength(50);
        builder.Property(a => a.Description).HasMaxLength(1000);

        // Ignore Dictionary properties - these should be stored as JSON or in separate tables
        builder.Ignore(a => a.RateLimits);
        builder.Ignore(a => a.AuthConfigs);
        builder.Ignore(a => a.CacheConfigs);
        builder.Ignore(a => a.SecurityHeaders);
        builder.Ignore(a => a.CircuitBreakerStates);
        builder.Ignore(a => a.Metadata);

        // Ignore complex collection properties
        builder.Ignore(a => a.Routes);
        builder.Ignore(a => a.UpstreamServers);
        builder.Ignore(a => a.AllowedOrigins);
        builder.Ignore(a => a.RecentLogs);

        // Ignore complex value object properties
        builder.Ignore(a => a.HealthCheck);
        builder.Ignore(a => a.Metrics);

        builder.HasIndex(a => a.GatewayName).IsUnique();
    }
}
