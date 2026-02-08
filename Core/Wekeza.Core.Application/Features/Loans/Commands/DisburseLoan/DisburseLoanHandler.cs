using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Features.Loans.Commands.DisburseLoan;

/// <summary>
/// Disburse Loan Handler - Processes loan disbursements with GL integration
/// Uses LoanServicingService for complete disbursement with accounting
/// </summary>
public class DisburseLoanHandler : IRequestHandler<DisburseLoanCommand, DisburseLoanResult>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly LoanServicingService _loanServicingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DisburseLoanHandler(
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

    public async Task<DisburseLoanResult> Handle(DisburseLoanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get the loan
            var loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);
            if (loan == null)
            {
                return DisburseLoanResult.Failed("Loan not found");
            }

            // 2. Get the disbursement account
            var disbursementAccount = await _accountRepository.GetByIdAsync(request.DisbursementAccountId, cancellationToken);
            if (disbursementAccount == null)
            {
                return DisburseLoanResult.Failed("Disbursement account not found");
            }

            // 3. Validate account belongs to loan customer
            if (disbursementAccount.CustomerId != loan.CustomerId)
            {
                return DisburseLoanResult.Failed("Disbursement account must belong to the loan customer");
            }

            // 4. Use loan servicing service for disbursement
            var disbursedBy = request.DisbursedBy ?? (_currentUserService.UserId ?? Guid.Empty).ToString();
            var servicingResult = await _loanServicingService.DisburseLoanAsync(
                loan, request.DisbursementAccountId, disbursedBy);

            if (!servicingResult.IsSuccess)
            {
                return DisburseLoanResult.Failed(servicingResult.ErrorMessage ?? "Disbursement failed");
            }

            // 5. Save changes
            _loanRepository.Update(loan);
            _accountRepository.Update(disbursementAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return DisburseLoanResult.Success(
                loan.LoanNumber,
                loan.Principal.Amount,
                disbursementAccount.AccountNumber.Value,
                loan.DisbursementDate ?? DateTime.UtcNow,
                servicingResult.JournalNumber,
                servicingResult.Message);
        }
        catch (Exception ex)
        {
            return DisburseLoanResult.Failed($"Error disbursing loan: {ex.Message}");
        }
    }
}
