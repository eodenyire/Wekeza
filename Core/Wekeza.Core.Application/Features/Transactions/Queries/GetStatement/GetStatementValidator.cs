using FluentValidation;

namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;
///<summary>
///3. GetStatementValidator.cs (The Guard)
/// We ensure the user isn't asking for a 10-year statement in one go, which could be a Denial of Service (DoS) risk.
///</summary>
public class GetStatementValidator : AbstractValidator<GetStatementQuery>
{
    public GetStatementValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty();
        RuleFor(x => x.FromDate).LessThanOrEqualTo(x => x.ToDate)
            .WithMessage("Start date cannot be after the end date.");
            
        // Limit the range to 1 year for performance
        RuleFor(x => x.FromDate)
            .Must((query, fromDate) => (query.ToDate - fromDate).TotalDays <= 366)
            .WithMessage("Statement range cannot exceed 12 months.");

        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
