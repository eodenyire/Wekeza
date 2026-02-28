using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class LoanRepayment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid LoanId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal InterestAmount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(50)]
    public string Reference { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Completed";
    
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Loan Loan { get; set; } = null!;
}