using System.ComponentModel.DataAnnotations;

namespace Wekeza.MVP4._0.Models
{
    public class Authorization
    {
        public int Id { get; set; }
        
        [Required]
        public string TransactionId { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = string.Empty; // CashWithdrawal, AccountOpening, Override, etc.
        
        [Required]
        public string TellerId { get; set; } = string.Empty;
        
        [Required]
        public string CustomerAccount { get; set; } = string.Empty;
        
        public string? CustomerName { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        public string Description { get; set; } = string.Empty;
        
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        
        public string? AuthorizedBy { get; set; }
        
        public DateTime? AuthorizedAt { get; set; }
        
        public string? Reason { get; set; }
    }
    
    public class CashPosition
    {
        public int Id { get; set; }
        
        [Required]
        public decimal VaultCash { get; set; }
        
        [Required]
        public decimal TellerCash { get; set; }
        
        [Required]
        public decimal ATMCash { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        public string UpdatedBy { get; set; } = string.Empty;
    }
    
    public class RiskAlert
    {
        public int Id { get; set; }
        
        [Required]
        public string AlertType { get; set; } = string.Empty; // AML, SuspiciousActivity, KYC, Dormant
        
        [Required]
        public string AccountNumber { get; set; } = string.Empty;
        
        public string? CustomerName { get; set; }
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string RiskLevel { get; set; } = string.Empty; // Critical, High, Medium, Low
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string Status { get; set; } = "Active"; // Active, Investigating, Escalated, Resolved
        
        public string? AssignedTo { get; set; }
        
        public string? Resolution { get; set; }
        
        public DateTime? ResolvedAt { get; set; }
    }
    
    public class BranchReport
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = string.Empty; // Daily, Weekly, Monthly, Staff
        
        public string? FilePath { get; set; }
        
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        
        public string GeneratedBy { get; set; } = string.Empty;
        
        public string Status { get; set; } = "Ready"; // Generating, Ready, Failed
        
        public long FileSize { get; set; }
    }
}