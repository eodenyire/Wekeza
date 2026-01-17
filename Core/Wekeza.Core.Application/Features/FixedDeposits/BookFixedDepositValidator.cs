using FluentValidation;

namespace Wekeza.Core.Application.Features.FixedDeposits.Commands.BookFixedDeposit;
///
///2. BookFixedDepositValidator.cs (The Yield Guard)
///We ensure the term is valid and the principal meets the minimum requirement for an investment account (e.g., 50,000 KES).
///
public class BookFixedDepositValidator : AbstractValidator<BookFixedDepositCommand>
{
    public BookFixedDepositValidator()
    {
        RuleFor(x => x.SourceAccountNumber).NotEmpty();
        RuleFor(x => x.PrincipalAmount).GreaterThanOrEqualTo(50000)
            .WithMessage("Minimum principal for a Fixed Deposit is 50,000 KES.");
        
        RuleFor(x => x.TermInMonths).Must(x => new[] { 3, 6, 12, 24 }.Contains(x))
            .WithMessage("Invalid term. Please choose 3, 6, 12, or 24 months.");

        RuleFor(x => x.AgreedInterestRate).InclusiveBetween(5, 15)
            .WithMessage("Interest rate must be between 5% and 15% APR.");
    }
}
