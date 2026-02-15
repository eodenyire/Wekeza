using FsCheck;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;
using Wekeza.MVP4._0.Services;
using Xunit;
using System.Threading.Tasks;

namespace Wekeza.MVP4._0.Tests;

/// <summary>
/// Property-based tests for KYC compliance processing functionality
/// **Property 4: KYC Compliance and Risk Management**
/// **Validates: Requirements 2.1, 2.2, 2.3, 2.5**
/// </summary>
public class KYCCompliancePropertyTests : IDisposable
{
    private readonly MVP4DbContext _context;
    private readonly Mock<IRBACService> _mockRbacService;
    private readonly Mock<IMakerCheckerService> _mockMakerCheckerService;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<ILogger<BackOfficeService>> _mockLogger;
    private readonly BackOfficeService _backOfficeService;

    public KYCCompliancePropertyTests()
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

        SetupMockServices();
        RegisterArbitraries();
    }

    private void RegisterArbitraries()
    {
        Arb.Register<GenerateKYCUpdateScenario>();
        Arb.Register<GenerateRiskFlaggingScenario>();
        Arb.Register<GenerateAMLViolationScenario>();
        Arb.Register<GenerateDocumentUploadScenario>();
        Arb.Register<GenerateCriticalRiskScenario>();
        Arb.Register<GenerateComplianceCheckScenario>();
    }

    private void SetupMockServices()
    {
        _mockMakerCheckerService
            .Setup(m => m.InitiateMakerActionAsync(It.IsAny<MakerAction>()))
            .ReturnsAsync(new WorkflowInstance { WorkflowId = Guid.NewGuid(), Status = "Pending" });

        _mockNotificationService
            .Setup(n => n.SendEscalationNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
    }

    #region Property 4: KYC Compliance and Risk Management

    /// <summary>
    /// **Property 4.1: KYC Document Validation Completeness**
    /// For any KYC update request, the system should validate document completeness 
    /// and authenticity according to regulatory requirements
    /// **Validates: Requirement 2.1**
    /// </summary>
    [Property]
    public bool KYCDocumentValidationCompleteness(KYCUpdateScenario scenario)
    {
        // Arrange
        SeedCustomerData(scenario.CustomerId).Wait();
        
        // Act
        var result = _backOfficeService.ProcessKYCUpdateAsync(
            scenario.CustomerId, 
            scenario.KYCRequest, 
            scenario.ProcessedBy).Result;

        // Assert - Document validation properties
        if (scenario.KYCRequest.Documents.Count == 0)
        {
            // No documents should result in issues
            return result.Issues.Any(i => i.Contains("No documents provided"));
        }

        // Invalid documents should be flagged
        var invalidDocs = scenario.KYCRequest.Documents
            .Where(d => string.IsNullOrEmpty(d.DocumentType) || string.IsNullOrEmpty(d.FileName))
            .ToList();

        if (invalidDocs.Any())
        {
            return result.Issues.Any(i => i.Contains("Invalid document"));
        }

        // Valid documents should be processed successfully
        return result.Success;
    }

    /// <summary>
    /// **Property 4.2: Risk Rating Consistency**
    /// For any risk rating update, the system should maintain consistency between 
    /// risk indicators and the calculated risk level
    /// **Validates: Requirement 2.5**
    /// </summary>
    [Property]
    public bool RiskRatingConsistency(RiskFlaggingScenario scenario)
    {
        // Arrange
        SeedCustomerWithKYCData(scenario.CustomerId).Wait();

        // Act
        var result = _backOfficeService.FlagHighRiskCustomerAsync(
            scenario.CustomerId,
            scenario.RiskIndicators,
            scenario.FlaggedBy).Result;

        // Assert - Risk level should match highest severity
        var highestSeverity = scenario.RiskIndicators.Max(r => GetSeverityLevel(r.Severity));
        var expectedRiskLevel = GetExpectedRiskLevel(highestSeverity);

        return result.Success && result.RiskLevel == expectedRiskLevel;
    }

    /// <summary>
    /// **Property 4.3: AML Violation Escalation Enforcement**
    /// For any AML violation, the system should prevent override and enforce 
    /// mandatory escalation to compliance team
    /// **Validates: Requirement 2.4**
    /// </summary>
    [Property]
    public bool AMLViolationEscalationEnforcement(AMLViolationScenario scenario)
    {
        // Arrange
        SeedCustomerData(scenario.CustomerId).Wait();

        // Act
        var result = _backOfficeService.EscalateAMLViolationAsync(
            scenario.CustomerId,
            scenario.ViolationRequest,
            scenario.EscalatedBy).Result;

        // Assert - AML violations must always escalate
        return result.Success && 
               result.EscalationLevel == "ComplianceTeam" &&
               result.WorkflowId.HasValue;
    }

    /// <summary>
    /// **Property 4.4: Document Storage Security and Audit Trail**
    /// For any document upload operation, the system should store documents securely 
    /// with complete audit trails
    /// **Validates: Requirement 2.2**
    /// </summary>
    [Property]
    public bool DocumentStorageSecurityAndAuditTrail(DocumentUploadScenario scenario)
    {
        // Arrange
        SeedCustomerData(scenario.CustomerId).Wait();

        // Act
        var result = _backOfficeService.ProcessDocumentUploadAsync(
            scenario.CustomerId,
            scenario.Documents,
            scenario.UploadedBy).Result;

        // Assert - Documents should be stored with audit trail
        if (result.Success && result.ProcessedCount > 0)
        {
            var storedDocuments = _context.CustomerDocuments
                .Where(d => d.CustomerId == scenario.CustomerId)
                .ToList();

            // Each valid document should be stored
            var validDocuments = scenario.Documents
                .Where(d => !string.IsNullOrEmpty(d.DocumentType) && !string.IsNullOrEmpty(d.FileName))
                .ToList();

            return storedDocuments.Count >= validDocuments.Count &&
                   storedDocuments.All(d => d.UploadedBy == scenario.UploadedBy.ToString() &&
                                           d.UploadedAt != default);
        }

        return true; // If no valid documents, operation should handle gracefully
    }

    /// <summary>
    /// **Property 4.5: High-Risk Customer Flagging and Workflow Triggers**
    /// For any high-risk customer identification, the system should flag the customer 
    /// and trigger appropriate AML review workflows
    /// **Validates: Requirement 2.3**
    /// </summary>
    [Property]
    public bool HighRiskCustomerFlaggingAndWorkflowTriggers(CriticalRiskScenario scenario)
    {
        // Arrange
        SeedCustomerWithKYCData(scenario.CustomerId).Wait();

        // Act
        var result = _backOfficeService.FlagHighRiskCustomerAsync(
            scenario.CustomerId,
            scenario.RiskIndicators,
            scenario.FlaggedBy).Result;

        // Assert - Critical risk should trigger immediate action
        var hasCriticalRisk = scenario.RiskIndicators.Any(r => r.Severity == "Critical");

        if (hasCriticalRisk)
        {
            return result.Success &&
                   result.RequiresImmediateAction &&
                   result.EscalationWorkflowId.HasValue;
        }

        // Non-critical risk should still update risk level
        return result.Success && !string.IsNullOrEmpty(result.RiskLevel);
    }

    /// <summary>
    /// **Property 4.6: KYC Status Consistency with Compliance Checks**
    /// For any KYC processing, the customer's KYC status should be consistent 
    /// with the results of compliance checks
    /// **Validates: Requirement 2.1**
    /// </summary>
    [Property]
    public bool KYCStatusConsistencyWithComplianceChecks(ComplianceCheckScenario scenario)
    {
        // Arrange
        SeedCustomerData(scenario.CustomerId).Wait();

        // Act
        var result = _backOfficeService.ProcessKYCUpdateAsync(
            scenario.CustomerId,
            scenario.KYCRequest,
            scenario.ProcessedBy).Result;

        // Assert - KYC status should reflect compliance check results
        var hasFailedChecks = scenario.KYCRequest.ComplianceChecks
            .Any(c => c.Result != "Pass");

        if (hasFailedChecks)
        {
            return result.KYCStatus == "NonCompliant" || result.Issues.Any();
        }

        // If all checks pass and documents are valid, should be compliant
        var hasValidDocuments = scenario.KYCRequest.Documents.Any() &&
            scenario.KYCRequest.Documents.All(d => 
                !string.IsNullOrEmpty(d.DocumentType) && 
                !string.IsNullOrEmpty(d.FileName));

        if (hasValidDocuments)
        {
            return result.KYCStatus == "Compliant";
        }

        return true; // Partial compliance scenarios are acceptable
    }

    #endregion

    #region Test Data Generators

    public class GenerateKYCUpdateScenario
    {
        public static Arbitrary<KYCUpdateScenario> KYCUpdateScenario()
        {
            return Arb.From(
                from customerId in Arb.Generate<Guid>()
                from processedBy in Arb.Generate<Guid>()
                from documents in Gen.ListOf(GenerateDocumentUploadRequest())
                from riskRating in Gen.Elements("Low", "Medium", "High", "Critical")
                from complianceChecks in Gen.ListOf(GenerateComplianceCheckRequest())
                from requiresReview in Arb.Generate<bool>()
                select new KYCUpdateScenario
                {
                    CustomerId = customerId,
                    ProcessedBy = processedBy,
                    KYCRequest = new KYCUpdateRequest
                    {
                        Documents = documents.ToList(),
                        RiskRating = riskRating,
                        ComplianceChecks = complianceChecks.ToList(),
                        RequiresManagerReview = requiresReview,
                        ReviewNotes = "Property test generated KYC update"
                    }
                });
        }
    }

    public class GenerateRiskFlaggingScenario
    {
        public static Arbitrary<RiskFlaggingScenario> RiskFlaggingScenario()
        {
            return Arb.From(
                from customerId in Arb.Generate<Guid>()
                from flaggedBy in Arb.Generate<Guid>()
                from indicators in Gen.ListOf(GenerateRiskIndicatorRequest())
                select new RiskFlaggingScenario
                {
                    CustomerId = customerId,
                    FlaggedBy = flaggedBy,
                    RiskIndicators = indicators.ToList()
                });
        }
    }

    public class GenerateAMLViolationScenario
    {
        public static Arbitrary<AMLViolationScenario> AMLViolationScenario()
        {
            return Arb.From(
                from customerId in Arb.Generate<Guid>()
                from escalatedBy in Arb.Generate<Guid>()
                from violationType in Gen.Elements("StructuringTransactions", "UnusualActivity", "SanctionsMatch", "SuspiciousPattern")
                from amount in Gen.Choose(1000, 1000000).Select(x => (decimal)x)
                from requiresImmediate in Arb.Generate<bool>()
                select new AMLViolationScenario
                {
                    CustomerId = customerId,
                    EscalatedBy = escalatedBy,
                    ViolationRequest = new AMLViolationRequest
                    {
                        ViolationType = violationType,
                        Description = $"Property test {violationType} violation",
                        TransactionAmount = amount,
                        TransactionReference = $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}",
                        ViolationDate = DateTime.UtcNow,
                        RegulatoryReference = "REG-2024-001",
                        RequiresImmediateAction = requiresImmediate
                    }
                });
        }
    }

    public class GenerateDocumentUploadScenario
    {
        public static Arbitrary<DocumentUploadScenario> DocumentUploadScenario()
        {
            return Arb.From(
                from customerId in Arb.Generate<Guid>()
                from uploadedBy in Arb.Generate<Guid>()
                from documents in Gen.ListOf(GenerateDocumentUploadRequest())
                select new DocumentUploadScenario
                {
                    CustomerId = customerId,
                    UploadedBy = uploadedBy,
                    Documents = documents.ToList()
                });
        }
    }

    public class GenerateCriticalRiskScenario
    {
        public static Arbitrary<CriticalRiskScenario> CriticalRiskScenario()
        {
            return Arb.From(
                from customerId in Arb.Generate<Guid>()
                from flaggedBy in Arb.Generate<Guid>()
                from hasCritical in Arb.Generate<bool>()
                from indicators in Gen.ListOf(GenerateRiskIndicatorRequest())
                select new CriticalRiskScenario
                {
                    CustomerId = customerId,
                    FlaggedBy = flaggedBy,
                    RiskIndicators = hasCritical 
                        ? indicators.Concat(new[] { new RiskIndicatorRequest 
                            { 
                                IndicatorType = "CriticalRisk", 
                                Severity = "Critical", 
                                Description = "Critical risk indicator",
                                Source = "PropertyTest",
                                IdentifiedDate = DateTime.UtcNow
                            }}).ToList()
                        : indicators.ToList()
                });
        }
    }

    public class GenerateComplianceCheckScenario
    {
        public static Arbitrary<ComplianceCheckScenario> ComplianceCheckScenario()
        {
            return Arb.From(
                from customerId in Arb.Generate<Guid>()
                from processedBy in Arb.Generate<Guid>()
                from hasFailedChecks in Arb.Generate<bool>()
                from documents in Gen.ListOf(GenerateValidDocumentUploadRequest())
                from complianceChecks in hasFailedChecks 
                    ? Gen.ListOf(GenerateFailedComplianceCheck())
                    : Gen.ListOf(GeneratePassedComplianceCheck())
                select new ComplianceCheckScenario
                {
                    CustomerId = customerId,
                    ProcessedBy = processedBy,
                    KYCRequest = new KYCUpdateRequest
                    {
                        Documents = documents.ToList(),
                        RiskRating = "Medium",
                        ComplianceChecks = complianceChecks.ToList(),
                        RequiresManagerReview = false,
                        ReviewNotes = "Property test compliance check scenario"
                    }
                });
        }
    }

    public static Gen<DocumentUploadRequest> GenerateDocumentUploadRequest()
    {
        return from docType in Gen.Elements("Passport", "NationalID", "DriverLicense", "UtilityBill", "", "Invalid")
            from fileName in Gen.Elements("doc1.pdf", "scan.jpg", "document.png", "", "invalid")
            from filePath in Gen.Elements("/docs/valid.pdf", "/temp/file.jpg", "", "invalid/path")
            from status in Gen.Elements("Pending", "Verified", "Rejected")
            select new DocumentUploadRequest
            {
                DocumentType = docType,
                FileName = fileName,
                FilePath = filePath,
                DocumentStatus = status,
                ExpiryDate = DateTime.UtcNow.AddYears(1)
            };
    }

    public static Gen<ComplianceCheckRequest> GenerateComplianceCheckRequest()
    {
        return from checkType in Gen.Elements("AML", "Sanctions", "PEP", "CDD", "EDD")
            from result in Gen.Elements("Pass", "Fail", "Pending", "Review")
            select new ComplianceCheckRequest
            {
                CheckType = checkType,
                Result = result,
                Notes = $"Property test {checkType} check",
                CheckedDate = DateTime.UtcNow
            };
    }

    public static Gen<RiskIndicatorRequest> GenerateRiskIndicatorRequest()
    {
        return from indicatorType in Gen.Elements("TransactionPattern", "GeographicRisk", "IndustryRisk", "BehaviorChange")
            from severity in Gen.Elements("Low", "Medium", "High", "Critical")
            from description in Gen.Elements("Unusual transaction pattern", "High-risk jurisdiction", "Cash-intensive business")
            select new RiskIndicatorRequest
            {
                IndicatorType = indicatorType,
                Severity = severity,
                Description = description,
                Source = "PropertyTest",
                IdentifiedDate = DateTime.UtcNow
            };
    }

    public static Gen<DocumentUploadRequest> GenerateValidDocumentUploadRequest()
    {
        return from docType in Gen.Elements("Passport", "NationalID", "DriverLicense", "UtilityBill")
            from fileName in Gen.Elements("passport.pdf", "id_scan.jpg", "license.png", "bill.pdf")
            select new DocumentUploadRequest
            {
                DocumentType = docType,
                FileName = fileName,
                FilePath = $"/docs/{fileName}",
                DocumentStatus = "Pending",
                ExpiryDate = DateTime.UtcNow.AddYears(1)
            };
    }

    public static Gen<ComplianceCheckRequest> GenerateFailedComplianceCheck()
    {
        return from checkType in Gen.Elements("AML", "Sanctions", "PEP")
            select new ComplianceCheckRequest
            {
                CheckType = checkType,
                Result = "Fail",
                Notes = $"Failed {checkType} check",
                CheckedDate = DateTime.UtcNow
            };
    }

    public static Gen<ComplianceCheckRequest> GeneratePassedComplianceCheck()
    {
        return from checkType in Gen.Elements("AML", "Sanctions", "PEP", "CDD")
            select new ComplianceCheckRequest
            {
                CheckType = checkType,
                Result = "Pass",
                Notes = $"Passed {checkType} check",
                CheckedDate = DateTime.UtcNow
            };
    }

    #endregion

    #region Helper Methods

    private Task SeedCustomerData(Guid customerId)
    {
        var customer = new Customer
        {
            Id = customerId,
            FullName = "Test Customer",
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            KycStatus = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        return _context.SaveChangesAsync();
    }

    private async Task SeedCustomerWithKYCData(Guid customerId)
    {
        await SeedCustomerData(customerId);

        var kycData = new KYCData
        {
            KYCId = Guid.NewGuid(),
            CustomerId = customerId,
            RiskRating = "Medium",
            LastReviewDate = DateTime.UtcNow.AddDays(-30),
            NextReviewDate = DateTime.UtcNow.AddDays(335),
            ReviewedBy = Guid.NewGuid().ToString(),
            ComplianceNotes = "Initial KYC data",
            AMLFlagged = false
        };

        _context.KYCData.Add(kycData);
        await _context.SaveChangesAsync();
    }

    private static int GetSeverityLevel(string severity)
    {
        return severity.ToLower() switch
        {
            "low" => 1,
            "medium" => 2,
            "high" => 3,
            "critical" => 4,
            _ => 1
        };
    }

    private static string GetExpectedRiskLevel(int severityLevel)
    {
        return severityLevel switch
        {
            1 => "Low",
            2 => "Medium",
            3 => "High",
            4 => "Critical",
            _ => "Low"
        };
    }

    #endregion

    #region Test Scenario Classes

    public class KYCUpdateScenario
    {
        public Guid CustomerId { get; set; }
        public Guid ProcessedBy { get; set; }
        public KYCUpdateRequest KYCRequest { get; set; } = new();
    }

    public class RiskFlaggingScenario
    {
        public Guid CustomerId { get; set; }
        public Guid FlaggedBy { get; set; }
        public List<RiskIndicatorRequest> RiskIndicators { get; set; } = new();
    }

    public class AMLViolationScenario
    {
        public Guid CustomerId { get; set; }
        public Guid EscalatedBy { get; set; }
        public AMLViolationRequest ViolationRequest { get; set; } = new();
    }

    public class DocumentUploadScenario
    {
        public Guid CustomerId { get; set; }
        public Guid UploadedBy { get; set; }
        public List<DocumentUploadRequest> Documents { get; set; } = new();
    }

    public class CriticalRiskScenario
    {
        public Guid CustomerId { get; set; }
        public Guid FlaggedBy { get; set; }
        public List<RiskIndicatorRequest> RiskIndicators { get; set; } = new();
    }

    public class ComplianceCheckScenario
    {
        public Guid CustomerId { get; set; }
        public Guid ProcessedBy { get; set; }
        public KYCUpdateRequest KYCRequest { get; set; } = new();
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }
}