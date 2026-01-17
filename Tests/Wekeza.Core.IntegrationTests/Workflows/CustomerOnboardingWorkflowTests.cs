using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Wekeza.Core.Application.Features.CIF.Commands.CreateIndividualParty;
using Wekeza.Core.Application.Features.CIF.Commands.UpdateKYCStatus;
using Wekeza.Core.Application.Features.Accounts.Commands.OpenAccount;
using Wekeza.Core.Application.Features.Transactions.Commands.DepositFunds;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.IntegrationTests.TestFixtures;
using MediatR;
using Xunit;

namespace Wekeza.Core.IntegrationTests.Workflows;

/// <summary>
/// Integration tests for complete customer onboarding workflow
/// Tests: CIF creation → KYC verification → Account opening → First transaction
/// </summary>
public class CustomerOnboardingWorkflowTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly IMediator _mediator;

    public CustomerOnboardingWorkflowTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _mediator = _fixture.ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task CompleteCustomerOnboarding_ShouldSucceed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        // Act & Assert - Step 1: Create Individual Party (CIF)
        var createPartyCommand = new CreateIndividualPartyCommand
        {
            PartyId = customerId,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            NationalId = "12345678",
            Email = "john.doe@example.com",
            PhoneNumber = "+254700123456",
            Address = "123 Main Street, Nairobi"
        };

        var partyResult = await _mediator.Send(createPartyCommand);
        partyResult.Should().NotBeNull();
        partyResult.IsSuccess.Should().BeTrue();

        // Step 2: Update KYC Status
        var updateKycCommand = new UpdateKYCStatusCommand
        {
            PartyId = customerId,
            KYCStatus = KYCStatus.Verified,
            VerifiedBy = "SYSTEM_TEST",
            VerificationDate = DateTime.UtcNow,
            Comments = "Integration test verification"
        };

        var kycResult = await _mediator.Send(updateKycCommand);
        kycResult.Should().NotBeNull();
        kycResult.IsSuccess.Should().BeTrue();

        // Step 3: Open Account
        var openAccountCommand = new OpenAccountCommand
        {
            AccountId = accountId,
            CustomerId = customerId,
            ProductId = Guid.NewGuid(), // Assume product exists
            AccountType = AccountType.Savings,
            Currency = "KES",
            InitialDeposit = 1000.00m,
            BranchCode = "001"
        };

        var accountResult = await _mediator.Send(openAccountCommand);
        accountResult.Should().NotBeNull();
        accountResult.IsSuccess.Should().BeTrue();

        // Step 4: First Transaction (Deposit)
        var depositCommand = new DepositFundsCommand
        {
            AccountId = accountId,
            Amount = new Money(5000.00m, Currency.KES),
            TransactionType = TransactionType.CashDeposit,
            Description = "Initial deposit - integration test",
            Reference = "TEST_DEP_001"
        };

        var depositResult = await _mediator.Send(depositCommand);
        depositResult.Should().NotBeNull();
        depositResult.IsSuccess.Should().BeTrue();

        // Verify final state
        var account = await _fixture.Context.Accounts.FindAsync(accountId);
        account.Should().NotBeNull();
        account!.Balance.Amount.Should().Be(6000.00m); // Initial + Deposit
        account.Status.Should().Be(AccountStatus.Active);
    }

    [Fact]
    public async Task CustomerOnboarding_WithInvalidKYC_ShouldFail()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act & Assert - Create party but don't verify KYC
        var createPartyCommand = new CreateIndividualPartyCommand
        {
            PartyId = customerId,
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1985, 5, 15),
            NationalId = "87654321",
            Email = "jane.smith@example.com",
            PhoneNumber = "+254700654321",
            Address = "456 Oak Avenue, Mombasa"
        };

        var partyResult = await _mediator.Send(createPartyCommand);
        partyResult.IsSuccess.Should().BeTrue();

        // Try to open account without KYC verification
        var openAccountCommand = new OpenAccountCommand
        {
            AccountId = Guid.NewGuid(),
            CustomerId = customerId,
            ProductId = Guid.NewGuid(),
            AccountType = AccountType.Current,
            Currency = "KES",
            InitialDeposit = 1000.00m,
            BranchCode = "002"
        };

        // This should fail due to KYC requirements
        var accountResult = await _mediator.Send(openAccountCommand);
        accountResult.IsSuccess.Should().BeFalse();
        accountResult.Error.Should().Contain("KYC");
    }

    [Theory]
    [InlineData(AccountType.Savings, "KES", 500.00)]
    [InlineData(AccountType.Current, "USD", 100.00)]
    [InlineData(AccountType.FixedDeposit, "EUR", 1000.00)]
    public async Task CustomerOnboarding_MultipleAccountTypes_ShouldSucceed(
        AccountType accountType, string currency, decimal initialDeposit)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        // Complete onboarding flow for different account types
        await CreateVerifiedCustomer(customerId);

        // Act
        var openAccountCommand = new OpenAccountCommand
        {
            AccountId = accountId,
            CustomerId = customerId,
            ProductId = Guid.NewGuid(),
            AccountType = accountType,
            Currency = currency,
            InitialDeposit = initialDeposit,
            BranchCode = "003"
        };

        var result = await _mediator.Send(openAccountCommand);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var account = await _fixture.Context.Accounts.FindAsync(accountId);
        account.Should().NotBeNull();
        account!.AccountType.Should().Be(accountType);
        account.Currency.Code.Should().Be(currency);
        account.Balance.Amount.Should().Be(initialDeposit);
    }

    private async Task CreateVerifiedCustomer(Guid customerId)
    {
        // Create party
        var createPartyCommand = new CreateIndividualPartyCommand
        {
            PartyId = customerId,
            FirstName = "Test",
            LastName = "Customer",
            DateOfBirth = new DateTime(1980, 1, 1),
            NationalId = Guid.NewGuid().ToString()[..8],
            Email = $"test.{customerId:N}@example.com",
            PhoneNumber = "+254700000000",
            Address = "Test Address"
        };

        await _mediator.Send(createPartyCommand);

        // Verify KYC
        var updateKycCommand = new UpdateKYCStatusCommand
        {
            PartyId = customerId,
            KYCStatus = KYCStatus.Verified,
            VerifiedBy = "SYSTEM_TEST",
            VerificationDate = DateTime.UtcNow,
            Comments = "Test verification"
        };

        await _mediator.Send(updateKycCommand);
    }
}