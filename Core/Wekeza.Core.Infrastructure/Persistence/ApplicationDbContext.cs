using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Application.Common.Interfaces;
using System.Reflection;
///<summary>
///📂 1. Wekeza.Core.Infrastructure/Persistence
///We will start by defining how our Domain objects "sit" in the database.
///ApplicationDbContext.cs This is where we tell EF Core how to handle our Aggregates. 
/// Note the use of Domain Event dispatching—this is where the "Statement" is made.
///</summary>

namespace Wekeza.Core.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
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
    public DbSet<TellerSession> TellerSessions => Set<TellerSession>();
    public DbSet<CashDrawer> CashDrawers => Set<CashDrawer>();
    public DbSet<TellerTransaction> TellerTransactions => Set<TellerTransaction>();
    
    // Week 8: Cards & Channels Management
    public DbSet<ATMTransaction> ATMTransactions => Set<ATMTransaction>();
    public DbSet<POSTransaction> POSTransactions => Set<POSTransaction>();
    public DbSet<CardApplication> CardApplications => Set<CardApplication>();

    // Week 9: Trade Finance
    public DbSet<LetterOfCredit> LetterOfCredits => Set<LetterOfCredit>();
    public DbSet<BankGuarantee> BankGuarantees => Set<BankGuarantee>();
    public DbSet<DocumentaryCollection> DocumentaryCollections => Set<DocumentaryCollection>();

    // Week 9: Treasury & Markets (Corrected)
    public DbSet<MoneyMarketDeal> MoneyMarketDeals => Set<MoneyMarketDeal>();
    public DbSet<FXDeal> FXDeals => Set<FXDeal>();
    public DbSet<SecurityDeal> SecurityDeals => Set<SecurityDeal>();

    // Week 10: Risk, Compliance & Controls
    public DbSet<AMLCase> AMLCases => Set<AMLCase>();
    public DbSet<TransactionMonitoring> TransactionMonitorings => Set<TransactionMonitoring>();
    public DbSet<SanctionsScreening> SanctionsScreenings => Set<SanctionsScreening>();

    // Week 11: Reporting & Analytics
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Dashboard> Dashboards => Set<Dashboard>();
    public DbSet<Analytics> Analytics => Set<Analytics>();

    // Week 12: Integration & Middleware
    public DbSet<IntegrationEndpoint> IntegrationEndpoints => Set<IntegrationEndpoint>();
    public DbSet<MessageQueue> MessageQueues => Set<MessageQueue>();
    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();

    // Week 12: Integration & Middleware (Legacy - to be removed)
    public DbSet<Integration> Integrations => Set<Integration>();
    public DbSet<APIGateway> APIGateways => Set<APIGateway>();

    // Week 13: Security & Administration
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemParameter> SystemParameters => Set<SystemParameter>();
    public DbSet<SystemMonitor> SystemMonitors => Set<SystemMonitor>();

    // Deposits & Investments Module
    public DbSet<FixedDeposit> FixedDeposits => Set<FixedDeposit>();
    public DbSet<RecurringDeposit> RecurringDeposits => Set<RecurringDeposit>();
    public DbSet<TermDeposit> TermDeposits => Set<TermDeposit>();
    public DbSet<CallDeposit> CallDeposits => Set<CallDeposit>();
    public DbSet<InterestAccrualEngine> InterestAccrualEngines => Set<InterestAccrualEngine>();

    // Reporting & Analytics Module
    public DbSet<RegulatoryReport> RegulatoryReports => Set<RegulatoryReport>();
    public DbSet<MISReport> MISReports => Set<MISReport>();

    // Payment Systems Module
    public DbSet<RTGSTransaction> RTGSTransactions => Set<RTGSTransaction>();
    public DbSet<SWIFTMessage> SWIFTMessages => Set<SWIFTMessage>();

    // Workflow & BPM Module
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<ApprovalWorkflow> ApprovalWorkflows => Set<ApprovalWorkflow>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();

    // Branch Operations Module
    public DbSet<Branch> Branches => Set<Branch>();

    // Digital Channels Module
    public DbSet<DigitalChannel> DigitalChannels => Set<DigitalChannel>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // This scans the Infrastructure assembly for "IEntityTypeConfiguration" classes.
        // It keeps this file clean and moves table definitions to the "Configurations" folder.
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
