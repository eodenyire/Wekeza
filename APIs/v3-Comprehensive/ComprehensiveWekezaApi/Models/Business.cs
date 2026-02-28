using System.ComponentModel.DataAnnotations;

namespace DatabaseWekezaApi.Models;

public class Business
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;
    
    // Additional properties for comprehensive corporate banking
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string RegistrationNumber { get; set; } = string.Empty;
    
    public DateTime IncorporationDate { get; set; }
    
    [MaxLength(50)]
    public string CompanyType { get; set; } = "LLC";
    
    [MaxLength(100)]
    public string Industry { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Website { get; set; }
    
    public decimal? AnnualTurnover { get; set; }
    
    public int? NumberOfEmployees { get; set; }
    
    [MaxLength(50)]
    public string? TaxIdentificationNumber { get; set; }
    
    [MaxLength(50)]
    public string BusinessNumber { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string KraPin { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string BusinessType { get; set; } = "LLC"; // Sole Prop, Partnership, LLC
    
    [MaxLength(200)]
    public string PrimaryContactPerson { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active";
    
    [MaxLength(20)]
    public string VerificationStatus { get; set; } = "Pending";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}