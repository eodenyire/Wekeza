using Microsoft.EntityFrameworkCore;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Data;

public class MVP4DbContext : DbContext
{
    public MVP4DbContext(DbContextOptions<MVP4DbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Authorization> Authorizations { get; set; }
    public DbSet<CashPosition> CashPositions { get; set; }
    public DbSet<RiskAlert> RiskAlerts { get; set; }
    public DbSet<BranchReport> BranchReports { get; set; }
    
    // Customer Care entities
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<StandingInstruction> StandingInstructions { get; set; }
    public DbSet<CustomerComplaint> CustomerComplaints { get; set; }
    public DbSet<ComplaintUpdate> ComplaintUpdates { get; set; }
    public DbSet<ComplaintDocument> ComplaintDocuments { get; set; }
    public DbSet<CustomerDocument> CustomerDocuments { get; set; }
    public DbSet<AccountStatusRequest> AccountStatusRequests { get; set; }
    public DbSet<CardRequest> CardRequests { get; set; }
    
    // Banking Workflow entities
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<ApprovalStep> ApprovalSteps { get; set; }
    public DbSet<SignatoryRule> SignatoryRules { get; set; }
    public DbSet<AccountSignatory> AccountSignatories { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<CreditAssessment> CreditAssessments { get; set; }
    public DbSet<LoanRepayment> LoanRepayments { get; set; }
    public DbSet<Collateral> Collaterals { get; set; }
    public DbSet<LoanDocument> LoanDocuments { get; set; }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
    public DbSet<PolicyBeneficiary> PolicyBeneficiaries { get; set; }
    public DbSet<PremiumPayment> PremiumPayments { get; set; }
    public DbSet<InsuranceClaim> InsuranceClaims { get; set; }
    public DbSet<ClaimDocument> ClaimDocuments { get; set; }
    public DbSet<BankingAuditLog> BankingAuditLogs { get; set; }
    public DbSet<KYCData> KYCData { get; set; }
    public DbSet<RiskIndicator> RiskIndicators { get; set; }

    // Notification entities
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationSettings> NotificationSettings { get; set; }

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

        // Configure Authorization
        modelBuilder.Entity<Authorization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TellerId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerAccount).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.AuthorizedBy).HasMaxLength(100);
        });

        // Configure CashPosition
        modelBuilder.Entity<CashPosition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VaultCash).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TellerCash).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ATMCash).HasColumnType("decimal(18,2)");
            entity.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(100);
        });

        // Configure RiskAlert
        modelBuilder.Entity<RiskAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AlertType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.RiskLevel).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
        });

        // Configure BranchReport
        modelBuilder.Entity<BranchReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.GeneratedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        // Configure Customer Care entities
        ConfigureCustomerCareEntities(modelBuilder);
        
        // Configure Banking Workflow entities
        ConfigureBankingWorkflowEntities(modelBuilder);

        // Configure Notification entities
        ConfigureNotificationEntities(modelBuilder);

        // Seed initial users for all roles
        SeedUsers(modelBuilder);
        
        // Seed banking workflow data
        SeedBankingWorkflowData(modelBuilder);
    }

    private void ConfigureCustomerCareEntities(ModelBuilder modelBuilder)
    {
        // Configure Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.IdNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IdType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.KycStatus).HasMaxLength(20);
            entity.Property(e => e.CustomerStatus).HasMaxLength(20);
            entity.HasIndex(e => e.CustomerNumber).IsUnique();
            entity.HasIndex(e => e.IdNumber).IsUnique();
        });

        // Configure Account
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.AccountName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AccountType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AvailableBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.BranchCode).HasMaxLength(10);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.HasOne(e => e.Customer).WithMany(e => e.Accounts).HasForeignKey(e => e.CustomerId);
        });

        // Configure Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.RunningBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Channel).HasMaxLength(20);
            entity.HasOne(e => e.Account).WithMany(e => e.Transactions).HasForeignKey(e => e.AccountId);
        });

        // Configure StandingInstruction
        modelBuilder.Entity<StandingInstruction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InstructionId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FromAccount).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ToAccount).IsRequired().HasMaxLength(20);
            entity.Property(e => e.BeneficiaryName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Frequency).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.HasOne(e => e.Account).WithMany(e => e.StandingInstructions).HasForeignKey(e => e.AccountId);
        });

        // Configure CustomerComplaint
        modelBuilder.Entity<CustomerComplaint>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ComplaintNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Priority).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.HasOne(e => e.Customer).WithMany(e => e.Complaints).HasForeignKey(e => e.CustomerId);
        });

        // Configure ComplaintUpdate
        modelBuilder.Entity<ComplaintUpdate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Complaint).WithMany(e => e.Updates).HasForeignKey(e => e.ComplaintId);
        });

        // Configure ComplaintDocument
        modelBuilder.Entity<ComplaintDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.UploadedBy).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Complaint).WithMany(e => e.Documents).HasForeignKey(e => e.ComplaintId);
        });

        // Configure CustomerDocument
        modelBuilder.Entity<CustomerDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.UploadedBy).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Customer).WithMany(e => e.Documents).HasForeignKey(e => e.CustomerId);
        });

        // Configure AccountStatusRequest
        modelBuilder.Entity<AccountStatusRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.RequestType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.RequestedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProcessedBy).HasMaxLength(100);
        });

        // Configure CardRequest
        modelBuilder.Entity<CardRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CardNumber).HasMaxLength(20);
            entity.Property(e => e.RequestType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.RequestedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProcessedBy).HasMaxLength(100);
        });
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

    private void ConfigureBankingWorkflowEntities(ModelBuilder modelBuilder)
    {
        // Configure Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ApprovalLimit).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.RoleName).IsUnique();
        });

        // Configure UserRoleAssignment
        modelBuilder.Entity<UserRoleAssignment>(entity =>
        {
            entity.HasKey(e => e.UserRoleId);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Role).WithMany(e => e.UserRoles).HasForeignKey(e => e.RoleId);
            entity.HasOne(e => e.AssignedByUser).WithMany().HasForeignKey(e => e.AssignedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Permission
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId);
            entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique();
        });

        // Configure RolePermission
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId);
            entity.HasOne(e => e.Role).WithMany(e => e.RolePermissions).HasForeignKey(e => e.RoleId);
            entity.HasOne(e => e.Permission).WithMany(e => e.RolePermissions).HasForeignKey(e => e.PermissionId);
        });

        // Configure WorkflowInstance
        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.HasKey(e => e.WorkflowId);
            entity.Property(e => e.WorkflowType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ResourceType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.HasOne(e => e.InitiatedByUser).WithMany().HasForeignKey(e => e.InitiatedBy);
        });

        // Configure ApprovalStep
        modelBuilder.Entity<ApprovalStep>(entity =>
        {
            entity.HasKey(e => e.StepId);
            entity.Property(e => e.ApproverRole).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Comments).HasMaxLength(500);
            entity.HasOne(e => e.Workflow).WithMany(e => e.ApprovalSteps).HasForeignKey(e => e.WorkflowId);
            entity.HasOne(e => e.AssignedToUser).WithMany().HasForeignKey(e => e.AssignedTo).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ApprovedByUser).WithMany().HasForeignKey(e => e.ApprovedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // Configure SignatoryRule
        modelBuilder.Entity<SignatoryRule>(entity =>
        {
            entity.HasKey(e => e.RuleId);
            entity.Property(e => e.SignatoryType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MaximumAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Account).WithMany().HasForeignKey(e => e.AccountId);
            entity.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy);
        });

        // Configure AccountSignatory
        modelBuilder.Entity<AccountSignatory>(entity =>
        {
            entity.HasKey(e => e.SignatoryId);
            entity.Property(e => e.SignatoryRole).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Account).WithMany().HasForeignKey(e => e.AccountId);
            entity.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerId);
            entity.HasOne(e => e.AddedByUser).WithMany().HasForeignKey(e => e.AddedBy);
        });

        // Configure Loan
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId);
            entity.Property(e => e.LoanNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.LoanType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PrincipalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.InterestRate).HasColumnType("decimal(5,4)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Purpose).HasMaxLength(255);
            entity.Property(e => e.OutstandingBalance).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.LoanNumber).IsUnique();
            entity.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerId);
            entity.HasOne(e => e.LoanOfficer).WithMany().HasForeignKey(e => e.LoanOfficerId);
            entity.HasOne(e => e.ApprovedByUser).WithMany().HasForeignKey(e => e.ApprovedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // Configure CreditAssessment
        modelBuilder.Entity<CreditAssessment>(entity =>
        {
            entity.HasKey(e => e.AssessmentId);
            entity.Property(e => e.RiskGrade).IsRequired().HasMaxLength(10);
            entity.Property(e => e.DebtToIncomeRatio).HasColumnType("decimal(5,4)");
            entity.Property(e => e.CollateralValue).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Loan).WithMany(e => e.CreditAssessments).HasForeignKey(e => e.LoanId);
            entity.HasOne(e => e.AssessedByUser).WithMany().HasForeignKey(e => e.AssessedBy);
        });

        // Configure LoanRepayment
        modelBuilder.Entity<LoanRepayment>(entity =>
        {
            entity.HasKey(e => e.RepaymentId);
            entity.Property(e => e.ScheduledAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.PaymentReference).HasMaxLength(50);
            entity.HasOne(e => e.Loan).WithMany(e => e.LoanRepayments).HasForeignKey(e => e.LoanId);
        });

        // Configure Collateral
        modelBuilder.Entity<Collateral>(entity =>
        {
            entity.HasKey(e => e.CollateralId);
            entity.Property(e => e.CollateralType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EstimatedValue).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ValuationReference).HasMaxLength(50);
            entity.HasOne(e => e.Loan).WithMany(e => e.Collaterals).HasForeignKey(e => e.LoanId);
        });

        // Configure LoanDocument
        modelBuilder.Entity<LoanDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId);
            entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.HasOne(e => e.Loan).WithMany(e => e.LoanDocuments).HasForeignKey(e => e.LoanId);
            entity.HasOne(e => e.UploadedByUser).WithMany().HasForeignKey(e => e.UploadedBy);
        });

        // Configure InsurancePolicy
        modelBuilder.Entity<InsurancePolicy>(entity =>
        {
            entity.HasKey(e => e.PolicyId);
            entity.Property(e => e.PolicyNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PolicyType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CoverageAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PremiumAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PremiumFrequency).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.HasIndex(e => e.PolicyNumber).IsUnique();
            entity.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerId);
            entity.HasOne(e => e.LinkedAccount).WithMany().HasForeignKey(e => e.LinkedAccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.BancassuranceOfficer).WithMany().HasForeignKey(e => e.BancassuranceOfficerId);
        });

        // Configure PolicyBeneficiary
        modelBuilder.Entity<PolicyBeneficiary>(entity =>
        {
            entity.HasKey(e => e.BeneficiaryId);
            entity.Property(e => e.BeneficiaryName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Relationship).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BenefitPercentage).HasColumnType("decimal(5,2)");
            entity.HasOne(e => e.Policy).WithMany(e => e.PolicyBeneficiaries).HasForeignKey(e => e.PolicyId);
        });

        // Configure PremiumPayment
        modelBuilder.Entity<PremiumPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.ScheduledAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.HasOne(e => e.Policy).WithMany(e => e.PremiumPayments).HasForeignKey(e => e.PolicyId);
            entity.HasOne(e => e.Transaction).WithMany().HasForeignKey(e => e.TransactionId).OnDelete(DeleteBehavior.Restrict);
        });

        // Configure InsuranceClaim
        modelBuilder.Entity<InsuranceClaim>(entity =>
        {
            entity.HasKey(e => e.ClaimId);
            entity.Property(e => e.ClaimNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ClaimType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ClaimAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.HasIndex(e => e.ClaimNumber).IsUnique();
            entity.HasOne(e => e.Policy).WithMany(e => e.InsuranceClaims).HasForeignKey(e => e.PolicyId);
            entity.HasOne(e => e.ProcessedByUser).WithMany().HasForeignKey(e => e.ProcessedBy).OnDelete(DeleteBehavior.Restrict);
        });

        // Configure ClaimDocument
        modelBuilder.Entity<ClaimDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId);
            entity.Property(e => e.DocumentType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.HasOne(e => e.Claim).WithMany(e => e.ClaimDocuments).HasForeignKey(e => e.ClaimId);
            entity.HasOne(e => e.UploadedByUser).WithMany().HasForeignKey(e => e.UploadedBy);
        });

        // Configure BankingAuditLog
        modelBuilder.Entity<BankingAuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ResourceType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IPAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(255);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });

        // Configure KYCData
        modelBuilder.Entity<KYCData>(entity =>
        {
            entity.HasKey(e => e.KYCId);
            entity.Property(e => e.RiskRating).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ReviewedBy).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerId);
        });

        // Configure RiskIndicator
        modelBuilder.Entity<RiskIndicator>(entity =>
        {
            entity.HasKey(e => e.IndicatorId);
            entity.Property(e => e.IndicatorType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Severity).IsRequired().HasMaxLength(20);
            entity.HasOne(e => e.KYCData).WithMany(e => e.RiskIndicators).HasForeignKey(e => e.KYCId);
            entity.HasOne(e => e.IdentifiedByUser).WithMany().HasForeignKey(e => e.IdentifiedBy);
        });
    }

    private void SeedBankingWorkflowData(ModelBuilder modelBuilder)
    {
        // Seed Roles
        var roles = new List<Role>
        {
            new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = "BackOfficeOfficer",
                Description = "Handles account operations, KYC processing, and non-cash transactions",
                ApprovalLimit = 10000,
                IsActive = true
            },
            new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = "LoanOfficer",
                Description = "Manages complete credit lifecycle within approval limits",
                ApprovalLimit = 50000,
                IsActive = true
            },
            new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = "BancassuranceOfficer",
                Description = "Manages insurance products within banking system",
                ApprovalLimit = 25000,
                IsActive = true
            },
            new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = "BranchManager",
                Description = "Approval authority for high-risk actions and policy overrides",
                ApprovalLimit = 500000,
                IsActive = true
            }
        };

        modelBuilder.Entity<Role>().HasData(roles);

        // Seed Permissions
        var permissions = new List<Permission>
        {
            // Back Office permissions
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Account", Action = "Create" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Account", Action = "Update" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Account", Action = "Close" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "KYC", Action = "Process" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Transaction", Action = "Process" },
            
            // Loan Officer permissions
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Loan", Action = "Create" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Loan", Action = "Assess" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Loan", Action = "Disburse" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Loan", Action = "Service" },
            
            // Bancassurance permissions
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Policy", Action = "Create" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Policy", Action = "Service" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Claim", Action = "Process" },
            
            // Branch Manager permissions
            new Permission { PermissionId = Guid.NewGuid(), Resource = "All", Action = "Approve" },
            new Permission { PermissionId = Guid.NewGuid(), Resource = "Policy", Action = "Override" }
        };

        modelBuilder.Entity<Permission>().HasData(permissions);
    }

    private void ConfigureNotificationEntities(ModelBuilder modelBuilder)
    {
        // Configure Notification entity
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.NotificationType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Priority).HasMaxLength(20);
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.ActionUrl).HasMaxLength(500);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RoleName);
            entity.HasIndex(e => e.RelatedWorkflowId);
            entity.HasIndex(e => e.CreatedAt);
            
            // Ignore the Metadata property for now (can be added later with proper JSON serialization)
            entity.Ignore(e => e.Metadata);
        });

        // Configure NotificationSettings entity
        modelBuilder.Entity<NotificationSettings>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.ReminderFrequencyHours).HasDefaultValue(24);
            
            // Ignore the NotificationTypeSettings property for now
            entity.Ignore(e => e.NotificationTypeSettings);
        });
    }
}