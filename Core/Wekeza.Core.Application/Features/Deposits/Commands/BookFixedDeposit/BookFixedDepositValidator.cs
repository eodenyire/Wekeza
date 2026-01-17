using FluentValidation;

namespace Wekeza.Core.Application.Features.Deposits.Commands.BookFixedDeposit;

/// <summary>
/// Validator for BookFixedDepositCommand
/// </summary>
public class BookFixedDepositValidator : AbstractValidator<BookFixedDepositCommand>
{
    public BookFixedDepositValidator()
    {
        RuleFor(x => x.DepositId)
            .NotEmpty()
            .WithMessage("Deposit ID is required");

        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.DepositNumber)
            .NotEmpty()
            .WithMessage("Deposit number is required")
            .MaximumLength(20)
            .WithMessage("Deposit number cannot exceed 20 characters")
            .Matches("^[A-Z0-9]+$")
            .WithMessage("Deposit number must contain only uppercase letters and numbers");

        RuleFor(x => x.PrincipalAmount)
            .GreaterThan(0)
            .WithMessage("Principal amount must be greater than zero")
            .LessThanOrEqualTo(10_000_000)
            .WithMessage("Principal amount cannot exceed 10,000,000");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be 3 characters")
            .Must(BeValidCurrency)
            .WithMessage("Invalid currency code");

        RuleFor(x => x.InterestRate)
            .GreaterThan(0)
            .WithMessage("Interest rate must be greater than zero")
            .LessThanOrEqualTo(50)
            .WithMessage("Interest rate cannot exceed 50%");

        RuleFor(x => x.TermInDays)
            .GreaterThanOrEqualTo(7)
            .WithMessage("Term must be at least 7 days")
            .LessThanOrEqualTo(3650)
            .WithMessage("Term cannot exceed 10 years (3650 days)");

        RuleFor(x => x.BranchCode)
            .NotEmpty()
            .WithMessage("Branch code is required")
            .Length(3)
            .WithMessage("Branch code must be 3 characters");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("Created by is required")
            .MaximumLength(100)
            .WithMessage("Created by cannot exceed 100 characters");

        RuleFor(x => x.RenewalInstructions)
            .MaximumLength(500)
            .WithMessage("Renewal instructions cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.RenewalInstructions));

        // Business rules
        RuleFor(x => x)
            .Must(HaveValidTermForInterestFrequency)
            .WithMessage("Term must be compatible with interest payment frequency")
            .WithName("TermAndFrequency");
    }

    private bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[] { "KES", "USD", "EUR", "GBP", "UGX", "TZS" };
        return validCurrencies.Contains(currency.ToUpper());
    }

    private bool HaveValidTermForInterestFrequency(BookFixedDepositCommand command)
    {
        return command.InterestFrequency switch
        {
            Domain.Enums.InterestPaymentFrequency.Monthly => command.TermInDays >= 30,
            Domain.Enums.InterestPaymentFrequency.Quarterly => command.TermInDays >= 90,
            Domain.Enums.InterestPaymentFrequency.HalfYearly => command.TermInDays >= 180,
            Domain.Enums.InterestPaymentFrequency.Yearly => command.TermInDays >= 365,
            Domain.Enums.InterestPaymentFrequency.OnMaturity => command.TermInDays >= 7,
            _ => true
        };
    }
}