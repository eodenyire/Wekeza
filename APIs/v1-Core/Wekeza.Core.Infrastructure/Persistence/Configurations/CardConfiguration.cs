using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CardNumber)
            .IsRequired()
            .HasMaxLength(16);

        builder.HasIndex(c => c.CardNumber)
            .IsUnique();

        builder.Property(c => c.CardType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.NameOnCard)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(c => c.DailyWithdrawalLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DailyWithdrawalLimitAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("DailyWithdrawalLimitCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(c => c.DailyPurchaseLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DailyPurchaseLimitAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("DailyPurchaseLimitCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(c => c.MonthlyLimit, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("MonthlyLimitAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("MonthlyLimitCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(c => c.DailyWithdrawnToday, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DailyWithdrawnTodayAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("DailyWithdrawnTodayCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(c => c.DailyPurchasedToday, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("DailyPurchasedTodayAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("DailyPurchasedTodayCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(c => c.MonthlySpent, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("MonthlySpentAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("MonthlySpentCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.Property(c => c.CancellationReason)
            .HasMaxLength(200);

        // Relationship with Account
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
