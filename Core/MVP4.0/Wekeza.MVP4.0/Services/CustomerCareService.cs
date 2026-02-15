using Microsoft.EntityFrameworkCore;
using System.Text;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services
{
    public class CustomerCareService : ICustomerCareService
    {
        private readonly MVP4DbContext _context;
        private readonly ILogger<CustomerCareService> _logger;

        public CustomerCareService(MVP4DbContext context, ILogger<CustomerCareService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Customer & Account Enquiries
        public async Task<List<Customer>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                return await _context.Customers
                    .Where(c => c.FullName.Contains(searchTerm) || 
                               c.CustomerNumber.Contains(searchTerm) ||
                               c.Email.Contains(searchTerm) ||
                               c.PhoneNumber.Contains(searchTerm) ||
                               c.IdNumber.Contains(searchTerm))
                    .Include(c => c.Accounts)
                    .OrderBy(c => c.FullName)
                    .Take(20)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with term: {SearchTerm}", searchTerm);
                return new List<Customer>();
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid customerId)
        {
            try
            {
                return await _context.Customers
                    .Include(c => c.Accounts)
                    .Include(c => c.Documents)
                    .Include(c => c.Complaints)
                    .FirstOrDefaultAsync(c => c.Id == customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by ID: {CustomerId}", customerId);
                return null;
            }
        }

        public async Task<Customer?> GetCustomerByNumberAsync(string customerNumber)
        {
            try
            {
                return await _context.Customers
                    .Include(c => c.Accounts)
                    .Include(c => c.Documents)
                    .Include(c => c.Complaints)
                    .FirstOrDefaultAsync(c => c.CustomerNumber == customerNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by number: {CustomerNumber}", customerNumber);
                return null;
            }
        }

        public async Task<List<Account>> GetCustomerAccountsAsync(Guid customerId)
        {
            try
            {
                return await _context.Accounts
                    .Where(a => a.CustomerId == customerId)
                    .Include(a => a.Transactions.Take(10))
                    .Include(a => a.StandingInstructions)
                    .OrderBy(a => a.AccountNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer accounts: {CustomerId}", customerId);
                return new List<Account>();
            }
        }

        public async Task<Account?> GetAccountByNumberAsync(string accountNumber)
        {
            try
            {
                return await _context.Accounts
                    .Include(a => a.Customer)
                    .Include(a => a.Transactions.OrderByDescending(t => t.TransactionDate).Take(20))
                    .Include(a => a.StandingInstructions)
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account by number: {AccountNumber}", accountNumber);
                return null;
            }
        }

        public async Task<List<Transaction>> GetAccountTransactionsAsync(Guid accountId, int pageSize = 50)
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.AccountId == accountId)
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account transactions: {AccountId}", accountId);
                return new List<Transaction>();
            }
        }

        public async Task<List<StandingInstruction>> GetAccountStandingInstructionsAsync(Guid accountId)
        {
            try
            {
                return await _context.StandingInstructions
                    .Where(si => si.AccountId == accountId)
                    .OrderBy(si => si.NextExecutionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting standing instructions: {AccountId}", accountId);
                return new List<StandingInstruction>();
            }
        }

        // Customer Profile Maintenance (Maker Role)
        public async Task<bool> UpdateCustomerProfileAsync(Guid customerId, Customer updatedCustomer, string updatedBy)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null) return false;

                // Update allowed fields (CSO can capture changes but not authorize)
                customer.Address = updatedCustomer.Address;
                customer.PhoneNumber = updatedCustomer.PhoneNumber;
                customer.Email = updatedCustomer.Email;
                customer.EmployerName = updatedCustomer.EmployerName;
                customer.EmployerAddress = updatedCustomer.EmployerAddress;
                customer.UpdatedAt = DateTime.UtcNow;
                customer.UpdatedBy = updatedBy;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer profile: {CustomerId}", customerId);
                return false;
            }
        }

        public async Task<bool> UploadCustomerDocumentAsync(CustomerDocument document)
        {
            try
            {
                document.Id = Guid.NewGuid();
                document.UploadedAt = DateTime.UtcNow;
                document.Status = "Pending"; // Requires approval

                _context.CustomerDocuments.Add(document);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading customer document");
                return false;
            }
        }

        public async Task<List<CustomerDocument>> GetCustomerDocumentsAsync(Guid customerId)
        {
            try
            {
                return await _context.CustomerDocuments
                    .Where(d => d.CustomerId == customerId)
                    .OrderByDescending(d => d.UploadedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer documents: {CustomerId}", customerId);
                return new List<CustomerDocument>();
            }
        }

        // Account Status Requests (Initiate Only)
        public async Task<bool> CreateAccountStatusRequestAsync(AccountStatusRequest request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.RequestNumber = $"ASR{DateTime.Now:yyyyMMddHHmmss}";
                request.RequestedAt = DateTime.UtcNow;
                request.Status = "Pending";

                _context.AccountStatusRequests.Add(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account status request");
                return false;
            }
        }

        public async Task<List<AccountStatusRequest>> GetAccountStatusRequestsAsync(string? status = null)
        {
            try
            {
                var query = _context.AccountStatusRequests.AsQueryable();
                
                if (!string.IsNullOrEmpty(status))
                    query = query.Where(r => r.Status == status);

                return await query
                    .OrderByDescending(r => r.RequestedAt)
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account status requests");
                return new List<AccountStatusRequest>();
            }
        }

        public async Task<AccountStatusRequest?> GetAccountStatusRequestByIdAsync(Guid requestId)
        {
            try
            {
                return await _context.AccountStatusRequests.FindAsync(requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account status request: {RequestId}", requestId);
                return null;
            }
        }

        // Card & Channel Support
        public async Task<bool> CreateCardRequestAsync(CardRequest request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.RequestNumber = $"CR{DateTime.Now:yyyyMMddHHmmss}";
                request.RequestedAt = DateTime.UtcNow;
                request.Status = "Pending";

                _context.CardRequests.Add(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating card request");
                return false;
            }
        }

        public async Task<List<CardRequest>> GetCardRequestsAsync(string? status = null)
        {
            try
            {
                var query = _context.CardRequests.AsQueryable();
                
                if (!string.IsNullOrEmpty(status))
                    query = query.Where(r => r.Status == status);

                return await query
                    .OrderByDescending(r => r.RequestedAt)
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting card requests");
                return new List<CardRequest>();
            }
        }

        public async Task<CardRequest?> GetCardRequestByIdAsync(Guid requestId)
        {
            try
            {
                return await _context.CardRequests.FindAsync(requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting card request: {RequestId}", requestId);
                return null;
            }
        }

        // Complaint & Issue Handling
        public async Task<bool> CreateComplaintAsync(CustomerComplaint complaint)
        {
            try
            {
                complaint.Id = Guid.NewGuid();
                complaint.ComplaintNumber = $"CMP{DateTime.Now:yyyyMMddHHmmss}";
                complaint.CreatedAt = DateTime.UtcNow;
                complaint.Status = "Open";

                _context.CustomerComplaints.Add(complaint);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating complaint");
                return false;
            }
        }

        public async Task<List<CustomerComplaint>> GetComplaintsAsync(string? status = null)
        {
            try
            {
                var query = _context.CustomerComplaints
                    .Include(c => c.Customer)
                    .AsQueryable();
                
                if (!string.IsNullOrEmpty(status))
                    query = query.Where(c => c.Status == status);

                return await query
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaints");
                return new List<CustomerComplaint>();
            }
        }

        public async Task<CustomerComplaint?> GetComplaintByIdAsync(Guid complaintId)
        {
            try
            {
                return await _context.CustomerComplaints
                    .Include(c => c.Customer)
                    .Include(c => c.Updates)
                    .Include(c => c.Documents)
                    .FirstOrDefaultAsync(c => c.Id == complaintId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaint: {ComplaintId}", complaintId);
                return null;
            }
        }

        public async Task<bool> UpdateComplaintAsync(Guid complaintId, string updateText, string updatedBy)
        {
            try
            {
                var update = new ComplaintUpdate
                {
                    Id = Guid.NewGuid(),
                    ComplaintId = complaintId,
                    UpdateText = updateText,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = updatedBy
                };

                _context.ComplaintUpdates.Add(update);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating complaint: {ComplaintId}", complaintId);
                return false;
            }
        }

        public async Task<bool> UploadComplaintDocumentAsync(ComplaintDocument document)
        {
            try
            {
                document.Id = Guid.NewGuid();
                document.UploadedAt = DateTime.UtcNow;

                _context.ComplaintDocuments.Add(document);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading complaint document");
                return false;
            }
        }

        // Dashboard Statistics
        public async Task<int> GetActiveInquiriesCountAsync()
        {
            try
            {
                return await _context.CustomerComplaints
                    .CountAsync(c => c.Status == "Open" || c.Status == "In Progress");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active inquiries count");
                return 0;
            }
        }

        public async Task<int> GetResolvedTodayCountAsync()
        {
            try
            {
                var today = DateTime.Today;
                return await _context.CustomerComplaints
                    .CountAsync(c => c.Status == "Resolved" && 
                               c.ResolvedAt.HasValue && 
                               c.ResolvedAt.Value.Date == today);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resolved today count");
                return 0;
            }
        }

        public async Task<string> GetAverageResponseTimeAsync()
        {
            try
            {
                var resolvedComplaints = await _context.CustomerComplaints
                    .Where(c => c.Status == "Resolved" && c.ResolvedAt.HasValue)
                    .Select(c => new { c.CreatedAt, c.ResolvedAt })
                    .Take(100)
                    .ToListAsync();

                if (!resolvedComplaints.Any()) return "N/A";

                var avgMinutes = resolvedComplaints
                    .Average(c => (c.ResolvedAt!.Value - c.CreatedAt).TotalMinutes);

                return avgMinutes < 60 ? $"{avgMinutes:F0}m" : $"{avgMinutes / 60:F1}h";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average response time");
                return "N/A";
            }
        }

        public async Task<decimal> GetSatisfactionScoreAsync()
        {
            try
            {
                // Simulate satisfaction score based on resolution rate
                var totalComplaints = await _context.CustomerComplaints.CountAsync();
                var resolvedComplaints = await _context.CustomerComplaints
                    .CountAsync(c => c.Status == "Resolved");

                if (totalComplaints == 0) return 4.5m;

                var resolutionRate = (decimal)resolvedComplaints / totalComplaints;
                return Math.Round(3.5m + (resolutionRate * 1.5m), 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting satisfaction score");
                return 4.5m;
            }
        }

        public async Task<List<CustomerComplaint>> GetRecentComplaintsAsync(int count = 5)
        {
            try
            {
                return await _context.CustomerComplaints
                    .Include(c => c.Customer)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent complaints");
                return new List<CustomerComplaint>();
            }
        }

        public async Task<List<object>> GetRecentInquiriesAsync(int count = 5)
        {
            try
            {
                // Simulate inquiries from various sources
                var complaints = await _context.CustomerComplaints
                    .Include(c => c.Customer)
                    .Where(c => c.Category == "Inquiry" || c.Category == "Account Balance" || c.Category == "Transaction Query")
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(count)
                    .Select(c => new
                    {
                        Customer = c.Customer.FullName,
                        Subject = c.Subject,
                        Status = c.Status
                    })
                    .ToListAsync();

                return complaints.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent inquiries");
                return new List<object>();
            }
        }

        // Reports & Documentation
        public async Task<byte[]> GenerateCustomerStatementAsync(string accountNumber, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var account = await GetAccountByNumberAsync(accountNumber);
                if (account == null) return Array.Empty<byte>();

                var transactions = await _context.Transactions
                    .Where(t => t.AccountNumber == accountNumber && 
                               t.TransactionDate >= fromDate && 
                               t.TransactionDate <= toDate)
                    .OrderBy(t => t.TransactionDate)
                    .ToListAsync();

                var csv = new StringBuilder();
                csv.AppendLine("Account Statement");
                csv.AppendLine($"Account Number: {accountNumber}");
                csv.AppendLine($"Account Name: {account.AccountName}");
                csv.AppendLine($"Period: {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}");
                csv.AppendLine();
                csv.AppendLine("Date,Description,Reference,Debit,Credit,Balance");

                foreach (var txn in transactions)
                {
                    var debit = txn.Amount < 0 ? Math.Abs(txn.Amount).ToString("F2") : "";
                    var credit = txn.Amount > 0 ? txn.Amount.ToString("F2") : "";
                    csv.AppendLine($"{txn.TransactionDate:yyyy-MM-dd},{txn.Description},{txn.Reference},{debit},{credit},{txn.RunningBalance:F2}");
                }

                return Encoding.UTF8.GetBytes(csv.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating customer statement");
                return Array.Empty<byte>();
            }
        }

        public async Task<byte[]> GenerateBalanceConfirmationAsync(string accountNumber)
        {
            try
            {
                var account = await GetAccountByNumberAsync(accountNumber);
                if (account == null) return Array.Empty<byte>();

                var confirmation = new StringBuilder();
                confirmation.AppendLine("BALANCE CONFIRMATION LETTER");
                confirmation.AppendLine();
                confirmation.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd}");
                confirmation.AppendLine();
                confirmation.AppendLine($"Account Number: {account.AccountNumber}");
                confirmation.AppendLine($"Account Name: {account.AccountName}");
                confirmation.AppendLine($"Account Type: {account.AccountType}");
                confirmation.AppendLine();
                confirmation.AppendLine($"Current Balance: {account.Currency} {account.Balance:N2}");
                confirmation.AppendLine($"Available Balance: {account.Currency} {account.AvailableBalance:N2}");
                confirmation.AppendLine($"Account Status: {account.Status}");
                confirmation.AppendLine();
                confirmation.AppendLine("This letter confirms the account balance as of the date stated above.");

                return Encoding.UTF8.GetBytes(confirmation.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating balance confirmation");
                return Array.Empty<byte>();
            }
        }

        public async Task<byte[]> GenerateInterestCertificateAsync(string accountNumber, int year)
        {
            try
            {
                var account = await GetAccountByNumberAsync(accountNumber);
                if (account == null) return Array.Empty<byte>();

                // Simulate interest calculation
                var interestTransactions = await _context.Transactions
                    .Where(t => t.AccountNumber == accountNumber && 
                               t.TransactionType == "Interest Credit" &&
                               t.TransactionDate.Year == year)
                    .ToListAsync();

                var totalInterest = interestTransactions.Sum(t => t.Amount);

                var certificate = new StringBuilder();
                certificate.AppendLine("INTEREST CERTIFICATE");
                certificate.AppendLine();
                certificate.AppendLine($"Certificate Year: {year}");
                certificate.AppendLine($"Date Issued: {DateTime.Now:yyyy-MM-dd}");
                certificate.AppendLine();
                certificate.AppendLine($"Account Number: {account.AccountNumber}");
                certificate.AppendLine($"Account Name: {account.AccountName}");
                certificate.AppendLine();
                certificate.AppendLine($"Total Interest Earned in {year}: {account.Currency} {totalInterest:N2}");
                certificate.AppendLine();
                certificate.AppendLine("This certificate is issued for tax and record-keeping purposes.");

                return Encoding.UTF8.GetBytes(certificate.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating interest certificate");
                return Array.Empty<byte>();
            }
        }
    }
}