using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class TellerSession
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string TellerName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string TellerId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string BranchCode { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningCash { get; set; }
    
    // Alias properties for API compatibility
    [Column(TypeName = "decimal(18,2)")]
    public decimal StartingCash { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentCash { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ClosingCash { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? EndingCash { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, Closed
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndTime { get; set; }
    public DateTime? EndedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<TellerTransaction> Transactions { get; set; } = new List<TellerTransaction>();
}