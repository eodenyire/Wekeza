using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
using System.Text.Json;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class WorkflowInstanceConfiguration : IEntityTypeConfiguration<WorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        builder.ToTable("WorkflowInstances");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.WorkflowCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.WorkflowName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.EntityReference)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.InitiatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.CompletedBy)
            .HasMaxLength(100);

        builder.Property(w => w.EscalatedTo)
            .HasMaxLength(100);

        builder.Property(w => w.RequestData)
            .HasColumnType("jsonb");

        // JSON columns for collections
        builder.Property(w => w.ApprovalSteps)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<ApprovalStep>>(v, (JsonSerializerOptions)null!) ?? new List<ApprovalStep>());

        builder.Property(w => w.Comments)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<WorkflowComment>>(v, (JsonSerializerOptions)null!) ?? new List<WorkflowComment>());

        // Indexes
        builder.HasIndex(w => w.WorkflowCode);
        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.EntityType);
        builder.HasIndex(w => new { w.EntityType, w.EntityId });
        builder.HasIndex(w => w.InitiatedBy);
        builder.HasIndex(w => w.DueDate);
        builder.HasIndex(w => w.IsEscalated);
        builder.HasIndex(w => new { w.Status, w.DueDate });
    }
}

public class ApprovalMatrixConfiguration : IEntityTypeConfiguration<ApprovalMatrix>
{
    public void Configure(EntityTypeBuilder<ApprovalMatrix> builder)
    {
        builder.ToTable("ApprovalMatrices");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MatrixCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.MatrixName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.LastModifiedBy)
            .HasMaxLength(100);

        // JSON column for rules
        builder.Property(m => m.Rules)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<ApprovalRule>>(v, (JsonSerializerOptions)null!) ?? new List<ApprovalRule>());

        // Indexes
        builder.HasIndex(m => m.MatrixCode).IsUnique();
        builder.HasIndex(m => m.EntityType);
        builder.HasIndex(m => m.Status);
    }
}
