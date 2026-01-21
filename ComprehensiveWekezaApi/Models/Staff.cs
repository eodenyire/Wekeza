using System.ComponentModel.DataAnnotations;

namespace ComprehensiveWekezaApi.Models;

public class Staff
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string EmployeeId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    public string FullName => $"{FirstName} {LastName}";
    
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Role { get; set; } = string.Empty;
    
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    
    public DateTime? LastLogin { get; set; }
    
    // Navigation properties
    public virtual ICollection<StaffLogin> LoginHistory { get; set; } = new List<StaffLogin>();
}

public class StaffLogin
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public DateTime LoginTime { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    public virtual Staff Staff { get; set; } = null!;
}