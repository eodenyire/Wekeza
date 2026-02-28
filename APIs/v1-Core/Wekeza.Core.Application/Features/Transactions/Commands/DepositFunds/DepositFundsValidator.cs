using FluentValidation;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Transactions.Commands.DepositFunds;
///
/// 2. The Gatekeeper: DepositFundsValidator.cs
/// We implement "The Beast" level checks. No zero deposits, and we validate the reference length for mobile money to ensure data quality.
///
public class DepositFundsValidator : AbstractValidator<DepositFundsCommand>
{
    public DepositFundsValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Deposit amount must be positive.");
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Channel).Must(x => new[] { "Cash", "M-Pesa", "Swift", "Internal" }.Contains(x));
        
        // Ensure reference is provided for non-cash deposits to prevent reconciliation nightmares
        When(x => x.Channel != "Cash", () => {
            RuleFor(x => x.ExternalReference).NotEmpty().WithMessage("External reference is required for digital deposits.");
        });
    }
}
