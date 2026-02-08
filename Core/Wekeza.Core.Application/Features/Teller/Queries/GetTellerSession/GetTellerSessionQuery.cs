using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Queries.GetTellerSession;

public record GetTellerSessionQuery : IRequest<Result<TellerSessionDto>>
{
    public Guid SessionId { get; init; }
}

public class TellerSessionDto
{
    public Guid SessionId { get; set; }
    public string TellerId { get; set; } = string.Empty;
    public string TellerName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public decimal OpeningBalance { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class GetTellerSessionHandler : IRequestHandler<GetTellerSessionQuery, Result<TellerSessionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTellerSessionHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TellerSessionDto>> Handle(GetTellerSessionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var session = new TellerSessionDto
            {
                SessionId = request.SessionId,
                TellerId = "TELLER001",
                TellerName = "John Doe",
                BranchCode = "BR001",
                StartTime = DateTime.UtcNow,
                OpeningBalance = 10000,
                Status = "Active"
            };

            await Task.CompletedTask;
            return Result<TellerSessionDto>.Success(session);
        }
        catch (Exception ex)
        {
            return Result<TellerSessionDto>.Failure($"Failed to get teller session: {ex.Message}");
        }
    }
}
