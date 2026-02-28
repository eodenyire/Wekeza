using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;

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
        var transactionReference = Guid.NewGuid().ToString();
        sourceAccount.Debit(amount, transactionReference); // This will throw InsufficientFunds if they don't have it

        // 3. Create Fixed Deposit Aggregate
        // We calculate maturity date: Today + Term
        var maturityDate = DateTime.UtcNow.AddDays(request.TermInDays);
        var depositNumber = $"FD-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        
        // This represents a new 'Account' type in the system
        var fdAccount = new Domain.Aggregates.FixedDeposit(
            Guid.NewGuid(),
            sourceAccount.Id,
            sourceAccount.CustomerId,
            depositNumber,
            amount,
            new InterestRate(request.InterestRate),
            request.TermInDays,
            Domain.Enums.InterestPaymentFrequency.OnMaturity,
            false,
            "HEAD",
            "System"
        );

        await _fdRepository.AddAsync(fdAccount);
        
        // 4. Update the source account state
        _accountRepository.Update(sourceAccount);

        // TransactionBehavior handles the SaveChanges atomicity!
        return fdAccount.Id;
    }
}
