using FsCheck;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;
using Wekeza.MVP4._0.Services;
using Xunit;

namespace Wekeza.MVP4._0.Tests;

/// <summary>
/// Property-based tests for Back Office Officer services
/// Feature: banking-workflow-roles, Property 3: Data Validation and Business Rules (Account Operations)
/// Validates: Requirements 1.1, 1.2, 1.3, 1.4
/// </summary>
public class BackOfficePropertyTests : IDisposable
{
    private readonly MVP4DbContext _context;
    private readonly Mock<IRBACService> _mockRbacService;
    private readonly Mock<IMakerCheckerService> _mockMakerCheckerService;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<ILogger<BackOfficeService>> _mockLogger;
    private readonly BackOfficeService _backOfficeService;

    public BackOfficePropertyTests()
    {
        var options = new DbContextOptionsBuilder<MVP4DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MVP4DbContext(options);
        _mockRbacService = new Mock<IRBACService>();
        _mockMakerCheckerService = new Mock<IMakerCheckerService>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockLogger = new Mock<ILogger<BackOfficeService>>();

        _backOfficeService = new BackOfficeService(
            _context,
            _mockRbacService.Object,
            _mockMakerCheckerService.Object,
            _mockNotificationService.Object,
            _mockLogger.Object);

        SeedTestData();
        SetupMocks();
    }

    private void SeedTestData()
    {
        // Seed test customers
        var customers = new List<Customer>
        {
            new Customer
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CustomerNumber = "CUST001",
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                IdNumber = "ID001",
                IdType = "Passport",
                KycStatus = "Compliant",
                CustomerStatus = "Active"
            },
            new Customer
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CustomerNumber = "CUST002",
                FullName = "Jane Smith",
                Email = "jane.smith@example.com",
                PhoneNumber = "+1234567891",
                IdNumber = "ID002",
                IdType = "NationalID",
                KycStatus = "Compliant",
                CustomerStatus = "Active"
            }
        };

        _context.Customers.AddRange(customers);

        // Seed test accounts
        var accounts = new List<Account>
        {
            new Account
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                AccountNumber = "ACC001",
                AccountName = "John Doe Savings",
                CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                AccountType = "Savings",
                Balance = 5000,
                AvailableBalance = 5000,
                Currency = "USD",
                Status = "Active",
                BranchCode = "BR001"
            },
            new Account
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                AccountNumber = "ACC002",
                AccountName = "Jane Smith Current",
                CustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                AccountType = "Current",
                Balance = 10000,
                AvailableBalance = 10000,
                Currency = "USD",
                Status = "Active",
                BranchCode = "BR001"
            }
        };

        _context.Accounts.AddRange(accounts);
        _context.SaveChanges();
    }

    private void SetupMocks()
    {
        // Setup RBAC service mock
        _mockRbacService.Setup(x => x.AuthorizeActionAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Setup MakerChecker service mock
        _mockMakerCheckerService.Setup(x => x.InitiateMakerActionAsync(It.IsAny<MakerAction>()))
            .ReturnsAsync(new WorkflowInstance
            {
                WorkflowId = Guid.NewGuid(),
                Status = "Pending",
                InitiatedAt = DateTime.UtcNow
            });

        // Setup Notification service mock
        _mockNotificationService.Setup(x => x.SendApprovalNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    /// <summary>
    /// Property: Account Creation Data Validation
    /// For any valid account creation request, the system should validate all required information 
    /// and create the account with proper authorization workflow
    /// </summary>
    [Property]
    [Trait("Feature", "banking-workflow-roles")]
    [Trait("Property", "3: Data Validation and Business Rules (Account Operations)")]
    public void AccountCreation_ShouldValidateDataAndCreateAccount(NonEmptyString accountType, PositiveInt initialDeposit)
    {
        // Arrange
        var validAccountTypes = new[] { "Savings", "Current", "Corporate" };
        var normalizedAccountType = validAccountTypes[Math.Abs(accountType.Get.GetHashCode()) % validAccountTypes.Length];
        
        // Ensure minimum deposit based on account type
        var minimumDeposit = normalizedAccountType.ToLower() switch
        {
            "savings" => 100,
            "current" => 1000,
            "corporate" => 10000,
            _ => 100
        };
        var depositAmount = Math.Max(Math.Min(initialDeposit.Get, 1000000), minimumDeposit); // Ensure minimum deposit and cap at reasonable amount

        var request = new AccountCreationRequest
        {
            CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            AccountType = normalizedAccountType,
            InitialDeposit = depositAmount,
            BranchCode = "BR001",
            Currency = "USD",
            SignatoryRules = new List<SignatoryRuleRequest>
            {
                new SignatoryRuleRequest
                {
                    SignatoryType = "Single",
                    MinimumSignatures = 1,
                    AuthorizedSignatories = new List<Guid> { Guid.Parse("11111111-1111-1111-1111-111111111111") }
                }
            }
        };

        // Act
        var result = _backOfficeService.CreateCustomerAccountAsync(request).Result;

        // Assert
        Assert.True(result.Success, $"Account creation should succeed for valid data. Error: {string.Join(", ", result.Errors)}");
        Assert.NotNull(result.AccountId);
        Assert.False(string.IsNullOrEmpty(result.AccountNumber));
        
        // Verify account was created in database
        var createdAccount = _context.Accounts.FirstOrDefault(a => a.Id == result.AccountId);
        Assert.NotNull(createdAccount);
        Assert.Equal(normalizedAccountType, createdAccount.AccountType);
        Assert.Equal(depositAmount, createdAccount.Balance);
        Assert.Equal("USD", createdAccount.Currency);
    }

    /// <summary>
    /// Property: Account Creation Business Rules Enforcement
    /// For any account creation request that violates business rules, the system should 
    /// prevent creation and provide clear error messages
    /// </summary>
    [Property]
    [Trait("Feature", "banking-workflow-roles")]
    [Trait("Property", "3: Data Validation and Business Rules (Account Operations)")]
    public void AccountCreation_ShouldEnforceBusinessRules(NonEmptyString accountType)
    {
        // Arrange - Create request with invalid customer ID
        var request = new AccountCreationRequest
        {
            CustomerId = Guid.NewGuid(), // Non-existent customer
            AccountType = accountType.Get,
            InitialDeposit = 100,
            BranchCode = "BR001",
            Currency = "USD",
            SignatoryRules = new List<SignatoryRuleRequest>()
        };

        // Act
        var result = _backOfficeService.CreateCustomerAccountAsync(request).Result;

        // Assert
        Assert.False(result.Success, "Account creation should fail for non-existent customer");
        Assert.Contains("Customer not found", result.Message);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Property: Account Verification Process Integrity
    /// For any account verification request, the system should validate KYC compliance 
    /// and route for appropriate approval based on verification data
    /// </summary>
    [Property]
    [Trait("Feature", "banking-workflow-roles")]
    [Trait("Property", "3: Data Validation and Business Rules (Account Operations)")]
    public void AccountVerification_ShouldValidateKYCAndRouteApproval(bool kycCompliant, bool requiresManagerApproval)
    {
        // Arrange
        var accountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var verificationData = new VerificationData
        {
            KYCCompliant = kycCompliant,
            DocumentsVerified = kycCompliant ? new List<string> { "Passport", "ProofOfAddress" } : new List<string>(),
            VerificationNotes = "Test verification",
            RequiresManagerApproval = requiresManagerApproval,
            ComplianceChecks = new Dictionary<string, bool>
            {
                { "AMLCheck", kycCompliant },
                { "SanctionsCheck", kycCompliant }
            }
        };

        // Act
        var result = _backOfficeService.VerifyAccountOpeningAsync(accountId, verificationData).Result;

        // Assert
        Assert.True(result.Success, "Verification process should complete successfully");
        
        if (kycCompliant && !requiresManagerApproval)
        {
            Assert.True(result.Approved, "Account should be approved when KYC compliant and no manager approval required");
            Assert.Equal("Approved", result.Status);
        }
        else
        {
            Assert.False(result.Approved, "Account should require approval when KYC non-compliant or manager approval required");
            Assert.Equal("PendingManagerApproval", result.Status);
            Assert.NotNull(result.WorkflowId);
        }
    }

    /// <summary>
    /// Property: Account Closure Validation
    /// For any account closure request, the system should validate closure conditions 
    /// (zero balance, no pending transactions) before processing
    /// </summary>
    [Property]
    [Trait("Feature", "banking-workflow-roles")]
    [Trait("Property", "3: Data Validation and Business Rules (Account Operations)")]
    public void AccountClosure_ShouldValidateClosureConditions(NonNegativeInt balance)
    {
        // Arrange
        var accountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var account = _context.Accounts.Find(accountId);
        account!.Balance = balance.Get;
        account.AvailableBalance = balance.Get;
        _context.SaveChanges();

        var requestedBy = Guid.NewGuid();
        var closureReason = "Customer request";

        // Act
        var result = _backOfficeService.ProcessAccountClosureAsync(accountId, closureReason, requestedBy).Result;

        // Assert
        if (balance.Get == 0)
        {
            Assert.True(result.Success, "Account closure should succeed when balance is zero");
            Assert.Equal("PendingClosure", result.Status);
            Assert.NotNull(result.WorkflowId);
        }
        else
        {
            Assert.False(result.Success, "Account closure should fail when balance is not zero");
            Assert.Contains("balance must be zero", result.Message);
        }
    }

    /// <summary>
    /// Property: Signatory Rules Validation
    /// For any signatory rule update, the system should validate rule consistency 
    /// and enforce maker-checker workflow for high-risk changes
    /// </summary>
    [Property]
    [Trait("Feature", "banking-workflow-roles")]
    [Trait("Property", "3: Data Validation and Business Rules (Account Operations)")]
    public void SignatoryRulesUpdate_ShouldValidateRulesAndEnforceWorkflow(PositiveInt minSignatures)
    {
        // Arrange
        var accountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var requestedBy = Guid.NewGuid();
        var normalizedMinSignatures = Math.Min(minSignatures.Get, 5); // Cap at reasonable number

        var signatoryRules = new List<SignatoryRuleRequest>
        {
            new SignatoryRuleRequest
            {
                SignatoryType = "Joint",
                MinimumSignatures = normalizedMinSignatures,
                MaximumAmount = 50000,
                AuthorizedSignatories = Enumerable.Range(0, Math.Max(normalizedMinSignatures, 2))
                    .Select(_ => Guid.NewGuid()).ToList()
            }
        };

        // Act
        var result = _backOfficeService.UpdateSignatoryRulesAsync(accountId, signatoryRules, requestedBy).Result;

        // Assert
        Assert.True(result.Success, "Signatory rule update should succeed for valid rules");
        Assert.True(result.RequiresApproval, "Signatory rule updates should require approval (high-risk change)");
        Assert.NotNull(result.WorkflowId);
        Assert.Contains("approval", result.Message.ToLower());
    }

    /// <summary>
    /// Property: Internal Transfer Validation
    /// For any internal transfer request, the system should validate account balances 
    /// and enforce approval workflows based on amount thresholds
    /// </summary>
    [Property]
    [Trait("Feature", "banking-workflow-roles")]
    [Trait("Property", "3: Data Validation and Business Rules (Account Operations)")]
    public void InternalTransfer_ShouldValidateBalanceAndEnforceApprovalLimits(PositiveInt transferAmount)
    {
        // Arrange
        var fromAccountId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var toAccountId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var initiatedBy = Guid.NewGuid();
        var normalizedAmount = Math.Min(transferAmount.Get, 100000); // Cap at reasonable amount

        var transferRequest = new InternalTransferRequest
        {
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = normalizedAmount,
            Currency = "USD",
            TransactionReference = "TEST_TRANSFER",
            Narration = "Test transfer",
            ValueDate = DateTime.UtcNow
        };

        // Act
        var result = _backOfficeService.PostInternalTransferAsync(transferRequest, initiatedBy).Result;

        // Assert
        var fromAccount = _context.Accounts.Find(fromAccountId);
        
        if (normalizedAmount <= fromAccount!.AvailableBalance)
        {
            Assert.True(result.Success, "Transfer should succeed when sufficient balance available");
            Assert.False(string.IsNullOrEmpty(result.TransactionReference));
            
            if (normalizedAmount > 50000)
            {
                Assert.Equal("PendingApproval", result.Status);
                Assert.NotNull(result.WorkflowId);
            }
            else
            {
                Assert.Equal("Completed", result.Status);
                Assert.NotNull(result.TransactionId);
            }
        }
        else
        {
            Assert.False(result.Success, "Transfer should fail when insufficient balance");
            Assert.Contains("Insufficient funds", result.Message);
        }
    }

    /// <summary>
    /// Property: KYC Update Processing Integrity
    /// For any KYC update request, the system should validate document completeness 
    /// and update customer compliance status appropriately
    /// </summary>
    [Property]
    [Trait("Feature", "banking-workflow-roles")]
    [Trait("Property", "3: Data Validation and Business Rules (Account Operations)")]
    public void KYCUpdate_ShouldValidateDocumentsAndUpdateComplianceStatus(bool hasDocuments, bool allChecksPass)
    {
        // Arrange
        var customerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var processedBy = Guid.NewGuid();

        var kycData = new KYCUpdateRequest
        {
            Documents = hasDocuments ? new List<DocumentUploadRequest>
            {
                new DocumentUploadRequest
                {
                    DocumentType = "Passport",
                    FileName = "passport.pdf",
                    FilePath = "/documents/passport.pdf",
                    DocumentStatus = "Verified"
                }
            } : new List<DocumentUploadRequest>(),
            RiskRating = allChecksPass ? "Low" : "High",
            ComplianceChecks = new List<ComplianceCheckRequest>
            {
                new ComplianceCheckRequest
                {
                    CheckType = "AMLCheck",
                    Result = allChecksPass ? "Pass" : "Fail",
                    CheckedDate = DateTime.UtcNow
                }
            },
            RequiresManagerReview = !allChecksPass
        };

        // Act
        var result = _backOfficeService.ProcessKYCUpdateAsync(customerId, kycData, processedBy).Result;

        // Assert
        Assert.True(result.Success, "KYC update should complete successfully");
        
        if (hasDocuments && allChecksPass)
        {
            Assert.Equal("Compliant", result.KYCStatus);
            Assert.Empty(result.Issues);
        }
        else
        {
            Assert.Equal("NonCompliant", result.KYCStatus);
            Assert.NotEmpty(result.Issues);
            Assert.True(result.RequiresManagerReview);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}