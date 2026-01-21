using FluentValidation;
///
/// 2. The Gatekeeper: VerifyCustomerValidator.cs
/// We ensure that no verification is processed without a clear trail of who did it and why.
///
namespace Wekeza.Core.Application.Features.Accounts.Commands.VerifyCustomer;

public class VerifyCustomerValidator : AbstractValidator<VerifyCustomerCommand>
{
    public VerifyCustomerValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.VerifiedBy).NotEmpty().WithMessage("Verifier identity is required for audit.");
        RuleFor(x => x.VerificationSource).NotEmpty();
    }
}
