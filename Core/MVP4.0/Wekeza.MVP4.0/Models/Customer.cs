using System.ComponentModel.DataAnnotations;

namespace Wekeza.MVP4._0.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string IdType { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string EmployerName { get; set; } = string.Empty;
        public string EmployerAddress { get; set; } = string.Empty;
        public string KycStatus { get; set; } = "Pending";
        public DateTime KycExpiryDate { get; set; }
        public string CustomerStatus { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        
        // Navigation properties
        public List<Account> Accounts { get; set; } = new();
        public List<CustomerComplaint> Complaints { get; set; } = new();
        public List<CustomerDocument> Documents { get; set; } = new();
    }

    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = "Active";
        public DateTime OpenedDate { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public bool IsDormant { get; set; }
        public bool IsFrozen { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        
        // Navigation properties
        public List<Transaction> Transactions { get; set; } = new();
        public List<StandingInstruction> StandingInstructions { get; set; } = new();
    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Description { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public DateTime ValueDate { get; set; }
        public string Status { get; set; } = "Completed";
        public decimal RunningBalance { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string InitiatedBy { get; set; } = string.Empty;
        public Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;
    }

    public class StandingInstruction
    {
        public Guid Id { get; set; }
        public string InstructionId { get; set; } = string.Empty;
        public string FromAccount { get; set; } = string.Empty;
        public string ToAccount { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime NextExecutionDate { get; set; }
        public string Status { get; set; } = "Active";
        public string Description { get; set; } = string.Empty;
        public Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;
    }

    public class CustomerComplaint
    {
        public Guid Id { get; set; }
        public string ComplaintNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Open";
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        
        // Navigation properties
        public List<ComplaintUpdate> Updates { get; set; } = new();
        public List<ComplaintDocument> Documents { get; set; } = new();
    }

    public class ComplaintUpdate
    {
        public Guid Id { get; set; }
        public string UpdateText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public Guid ComplaintId { get; set; }
        public CustomerComplaint Complaint { get; set; } = null!;
    }

    public class ComplaintDocument
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public Guid ComplaintId { get; set; }
        public CustomerComplaint Complaint { get; set; } = null!;
    }

    public class CustomerDocument
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string UploadedBy { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }

    public class AccountStatusRequest
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty; // Freeze, Unfreeze, Activate, Close
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string ProcessedBy { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }

    public class CardRequest
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty; // Block, Unblock, Replace, PIN Reset
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string ProcessedBy { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }
}