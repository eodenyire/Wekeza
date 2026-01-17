using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
///2. TransactionConfiguration.cs (Indexing & Performance)
///The Ledger is your "hottest" table. We must ensure that generating a 3-month statement for a customer in Nairobi takes milliseconds, not seconds.
///
///
namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);

        // Performance: Composite Index for fast statement generation
        // Allows filtering by Account and sorting by Date simultaneously
        builder.HasIndex(t => new { t.AccountId, t.Timestamp })
               .HasDatabaseName("IX_Transactions_AccountId_Timestamp");

        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.CorrelationId).IsRequired();
    }
}
