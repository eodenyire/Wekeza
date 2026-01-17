using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Common;
using Xunit;

namespace Wekeza.Core.UnitTests.Domain.Aggregates;

/// <summary>
/// Unit tests for Loan aggregate
/// </summary>
public class LoanTests
{
    [Fact]
    public void Loan_Creation_ShouldInitializeWithAppliedStatus()
    {
        // Arrange & Act
        var loan = new Loan(
            customerId: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            principal: 100000m,
            interestRate: 12.5m,
            termInMonths: 12
        );

        // Assert
        Assert.Equal(LoanStatus.Applied, loan.Status);
        Assert.Equal(100000m, loan.Principal);
        Assert.Equal(12.5m, loan.InterestRate);
        Assert.Equal(12, loan.TermInMonths);
    }

    [Fact]
    public void Loan_Approve_ShouldChangeStatusAndGenerateSchedule()
    {
        // Arrange
        var loan = new Loan(
            customerId: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            principal: 100000m,
            interestRate: 12m,
            termInMonths: 12
        );

        // Act
        loan.Approve("John Doe");

        // Assert
        Assert.Equal(LoanStatus.Approved, loan.Status);
        Assert.NotNull(loan.ApprovedDate);
        Assert.Equal("John Doe", loan.ApprovedBy);
        Assert.Equal(12, loan.RepaymentSchedule.Count);
    }

    [Fact]
    public void Loan_Approve_WhenNotApplied_ShouldThrowException()
    {
        // Arrange
        var loan = new Loan(
            customerId: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            principal: 100000m,
            interestRate: 12m,
            termInMonths: 12
        );
        loan.Approve("John Doe");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loan.Approve("Jane Doe"));
        Assert.Contains("Only applied loans can be approved", exception.Message);
    }

    [Fact]
    public void Loan_Disburse_ShouldChangeStatusToActive()
    {
        // Arrange
        var loan = new Loan(
            customerId: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            principal: 100000m,
            interestRate: 12m,
            termInMonths: 12
        );
        loan.Approve("John Doe");

        // Act
        loan.Disburse();

        // Assert
        Assert.Equal(LoanStatus.Active, loan.Status);
    }

    [Fact]
    public void Loan_Disburse_WhenNotApproved_ShouldThrowException()
    {
        // Arrange
        var loan = new Loan(
            customerId: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            principal: 100000m,
            interestRate: 12m,
            termInMonths: 12
        );

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loan.Disburse());
        Assert.Contains("must be approved before disbursal", exception.Message);
    }

    [Fact]
    public void Loan_ApplyPayment_ShouldReduceRemainingBalance()
    {
        // Arrange
        var loan = new Loan(
            customerId: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            principal: 100000m,
            interestRate: 12m,
            termInMonths: 12
        );
        loan.Approve("John Doe");
        loan.Disburse();
        var initialBalance = loan.RemainingBalance;

        // Act
        loan.ApplyPayment(10000m);

        // Assert
        Assert.True(loan.RemainingBalance < initialBalance);
    }

    [Fact]
    public void Loan_ApplyPayment_WhenNotActive_ShouldThrowException()
    {
        // Arrange
        var loan = new Loan(
            customerId: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            principal: 100000m,
            interestRate: 12m,
            termInMonths: 12
        );

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loan.ApplyPayment(10000m));
        Assert.Contains("Can only make payments on active loans", exception.Message);
    }

    [Fact]
    public void Loan_ApplyPayment_ExceedingBalance_ShouldThrowException()
    {
        // Arrange
        var loan = new Loan(
            customerId: Guid.NewGuid(),
            accountId: Guid.NewGuid(),
            principal: 100000m,
            interestRate: 12m,
            termInMonths: 12
        );
        loan.Approve("John Doe");
        loan.Disburse();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            loan.ApplyPayment(loan.RemainingBalance + 1000m));
        Assert.Contains("Payment exceeds remaining balance", exception.Message);
    }
}
