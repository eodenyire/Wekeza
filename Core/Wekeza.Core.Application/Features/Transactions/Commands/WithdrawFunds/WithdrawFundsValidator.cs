using FluentValidation;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Transactions.Commands.WithdrawFunds;
/// 2. The Gatekeeper: WithdrawFundsValidator.cs
/// We prevent "The Beast" from being tricked by small amounts or invalid terminals.
public class WithdrawFundsValidator : AbstractValidator<WithdrawFundsCommand>
{
    public WithdrawFundsValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Withdrawal amount must be positive.");
        RuleFor(x => x.Channel).Must(x => new[] { "ATM", "Branch", "Agent" }.Contains(x));
        RuleFor(x => x.TerminalId).NotEmpty().WithMessage("Terminal/Branch ID is required for audit.");
    }
}
