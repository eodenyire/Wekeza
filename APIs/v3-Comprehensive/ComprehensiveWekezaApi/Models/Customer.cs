using System.ComponentModel.DataAnnotations;

namespace DatabaseWekezaApi.Models;

public class Customer
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? MiddleName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string IdentificationNumber { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string CustomerNumber { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; set; } = "Active";
    
    // Enhanced CIF fields
    public DateTime? DateOfBirth { get; set; }
    
    [MaxLength(20)]
    public string? Gender { get; set; }
    
    [MaxLength(50)]
    public string? MaritalStatus { get; set; }
    
    [MaxLength(100)]
    public string Nationality { get; set; } = "Kenyan";
    
    [MaxLength(20)]
    public string PrimaryPhone { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? SecondaryPhone { get; set; }
    
    [MaxLength(50)]
    public string? PreferredLanguage { get; set; } = "English";
    
    // KYC and AML fields
    [MaxLength(20)]
    public string KYCStatus { get; set; } = "Pending";
    
    [MaxLength(20)]
    public string AMLRiskRating { get; set; } = "Low";
    
    public DateTime? KYCCompletedAt { get; set; }
    public DateTime? LastAMLScreening { get; set; }
    
    // Preferences
    public bool OptInMarketing { get; set; } = false;
    public bool OptInSMS { get; set; } = true;
    public bool OptInEmail { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
}