using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for ExceptionCase aggregate
/// </summary>
public class ExceptionCaseConfiguration : IEntityTypeConfiguration<ExceptionCase>
{
    public void Configure(EntityTypeBuilder<ExceptionCase> builder)
    {
        builder.ToTable("ExceptionCases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CaseNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(c => c.CaseNumber)
            .IsUnique();

        builder.Property(c => c.CaseTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CaseDescription)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.ExceptionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(c => c.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(c => c.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.EntityId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.ResolvedAt);

        builder.Property(c => c.ResolvedBy)
            .HasMaxLength(200);

        builder.Property(c => c.AssignedToUserId);

        builder.Property(c => c.AssignedToUser)
            .HasMaxLength(200);

        builder.Property(c => c.AssignedAt);

        builder.Property(c => c.RootCauseAnalysis)
            .HasMaxLength(2000);

        builder.Property(c => c.Resolution)
            .HasMaxLength(2000);

        builder.Property(c => c.ResolutionAction)
            .HasMaxLength(1000);

        builder.Property(c => c.RequiresApproval)
            .IsRequired();

        builder.Property(c => c.ApprovalStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.FinancialImpact);

        builder.Property(c => c.OperationalImpact)
            .HasMaxLength(1000);

        builder.Property(c => c.RegulatoryImpact)
            .HasMaxLength(1000);

        builder.Property(c => c.SLA_DueDate);

        builder.Property(c => c.IsEscalated)
            .IsRequired();

        builder.Property(c => c.EscalatedAt);

        builder.Property(c => c.EscalationReason)
            .HasMaxLength(1000);

        builder.Property(c => c.EscalationLevel)
            .IsRequired();

        builder.Property(c => c.CommentCount)
            .IsRequired();

        // Store complex types as JSON
        builder.Property(c => c.EntityDetails)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        builder.Property(c => c.AffectedRecords)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(4000);

        builder.Property(c => c.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2000);

        // Owned collections stored as JSON
        builder.OwnsMany(c => c.Comments, comm =>
        {
            comm.ToJson();
                comm.Ignore(x => x.Id); // Guid Id conflicts with ToJson implicit ordinal key
                comm.Property(x => x.CommentedBy).HasMaxLength(200).IsRequired();
            comm.Property(x => x.CommentText).HasMaxLength(2000).IsRequired();
            comm.Property(x => x.IsInternalNote).IsRequired();
        });

        builder.OwnsMany(c => c.Attachments, att =>
        {
            att.ToJson();
                att.Ignore(x => x.Id); // Guid Id conflicts with ToJson implicit ordinal key
                att.Property(x => x.FileName).HasMaxLength(500).IsRequired();
            att.Property(x => x.FileUrl).HasMaxLength(1000).IsRequired();
            att.Property(x => x.UploadedBy).HasMaxLength(200).IsRequired();
            att.Property(x => x.FileSizeBytes).IsRequired();
        });

        builder.OwnsMany(c => c.AssignmentHistory, ah =>
        {
            ah.ToJson();
                ah.Ignore(x => x.Id); // Guid Id conflicts with ToJson implicit ordinal key
                ah.Property(x => x.UserName).HasMaxLength(200).IsRequired();
            ah.Property(x => x.UserId).IsRequired();
            ah.Property(x => x.AssignedBy).HasMaxLength(200).IsRequired();
        });

        builder.OwnsMany(c => c.ApprovalChain, ac =>
        {
            ac.ToJson();
                ac.Ignore(x => x.Id); // Guid Id conflicts with ToJson implicit ordinal key
                ac.Property(x => x.ApproverId).IsRequired();
            ac.Property(x => x.ApprovedBy).HasMaxLength(200);
            ac.Property(x => x.RequestedBy).HasMaxLength(200).IsRequired();
            ac.Property(x => x.Status).IsRequired();
            ac.Property(x => x.Comments).HasMaxLength(1000);
        });

        // Indexes for performance
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.Priority);
        builder.HasIndex(c => c.Category);
        builder.HasIndex(c => c.CreatedAt);
        builder.HasIndex(c => c.AssignedToUserId);
        builder.HasIndex(c => c.SLA_DueDate);
        builder.HasIndex(c => c.IsEscalated);
    }
}
