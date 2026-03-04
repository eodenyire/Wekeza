using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Common;
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
    
    // Customer Service Entities
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<CommunicationRecord> CommunicationRecords => Set<CommunicationRecord>();
    public DbSet<ApprovalMatrix> ApprovalMatrices => Set<ApprovalMatrix>();
    public DbSet<GLAccount> GLAccounts => Set<GLAccount>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<Reconciliation> Reconciliations => Set<Reconciliation>();
    public DbSet<InterestAccrual> InterestAccruals => Set<InterestAccrual>();
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

    // Admin Portal Module
    public DbSet<AdminSession> AdminSessions => Set<AdminSession>();
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public DbSet<BatchJob> BatchJobs => Set<BatchJob>();
    public DbSet<ExceptionCase> ExceptionCases => Set<ExceptionCase>();

    // Admin Portal Phase 3 - Enterprise Services (Product, Risk, Analytics)
    public DbSet<ProductTemplate> ProductTemplates => Set<ProductTemplate>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<InterestRateTable> InterestRateTables => Set<InterestRateTable>();
    public DbSet<PostingRule> PostingRules => Set<PostingRule>();
    public DbSet<LimitDefinition> LimitDefinitions => Set<LimitDefinition>();
    // public DbSet<ThresholdConfig> ThresholdConfigs => Set<ThresholdConfig>(); // TODO: Create ThresholdConfig aggregate
    public DbSet<Anomaly> Anomalies => Set<Anomaly>();
    public DbSet<AnomalyRule> AnomalyRules => Set<AnomalyRule>();
    public DbSet<CustomDashboard> CustomDashboards => Set<CustomDashboard>();
    public DbSet<KPIDefinition> KPIDefinitions => Set<KPIDefinition>();
    public DbSet<Wekeza.Core.Infrastructure.Persistence.Configurations.SavedAnalysis> SavedAnalyses => Set<Wekeza.Core.Infrastructure.Persistence.Configurations.SavedAnalysis>();
    public DbSet<KYCVerification> KYCVerifications => Set<KYCVerification>();

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
        try
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            // Log but don't fail during configuration scanning
            System.Console.WriteLine($"Warning during configuration scanning: {ex.Message}");
        }

        // Temporary: Ignore all dictionary properties and complex value objects in legacy aggregates
        // until proper configurations are created
        builder.Entity<APIGateway>(entity =>
        {
            entity.Ignore(g => g.RateLimits);
            entity.Ignore(g => g.AuthConfigs);
            entity.Ignore(g => g.CacheConfigs);
            entity.Ignore(g => g.SecurityHeaders);
            entity.Ignore(g => g.CircuitBreakerStates);
            entity.Ignore(g => g.Metadata);
        });

        builder.Entity<ATMTransaction>(entity =>
        {
            entity.Ignore(t => t.Amount);
            entity.Ignore(t => t.AccountBalanceBefore);
            entity.Ignore(t => t.AccountBalanceAfter);
            entity.Ignore(t => t.ATMFee);
            entity.Ignore(t => t.InterchangeFee);
        });

        builder.Entity<POSTransaction>(entity =>
        {
            entity.Ignore(t => t.TotalAmount);
            entity.Ignore(t => t.AccountBalanceBefore);
            entity.Ignore(t => t.AccountBalanceAfter);
            entity.Ignore(t => t.InterchangeFee);
            entity.Ignore(t => t.MerchantFee);
            entity.Ignore(t => t.NetworkFee);
        });

        builder.Entity<InterestAccrualEngine>(entity =>
        {
            entity.Ignore(e => e.AccrualEntries);
        });

        builder.Entity<SystemMonitor>(entity =>
        {
            entity.Ignore(m => m.MonitoringRules);
            entity.Ignore(m => m.Thresholds);
            entity.Ignore(m => m.AlertRules);
            entity.Ignore(m => m.NotificationChannels);
            entity.Ignore(m => m.LastCheckResult);
            entity.Ignore(m => m.Metadata);
        });

        // Ignore shared-type owned navigation issues  
        var types = typeof(ApplicationDbContext).Assembly.GetTypes();
        foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract && typeof(Entity).IsAssignableFrom(t)))
        {
            try
            {
                var et = builder.Model.FindEntityType(type);
                if (et != null)
                {
                    foreach (var nav in et.GetNavigations())
                    {
                        if (nav.ForeignKey?.IsRequired == false && !nav.ForeignKey.IsUnique)
                        {
                            // Likely a misconfigured navigation
                        }
                    }
                }
            }
            catch
            {
                // Silently skip entities that can't be analyzed
            }
        }

        base.OnModelCreating(builder);
    }
}
