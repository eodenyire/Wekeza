using Microsoft.EntityFrameworkCore;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Infrastructure.Persistence;

public class ERMSDbContext : DbContext
{
    public ERMSDbContext(DbContextOptions<ERMSDbContext> options) : base(options)
    {
    }

    public DbSet<Risk> Risks { get; set; } = null!;
    public DbSet<RiskControl> RiskControls { get; set; } = null!;
    public DbSet<MitigationAction> MitigationActions { get; set; } = null!;
    public DbSet<KeyRiskIndicator> KeyRiskIndicators { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ERMSDbContext).Assembly);

        // Risk entity configuration
        modelBuilder.Entity<Risk>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RiskCode).IsRequired().HasMaxLength(50);
            entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
            entity.Property(r => r.Description).IsRequired().HasMaxLength(2000);
            entity.Property(r => r.Department).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(r => r.RiskCode).IsUnique();
            entity.HasIndex(r => r.Category);
            entity.HasIndex(r => r.Status);

            // Configure relationships
            entity.HasMany(r => r.Controls)
                .WithOne()
                .HasForeignKey("RiskId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.MitigationActions)
                .WithOne()
                .HasForeignKey("RiskId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.KeyRiskIndicators)
                .WithOne()
                .HasForeignKey("RiskId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RiskControl entity configuration
        modelBuilder.Entity<RiskControl>(entity =>
        {
            entity.HasKey(rc => rc.Id);
            entity.Property(rc => rc.ControlName).IsRequired().HasMaxLength(200);
            entity.Property(rc => rc.Description).HasMaxLength(2000);
        });

        // MitigationAction entity configuration
        modelBuilder.Entity<MitigationAction>(entity =>
        {
            entity.HasKey(ma => ma.Id);
            entity.Property(ma => ma.ActionTitle).IsRequired().HasMaxLength(200);
            entity.Property(ma => ma.Description).HasMaxLength(2000);
        });

        // KeyRiskIndicator entity configuration
        modelBuilder.Entity<KeyRiskIndicator>(entity =>
        {
            entity.HasKey(kri => kri.Id);
            entity.Property(kri => kri.Name).IsRequired().HasMaxLength(200);
            entity.Property(kri => kri.Description).HasMaxLength(2000);
            entity.Property(kri => kri.MeasurementUnit).HasMaxLength(50);
            entity.Property(kri => kri.DataSource).HasMaxLength(200);
        });

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(200);
            
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}
