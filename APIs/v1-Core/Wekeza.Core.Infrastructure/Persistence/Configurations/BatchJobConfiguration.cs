using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for BatchJob aggregate
/// </summary>
public class BatchJobConfiguration : IEntityTypeConfiguration<BatchJob>
{
    public void Configure(EntityTypeBuilder<BatchJob> builder)
    {
        builder.ToTable("BatchJobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.JobCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(j => j.JobCode)
            .IsUnique();

        builder.Property(j => j.JobName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.Description)
            .HasMaxLength(1000);

        builder.Property(j => j.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.ScheduleExpression)
            .HasMaxLength(200);

        builder.Property(j => j.NextScheduledRun);
        builder.Property(j => j.LastRunTime);

        builder.Property(j => j.TimeoutSeconds)
            .IsRequired();

        builder.Property(j => j.RetryCount)
            .IsRequired();

        builder.Property(j => j.MaxConcurrentRuns)
            .IsRequired();

        builder.Property(j => j.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(j => j.CurrentExecutionStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(j => j.IsEnabled)
            .IsRequired();

        builder.Property(j => j.IsRunning)
            .IsRequired();

        builder.Property(j => j.CurrentRunStartedAt);

        builder.Property(j => j.TotalRecordsProcessed)
            .IsRequired();

        builder.Property(j => j.TotalRecordsFailed)
            .IsRequired();

        builder.Property(j => j.AverageExecutionTime)
            .IsRequired();

        builder.Property(j => j.LastErrorMessage)
            .HasMaxLength(2000);

        builder.Property(j => j.LastErrorTime);

        builder.Property(j => j.CreatedAt)
            .IsRequired();

        builder.Property(j => j.CreatedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.LastModifiedAt);

        builder.Property(j => j.LastModifiedBy)
            .HasMaxLength(200);

        // Store complex types as JSON
        builder.Property(j => j.Parameters)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        builder.Property(j => j.Dependencies)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        // Owned collections stored as JSON
        builder.OwnsMany(j => j.ExecutionHistory, e =>
        {
            e.ToJson();
            e.Property(x => x.StartedAt).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50);
            e.Property(x => x.ErrorMessage).HasMaxLength(2000);
            e.Ignore(x => x.RuntimeParameters);
        });

        builder.OwnsMany(j => j.PerformanceMetrics, m =>
        {
            m.ToJson();
            m.Property(x => x.RecordsProcessed).IsRequired();
            m.Property(x => x.RecordsFailed).IsRequired();
            m.Property(x => x.ExecutionTime).IsRequired();
        });

        builder.OwnsMany(j => j.Errors, e =>
        {
            e.ToJson();
            e.Property(x => x.ErrorMessage).HasMaxLength(2000).IsRequired();
            e.Property(x => x.ErrorTime).IsRequired();
            e.Property(x => x.RetryCount).IsRequired();
            e.Ignore(x => x.StackTrace);
        });

        // Indexes for performance
        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => j.Category);
        builder.HasIndex(j => j.IsEnabled);
        builder.HasIndex(j => j.NextScheduledRun);
    }
}
