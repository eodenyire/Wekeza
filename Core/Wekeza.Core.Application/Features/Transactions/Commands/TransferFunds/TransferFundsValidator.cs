using FluentValidation;

namespace Wekeza.Core.Application.Features.Transactions.Commands.TransferFunds;
///<summary>
/// 2. The Business Guard: TransferFundsValidator.cs
// We implement strict validation. A bank must never allow a transfer to the same account or a zero/negative amount.
///</summary>
public class TransferFundsValidator : AbstractValidator<TransferFundsCommand>
{
    public TransferFundsValidator()
    {
        RuleFor(x => x.FromAccountNumber).NotEmpty().NotEqual(x => x.ToAccountNumber)
            .WithMessage("Source and Destination accounts must be different.");
            
        RuleFor(x => x.ToAccountNumber).NotEmpty();
        
        RuleFor(x => x.Amount).GreaterThan(0)
            .WithMessage("Transfer amount must be greater than zero.");
            
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}
