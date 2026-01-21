using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string ProductCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string ProductType { get; set; } = string.Empty; // Savings, Current, Loan, FixedDeposit, Card
    
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Retail, Corporate, SME
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal InterestRate { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinimumBalance { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal MaximumBalance { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyFee { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TransactionFee { get; set; } = 0;
    
    [MaxLength(3)]
    public string Currency { get; set; } = "KES";
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Discontinued
    
    public bool IsActive { get; set; } = true;
    public bool RequiresApproval { get; set; } = false;
    
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    
    [MaxLength(1000)]
    public string EligibilityCriteria { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Features { get; set; } = string.Empty;
    
    public DateTime LaunchDate { get; set; } = DateTime.UtcNow;
    public DateTime? DiscontinuedDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}