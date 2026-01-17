using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

public class LetterOfCreditConfiguration : IEntityTypeConfiguration<LetterOfCredit>
{
    public void Configure(EntityTypeBuilder<LetterOfCredit> builder)
    {
        builder.ToTable("LetterOfCredits");

        builder.HasKey(lc => lc.Id);

        builder.Property(lc => lc.LCNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(lc => lc.LCNumber)
            .IsUnique();

        builder.Property(lc => lc.ApplicantId)
            .IsRequired();

        builder.Property(lc => lc.BeneficiaryId)
            .IsRequired();

        builder.Property(lc => lc.IssuingBankId)
            .IsRequired();

        builder.Property(lc => lc.AdvisingBankId);

        // Configure Money value object
        builder.OwnsOne(lc => lc.Amount, money =>
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

        builder.Property(lc => lc.IssueDate)
            .IsRequired();

        builder.Property(lc => lc.ExpiryDate)
            .IsRequired();

        builder.Property(lc => lc.LastShipmentDate);

        builder.Property(lc => lc.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(lc => lc.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(lc => lc.Terms)
            .HasMaxLength(2000);

        builder.Property(lc => lc.GoodsDescription)
            .HasMaxLength(1000);

        builder.Property(lc => lc.IsTransferable)
            .IsRequired();

        builder.Property(lc => lc.IsConfirmed)
            .IsRequired();

        builder.Property(lc => lc.CreatedAt)
            .IsRequired();

        builder.Property(lc => lc.UpdatedAt)
            .IsRequired();

        // Configure owned entities
        builder.OwnsMany(lc => lc.Amendments, amendment =>
        {
            amendment.ToTable("LCAmendments");
            amendment.WithOwner().HasForeignKey("LetterOfCreditId");
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

        builder.OwnsMany(lc => lc.Documents, document =>
        {
            document.ToTable("TradeDocuments");
            document.WithOwner().HasForeignKey("TradeTransactionId");
            document.HasKey("Id");

            document.Property(d => d.DocumentType)
                .IsRequired()
                .HasMaxLength(100);

            document.Property(d => d.DocumentNumber)
                .HasMaxLength(100);

            document.Property(d => d.TradeTransactionType)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("LC");

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

        // Indexes for performance
        builder.HasIndex(lc => lc.ApplicantId);
        builder.HasIndex(lc => lc.BeneficiaryId);
        builder.HasIndex(lc => lc.IssuingBankId);
        builder.HasIndex(lc => lc.Status);
        builder.HasIndex(lc => lc.ExpiryDate);
        builder.HasIndex(lc => lc.IssueDate);
    }
}