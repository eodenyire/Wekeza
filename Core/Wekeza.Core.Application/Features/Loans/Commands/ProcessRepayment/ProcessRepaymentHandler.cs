using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Loans.Commands.ProcessRepayment;

/// <summary>
/// Process Repayment Handler - Processes loan repayments with GL integration
/// Uses LoanServicingService for complete repayment processing with accounting
/// </summary>
public class ProcessRepaymentHandler : IRequestHandler<ProcessRepaymentCommand, ProcessRepaymentResult>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly LoanServicingService _loanServicingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessRepaymentHandler(
        ILoanRepository loanRepository,
        IAccountRepository accountRepository,
        LoanServicingService loanServicingService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _accountRepository = accountRepository;
        _loanServicingService = loanServicingService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcessRepaymentResult> Handle(ProcessRepaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get the loan
            var loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);
            if (loan == null)
            {
                return ProcessRepaymentResult.Failed("Loan not found");
            }

            // 2. Get the payment account
            var paymentAccount = await _accountRepository.GetByIdAsync(request.PaymentAccountId, cancellationToken);
            if (paymentAccount == null)
            {
                return ProcessRepaymentResult.Failed("Payment account not found");
            }

            // 3. Create payment amount
            var paymentAmount = new Money(request.PaymentAmount, Currency.FromCode(request.Currency));
            var paymentDate = request.PaymentDate ?? DateTime.UtcNow;

            // 4. Use loan servicing service for repayment processing
            var processedBy = request.ProcessedBy ?? (_currentUserService.UserId ?? Guid.Empty).ToString();
            var servicingResult = await _loanServicingService.ProcessRepaymentAsync(
                loan,
                paymentAmount,
                request.PaymentAccountId,
                paymentDate,
                processedBy,
                request.PaymentReference);

            if (!servicingResult.IsSuccess)
            {
                return ProcessRepaymentResult.Failed(servicingResult.ErrorMessage ?? "Repayment processing failed");
            }

            // 5. Save changes
            _loanRepository.Update(loan);
            _accountRepository.Update(paymentAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Return result with payment allocation details
            var allocation = servicingResult.PaymentAllocation;
            var isLoanPaidInFull = loan.Status == LoanStatus.PaidInFull;

            return ProcessRepaymentResult.Success(
                loan.LoanNumber,
                allocation?.TotalPayment.Amount ?? paymentAmount.Amount,
                allocation?.PrincipalPayment.Amount ?? 0,
                allocation?.InterestPayment.Amount ?? 0,
                allocation?.RemainingBalance.Amount ?? loan.OutstandingPrincipal.Amount,
                paymentDate,
                servicingResult.JournalNumber,
                isLoanPaidInFull,
                servicingResult.Message);
        }
        catch (Exception ex)
        {
            return ProcessRepaymentResult.Failed($"Error processing repayment: {ex.Message}");
        }
    }
}