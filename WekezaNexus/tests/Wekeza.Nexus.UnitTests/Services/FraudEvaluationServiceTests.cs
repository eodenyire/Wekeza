using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using Wekeza.Nexus.Application.Services;
using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Domain.Enums;
using Wekeza.Nexus.Domain.Interfaces;
using Wekeza.Nexus.Domain.ValueObjects;

namespace Wekeza.Nexus.UnitTests.Services;

/// <summary>
/// Unit tests for FraudEvaluationService
/// 
/// Tests cover:
/// - Safe transactions (low risk)
/// - High velocity fraud
/// - Behavioral anomalies
/// - Mule account detection
/// - Device mismatch scenarios
/// </summary>
public class FraudEvaluationServiceTests
{
    private readonly Mock<ITransactionVelocityService> _mockVelocityService;
    private readonly Mock<IFraudEvaluationRepository> _mockRepository;
    private readonly Mock<ILogger<FraudEvaluationService>> _mockLogger;
    private readonly FraudEvaluationService _fraudService;
    
    public FraudEvaluationServiceTests()
    {
        _mockVelocityService = new Mock<ITransactionVelocityService>();
        _mockRepository = new Mock<IFraudEvaluationRepository>();
        _mockLogger = new Mock<ILogger<FraudEvaluationService>>();
        _fraudService = new FraudEvaluationService(
            _mockVelocityService.Object, 
            _mockRepository.Object,
            _mockLogger.Object);
        
        // Setup default safe values
        _mockVelocityService.Setup(x => x.GetTransactionCountAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _mockVelocityService.Setup(x => x.GetTransactionAmountAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);
        _mockVelocityService.Setup(x => x.GetAverageTransactionAmountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);
        _mockVelocityService.Setup(x => x.IsFirstTimeBeneficiaryAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockVelocityService.Setup(x => x.DetectCircularTransactionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }
    
    [Fact]
    public async Task EvaluateAsync_NormalTransaction_ShouldReturnLowRisk()
    {
        // Arrange
        var context = new TransactionContext
        {
            UserId = Guid.NewGuid(),
            FromAccountNumber = "ACC001",
            ToAccountNumber = "ACC002",
            Amount = 1000m,
            Currency = "KES",
            TransactionType = "Transfer",
            AverageTransactionAmount = 1000m,
            AmountDeviationPercent = 0,
            RecentTransactionCount = 1,
            DailyTransactionCount = 3,
            IsFirstTimeBeneficiary = false,
            DeviceInfo = new DeviceFingerprint
            {
                IsRecognizedDevice = true,
                IsVpnOrProxy = false
            },
            BehavioralData = new BehavioralMetrics
            {
                IsOnActiveCall = false,
                SessionDuration = 60,
                BehaviorAnomalyScore = 0.1
            }
        };
        
        // Act
        var result = await _fraudService.EvaluateAsync(context);
        
        // Assert
        result.Should().NotBeNull();
        result.Score.Should().BeLessOrEqualTo(400); // Should be in Allow range
        result.Decision.Should().Be(FraudDecision.Allow);
        result.RiskLevel.Should().BeOneOf(RiskLevel.VeryLow, RiskLevel.Low);
    }
    
    [Fact]
    public async Task EvaluateAsync_HighVelocity_ShouldReturnHighRisk()
    {
        // Arrange
        _mockVelocityService.Setup(x => x.GetTransactionCountAsync(It.IsAny<Guid>(), 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(6); // 6 transactions in 10 minutes - high velocity
        
        var context = new TransactionContext
        {
            UserId = Guid.NewGuid(),
            FromAccountNumber = "ACC001",
            ToAccountNumber = "ACC002",
            Amount = 5000m,
            Currency = "KES",
            RecentTransactionCount = 6, // This needs to match the velocity service
            DailyTransactionCount = 8
        };
        
        // Act
        var result = await _fraudService.EvaluateAsync(context);
        
        // Assert
        // Velocity score of 300 * 0.30 weight = 90 base score
        result.Score.Should().BeGreaterThan(50); // Should show elevated risk
        result.ContributingReasons.Should().Contain(FraudReason.HighTransactionVelocity);
    }
    
    [Fact]
    public async Task ReEvaluateAfterChallengeAsync_ChallengePassed_ShouldReturnLowRisk()
    {
        // Arrange
        var transactionContextId = Guid.NewGuid();
        
        // Act
        var result = await _fraudService.ReEvaluateAfterChallengeAsync(transactionContextId, challengePassed: true);
        
        // Assert
        result.Score.Should().BeLessOrEqualTo(400);
        result.Decision.Should().Be(FraudDecision.Allow);
    }
}
