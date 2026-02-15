using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Deposits.Commands.OpenRecurringDeposit;

/// <summary>
/// Handler for opening Recurring Deposits
/// </summary>
public class OpenRecurringDepositHandler : IRequestHandler<OpenRecurringDepositCommand, Result<Guid>>
{
    private readonly IRecurringDepositRepository _recurringDepositRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OpenRecurringDepositHandler(
        IRecurringDepositRepository recurringDepositRepository,
        IAccountRepository accountRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _recurringDepositRepository = recurringDepositRepository;
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(OpenRecurringDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate account exists and is active
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                return Result<Guid>.Failure("Account not found");

            if (account.Status != Domain.Enums.AccountStatus.Active)
                return Result<Guid>.Failure("Account is not active");

            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
                return Result<Guid>.Failure("Customer not found");

            // Validate auto-debit account if specified
            Account? autoDebitAccount = null;
            if (request.AutoDebit && request.AutoDebitAccountId.HasValue)
            {
                autoDebitAccount = await _accountRepository.GetByIdAsync(request.AutoDebitAccountId.Value);
                if (autoDebitAccount == null)
                    return Result<Guid>.Failure("Auto-debit account not found");

                if (autoDebitAccount.Status != Domain.Enums.AccountStatus.Active)
                    return Result<Guid>.Failure("Auto-debit account is not active");

                // Ensure auto-debit account has sufficient balance for first installment
                var monthlyInstallment = new Money(request.MonthlyInstallment, new Currency(request.Currency));
                if (autoDebitAccount.Balance.Amount < monthlyInstallment.Amount)
                    return Result<Guid>.Failure("Insufficient balance in auto-debit account for first installment");
            }

            // Check for duplicate deposit number
            var existingDeposit = await _recurringDepositRepository.GetByDepositNumberAsync(request.DepositNumber);
            if (existingDeposit != null)
                return Result<Guid>.Failure("Deposit number already exists");

            // Create Recurring Deposit
            var monthlyInstallmentAmount = new Money(request.MonthlyInstallment, new Currency(request.Currency));
            var interestRate = new InterestRate(request.InterestRate);
            
            var recurringDeposit = new RecurringDeposit(
                request.DepositId,
                request.AccountId,
                request.CustomerId,
                request.DepositNumber,
                monthlyInstallmentAmount,
                interestRate,
                request.TenureInMonths,
                request.AutoDebit,
                request.AutoDebitAccountId,
                request.BranchCode,
                request.CreatedBy);

            // Process first installment if auto-debit is enabled
            if (request.AutoDebit && autoDebitAccount != null)
            {
                // Debit auto-debit account for first installment
                autoDebitAccount.Debit(monthlyInstallmentAmount, 
                    $"RD installment - {request.DepositNumber} - Month 1");

                // Credit the RD account
                account.Credit(monthlyInstallmentAmount, 
                    $"RD installment - {request.DepositNumber} - Month 1");

                // Process the installment in RD
                recurringDeposit.ProcessInstallment(monthlyInstallmentAmount, DateTime.UtcNow, request.CreatedBy);

                // Update auto-debit account
                await _accountRepository.UpdateAsync(autoDebitAccount);
            }

            // Save changes
            await _recurringDepositRepository.AddAsync(recurringDeposit);
            await _accountRepository.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(recurringDeposit.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to open recurring deposit: {ex.Message}");
        }
    }
}