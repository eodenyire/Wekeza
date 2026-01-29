using Microsoft.EntityFrameworkCore;
using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Domain.ValueObjects;
using Wekeza.Nexus.Domain.Enums;

namespace Wekeza.Nexus.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for Wekeza Nexus fraud detection system
/// Integrated with PostgreSQL for persistence
/// </summary>
public class NexusDbContext : DbContext
{
    public NexusDbContext(DbContextOptions<NexusDbContext> options) : base(options)
    {
    }

    public DbSet<FraudEvaluation> FraudEvaluations => Set<FraudEvaluation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure FraudEvaluation entity
        modelBuilder.Entity<FraudEvaluation>(entity =>
        {
            entity.ToTable("fraud_evaluations");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();
            
            entity.Property(e => e.TransactionContextId)
                .HasColumnName("transaction_context_id")
                .IsRequired();
            
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            
            entity.Property(e => e.TransactionReference)
                .HasColumnName("transaction_reference")
                .HasMaxLength(100)
                .IsRequired();
            
            entity.HasIndex(e => e.TransactionReference)
                .HasDatabaseName("ix_fraud_evaluations_transaction_reference");
            
            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.Property(e => e.EvaluatedAt)
                .HasColumnName("evaluated_at")
                .IsRequired();
            
            entity.HasIndex(e => e.EvaluatedAt)
                .HasDatabaseName("ix_fraud_evaluations_evaluated_at");
            
            entity.Property(e => e.ProcessingTimeMs)
                .HasColumnName("processing_time_ms")
                .IsRequired();
            
            entity.Property(e => e.ModelVersion)
                .HasColumnName("model_version")
                .HasMaxLength(20)
                .IsRequired();
            
            entity.Property(e => e.WasAllowed)
                .HasColumnName("was_allowed")
                .IsRequired();
            
            entity.Property(e => e.RequiresReview)
                .HasColumnName("requires_review")
                .IsRequired();
            
            entity.HasIndex(e => e.RequiresReview)
                .HasDatabaseName("ix_fraud_evaluations_requires_review");
            
            entity.Property(e => e.AnalystNotes)
                .HasColumnName("analyst_notes")
                .HasMaxLength(2000);
            
            entity.Property(e => e.WasActualFraud)
                .HasColumnName("was_actual_fraud");
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("ix_fraud_evaluations_user_id");
            
            // Configure FraudScore as owned entity (Value Object)
            entity.OwnsOne(e => e.FraudScore, fraudScore =>
            {
                fraudScore.Property(fs => fs.TotalScore)
                    .HasColumnName("fraud_score")
                    .IsRequired();
                
                fraudScore.Property(fs => fs.VelocityScore)
                    .HasColumnName("velocity_score")
                    .IsRequired();
                
                fraudScore.Property(fs => fs.BehavioralScore)
                    .HasColumnName("behavioral_score")
                    .IsRequired();
                
                fraudScore.Property(fs => fs.RelationshipScore)
                    .HasColumnName("relationship_score")
                    .IsRequired();
                
                fraudScore.Property(fs => fs.AmountScore)
                    .HasColumnName("amount_score")
                    .IsRequired();
                
                fraudScore.Property(fs => fs.DeviceScore)
                    .HasColumnName("device_score")
                    .IsRequired();
                
                fraudScore.Property(fs => fs.Decision)
                    .HasColumnName("decision")
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();
                
                fraudScore.Property(fs => fs.RiskLevel)
                    .HasColumnName("risk_level")
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();
                
                fraudScore.Property(fs => fs.Reasons)
                    .HasColumnName("fraud_reasons")
                    .HasConversion(
                        v => string.Join(',', v.Select(r => r.ToString())),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(s => Enum.Parse<FraudReason>(s))
                              .ToList()
                    )
                    .HasMaxLength(500);
                
                fraudScore.Property(fs => fs.Explanation)
                    .HasColumnName("explanation")
                    .HasMaxLength(2000)
                    .IsRequired();
            });
        });
    }
}
