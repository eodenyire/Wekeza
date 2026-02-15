using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Interfaces;

public interface IPaymentOrderRepository
{
    Task<PaymentOrder?> GetByIdAsync(Guid id);
    Task<PaymentOrder?> GetByPaymentReferenceAsync(string paymentReference);
    Task<IEnumerable<PaymentOrder>> GetByAccountIdAsync(Guid accountId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<PaymentOrder>> GetByStatusAsync(PaymentStatus status);
    Task<IEnumerable<PaymentOrder>> GetByTypeAsync(PaymentType type);
    Task<IEnumerable<PaymentOrder>> GetByChannelAsync(PaymentChannel channel);
    Task<IEnumerable<PaymentOrder>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<PaymentOrder>> GetPendingApprovalsAsync();
    Task<IEnumerable<PaymentOrder>> GetFailedPaymentsAsync();
    Task<IEnumerable<PaymentOrder>> GetHighValuePaymentsAsync(decimal threshold);
    Task<IEnumerable<PaymentOrder>> GetByCustomerAsync(Guid customerId, int pageSize = 50, int pageNumber = 1);
    Task<bool> ExistsByReferenceAsync(string paymentReference);
    Task<decimal> GetTotalAmountByAccountAsync(Guid accountId, DateTime date);
    Task<int> GetTransactionCountByAccountAsync(Guid accountId, DateTime date);
    void Add(PaymentOrder paymentOrder);
    void Update(PaymentOrder paymentOrder);
    void Remove(PaymentOrder paymentOrder);
}