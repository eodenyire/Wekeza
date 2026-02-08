using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Queries.GetAuditLogs;

public record GetAuditLogsQuery : IRequest<Result<List<AuditLogDto>>>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? EntityType { get; init; }
    public string? UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Changes { get; set; }
}

public class GetAuditLogsHandler : IRequestHandler<GetAuditLogsQuery, Result<List<AuditLogDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAuditLogsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var logs = new List<AuditLogDto>();
            return Result<List<AuditLogDto>>.Success(logs);
        }
        catch (Exception ex)
        {
            return Result<List<AuditLogDto>>.Failure($"Failed to get audit logs: {ex.Message}");
        }
    }
}
