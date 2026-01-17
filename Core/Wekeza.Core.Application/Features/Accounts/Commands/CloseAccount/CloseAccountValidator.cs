using FluentValidation;
/// 2. The Gatekeeper: CloseAccountValidator.cs
/// Basic structure check. The real heavy lifting happens in the Handler's business logic.
///
namespace Wekeza.Core.Application.Features.Accounts.Commands.CloseAccount;

public class CloseAccountValidator : AbstractValidator<CloseAccountCommand>
{
    public CloseAccountValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty();
        RuleFor(x => x.ClosureReason).NotEmpty().MinimumLength(5);
    }
}
