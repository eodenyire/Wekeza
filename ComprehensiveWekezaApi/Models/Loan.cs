using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class Loan
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string LoanNumber { get; set; } = string.Empty;
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    public Guid AccountId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalAmount { get; set; }
    
    // Alias for Amount property used in API
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal OutstandingBalance { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal InterestRate { get; set; }
    
    public int TermInMonths { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(200)]
    public string Purpose { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Disbursed, Active, Closed
    
    [MaxLength(20)]
    public string RiskGrade { get; set; } = "Low";
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? CreditScore { get; set; }
    
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovalComments { get; set; }
    
    public DateTime? DisbursedAt { get; set; }
    public string? DisbursedBy { get; set; }
    
    public DateTime? MaturityDate { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public DateTime? ClosedAt { get; set; }
    
    public DateTime? PreferredDisbursementDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Account Account { get; set; } = null!;
    public virtual ICollection<LoanRepayment> Repayments { get; set; } = new List<LoanRepayment>();
}