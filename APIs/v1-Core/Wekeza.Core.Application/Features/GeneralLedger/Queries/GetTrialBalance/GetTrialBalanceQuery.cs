using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.GeneralLedger.Queries.GetTrialBalance;

[Authorize(UserRole.Administrator)]
public record GetTrialBalanceQuery : IQuery<TrialBalanceDto>
{
    public DateTime AsOfDate { get; init; }
}

public record TrialBalanceDto
{
    public DateTime AsOfDate { get; init; }
    public List<TrialBalanceLineDto> Lines { get; init; } = new();
    public decimal TotalDebit { get; init; }
    public decimal TotalCredit { get; init; }
    public bool IsBalanced => TotalDebit == TotalCredit;
}

public record TrialBalanceLineDto
{
    public string GLCode { get; init; } = string.Empty;
    public string GLName { get; init; } = string.Empty;
    public string AccountType { get; init; } = string.Empty;
    public decimal DebitBalance { get; init; }
    public decimal CreditBalance { get; init; }
}
