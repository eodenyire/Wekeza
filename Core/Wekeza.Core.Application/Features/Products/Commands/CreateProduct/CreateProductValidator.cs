using FluentValidation;

namespace Wekeza.Core.Application.Features.Products.Commands.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.ProductCode)
            .NotEmpty().WithMessage("Product code is required")
            .MaximumLength(20).WithMessage("Product code must not exceed 20 characters")
            .Matches("^[A-Z0-9]+$").WithMessage("Product code must contain only uppercase letters and numbers");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid product category");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid product type");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters (ISO code)");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        // Interest configuration validation
        When(x => x.InterestConfig != null, () =>
        {
            RuleFor(x => x.InterestConfig!.Rate)
                .GreaterThanOrEqualTo(0).WithMessage("Interest rate must be non-negative")
                .LessThanOrEqualTo(100).WithMessage("Interest rate must not exceed 100%");

            RuleFor(x => x.InterestConfig!.Type)
                .IsInEnum().WithMessage("Invalid interest type");

            RuleFor(x => x.InterestConfig!.CalculationMethod)
                .IsInEnum().WithMessage("Invalid calculation method");

            RuleFor(x => x.InterestConfig!.PostingFrequency)
                .IsInEnum().WithMessage("Invalid posting frequency");
        });

        // Fee configuration validation
        When(x => x.Fees != null && x.Fees.Any(), () =>
        {
            RuleForEach(x => x.Fees).ChildRules(fee =>
            {
                fee.RuleFor(f => f.FeeCode)
                    .NotEmpty().WithMessage("Fee code is required");

                fee.RuleFor(f => f.FeeName)
                    .NotEmpty().WithMessage("Fee name is required");

                fee.RuleFor(f => f.Amount)
                    .GreaterThanOrEqualTo(0).WithMessage("Fee amount must be non-negative");
            });
        });

        // Limit configuration validation
        When(x => x.Limits != null, () =>
        {
            RuleFor(x => x.Limits!.MinBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum balance must be non-negative")
                .When(x => x.Limits!.MinBalance.HasValue);

            RuleFor(x => x.Limits!.MaxBalance)
                .GreaterThan(x => x.Limits!.MinBalance ?? 0).WithMessage("Maximum balance must be greater than minimum balance")
                .When(x => x.Limits!.MaxBalance.HasValue && x.Limits!.MinBalance.HasValue);
        });
    }
}
