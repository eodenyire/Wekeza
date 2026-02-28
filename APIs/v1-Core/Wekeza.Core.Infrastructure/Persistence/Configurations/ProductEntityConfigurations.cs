using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class ProductTemplateConfiguration : IEntityTypeConfiguration<ProductTemplate>
{
    public void Configure(EntityTypeBuilder<ProductTemplate> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ProductCode).IsRequired().HasMaxLength(20);
        builder.Property(p => p.ProductType).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Status).IsRequired().HasMaxLength(30);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.ModifiedAt).IsRequired();
        builder.HasIndex(p => p.ProductCode).IsUnique();
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => new { p.ProductType, p.Status });
    }
}

public class FeeStructureConfiguration : IEntityTypeConfiguration<FeeStructure>
{
    public void Configure(EntityTypeBuilder<FeeStructure> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.ProductCode).IsRequired().HasMaxLength(20);
        builder.Property(f => f.FeeName).IsRequired().HasMaxLength(100);
        builder.Property(f => f.FeeAmount).HasPrecision(18, 2);
        builder.Property(f => f.FeePercentage).HasPrecision(10, 4);
        builder.Property(f => f.Status).IsRequired().HasMaxLength(30);
        builder.HasIndex(f => f.ProductCode);
        builder.HasIndex(f => new { f.ProductCode, f.Status });
    }
}

public class InterestRateTableConfiguration : IEntityTypeConfiguration<InterestRateTable>
{
    public void Configure(EntityTypeBuilder<InterestRateTable> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductCode).IsRequired().HasMaxLength(20);
        builder.Property(i => i.EffectiveDate).IsRequired();
        builder.Property(i => i.BaseRate).HasPrecision(10, 4);
        builder.Property(i => i.Spread).HasPrecision(10, 4);
        builder.Property(i => i.Status).IsRequired().HasMaxLength(30);
        builder.HasIndex(i => i.ProductCode);
        builder.HasIndex(i => new { i.ProductCode, i.EffectiveDate });
    }
}

public class PostingRuleConfiguration : IEntityTypeConfiguration<PostingRule>
{
    public void Configure(EntityTypeBuilder<PostingRule> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ProductCode).IsRequired().HasMaxLength(20);
        builder.Property(p => p.TransactionType).IsRequired().HasMaxLength(50);
        builder.Property(p => p.DebitAccount).IsRequired().HasMaxLength(20);
        builder.Property(p => p.CreditAccount).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Status).IsRequired().HasMaxLength(30);
        builder.HasIndex(p => p.ProductCode);
        builder.HasIndex(p => new { p.ProductCode, p.TransactionType });
    }
}

public class ProductTemplate
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public string ProductType { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

public class FeeStructure
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public string FeeName { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal FeePercentage { get; set; }
    public string Status { get; set; }
}

public class InterestRateTable
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal BaseRate { get; set; }
    public decimal Spread { get; set; }
    public string Status { get; set; }
}

public class PostingRule
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public string TransactionType { get; set; }
    public string DebitAccount { get; set; }
    public string CreditAccount { get; set; }
    public string Status { get; set; }
}
