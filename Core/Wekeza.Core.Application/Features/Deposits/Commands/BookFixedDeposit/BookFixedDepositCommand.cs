using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Deposits.Commands.BookFixedDeposit;

/// <summary>
/// Command to book a new Fixed Deposit
/// </summary>
public record BookFixedDepositCommand : IRequest<Result<Guid>>
{
    public Guid DepositId { get; init; }
    public Guid AccountId { get; init; }
    public Guid CustomerId { get; init; }
    public string DepositNumber { get; init; } = string.Empty;
    public decimal PrincipalAmount { get; init; }
    public string Currency { get; init; } = "KES";
    public decimal InterestRate { get; init; }
    public InterestRateType InterestRateType { get; init; } = InterestRateType.Annual;
    public int TermInDays { get; init; }
    public InterestPaymentFrequency InterestFrequency { get; init; } = InterestPaymentFrequency.OnMaturity;
    public bool AutoRenewal { get; init; } = false;
    public string? RenewalInstructions { get; init; }
    public string BranchCode { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
}