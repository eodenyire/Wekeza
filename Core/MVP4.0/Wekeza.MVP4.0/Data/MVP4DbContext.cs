using Microsoft.EntityFrameworkCore;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Data;

public class MVP4DbContext : DbContext
{
    public MVP4DbContext(DbContextOptions<MVP4DbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasConversion<string>();
        });

        // Seed initial users for all roles
        SeedUsers(modelBuilder);
    }

    private void SeedUsers(ModelBuilder modelBuilder)
    {
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "System Administrator",
                Role = UserRole.Administrator,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "teller1",
                Email = "teller1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("teller123"),
                FullName = "John Teller",
                Role = UserRole.Teller,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "supervisor1",
                Email = "supervisor1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("supervisor123"),
                FullName = "Jane Supervisor",
                Role = UserRole.Supervisor,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "branchmanager1",
                Email = "branchmanager1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                FullName = "Michael Manager",
                Role = UserRole.BranchManager,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "cashofficer1",
                Email = "cashofficer1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("cash123"),
                FullName = "Sarah Cash",
                Role = UserRole.CashOfficer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "backoffice1",
                Email = "backoffice1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("backoffice123"),
                FullName = "David BackOffice",
                Role = UserRole.BackOfficeStaff,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "customercare1",
                Email = "customercare1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("customercare123"),
                FullName = "Emily Care",
                Role = UserRole.CustomerCareOfficer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "bancassurance1",
                Email = "bancassurance1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("bancassurance123"),
                FullName = "Robert Insurance",
                Role = UserRole.BancassuranceAgent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "compliance1",
                Email = "compliance1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("compliance123"),
                FullName = "Linda Compliance",
                Role = UserRole.ComplianceOfficer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "risk1",
                Email = "risk1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("risk123"),
                FullName = "James Risk",
                Role = UserRole.RiskOfficer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "loanofficer1",
                Email = "loanofficer1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("loan123"),
                FullName = "Patricia Loan",
                Role = UserRole.LoanOfficer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "auditor1",
                Email = "auditor1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("auditor123"),
                FullName = "William Auditor",
                Role = UserRole.Auditor,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "itadmin1",
                Email = "itadmin1@wekeza.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("itadmin123"),
                FullName = "Thomas IT",
                Role = UserRole.ITAdministrator,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<ApplicationUser>().HasData(users);
    }
}
