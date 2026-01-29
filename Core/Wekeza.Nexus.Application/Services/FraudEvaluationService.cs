using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Domain.Enums;
using Wekeza.Nexus.Domain.Interfaces;
using Wekeza.Nexus.Domain.ValueObjects;
using System.Diagnostics;

namespace Wekeza.Nexus.Application.Services;

/// <summary>
/// Core fraud evaluation service implementing the Wekeza Nexus "Deep Brain"
/// 
/// This service combines multiple fraud detection signals:
/// 1. Velocity checks (transactions over time)
/// 2. Behavioral biometrics (typing, mouse, device patterns)
/// 3. Graph relationships (circular transactions, mule accounts)
/// 4. Amount anomalies (unusual amounts)
/// 5. Device and location intelligence
/// 
/// Inspired by: Feedzai, BioCatch, RembrandtAi, NICE Actimize
/// </summary>
public class FraudEvaluationService : IFraudEvaluationService
{
    private readonly ITransactionVelocityService _velocityService;
    private readonly IFraudEvaluationRepository _evaluationRepository;
    
    // Scoring weights for the ensemble model
    private const double VelocityWeight = 0.30;
    private const double BehavioralWeight = 0.25;
    private const double RelationshipWeight = 0.25;
    private const double AmountWeight = 0.15;
    private const double DeviceWeight = 0.05;
    
    public FraudEvaluationService(
        ITransactionVelocityService velocityService,
        IFraudEvaluationRepository evaluationRepository)
    {
        _velocityService = velocityService;
        _evaluationRepository = evaluationRepository;
    }
    
    public async Task<FraudScore> EvaluateAsync(
        TransactionContext context, 
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Run all scoring engines in parallel for speed
            var velocityScoreTask = CalculateVelocityScore(context, cancellationToken);
            var behavioralScoreTask = CalculateBehavioralScore(context, cancellationToken);
            var relationshipScoreTask = CalculateRelationshipScore(context, cancellationToken);
            var amountScoreTask = CalculateAmountScore(context, cancellationToken);
            var deviceScoreTask = CalculateDeviceScore(context, cancellationToken);
            
            await Task.WhenAll(
                velocityScoreTask, 
                behavioralScoreTask, 
                relationshipScoreTask, 
                amountScoreTask, 
                deviceScoreTask);
            
            // Weighted ensemble scoring
            var velocityScore = await velocityScoreTask;
            var behavioralScore = await behavioralScoreTask;
            var relationshipScore = await relationshipScoreTask;
            var amountScore = await amountScoreTask;
            var deviceScore = await deviceScoreTask;
            
            var totalScore = (int)(
                velocityScore.Score * VelocityWeight +
                behavioralScore.Score * BehavioralWeight +
                relationshipScore.Score * RelationshipWeight +
                amountScore.Score * AmountWeight +
                deviceScore.Score * DeviceWeight);
            
            // Determine primary reason based on highest contributing score
            var primaryReason = GetPrimaryReason(
                velocityScore, behavioralScore, relationshipScore, amountScore, deviceScore);
            
            // Build list of contributing reasons
            var contributingReasons = new List<FraudReason>();
            if (velocityScore.Score > 200) contributingReasons.AddRange(velocityScore.Reasons);
            if (behavioralScore.Score > 200) contributingReasons.AddRange(behavioralScore.Reasons);
            if (relationshipScore.Score > 200) contributingReasons.AddRange(relationshipScore.Reasons);
            if (amountScore.Score > 200) contributingReasons.AddRange(amountScore.Reasons);
            if (deviceScore.Score > 200) contributingReasons.AddRange(deviceScore.Reasons);
            
            // Generate human-readable explanation
            var explanation = GenerateExplanation(
                totalScore, 
                primaryReason, 
                velocityScore, 
                behavioralScore, 
                relationshipScore,
                amountScore,
                deviceScore,
                context);
            
            stopwatch.Stop();
            
            // Create final fraud score
            var fraudScore = FraudScore.Create(
                totalScore,
                primaryReason,
                contributingReasons.Distinct().ToList(),
                explanation,
                confidence: 0.85);
            
            // Store evaluation for audit trail
            var evaluation = FraudEvaluation.Create(
                context.Id,
                context.UserId,
                $"TXN-{context.Id}",
                context.Amount,
                fraudScore,
                stopwatch.ElapsedMilliseconds);
            
            await _evaluationRepository.AddAsync(evaluation, cancellationToken);
            
            return fraudScore;
        }
        catch (Exception)
        {
            // On error, fail-safe: allow transaction but flag for review
            // Log full exception details internally but don't expose to users
            // TODO: Add proper logging infrastructure
            return FraudScore.Create(
                500, 
                FraudReason.None, 
                explanation: "Fraud evaluation encountered a technical error. Transaction flagged for manual review.",
                confidence: 0.0);
        }
    }
    
    public async Task<FraudScore> ReEvaluateAfterChallengeAsync(
        Guid transactionContextId, 
        bool challengePassed,
        CancellationToken cancellationToken = default)
    {
        // If challenge passed, reduce risk score
        if (challengePassed)
        {
            return FraudScore.Create(
                200, 
                FraudReason.None, 
                explanation: "Challenge completed successfully. Transaction authorized.",
                confidence: 0.95);
        }
        
        // If challenge failed, increase risk
        return FraudScore.Create(
            950, 
            FraudReason.MultipleFailedAttempts, 
            explanation: "Challenge authentication failed. Transaction blocked for security.",
            confidence: 0.98);
    }
    
    #region Scoring Engines
    
    private async Task<ScoringResult> CalculateVelocityScore(
        TransactionContext context, 
        CancellationToken cancellationToken)
    {
        var score = 0;
        var reasons = new List<FraudReason>();
        
        // Check transaction velocity (10-minute window)
        var recentCount = await _velocityService.GetTransactionCountAsync(
            context.UserId, 10, cancellationToken);
        
        if (recentCount >= 5)
        {
            score += 300;
            reasons.Add(FraudReason.HighTransactionVelocity);
        }
        else if (recentCount >= 3)
        {
            score += 150;
        }
        
        // Check amount velocity
        var recentAmount = await _velocityService.GetTransactionAmountAsync(
            context.UserId, 10, cancellationToken);
        
        if (recentAmount > context.Amount * 10)
        {
            score += 250;
            reasons.Add(FraudReason.HighAmountVelocity);
        }
        
        // Check daily transaction count
        if (context.DailyTransactionCount > 20)
        {
            score += 200;
            reasons.Add(FraudReason.UnusualTransactionPattern);
        }
        
        return new ScoringResult(score, reasons);
    }
    
    private Task<ScoringResult> CalculateBehavioralScore(
        TransactionContext context, 
        CancellationToken cancellationToken)
    {
        var score = 0;
        var reasons = new List<FraudReason>();
        
        if (context.BehavioralData == null)
        {
            // No behavioral data available - slight risk increase
            return Task.FromResult(new ScoringResult(100, reasons));
        }
        
        var behavioral = context.BehavioralData;
        
        // Check for social engineering indicators
        if (behavioral.IsOnActiveCall)
        {
            score += 400; // Major red flag
            reasons.Add(FraudReason.AnomalousBehavior);
        }
        
        // Screen sharing during financial transaction
        if (behavioral.IsScreenShared)
        {
            score += 350;
            reasons.Add(FraudReason.AnomalousBehavior);
        }
        
        // High behavioral anomaly score
        if (behavioral.BehaviorAnomalyScore > 0.7)
        {
            score += 300;
            reasons.Add(FraudReason.AnomalousBehavior);
        }
        
        // Very short session (automation)
        if (behavioral.SessionDuration < 5)
        {
            score += 200;
            reasons.Add(FraudReason.AnomalousBehavior);
        }
        
        // Excessive copy-paste (social engineering)
        if (behavioral.CopyPasteCount > 3)
        {
            score += 150;
        }
        
        return Task.FromResult(new ScoringResult(score, reasons));
    }
    
    private async Task<ScoringResult> CalculateRelationshipScore(
        TransactionContext context, 
        CancellationToken cancellationToken)
    {
        var score = 0;
        var reasons = new List<FraudReason>();
        
        // First-time beneficiary risk
        if (context.IsFirstTimeBeneficiary)
        {
            score += 200;
            reasons.Add(FraudReason.FirstTimeBeneficiary);
        }
        
        // New beneficiary account (mule account pattern)
        if (context.BeneficiaryAccountAgeDays.HasValue && 
            context.BeneficiaryAccountAgeDays.Value < 7)
        {
            score += 350;
            reasons.Add(FraudReason.NewAccountBeneficiary);
            reasons.Add(FraudReason.MuleAccountPattern);
        }
        
        // Check for circular transactions
        var isCircular = await _velocityService.DetectCircularTransactionAsync(
            context.FromAccountNumber, 
            context.ToAccountNumber, 
            24, 
            cancellationToken);
        
        if (isCircular)
        {
            score += 400;
            reasons.Add(FraudReason.CircularTransactionDetected);
        }
        
        return new ScoringResult(score, reasons);
    }
    
    private Task<ScoringResult> CalculateAmountScore(
        TransactionContext context, 
        CancellationToken cancellationToken)
    {
        var score = 0;
        var reasons = new List<FraudReason>();
        
        // Check deviation from average
        if (context.AverageTransactionAmount > 0)
        {
            var deviation = Math.Abs(context.AmountDeviationPercent);
            
            if (deviation > 500) // 5x normal amount
            {
                score += 300;
                reasons.Add(FraudReason.UnusuallyHighAmount);
            }
            else if (deviation > 200) // 2x normal amount
            {
                score += 150;
            }
        }
        
        // Round amount pattern (common in fraud)
        // Note: Only flag for very large round amounts to reduce false positives
        if (context.Amount % 10000 == 0 && context.Amount >= 100000)
        {
            score += 50; // Reduced from 100
            reasons.Add(FraudReason.RoundAmountPattern);
        }
        
        // Very large amounts
        if (context.Amount > 1000000)
        {
            score += 200;
        }
        
        return Task.FromResult(new ScoringResult(score, reasons));
    }
    
    private Task<ScoringResult> CalculateDeviceScore(
        TransactionContext context, 
        CancellationToken cancellationToken)
    {
        var score = 0;
        var reasons = new List<FraudReason>();
        
        if (context.DeviceInfo == null)
        {
            return Task.FromResult(new ScoringResult(50, reasons));
        }
        
        var device = context.DeviceInfo;
        
        // Unrecognized device
        if (!device.IsRecognizedDevice)
        {
            score += 150;
            reasons.Add(FraudReason.DeviceMismatch);
        }
        
        // VPN/Proxy usage
        if (device.IsVpnOrProxy)
        {
            score += 100;
        }
        
        // Location anomaly
        if (!string.IsNullOrEmpty(device.Location) && device.Location.Contains("Unknown"))
        {
            score += 100;
            reasons.Add(FraudReason.LocationAnomaly);
        }
        
        return Task.FromResult(new ScoringResult(score, reasons));
    }
    
    #endregion
    
    #region Helper Methods
    
    private FraudReason GetPrimaryReason(
        ScoringResult velocity,
        ScoringResult behavioral,
        ScoringResult relationship,
        ScoringResult amount,
        ScoringResult device)
    {
        var scores = new Dictionary<ScoringResult, string>
        {
            { velocity, "velocity" },
            { behavioral, "behavioral" },
            { relationship, "relationship" },
            { amount, "amount" },
            { device, "device" }
        };
        
        var highest = scores.OrderByDescending(x => x.Key.Score).First();
        
        var firstReason = highest.Key.Reasons.FirstOrDefault();
        return firstReason != default(FraudReason) ? firstReason : FraudReason.None;
    }
    
    private string GenerateExplanation(
        int totalScore,
        FraudReason primaryReason,
        ScoringResult velocity,
        ScoringResult behavioral,
        ScoringResult relationship,
        ScoringResult amount,
        ScoringResult device,
        TransactionContext context)
    {
        if (totalScore <= 200)
        {
            return "Transaction appears normal with no significant risk indicators detected.";
        }
        
        var explanation = $"Transaction flagged with risk score {totalScore}/1000. ";
        
        // Add specific reasons based on what triggered
        var details = new List<string>();
        
        if (velocity.Score > 200)
        {
            details.Add($"high transaction velocity ({context.RecentTransactionCount} transactions in 10 minutes)");
        }
        
        if (behavioral.Score > 200 && context.BehavioralData != null)
        {
            if (context.BehavioralData.IsOnActiveCall)
                details.Add("user on active call during transaction (social engineering risk)");
            if (context.BehavioralData.BehaviorAnomalyScore > 0.7)
                details.Add($"behavior anomaly detected ({context.BehavioralData.BehaviorAnomalyScore:P0} deviation)");
        }
        
        if (relationship.Score > 200)
        {
            if (context.IsFirstTimeBeneficiary)
                details.Add("first-time beneficiary");
            if (context.BeneficiaryAccountAgeDays.HasValue && context.BeneficiaryAccountAgeDays.Value < 7)
                details.Add($"recipient account only {context.BeneficiaryAccountAgeDays} days old (mule account pattern)");
        }
        
        if (amount.Score > 200)
        {
            // Check if deviation is positive (higher) or negative (lower)
            if (context.AmountDeviationPercent > 0)
            {
                details.Add($"amount {Math.Abs(context.AmountDeviationPercent):F0}% higher than normal");
            }
            else if (context.AmountDeviationPercent < 0)
            {
                details.Add($"amount {Math.Abs(context.AmountDeviationPercent):F0}% lower than normal");
            }
        }
        
        if (device.Score > 100 && context.DeviceInfo != null)
        {
            if (!context.DeviceInfo.IsRecognizedDevice)
                details.Add("unrecognized device");
            if (context.DeviceInfo.IsVpnOrProxy)
                details.Add("VPN/proxy detected");
        }
        
        if (details.Any())
        {
            explanation += "Key factors: " + string.Join(", ", details) + ".";
        }
        
        return explanation;
    }
    
    #endregion
    
    /// <summary>
    /// Internal result structure for individual scoring engines
    /// </summary>
    private record ScoringResult(int Score, List<FraudReason> Reasons);
}
