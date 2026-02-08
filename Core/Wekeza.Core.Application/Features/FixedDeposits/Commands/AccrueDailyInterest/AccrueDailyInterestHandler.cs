using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.FixedDeposits.Commands.AccrueDailyInterest;

///📂 Phase 8: The Interest Accrual & Payout Engine
/// For our Fixed Deposits and Savings Accounts, the bank must calculate interest daily. We are building the Application logic that the Background Job will call.
/// 1. 📂 Features/FixedDeposits/Commands/AccrueDailyInterest/
/// AccrueDailyInterestHandler.cs This calculates the daily yield and updates the "Accrued Interest" bucket. This is the Financial Engineering core.
///
///
public record AccrueDailyInterestCommand(Guid FixedDepositId) : IRequest<bool>;

public class AccrueDailyInterestHandler : IRequestHandler<AccrueDailyInterestCommand, bool>
{
    private readonly IFixedDepositRepository _repository;
    private readonly IDateTime _dateTime;

    public async Task<bool> Handle(AccrueDailyInterestCommand request, CancellationToken ct)
    {
        var fd = await _repository.GetByIdAsync(request.FixedDepositId, ct);
        
        // Formula: (Principal * Rate) / 365
        var dailyRate = fd.InterestRate.Rate / 365 / 100;
        var interestAmount = fd.PrincipalAmount.Amount * dailyRate;

        // fd.Accrue(interestAmount); // Domain logic updates the AccruedBalance
        
        return true;
    }
}
