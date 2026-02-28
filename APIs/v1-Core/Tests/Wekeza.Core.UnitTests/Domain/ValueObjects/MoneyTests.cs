using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Xunit;

namespace Wekeza.Core.UnitTests.Domain.ValueObjects;

/// <summary>
/// Unit tests for Money value object
/// </summary>
public class MoneyTests
{
    private readonly Currency _kes = Currency.FromCode("KES");
    private readonly Currency _usd = Currency.FromCode("USD");

    [Fact]
    public void Money_Creation_ShouldRoundToTwoDecimalPlaces()
    {
        // Arrange & Act
        var money = new Money(100.12345m, _kes);

        // Assert
        Assert.Equal(100.12m, money.Amount);
    }

    [Fact]
    public void Money_Addition_WithSameCurrency_ShouldSucceed()
    {
        // Arrange
        var money1 = new Money(100m, _kes);
        var money2 = new Money(50m, _kes);

        // Act
        var result = money1 + money2;

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal(_kes, result.Currency);
    }

    [Fact]
    public void Money_Addition_WithDifferentCurrency_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(100m, _kes);
        var money2 = new Money(50m, _usd);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => money1 + money2);
        Assert.Contains("Cannot add amounts with different currencies", exception.Message);
    }

    [Fact]
    public void Money_Subtraction_WithSameCurrency_ShouldSucceed()
    {
        // Arrange
        var money1 = new Money(100m, _kes);
        var money2 = new Money(30m, _kes);

        // Act
        var result = money1 - money2;

        // Assert
        Assert.Equal(70m, result.Amount);
    }

    [Fact]
    public void Money_IsZero_ShouldReturnTrue_WhenAmountIsZero()
    {
        // Arrange
        var money = Money.Zero(_kes);

        // Act & Assert
        Assert.True(money.IsZero());
    }

    [Fact]
    public void Money_IsNegative_ShouldReturnTrue_WhenAmountIsNegative()
    {
        // Arrange
        var money = new Money(-50m, _kes);

        // Act & Assert
        Assert.True(money.IsNegative());
    }

    [Fact]
    public void Money_IsGreaterThan_ShouldCompareCorrectly()
    {
        // Arrange
        var money1 = new Money(100m, _kes);
        var money2 = new Money(50m, _kes);

        // Act & Assert
        Assert.True(money1.IsGreaterThan(money2));
        Assert.False(money2.IsGreaterThan(money1));
    }

    [Fact]
    public void Money_ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var money = new Money(1234.56m, _kes);

        // Act
        var result = money.ToString();

        // Assert
        Assert.Contains("1,234.56", result);
    }
}
