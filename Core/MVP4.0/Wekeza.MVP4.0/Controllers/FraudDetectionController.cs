using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Nexus.Application.Services;
using Wekeza.Nexus.Application.Exceptions;
using Wekeza.Nexus.Domain.ValueObjects;
using Wekeza.Nexus.Domain.Enums;
using Wekeza.Nexus.Domain.Interfaces;

namespace Wekeza.MVP4._0.Controllers;

/// <summary>
/// Fraud detection API controller for Wekeza Nexus
/// Provides endpoints for fraud evaluation and review management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FraudDetectionController : ControllerBase
{
    private readonly WekezaNexusClient _nexusClient;
    private readonly IFraudEvaluationRepository _repository;
    private readonly ILogger<FraudDetectionController> _logger;

    public FraudDetectionController(
        WekezaNexusClient nexusClient,
        IFraudEvaluationRepository repository,
        ILogger<FraudDetectionController> logger)
    {
        _nexusClient = nexusClient ?? throw new ArgumentNullException(nameof(nexusClient));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Evaluate a transaction for fraud
    /// </summary>
    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateTransaction([FromBody] EvaluateTransactionRequest request)
    {
        try
        {
            var verdict = await _nexusClient.EvaluateTransactionAsync(
                userId: request.UserId,
                fromAccountNumber: request.FromAccountNumber,
                toAccountNumber: request.ToAccountNumber,
                amount: request.Amount,
                currency: request.Currency,
                deviceInfo: request.DeviceInfo,
                behavioralData: request.BehavioralData
            );

            return Ok(new
            {
                TransactionContextId = verdict.TransactionContextId,
                Decision = verdict.Decision.ToString(),
                RiskScore = verdict.RiskScore,
                RiskLevel = verdict.RiskLevel.ToString(),
                Reasons = verdict.Reasons.Select(r => r.ToString()),
                Explanation = verdict.Explanation,
                RequiresChallenge = verdict.Decision == FraudDecision.Challenge,
                RequiresReview = verdict.Decision == FraudDecision.Review,
                IsBlocked = verdict.Decision == FraudDecision.Block
            });
        }
        catch (FraudDetectedException ex)
        {
            _logger.LogWarning(ex, "Transaction blocked by fraud detection");
            return BadRequest(new { Error = "Transaction blocked", Reason = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating transaction");
            return StatusCode(500, new { Error = "An error occurred while evaluating the transaction" });
        }
    }

    /// <summary>
    /// Get fraud evaluation by ID
    /// </summary>
    [HttpGet("evaluation/{id}")]
    public async Task<IActionResult> GetEvaluation(Guid id)
    {
        try
        {
            var evaluation = await _repository.GetByIdAsync(id);
            
            if (evaluation == null)
            {
                return NotFound(new { Error = "Evaluation not found" });
            }

            return Ok(new
            {
                evaluation.Id,
                evaluation.TransactionContextId,
                evaluation.UserId,
                evaluation.TransactionReference,
                evaluation.Amount,
                FraudScore = new
                {
                    evaluation.FraudScore.TotalScore,
                    evaluation.FraudScore.VelocityScore,
                    evaluation.FraudScore.BehavioralScore,
                    evaluation.FraudScore.RelationshipScore,
                    evaluation.FraudScore.AmountScore,
                    evaluation.FraudScore.DeviceScore,
                    Decision = evaluation.FraudScore.Decision.ToString(),
                    RiskLevel = evaluation.FraudScore.RiskLevel.ToString(),
                    Reasons = evaluation.FraudScore.Reasons.Select(r => r.ToString()),
                    evaluation.FraudScore.Explanation
                },
                evaluation.EvaluatedAt,
                evaluation.ProcessingTimeMs,
                evaluation.WasAllowed,
                evaluation.RequiresReview,
                evaluation.AnalystNotes,
                evaluation.WasActualFraud
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting evaluation {Id}", id);
            return StatusCode(500, new { Error = "An error occurred while retrieving the evaluation" });
        }
    }

    /// <summary>
    /// Get evaluations by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserEvaluations(Guid userId, [FromQuery] int limit = 100)
    {
        try
        {
            var evaluations = await _repository.GetByUserIdAsync(userId, limit);
            
            return Ok(evaluations.Select(e => new
            {
                e.Id,
                e.TransactionReference,
                e.Amount,
                RiskScore = e.FraudScore.TotalScore,
                Decision = e.FraudScore.Decision.ToString(),
                e.EvaluatedAt,
                e.WasAllowed,
                e.RequiresReview
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting evaluations for user {UserId}", userId);
            return StatusCode(500, new { Error = "An error occurred while retrieving user evaluations" });
        }
    }

    /// <summary>
    /// Get pending reviews (for fraud analysts)
    /// </summary>
    [HttpGet("reviews/pending")]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> GetPendingReviews([FromQuery] int limit = 100)
    {
        try
        {
            var reviews = await _repository.GetPendingReviewsAsync(limit);
            
            return Ok(reviews.Select(r => new
            {
                r.Id,
                r.UserId,
                r.TransactionReference,
                r.Amount,
                RiskScore = r.FraudScore.TotalScore,
                Explanation = r.FraudScore.Explanation,
                r.EvaluatedAt
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending reviews");
            return StatusCode(500, new { Error = "An error occurred while retrieving pending reviews" });
        }
    }

    /// <summary>
    /// Add analyst review (for fraud analysts)
    /// </summary>
    [HttpPut("review/{id}")]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> AddReview(Guid id, [FromBody] ReviewRequest request)
    {
        try
        {
            var evaluation = await _repository.GetByIdAsync(id);
            
            if (evaluation == null)
            {
                return NotFound(new { Error = "Evaluation not found" });
            }

            evaluation.AddAnalystReview(request.Notes, request.WasActualFraud);
            await _repository.UpdateAsync(evaluation);
            
            return Ok(new { Message = "Review added successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding review for evaluation {Id}", id);
            return StatusCode(500, new { Error = "An error occurred while adding the review" });
        }
    }
}

/// <summary>
/// Request model for transaction evaluation
/// </summary>
public record EvaluateTransactionRequest
{
    public required Guid UserId { get; init; }
    public required string FromAccountNumber { get; init; }
    public required string ToAccountNumber { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public DeviceFingerprint? DeviceInfo { get; init; }
    public BehavioralMetrics? BehavioralData { get; init; }
}

/// <summary>
/// Request model for analyst review
/// </summary>
public record ReviewRequest
{
    public required string Notes { get; init; }
    public required bool WasActualFraud { get; init; }
}
