using MediatR;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.GeneralLedger.Queries.GetChartOfAccounts;

public class GetChartOfAccountsHandler : IRequestHandler<GetChartOfAccountsQuery, List<GLAccountDto>>
{
    private readonly IGLAccountRepository _glAccountRepository;

    public GetChartOfAccountsHandler(IGLAccountRepository glAccountRepository)
    {
        _glAccountRepository = glAccountRepository;
    }

    public async Task<List<GLAccountDto>> Handle(GetChartOfAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _glAccountRepository.GetChartOfAccountsAsync();

        return accounts.Select(a => new GLAccountDto
        {
            GLCode = a.GLCode,
            GLName = a.GLName,
            AccountType = a.AccountType.ToString(),
            Category = a.Category.ToString(),
            Status = a.Status.ToString(),
            Level = a.Level,
            IsLeaf = a.IsLeaf,
            ParentGLCode = a.ParentGLCode,
            DebitBalance = a.DebitBalance,
            CreditBalance = a.CreditBalance,
            NetBalance = a.NetBalance,
            Currency = a.Currency
        }).ToList();
    }
}
