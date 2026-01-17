using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Enhanced Loan Repository - Complete loan portfolio management
/// Contract for managing the Bank's Credit Portfolio with comprehensive querying capabilities
/// </summary>
public interface ILoanRepository
{
    // Basic CRUD operations
    Task<Loan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Loan?> GetByLoanNumberAsync(string loanNumber, CancellationToken ct = default);
    Task AddAsync(Loan loan, CancellationToken ct = default);
    void Add(Loan loan);
    void Update(Loan loan);
    void Remove(Loan loan);

    // Customer-based queries
    Task<IEnumerable<Loan>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetActiveLoansForCustomerAsync(Guid customerId, CancellationToken ct = default);

    // Status-based queries
    Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetBySubStatusAsync(LoanSubStatus subStatus, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetPendingDisbursalAsync(CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetPendingApprovalsAsync(CancellationToken ct = default);

    // Risk and performance queries
    Task<IEnumerable<Loan>> GetByRiskGradeAsync(CreditRiskGrade riskGrade, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetPastDueLoansAsync(int daysPastDue = 1, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetNonPerformingLoansAsync(CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetLoansForProvisioningAsync(DateTime asOfDate, CancellationToken ct = default);

    // Product-based queries
    Task<IEnumerable<Loan>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetByProductTypeAsync(ProductType productType, CancellationToken ct = default);

    // Date-based queries
    Task<IEnumerable<Loan>> GetByApplicationDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetByDisbursementDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetMaturityDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);

    // Interest and servicing queries
    Task<IEnumerable<Loan>> GetLoansForInterestAccrualAsync(DateTime calculationDate, CancellationToken ct = default);
    Task<IEnumerable<Loan>> GetLoansWithPaymentsDueAsync(DateTime dueDate, CancellationToken ct = default);

    // Portfolio analytics
    Task<decimal> GetTotalOutstandingPrincipalAsync(CancellationToken ct = default);
    Task<decimal> GetTotalOutstandingPrincipalByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<decimal> GetTotalProvisionAmountAsync(CancellationToken ct = default);
    Task<int> GetLoanCountByStatusAsync(LoanStatus status, CancellationToken ct = default);

    // Search and filtering
    Task<IEnumerable<Loan>> SearchLoansAsync(
        string? searchTerm = null,
        LoanStatus? status = null,
        CreditRiskGrade? riskGrade = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken ct = default);

    // Validation helpers
    Task<bool> ExistsByLoanNumberAsync(string loanNumber, CancellationToken ct = default);
    Task<bool> HasActiveLoansAsync(Guid customerId, CancellationToken ct = default);
}
