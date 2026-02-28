using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Deposits.Commands.OpenCallDeposit;

/// <summary>
/// Command to open a new call deposit account
/// </summary>
public record OpenCallDepositCommand : IRequest<Result<Guid>>
{
    public Guid CallDepositId { get; init; }
    public Guid AccountId { get; init; }
    public Guid CustomerId { get; init; }
    public string DepositNumber { get; init; } = string.Empty;
    public decimal InitialDeposit { get; init; }
    public string Currency { get; init; } = "KES";
    public decimal InterestRate { get; init; }
    public int NoticePeriodDays { get; init; }
    public decimal MinimumBalance { get; init; }
    public decimal MaximumBalance { get; init; }
    public InterestPaymentFrequency InterestFrequency { get; init; }
    public bool InstantAccess { get; init; }
    public string BranchCode { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
}