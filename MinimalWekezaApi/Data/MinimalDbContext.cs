using Microsoft.EntityFrameworkCore;

namespace MinimalWekezaApi.Data;

public class MinimalDbContext : DbContext
{
    public MinimalDbContext(DbContextOptions<MinimalDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.IdentificationNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.CustomerNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.IdentificationNumber).IsUnique();
        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.AvailableBalance).HasPrecision(18, 2);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.HasOne<Customer>().WithMany().HasForeignKey(e => e.CustomerId);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reference).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.BalanceAfter).HasPrecision(18, 2);
            entity.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Reference).IsUnique();
            entity.HasOne<Account>().WithMany().HasForeignKey(e => e.AccountId);
        });

        base.OnModelCreating(modelBuilder);
    }
}

// Simple entity models
public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AccountNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CurrencyCode { get; set; } = "KES";
    public decimal Balance { get; set; }
    public decimal AvailableBalance { get; set; }
    public string Status { get; set; } = "Active";
    public string AccountType { get; set; } = "Savings";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string CurrencyCode { get; set; } = "KES";
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed";
    public string ProcessedBy { get; set; } = "System";
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}