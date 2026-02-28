using System.ComponentModel.DataAnnotations;

namespace DatabaseWekezaApi.Models;

public class Address
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [MaxLength(50)]
    public string AddressType { get; set; } = "Residential";
    
    [Required]
    [MaxLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? AddressLine2 { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string State { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;
    
    public bool IsPrimary { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
}