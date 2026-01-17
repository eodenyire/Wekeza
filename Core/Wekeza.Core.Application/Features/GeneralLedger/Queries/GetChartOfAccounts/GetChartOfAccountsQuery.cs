using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.GeneralLedger.Queries.GetChartOfAccounts;

[Authorize(UserRole.Teller, UserRole.RiskOfficer, UserRole.Administrator)]
public record GetChartOfAccountsQuery : IQuery<List<GLAccountDto>>;

public record GLAccountDto
{
    public string GLCode { get; init; } = string.Empty;
    public string GLName { get; init; } = string.Empty;
    public string AccountType { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int Level { get; init; }
    public bool IsLeaf { get; init; }
    public string? ParentGLCode { get; init; }
    public decimal DebitBalance { get; init; }
    public decimal CreditBalance { get; init; }
    public decimal NetBalance { get; init; }
    public string Currency { get; init; } = string.Empty;
}
