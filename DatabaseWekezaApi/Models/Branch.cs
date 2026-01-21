using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class Branch
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string BranchCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string BranchName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Region { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Manager { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CashLimit { get; set; } = 1000000;
    
    public DateTime OpeningTime { get; set; } = DateTime.Today.AddHours(8);
    public DateTime ClosingTime { get; set; } = DateTime.Today.AddHours(17);
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<TellerSession> TellerSessions { get; set; } = new List<TellerSession>();
    public virtual ICollection<CashDrawer> CashDrawers { get; set; } = new List<CashDrawer>();
}