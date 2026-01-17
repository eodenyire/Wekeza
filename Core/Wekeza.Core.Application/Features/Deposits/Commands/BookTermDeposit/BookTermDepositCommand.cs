using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Deposits.Commands.BookTermDeposit;

/// <summary>
/// Command to book a new term deposit
/// </summary>
public record BookTermDepositCommand : IRequest<Result<Guid>>
{
    public Guid TermDepositId { get; init; }
    public Guid AccountId { get; init; }
    public Guid CustomerId { get; init; }
    public string DepositNumber { get; init; } = string.Empty;
    public decimal PrincipalAmount { get; init; }
    public string Currency { get; init; } = "KES";
    public decimal InterestRate { get; init; }
    public int TermInMonths { get; init; }
    public InterestPaymentFrequency InterestFrequency { get; init; }
    public bool AllowPartialWithdrawal { get; init; }
    public decimal MinimumBalance { get; init; }
    public bool AutoRenewal { get; init; }
    public string BranchCode { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
}