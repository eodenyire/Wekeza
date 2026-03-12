using Xunit;
using FluentAssertions;
using Wekeza.Nexus.Application.Services;
using Wekeza.Nexus.Domain.Entities;
using Wekeza.Nexus.Infrastructure.Repositories;

namespace Wekeza.Nexus.UnitTests.Services;

/// <summary>
/// Integration tests for TransactionVelocityService with actual repository
/// Tests that the TODO implementations actually work
/// </summary>
public class TransactionVelocityServiceTests
{
    private readonly InMemoryTransactionHistoryRepository _repository;
    private readonly TransactionVelocityService _velocityService;
    
    public TransactionVelocityServiceTests()
    {
        _repository = new InMemoryTransactionHistoryRepository();
        _velocityService = new TransactionVelocityService(_repository);
    }
    
    [Fact]
    public async Task GetTransactionCountAsync_WithTransactions_ShouldReturnCorrectCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        // Add 3 transactions in the last 10 minutes
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 1000m,
            TransactionTime = DateTime.UtcNow.AddMinutes(-5)
        });
        
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 2000m,
            TransactionTime = DateTime.UtcNow.AddMinutes(-8)
        });
        
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 500m,
            TransactionTime = DateTime.UtcNow.AddMinutes(-2)
        });
        
        // Add 1 transaction from 15 minutes ago (should not be counted)
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 3000m,
            TransactionTime = DateTime.UtcNow.AddMinutes(-15)
        });
        
        // Act
        var count = await _velocityService.GetTransactionCountAsync(userId, 10);
        
        // Assert
        count.Should().Be(3);
    }
    
    [Fact]
    public async Task GetTransactionAmountAsync_WithTransactions_ShouldReturnCorrectSum()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 1000m,
            TransactionTime = DateTime.UtcNow.AddMinutes(-5)
        });
        
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 2000m,
            TransactionTime = DateTime.UtcNow.AddMinutes(-8)
        });
        
        // Act
        var totalAmount = await _velocityService.GetTransactionAmountAsync(userId, 10);
        
        // Assert
        totalAmount.Should().Be(3000m);
    }
    
    [Fact]
    public async Task GetAverageTransactionAmountAsync_WithTransactions_ShouldReturnAverage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        // Add 3 transactions with amounts: 1000, 2000, 3000
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 1000m,
            TransactionTime = DateTime.UtcNow.AddDays(-5)
        });
        
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 2000m,
            TransactionTime = DateTime.UtcNow.AddDays(-10)
        });
        
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            Amount = 3000m,
            TransactionTime = DateTime.UtcNow.AddDays(-15)
        });
        
        // Act
        var average = await _velocityService.GetAverageTransactionAmountAsync(userId);
        
        // Assert
        average.Should().Be(2000m); // (1000 + 2000 + 3000) / 3 = 2000
    }
    
    [Fact]
    public async Task GetAverageTransactionAmountAsync_WithNoTransactions_ShouldReturnDefault()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        // Act
        var average = await _velocityService.GetAverageTransactionAmountAsync(userId);
        
        // Assert
        average.Should().Be(5000m); // Default baseline
    }
    
    [Fact]
    public async Task IsFirstTimeBeneficiaryAsync_WithHistory_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beneficiaryAccount = "ACC999";
        
        // Add a previous transaction to this beneficiary
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            UserId = userId,
            ToAccountNumber = beneficiaryAccount,
            Amount = 1000m,
            TransactionTime = DateTime.UtcNow.AddDays(-5)
        });
        
        // Act
        var isFirstTime = await _velocityService.IsFirstTimeBeneficiaryAsync(userId, beneficiaryAccount);
        
        // Assert
        isFirstTime.Should().BeFalse();
    }
    
    [Fact]
    public async Task IsFirstTimeBeneficiaryAsync_WithoutHistory_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var beneficiaryAccount = "ACC999";
        
        // Act
        var isFirstTime = await _velocityService.IsFirstTimeBeneficiaryAsync(userId, beneficiaryAccount);
        
        // Assert
        isFirstTime.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetAccountAgeDaysAsync_WithMetadata_ShouldReturnAge()
    {
        // Arrange
        var accountNumber = "ACC123";
        var createdDate = DateTime.UtcNow.AddDays(-10);
        
        await _repository.UpsertAccountMetadataAsync(new AccountMetadata
        {
            AccountNumber = accountNumber,
            CreatedAt = createdDate
        });
        
        // Act
        var age = await _velocityService.GetAccountAgeDaysAsync(accountNumber);
        
        // Assert
        age.Should().NotBeNull();
        age.Should().BeGreaterOrEqualTo(9); // Could be 9 or 10 depending on timing
        age.Should().BeLessOrEqualTo(10);
    }
    
    [Fact]
    public async Task GetAccountAgeDaysAsync_WithoutMetadata_ShouldReturnNull()
    {
        // Arrange
        var accountNumber = "ACC999";
        
        // Act
        var age = await _velocityService.GetAccountAgeDaysAsync(accountNumber);
        
        // Assert
        age.Should().BeNull();
    }
    
    [Fact]
    public async Task DetectCircularTransactionAsync_WithCircularPattern_ShouldReturnTrue()
    {
        // Arrange
        // Create circular pattern: A → B → C → A
        var accountA = "ACC-A";
        var accountB = "ACC-B";
        var accountC = "ACC-C";
        
        // A → B
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            FromAccountNumber = accountA,
            ToAccountNumber = accountB,
            Amount = 1000m,
            TransactionTime = DateTime.UtcNow.AddHours(-2)
        });
        
        // B → C
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            FromAccountNumber = accountB,
            ToAccountNumber = accountC,
            Amount = 900m,
            TransactionTime = DateTime.UtcNow.AddHours(-1)
        });
        
        // C → A (completing the circle)
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            FromAccountNumber = accountC,
            ToAccountNumber = accountA,
            Amount = 800m,
            TransactionTime = DateTime.UtcNow.AddMinutes(-30)
        });
        
        // Act
        var isCircular = await _velocityService.DetectCircularTransactionAsync(
            accountA, accountB, lookbackHours: 24);
        
        // Assert
        isCircular.Should().BeTrue();
    }
    
    [Fact]
    public async Task DetectCircularTransactionAsync_WithoutCircularPattern_ShouldReturnFalse()
    {
        // Arrange
        // Create linear pattern: A → B → C (no return to A)
        var accountA = "ACC-A";
        var accountB = "ACC-B";
        var accountC = "ACC-C";
        
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            FromAccountNumber = accountA,
            ToAccountNumber = accountB,
            Amount = 1000m,
            TransactionTime = DateTime.UtcNow.AddHours(-2)
        });
        
        await _repository.AddTransactionAsync(new TransactionRecord
        {
            FromAccountNumber = accountB,
            ToAccountNumber = accountC,
            Amount = 900m,
            TransactionTime = DateTime.UtcNow.AddHours(-1)
        });
        
        // Act
        var isCircular = await _velocityService.DetectCircularTransactionAsync(
            accountA, accountB, lookbackHours: 24);
        
        // Assert
        isCircular.Should().BeFalse();
    }
}
