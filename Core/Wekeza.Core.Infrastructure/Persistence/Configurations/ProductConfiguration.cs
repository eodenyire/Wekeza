using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
using System.Text.Json;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProductCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.MarketingDescription)
            .HasMaxLength(2000);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastModifiedBy)
            .HasMaxLength(100);

        // JSON columns for complex objects
        builder.Property(p => p.InterestConfig)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<InterestConfiguration>(v, (JsonSerializerOptions)null!));

        builder.Property(p => p.Fees)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<FeeConfiguration>>(v, (JsonSerializerOptions)null!) ?? new List<FeeConfiguration>());

        builder.Property(p => p.Limits)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<LimitConfiguration>(v, (JsonSerializerOptions)null!));

        builder.Property(p => p.EligibilityRules)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<EligibilityRule>>(v, (JsonSerializerOptions)null!) ?? new List<EligibilityRule>());

        builder.Property(p => p.Attributes)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<string, string>());

        builder.Property(p => p.AccountingConfig)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<AccountingConfiguration>(v, (JsonSerializerOptions)null!));

        // Indexes for performance
        builder.HasIndex(p => p.ProductCode).IsUnique();
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.EffectiveDate);
        builder.HasIndex(p => p.ExpiryDate);
        builder.HasIndex(p => new { p.Category, p.Status });
    }
}
