using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
using System.Text.Json;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class GLAccountConfiguration : IEntityTypeConfiguration<GLAccount>
{
    public void Configure(EntityTypeBuilder<GLAccount> builder)
    {
        builder.ToTable("GLAccounts");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.GLCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(g => g.GLName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.ParentGLCode)
            .HasMaxLength(20);

        builder.Property(g => g.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(g => g.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.LastModifiedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(g => g.GLCode).IsUnique();
        builder.HasIndex(g => g.AccountType);
        builder.HasIndex(g => g.Category);
        builder.HasIndex(g => g.Status);
        builder.HasIndex(g => g.ParentGLCode);
        builder.HasIndex(g => g.IsLeaf);
        builder.HasIndex(g => new { g.AccountType, g.Status });
    }
}

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.JournalNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.SourceType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.SourceReference)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(j => j.Description)
            .HasMaxLength(500);

        builder.Property(j => j.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.PostedBy)
            .HasMaxLength(100);

        builder.Property(j => j.ReversedBy)
            .HasMaxLength(100);

        // JSON column for lines
        builder.Property(j => j.Lines)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<JournalLine>>(v, (JsonSerializerOptions)null!) ?? new List<JournalLine>());

        // Indexes
        builder.HasIndex(j => j.JournalNumber).IsUnique();
        builder.HasIndex(j => j.PostingDate);
        builder.HasIndex(j => j.ValueDate);
        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => new { j.SourceType, j.SourceId });
        builder.HasIndex(j => new { j.PostingDate, j.Status });
    }
}
