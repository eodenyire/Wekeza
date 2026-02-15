using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class ATMTransactionConfiguration : IEntityTypeConfiguration<ATMTransaction>
{
    public void Configure(EntityTypeBuilder<ATMTransaction> builder)
    {
        builder.ToTable("ATMTransactions");
        builder.HasKey(a => a.Id);

        // Money Value Objects
        builder.OwnsOne(a => a.Amount, nav =>
        {
            nav.Property(p => p.Amount).HasColumnName("Amount").HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasColumnName("CurrencyCode").HasMaxLength(3);
            });
        });

        builder.OwnsOne(a => a.InterchangeFee, nav =>
        {
            nav.Property(p => p.Amount).HasColumnName("InterchangeFeeAmount").HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasColumnName("InterchangeFeeCurrency").HasMaxLength(3);
            });
        });

        builder.OwnsOne(a => a.ATMFee, nav =>
        {
            nav.Property(p => p.Amount).HasColumnName("ATMFeeAmount").HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasColumnName("ATMFeeCurrency").HasMaxLength(3);
            });
        });

        builder.OwnsOne(a => a.AccountBalanceAfter, nav =>
        {
            nav.Property(p => p.Amount).HasColumnName("AccountBalanceAfterAmount").HasPrecision(18, 2);
            nav.OwnsOne(p => p.Currency, c =>
            {
                c.Property(x => x.Code).HasColumnName("AccountBalanceAfterCurrency").HasMaxLength(3);
            });
        });

        // Indexes
        builder.HasIndex(a => a.AccountId);
        builder.HasIndex(a => a.ATMId);
        builder.HasIndex(a => a.TransactionDateTime);
        builder.HasIndex(a => a.Status);
    }
}
