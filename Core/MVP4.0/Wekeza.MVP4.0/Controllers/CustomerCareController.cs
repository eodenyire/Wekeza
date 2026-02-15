using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.MVP4._0.Models;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "CustomerCareOfficer")]
    public class CustomerCareController : ControllerBase
    {
        private readonly ICustomerCareService _customerCareService;
        private readonly ILogger<CustomerCareController> _logger;

        public CustomerCareController(ICustomerCareService customerCareService, ILogger<CustomerCareController> logger)
        {
            _customerCareService = customerCareService;
            _logger = logger;
        }

        // Customer Search
        [HttpGet("customers/search")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest(new { success = false, message = "Search term is required" });

                var customers = await _customerCareService.SearchCustomersAsync(searchTerm);
                return Ok(new { success = true, data = customers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers");
                return StatusCode(500, new { success = false, message = "Error searching customers" });
            }
        }

        // Customer Details
        [HttpGet("customers/{customerId}")]
        public async Task<IActionResult> GetCustomer(Guid customerId)
        {
            try
            {
                var customer = await _customerCareService.GetCustomerByIdAsync(customerId);
                if (customer == null)
                    return NotFound(new { success = false, message = "Customer not found" });

                return Ok(new { success = true, data = customer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer details");
                return StatusCode(500, new { success = false, message = "Error retrieving customer details" });
            }
        }

        // Customer Accounts
        [HttpGet("customers/{customerId}/accounts")]
        public async Task<IActionResult> GetCustomerAccounts(Guid customerId)
        {
            try
            {
                var accounts = await _customerCareService.GetCustomerAccountsAsync(customerId);
                return Ok(new { success = true, data = accounts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer accounts");
                return StatusCode(500, new { success = false, message = "Error retrieving customer accounts" });
            }
        }

        // Account Details
        [HttpGet("accounts/{accountNumber}")]
        public async Task<IActionResult> GetAccount(string accountNumber)
        {
            try
            {
                var account = await _customerCareService.GetAccountByNumberAsync(accountNumber);
                if (account == null)
                    return NotFound(new { success = false, message = "Account not found" });

                return Ok(new { success = true, data = account });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account details");
                return StatusCode(500, new { success = false, message = "Error retrieving account details" });
            }
        }

        // Account Transactions
        [HttpGet("accounts/{accountId}/transactions")]
        public async Task<IActionResult> GetAccountTransactions(Guid accountId, [FromQuery] int pageSize = 50)
        {
            try
            {
                var transactions = await _customerCareService.GetAccountTransactionsAsync(accountId, pageSize);
                return Ok(new { success = true, data = transactions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account transactions");
                return StatusCode(500, new { success = false, message = "Error retrieving transactions" });
            }
        }

        // Standing Instructions
        [HttpGet("accounts/{accountId}/standing-instructions")]
        public async Task<IActionResult> GetStandingInstructions(Guid accountId)
        {
            try
            {
                var instructions = await _customerCareService.GetAccountStandingInstructionsAsync(accountId);
                return Ok(new { success = true, data = instructions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting standing instructions");
                return StatusCode(500, new { success = false, message = "Error retrieving standing instructions" });
            }
        }

        // Update Customer Profile (Maker Role)
        [HttpPut("customers/{customerId}/profile")]
        public async Task<IActionResult> UpdateCustomerProfile(Guid customerId, [FromBody] Customer updatedCustomer)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                var success = await _customerCareService.UpdateCustomerProfileAsync(customerId, updatedCustomer, username);
                
                if (success)
                    return Ok(new { success = true, message = "Customer profile updated successfully (pending approval)" });
                else
                    return BadRequest(new { success = false, message = "Failed to update customer profile" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer profile");
                return StatusCode(500, new { success = false, message = "Error updating customer profile" });
            }
        }

        // Account Status Requests
        [HttpPost("account-status-requests")]
        public async Task<IActionResult> CreateAccountStatusRequest([FromBody] AccountStatusRequest request)
        {
            try
            {
                request.RequestedBy = User.Identity?.Name ?? "Unknown";
                var success = await _customerCareService.CreateAccountStatusRequestAsync(request);
                
                if (success)
                    return Ok(new { success = true, message = "Account status request created successfully" });
                else
                    return BadRequest(new { success = false, message = "Failed to create account status request" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account status request");
                return StatusCode(500, new { success = false, message = "Error creating account status request" });
            }
        }

        [HttpGet("account-status-requests")]
        public async Task<IActionResult> GetAccountStatusRequests([FromQuery] string? status = null)
        {
            try
            {
                var requests = await _customerCareService.GetAccountStatusRequestsAsync(status);
                return Ok(new { success = true, data = requests });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account status requests");
                return StatusCode(500, new { success = false, message = "Error retrieving account status requests" });
            }
        }

        // Card Requests
        [HttpPost("card-requests")]
        public async Task<IActionResult> CreateCardRequest([FromBody] CardRequest request)
        {
            try
            {
                request.RequestedBy = User.Identity?.Name ?? "Unknown";
                var success = await _customerCareService.CreateCardRequestAsync(request);
                
                if (success)
                    return Ok(new { success = true, message = "Card request created successfully" });
                else
                    return BadRequest(new { success = false, message = "Failed to create card request" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating card request");
                return StatusCode(500, new { success = false, message = "Error creating card request" });
            }
        }

        [HttpGet("card-requests")]
        public async Task<IActionResult> GetCardRequests([FromQuery] string? status = null)
        {
            try
            {
                var requests = await _customerCareService.GetCardRequestsAsync(status);
                return Ok(new { success = true, data = requests });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting card requests");
                return StatusCode(500, new { success = false, message = "Error retrieving card requests" });
            }
        }

        // Complaints
        [HttpPost("complaints")]
        public async Task<IActionResult> CreateComplaint([FromBody] CustomerComplaint complaint)
        {
            try
            {
                complaint.CreatedBy = User.Identity?.Name ?? "Unknown";
                var success = await _customerCareService.CreateComplaintAsync(complaint);
                
                if (success)
                    return Ok(new { success = true, message = "Complaint created successfully" });
                else
                    return BadRequest(new { success = false, message = "Failed to create complaint" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating complaint");
                return StatusCode(500, new { success = false, message = "Error creating complaint" });
            }
        }

        [HttpGet("complaints")]
        public async Task<IActionResult> GetComplaints([FromQuery] string? status = null)
        {
            try
            {
                var complaints = await _customerCareService.GetComplaintsAsync(status);
                return Ok(new { success = true, data = complaints });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaints");
                return StatusCode(500, new { success = false, message = "Error retrieving complaints" });
            }
        }

        [HttpGet("complaints/{complaintId}")]
        public async Task<IActionResult> GetComplaint(Guid complaintId)
        {
            try
            {
                var complaint = await _customerCareService.GetComplaintByIdAsync(complaintId);
                if (complaint == null)
                    return NotFound(new { success = false, message = "Complaint not found" });

                return Ok(new { success = true, data = complaint });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaint details");
                return StatusCode(500, new { success = false, message = "Error retrieving complaint details" });
            }
        }

        [HttpPost("complaints/{complaintId}/updates")]
        public async Task<IActionResult> UpdateComplaint(Guid complaintId, [FromBody] dynamic updateData)
        {
            try
            {
                string updateText = updateData.updateText;
                var username = User.Identity?.Name ?? "Unknown";
                var success = await _customerCareService.UpdateComplaintAsync(complaintId, updateText, username);
                
                if (success)
                    return Ok(new { success = true, message = "Complaint updated successfully" });
                else
                    return BadRequest(new { success = false, message = "Failed to update complaint" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating complaint");
                return StatusCode(500, new { success = false, message = "Error updating complaint" });
            }
        }

        // Dashboard Statistics
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    activeInquiries = await _customerCareService.GetActiveInquiriesCountAsync(),
                    resolvedToday = await _customerCareService.GetResolvedTodayCountAsync(),
                    avgResponseTime = await _customerCareService.GetAverageResponseTimeAsync(),
                    satisfactionScore = await _customerCareService.GetSatisfactionScoreAsync(),
                    recentComplaints = await _customerCareService.GetRecentComplaintsAsync(),
                    recentInquiries = await _customerCareService.GetRecentInquiriesAsync()
                };

                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, new { success = false, message = "Error retrieving dashboard statistics" });
            }
        }

        // Reports
        [HttpGet("reports/statement/{accountNumber}")]
        public async Task<IActionResult> GenerateStatement(string accountNumber, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var statement = await _customerCareService.GenerateCustomerStatementAsync(accountNumber, fromDate, toDate);
                if (statement.Length == 0)
                    return NotFound(new { success = false, message = "Account not found or no transactions" });

                return File(statement, "text/csv", $"statement_{accountNumber}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating statement");
                return StatusCode(500, new { success = false, message = "Error generating statement" });
            }
        }

        [HttpGet("reports/balance-confirmation/{accountNumber}")]
        public async Task<IActionResult> GenerateBalanceConfirmation(string accountNumber)
        {
            try
            {
                var confirmation = await _customerCareService.GenerateBalanceConfirmationAsync(accountNumber);
                if (confirmation.Length == 0)
                    return NotFound(new { success = false, message = "Account not found" });

                return File(confirmation, "text/plain", $"balance_confirmation_{accountNumber}_{DateTime.Now:yyyyMMdd}.txt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating balance confirmation");
                return StatusCode(500, new { success = false, message = "Error generating balance confirmation" });
            }
        }

        [HttpGet("reports/interest-certificate/{accountNumber}")]
        public async Task<IActionResult> GenerateInterestCertificate(string accountNumber, [FromQuery] int year)
        {
            try
            {
                var certificate = await _customerCareService.GenerateInterestCertificateAsync(accountNumber, year);
                if (certificate.Length == 0)
                    return NotFound(new { success = false, message = "Account not found" });

                return File(certificate, "text/plain", $"interest_certificate_{accountNumber}_{year}.txt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating interest certificate");
                return StatusCode(500, new { success = false, message = "Error generating interest certificate" });
            }
        }
    }
}