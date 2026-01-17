using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
///<summary>
///📂 2. Wekeza.Core.Infrastructure/Persistence/Configurations
///To be "IBM-grade," we don't let EF Core guess our database schema. We define it explicitly.
///</summary>
namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.Id);

        // Basic properties
        builder.Property(a => a.CustomerId).IsRequired();
        builder.Property(a => a.ProductId).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.OpenedDate).IsRequired();
        builder.Property(a => a.ClosedDate);
        builder.Property(a => a.OpenedBy).HasMaxLength(100).IsRequired();
        builder.Property(a => a.ClosedBy).HasMaxLength(100);
        builder.Property(a => a.InterestRate).HasPrecision(5, 4);
        builder.Property(a => a.LastInterestCalculationDate).IsRequired();
        builder.Property(a => a.CustomerGLCode).HasMaxLength(20).IsRequired();

        // Security: Ensure AccountNumber is unique and indexed for speed
        builder.OwnsOne(a => a.AccountNumber, nav =>
        {
            nav.Property(p => p.Value).HasColumnName("AccountNumber").IsRequired().HasMaxLength(20);
            nav.HasIndex(p => p.Value).IsUnique();
        });

        // Money Value Object Mapping - Balance
        builder.OwnsOne(a => a.Balance, nav =>
        {
            nav.Property(p => p.Amount).HasColumnName("BalanceAmount").HasPrecision(18, 4);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasColumnName("CurrencyCode").HasMaxLength(3);
            });
        });

        // Money Value Object Mapping - OverdraftLimit
        builder.OwnsOne(a => a.OverdraftLimit, nav =>
        {
            nav.Property(p => p.Amount).HasColumnName("OverdraftLimitAmount").HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasColumnName("OverdraftLimitCurrency").HasMaxLength(3);
            });
        });

        // Money Value Object Mapping - AccruedInterest
        builder.OwnsOne(a => a.AccruedInterest, nav =>
        {
            nav.Property(p => p.Amount).HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasMaxLength(3);
            });
        });

        // Money Value Object Mapping - DailyTransactionLimit (nullable)
        builder.OwnsOne(a => a.DailyTransactionLimit, nav =>
        {
            nav.Property(p => p.Amount).HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasMaxLength(3);
            });
        });

        // Money Value Object Mapping - MonthlyTransactionLimit (nullable)
        builder.OwnsOne(a => a.MonthlyTransactionLimit, nav =>
        {
            nav.Property(p => p.Amount).HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasMaxLength(3);
            });
        });

        // Money Value Object Mapping - MinimumBalance (nullable)
        builder.OwnsOne(a => a.MinimumBalance, nav =>
        {
            nav.Property(p => p.Amount).HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasMaxLength(3);
            });
        });

        // Relationships
        builder.HasOne(a => a.Customer)
            .WithMany()
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Product)
            .WithMany()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(a => a.CustomerId);
        builder.HasIndex(a => a.ProductId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.CustomerGLCode);

        // Concurrency Token: The "No Double Spend" guard at the DB level
        builder.Property<byte[]>("RowVersion").IsRowVersion();
    }
}
