using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class LetterOfCredit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string LCNumber { get; set; } = string.Empty;
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Applicant { get; set; } = string.Empty; // Customer/Importer
    
    [Required]
    [MaxLength(200)]
    public string Beneficiary { get; set; } = string.Empty; // Exporter/Supplier
    
    [MaxLength(200)]
    public string AdvisingBank { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string ConfirmingBank { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "USD";
    
    [MaxLength(50)]
    public string LCType { get; set; } = "Import"; // Import, Export, Standby
    
    [MaxLength(50)]
    public string PaymentTerms { get; set; } = "Sight"; // Sight, Usance
    
    [MaxLength(1000)]
    public string GoodsDescription { get; set; } = string.Empty;
    
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public DateTime LatestShipmentDate { get; set; }
    
    [MaxLength(200)]
    public string PortOfLoading { get; set; } = string.Empty;
    [MaxLength(200)]
    public string PortOfDischarge { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Issued"; // Issued, Advised, Confirmed, Utilized, Expired, Cancelled
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UtilizedAmount { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Commission { get; set; } = 0;
    
    [MaxLength(1000)]
    public string SpecialInstructions { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
}