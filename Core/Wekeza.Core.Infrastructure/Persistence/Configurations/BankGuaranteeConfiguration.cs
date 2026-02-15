using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class BankGuaranteeConfiguration : IEntityTypeConfiguration<BankGuarantee>
{
    public void Configure(EntityTypeBuilder<BankGuarantee> builder)
    {
        builder.ToTable("BankGuarantees");

        builder.HasKey(bg => bg.Id);

        builder.Property(bg => bg.BGNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(bg => bg.BGNumber)
            .IsUnique();

        builder.Property(bg => bg.PrincipalId)
            .IsRequired();

        builder.Property(bg => bg.BeneficiaryId)
            .IsRequired();

        builder.Property(bg => bg.IssuingBankId)
            .IsRequired();

        builder.Property(bg => bg.CounterGuaranteeId);

        // Configure Money value object
        builder.OwnsOne(bg => bg.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Configure ClaimedAmount as nullable Money
        builder.OwnsOne(bg => bg.ClaimedAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("ClaimedAmount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("ClaimedCurrency")
                .HasMaxLength(3);
        });

        builder.Property(bg => bg.IssueDate)
            .IsRequired();

        builder.Property(bg => bg.ExpiryDate)
            .IsRequired();

        builder.Property(bg => bg.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(bg => bg.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(bg => bg.Terms)
            .HasMaxLength(2000);

        builder.Property(bg => bg.Purpose)
            .HasMaxLength(1000);

        builder.Property(bg => bg.IsRevocable)
            .IsRequired();

        builder.Property(bg => bg.CreatedAt)
            .IsRequired();

        builder.Property(bg => bg.UpdatedAt)
            .IsRequired();

        // Configure owned entities - Claims
        builder.OwnsMany(bg => bg.Claims, claim =>
        {
            claim.ToTable("BGClaims");
            claim.WithOwner().HasForeignKey("BankGuaranteeId");
            claim.HasKey("Id");

            claim.OwnsOne(c => c.ClaimAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("ClaimAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("ClaimCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            claim.Property(c => c.ClaimReason)
                .IsRequired()
                .HasMaxLength(1000);

            claim.Property(c => c.ClaimDate)
                .IsRequired();

            claim.Property(c => c.ProcessedDate);

            claim.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            claim.Property(c => c.ProcessingNotes)
                .HasMaxLength(2000);

            claim.OwnsMany(c => c.SupportingDocuments, document =>
            {
                document.ToTable("BGClaimDocuments");
                document.WithOwner().HasForeignKey("BGClaimId");
                document.HasKey("Id");

                document.Property(d => d.DocumentType)
                    .IsRequired()
                    .HasMaxLength(100);

                document.Property(d => d.DocumentNumber)
                    .HasMaxLength(100);

                document.Property(d => d.TradeTransactionType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("BG_CLAIM");

                document.Property(d => d.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                document.Property(d => d.UploadedAt)
                    .IsRequired();

                document.Property(d => d.FilePath)
                    .HasMaxLength(500);

                document.Property(d => d.UploadedBy)
                    .HasMaxLength(100);

                document.Property(d => d.Comments)
                    .HasMaxLength(1000);
            });
        });

        // Configure owned entities - Amendments
        builder.OwnsMany(bg => bg.Amendments, amendment =>
        {
            amendment.ToTable("BGAmendments");
            amendment.WithOwner().HasForeignKey("BankGuaranteeId");
            amendment.HasKey("Id");

            amendment.Property(a => a.AmendmentNumber)
                .IsRequired();

            amendment.Property(a => a.AmendmentDetails)
                .HasMaxLength(2000);

            amendment.OwnsOne(a => a.PreviousAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("PreviousAmount")
                    .HasColumnType("decimal(18,2)");

                money.Property(m => m.Currency)
                    .HasColumnName("PreviousCurrency")
                    .HasMaxLength(3);
            });

            amendment.OwnsOne(a => a.NewAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("NewAmount")
                    .HasColumnType("decimal(18,2)");

                money.Property(m => m.Currency)
                    .HasColumnName("NewCurrency")
                    .HasMaxLength(3);
            });

            amendment.Property(a => a.PreviousExpiryDate);
            amendment.Property(a => a.NewExpiryDate);
            amendment.Property(a => a.AmendmentDate);

            amendment.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        // Indexes for performance
        builder.HasIndex(bg => bg.PrincipalId);
        builder.HasIndex(bg => bg.BeneficiaryId);
        builder.HasIndex(bg => bg.IssuingBankId);
        builder.HasIndex(bg => bg.Status);
        builder.HasIndex(bg => bg.Type);
        builder.HasIndex(bg => bg.ExpiryDate);
        builder.HasIndex(bg => bg.IssueDate);
    }
}