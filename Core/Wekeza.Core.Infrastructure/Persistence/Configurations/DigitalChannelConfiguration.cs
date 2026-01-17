using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for DigitalChannel aggregate
/// </summary>
public class DigitalChannelConfiguration : IEntityTypeConfiguration<DigitalChannel>
{
    public void Configure(EntityTypeBuilder<DigitalChannel> builder)
    {
        builder.ToTable("DigitalChannels");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ChannelCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.ChannelName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.BaseUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.ApiVersion)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ModifiedBy)
            .HasMaxLength(100);

        // Money value objects
        builder.OwnsOne(c => c.DailyTransactionLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DailyTransactionLimit")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(cur => cur.Code)
                    .HasColumnName("DailyTransactionLimitCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(c => c.SingleTransactionLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("SingleTransactionLimit")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(cur => cur.Code)
                    .HasColumnName("SingleTransactionLimitCurrency")
                    .HasMaxLength(3);
            });
        });

        // Enum conversions
        builder.Property(c => c.ChannelType)
            .HasConversion<int>();

        builder.Property(c => c.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(c => c.ChannelCode).IsUnique();
        builder.HasIndex(c => c.ChannelType);
        builder.HasIndex(c => c.Status);

        // Navigation properties
        builder.HasMany(c => c.Services)
            .WithOne()
            .HasForeignKey("ChannelId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Sessions)
            .WithOne()
            .HasForeignKey("ChannelId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Transactions)
            .WithOne()
            .HasForeignKey("ChannelId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Alerts)
            .WithOne()
            .HasForeignKey("ChannelId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for ChannelService
/// </summary>
public class ChannelServiceConfiguration : IEntityTypeConfiguration<ChannelService>
{
    public void Configure(EntityTypeBuilder<ChannelService> builder)
    {
        builder.ToTable("ChannelServices");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ServiceCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.ServiceName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(s => s.DisabledReason)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(s => new { s.ChannelId, s.ServiceCode }).IsUnique();
        builder.HasIndex(s => s.IsEnabled);
    }
}

/// <summary>
/// Entity Framework configuration for ChannelSession
/// </summary>
public class ChannelSessionConfiguration : IEntityTypeConfiguration<ChannelSession>
{
    public void Configure(EntityTypeBuilder<ChannelSession> builder)
    {
        builder.ToTable("ChannelSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SessionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.DeviceId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // IPv6 max length

        builder.Property(s => s.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.EndReason)
            .HasMaxLength(200);

        // Enum conversion
        builder.Property(s => s.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(s => s.SessionId).IsUnique();
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.StartTime);
        builder.HasIndex(s => s.ExpiryTime);
    }
}

/// <summary>
/// Entity Framework configuration for ChannelTransaction
/// </summary>
public class ChannelTransactionConfiguration : IEntityTypeConfiguration<ChannelTransaction>
{
    public void Configure(EntityTypeBuilder<ChannelTransaction> builder)
    {
        builder.ToTable("ChannelTransactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TransactionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.SessionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.ServiceCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.ProcessedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.FailureReason)
            .HasMaxLength(500);

        // Money value object
        builder.OwnsOne(t => t.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("Currency")
                    .HasMaxLength(3);
            });
        });

        // Enum conversion
        builder.Property(t => t.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(t => t.TransactionId).IsUnique();
        builder.HasIndex(t => t.SessionId);
        builder.HasIndex(t => t.ServiceCode);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.ProcessedAt);
    }
}

/// <summary>
/// Entity Framework configuration for ChannelAlert
/// </summary>
public class ChannelAlertConfiguration : IEntityTypeConfiguration<ChannelAlert>
{
    public void Configure(EntityTypeBuilder<ChannelAlert> builder)
    {
        builder.ToTable("ChannelAlerts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.ResolvedBy)
            .HasMaxLength(100);

        // Enum conversions
        builder.Property(a => a.AlertType)
            .HasConversion<int>();

        builder.Property(a => a.Severity)
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(a => a.AlertType);
        builder.HasIndex(a => a.Severity);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.CreatedAt);
    }
}