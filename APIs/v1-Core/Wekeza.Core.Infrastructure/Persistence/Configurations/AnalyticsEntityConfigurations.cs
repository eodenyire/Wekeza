using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class CustomDashboardConfiguration : IEntityTypeConfiguration<CustomDashboard>
{
    public void Configure(EntityTypeBuilder<CustomDashboard> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.UserId).IsRequired();
        builder.Property(d => d.DashboardCode).IsRequired().HasMaxLength(30);
        builder.Property(d => d.DashboardName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.DashboardType).IsRequired().HasMaxLength(50);
        builder.Property(d => d.IsDefault).IsRequired();
        builder.Property(d => d.CreatedAt).IsRequired();
        builder.Property(d => d.ModifiedAt).IsRequired();
        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => new { d.UserId, d.IsDefault });
    }
}

public class KPIDefinitionConfiguration : IEntityTypeConfiguration<KPIDefinition>
{
    public void Configure(EntityTypeBuilder<KPIDefinition> builder)
    {
        builder.HasKey(k => k.Id);
        builder.Property(k => k.KPICode).IsRequired().HasMaxLength(30);
        builder.Property(k => k.KPIName).IsRequired().HasMaxLength(100);
        builder.Property(k => k.KPIType).IsRequired().HasMaxLength(50);
        builder.Property(k => k.CalculationFormula).IsRequired();
        builder.Property(k => k.Status).IsRequired().HasMaxLength(30);
        builder.HasIndex(k => k.KPICode).IsUnique();
        builder.HasIndex(k => k.KPIType);
        builder.HasIndex(k => k.Status);
    }
}

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.ReportCode).IsRequired().HasMaxLength(30);
        builder.Property(r => r.ReportName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.ReportType).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Status).IsRequired().HasMaxLength(30);
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.ModifiedAt).IsRequired();
        builder.HasIndex(r => r.ReportCode).IsUnique();
        builder.HasIndex(r => new { r.ReportType, r.Status });
        builder.HasIndex(r => r.CreatedAt);
    }
}

public class SavedAnalysisConfiguration : IEntityTypeConfiguration<SavedAnalysis>
{
    public void Configure(EntityTypeBuilder<SavedAnalysis> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.AnalysisName).IsRequired().HasMaxLength(100);
        builder.Property(a => a.AnalysisType).IsRequired().HasMaxLength(50);
        builder.Property(a => a.SavedAt).IsRequired();
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => new { a.UserId, a.SavedAt });
    }
}

public class CustomDashboard
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DashboardCode { get; set; }
    public string DashboardName { get; set; }
    public string DashboardType { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

public class KPIDefinition
{
    public Guid Id { get; set; }
    public string KPICode { get; set; }
    public string KPIName { get; set; }
    public string KPIType { get; set; }
    public string CalculationFormula { get; set; }
    public string Status { get; set; }
}

public class Report
{
    public Guid Id { get; set; }
    public string ReportCode { get; set; }
    public string ReportName { get; set; }
    public string ReportType { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

public class SavedAnalysis
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AnalysisName { get; set; }
    public string AnalysisType { get; set; }
    public DateTime SavedAt { get; set; }
}
