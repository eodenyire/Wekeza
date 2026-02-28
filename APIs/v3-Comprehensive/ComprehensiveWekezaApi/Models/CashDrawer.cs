using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class CashDrawer
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid BranchId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string DrawerNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string AssignedTeller { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CashLimit { get; set; } = 500000;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, Locked, Maintenance
    
    public DateTime LastReconciliation { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Branch Branch { get; set; } = null!;
}