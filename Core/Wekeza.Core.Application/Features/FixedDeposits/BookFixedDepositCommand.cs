using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.FixedDeposits.Commands.BookFixedDeposit;
///
///ðŸ“‚ Wekeza.Core.Application/Features/FixedDeposits
///1. BookFixedDepositCommand.cs (The Investment Intent)
///This command moves money from a "Current Account" into a new "Fixed Deposit" sub-account.
///
public record BookFixedDepositCommand : ICommand<Guid>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string SourceAccountNumber { get; init; } = string.Empty;
    public decimal PrincipalAmount { get; init; }
    public int TermInMonths { get; init; } // e.g., 3, 6, 12
    public int TermInDays => TermInMonths * 30; // Approximate conversion
    public decimal InterestRate { get; init; } // The APR (alias for AgreedInterestRate)
    public decimal AgreedInterestRate { get; init; } // The APR
}
