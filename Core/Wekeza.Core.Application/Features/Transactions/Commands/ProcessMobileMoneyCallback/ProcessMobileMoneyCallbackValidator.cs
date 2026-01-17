using FluentValidation;

namespace Wekeza.Core.Application.Features.Transactions.Commands.ProcessMobileMoneyCallback;

public class ProcessMobileMoneyCallbackValidator : AbstractValidator<ProcessMobileMoneyCallbackCommand>
{
    public ProcessMobileMoneyCallbackValidator()
    {
        RuleFor(x => x.CheckoutRequestID)
            .NotEmpty()
            .WithMessage("Checkout request ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.")
            .When(x => x.ResultCode == 0); // Only validate amount if payment was successful

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^254[0-9]{9}$")
            .WithMessage("Phone number must be in format 254XXXXXXXXX.");

        RuleFor(x => x.MpesaReceiptNumber)
            .NotEmpty()
            .WithMessage("M-Pesa receipt number is required.")
            .When(x => x.ResultCode == 0);
    }
}
