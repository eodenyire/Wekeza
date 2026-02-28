using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Loan> Loans { get; }
    DbSet<Product> Products { get; }
    DbSet<Branch> Branches { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<GLAccount> GLAccounts { get; }
    DbSet<JournalEntry> JournalEntries { get; }
    DbSet<PaymentOrder> PaymentOrders { get; }
    DbSet<Card> Cards { get; }
    DbSet<FixedDeposit> FixedDeposits { get; }
    DbSet<TermDeposit> TermDeposits { get; }
    DbSet<CallDeposit> CallDeposits { get; }
    DbSet<RecurringDeposit> RecurringDeposits { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}