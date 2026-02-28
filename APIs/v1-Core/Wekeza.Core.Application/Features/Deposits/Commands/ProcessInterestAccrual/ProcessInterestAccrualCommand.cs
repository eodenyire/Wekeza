using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Deposits.Commands.ProcessInterestAccrual;

/// <summary>
/// Command to process interest accrual for all eligible accounts
/// </summary>
public record ProcessInterestAccrualCommand : IRequest<Result<Guid>>
{
    public Guid AccrualEngineId { get; init; }
    public string EngineName { get; init; } = "Daily Interest Accrual";
    public DateTime ProcessingDate { get; init; } = DateTime.UtcNow.Date;
    public AccountType? AccountTypeFilter { get; init; } // Optional filter for specific account types
    public string? BranchCodeFilter { get; init; } // Optional filter for specific branch
    public InterestCalculationMethod CalculationMethod { get; init; } = InterestCalculationMethod.SimpleInterest;
    public string ProcessedBy { get; init; } = string.Empty;
    public bool DryRun { get; init; } = false; // For testing without actual posting
}