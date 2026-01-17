using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Deposits.Commands.OpenRecurringDeposit;

/// <summary>
/// Command to open a new Recurring Deposit
/// </summary>
public record OpenRecurringDepositCommand : IRequest<Result<Guid>>
{
    public Guid DepositId { get; init; }
    public Guid AccountId { get; init; }
    public Guid CustomerId { get; init; }
    public string DepositNumber { get; init; } = string.Empty;
    public decimal MonthlyInstallment { get; init; }
    public string Currency { get; init; } = "KES";
    public decimal InterestRate { get; init; }
    public InterestRateType InterestRateType { get; init; } = InterestRateType.Annual;
    public int TenureInMonths { get; init; }
    public bool AutoDebit { get; init; } = false;
    public Guid? AutoDebitAccountId { get; init; }
    public string BranchCode { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
}