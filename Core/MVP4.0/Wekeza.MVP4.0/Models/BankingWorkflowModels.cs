using System.ComponentModel.DataAnnotations;

namespace Wekeza.MVP4._0.Models
{
    // Core RBAC Models
    public class Role
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal ApprovalLimit { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public List<UserRoleAssignment> UserRoles { get; set; } = new();
        public List<RolePermission> RolePermissions { get; set; } = new();
    }

    public class UserRoleAssignment
    {
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public Guid AssignedBy { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Role Role { get; set; } = null!;
        public ApplicationUser AssignedByUser { get; set; } = null!;
    }

    public class Permission
    {
        public Guid PermissionId { get; set; }
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Conditions { get; set; } // JSON conditions
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public List<RolePermission> RolePermissions { get; set; } = new();
    }

    public class RolePermission
    {
        public Guid RolePermissionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }

    // Workflow Models
    public class WorkflowInstance
    {
        public Guid WorkflowId { get; set; }
        public string WorkflowType { get; set; } = string.Empty;
        public Guid ResourceId { get; set; }
        public string ResourceType { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public Guid InitiatedBy { get; set; }
        public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string? Data { get; set; } // JSON data
        public string? BusinessJustification { get; set; }
        public string? Priority { get; set; } = "Normal";
        public decimal? Amount { get; set; }
        public DateTime? ApprovalDeadline { get; set; }
        public string? EscalationReason { get; set; }
        public DateTime? EscalatedAt { get; set; }
        public string? RejectionReason { get; set; }
        public string? CancellationReason { get; set; }
        public Guid? CancelledBy { get; set; }
        
        // Navigation properties
        public ApplicationUser InitiatedByUser { get; set; } = null!;
        public ApplicationUser? CancelledByUser { get; set; }
        public List<ApprovalStep> ApprovalSteps { get; set; } = new();
    }

    public class ApprovalStep
    {
        public Guid StepId { get; set; }
        public Guid WorkflowId { get; set; }
        public int StepOrder { get; set; }
        public string ApproverRole { get; set; } = string.Empty;
        public Guid? AssignedTo { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? ApprovedAt { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsEscalated { get; set; } = false;
        
        // Navigation properties
        public WorkflowInstance Workflow { get; set; } = null!;
        public ApplicationUser? AssignedToUser { get; set; }
        public ApplicationUser? ApprovedByUser { get; set; }
    }

    // Extended Account Models
    public class SignatoryRule
    {
        public Guid RuleId { get; set; }
        public Guid AccountId { get; set; }
        public string SignatoryType { get; set; } = string.Empty; // Single, Joint, Either
        public int MinimumSignatures { get; set; } = 1;
        public decimal? MaximumAmount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        
        // Navigation properties
        public Account Account { get; set; } = null!;
        public ApplicationUser CreatedByUser { get; set; } = null!;
        public List<AccountSignatory> AccountSignatories { get; set; } = new();
    }

    public class AccountSignatory
    {
        public Guid SignatoryId { get; set; }
        public Guid AccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string SignatoryRole { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public Guid AddedBy { get; set; }
        
        // Navigation properties
        public Account Account { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public ApplicationUser AddedByUser { get; set; } = null!;
    }

    // Loan Models
    public class Loan
    {
        public Guid LoanId { get; set; }
        public string LoanNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string LoanType { get; set; } = string.Empty;
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int Tenure { get; set; } // in months
        public string Status { get; set; } = "Applied";
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovalDate { get; set; }
        public DateTime? DisbursementDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public Guid LoanOfficerId { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public decimal OutstandingBalance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public ApplicationUser LoanOfficer { get; set; } = null!;
        public ApplicationUser? ApprovedByUser { get; set; }
        public List<CreditAssessment> CreditAssessments { get; set; } = new();
        public List<LoanRepayment> LoanRepayments { get; set; } = new();
        public List<Collateral> Collaterals { get; set; } = new();
        public List<LoanDocument> LoanDocuments { get; set; } = new();
    }

    public class CreditAssessment
    {
        public Guid AssessmentId { get; set; }
        public Guid LoanId { get; set; }
        public int CreditScore { get; set; }
        public string RiskGrade { get; set; } = string.Empty;
        public decimal? DebtToIncomeRatio { get; set; }
        public decimal? CollateralValue { get; set; }
        public string? AssessmentNotes { get; set; }
        public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
        public Guid AssessedBy { get; set; }
        
        // Navigation properties
        public Loan Loan { get; set; } = null!;
        public ApplicationUser AssessedByUser { get; set; } = null!;
    }

    public class LoanRepayment
    {
        public Guid RepaymentId { get; set; }
        public Guid LoanId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public decimal ScheduledAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? PaidAmount { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string? PaymentReference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Loan Loan { get; set; } = null!;
    }

    public class Collateral
    {
        public Guid CollateralId { get; set; }
        public Guid LoanId { get; set; }
        public string CollateralType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedValue { get; set; }
        public DateTime ValuationDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ValuationReference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Loan Loan { get; set; } = null!;
    }

    public class LoanDocument
    {
        public Guid DocumentId { get; set; }
        public Guid LoanId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = "Pending";
        public Guid UploadedBy { get; set; }
        
        // Navigation properties
        public Loan Loan { get; set; } = null!;
        public ApplicationUser UploadedByUser { get; set; } = null!;
    }

    // Insurance/Bancassurance Models
    public class InsurancePolicy
    {
        public Guid PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string PolicyType { get; set; } = string.Empty;
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public string PremiumFrequency { get; set; } = string.Empty; // Monthly, Quarterly, Annual
        public Guid? LinkedAccountId { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid BancassuranceOfficerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public Account? LinkedAccount { get; set; }
        public ApplicationUser BancassuranceOfficer { get; set; } = null!;
        public List<PolicyBeneficiary> PolicyBeneficiaries { get; set; } = new();
        public List<PremiumPayment> PremiumPayments { get; set; } = new();
        public List<InsuranceClaim> InsuranceClaims { get; set; } = new();
    }

    public class PolicyBeneficiary
    {
        public Guid BeneficiaryId { get; set; }
        public Guid PolicyId { get; set; }
        public string BeneficiaryName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public decimal BenefitPercentage { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public InsurancePolicy Policy { get; set; } = null!;
    }

    public class PremiumPayment
    {
        public Guid PaymentId { get; set; }
        public Guid PolicyId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public decimal ScheduledAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public decimal? PaidAmount { get; set; }
        public string Status { get; set; } = "Scheduled";
        public Guid? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public InsurancePolicy Policy { get; set; } = null!;
        public Transaction? Transaction { get; set; }
    }

    public class InsuranceClaim
    {
        public Guid ClaimId { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public Guid PolicyId { get; set; }
        public string ClaimType { get; set; } = string.Empty;
        public decimal ClaimAmount { get; set; }
        public DateTime IncidentDate { get; set; }
        public DateTime ClaimDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Submitted";
        public Guid? ProcessedBy { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ProcessingNotes { get; set; }
        
        // Navigation properties
        public InsurancePolicy Policy { get; set; } = null!;
        public ApplicationUser? ProcessedByUser { get; set; }
        public List<ClaimDocument> ClaimDocuments { get; set; } = new();
    }

    public class ClaimDocument
    {
        public Guid DocumentId { get; set; }
        public Guid ClaimId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public Guid UploadedBy { get; set; }
        
        // Navigation properties
        public InsuranceClaim Claim { get; set; } = null!;
        public ApplicationUser UploadedByUser { get; set; } = null!;
    }

    // Audit Models
    public class BankingAuditLog
    {
        public Guid AuditId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public Guid ResourceId { get; set; }
        public string? OldValues { get; set; } // JSON
        public string? NewValues { get; set; } // JSON
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
    }

    // KYC and Risk Models
    public class KYCData
    {
        public Guid KYCId { get; set; }
        public Guid CustomerId { get; set; }
        public string RiskRating { get; set; } = string.Empty;
        public DateTime LastReviewDate { get; set; }
        public DateTime NextReviewDate { get; set; }
        public string ReviewedBy { get; set; } = string.Empty;
        public string ComplianceNotes { get; set; } = string.Empty;
        public bool AMLFlagged { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public List<RiskIndicator> RiskIndicators { get; set; } = new();
    }

    public class RiskIndicator
    {
        public Guid IndicatorId { get; set; }
        public Guid KYCId { get; set; }
        public string IndicatorType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime IdentifiedAt { get; set; } = DateTime.UtcNow;
        public Guid IdentifiedBy { get; set; }
        public bool IsResolved { get; set; } = false;
        
        // Navigation properties
        public KYCData KYCData { get; set; } = null!;
        public ApplicationUser IdentifiedByUser { get; set; } = null!;
    }
}
// Notification entities
public class Notification
{
    public Guid NotificationId { get; set; }
    public Guid? UserId { get; set; }
    public string? RoleName { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? RelatedWorkflowId { get; set; }
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ActionUrl { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class NotificationSettings
{
    public Guid UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool InAppNotifications { get; set; } = true;
    public bool ApprovalReminders { get; set; } = true;
    public bool EscalationAlerts { get; set; } = true;
    public bool DeadlineWarnings { get; set; } = true;
    public int ReminderFrequencyHours { get; set; } = 24;
    public Dictionary<string, bool> NotificationTypeSettings { get; set; } = new();
}