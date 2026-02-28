using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Queries.SearchCustomers;

public record SearchCustomersQuery : IRequest<Result<List<CustomerSearchResultDto>>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string SearchTerm { get; init; } = string.Empty;
    public string SearchType { get; init; } = "name";
}

public class CustomerSearchResultDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class SearchCustomersHandler : IRequestHandler<SearchCustomersQuery, Result<List<CustomerSearchResultDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchCustomersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<CustomerSearchResultDto>>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Implement actual customer search logic
            var results = new List<CustomerSearchResultDto>();
            
            await Task.CompletedTask;
            return Result<List<CustomerSearchResultDto>>.Success(results);
        }
        catch (Exception ex)
        {
            return Result<List<CustomerSearchResultDto>>.Failure($"Failed to search customers: {ex.Message}");
        }
    }
}
