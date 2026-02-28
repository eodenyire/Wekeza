using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for TaskAssignment aggregate
/// </summary>
public class TaskAssignmentConfiguration : IEntityTypeConfiguration<TaskAssignment>
{
    public void Configure(EntityTypeBuilder<TaskAssignment> builder)
    {
        builder.ToTable("TaskAssignments");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TaskCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.AssignedTo)
            .HasMaxLength(100);

        builder.Property(t => t.AssignedBy)
            .HasMaxLength(100);

        builder.Property(t => t.CompletedBy)
            .HasMaxLength(100);

        builder.Property(t => t.CompletionNotes)
            .HasMaxLength(2000);

        builder.Property(t => t.CancellationReason)
            .HasMaxLength(1000);

        builder.Property(t => t.BranchCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(t => t.EscalationReason)
            .HasMaxLength(1000);

        // Enum conversions
        builder.Property(t => t.TaskType)
            .HasConversion<int>();

        builder.Property(t => t.Status)
            .HasConversion<int>();

        builder.Property(t => t.Priority)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(t => t.TaskCode).IsUnique();
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.AssignedTo);
        builder.HasIndex(t => t.CreatedBy);
        builder.HasIndex(t => t.DueDate);
        builder.HasIndex(t => t.BranchCode);
        builder.HasIndex(t => new { t.RelatedEntityType, t.RelatedEntityId });

        // Navigation properties
        builder.HasMany(t => t.Comments)
            .WithOne()
            .HasForeignKey("TaskId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Attachments)
            .WithOne()
            .HasForeignKey("TaskId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Dependencies)
            .WithOne()
            .HasForeignKey("TaskId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for TaskComment
/// </summary>
public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        builder.ToTable("TaskComments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Comment)
            .IsRequired()
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(c => c.TaskId);
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.CreatedAt);
    }
}

/// <summary>
/// Entity Framework configuration for TaskAttachment
/// </summary>
public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
{
    public void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        builder.ToTable("TaskAttachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.UploadedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(a => a.TaskId);
        builder.HasIndex(a => a.UploadedBy);
        builder.HasIndex(a => a.UploadedAt);
    }
}

/// <summary>
/// Entity Framework configuration for TaskDependency
/// </summary>
public class TaskDependencyConfiguration : IEntityTypeConfiguration<TaskDependency>
{
    public void Configure(EntityTypeBuilder<TaskDependency> builder)
    {
        builder.ToTable("TaskDependencies");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.AddedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.CompletedBy)
            .HasMaxLength(100);

        // Enum conversion
        builder.Property(d => d.DependencyType)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(d => new { d.TaskId, d.DependentTaskId }).IsUnique();
        builder.HasIndex(d => d.IsCompleted);
    }
}