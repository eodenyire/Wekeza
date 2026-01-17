using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Infrastructure.Persistence;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

/// <summary>
/// Enhanced Loan Repository Implementation - Complete loan portfolio management
/// Manages the Bank's Credit Portfolio with comprehensive querying capabilities
/// Inspired by Finacle LMS and T24 Lending repositories
/// </summary>
public class LoanRepository : ILoanRepository
{
    private readonly ApplicationDbContext _context;

    public LoanRepository(ApplicationDbContext context) => _context = context;

    // Basic CRUD operations
    public async Task<Loan?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Include(l => l.DisbursementAccount)
            .FirstOrDefaultAsync(l => l.Id == id, ct);
    }

    public async Task<Loan?> GetByLoanNumberAsync(string loanNumber, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Include(l => l.DisbursementAccount)
            .FirstOrDefaultAsync(l => l.LoanNumber == loanNumber, ct);
    }

    public async Task AddAsync(Loan loan, CancellationToken ct = default)
    {
        await _context.Loans.AddAsync(loan, ct);
    }

    public void Add(Loan loan)
    {
        _context.Loans.Add(loan);
    }

    public void Update(Loan loan)
    {
        _context.Loans.Update(loan);
    }

    public void Remove(Loan loan)
    {
        _context.Loans.Remove(loan);
    }

    // Customer-based queries
    public async Task<IEnumerable<Loan>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Product)
            .Where(l => l.CustomerId == customerId)
            .OrderByDescending(l => l.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansForCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Product)
            .Where(l => l.CustomerId == customerId && l.Status == LoanStatus.Active)
            .ToListAsync(ct);
    }

    // Status-based queries
    public async Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.Status == status)
            .OrderByDescending(l => l.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetBySubStatusAsync(LoanSubStatus subStatus, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.SubStatus == subStatus)
            .OrderByDescending(l => l.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetPendingDisbursalAsync(CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.Status == LoanStatus.Approved && l.SubStatus == LoanSubStatus.AwaitingDisbursement)
            .OrderBy(l => l.ApprovalDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetPendingApprovalsAsync(CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.Status == LoanStatus.Applied)
            .OrderBy(l => l.ApplicationDate)
            .ToListAsync(ct);
    }

    // Risk and performance queries
    public async Task<IEnumerable<Loan>> GetByRiskGradeAsync(CreditRiskGrade riskGrade, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.RiskGrade == riskGrade)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetPastDueLoansAsync(int daysPastDue = 1, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.Status == LoanStatus.Active && l.DaysPastDue >= daysPastDue)
            .OrderByDescending(l => l.DaysPastDue)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetNonPerformingLoansAsync(CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.Status == LoanStatus.Active && l.SubStatus == LoanSubStatus.NonPerforming)
            .OrderByDescending(l => l.DaysPastDue)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetLoansForProvisioningAsync(DateTime asOfDate, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.Status == LoanStatus.Active && 
                       (l.LastProvisionDate == null || l.LastProvisionDate < asOfDate.Date))
            .ToListAsync(ct);
    }

    // Product-based queries
    public async Task<IEnumerable<Loan>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.ProductId == productId)
            .OrderByDescending(l => l.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetByProductTypeAsync(ProductType productType, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.Product!.Type == productType)
            .OrderByDescending(l => l.ApplicationDate)
            .ToListAsync(ct);
    }

    // Date-based queries
    public async Task<IEnumerable<Loan>> GetByApplicationDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.ApplicationDate.Date >= fromDate.Date && l.ApplicationDate.Date <= toDate.Date)
            .OrderByDescending(l => l.ApplicationDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetByDisbursementDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.DisbursementDate.HasValue && 
                       l.DisbursementDate.Value.Date >= fromDate.Date && 
                       l.DisbursementDate.Value.Date <= toDate.Date)
            .OrderByDescending(l => l.DisbursementDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetMaturityDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.MaturityDate.HasValue && 
                       l.MaturityDate.Value.Date >= fromDate.Date && 
                       l.MaturityDate.Value.Date <= toDate.Date)
            .OrderBy(l => l.MaturityDate)
            .ToListAsync(ct);
    }

    // Interest and servicing queries
    public async Task<IEnumerable<Loan>> GetLoansForInterestAccrualAsync(DateTime calculationDate, CancellationToken ct = default)
    {
        return await _context.Loans
            .Where(l => l.Status == LoanStatus.Active && 
                       l.LastInterestCalculationDate < calculationDate.Date)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Loan>> GetLoansWithPaymentsDueAsync(DateTime dueDate, CancellationToken ct = default)
    {
        // This would require schedule data - simplified for now
        return await _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .Where(l => l.Status == LoanStatus.Active)
            .ToListAsync(ct);
    }

    // Portfolio analytics
    public async Task<decimal> GetTotalOutstandingPrincipalAsync(CancellationToken ct = default)
    {
        return await _context.Loans
            .Where(l => l.Status == LoanStatus.Active)
            .SumAsync(l => l.OutstandingPrincipal.Amount, ct);
    }

    public async Task<decimal> GetTotalOutstandingPrincipalByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Loans
            .Where(l => l.CustomerId == customerId && l.Status == LoanStatus.Active)
            .SumAsync(l => l.OutstandingPrincipal.Amount, ct);
    }

    public async Task<decimal> GetTotalProvisionAmountAsync(CancellationToken ct = default)
    {
        return await _context.Loans
            .Where(l => l.Status == LoanStatus.Active)
            .SumAsync(l => l.ProvisionAmount.Amount, ct);
    }

    public async Task<int> GetLoanCountByStatusAsync(LoanStatus status, CancellationToken ct = default)
    {
        return await _context.Loans
            .CountAsync(l => l.Status == status, ct);
    }

    // Search and filtering
    public async Task<IEnumerable<Loan>> SearchLoansAsync(
        string? searchTerm = null,
        LoanStatus? status = null,
        CreditRiskGrade? riskGrade = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken ct = default)
    {
        var query = _context.Loans
            .Include(l => l.Customer)
            .Include(l => l.Product)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(l => 
                l.LoanNumber.Contains(searchTerm) ||
                (l.Customer != null && l.Customer.FullName.Contains(searchTerm)));
        }

        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status.Value);
        }

        if (riskGrade.HasValue)
        {
            query = query.Where(l => l.RiskGrade == riskGrade.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(l => l.ApplicationDate.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(l => l.ApplicationDate.Date <= toDate.Value.Date);
        }

        // Apply pagination
        return await query
            .OrderByDescending(l => l.ApplicationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    // Validation helpers
    public async Task<bool> ExistsByLoanNumberAsync(string loanNumber, CancellationToken ct = default)
    {
        return await _context.Loans
            .AnyAsync(l => l.LoanNumber == loanNumber, ct);
    }

    public async Task<bool> HasActiveLoansAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Loans
            .AnyAsync(l => l.CustomerId == customerId && l.Status == LoanStatus.Active, ct);
    }
}
