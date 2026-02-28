using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class LimitDefinitionConfiguration : IEntityTypeConfiguration<LimitDefinition>
{
    public void Configure(EntityTypeBuilder<LimitDefinition> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.LimitCode).IsRequired().HasMaxLength(30);
        builder.Property(l => l.LimitType).IsRequired().HasMaxLength(50);
        builder.Property(l => l.LimitAmount).HasPrecision(18, 2);
        builder.Property(l => l.Status).IsRequired().HasMaxLength(30);
        builder.Property(l => l.Hierarchy).HasMaxLength(100);
        builder.HasIndex(l => l.LimitCode).IsUnique();
        builder.HasIndex(l => new { l.LimitType, l.Status });
        builder.HasIndex(l => l.Hierarchy);
    }
}

public class ThresholdConfigConfiguration : IEntityTypeConfiguration<ThresholdConfig>
{
    public void Configure(EntityTypeBuilder<ThresholdConfig> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.ThresholdCode).IsRequired().HasMaxLength(30);
        builder.Property(t => t.ThresholdType).IsRequired().HasMaxLength(50);
        builder.Property(t => t.ThresholdValue).HasPrecision(18, 2);
        builder.Property(t => t.WarningLevel).HasPrecision(10, 2);
        builder.Property(t => t.CriticalLevel).HasPrecision(10, 2);
        builder.Property(t => t.Status).IsRequired().HasMaxLength(30);
        builder.HasIndex(t => t.ThresholdCode).IsUnique();
        builder.HasIndex(t => new { t.ThresholdType, t.Status });
    }
}

public class AnomalyConfiguration : IEntityTypeConfiguration<Anomaly>
{
    public void Configure(EntityTypeBuilder<Anomaly> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AnomalyCode).IsRequired().HasMaxLength(30);
        builder.Property(a => a.Severity).IsRequired().HasMaxLength(20);
        builder.Property(a => a.Status).IsRequired().HasMaxLength(30);
        builder.Property(a => a.DetectedAt).IsRequired();
        builder.Property(a => a.DetectionScore).HasPrecision(10, 4);
        builder.HasIndex(a => a.AnomalyCode).IsUnique();
        builder.HasIndex(a => new { a.Severity, a.Status });
        builder.HasIndex(a => a.DetectedAt);
        builder.HasIndex(a => new { a.Status, a.DetectedAt });
    }
}

public class AnomalyRuleConfiguration : IEntityTypeConfiguration<AnomalyRule>
{
    public void Configure(EntityTypeBuilder<AnomalyRule> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RuleCode).IsRequired().HasMaxLength(30);
        builder.Property(r => r.RuleName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.RuleType).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Status).IsRequired().HasMaxLength(30);
        builder.Property(r => r.Sensitivity).HasPrecision(10, 4);
        builder.HasIndex(r => r.RuleCode).IsUnique();
        builder.HasIndex(r => new { r.RuleType, r.Status });
    }
}

public class LimitDefinition
{
    public Guid Id { get; set; }
    public string LimitCode { get; set; }
    public string LimitType { get; set; }
    public decimal LimitAmount { get; set; }
    public string Status { get; set; }
    public string Hierarchy { get; set; }
}

public class ThresholdConfig
{
    public Guid Id { get; set; }
    public string ThresholdCode { get; set; }
    public string ThresholdType { get; set; }
    public decimal ThresholdValue { get; set; }
    public decimal WarningLevel { get; set; }
    public decimal CriticalLevel { get; set; }
    public string Status { get; set; }
}

public class Anomaly
{
    public Guid Id { get; set; }
    public string AnomalyCode { get; set; }
    public string Severity { get; set; }
    public string Status { get; set; }
    public DateTime DetectedAt { get; set; }
    public decimal DetectionScore { get; set; }
}

public class AnomalyRule
{
    public Guid Id { get; set; }
    public string RuleCode { get; set; }
    public string RuleName { get; set; }
    public string RuleType { get; set; }
    public string Status { get; set; }
    public decimal Sensitivity { get; set; }
}
