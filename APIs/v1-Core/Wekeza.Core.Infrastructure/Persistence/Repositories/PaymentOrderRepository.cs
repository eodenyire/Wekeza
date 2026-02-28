using Microsoft.EntityFrameworkCore;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Infrastructure.Persistence.Repositories;

public class PaymentOrderRepository : IPaymentOrderRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentOrder?> GetByIdAsync(Guid id)
    {
        return await _context.PaymentOrders
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PaymentOrder?> GetByPaymentReferenceAsync(string paymentReference)
    {
        return await _context.PaymentOrders
            .FirstOrDefaultAsync(p => p.PaymentReference == paymentReference);
    }

    public async Task<IEnumerable<PaymentOrder>> GetByAccountIdAsync(Guid accountId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.PaymentOrders
            .Where(p => p.FromAccountId == accountId || p.ToAccountId == accountId);

        if (fromDate.HasValue)
            query = query.Where(p => p.CreatedDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.CreatedDate <= toDate.Value);

        return await query
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentOrder>> GetByStatusAsync(PaymentStatus status)
    {
        return await _context.PaymentOrders
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentOrder>> GetByTypeAsync(PaymentType type)
    {
        return await _context.PaymentOrders
            .Where(p => p.Type == type)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentOrder>> GetByChannelAsync(PaymentChannel channel)
    {
        return await _context.PaymentOrders
            .Where(p => p.Channel == channel)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentOrder>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.PaymentOrders
            .Where(p => p.CreatedDate >= fromDate && p.CreatedDate <= toDate)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentOrder>> GetPendingApprovalsAsync()
    {
        return await _context.PaymentOrders
            .Where(p => p.Status == PaymentStatus.Pending && p.RequiresApproval)
            .OrderBy(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentOrder>> GetFailedPaymentsAsync()
    {
        return await _context.PaymentOrders
            .Where(p => p.Status == PaymentStatus.Failed)
            .OrderByDescending(p => p.LastModifiedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentOrder>> GetHighValuePaymentsAsync(decimal threshold)
    {
        return await _context.PaymentOrders
            .Where(p => p.Amount.Amount >= threshold)
            .OrderByDescending(p => p.Amount.Amount)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentOrder>> GetByCustomerAsync(Guid customerId, int pageSize = 50, int pageNumber = 1)
    {
        // Join with Accounts to get customer payments
        return await _context.PaymentOrders
            .Join(_context.Accounts,
                p => p.FromAccountId,
                a => a.Id,
                (p, a) => new { Payment = p, Account = a })
            .Where(pa => pa.Account.CustomerId == customerId)
            .Select(pa => pa.Payment)
            .OrderByDescending(p => p.CreatedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> ExistsByReferenceAsync(string paymentReference)
    {
        return await _context.PaymentOrders
            .AnyAsync(p => p.PaymentReference == paymentReference);
    }

    public async Task<decimal> GetTotalAmountByAccountAsync(Guid accountId, DateTime date)
    {
        return await _context.PaymentOrders
            .Where(p => p.FromAccountId == accountId 
                       && p.CreatedDate.Date == date.Date 
                       && p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount.Amount);
    }

    public async Task<int> GetTransactionCountByAccountAsync(Guid accountId, DateTime date)
    {
        return await _context.PaymentOrders
            .CountAsync(p => p.FromAccountId == accountId 
                           && p.CreatedDate.Date == date.Date 
                           && p.Status == PaymentStatus.Completed);
    }

    public void Add(PaymentOrder paymentOrder)
    {
        _context.PaymentOrders.Add(paymentOrder);
    }

    public void Update(PaymentOrder paymentOrder)
    {
        _context.PaymentOrders.Update(paymentOrder);
    }

    public void Remove(PaymentOrder paymentOrder)
    {
        _context.PaymentOrders.Remove(paymentOrder);
    }
}