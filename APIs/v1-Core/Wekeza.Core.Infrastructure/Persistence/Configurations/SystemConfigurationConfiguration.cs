using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for SystemConfiguration aggregate
/// </summary>
public class SystemConfigurationConfiguration : IEntityTypeConfiguration<SystemConfiguration>
{
    public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
    {
        builder.ToTable("SystemConfigurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ConfigCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.ConfigCode)
            .IsUnique();

        builder.Property(c => c.ConfigName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ConfigType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.VersionNumber)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.IsProductionReady)
            .IsRequired();

        builder.Property(c => c.ActivationDate);
        builder.Property(c => c.SuspensionDate);

        builder.Property(c => c.ApprovedBy);
        builder.Property(c => c.ApprovedAt);

        builder.Property(c => c.RequiresRestart)
            .IsRequired();

        builder.Property(c => c.TestResult)
            .HasMaxLength(1000);

        builder.Property(c => c.LastTestedAt);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.LastModifiedAt);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(200);

        builder.Property(c => c.ChangeReason)
            .HasMaxLength(1000);

        // Store complex types as JSON
        builder.Property(c => c.ConfigurationData)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        builder.Property(c => c.TestData)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        builder.Property(c => c.AffectedModules)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(c => c.Dependencies)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(c => c.ReferencedConfigs)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(c => c.ValidationRules)
            .HasConversion(
                v => string.Join('|', v),
                v => v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(4000);

        // Owned collections stored as JSON
        builder.OwnsMany(c => c.ApprovalHistory, a =>
        {
            a.ToJson();
            a.Property(x => x.RequestedBy).HasMaxLength(200);
            a.Property(x => x.ApprovedBy).HasMaxLength(200);
            a.Property(x => x.Status).IsRequired();
            a.Property(x => x.Comments).HasMaxLength(1000);
        });

        builder.OwnsMany(c => c.ChangeHistory, ch =>
        {
            ch.ToJson();
            ch.Property(x => x.Version).HasMaxLength(50);
            ch.Property(x => x.ChangedBy).HasMaxLength(200);
            ch.Property(x => x.ChangeReason).HasMaxLength(1000);
        });

        // Indexes for performance
        builder.HasIndex(c => c.Category);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.CreatedAt);
    }
}
