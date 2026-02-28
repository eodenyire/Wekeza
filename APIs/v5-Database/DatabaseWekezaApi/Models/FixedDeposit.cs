using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class FixedDeposit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string DepositNumber { get; set; } = string.Empty;
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    public Guid SourceAccountId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalAmount { get; set; }
    
    // Alias for Amount property used in API
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal MaturityAmount { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal InterestRate { get; set; }
    
    public int TermInMonths { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, Matured, Closed
    
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public DateTime MaturityDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Account SourceAccount { get; set; } = null!;
}