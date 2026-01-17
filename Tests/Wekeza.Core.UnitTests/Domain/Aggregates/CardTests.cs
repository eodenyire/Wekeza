using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Common;
using Xunit;

namespace Wekeza.Core.UnitTests.Domain.Aggregates;

/// <summary>
/// Unit tests for Card aggregate
/// </summary>
public class CardTests
{
    [Fact]
    public void Card_Creation_ShouldInitializeWithCorrectValues()
    {
        // Arrange & Act
        var card = new Card(
            id: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            cardType: "Debit",
            nameOnCard: "John Doe",
            dailyWithdrawalLimit: 50000m
        );

        // Assert
        Assert.False(card.IsCancelled);
        Assert.Equal(50000m, card.DailyWithdrawalLimit);
        Assert.Equal(0m, card.DailyWithdrawnToday);
        Assert.NotNull(card.CardNumber);
        Assert.Equal(16, card.CardNumber.Length);
    }

    [Fact]
    public void Card_CanWithdraw_WithinLimit_ShouldReturnTrue()
    {
        // Arrange
        var card = new Card(
            id: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            cardType: "Debit",
            nameOnCard: "John Doe",
            dailyWithdrawalLimit: 50000m
        );

        // Act
        var canWithdraw = card.CanWithdraw(10000m);

        // Assert
        Assert.True(canWithdraw);
    }

    [Fact]
    public void Card_CanWithdraw_ExceedingLimit_ShouldReturnFalse()
    {
        // Arrange
        var card = new Card(
            id: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            cardType: "Debit",
            nameOnCard: "John Doe",
            dailyWithdrawalLimit: 50000m
        );

        // Act
        var canWithdraw = card.CanWithdraw(60000m);

        // Assert
        Assert.False(canWithdraw);
    }

    [Fact]
    public void Card_RecordWithdrawal_ShouldUpdateDailyTotal()
    {
        // Arrange
        var card = new Card(
            id: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            cardType: "Debit",
            nameOnCard: "John Doe",
            dailyWithdrawalLimit: 50000m
        );

        // Act
        card.RecordWithdrawal(10000m);

        // Assert
        Assert.Equal(10000m, card.DailyWithdrawnToday);
    }

    [Fact]
    public void Card_RecordWithdrawal_ExceedingLimit_ShouldThrowException()
    {
        // Arrange
        var card = new Card(
            id: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            cardType: "Debit",
            nameOnCard: "John Doe",
            dailyWithdrawalLimit: 50000m
        );

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            card.RecordWithdrawal(60000m));
        Assert.Contains("exceed daily limit", exception.Message);
    }

    [Fact]
    public void Card_Cancel_ShouldSetCancelledStatus()
    {
        // Arrange
        var card = new Card(
            id: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            cardType: "Debit",
            nameOnCard: "John Doe",
            dailyWithdrawalLimit: 50000m
        );

        // Act
        card.Cancel("Lost card");

        // Assert
        Assert.True(card.IsCancelled);
        Assert.Equal("Lost card", card.CancellationReason);
    }

    [Fact]
    public void Card_Cancel_WhenAlreadyCancelled_ShouldThrowException()
    {
        // Arrange
        var card = new Card(
            id: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            cardType: "Debit",
            nameOnCard: "John Doe",
            dailyWithdrawalLimit: 50000m
        );
        card.Cancel("Lost card");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            card.Cancel("Stolen"));
        Assert.Contains("already cancelled", exception.Message);
    }

    [Fact]
    public void Card_CanWithdraw_WhenCancelled_ShouldReturnFalse()
    {
        // Arrange
        var card = new Card(
            id: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            cardType: "Debit",
            nameOnCard: "John Doe",
            dailyWithdrawalLimit: 50000m
        );
        card.Cancel("Lost card");

        // Act
        var canWithdraw = card.CanWithdraw(1000m);

        // Assert
        Assert.False(canWithdraw);
    }
}
