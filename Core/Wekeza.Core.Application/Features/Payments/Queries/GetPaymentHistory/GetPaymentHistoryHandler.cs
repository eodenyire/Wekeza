using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Payments.Queries.GetPaymentHistory;

public class GetPaymentHistoryHandler : IRequestHandler<GetPaymentHistoryQuery, GetPaymentHistoryResult>
{
    private readonly IPaymentOrderRepository _paymentOrderRepository;
    private readonly IAccountRepository _accountRepository;

    public GetPaymentHistoryHandler(
        IPaymentOrderRepository paymentOrderRepository,
        IAccountRepository accountRepository)
    {
        _paymentOrderRepository = paymentOrderRepository;
        _accountRepository = accountRepository;
    }

    public async Task<GetPaymentHistoryResult> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        // Resolve account ID if account number provided
        var accountId = request.AccountId;
        if (!accountId.HasValue && !string.IsNullOrEmpty(request.AccountNumber))
        {
            var account = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.AccountNumber));
            accountId = account?.Id;
        }

        // Get payments based on criteria
        IEnumerable<Domain.Aggregates.PaymentOrder> payments;

        if (request.CustomerId.HasValue)
        {
            payments = await _paymentOrderRepository.GetByCustomerAsync(
                request.CustomerId, 
                request.PageSize * 2, // Get more to filter
                1);
        }
        else if (accountId.HasValue)
        {
            payments = await _paymentOrderRepository.GetByAccountIdAsync(
                accountId.Value,
                request.FromDate,
                request.ToDate);
        }
        else if (request.FromDate.HasValue && request.ToDate.HasValue)
        {
            payments = await _paymentOrderRepository.GetByDateRangeAsync(
                request.FromDate.Value,
                request.ToDate.Value);
        }
        else
        {
            // Default to recent payments
            payments = await _paymentOrderRepository.GetByDateRangeAsync(
                DateTime.UtcNow.AddDays(-30),
                DateTime.UtcNow);
        }

        // Apply filters
        var filteredPayments = payments.AsQueryable();

        if (request.Type.HasValue)
            filteredPayments = filteredPayments.Where(p => p.Type == request.Type.Value);

        if (request.Status.HasValue)
            filteredPayments = filteredPayments.Where(p => p.Status == request.Status.Value);

        if (request.Channel.HasValue)
            filteredPayments = filteredPayments.Where(p => p.Channel == request.Channel.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            filteredPayments = filteredPayments.Where(p =>
                p.PaymentReference.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm) ||
                (p.BeneficiaryName != null && p.BeneficiaryName.ToLower().Contains(searchTerm)) ||
                (p.CustomerReference != null && p.CustomerReference.ToLower().Contains(searchTerm)));
        }

        // Calculate totals
        var totalCount = filteredPayments.Count();
        var totalAmount = filteredPayments.Sum(p => p.Amount.Amount);

        // Apply pagination
        var pagedPayments = filteredPayments
            .OrderByDescending(p => p.CreatedDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Map to DTOs
        var paymentDtos = pagedPayments.Select(MapToDto).ToList();

        return new GetPaymentHistoryResult
        {
            Payments = paymentDtos,
            TotalCount = totalCount,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
            TotalAmount = totalAmount
        };
    }

    private static PaymentHistoryDto MapToDto(Domain.Aggregates.PaymentOrder payment)
    {
        return new PaymentHistoryDto
        {
            Id = payment.Id,
            PaymentReference = payment.PaymentReference,
            Type = payment.Type,
            Channel = payment.Channel,
            Status = payment.Status,
            Priority = payment.Priority,
            FromAccountNumber = payment.FromAccountNumber,
            ToAccountNumber = payment.ToAccountNumber,
            BeneficiaryName = payment.BeneficiaryName,
            BeneficiaryBank = payment.BeneficiaryBank,
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency.Code,
            Description = payment.Description,
            CustomerReference = payment.CustomerReference,
            ExternalReference = payment.ExternalReference,
            FeeAmount = payment.FeeAmount?.Amount,
            FeeBearer = payment.FeeBearer,
            RequestedDate = payment.RequestedDate,
            ValueDate = payment.ValueDate,
            ProcessedDate = payment.ProcessedDate,
            SettledDate = payment.SettledDate,
            RequiresApproval = payment.RequiresApproval,
            ApprovedBy = payment.ApprovedBy,
            ApprovedDate = payment.ApprovedDate,
            FailureReason = payment.FailureReason,
            RetryCount = payment.RetryCount,
            CreatedBy = payment.CreatedBy,
            CreatedDate = payment.CreatedDate
        };
    }
}