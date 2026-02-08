using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Deposits.Commands.BookFixedDeposit;

/// <summary>
/// Handler for booking Fixed Deposits
/// </summary>
public class BookFixedDepositHandler : IRequestHandler<BookFixedDepositCommand, Result<Guid>>
{
    private readonly IFixedDepositRepository _fixedDepositRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookFixedDepositHandler(
        IFixedDepositRepository fixedDepositRepository,
        IAccountRepository accountRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _fixedDepositRepository = fixedDepositRepository;
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(BookFixedDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate account exists and is active
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
                return Result<Guid>.Failure("Account not found");

            if (account.Status != Domain.Enums.AccountStatus.Active)
                return Result<Guid>.Failure("Account is not active");

            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null)
                return Result<Guid>.Failure("Customer not found");

            // Validate sufficient balance for deposit
            var principalAmount = new Money(request.PrincipalAmount, Currency.FromCode(request.Currency));
            if (account.Balance.Amount < principalAmount.Amount)
                return Result<Guid>.Failure("Insufficient account balance for deposit");

            // Check for duplicate deposit number
            var existingDeposit = await _fixedDepositRepository.GetByDepositNumberAsync(request.DepositNumber, cancellationToken);
            if (existingDeposit != null)
                return Result<Guid>.Failure("Deposit number already exists");

            // Create Fixed Deposit
            var interestRate = new InterestRate(request.InterestRate);
            var fixedDeposit = new FixedDeposit(
                request.DepositId,
                request.AccountId,
                request.CustomerId,
                request.DepositNumber,
                principalAmount,
                interestRate,
                request.TermInDays,
                request.InterestFrequency,
                request.AutoRenewal,
                request.BranchCode,
                request.CreatedBy);

            // Update renewal instructions if provided
            if (!string.IsNullOrEmpty(request.RenewalInstructions))
            {
                fixedDeposit.UpdateRenewalInstructions(request.RenewalInstructions, request.CreatedBy);
            }

            // Debit the account for the deposit amount
            account.Debit(principalAmount, $"Fixed Deposit booking - {request.DepositNumber}");

            // Save changes
            await _fixedDepositRepository.AddAsync(fixedDeposit, cancellationToken);
            await _accountRepository.UpdateAsync(account, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(fixedDeposit.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to book fixed deposit: {ex.Message}");
        }
    }
}