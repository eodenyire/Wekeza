using MediatR;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.GeneralLedger.Queries.GetTrialBalance;

public class GetTrialBalanceHandler : IRequestHandler<GetTrialBalanceQuery, TrialBalanceDto>
{
    private readonly IGLAccountRepository _glAccountRepository;

    public GetTrialBalanceHandler(IGLAccountRepository glAccountRepository)
    {
        _glAccountRepository = glAccountRepository;
    }

    public async Task<TrialBalanceDto> Handle(GetTrialBalanceQuery request, CancellationToken cancellationToken)
    {
        // Get all leaf accounts (only leaf accounts have balances)
        var accounts = await _glAccountRepository.GetLeafAccountsAsync();

        var lines = accounts.Select(a => new TrialBalanceLineDto
        {
            GLCode = a.GLCode,
            GLName = a.GLName,
            AccountType = a.AccountType.ToString(),
            DebitBalance = a.DebitBalance,
            CreditBalance = a.CreditBalance
        }).OrderBy(l => l.GLCode).ToList();

        return new TrialBalanceDto
        {
            AsOfDate = request.AsOfDate,
            Lines = lines,
            TotalDebit = lines.Sum(l => l.DebitBalance),
            TotalCredit = lines.Sum(l => l.CreditBalance)
        };
    }
}
