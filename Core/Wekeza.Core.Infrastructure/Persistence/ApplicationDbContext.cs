using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using System.Reflection;
///<summary>
///📂 1. Wekeza.Core.Infrastructure/Persistence
///We will start by defining how our Domain objects "sit" in the database.
///ApplicationDbContext.cs This is where we tell EF Core how to handle our Aggregates. 
/// Note the use of Domain Event dispatching—this is where the "Statement" is made.
///</summary>

namespace Wekeza.Core.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Party> Parties => Set<Party>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<ApprovalMatrix> ApprovalMatrices => Set<ApprovalMatrix>();
    public DbSet<GLAccount> GLAccounts => Set<GLAccount>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<PaymentOrder> PaymentOrders => Set<PaymentOrder>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // This scans the Infrastructure assembly for "IEntityTypeConfiguration" classes.
        // It keeps this file clean and moves table definitions to the "Configurations" folder.
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
