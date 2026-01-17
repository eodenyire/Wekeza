using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for ApprovalWorkflow aggregate
/// </summary>
public class ApprovalWorkflowConfiguration : IEntityTypeConfiguration<ApprovalWorkflow>
{
    public void Configure(EntityTypeBuilder<ApprovalWorkflow> builder)
    {
        builder.ToTable("ApprovalWorkflows");

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

        builder.Property(w => w.InitiatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.BranchCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.CompletedBy)
            .HasMaxLength(100);

        builder.Property(w => w.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(w => w.EscalationReason)
            .HasMaxLength(1000);

        // Money value object configuration
        builder.OwnsOne(w => w.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("Currency")
                    .HasMaxLength(3);
            });
        });

        // Enum conversions
        builder.Property(w => w.WorkflowType)
            .HasConversion<int>();

        builder.Property(w => w.Status)
            .HasConversion<int>();

        builder.Property(w => w.Priority)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(w => w.WorkflowCode).IsUnique();
        builder.HasIndex(w => new { w.EntityType, w.EntityId });
        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.BranchCode);
        builder.HasIndex(w => w.InitiatedBy);
        builder.HasIndex(w => w.DueDate);

        // Navigation properties
        builder.HasMany(w => w.ApprovalSteps)
            .WithOne()
            .HasForeignKey("WorkflowId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Comments)
            .WithOne()
            .HasForeignKey("WorkflowId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Documents)
            .WithOne()
            .HasForeignKey("WorkflowId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for ApprovalStep
/// </summary>
public class ApprovalStepConfiguration : IEntityTypeConfiguration<ApprovalStep>
{
    public void Configure(EntityTypeBuilder<ApprovalStep> builder)
    {
        builder.ToTable("ApprovalSteps");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ApproverRole)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.SpecificApprover)
            .HasMaxLength(100);

        builder.Property(s => s.ProcessedBy)
            .HasMaxLength(100);

        builder.Property(s => s.Comments)
            .HasMaxLength(1000);

        // Money value objects
        builder.OwnsOne(s => s.MinimumAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("MinimumAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("MinimumAmountCurrency")
                    .HasMaxLength(3);
            });
        });

        builder.OwnsOne(s => s.MaximumAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("MaximumAmount")
                .HasPrecision(18, 2);

            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("MaximumAmountCurrency")
                    .HasMaxLength(3);
            });
        });

        // Enum conversion
        builder.Property(s => s.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(s => new { s.WorkflowId, s.Level });
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.SpecificApprover);
    }
}

/// <summary>
/// Entity Framework configuration for WorkflowComment
/// </summary>
public class WorkflowCommentConfiguration : IEntityTypeConfiguration<WorkflowComment>
{
    public void Configure(EntityTypeBuilder<WorkflowComment> builder)
    {
        builder.ToTable("WorkflowComments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Comment)
            .IsRequired()
            .HasMaxLength(2000);

        // Enum conversion
        builder.Property(c => c.CommentType)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(c => c.WorkflowId);
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.CreatedAt);
    }
}

/// <summary>
/// Entity Framework configuration for WorkflowDocument
/// </summary>
public class WorkflowDocumentConfiguration : IEntityTypeConfiguration<WorkflowDocument>
{
    public void Configure(EntityTypeBuilder<WorkflowDocument> builder)
    {
        builder.ToTable("WorkflowDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.UploadedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(d => d.WorkflowId);
        builder.HasIndex(d => d.UploadedBy);
        builder.HasIndex(d => d.UploadedAt);
    }
}