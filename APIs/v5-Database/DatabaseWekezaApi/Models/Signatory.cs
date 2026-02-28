using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class Signatory
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid AccountId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string SignatoryName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string IdNumber { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Role { get; set; } = "Approver"; // Initiator, Approver, Viewer
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal SignatureLimit { get; set; } // Max amount this person can authorize alone
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active";
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Account Account { get; set; } = null!;
}