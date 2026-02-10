using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Queries.GetSystemParameters;

public record GetSystemParametersQuery : IRequest<Result<List<SystemParameterDto>>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}

public class SystemParameterDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
}

public class GetSystemParametersHandler : IRequestHandler<GetSystemParametersQuery, Result<List<SystemParameterDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSystemParametersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<SystemParameterDto>>> Handle(GetSystemParametersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Implement actual system parameters retrieval
            var parameters = new List<SystemParameterDto>();
            
            await Task.CompletedTask;
            return Result<List<SystemParameterDto>>.Success(parameters);
        }
        catch (Exception ex)
        {
            return Result<List<SystemParameterDto>>.Failure($"Failed to get system parameters: {ex.Message}");
        }
    }
}
