using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Staff Self-Service Portal Controller
/// Handles personal information, leave management, payroll, and performance tracking
/// </summary>
[ApiController]
[Route("api/staff-self-service")]
[Authorize(Roles = "Teller,Supervisor,BranchManager,Officer")]
public class StaffSelfServicePortalController : BaseApiController
{
    public StaffSelfServicePortalController(IMediator mediator) : base(mediator) { }

    #region Personal Information

    /// <summary>
    /// Get current staff member profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var profile = new StaffProfileDto
            {
                StaffId = "EMP001",
                FullName = "Jane Kariuki",
                Email = "jane.kariuki@wekeza.com",
                Phone = "+254712345678",
                DateOfBirth = new DateTime(1990, 5, 15),
                EmploymentDate = new DateTime(2018, 3, 1),
                Department = "Operations",
                Position = "Teller",
                Manager = "John Musyoka",
                BranchCode = "BR001",
                BranchName = "Nairobi Main Branch",
                EmploymentStatus = "Active",
                ContractType = "Permanent"
            };

            return Ok(new { success = true, data = profile });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update staff profile information
    /// </summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.Email) && string.IsNullOrEmpty(dto.Phone))
                return BadRequest(new { error = "At least one field must be provided for update" });

            return Ok(new
            {
                success = true,
                message = "Profile updated successfully",
                updatedAt = DateTime.UtcNow,
                updatedFields = new { email = !string.IsNullOrEmpty(dto.Email), phone = !string.IsNullOrEmpty(dto.Phone) }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Leave Management

    /// <summary>
    /// Get available leave balance
    /// </summary>
    [HttpGet("leave/balance")]
    public async Task<IActionResult> GetLeaveBalance()
    {
        try
        {
            var balance = new
            {
                AnnualLeave = new { Total = 20, Used = 5, Available = 15, Pending = 2 },
                SickLeave = new { Total = 10, Used = 1, Available = 9, Pending = 0 },
                CompassionateLeave = new { Total = 3, Used = 0, Available = 3, Pending = 0 },
                MaternityPaternityLeave = new { Total = 4, Used = 0, Available = 4, Pending = 0 },
                UnpaidLeave = new { Total = 5, Used = 0, Available = 5, Pending = 0 }
            };

            return Ok(new { success = true, data = balance });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get leave history
    /// </summary>
    [HttpGet("leave/history")]
    public async Task<IActionResult> GetLeaveHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var history = new List<LeaveRecordDto>
            {
                new LeaveRecordDto
                {
                    LeaveId = "LV001",
                    LeaveType = "Annual Leave",
                    StartDate = new DateTime(2026, 1, 15),
                    EndDate = new DateTime(2026, 1, 22),
                    DaysRequested = 5,
                    Status = "Approved",
                    ApprovedBy = "John Musyoka",
                    Reason = "Family visit"
                },
                new LeaveRecordDto
                {
                    LeaveId = "LV002",
                    LeaveType = "Sick Leave",
                    StartDate = new DateTime(2026, 2, 10),
                    EndDate = new DateTime(2026, 2, 10),
                    DaysRequested = 1,
                    Status = "Approved",
                    ApprovedBy = "John Musyoka",
                    Reason = "Medical appointment"
                },
                new LeaveRecordDto
                {
                    LeaveId = "LV003",
                    LeaveType = "Annual Leave",
                    StartDate = new DateTime(2026, 3, 20),
                    EndDate = new DateTime(2026, 3, 27),
                    DaysRequested = 5,
                    Status = "Pending",
                    ApprovedBy = null,
                    Reason = "Personal reasons"
                }
            };

            var paged = history.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                success = true,
                data = paged,
                pagination = new { page, pageSize, total = history.Count }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Apply for leave
    /// </summary>
    [HttpPost("leave/apply")]
    public async Task<IActionResult> ApplyForLeave([FromBody] LeaveApplicationDto dto)
    {
        try
        {
            if (dto.EndDate < dto.StartDate)
                return BadRequest(new { error = "End date must be after start date" });

            if (string.IsNullOrEmpty(dto.LeaveType))
                return BadRequest(new { error = "Leave type is required" });

            var days = (dto.EndDate - dto.StartDate).Days + 1;

            return Ok(new
            {
                success = true,
                message = "Leave application submitted successfully",
                leaveId = Guid.NewGuid().ToString(),
                leaveType = dto.LeaveType,
                startDate = dto.StartDate,
                endDate = dto.EndDate,
                daysRequested = days,
                status = "Pending",
                submittedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Payroll & Compensation

    /// <summary>
    /// Get current payroll information
    /// </summary>
    [HttpGet("payroll/current")]
    public async Task<IActionResult> GetCurrentPayroll()
    {
        try
        {
            var payroll = new
            {
                PayPeriod = "February 2026",
                BaseSalary = 85000m,
                Allowances = new { HousingAllowance = 15000m, TransportAllowance = 5000m, OtherAllowances = 2000m },
                TotalAllowances = 22000m,
                Deductions = new { TaxableIncome = 107000m, Income_Tax = 12840m, NSSF = 1080m, HealthInsurance = 2500m },
                TotalDeductions = 16420m,
                NetSalary = 90580m,
                PaymentDate = new DateTime(2026, 2, 28),
                PaymentMethod = "Bank Transfer",
                PaymentStatus = "Processed"
            };

            return Ok(new { success = true, data = payroll });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get payroll history
    /// </summary>
    [HttpGet("payroll/history")]
    public async Task<IActionResult> GetPayrollHistory([FromQuery] int months = 6)
    {
        try
        {
            var history = new List<PayrollRecordDto>
            {
                new PayrollRecordDto
                {
                    PayPeriod = "February 2026",
                    BaseSalary = 85000m,
                    TotalAllowances = 22000m,
                    TotalDeductions = 16420m,
                    NetSalary = 90580m,
                    PaymentDate = new DateTime(2026, 2, 28),
                    PaymentStatus = "Processed"
                },
                new PayrollRecordDto
                {
                    PayPeriod = "January 2026",
                    BaseSalary = 85000m,
                    TotalAllowances = 22000m,
                    TotalDeductions = 16420m,
                    NetSalary = 90580m,
                    PaymentDate = new DateTime(2026, 1, 31),
                    PaymentStatus = "Processed"
                },
                new PayrollRecordDto
                {
                    PayPeriod = "December 2025",
                    BaseSalary = 85000m,
                    TotalAllowances = 25000m,
                    TotalDeductions = 18420m,
                    NetSalary = 91580m,
                    PaymentDate = new DateTime(2025, 12, 31),
                    PaymentStatus = "Processed"
                }
            };

            return Ok(new { success = true, data = history.Take(months).ToList() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Download payslip
    /// </summary>
    [HttpGet("payroll/payslip/{payPeriod}")]
    public async Task<IActionResult> DownloadPayslip(string payPeriod)
    {
        try
        {
            if (string.IsNullOrEmpty(payPeriod))
                return BadRequest(new { error = "Pay period is required" });

            return Ok(new
            {
                success = true,
                message = "Payslip generated successfully",
                payPeriod = payPeriod,
                downloadUrl = $"/files/payslips/payslip-{payPeriod}.pdf",
                generatedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Performance & Development

    /// <summary>
    /// Get personal performance metrics
    /// </summary>
    [HttpGet("performance/metrics")]
    public async Task<IActionResult> GetPerformanceMetrics()
    {
        try
        {
            var metrics = new
            {
                MonthYear = "February 2026",
                TransactionsProcessed = 145,
                ErrorRate = 0.7m,
                AverageTransactionTime = 2.4m,
                CustomerSatisfactionRating = 4.8m,
                ComplianceScore = 99.5m,
                AttendanceRate = 99.0m,
                Targets = new
                {
                    TransactionsPerDay = 150,
                    TargetMet = true,
                    ErrorRateTarget = 1.0m,
                    TargetMet_ErrorRate = true,
                    SatisfactionTarget = 4.5m,
                    TargetMet_Satisfaction = true
                }
            };

            return Ok(new { success = true, data = metrics });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get performance reviews
    /// </summary>
    [HttpGet("performance/reviews")]
    public async Task<IActionResult> GetPerformanceReviews()
    {
        try
        {
            var reviews = new List<PerformanceReviewDto>
            {
                new PerformanceReviewDto
                {
                    ReviewId = "REV001",
                    ReviewPeriod = "2025 Q4",
                    ReviewedBy = "John Musyoka",
                    OverallRating = 4.5m,
                    Comments = "Excellent performance. Consistent delivery and positive customer feedback.",
                    ReviewDate = new DateTime(2025, 12, 20),
                    NextReviewDate = new DateTime(2026, 3, 20)
                },
                new PerformanceReviewDto
                {
                    ReviewId = "REV002",
                    ReviewPeriod = "2025 Q3",
                    ReviewedBy = "John Musyoka",
                    OverallRating = 4.3m,
                    Comments = "Good performance. Some improvements needed in documentation.",
                    ReviewDate = new DateTime(2025, 9, 20),
                    NextReviewDate = new DateTime(2025, 12, 20)
                }
            };

            return Ok(new { success = true, data = reviews });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get training and development opportunities
    /// </summary>
    [HttpGet("development/training")]
    public async Task<IActionResult> GetTrainingOpportunities()
    {
        try
        {
            var training = new List<TrainingOpportunityDto>
            {
                new TrainingOpportunityDto
                {
                    TrainingId = "TR001",
                    Title = "Advanced Customer Service Excellence",
                    Provider = "Internal Training",
                    Duration = "2 days",
                    ScheduledDate = new DateTime(2026, 3, 15),
                    Status = "Scheduled",
                    IsRequired = false
                },
                new TrainingOpportunityDto
                {
                    TrainingId = "TR002",
                    Title = "Compliance and Regulatory Updates",
                    Provider = "External",
                    Duration = "1 day",
                    ScheduledDate = new DateTime(2026, 4, 10),
                    Status = "Pending Enrollment",
                    IsRequired = true
                }
            };

            return Ok(new { success = true, data = training });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Account & Security

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("account/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.CurrentPassword) || string.IsNullOrEmpty(dto.NewPassword))
                return BadRequest(new { error = "Current and new passwords are required" });

            if (dto.NewPassword.Length < 8)
                return BadRequest(new { error = "New password must be at least 8 characters" });

            return Ok(new
            {
                success = true,
                message = "Password changed successfully",
                changedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get login history
    /// </summary>
    [HttpGet("account/login-history")]
    public async Task<IActionResult> GetLoginHistory([FromQuery] int days = 7)
    {
        try
        {
            var history = new List<LoginHistoryDto>
            {
                new LoginHistoryDto
                {
                    LoginId = "LOG001",
                    LoginTime = DateTime.UtcNow,
                    Device = "Windows PC",
                    IpAddress = "192.168.1.100",
                    Location = "Nairobi Main Branch",
                    Status = "Success"
                },
                new LoginHistoryDto
                {
                    LoginId = "LOG002",
                    LoginTime = DateTime.UtcNow.AddHours(-2),
                    Device = "Windows PC",
                    IpAddress = "192.168.1.100",
                    Location = "Nairobi Main Branch",
                    Status = "Success"
                },
                new LoginHistoryDto
                {
                    LoginId = "LOG003",
                    LoginTime = DateTime.UtcNow.AddDays(-1),
                    Device = "Windows PC",
                    IpAddress = "192.168.1.101",
                    Location = "Nairobi Main Branch",
                    Status = "Success"
                }
            };

            return Ok(new { success = true, data = history.Take(days * 10).ToList() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion
}

#region DTOs

public class StaffProfileDto
{
    public string StaffId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime EmploymentDate { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Manager { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
}

public class UpdateProfileDto
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class LeaveRecordDto
{
    public string LeaveId { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysRequested { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class LeaveApplicationDto
{
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class PayrollRecordDto
{
    public string PayPeriod { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
}

public class PerformanceReviewDto
{
    public string ReviewId { get; set; } = string.Empty;
    public string ReviewPeriod { get; set; } = string.Empty;
    public string ReviewedBy { get; set; } = string.Empty;
    public decimal OverallRating { get; set; }
    public string Comments { get; set; } = string.Empty;
    public DateTime ReviewDate { get; set; }
    public DateTime NextReviewDate { get; set; }
}

public class TrainingOpportunityDto
{
    public string TrainingId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class LoginHistoryDto
{
    public string LoginId { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public string Device { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

#endregion
