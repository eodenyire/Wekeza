using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for AdminSession aggregate
/// </summary>
public class AdminSessionConfiguration : IEntityTypeConfiguration<AdminSession>
{
    public void Configure(EntityTypeBuilder<AdminSession> builder)
    {
        builder.ToTable("AdminSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.SessionToken)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(s => s.SessionToken)
            .IsUnique();

        builder.Property(s => s.AdminRole)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // IPv6 max length

        builder.Property(s => s.UserAgent)
            .HasMaxLength(500);

        builder.Property(s => s.Hostname)
            .HasMaxLength(255);

        builder.Property(s => s.DeviceIdFingernprint)
            .HasMaxLength(500);

        builder.Property(s => s.LoginAt)
            .IsRequired();

        builder.Property(s => s.LastActivityAt);

        builder.Property(s => s.LogoutAt);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.MfaVerified)
            .IsRequired();

        builder.Property(s => s.AuthenticationMethod)
            .HasMaxLength(100);

        builder.Property(s => s.RiskLevel)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.ActionCount)
            .IsRequired();

        // Owned collections stored as JSON
        builder.OwnsMany(s => s.Actions, a =>
        {
            a.ToJson();
            a.Property(x => x.Module).HasMaxLength(100);
            a.Property(x => x.Action).HasMaxLength(100);
            a.Property(x => x.Resource).HasMaxLength(200);
            a.Property(x => x.ResourceId).HasMaxLength(100);
            a.Property(x => x.OperationType).HasMaxLength(50);
            a.Property(x => x.Status).HasMaxLength(50);
        });

        builder.Property(s => s.AccessedModules)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(s => s.SecurityEvents)
            .HasConversion(
                v => string.Join('|', v),
                v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(4000);

        builder.Property(s => s.AnomaliesDetected)
            .HasConversion(
                v => string.Join('|', v),
                v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(4000);

        // Metadata stored as JSON
        builder.Property(s => s.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        // Indexes for performance
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.LoginAt);
        builder.HasIndex(s => s.LastActivityAt);
    }
}
