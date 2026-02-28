using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Branch aggregate
/// </summary>
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BranchCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.BranchName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.TimeZone)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.ManagerId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.DeputyManagerId)
            .HasMaxLength(100);

        builder.Property(b => b.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.ModifiedBy)
            .HasMaxLength(100);

        // Money value objects
        builder.OwnsOne(b => b.CashLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("CashLimit")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("CashLimitCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(b => b.TransactionLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TransactionLimit")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("TransactionLimitCurrency")
                    .HasMaxLength(3);
            });
        });

        // Enum conversions
        builder.Property(b => b.BranchType)
            .HasConversion<int>();

        builder.Property(b => b.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(b => b.BranchCode).IsUnique();
        builder.HasIndex(b => b.BranchName);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.BranchType);
        builder.HasIndex(b => b.ManagerId);
        builder.HasIndex(b => b.City);

        // Navigation properties
        builder.HasMany(b => b.Vaults)
            .WithOne()
            .HasForeignKey("BranchId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Limits)
            .WithOne()
            .HasForeignKey("BranchId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.PerformanceMetrics)
            .WithOne()
            .HasForeignKey("BranchId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for BranchVault
/// </summary>
public class BranchVaultConfiguration : IEntityTypeConfiguration<BranchVault>
{
    public void Configure(EntityTypeBuilder<BranchVault> builder)
    {
        builder.ToTable("BranchVaults");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VaultCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.VaultName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.ModifiedBy)
            .HasMaxLength(100);

        // Money value objects
        builder.OwnsOne(v => v.Capacity, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Capacity")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("CapacityCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(v => v.CurrentBalance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("CurrentBalance")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("CurrentBalanceCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(v => v.DailyLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DailyLimit")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("DailyLimitCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(v => v.DailyUsed, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DailyUsed")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("DailyUsedCurrency")
                    .HasMaxLength(3);
            });
        });

        // Enum conversion
        builder.Property(v => v.VaultType)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(v => new { v.BranchId, v.VaultCode }).IsUnique();
        builder.HasIndex(v => v.VaultType);
        builder.HasIndex(v => v.IsActive);
    }
}

/// <summary>
/// Entity Framework configuration for BranchLimit
/// </summary>
public class BranchLimitConfiguration : IEntityTypeConfiguration<BranchLimit>
{
    public void Configure(EntityTypeBuilder<BranchLimit> builder)
    {
        builder.ToTable("BranchLimits");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.LimitType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.ModifiedBy)
            .HasMaxLength(100);

        // Money value object
        builder.OwnsOne(l => l.LimitAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("LimitAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("LimitAmountCurrency")
                    .HasMaxLength(3);
            });
        });

        // Indexes
        builder.HasIndex(l => new { l.BranchId, l.LimitType }).IsUnique();
    }
}

/// <summary>
/// Entity Framework configuration for BranchPerformance
/// </summary>
public class BranchPerformanceConfiguration : IEntityTypeConfiguration<BranchPerformance>
{
    public void Configure(EntityTypeBuilder<BranchPerformance> builder)
    {
        builder.ToTable("BranchPerformance");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CalculatedBy)
            .IsRequired()
            .HasMaxLength(100);

        // Money value object
        builder.OwnsOne(p => p.TransactionVolume, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TransactionVolume")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("TransactionVolumeCurrency")
                    .HasMaxLength(3);
            });
        });

        // Indexes
        builder.HasIndex(p => new { p.BranchId, p.PerformanceDate }).IsUnique();
        builder.HasIndex(p => p.PerformanceDate);
    }
}