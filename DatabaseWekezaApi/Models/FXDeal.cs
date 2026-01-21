using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseWekezaApi.Models;

public class FXDeal
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string DealNumber { get; set; } = string.Empty;
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string BaseCurrency { get; set; } = "KES";
    
    [Required]
    [MaxLength(3)]
    public string QuoteCurrency { get; set; } = "USD";
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal QuoteAmount { get; set; }
    
    [Column(TypeName = "decimal(10,6)")]
    public decimal ExchangeRate { get; set; }
    
    [MaxLength(20)]
    public string DealType { get; set; } = "Spot"; // Spot, Forward, Swap
    
    [MaxLength(20)]
    public string Side { get; set; } = "Buy"; // Buy, Sell
    
    public DateTime TradeDate { get; set; } = DateTime.UtcNow;
    public DateTime ValueDate { get; set; }
    public DateTime? MaturityDate { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Settled, Cancelled
    
    [MaxLength(100)]
    public string Trader { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Purpose { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Margin { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
}