using FluentValidation;

namespace Wekeza.Core.Application.Features.Accounts.Commands.OpenAccount;
/// <summary> 
/// 📂 Wekeza.Core.Application/Features/Accounts
/// We are starting with the OpenAccount vertical slice. This is our first end-to-end mission.
/// 2. The Business Guard: Commands/OpenAccount/OpenAccountValidator.cs
/// This is where our ValidationBehavior gets its instructions. We use FluentValidation to ensure Wekeza Bank never accepts "junk" data.
/// </summary>
public class OpenAccountValidator : AbstractValidator<OpenAccountCommand>
{
    public OpenAccountValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
        RuleFor(v => v.IdentificationNumber).NotEmpty().MinimumLength(6);
        RuleFor(v => v.CurrencyCode).NotEmpty().Length(3);
        
        // Billion-dollar rule: You can't open an account with negative money!
        RuleFor(v => v.InitialDeposit).GreaterThanOrEqualTo(0)
            .WithMessage("Initial deposit cannot be negative.");
    }
}
