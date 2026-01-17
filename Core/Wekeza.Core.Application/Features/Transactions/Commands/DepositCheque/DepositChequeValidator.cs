using FluentValidation;

namespace Wekeza.Core.Application.Features.Transactions.Commands.DepositCheque;
///2. The Gatekeeper: DepositChequeValidator.cs
/// We ensure the cheque number is a valid 6-digit string and the amount is non-zero.
public class DepositChequeValidator : AbstractValidator<DepositChequeCommand>
{
    public DepositChequeValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.ChequeNumber).NotEmpty().Length(6);
        RuleFor(x => x.DrawerBank).NotEmpty();
    }
}
