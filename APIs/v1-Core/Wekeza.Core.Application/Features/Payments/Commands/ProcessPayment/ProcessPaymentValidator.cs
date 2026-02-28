using FluentValidation;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid payment type");

        RuleFor(x => x.Channel)
            .IsInEnum()
            .WithMessage("Invalid payment channel");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than zero")
            .LessThanOrEqualTo(10000000) // 10M limit
            .WithMessage("Payment amount exceeds maximum limit");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a valid 3-letter code");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Description is required and cannot exceed 500 characters");

        // Internal transfer validation
        When(x => x.Type == PaymentType.InternalTransfer, () =>
        {
            RuleFor(x => x.FromAccountId)
                .NotEmpty()
                .When(x => string.IsNullOrEmpty(x.FromAccountNumber))
                .WithMessage("From account ID or account number is required for internal transfer");

            RuleFor(x => x.ToAccountId)
                .NotEmpty()
                .When(x => string.IsNullOrEmpty(x.ToAccountNumber))
                .WithMessage("To account ID or account number is required for internal transfer");

            RuleFor(x => x.FromAccountId)
                .NotEqual(x => x.ToAccountId)
                .When(x => x.FromAccountId.HasValue && x.ToAccountId.HasValue)
                .WithMessage("Cannot transfer to the same account");
        });

        // External payment validation
        When(x => x.Type == PaymentType.ExternalTransfer, () =>
        {
            RuleFor(x => x.FromAccountId)
                .NotEmpty()
                .When(x => string.IsNullOrEmpty(x.FromAccountNumber))
                .WithMessage("From account ID or account number is required for external payment");

            RuleFor(x => x.BeneficiaryName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Beneficiary name is required and cannot exceed 100 characters");

            RuleFor(x => x.BeneficiaryAccountNumber)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Beneficiary account number is required and cannot exceed 50 characters");

            RuleFor(x => x.BeneficiaryBank)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Beneficiary bank is required and cannot exceed 100 characters");

            RuleFor(x => x.BeneficiaryBankCode)
                .MaximumLength(20)
                .When(x => !string.IsNullOrEmpty(x.BeneficiaryBankCode))
                .WithMessage("Beneficiary bank code cannot exceed 20 characters");
        });

        RuleFor(x => x.CustomerReference)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.CustomerReference))
            .WithMessage("Customer reference cannot exceed 100 characters");

        RuleFor(x => x.ValueDate)
            .GreaterThanOrEqualTo(DateTime.Today.AddDays(-1))
            .When(x => x.ValueDate.HasValue)
            .WithMessage("Value date cannot be more than 1 day in the past");

        RuleFor(x => x.FeeBearer)
            .IsInEnum()
            .WithMessage("Invalid fee bearer");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid payment priority");
    }
}