using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Domain.Enums;
using Wekeza.Nexus.Domain.Interfaces;
using Wekeza.Nexus.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Wekeza.Nexus.Application.Services;

/// <summary>
/// Client wrapper for integrating Wekeza Nexus into existing payment flows
/// This is the "Hook" mentioned in the problem statement
/// 
/// Usage in existing TransferFundsHandler or PaymentProcessingService:
/// 
/// var verdict = await _nexusClient.EvaluateTransactionAsync(
///     userId: user.Id,
///     amount: request.Amount,
///     beneficiary: request.ToAccountNumber,
///     telemetry: deviceAndBehavioralData
/// );
/// 
/// if (verdict.Decision == FraudDecision.Block)
///     throw new FraudDetectedException(verdict.Explanation);
/// </summary>
public class WekezaNexusClient
{
    private readonly IFraudEvaluationService _fraudService;
    private readonly ITransactionVelocityService _velocityService;
    private readonly ITransactionHistoryRepository _historyRepository;
    private readonly ILogger<WekezaNexusClient> _logger;
    
    public WekezaNexusClient(
        IFraudEvaluationService fraudService,
        ITransactionVelocityService velocityService,
        ITransactionHistoryRepository historyRepository,
        ILogger<WekezaNexusClient> logger)
    {
        _fraudService = fraudService;
        _velocityService = velocityService;
        _historyRepository = historyRepository;
        _logger = logger;
    }
    
    /// <summary>
    /// Main evaluation method called from payment controllers
    /// </summary>
    public async Task<NexusVerdict> EvaluateTransactionAsync(
        Guid userId,
        string fromAccountNumber,
        string toAccountNumber,
        decimal amount,
        string currency,
        string transactionType = "Transfer",
        string description = "",
        DeviceFingerprint? deviceInfo = null,
        BehavioralMetrics? behavioralData = null,
        string channel = "Web",
        string sessionId = "",
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Wekeza Nexus evaluating {TransactionType} of {Amount} {Currency} from {FromAccount} to {ToAccount} for user {UserId}",
                transactionType, amount, currency, fromAccountNumber, toAccountNumber, userId);
            
            // Build transaction context
            var context = await BuildTransactionContext(
                userId,
                fromAccountNumber,
                toAccountNumber,
                amount,
                currency,
                transactionType,
                description,
                deviceInfo,
                behavioralData,
                channel,
                sessionId,
                cancellationToken);
            
            // Evaluate through Nexus
            var fraudScore = await _fraudService.EvaluateAsync(context, cancellationToken);
            
            // Record transaction in history for future velocity analysis
            // Only record if transaction is allowed or challenged (not blocked)
            if (fraudScore.Decision == FraudDecision.Allow || 
                fraudScore.Decision == FraudDecision.Challenge)
            {
                await RecordTransactionAsync(
                    userId, fromAccountNumber, toAccountNumber, amount, 
                    currency, transactionType, fraudScore.Decision.ToString(),
                    cancellationToken);
            }
            
            _logger.LogInformation(
                "Wekeza Nexus verdict: {Decision} (Score: {Score}, Risk: {RiskLevel}) for context {ContextId}",
                fraudScore.Decision, fraudScore.Score, fraudScore.RiskLevel, context.Id);
            
            return new NexusVerdict
            {
                Decision = fraudScore.Decision,
                RiskScore = fraudScore.Score,
                RiskLevel = fraudScore.RiskLevel,
                Reason = fraudScore.Explanation,
                TransactionContextId = context.Id,
                RequiresStepUpAuth = fraudScore.Decision == FraudDecision.Challenge
            };
        }
        catch (Exception ex)
        {
            // Fail-safe: On error, flag for review
            _logger.LogError(ex,
                "Wekeza Nexus encountered error evaluating transaction for user {UserId}. Failing safe to Review.",
                userId);
                
            return new NexusVerdict
            {
                Decision = FraudDecision.Review,
                RiskScore = 500,
                RiskLevel = RiskLevel.Medium,
                Reason = "Fraud check encountered a technical error. Transaction flagged for manual review.",
                RequiresStepUpAuth = false
            };
        }
    }
    
    /// <summary>
    /// Re-evaluate after step-up authentication (OTP, Biometric)
    /// </summary>
    public async Task<NexusVerdict> ReEvaluateAfterChallengeAsync(
        Guid transactionContextId,
        bool challengePassed,
        CancellationToken cancellationToken = default)
    {
        var fraudScore = await _fraudService.ReEvaluateAfterChallengeAsync(
            transactionContextId, 
            challengePassed, 
            cancellationToken);
        
        return new NexusVerdict
        {
            Decision = fraudScore.Decision,
            RiskScore = fraudScore.Score,
            RiskLevel = fraudScore.RiskLevel,
            Reason = fraudScore.Explanation,
            TransactionContextId = transactionContextId,
            RequiresStepUpAuth = false
        };
    }
    
    private async Task<TransactionContext> BuildTransactionContext(
        Guid userId,
        string fromAccountNumber,
        string toAccountNumber,
        decimal amount,
        string currency,
        string transactionType,
        string description,
        DeviceFingerprint? deviceInfo,
        BehavioralMetrics? behavioralData,
        string channel,
        string sessionId,
        CancellationToken cancellationToken)
    {
        // Get velocity metrics
        var recentCount10min = await _velocityService.GetTransactionCountAsync(userId, 10, cancellationToken);
        var recentAmount10min = await _velocityService.GetTransactionAmountAsync(userId, 10, cancellationToken);
        var dailyCount = await _velocityService.GetTransactionCountAsync(userId, 1440, cancellationToken);
        var dailyAmount = await _velocityService.GetTransactionAmountAsync(userId, 1440, cancellationToken);
        var avgAmount = await _velocityService.GetAverageTransactionAmountAsync(userId, cancellationToken);
        
        // Calculate deviation (as percentage)
        // Positive deviation = higher than average, Negative = lower than average
        var deviation = avgAmount > 0 ? ((amount - avgAmount) / avgAmount) * 100 : 0;
        
        // Check beneficiary
        var isFirstTime = await _velocityService.IsFirstTimeBeneficiaryAsync(userId, toAccountNumber, cancellationToken);
        var beneficiaryAge = await _velocityService.GetAccountAgeDaysAsync(toAccountNumber, cancellationToken);
        
        // Determine time of day
        var hour = DateTime.UtcNow.Hour;
        var timeOfDay = hour switch
        {
            >= 22 or < 6 => "Night",
            >= 6 and < 12 => "Morning",
            >= 12 and < 18 => "Afternoon",
            _ => "Evening"
        };
        
        return new TransactionContext
        {
            UserId = userId,
            FromAccountNumber = fromAccountNumber,
            ToAccountNumber = toAccountNumber,
            Amount = amount,
            Currency = currency,
            TransactionType = transactionType,
            Description = description,
            DeviceInfo = deviceInfo,
            BehavioralData = behavioralData,
            TimeOfDay = timeOfDay,
            IsFirstTimeBeneficiary = isFirstTime,
            BeneficiaryAccountAgeDays = beneficiaryAge,
            RecentTransactionCount = recentCount10min,
            RecentTransactionAmount = recentAmount10min,
            DailyTransactionCount = dailyCount,
            DailyTransactionAmount = dailyAmount,
            AverageTransactionAmount = avgAmount,
            AmountDeviationPercent = (double)deviation,
            SessionId = sessionId,
            Channel = channel
        };
    }
    
    /// <summary>
    /// Records a transaction in the history repository for velocity analysis
    /// </summary>
    private async Task RecordTransactionAsync(
        Guid userId,
        string fromAccountNumber,
        string toAccountNumber,
        decimal amount,
        string currency,
        string transactionType,
        string fraudDecision,
        CancellationToken cancellationToken)
    {
        var transactionRecord = new TransactionRecord
        {
            UserId = userId,
            FromAccountNumber = fromAccountNumber,
            ToAccountNumber = toAccountNumber,
            Amount = amount,
            Currency = currency,
            TransactionType = transactionType,
            TransactionTime = DateTime.UtcNow,
            TransactionReference = $"TXN-{Guid.NewGuid():N}",
            WasAllowed = fraudDecision == "Allow",
            FraudDecision = fraudDecision
        };
        
        await _historyRepository.AddTransactionAsync(transactionRecord, cancellationToken);
    }
}

/// <summary>
/// The verdict returned from Wekeza Nexus evaluation
/// </summary>
public class NexusVerdict
{
    /// <summary>
    /// The decision: Allow, Challenge, Block, or Review
    /// </summary>
    public FraudDecision Decision { get; init; }
    
    /// <summary>
    /// Risk score (0-1000)
    /// </summary>
    public int RiskScore { get; init; }
    
    /// <summary>
    /// Risk level classification
    /// </summary>
    public RiskLevel RiskLevel { get; init; }
    
    /// <summary>
    /// Human-readable explanation
    /// </summary>
    public string Reason { get; init; } = string.Empty;
    
    /// <summary>
    /// Transaction context ID for tracking
    /// </summary>
    public Guid TransactionContextId { get; init; }
    
    /// <summary>
    /// Whether step-up authentication is required
    /// </summary>
    public bool RequiresStepUpAuth { get; init; }
}
