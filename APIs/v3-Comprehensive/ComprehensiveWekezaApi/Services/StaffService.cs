using Microsoft.EntityFrameworkCore;
using DatabaseWekezaApi.Data;
using ComprehensiveWekezaApi.Models;

namespace ComprehensiveWekezaApi.Services;

public interface IStaffService
{
    Task<Staff> CreateStaffAsync(CreateStaffRequest request, string createdBy);
    Task<Staff?> GetStaffByIdAsync(Guid id);
    Task<Staff?> GetStaffByEmployeeIdAsync(string employeeId);
    Task<Staff?> GetStaffByEmailAsync(string email);
    Task<List<Staff>> GetAllStaffAsync();
    Task<Staff?> UpdateStaffAsync(Guid id, UpdateStaffRequest request, string updatedBy);
    Task<bool> DeactivateStaffAsync(Guid id, string reason, string deactivatedBy);
    Task<Staff?> AuthenticateStaffAsync(string username, string password);
    Task LogStaffLoginAsync(Guid staffId, string? ipAddress, string? userAgent);
}

public class StaffService : IStaffService
{
    private readonly WekezaDbContext _context;

    public StaffService(WekezaDbContext context)
    {
        _context = context;
    }

    public async Task<Staff> CreateStaffAsync(CreateStaffRequest request, string createdBy)
    {
        // Generate employee ID if not provided
        var employeeId = string.IsNullOrEmpty(request.EmployeeId) 
            ? await GenerateEmployeeIdAsync(request.Role, request.BranchId)
            : request.EmployeeId;

        // Check if employee ID already exists
        if (await _context.Staff.AnyAsync(s => s.EmployeeId == employeeId))
        {
            throw new InvalidOperationException($"Employee ID {employeeId} already exists");
        }

        // Check if email already exists
        if (await _context.Staff.AnyAsync(s => s.Email == request.Email))
        {
            throw new InvalidOperationException($"Email {request.Email} already exists");
        }

        var staff = new Staff
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Role = request.Role,
            BranchId = request.BranchId,
            BranchName = GetBranchName(request.BranchId),
            DepartmentId = request.DepartmentId,
            DepartmentName = GetDepartmentName(request.DepartmentId),
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();

        return staff;
    }

    public async Task<Staff?> GetStaffByIdAsync(Guid id)
    {
        return await _context.Staff
            .Include(s => s.LoginHistory)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Staff?> GetStaffByEmployeeIdAsync(string employeeId)
    {
        return await _context.Staff
            .Include(s => s.LoginHistory)
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId);
    }

    public async Task<Staff?> GetStaffByEmailAsync(string email)
    {
        return await _context.Staff
            .Include(s => s.LoginHistory)
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<List<Staff>> GetAllStaffAsync()
    {
        return await _context.Staff
            .Include(s => s.LoginHistory)
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.LastName)
            .ToListAsync();
    }

    public async Task<Staff?> UpdateStaffAsync(Guid id, UpdateStaffRequest request, string updatedBy)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null) return null;

        // Check if email is being changed and if new email already exists
        if (staff.Email != request.Email && await _context.Staff.AnyAsync(s => s.Email == request.Email && s.Id != id))
        {
            throw new InvalidOperationException($"Email {request.Email} already exists");
        }

        staff.FirstName = request.FirstName;
        staff.LastName = request.LastName;
        staff.Email = request.Email;
        staff.Phone = request.Phone;
        staff.Role = request.Role;
        staff.BranchId = request.BranchId;
        staff.BranchName = GetBranchName(request.BranchId);
        staff.DepartmentId = request.DepartmentId;
        staff.DepartmentName = GetDepartmentName(request.DepartmentId);
        staff.Status = request.Status;
        staff.UpdatedAt = DateTime.UtcNow;
        staff.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<bool> DeactivateStaffAsync(Guid id, string reason, string deactivatedBy)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null) return false;

        staff.Status = "Inactive";
        staff.UpdatedAt = DateTime.UtcNow;
        staff.UpdatedBy = deactivatedBy;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Staff?> AuthenticateStaffAsync(string username, string password)
    {
        // For demo purposes, we'll authenticate by username (employee ID or email)
        // In production, you'd hash and verify passwords
        var staff = await _context.Staff
            .FirstOrDefaultAsync(s => 
                (s.EmployeeId == username || s.Email == username) && 
                s.Status == "Active");

        return staff;
    }

    public async Task LogStaffLoginAsync(Guid staffId, string? ipAddress, string? userAgent)
    {
        var loginRecord = new StaffLogin
        {
            Id = Guid.NewGuid(),
            StaffId = staffId,
            LoginTime = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        _context.StaffLogins.Add(loginRecord);

        // Update last login time
        var staff = await _context.Staff.FindAsync(staffId);
        if (staff != null)
        {
            staff.LastLogin = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<string> GenerateEmployeeIdAsync(string role, int branchId)
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
            "TreasuryOfficer" => "TRS",
            "PaymentOfficer" => "PAY",
            "TradeFinanceOfficer" => "TRF",
            _ => "EMP"
        };

        var branchCode = branchId.ToString("D3");
        
        // Find the next sequence number
        var existingStaff = await _context.Staff
            .Where(s => s.EmployeeId.StartsWith($"{rolePrefix}{branchCode}"))
            .OrderByDescending(s => s.EmployeeId)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (existingStaff != null)
        {
            var lastSequence = existingStaff.EmployeeId.Substring(6); // Skip prefix and branch code
            if (int.TryParse(lastSequence, out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{rolePrefix}{branchCode}{sequence:D4}";
    }

    private string GetBranchName(int branchId)
    {
        return branchId switch
        {
            1 => "Main Branch",
            2 => "Westlands Branch", 
            3 => "Eastleigh Branch",
            4 => "Mombasa Branch",
            5 => "Kisumu Branch",
            _ => "Unknown Branch"
        };
    }

    private string GetDepartmentName(int departmentId)
    {
        return departmentId switch
        {
            1 => "Customer Service",
            2 => "Teller Operations",
            3 => "Loans & Credit",
            4 => "Treasury & Investments",
            5 => "Trade Finance",
            6 => "Payments",
            7 => "Compliance & Risk",
            8 => "Finance & Accounting",
            9 => "IT & Systems",
            10 => "Product Management",
            _ => "Unknown Department"
        };
    }
}

// Request DTOs
public class CreateStaffRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int DepartmentId { get; set; }
    public string? EmployeeId { get; set; }
}

public class UpdateStaffRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int DepartmentId { get; set; }
    public string Status { get; set; } = string.Empty;
}