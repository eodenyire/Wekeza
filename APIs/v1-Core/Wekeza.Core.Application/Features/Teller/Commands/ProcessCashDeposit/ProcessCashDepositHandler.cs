/*
 * TEMPORARILY COMMENTED OUT - FIXING COMPILATION ERRORS
 * This file will be restored and fixed incrementally
 * Missing ProcessCashDepositCommand and ProcessCashDepositResult classes
 */

/*
using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessCashDeposit;

/// <summary>
/// Process Cash Deposit Handler - Processes teller cash deposits
/// Uses TellerOperationsService for complete deposit processing with GL integration
/// </summary>
public class ProcessCashDepositHandler : IRequestHandler<ProcessCashDepositCommand, ProcessCashDepositResult>
{
    private readonly TellerOperationsService _tellerOperationsService;
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessCashDepositHandler(
        TellerOperationsService tellerOperationsService,
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _tellerOperationsService = tellerOperationsService;
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcessCashDepositResult> Handle(ProcessCashDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate account exists
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
            {
                return ProcessCashDepositResult.Failed("Account not found");
            }

            // 2. Validate account number matches
            if (account.AccountNumber.Value != request.AccountNumber)
            {
                return ProcessCashDepositResult.Failed("Account number mismatch");
            }

            // 3. Validate account is active
            if (account.Status != AccountStatus.Active)
            {
                return ProcessCashDepositResult.Failed($"Account is not active. Status: {account.Status}");
            }

            // 4. Create deposit amount
            var depositAmount = new Money(request.DepositAmount, new Currency(request.Currency));

            // 5. Validate deposit amount
            if (depositAmount.IsZero() || depositAmount.IsNegative())
            {
                return ProcessCashDepositResult.Failed("Deposit amount must be positive");
            }

            // 6. Process cash deposit using teller operations service
            var operationResult = await _tellerOperationsService.ProcessCashDepositAsync(
                request.SessionId,
                request.AccountId,
                depositAmount,
                request.VerificationMethod,
                request.CustomerPresent,
                request.Reference,
                request.Notes);

            if (!operationResult.IsSuccess)
            {
                return ProcessCashDepositResult.Failed(operationResult.ErrorMessage ?? "Cash deposit processing failed");
            }

            // 7. Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ProcessCashDepositResult.Success(
                operationResult.TransactionNumber!,
                operationResult.TransactionAmount!.Amount,
                operationResult.AccountBalance!.Amount,
                request.AccountNumber,
                operationResult.JournalNumber,
                operationResult.Message);
        }
        catch (Exception ex)
        {
            return ProcessCashDepositResult.Failed($"Error processing cash deposit: {ex.Message}");
        }
    }
}
*/