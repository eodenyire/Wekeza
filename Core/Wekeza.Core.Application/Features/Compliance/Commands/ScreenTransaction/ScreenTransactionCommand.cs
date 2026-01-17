using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Compliance.Commands.ScreenTransaction;

public record ScreenTransactionCommand : ICommand<ScreenTransactionResponse>
{
    public Guid TransactionId { get; init; }
    public List<string> MonitoringRules { get; init; } = new();
    public List<string> WatchlistsToScreen { get; init; } = new();
    public bool EnableFraudDetection { get; init; } = true;
    public bool EnableAMLMonitoring { get; init; } = true;
    public bool EnableSanctionsScreening { get; init; } = true;
    public string ScreenedBy { get; init; } = "SYSTEM";
}

public record ScreenTransactionResponse
{
    public Guid TransactionId { get; init; }
    public ScreeningResult OverallResult { get; init; }
    public AlertSeverity HighestSeverity { get; init; }
    public TransactionMonitoringResult? MonitoringResult { get; init; }
    public SanctionsScreeningResult? SanctionsResult { get; init; }
    public FraudScreeningResult? FraudResult { get; init; }
    public bool RequiresReview { get; init; }
    public bool IsBlocked { get; init; }
    public DateTime ScreeningCompletedAt { get; init; }
}

public record TransactionMonitoringResult
{
    public Guid MonitoringId { get; init; }
    public ScreeningResult Result { get; init; }
    public AlertSeverity Severity { get; init; }
    public List<string> TriggeredRules { get; init; } = new();
    public decimal? RiskScore { get; init; }
}

public record SanctionsScreeningResult
{
    public Guid ScreeningId { get; init; }
    public ScreeningStatus Status { get; init; }
    public int MatchCount { get; init; }
    public decimal HighestMatchScore { get; init; }
    public List<string> MatchedWatchlists { get; init; } = new();
}

public record FraudScreeningResult
{
    public Guid? AlertId { get; init; }
    public bool FraudDetected { get; init; }
    public string? FraudType { get; init; }
    public decimal? RiskScore { get; init; }
    public List<string> TriggeredRules { get; init; } = new();
}