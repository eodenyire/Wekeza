using FluentValidation;

namespace Wekeza.Core.Application.Features.Loans.Commands.ApplyForLoan;

/// <summary>
/// Apply for Loan Validator - Validates loan application requests
/// Ensures all required fields and business rules are met
/// </summary>
public class ApplyForLoanValidator : AbstractValidator<ApplyForLoanCommand>
{
    public ApplyForLoanValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Loan amount must be greater than zero")
            .LessThanOrEqualTo(10000000)
            .WithMessage("Loan amount cannot exceed 10 million");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be 3 characters (ISO code)");

        RuleFor(x => x.TermInMonths)
            .GreaterThan(0)
            .WithMessage("Loan term must be greater than zero")
            .LessThanOrEqualTo(360)
            .WithMessage("Loan term cannot exceed 30 years (360 months)");

        RuleFor(x => x.Purpose)
            .NotEmpty()
            .WithMessage("Loan purpose is required")
            .MaximumLength(500)
            .WithMessage("Loan purpose cannot exceed 500 characters");

        RuleFor(x => x.PreferredDisbursementDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .When(x => x.PreferredDisbursementDate.HasValue)
            .WithMessage("Preferred disbursement date cannot be in the past");

        // Validate collaterals if provided
        RuleForEach(x => x.Collaterals)
            .SetValidator(new LoanCollateralValidator())
            .When(x => x.Collaterals != null);

        // Validate guarantors if provided
        RuleForEach(x => x.Guarantors)
            .SetValidator(new LoanGuarantorValidator())
            .When(x => x.Guarantors != null);
    }
}

public class LoanCollateralValidator : AbstractValidator<LoanCollateralDto>
{
    public LoanCollateralValidator()
    {
        RuleFor(x => x.CollateralType)
            .NotEmpty()
            .WithMessage("Collateral type is required")
            .MaximumLength(100)
            .WithMessage("Collateral type cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Collateral description is required")
            .MaximumLength(500)
            .WithMessage("Collateral description cannot exceed 500 characters");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Collateral value must be greater than zero");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Collateral currency is required")
            .Length(3)
            .WithMessage("Currency must be 3 characters (ISO code)");

        RuleFor(x => x.ValuationDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Valuation date cannot be in the future")
            .GreaterThan(DateTime.Today.AddYears(-5))
            .WithMessage("Valuation date cannot be more than 5 years old");
    }
}

public class LoanGuarantorValidator : AbstractValidator<LoanGuarantorDto>
{
    public LoanGuarantorValidator()
    {
        RuleFor(x => x.GuarantorId)
            .NotEmpty()
            .WithMessage("Guarantor ID is required");

        RuleFor(x => x.GuarantorName)
            .NotEmpty()
            .WithMessage("Guarantor name is required")
            .MaximumLength(200)
            .WithMessage("Guarantor name cannot exceed 200 characters");

        RuleFor(x => x.GuaranteeAmount)
            .GreaterThan(0)
            .WithMessage("Guarantee amount must be greater than zero");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Guarantee currency is required")
            .Length(3)
            .WithMessage("Currency must be 3 characters (ISO code)");
    }
}