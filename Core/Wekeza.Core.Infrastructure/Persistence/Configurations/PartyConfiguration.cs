using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wekeza.Core.Domain.Aggregates;
using System.Text.Json;

namespace Wekeza.Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Party aggregate
/// </summary>
public class PartyConfiguration : IEntityTypeConfiguration<Party>
{
    public void Configure(EntityTypeBuilder<Party> builder)
    {
        builder.ToTable("Parties");

        builder.HasKey(p => p.Id);

        // Party Number - Unique identifier (like CIF number)
        builder.Property(p => p.PartyNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(p => p.PartyNumber)
            .IsUnique();

        // Party Type & Status
        builder.Property(p => p.PartyType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        // Individual Details
        builder.Property(p => p.FirstName)
            .HasMaxLength(100);

        builder.Property(p => p.MiddleName)
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .HasMaxLength(100);

        builder.Property(p => p.Gender)
            .HasMaxLength(20);

        builder.Property(p => p.MaritalStatus)
            .HasMaxLength(20);

        builder.Property(p => p.Nationality)
            .HasMaxLength(50);

        // Corporate Details
        builder.Property(p => p.CompanyName)
            .HasMaxLength(200);

        builder.Property(p => p.RegistrationNumber)
            .HasMaxLength(50);

        builder.Property(p => p.CompanyType)
            .HasMaxLength(50);

        builder.Property(p => p.Industry)
            .HasMaxLength(100);

        // Contact Information
        builder.Property(p => p.PrimaryEmail)
            .HasMaxLength(100);

        builder.HasIndex(p => p.PrimaryEmail);

        builder.Property(p => p.PrimaryPhone)
            .HasMaxLength(20);

        builder.HasIndex(p => p.PrimaryPhone);

        builder.Property(p => p.SecondaryPhone)
            .HasMaxLength(20);

        builder.Property(p => p.PreferredLanguage)
            .HasMaxLength(10);

        // KYC & Risk
        builder.Property(p => p.KYCStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.RiskRating)
            .IsRequired()
            .HasConversion<string>();

        builder.HasIndex(p => p.KYCStatus);
        builder.HasIndex(p => p.RiskRating);

        // Segmentation
        builder.Property(p => p.Segment)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.SubSegment)
            .HasMaxLength(50);

        builder.HasIndex(p => p.Segment);

        // Audit Fields
        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastModifiedBy)
            .HasMaxLength(100);

        // Owned Collections - Store as JSON for flexibility
        builder.OwnsMany(p => p.Addresses, a =>
        {
            a.ToJson();
            a.Property(x => x.AddressType).IsRequired();
            a.Property(x => x.AddressLine1).IsRequired();
            a.Property(x => x.City).IsRequired();
            a.Property(x => x.Country).IsRequired();
        });

        builder.OwnsMany(p => p.Identifications, i =>
        {
            i.ToJson();
            i.Property(x => x.DocumentType).IsRequired();
            i.Property(x => x.DocumentNumber).IsRequired();
            i.Property(x => x.IssuingCountry).IsRequired();
        });

        builder.OwnsMany(p => p.Relationships, r =>
        {
            r.ToJson();
            r.Property(x => x.RelatedPartyId).IsRequired();
            r.Property(x => x.RelationshipType).IsRequired();
        });

        // Indexes for performance
        builder.HasIndex(p => p.CreatedDate);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.IsPEP);
        builder.HasIndex(p => p.IsSanctioned);
    }
}
