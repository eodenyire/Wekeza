using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Api.Models;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Staff Management Controller - Handles banking staff operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,ITAdministrator,BranchManager")]
public class StaffManagementController : ControllerBase
{
    /// <summary>
    /// Create a new banking staff member
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
    {
        try
        {
            // Validate request
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Generate employee ID if not provided
            if (string.IsNullOrEmpty(request.EmployeeId))
            {
                request.EmployeeId = GenerateEmployeeId(request.Role, request.BranchId);
            }

            // Create staff member (mock implementation)
            var staffMember = new
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                BranchId = request.BranchId,
                DepartmentId = request.DepartmentId,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name
            };

            return Ok(new
            {
                Success = true,
                Message = "Staff member created successfully",
                Data = staffMember
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to create staff member",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all staff members with filtering
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetStaff(
        [FromQuery] string? role = null,
        [FromQuery] int? branchId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            // Mock staff data - replace with actual database query
            var staffMembers = GetMockStaffData()
                .Where(s => role == null || s.Role == role)
                .Where(s => branchId == null || s.BranchId == branchId)
                .Where(s => departmentId == null || s.DepartmentId == departmentId)
                .Where(s => status == null || s.Status == status)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Success = true,
                Data = staffMembers,
                Pagination = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = GetMockStaffData().Count()
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to retrieve staff members",
                Error = ex.Message
            });
        }
    }
    /// <summary>
    /// Update staff member details
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] UpdateStaffRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Mock update - replace with actual database update
            var updatedStaff = new
            {
                Id = id,
                EmployeeId = request.EmployeeId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                BranchId = request.BranchId,
                DepartmentId = request.DepartmentId,
                Status = request.Status,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = User.Identity?.Name
            };

            return Ok(new
            {
                Success = true,
                Message = "Staff member updated successfully",
                Data = updatedStaff
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to update staff member",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Deactivate staff member
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateStaff(Guid id, [FromBody] DeactivateStaffRequest request)
    {
        try
        {
            // Mock deactivation - replace with actual database update
            return Ok(new
            {
                Success = true,
                Message = "Staff member deactivated successfully",
                Data = new
                {
                    Id = id,
                    Status = "Inactive",
                    Reason = request.Reason,
                    DeactivatedAt = DateTime.UtcNow,
                    DeactivatedBy = User.Identity?.Name
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to deactivate staff member",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get staff member by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStaffById(Guid id)
    {
        try
        {
            var staffMember = GetMockStaffData().FirstOrDefault(s => s.Id == id);
            
            if (staffMember == null)
                return NotFound(new { Success = false, Message = "Staff member not found" });

            return Ok(new
            {
                Success = true,
                Data = staffMember
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to retrieve staff member",
                Error = ex.Message
            });
        }
    }
    /// <summary>
    /// Reset staff member password
    /// </summary>
    [HttpPost("{id:guid}/reset-password")]
    public async Task<IActionResult> ResetStaffPassword(Guid id)
    {
        try
        {
            // Generate temporary password
            var tempPassword = GenerateTemporaryPassword();

            return Ok(new
            {
                Success = true,
                Message = "Password reset successfully",
                Data = new
                {
                    Id = id,
                    TemporaryPassword = tempPassword,
                    MustChangeOnLogin = true,
                    ResetAt = DateTime.UtcNow,
                    ResetBy = User.Identity?.Name
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to reset password",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get staff statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStaffStatistics()
    {
        try
        {
            var mockData = GetMockStaffData();
            
            var statistics = new
            {
                TotalStaff = mockData.Count(),
                ActiveStaff = mockData.Count(s => s.Status == "Active"),
                InactiveStaff = mockData.Count(s => s.Status == "Inactive"),
                ByRole = mockData.GroupBy(s => s.Role)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToList(),
                ByBranch = mockData.GroupBy(s => s.BranchId)
                    .Select(g => new { BranchId = g.Key, Count = g.Count() })
                    .ToList(),
                ByDepartment = mockData.GroupBy(s => s.DepartmentId)
                    .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
                    .ToList()
            };

            return Ok(new
            {
                Success = true,
                Data = statistics
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to retrieve statistics",
                Error = ex.Message
            });
        }
    }

    #region Helper Methods

    private string GenerateEmployeeId(string role, int branchId)
    {
        var rolePrefix = role switch
        {
            "Teller" => "TEL",
            "CustomerService" => "CSR",
            "BackOfficeStaff" => "BOF",
            "LoanOfficer" => "LNO",
            "CashOfficer" => "CSH",
            "Supervisor" => "SUP",
            "BranchManager" => "BMG",
            "ComplianceOfficer" => "COM",
            "RiskOfficer" => "RSK",
            "InsuranceOfficer" => "INS",
            _ => "EMP"
        };

        var branchCode = branchId.ToString("D3");
        var sequence = new Random().Next(1000, 9999);
        
        return $"{rolePrefix}{branchCode}{sequence}";
    }

    private string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    private List<dynamic> GetMockStaffData()
    {
        return new List<dynamic>
        {
            new {
                Id = Guid.NewGuid(),
                EmployeeId = "TEL001001",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@wekeza.com",
                Phone = "+254700123456",
                Role = "Teller",
                BranchId = 1,
                BranchName = "Main Branch",
                DepartmentId = 1,
                DepartmentName = "Customer Service",
                Status = "Active",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastLogin = DateTime.UtcNow.AddHours(-2)
            },
            new {
                Id = Guid.NewGuid(),
                EmployeeId = "CSR001002",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@wekeza.com",
                Phone = "+254700123457",
                Role = "CustomerService",
                BranchId = 1,
                BranchName = "Main Branch",
                DepartmentId = 1,
                DepartmentName = "Customer Service",
                Status = "Active",
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                LastLogin = DateTime.UtcNow.AddHours(-1)
            },
            new {
                Id = Guid.NewGuid(),
                EmployeeId = "LNO001003",
                FirstName = "Michael",
                LastName = "Johnson",
                Email = "michael.johnson@wekeza.com",
                Phone = "+254700123458",
                Role = "LoanOfficer",
                BranchId = 1,
                BranchName = "Main Branch",
                DepartmentId = 3,
                DepartmentName = "Loans",
                Status = "Active",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                LastLogin = DateTime.UtcNow.AddHours(-3)
            },
            new {
                Id = Guid.NewGuid(),
                EmployeeId = "SUP001004",
                FirstName = "Sarah",
                LastName = "Wilson",
                Email = "sarah.wilson@wekeza.com",
                Phone = "+254700123459",
                Role = "Supervisor",
                BranchId = 1,
                BranchName = "Main Branch",
                DepartmentId = 1,
                DepartmentName = "Customer Service",
                Status = "Active",
                CreatedAt = DateTime.UtcNow.AddDays(-45),
                LastLogin = DateTime.UtcNow.AddMinutes(-30)
            },
            new {
                Id = Guid.NewGuid(),
                EmployeeId = "BMG001005",
                FirstName = "David",
                LastName = "Brown",
                Email = "david.brown@wekeza.com",
                Phone = "+254700123460",
                Role = "BranchManager",
                BranchId = 1,
                BranchName = "Main Branch",
                DepartmentId = 1,
                DepartmentName = "Customer Service",
                Status = "Active",
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                LastLogin = DateTime.UtcNow.AddMinutes(-15)
            }
        };
    }

    #endregion
}