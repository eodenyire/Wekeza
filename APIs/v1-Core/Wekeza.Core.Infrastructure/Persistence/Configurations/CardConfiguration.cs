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

        builder.Property(c => c.DailyWithdrawalLimit)
            .HasPrecision(18, 2);

        builder.Property(c => c.DailyWithdrawnToday)
            .HasPrecision(18, 2);

        builder.Property(c => c.CancellationReason)
            .HasMaxLength(200);

        // Relationship with Account
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
