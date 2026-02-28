using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Wekeza.Core.Application.Features.Loans.Commands.ApplyForLoan;
using Wekeza.Core.Application.Features.Loans.Commands.ApproveLoan;
using Wekeza.Core.Application.Features.Loans.Commands.DisburseLoan;
using Wekeza.Core.Application.Features.Loans.Commands.ProcessRepayment;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.IntegrationTests.TestFixtures;
using MediatR;
using Xunit;

namespace Wekeza.Core.IntegrationTests.Workflows;

/// <summary>
/// Integration tests for complete loan processing workflow
/// Tests: Application → Credit scoring → Approval → Disbursement → Repayment
/// </summary>
public class LoanProcessingWorkflowTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly IMediator _mediator;

    public LoanProcessingWorkflowTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _mediator = _fixture.ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task CompleteLoanProcessing_ShouldSucceed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var loanId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        // Setup customer and account first
        await SetupCustomerWithAccount(customerId, accountId);

        // Act & Assert - Step 1: Apply for Loan
        var applyCommand = new ApplyForLoanCommand
        {
            LoanId = loanId,
            CustomerId = customerId,
            LoanType = LoanType.Personal,
            RequestedAmount = new Money(100000.00m, Currency.KES),
            Purpose = "Home improvement",
            Term = 24, // 24 months
            RequestedInterestRate = new InterestRate(12.5m, InterestRateType.Annual)
        };

        var applicationResult = await _mediator.Send(applyCommand);
        applicationResult.Should().NotBeNull();
        applicationResult.IsSuccess.Should().BeTrue();

        // Step 2: Approve Loan
        var approveCommand = new ApproveLoanCommand
        {
            LoanId = loanId,
            ApprovedAmount = new Money(80000.00m, Currency.KES), // Approved for less
            ApprovedInterestRate = new InterestRate(13.0m, InterestRateType.Annual),
            ApprovedTerm = 24,
            ApprovedBy = "LOAN_OFFICER_TEST",
            ApprovalComments = "Good credit history, approved with conditions"
        };

        var approvalResult = await _mediator.Send(approveCommand);
        approvalResult.Should().NotBeNull();
        approvalResult.IsSuccess.Should().BeTrue();

        // Step 3: Disburse Loan
        var disburseCommand = new DisburseLoanCommand
        {
            LoanId = loanId,
            DisbursementAccountId = accountId,
            DisbursementAmount = new Money(80000.00m, Currency.KES),
            DisbursedBy = "SYSTEM_TEST",
            DisbursementMethod = DisbursementMethod.AccountCredit
        };

        var disbursementResult = await _mediator.Send(disburseCommand);
        disbursementResult.Should().NotBeNull();
        disbursementResult.IsSuccess.Should().BeTrue();

        // Step 4: Process Repayment
        var repaymentCommand = new ProcessRepaymentCommand
        {
            LoanId = loanId,
            PaymentAmount = new Money(5000.00m, Currency.KES),
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = PaymentMethod.BankTransfer,
            Reference = "REPAY_001"
        };

        var repaymentResult = await _mediator.Send(repaymentCommand);
        repaymentResult.Should().NotBeNull();
        repaymentResult.IsSuccess.Should().BeTrue();

        // Verify final state
        var loan = await _fixture.Context.Loans.FindAsync(loanId);
        loan.Should().NotBeNull();
        loan!.Status.Should().Be(LoanStatus.Active);
        loan.DisbursedAmount.Amount.Should().Be(80000.00m);
        loan.OutstandingBalance.Amount.Should().BeLessThan(80000.00m); // Reduced by repayment
    }

    [Fact]
    public async Task LoanApplication_InsufficientCreditScore_ShouldBeRejected()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var loanId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        await SetupCustomerWithAccount(customerId, accountId);

        // Act - Apply for high-risk loan
        var applyCommand = new ApplyForLoanCommand
        {
            LoanId = loanId,
            CustomerId = customerId,
            LoanType = LoanType.Personal,
            RequestedAmount = new Money(1000000.00m, Currency.KES), // Very high amount
            Purpose = "Speculative investment",
            Term = 12,
            RequestedInterestRate = new InterestRate(8.0m, InterestRateType.Annual) // Unrealistic rate
        };

        var applicationResult = await _mediator.Send(applyCommand);
        applicationResult.IsSuccess.Should().BeTrue(); // Application accepted

        // Try to approve - should fail credit assessment
        var approveCommand = new ApproveLoanCommand
        {
            LoanId = loanId,
            ApprovedAmount = new Money(1000000.00m, Currency.KES),
            ApprovedInterestRate = new InterestRate(8.0m, InterestRateType.Annual),
            ApprovedTerm = 12,
            ApprovedBy = "LOAN_OFFICER_TEST",
            ApprovalComments = "High risk application"
        };

        var approvalResult = await _mediator.Send(approveCommand);
        approvalResult.IsSuccess.Should().BeFalse();
        approvalResult.Error.Should().Contain("credit");
    }

    [Theory]
    [InlineData(LoanType.Personal, 50000.00, 12)]
    [InlineData(LoanType.Mortgage, 2000000.00, 240)]
    [InlineData(LoanType.Auto, 800000.00, 60)]
    [InlineData(LoanType.Business, 500000.00, 36)]
    public async Task LoanProcessing_DifferentLoanTypes_ShouldSucceed(
        LoanType loanType, decimal amount, int termMonths)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var loanId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        await SetupCustomerWithAccount(customerId, accountId);

        // Act - Apply for different loan types
        var applyCommand = new ApplyForLoanCommand
        {
            LoanId = loanId,
            CustomerId = customerId,
            LoanType = loanType,
            RequestedAmount = new Money(amount, Currency.KES),
            Purpose = $"{loanType} loan purpose",
            Term = termMonths,
            RequestedInterestRate = new InterestRate(15.0m, InterestRateType.Annual)
        };

        var applicationResult = await _mediator.Send(applyCommand);

        // Assert
        applicationResult.IsSuccess.Should().BeTrue();
        
        var loan = await _fixture.Context.Loans.FindAsync(loanId);
        loan.Should().NotBeNull();
        loan!.LoanType.Should().Be(loanType);
        loan.RequestedAmount.Amount.Should().Be(amount);
        loan.Term.Should().Be(termMonths);
    }

    [Fact]
    public async Task LoanRepayment_EarlyFullPayment_ShouldCloseAccount()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var loanId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        await SetupCustomerWithAccount(customerId, accountId);
        await ProcessLoanToActive(loanId, customerId, accountId);

        // Act - Full early repayment
        var loan = await _fixture.Context.Loans.FindAsync(loanId);
        var fullAmount = loan!.OutstandingBalance;

        var repaymentCommand = new ProcessRepaymentCommand
        {
            LoanId = loanId,
            PaymentAmount = fullAmount,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = PaymentMethod.BankTransfer,
            Reference = "FULL_REPAY_001"
        };

        var repaymentResult = await _mediator.Send(repaymentCommand);

        // Assert
        repaymentResult.IsSuccess.Should().BeTrue();
        
        var updatedLoan = await _fixture.Context.Loans.FindAsync(loanId);
        updatedLoan!.Status.Should().Be(LoanStatus.Closed);
        updatedLoan.OutstandingBalance.Amount.Should().Be(0);
    }

    private async Task SetupCustomerWithAccount(Guid customerId, Guid accountId)
    {
        // This would typically involve creating customer and account
        // For now, we'll assume they exist or create minimal test data
        // In a real implementation, this would call the customer onboarding workflow
    }

    private async Task ProcessLoanToActive(Guid loanId, Guid customerId, Guid accountId)
    {
        // Apply
        var applyCommand = new ApplyForLoanCommand
        {
            LoanId = loanId,
            CustomerId = customerId,
            LoanType = LoanType.Personal,
            RequestedAmount = new Money(50000.00m, Currency.KES),
            Purpose = "Test loan",
            Term = 12,
            RequestedInterestRate = new InterestRate(15.0m, InterestRateType.Annual)
        };
        await _mediator.Send(applyCommand);

        // Approve
        var approveCommand = new ApproveLoanCommand
        {
            LoanId = loanId,
            ApprovedAmount = new Money(50000.00m, Currency.KES),
            ApprovedInterestRate = new InterestRate(15.0m, InterestRateType.Annual),
            ApprovedTerm = 12,
            ApprovedBy = "TEST_OFFICER",
            ApprovalComments = "Test approval"
        };
        await _mediator.Send(approveCommand);

        // Disburse
        var disburseCommand = new DisburseLoanCommand
        {
            LoanId = loanId,
            DisbursementAccountId = accountId,
            DisbursementAmount = new Money(50000.00m, Currency.KES),
            DisbursedBy = "SYSTEM_TEST",
            DisbursementMethod = DisbursementMethod.AccountCredit
        };
        await _mediator.Send(disburseCommand);
    }
}