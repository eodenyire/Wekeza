using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Deposits.Commands.OpenCallDeposit;

/// <summary>
/// Handler for opening call deposit accounts
/// </summary>
public class OpenCallDepositHandler : IRequestHandler<OpenCallDepositCommand, Result<Guid>>
{
    private readonly ICallDepositRepository _callDepositRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OpenCallDepositHandler(
        ICallDepositRepository callDepositRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _callDepositRepository = callDepositRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(OpenCallDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate account exists and has sufficient balance
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
                return Result<Guid>.Failure("Account not found");

            if (account.Balance.Amount < request.InitialDeposit)
                return Result<Guid>.Failure("Insufficient account balance");

            // Check if deposit number already exists
            if (await _callDepositRepository.ExistsAsync(request.DepositNumber))
                return Result<Guid>.Failure("Deposit number already exists");

            // Validate minimum deposit requirement
            if (request.InitialDeposit < request.MinimumBalance)
                return Result<Guid>.Failure("Initial deposit is below minimum balance requirement");

            // Create call deposit
            var callDeposit = new CallDeposit(
                request.CallDepositId,
                request.AccountId,
                request.CustomerId,
                request.DepositNumber,
                new Money(request.InitialDeposit, new Currency(request.Currency)),
                new InterestRate(request.InterestRate),
                request.NoticePeriodDays,
                new Money(request.MinimumBalance, new Currency(request.Currency)),
                new Money(request.MaximumBalance, new Currency(request.Currency)),
                request.InterestFrequency,
                request.InstantAccess,
                request.BranchCode,
                request.CreatedBy);

            // Debit the account
            account.Debit(
                new Money(request.InitialDeposit, new Currency(request.Currency)),
                $"Call deposit opening - {request.DepositNumber}",
                request.CreatedBy);

            // Save changes
            await _callDepositRepository.AddAsync(callDeposit);
            await _accountRepository.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(callDeposit.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to open call deposit: {ex.Message}");
        }
    }
}