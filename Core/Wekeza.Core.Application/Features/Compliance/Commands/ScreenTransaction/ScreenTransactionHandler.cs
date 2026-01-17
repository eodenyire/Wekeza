using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Features.Compliance.Commands.ScreenTransaction;

public class ScreenTransactionHandler : IRequestHandler<ScreenTransactionCommand, ScreenTransactionResponse>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransactionMonitoringRepository _monitoringRepository;
    private readonly ISanctionsScreeningRepository _sanctionsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ScreenTransactionHandler(
        ITransactionRepository transactionRepository,
        ITransactionMonitoringRepository monitoringRepository,
        ISanctionsScreeningRepository sanctionsRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _monitoringRepository = monitoringRepository;
        _sanctionsRepository = sanctionsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ScreenTransactionResponse> Handle(ScreenTransactionCommand request, CancellationToken cancellationToken)
    {
        // Validate transaction exists
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId, cancellationToken);
        if (transaction == null)
        {
            throw new NotFoundException("Transaction", request.TransactionId);
        }

        var response = new ScreenTransactionResponse
        {
            TransactionId = request.TransactionId,
            OverallResult = ScreeningResult.Clear,
            HighestSeverity = AlertSeverity.Low,
            ScreeningCompletedAt = DateTime.UtcNow
        };

        // 1. AML Transaction Monitoring
        if (request.EnableAMLMonitoring)
        {
            var monitoringResult = await PerformAMLMonitoring(request, transaction, cancellationToken);
            response.MonitoringResult = monitoringResult;
            
            UpdateOverallResult(response, monitoringResult.Result, monitoringResult.Severity);
        }

        // 2. Sanctions Screening
        if (request.EnableSanctionsScreening)
        {
            var sanctionsResult = await PerformSanctionsScreening(request, cancellationToken);
            response.SanctionsResult = sanctionsResult;
            
            var sanctionsSeverity = DetermineSanctionsSeverity(sanctionsResult.Status);
            var sanctionsScreeningResult = DetermineSanctionsResult(sanctionsResult.Status);
            UpdateOverallResult(response, sanctionsScreeningResult, sanctionsSeverity);
        }

        // 3. Fraud Detection (placeholder - would integrate with fraud detection system)
        if (request.EnableFraudDetection)
        {
            var fraudResult = await PerformFraudDetection(request, transaction, cancellationToken);
            response.FraudResult = fraudResult;
            
            if (fraudResult.FraudDetected)
            {
                UpdateOverallResult(response, ScreeningResult.Block, AlertSeverity.High);
            }
        }

        // Determine final status
        response.RequiresReview = response.OverallResult == ScreeningResult.Review || response.OverallResult == ScreeningResult.Alert;
        response.IsBlocked = response.OverallResult == ScreeningResult.Block;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }

    private async Task<TransactionMonitoringResult> PerformAMLMonitoring(
        ScreenTransactionCommand request, 
        Transaction transaction, 
        CancellationToken cancellationToken)
    {
        // Apply monitoring rules
        var appliedRules = request.MonitoringRules.Any() ? request.MonitoringRules : GetDefaultMonitoringRules(transaction);
        
        // Determine screening result based on rules
        var (result, severity) = EvaluateMonitoringRules(transaction, appliedRules);
        
        // Calculate risk score
        var riskScore = CalculateTransactionRiskScore(transaction, appliedRules);

        // Create transaction monitoring record
        var monitoring = TransactionMonitoring.Create(
            request.TransactionId,
            appliedRules,
            result,
            severity,
            riskScore);

        await _monitoringRepository.AddAsync(monitoring, cancellationToken);

        return new TransactionMonitoringResult
        {
            MonitoringId = monitoring.Id,
            Result = result,
            Severity = severity,
            TriggeredRules = appliedRules,
            RiskScore = riskScore?.Score
        };
    }

    private async Task<SanctionsScreeningResult> PerformSanctionsScreening(
        ScreenTransactionCommand request, 
        CancellationToken cancellationToken)
    {
        var watchlists = request.WatchlistsToScreen.Any() ? request.WatchlistsToScreen : GetDefaultWatchlists();
        
        // Create sanctions screening
        var screening = SanctionsScreening.Create(EntityType.Transaction, request.TransactionId, watchlists);

        // Simulate sanctions screening (in real implementation, this would call external services)
        var matches = await SimulateSanctionsScreening(request.TransactionId, watchlists);
        
        foreach (var match in matches)
        {
            screening.AddMatch(match.WatchlistName, match.MatchedName, match.Score, match.MatchType);
        }

        screening.CompleteScreening();
        await _sanctionsRepository.AddAsync(screening, cancellationToken);

        return new SanctionsScreeningResult
        {
            ScreeningId = screening.Id,
            Status = screening.Status,
            MatchCount = screening.MatchCount,
            HighestMatchScore = screening.HighestMatchScore,
            MatchedWatchlists = matches.Select(m => m.WatchlistName).Distinct().ToList()
        };
    }

    private async Task<FraudScreeningResult> PerformFraudDetection(
        ScreenTransactionCommand request, 
        Transaction transaction, 
        CancellationToken cancellationToken)
    {
        // Simulate fraud detection (in real implementation, this would use ML models)
        var fraudRules = GetFraudDetectionRules(transaction);
        var fraudDetected = EvaluateFraudRules(transaction, fraudRules);
        
        if (fraudDetected)
        {
            var riskScore = RiskScore.ForTransaction(transaction.Amount.Amount, "FRAUD_DETECTED", true);
            
            return new FraudScreeningResult
            {
                FraudDetected = true,
                FraudType = "SUSPICIOUS_PATTERN",
                RiskScore = riskScore.Score,
                TriggeredRules = fraudRules
            };
        }

        return new FraudScreeningResult
        {
            FraudDetected = false,
            TriggeredRules = new List<string>()
        };
    }

    private void UpdateOverallResult(ScreenTransactionResponse response, ScreeningResult result, AlertSeverity severity)
    {
        // Update overall result to the most restrictive
        if (result > response.OverallResult)
            response.OverallResult = result;

        // Update highest severity
        if (severity > response.HighestSeverity)
            response.HighestSeverity = severity;
    }

    private List<string> GetDefaultMonitoringRules(Transaction transaction)
    {
        var rules = new List<string> { "THRESHOLD_CHECK", "VELOCITY_CHECK" };
        
        if (transaction.Amount.Amount >= 10000)
            rules.Add("CTR_THRESHOLD");
        
        if (transaction.TransactionType.ToString().Contains("CASH"))
            rules.Add("CASH_TRANSACTION");
        
        return rules;
    }

    private List<string> GetDefaultWatchlists()
    {
        return new List<string> { "OFAC_SDN", "UN_SANCTIONS", "EU_SANCTIONS", "PEP_LIST" };
    }

    private (ScreeningResult result, AlertSeverity severity) EvaluateMonitoringRules(Transaction transaction, List<string> rules)
    {
        var severity = AlertSeverity.Low;
        var result = ScreeningResult.Clear;

        foreach (var rule in rules)
        {
            switch (rule.ToUpper())
            {
                case "CTR_THRESHOLD":
                    if (transaction.Amount.Amount >= 10000)
                    {
                        severity = AlertSeverity.Medium;
                        result = ScreeningResult.Alert;
                    }
                    break;
                case "CASH_TRANSACTION":
                    if (transaction.Amount.Amount >= 5000)
                    {
                        severity = AlertSeverity.Medium;
                        result = ScreeningResult.Review;
                    }
                    break;
                case "VELOCITY_CHECK":
                    // Simplified velocity check
                    severity = AlertSeverity.Low;
                    break;
            }
        }

        return (result, severity);
    }

    private RiskScore? CalculateTransactionRiskScore(Transaction transaction, List<string> triggeredRules)
    {
        return RiskScore.ForTransaction(
            transaction.Amount.Amount, 
            transaction.TransactionType.ToString(), 
            false);
    }

    private async Task<List<SanctionsMatch>> SimulateSanctionsScreening(Guid transactionId, List<string> watchlists)
    {
        // Simulate sanctions screening - in real implementation, this would call external APIs
        await Task.Delay(10); // Simulate API call delay
        
        // Return empty matches for simulation
        return new List<SanctionsMatch>();
    }

    private List<string> GetFraudDetectionRules(Transaction transaction)
    {
        return new List<string> { "AMOUNT_ANOMALY", "TIME_ANOMALY", "LOCATION_ANOMALY" };
    }

    private bool EvaluateFraudRules(Transaction transaction, List<string> rules)
    {
        // Simplified fraud detection - in real implementation, this would use ML models
        return false; // No fraud detected in simulation
    }

    private AlertSeverity DetermineSanctionsSeverity(ScreeningStatus status)
    {
        return status switch
        {
            ScreeningStatus.Blocked => AlertSeverity.Critical,
            ScreeningStatus.UnderReview => AlertSeverity.High,
            ScreeningStatus.Alert => AlertSeverity.Medium,
            _ => AlertSeverity.Low
        };
    }

    private ScreeningResult DetermineSanctionsResult(ScreeningStatus status)
    {
        return status switch
        {
            ScreeningStatus.Blocked => ScreeningResult.Block,
            ScreeningStatus.UnderReview => ScreeningResult.Review,
            ScreeningStatus.Alert => ScreeningResult.Alert,
            _ => ScreeningResult.Clear
        };
    }
}

public class SanctionsMatch
{
    public string WatchlistName { get; set; } = string.Empty;
    public string MatchedName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string MatchType { get; set; } = string.Empty;
}