using System.ComponentModel.DataAnnotations;

namespace Wekeza.Core.Api.Models;

/// <summary>
/// Request model for creating a new staff member
/// </summary>
public class CreateStaffRequest
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public int BranchId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    public string? EmployeeId { get; set; }
}

/// <summary>
/// Request model for updating staff member details
/// </summary>
public class UpdateStaffRequest
{
    [Required]
    public string EmployeeId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public int BranchId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Request model for deactivating a staff member
/// </summary>
public class DeactivateStaffRequest
{
    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Response model for staff member data
/// </summary>
public class StaffMemberResponse
{
    public Guid Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Response model for staff statistics
/// </summary>
public class StaffStatisticsResponse
{
    public int TotalStaff { get; set; }
    public int ActiveStaff { get; set; }
    public int InactiveStaff { get; set; }
    public List<RoleStatistic> ByRole { get; set; } = new();
    public List<BranchStatistic> ByBranch { get; set; } = new();
    public List<DepartmentStatistic> ByDepartment { get; set; } = new();
}

public class RoleStatistic
{
    public string Role { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class BranchStatistic
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DepartmentStatistic
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int Count { get; set; }
}