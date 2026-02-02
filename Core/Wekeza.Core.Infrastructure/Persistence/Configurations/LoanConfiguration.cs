using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// Enhanced Loan Configuration - Complete loan entity mapping
/// Maps the comprehensive loan aggregate with all related data
/// </summary>
public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");
        builder.HasKey(l => l.Id);

        // Basic loan information
        builder.Property(l => l.LoanNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(l => l.LoanNumber)
            .IsUnique();

        builder.Property(l => l.CustomerId)
            .IsRequired();

        builder.Property(l => l.ProductId)
            .IsRequired();

        builder.Property(l => l.DisbursementAccountId);

        // Loan amounts and terms - using Money value object
        builder.OwnsOne(l => l.Principal, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PrincipalAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("PrincipalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.OwnsOne(l => l.OutstandingPrincipal, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("OutstandingPrincipalAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("OutstandingPrincipalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.Property(l => l.InterestRate)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(l => l.TermInMonths)
            .IsRequired();

        builder.Property(l => l.FirstPaymentDate);
        builder.Property(l => l.MaturityDate);

        // Loan status and lifecycle
        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(l => l.SubStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(l => l.ApplicationDate)
            .IsRequired();

        builder.Property(l => l.ApprovalDate);
        builder.Property(l => l.DisbursementDate);
        builder.Property(l => l.ClosureDate);

        // Credit assessment
        builder.Property(l => l.CreditScore)
            .HasPrecision(6, 2);

        builder.Property(l => l.RiskGrade)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(l => l.RiskPremium)
            .HasPrecision(5, 4);

        // Interest and fees - using Money value objects
        builder.OwnsOne(l => l.AccruedInterest, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("AccruedInterestAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("AccruedInterestCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.OwnsOne(l => l.TotalInterestPaid, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalInterestPaidAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("TotalInterestPaidCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.OwnsOne(l => l.TotalFeesPaid, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalFeesPaidAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("TotalFeesPaidCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.Property(l => l.LastInterestCalculationDate)
            .IsRequired();

        // Payment tracking
        builder.OwnsOne(l => l.TotalAmountPaid, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmountPaidAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("TotalAmountPaidCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.Property(l => l.LastPaymentDate);
        builder.Property(l => l.DaysPastDue);

        builder.OwnsOne(l => l.PastDueAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PastDueAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("PastDueAmountCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        // GL Integration
        builder.Property(l => l.LoanGLCode)
            .HasMaxLength(20);

        builder.Property(l => l.InterestReceivableGLCode)
            .HasMaxLength(20);

        // Provisioning
        builder.Property(l => l.ProvisionRate)
            .HasPrecision(5, 4);

        builder.OwnsOne(l => l.ProvisionAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("ProvisionAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            money.OwnsOne(m => m.Currency, currency =>
            {
                currency.Property(c => c.Code)
                    .HasColumnName("ProvisionAmountCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        builder.Property(l => l.LastProvisionDate);

        // Audit trail
        builder.Property(l => l.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.CreatedDate)
            .IsRequired();

        builder.Property(l => l.ApprovedBy)
            .HasMaxLength(100);

        builder.Property(l => l.DisbursedBy)
            .HasMaxLength(100);

        builder.Property(l => l.LastModifiedBy)
            .HasMaxLength(100);

        builder.Property(l => l.LastModifiedDate);

        // Repayment Schedule as JSON
        builder.OwnsMany(l => l.Schedule, schedule =>
        {
            schedule.ToJson("RepaymentSchedule");
            schedule.Property(s => s.ScheduleNumber).IsRequired();
            schedule.Property(s => s.DueDate).IsRequired();
            schedule.Property(s => s.IsPaid).IsRequired();
            schedule.Property(s => s.PaidDate);
        });

        // Collaterals as JSON
        builder.OwnsMany(l => l.Collaterals, collateral =>
        {
            collateral.ToJson("Collaterals");
            collateral.Property(c => c.CollateralId).IsRequired();
            collateral.Property(c => c.CollateralType).HasMaxLength(100).IsRequired();
            collateral.Property(c => c.Description).HasMaxLength(500).IsRequired();
            collateral.Property(c => c.ValuationDate).IsRequired();
            collateral.Property(c => c.ValuedBy).HasMaxLength(100);
        });

        // Guarantors as JSON
        builder.OwnsMany(l => l.Guarantors, guarantor =>
        {
            guarantor.ToJson("Guarantors");
            guarantor.Property(g => g.GuarantorId).IsRequired();
            guarantor.Property(g => g.GuarantorName).HasMaxLength(200).IsRequired();
            guarantor.Property(g => g.GuaranteeDate).IsRequired();
            guarantor.Property(g => g.GuaranteeDocument).HasMaxLength(200);
        });

        // Conditions as JSON
        builder.OwnsMany(l => l.Conditions, condition =>
        {
            condition.ToJson("Conditions");
            condition.Property(c => c.ConditionType).HasMaxLength(50).IsRequired();
            condition.Property(c => c.Description).HasMaxLength(500).IsRequired();
            condition.Property(c => c.IsMandatory).IsRequired();
            condition.Property(c => c.DueDate);
            condition.Property(c => c.IsComplied).IsRequired();
        });

        // Navigation properties
        builder.HasOne(l => l.Customer)
            .WithMany()
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.DisbursementAccount)
            .WithMany()
            .HasForeignKey(l => l.DisbursementAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(l => l.CustomerId);
        builder.HasIndex(l => l.ProductId);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => l.SubStatus);
        builder.HasIndex(l => l.RiskGrade);
        builder.HasIndex(l => l.ApplicationDate);
        builder.HasIndex(l => l.DisbursementDate);
        builder.HasIndex(l => l.MaturityDate);
        builder.HasIndex(l => l.DaysPastDue);
    }
}
