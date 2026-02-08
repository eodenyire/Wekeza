using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Deposits.Commands.BookTermDeposit;

/// <summary>
/// Handler for booking term deposits
/// </summary>
public class BookTermDepositHandler : IRequestHandler<BookTermDepositCommand, Result<Guid>>
{
    private readonly ITermDepositRepository _termDepositRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookTermDepositHandler(
        ITermDepositRepository termDepositRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _termDepositRepository = termDepositRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(BookTermDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate account exists and has sufficient balance
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (account == null)
                return Result<Guid>.Failure("Account not found");

            if (account.Balance.Amount < request.PrincipalAmount)
                return Result<Guid>.Failure("Insufficient account balance");

            // Check if deposit number already exists
            if (await _termDepositRepository.ExistsAsync(request.DepositNumber, cancellationToken))
                return Result<Guid>.Failure("Deposit number already exists");

            // Create term deposit
            var termDeposit = new TermDeposit(
                request.TermDepositId,
                request.AccountId,
                request.CustomerId,
                request.DepositNumber,
                new Money(request.PrincipalAmount, Currency.FromCode(request.Currency)),
                new InterestRate(request.InterestRate),
                request.TermInMonths,
                request.InterestFrequency,
                request.AllowPartialWithdrawal,
                new Money(request.MinimumBalance, Currency.FromCode(request.Currency)),
                request.AutoRenewal,
                request.BranchCode,
                request.CreatedBy);

            // Debit the account
            account.Debit(
                new Money(request.PrincipalAmount, Currency.FromCode(request.Currency)),
                $"Term deposit booking - {request.DepositNumber}",
                request.CreatedBy);

            // Save changes
            await _termDepositRepository.AddAsync(termDeposit, cancellationToken);
            await _accountRepository.UpdateAsync(account, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(termDeposit.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to book term deposit: {ex.Message}");
        }
    }
}