using FluentValidation;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Accounts.Commands.FreezeAccount;
/// 2. The Gatekeeper: FreezeAccountValidator.cs
/// We ensure that no one can freeze an account without a valid reason and proper authorization.

public class FreezeAccountValidator : AbstractValidator<FreezeAccountCommand>
{
    public FreezeAccountValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account Number is required to initiate a freeze.");
            
        RuleFor(x => x.Reason)
            .NotEmpty().MinimumLength(10).WithMessage("A detailed reason (min 10 chars) is required for audit purposes.");
            
        RuleFor(x => x.AuthorizedBy)
            .NotEmpty().WithMessage("The authorizing entity must be identified.");
    }
}
