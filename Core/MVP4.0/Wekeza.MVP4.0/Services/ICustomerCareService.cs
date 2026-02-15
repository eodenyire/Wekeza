using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services
{
    public interface ICustomerCareService
    {
        // Customer & Account Enquiries
        Task<List<Customer>> SearchCustomersAsync(string searchTerm);
        Task<Customer?> GetCustomerByIdAsync(Guid customerId);
        Task<Customer?> GetCustomerByNumberAsync(string customerNumber);
        Task<List<Account>> GetCustomerAccountsAsync(Guid customerId);
        Task<Account?> GetAccountByNumberAsync(string accountNumber);
        Task<List<Transaction>> GetAccountTransactionsAsync(Guid accountId, int pageSize = 50);
        Task<List<StandingInstruction>> GetAccountStandingInstructionsAsync(Guid accountId);

        // Customer Profile Maintenance (Maker Role)
        Task<bool> UpdateCustomerProfileAsync(Guid customerId, Customer updatedCustomer, string updatedBy);
        Task<bool> UploadCustomerDocumentAsync(CustomerDocument document);
        Task<List<CustomerDocument>> GetCustomerDocumentsAsync(Guid customerId);

        // Account Status Requests (Initiate Only)
        Task<bool> CreateAccountStatusRequestAsync(AccountStatusRequest request);
        Task<List<AccountStatusRequest>> GetAccountStatusRequestsAsync(string? status = null);
        Task<AccountStatusRequest?> GetAccountStatusRequestByIdAsync(Guid requestId);

        // Card & Channel Support
        Task<bool> CreateCardRequestAsync(CardRequest request);
        Task<List<CardRequest>> GetCardRequestsAsync(string? status = null);
        Task<CardRequest?> GetCardRequestByIdAsync(Guid requestId);

        // Complaint & Issue Handling
        Task<bool> CreateComplaintAsync(CustomerComplaint complaint);
        Task<List<CustomerComplaint>> GetComplaintsAsync(string? status = null);
        Task<CustomerComplaint?> GetComplaintByIdAsync(Guid complaintId);
        Task<bool> UpdateComplaintAsync(Guid complaintId, string updateText, string updatedBy);
        Task<bool> UploadComplaintDocumentAsync(ComplaintDocument document);

        // Dashboard Statistics
        Task<int> GetActiveInquiriesCountAsync();
        Task<int> GetResolvedTodayCountAsync();
        Task<string> GetAverageResponseTimeAsync();
        Task<decimal> GetSatisfactionScoreAsync();
        Task<List<CustomerComplaint>> GetRecentComplaintsAsync(int count = 5);
        Task<List<object>> GetRecentInquiriesAsync(int count = 5);

        // Reports & Documentation
        Task<byte[]> GenerateCustomerStatementAsync(string accountNumber, DateTime fromDate, DateTime toDate);
        Task<byte[]> GenerateBalanceConfirmationAsync(string accountNumber);
        Task<byte[]> GenerateInterestCertificateAsync(string accountNumber, int year);
    }
}