using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Exceptions;
using Wekeza.Core.Domain.ValueObjects;
using Xunit;

namespace Wekeza.Core.UnitTests.Domain.Aggregates;

/// <summary>
/// Unit tests for Account aggregate
/// </summary>
public class AccountTests
{
    private readonly Currency _kes = Currency.FromCode("KES");

    [Fact]
    public void Account_Creation_ShouldInitializeWithZeroBalance()
    {
        // Arrange & Act
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new AccountNumber("1234567890"),
            _kes
        );

        // Assert
        Assert.Equal(0m, account.Balance.Amount);
        Assert.False(account.IsFrozen);
    }

    [Fact]
    public void Account_Credit_ShouldIncreaseBalance()
    {
        // Arrange
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new AccountNumber("1234567890"),
            _kes
        );
        var amount = new Money(1000m, _kes);

        // Act
        account.Credit(amount);

        // Assert
        Assert.Equal(1000m, account.Balance.Amount);
    }

    [Fact]
    public void Account_Debit_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new AccountNumber("1234567890"),
            _kes
        );
        account.Credit(new Money(1000m, _kes));

        // Act
        account.Debit(new Money(300m, _kes));

        // Assert
        Assert.Equal(700m, account.Balance.Amount);
    }

    [Fact]
    public void Account_Debit_WithInsufficientFunds_ShouldThrowException()
    {
        // Arrange
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new AccountNumber("1234567890"),
            _kes
        );
        account.Credit(new Money(100m, _kes));

        // Act & Assert
        Assert.Throws<InsufficientFundsException>(() => 
            account.Debit(new Money(200m, _kes)));
    }

    [Fact]
    public void Account_Debit_WhenFrozen_ShouldThrowException()
    {
        // Arrange
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new AccountNumber("1234567890"),
            _kes
        );
        account.Credit(new Money(1000m, _kes));
        account.Freeze();

        // Act & Assert
        Assert.Throws<AccountFrozenException>(() => 
            account.Debit(new Money(100m, _kes)));
    }

    [Fact]
    public void Account_Credit_WhenFrozen_ShouldThrowException()
    {
        // Arrange
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new AccountNumber("1234567890"),
            _kes
        );
        account.Freeze();

        // Act & Assert
        Assert.Throws<AccountFrozenException>(() => 
            account.Credit(new Money(100m, _kes)));
    }

    [Fact]
    public void Account_Deactivate_WithNonZeroBalance_ShouldThrowException()
    {
        // Arrange
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new AccountNumber("1234567890"),
            _kes
        );
        account.Credit(new Money(100m, _kes));

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            account.Deactivate("Customer request"));
        Assert.Contains("non-zero balance", exception.Message);
    }

    [Fact]
    public void Account_Freeze_And_Unfreeze_ShouldToggleStatus()
    {
        // Arrange
        var account = new Account(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new AccountNumber("1234567890"),
            _kes
        );

        // Act
        account.Freeze();
        Assert.True(account.IsFrozen);

        account.Unfreeze();
        Assert.False(account.IsFrozen);
    }
}
