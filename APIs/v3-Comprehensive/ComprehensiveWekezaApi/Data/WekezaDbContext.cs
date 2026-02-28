using Microsoft.EntityFrameworkCore;
using DatabaseWekezaApi.Models;
using ComprehensiveWekezaApi.Models;

namespace DatabaseWekezaApi.Data;

public class WekezaDbContext : DbContext
{
    public WekezaDbContext(DbContextOptions<WekezaDbContext> options) : base(options)
    {
    }

    // Core Banking Entities
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Business> Businesses { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Signatory> Signatories { get; set; }
    
    // Staff Management
    public DbSet<Staff> Staff { get; set; }
    public DbSet<StaffLogin> StaffLogins { get; set; }
    
    // Loan Management
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanRepayment> LoanRepayments { get; set; }
    
    // Deposit Products
    public DbSet<FixedDeposit> FixedDeposits { get; set; }
    
    // Teller Operations
    public DbSet<TellerSession> TellerSessions { get; set; }
    public DbSet<TellerTransaction> TellerTransactions { get; set; }
    
    // Branch Operations
    public DbSet<Branch> Branches { get; set; }
    public DbSet<CashDrawer> CashDrawers { get; set; }
    
    // Cards and Instruments
    public DbSet<Card> Cards { get; set; }
    public DbSet<ATMTransaction> ATMTransactions { get; set; }
    public DbSet<POSTransaction> POSTransactions { get; set; }
    
    // General Ledger
    public DbSet<GLAccount> GLAccounts { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    
    // Payments
    public DbSet<PaymentOrder> PaymentOrders { get; set; }
    
    // Products
    public DbSet<Product> Products { get; set; }
    
    // Trade Finance
    public DbSet<LetterOfCredit> LettersOfCredit { get; set; }
    
    // Treasury
    public DbSet<FXDeal> FXDeals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CustomerNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.IdentificationNumber).IsUnique();
        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            
            entity.HasOne(e => e.Customer)
                  .WithMany(e => e.Accounts)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Business)
                  .WithMany(e => e.Accounts)
                  .HasForeignKey(e => e.BusinessId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Reference).IsUnique();
            
            entity.HasOne(e => e.Account)
                  .WithMany(e => e.Transactions)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Business configuration
        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.RegistrationNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Address configuration
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Customer)
                  .WithMany(e => e.Addresses)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Signatory configuration
        modelBuilder.Entity<Signatory>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Account)
                  .WithMany(e => e.Signatories)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Loan configuration
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LoanNumber).IsUnique();
            
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Account)
                  .WithMany()
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // LoanRepayment configuration
        modelBuilder.Entity<LoanRepayment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Reference).IsUnique();
            
            entity.HasOne(e => e.Loan)
                  .WithMany(e => e.Repayments)
                  .HasForeignKey(e => e.LoanId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // FixedDeposit configuration
        modelBuilder.Entity<FixedDeposit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DepositNumber).IsUnique();
            
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.SourceAccount)
                  .WithMany()
                  .HasForeignKey(e => e.SourceAccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // TellerSession configuration
        modelBuilder.Entity<TellerSession>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // TellerTransaction configuration
        modelBuilder.Entity<TellerTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Reference).IsUnique();
            
            entity.HasOne(e => e.TellerSession)
                  .WithMany(e => e.Transactions)
                  .HasForeignKey(e => e.TellerSessionId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Transaction)
                  .WithMany()
                  .HasForeignKey(e => e.TransactionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Branch configuration
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BranchCode).IsUnique();
        });

        // CashDrawer configuration
        modelBuilder.Entity<CashDrawer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DrawerNumber).IsUnique();
            
            entity.HasOne(e => e.Branch)
                  .WithMany(e => e.CashDrawers)
                  .HasForeignKey(e => e.BranchId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Card configuration
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CardNumber).IsUnique();
            
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Account)
                  .WithMany()
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ATMTransaction configuration
        modelBuilder.Entity<ATMTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Reference).IsUnique();
            
            entity.HasOne(e => e.Card)
                  .WithMany(e => e.ATMTransactions)
                  .HasForeignKey(e => e.CardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // POSTransaction configuration
        modelBuilder.Entity<POSTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Reference).IsUnique();
            
            entity.HasOne(e => e.Card)
                  .WithMany(e => e.POSTransactions)
                  .HasForeignKey(e => e.CardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // GLAccount configuration
        modelBuilder.Entity<GLAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AccountCode).IsUnique();
        });

        // JournalEntry configuration
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.JournalNumber);
            
            entity.HasOne(e => e.GLAccount)
                  .WithMany(e => e.JournalEntries)
                  .HasForeignKey(e => e.GLAccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // PaymentOrder configuration
        modelBuilder.Entity<PaymentOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PaymentReference).IsUnique();
            
            entity.HasOne(e => e.FromAccount)
                  .WithMany()
                  .HasForeignKey(e => e.FromAccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductCode).IsUnique();
        });

        // LetterOfCredit configuration
        modelBuilder.Entity<LetterOfCredit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LCNumber).IsUnique();
            
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // FXDeal configuration
        modelBuilder.Entity<FXDeal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DealNumber).IsUnique();
            
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Staff configuration
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EmployeeId).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // StaffLogin configuration
        modelBuilder.Entity<StaffLogin>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Staff)
                  .WithMany(e => e.LoginHistory)
                  .HasForeignKey(e => e.StaffId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}