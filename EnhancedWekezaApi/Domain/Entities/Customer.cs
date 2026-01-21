using System.ComponentModel.DataAnnotations;

namespace EnhancedWekezaApi.Domain.Entities;

public class Customer
{
    [Key]
    public Guid Id { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; private set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; private set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; private set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string IdentificationNumber { get; private set; } = string.Empty;
    
    [MaxLength(20)]
    public string CustomerNumber { get; private set; } = string.Empty;
    
    [MaxLength(20)]
    public string Status { get; private set; } = "Active";
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Account> Accounts { get; private set; } = new List<Account>();

    // Private constructor for EF
    private Customer() { }

    public Customer(Guid id, string firstName, string lastName, string email, string identificationNumber)
    {
        Id = id;
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        IdentificationNumber = identificationNumber ?? throw new ArgumentNullException(nameof(identificationNumber));
        CustomerNumber = $"CUS-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContactInfo(string email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";
}