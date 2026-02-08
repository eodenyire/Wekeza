using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Queries.GetLoans;

public record GetLoansQuery : IRequest<Result<List<LoanDto>>>
{
    public Guid CustomerId { get; init; }
}

public class LoanDto
{
    public Guid Id { get; set; }
    public string LoanNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal InterestRate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DisbursementDate { get; set; }
    public DateTime MaturityDate { get; set; }
}

public class GetLoansHandler : IRequestHandler<GetLoansQuery, Result<List<LoanDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLoansHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LoanDto>>> Handle(GetLoansQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var loans = new List<LoanDto>();
            return Result<List<LoanDto>>.Success(loans);
        }
        catch (Exception ex)
        {
            return Result<List<LoanDto>>.Failure($"Failed to get loans: {ex.Message}");
        }
    }
}
