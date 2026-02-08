using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.FixedDeposits;

///
///3. BookFixedDepositHandler.cs (The Financial Engineer)
///This handler performs the "Sweep." It debits the source account and creates the Fixed Deposit entity with a maturity date.
///

public record BookFixedDepositCommand(
    string SourceAccountNumber,
    decimal PrincipalAmount,
    int TermInDays,
    decimal InterestRate
) : IRequest<Guid>;

public class BookFixedDepositHandler : IRequestHandler<BookFixedDepositCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IFixedDepositRepository _fdRepository; // New port
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Guid> Handle(BookFixedDepositCommand request, CancellationToken ct)
    {
        // 1. Fetch Source Account
        var sourceAccount = await _accountRepository.GetByAccountNumberAsync(
            new AccountNumber(request.SourceAccountNumber), ct)
            ?? throw new NotFoundException("Account", request.SourceAccountNumber);

        // 2. Validate Funds Availability
        var amount = new Money(request.PrincipalAmount, sourceAccount.Balance.Currency);
        sourceAccount.Debit(amount, $"FD-{Guid.NewGuid()}", "Fixed Deposit booking"); // This will throw InsufficientFunds if they don't have it

        // 3. Create Fixed Deposit Aggregate
        var depositNumber = $"FD-{Guid.NewGuid().ToString()[..8]}";
        var interestRate = new InterestRate(request.InterestRate);
        
        // This represents a new 'Account' type in the system
        var fdAccount = new FixedDeposit(
            Guid.NewGuid(),
            sourceAccount.Id,
            sourceAccount.CustomerId,
            depositNumber,
            amount,
            interestRate,
            request.TermInDays,
            InterestPaymentFrequency.OnMaturity,
            false, // autoRenewal
            "HQ", // branchCode
            "SYSTEM" // createdBy
        );

        await _fdRepository.AddAsync(fdAccount, ct);
        
        // 4. Update the source account state
        _accountRepository.Update(sourceAccount);

        // TransactionBehavior handles the SaveChanges atomicity!
        return fdAccount.Id;
    }
}
