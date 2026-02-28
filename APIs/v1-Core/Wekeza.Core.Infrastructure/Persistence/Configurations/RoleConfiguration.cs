using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);

        // Basic properties
        builder.Property(r => r.RoleCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.RoleName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.RequiredClearance)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.LastModifiedBy)
            .HasMaxLength(200);

        // Boolean properties
        builder.Property(r => r.InheritsPermissions).IsRequired();
        builder.Property(r => r.RequiresMfa).IsRequired();
        builder.Property(r => r.RequiresApproval).IsRequired();

        // Numeric properties
        builder.Property(r => r.TransactionLimit).HasPrecision(18, 4);
        builder.Property(r => r.DailyLimit).HasPrecision(18, 4);
        builder.Property(r => r.MaxConcurrentUsers).IsRequired();

        // Owned collection for Permissions - stored as JSON with minimal property configuration
        // EF Core will serialize the entire Permission object as JSON
        builder.OwnsMany(r => r.Permissions, p =>
        {
            p.ToJson();
        });

        // List properties stored as CSV
        builder.Property(r => r.ChildRoleIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList())
            .HasMaxLength(2000);

        builder.Property(r => r.AllowedModules)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(r => r.RestrictedModules)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(r => r.IpRestrictions)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        builder.Property(r => r.ApprovalWorkflow)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        // Dictionary stored as JSON
        builder.Property(r => r.ModuleAccess)
            .HasColumnType("jsonb");

        builder.Property(r => r.Metadata)
            .HasColumnType("jsonb");

        // TimeRestrictions stored as JSON
        builder.OwnsMany(r => r.TimeRestrictions, t =>
        {
            t.ToJson();
        });

        // Unique index on RoleCode
        builder.HasIndex(r => r.RoleCode).IsUnique();

        // Indexes for performance
        builder.HasIndex(r => r.Type);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.CreatedAt);
    }
}
