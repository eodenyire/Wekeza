using FsCheck;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;
using Wekeza.MVP4._0.Services;
using Xunit;

namespace Wekeza.MVP4._0.Tests;

/// <summary>
/// Property-based tests for Maker-Checker Workflow Integrity
/// **Property 2: Maker-Checker Workflow Integrity**
/// **Validates: Requirements 1.5, 3.3, 4.4, 5.2, 5.4, 7.3, 7.4**
/// </summary>
public class MakerCheckerPropertyTests : IDisposable
{
    private readonly MVP4DbContext _context;
    private readonly IMakerCheckerService _makerCheckerService;
    private readonly IRBACService _rbacService;
    private readonly INotificationService _notificationService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<MakerCheckerService>> _mockLogger;
    private readonly Mock<ILogger<RBACService>> _mockRbacLogger;
    private readonly Mock<ILogger<NotificationService>> _mockNotificationLogger;

    public MakerCheckerPropertyTests()
    {
        var options = new DbContextOptionsBuilder<MVP4DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MVP4DbContext(options);
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<MakerCheckerService>>();
        _mockRbacLogger = new Mock<ILogger<RBACService>>();
        _mockNotificationLogger = new Mock<ILogger<NotificationService>>();

        // Setup configuration for JWT
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("WekeezaMVP4SecretKeyThatIsAtLeast32CharactersLong123456");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("WekeezaMVP4");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("WekeezaMVP4Users");

        _rbacService = new RBACService(_context, _mockConfiguration.Object, _mockRbacLogger.Object);
        _notificationService = new NotificationService(_context, _mockNotificationLogger.Object);
        _makerCheckerService = new MakerCheckerService(_context, _rbacService, _notificationService, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test roles
        var backOfficeRole = new Role
        {
            RoleId = Guid.NewGuid(),
            RoleName = "BackOfficeOfficer",
            Description = "Back Office Officer Role",
            ApprovalLimit = 10000,
            IsActive = true
        };

        var loanOfficerRole = new Role
        {
            RoleId = Guid.NewGuid(),
            RoleName = "LoanOfficer",
            Description = "Loan Officer Role",
            ApprovalLimit = 50000,
            IsActive = true
        };

        var branchManagerRole = new Role
        {
            RoleId = Guid.NewGuid(),
            RoleName = "BranchManager",
            Description = "Branch Manager Role",
            ApprovalLimit = 500000,
            IsActive = true
        };

        var bancassuranceRole = new Role
        {
            RoleId = Guid.NewGuid(),
            RoleName = "BancassuranceOfficer",
            Description = "Bancassurance Officer Role",
            ApprovalLimit = 25000,
            IsActive = true
        };

        _context.Roles.AddRange(backOfficeRole, loanOfficerRole, branchManagerRole, bancassuranceRole);

        // Create workflow approval permission
        var workflowApprovePermission = new Permission
        {
            PermissionId = Guid.NewGuid(),
            Resource = "Workflow",
            Action = "Approve"
        };

        _context.Permissions.Add(workflowApprovePermission);

        // Assign workflow approval permission to all roles
        _context.RolePermissions.AddRange(
            new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = backOfficeRole.RoleId,
                PermissionId = workflowApprovePermission.PermissionId
            },
            new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = loanOfficerRole.RoleId,
                PermissionId = workflowApprovePermission.PermissionId
            },
            new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = branchManagerRole.RoleId,
                PermissionId = workflowApprovePermission.PermissionId
            },
            new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = bancassuranceRole.RoleId,
                PermissionId = workflowApprovePermission.PermissionId
            }
        );

        _context.SaveChanges();
    }

    /// <summary>
    /// Property 2: Maker-Checker Workflow Integrity
    /// For any high-risk transaction or policy change, the system should enforce that 
    /// different users perform the maker and checker functions, and should escalate 
    /// when approval limits are exceeded
    /// </summary>
    [Fact]
    public async Task MakerCheckerWorkflowIntegrity_ShouldEnforceDifferentUsers()
    {
        // Test multiple scenarios with different maker-checker combinations
        var testCases = new[]
        {
            new { ActionType = "account_creation", Amount = 5000m, ExpectedApprovers = new[] { "BackOfficeOfficer" } },
            new { ActionType = "loan_approval", Amount = 25000m, ExpectedApprovers = new[] { "LoanOfficer" } },
            new { ActionType = "loan_approval", Amount = 75000m, ExpectedApprovers = new[] { "LoanOfficer", "BranchManager" } },
            new { ActionType = "transaction_reversal", Amount = 10000m, ExpectedApprovers = new[] { "BackOfficeOfficer", "BranchManager" } },
            new { ActionType = "policy_creation", Amount = 30000m, ExpectedApprovers = new[] { "BancassuranceOfficer", "BranchManager" } }
        };

        foreach (var testCase in testCases)
        {
            // Arrange - Create maker and checker users
            var maker = CreateTestUser($"maker_{Guid.NewGuid()}", "BackOfficeOfficer");
            var checker = CreateTestUser($"checker_{Guid.NewGuid()}", testCase.ExpectedApprovers[0]);
            
            _context.Users.AddRange(maker, checker);
            await _context.SaveChangesAsync();

            // Assign roles
            await AssignUserToRole(maker.Id, "BackOfficeOfficer");
            await AssignUserToRole(checker.Id, testCase.ExpectedApprovers[0]);

            var makerAction = new MakerAction
            {
                ActionType = testCase.ActionType,
                ResourceId = Guid.NewGuid(),
                ResourceType = "TestResource",
                Data = JsonSerializer.Serialize(new { TestData = "value" }),
                MakerId = maker.Id,
                BusinessJustification = "Test workflow",
                Amount = testCase.Amount,
                Priority = "Normal"
            };

            // Act - Initiate workflow
            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

            // Assert - Workflow should be created successfully
            Assert.NotNull(workflow);
            Assert.Equal(WorkflowStatus.Pending.ToString(), workflow.Status);
            Assert.Equal(maker.Id, workflow.InitiatedBy);

            // Verify approval steps were created correctly
            var steps = await _makerCheckerService.GetWorkflowStepsAsync(workflow.WorkflowId);
            Assert.Equal(testCase.ExpectedApprovers.Length, steps.Count);

            for (int i = 0; i < testCase.ExpectedApprovers.Length; i++)
            {
                Assert.Equal(testCase.ExpectedApprovers[i], steps[i].ApproverRole);
                Assert.Equal(ApprovalStepStatus.Pending.ToString(), steps[i].Status);
            }

            // Test maker-checker rule enforcement
            var validation = await _makerCheckerService.ValidateMakerCheckerRulesAsync(
                maker.Id, checker.Id, testCase.ActionType);
            Assert.True(validation.IsValid);

            // Test same user violation
            var sameUserValidation = await _makerCheckerService.ValidateMakerCheckerRulesAsync(
                maker.Id, maker.Id, testCase.ActionType);
            Assert.False(sameUserValidation.IsValid);
            Assert.Contains("same user", sameUserValidation.Message.ToLower());

            // Cleanup
            _context.Users.RemoveRange(maker, checker);
            _context.WorkflowInstances.Remove(workflow);
            _context.ApprovalSteps.RemoveRange(steps);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Property: Workflow Approval Process Integrity
    /// For any workflow approval process, the system should maintain proper state 
    /// transitions and ensure only authorized users can approve
    /// </summary>
    [Fact]
    public async Task WorkflowApprovalProcess_ShouldMaintainStateIntegrity()
    {
        // Test multiple approval scenarios
        var testCases = new[]
        {
            new { ActionType = "account_creation", Amount = 5000m, ApproverRole = "BackOfficeOfficer" },
            new { ActionType = "loan_approval", Amount = 25000m, ApproverRole = "LoanOfficer" },
            new { ActionType = "policy_creation", Amount = 15000m, ApproverRole = "BancassuranceOfficer" }
        };

        foreach (var testCase in testCases)
        {
            // Arrange
            var maker = CreateTestUser($"maker_{Guid.NewGuid()}", "BackOfficeOfficer");
            var approver = CreateTestUser($"approver_{Guid.NewGuid()}", testCase.ApproverRole);
            
            // Create unauthorized user with a different role than what's required for approval
            var unauthorizedRole = testCase.ApproverRole == "BackOfficeOfficer" ? "LoanOfficer" : "BackOfficeOfficer";
            var unauthorizedUser = CreateTestUser($"unauthorized_{Guid.NewGuid()}", unauthorizedRole);

            _context.Users.AddRange(maker, approver, unauthorizedUser);
            await _context.SaveChangesAsync();

            await AssignUserToRole(maker.Id, "BackOfficeOfficer");
            await AssignUserToRole(approver.Id, testCase.ApproverRole);
            await AssignUserToRole(unauthorizedUser.Id, unauthorizedRole);

            var makerAction = new MakerAction
            {
                ActionType = testCase.ActionType,
                ResourceId = Guid.NewGuid(),
                ResourceType = "TestResource",
                Data = JsonSerializer.Serialize(new { TestData = "value" }),
                MakerId = maker.Id,
                BusinessJustification = "Test workflow approval",
                Amount = testCase.Amount,
                Priority = "Normal"
            };

            // Act - Initiate workflow
            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

            // Test unauthorized approval attempt
            var unauthorizedResult = await _makerCheckerService.SubmitForApprovalAsync(
                workflow.WorkflowId, unauthorizedUser.Id, "Unauthorized approval attempt");
            Assert.False(unauthorizedResult.IsSuccess);
            Assert.Contains("not authorized", unauthorizedResult.Message.ToLower());

            // Test authorized approval
            var authorizedResult = await _makerCheckerService.SubmitForApprovalAsync(
                workflow.WorkflowId, approver.Id, "Authorized approval");
            
            // For single-step workflows, should be approved and completed
            if (testCase.Amount <= 25000) // Single approval workflows
            {
                Assert.True(authorizedResult.IsSuccess);
                var updatedWorkflow = await _makerCheckerService.GetWorkflowInstanceAsync(workflow.WorkflowId);
                Assert.NotNull(updatedWorkflow);
                Assert.Equal(WorkflowStatus.Approved.ToString(), updatedWorkflow.Status);
                Assert.NotNull(updatedWorkflow.CompletedAt);
            }

            // Cleanup
            _context.Users.RemoveRange(maker, approver, unauthorizedUser);
            var workflowToRemove = await _context.WorkflowInstances.FindAsync(workflow.WorkflowId);
            if (workflowToRemove != null)
            {
                var stepsToRemove = await _context.ApprovalSteps
                    .Where(s => s.WorkflowId == workflow.WorkflowId).ToListAsync();
                _context.ApprovalSteps.RemoveRange(stepsToRemove);
                _context.WorkflowInstances.Remove(workflowToRemove);
            }
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Property: Workflow Rejection Process
    /// For any workflow rejection, the system should properly update status and 
    /// prevent further processing
    /// </summary>
    [Fact]
    public async Task WorkflowRejection_ShouldPreventFurtherProcessing()
    {
        // Test rejection scenarios
        var testCases = new[]
        {
            new { ActionType = "account_creation", Amount = 5000m, ApproverRole = "BackOfficeOfficer", RejectionReason = "Insufficient documentation" },
            new { ActionType = "loan_approval", Amount = 25000m, ApproverRole = "LoanOfficer", RejectionReason = "Credit score too low" },
            new { ActionType = "policy_creation", Amount = 15000m, ApproverRole = "BancassuranceOfficer", RejectionReason = "Policy terms not acceptable" }
        };

        foreach (var testCase in testCases)
        {
            // Arrange
            var maker = CreateTestUser($"maker_{Guid.NewGuid()}", "BackOfficeOfficer");
            var approver = CreateTestUser($"approver_{Guid.NewGuid()}", testCase.ApproverRole);

            _context.Users.AddRange(maker, approver);
            await _context.SaveChangesAsync();

            await AssignUserToRole(maker.Id, "BackOfficeOfficer");
            await AssignUserToRole(approver.Id, testCase.ApproverRole);

            var makerAction = new MakerAction
            {
                ActionType = testCase.ActionType,
                ResourceId = Guid.NewGuid(),
                ResourceType = "TestResource",
                Data = JsonSerializer.Serialize(new { TestData = "value" }),
                MakerId = maker.Id,
                BusinessJustification = "Test workflow rejection",
                Amount = testCase.Amount,
                Priority = "Normal"
            };

            // Act - Initiate and reject workflow
            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);
            var rejectionResult = await _makerCheckerService.RejectWorkflowAsync(
                workflow.WorkflowId, approver.Id, testCase.RejectionReason);

            // Assert
            Assert.True(rejectionResult.IsSuccess);
            Assert.NotNull(rejectionResult.WorkflowInstance);
            Assert.Equal(WorkflowStatus.Rejected.ToString(), rejectionResult.WorkflowInstance.Status);
            Assert.Equal(testCase.RejectionReason, rejectionResult.WorkflowInstance.RejectionReason);
            Assert.NotNull(rejectionResult.WorkflowInstance.CompletedAt);

            // Test that further approval attempts fail
            var furtherApprovalResult = await _makerCheckerService.SubmitForApprovalAsync(
                workflow.WorkflowId, approver.Id, "Attempt after rejection");
            Assert.False(furtherApprovalResult.IsSuccess);

            // Cleanup
            _context.Users.RemoveRange(maker, approver);
            var workflowToRemove = await _context.WorkflowInstances.FindAsync(workflow.WorkflowId);
            if (workflowToRemove != null)
            {
                var stepsToRemove = await _context.ApprovalSteps
                    .Where(s => s.WorkflowId == workflow.WorkflowId).ToListAsync();
                _context.ApprovalSteps.RemoveRange(stepsToRemove);
                _context.WorkflowInstances.Remove(workflowToRemove);
            }
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Property: Approval Limit Escalation
    /// For any transaction exceeding approval limits, the system should automatically 
    /// escalate to higher authority levels
    /// </summary>
    [Fact]
    public async Task ApprovalLimitEscalation_ShouldEscalateHighValueTransactions()
    {
        // Test escalation scenarios
        var testCases = new[]
        {
            new { ActionType = "loan_approval", Amount = 75000m, ExpectedEscalation = true, InitialApprover = "LoanOfficer" },
            new { ActionType = "account_creation", Amount = 150000m, ExpectedEscalation = true, InitialApprover = "BackOfficeOfficer" },
            new { ActionType = "policy_creation", Amount = 50000m, ExpectedEscalation = true, InitialApprover = "BancassuranceOfficer" },
            new { ActionType = "loan_approval", Amount = 25000m, ExpectedEscalation = false, InitialApprover = "LoanOfficer" }
        };

        foreach (var testCase in testCases)
        {
            // Arrange
            var maker = CreateTestUser($"maker_{Guid.NewGuid()}", "BackOfficeOfficer");
            _context.Users.Add(maker);
            await _context.SaveChangesAsync();
            await AssignUserToRole(maker.Id, "BackOfficeOfficer");

            var makerAction = new MakerAction
            {
                ActionType = testCase.ActionType,
                ResourceId = Guid.NewGuid(),
                ResourceType = "TestResource",
                Data = JsonSerializer.Serialize(new { TestData = "value" }),
                MakerId = maker.Id,
                BusinessJustification = "Test escalation workflow",
                Amount = testCase.Amount,
                Priority = "Normal"
            };

            // Act
            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);
            var steps = await _makerCheckerService.GetWorkflowStepsAsync(workflow.WorkflowId);

            // Assert
            if (testCase.ExpectedEscalation)
            {
                // Should have multiple approval steps including BranchManager
                Assert.True(steps.Count > 1);
                Assert.Contains(steps, s => s.ApproverRole == "BranchManager");
            }
            else
            {
                // Should have single approval step
                Assert.Equal(1, steps.Count);
                Assert.Equal(testCase.InitialApprover, steps[0].ApproverRole);
            }

            // Test manual escalation
            var escalationResult = await _makerCheckerService.EscalateApprovalAsync(
                workflow.WorkflowId, "Manual escalation for testing");
            Assert.True(escalationResult.IsSuccess);
            Assert.Contains(workflow.WorkflowId, escalationResult.EscalatedWorkflowIds);

            // Verify workflow status changed to escalated
            var escalatedWorkflow = await _makerCheckerService.GetWorkflowInstanceAsync(workflow.WorkflowId);
            Assert.NotNull(escalatedWorkflow);
            Assert.Equal(WorkflowStatus.Escalated.ToString(), escalatedWorkflow.Status);
            Assert.NotNull(escalatedWorkflow.EscalatedAt);

            // Cleanup
            _context.Users.Remove(maker);
            var workflowToRemove = await _context.WorkflowInstances.FindAsync(workflow.WorkflowId);
            if (workflowToRemove != null)
            {
                var stepsToRemove = await _context.ApprovalSteps
                    .Where(s => s.WorkflowId == workflow.WorkflowId).ToListAsync();
                _context.ApprovalSteps.RemoveRange(stepsToRemove);
                _context.WorkflowInstances.Remove(workflowToRemove);
            }
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Property: Workflow Cancellation
    /// For any workflow cancellation, the system should properly update status and 
    /// prevent further processing while maintaining audit trail
    /// </summary>
    [Fact]
    public async Task WorkflowCancellation_ShouldPreventFurtherProcessing()
    {
        // Test cancellation scenarios
        var testCases = new[]
        {
            new { ActionType = "account_creation", Amount = 5000m, CancellationReason = "Customer request cancelled" },
            new { ActionType = "loan_approval", Amount = 25000m, CancellationReason = "Duplicate application found" },
            new { ActionType = "policy_creation", Amount = 15000m, CancellationReason = "Policy terms changed" }
        };

        foreach (var testCase in testCases)
        {
            // Arrange
            var maker = CreateTestUser($"maker_{Guid.NewGuid()}", "BackOfficeOfficer");
            var canceller = CreateTestUser($"canceller_{Guid.NewGuid()}", "BranchManager");

            _context.Users.AddRange(maker, canceller);
            await _context.SaveChangesAsync();

            await AssignUserToRole(maker.Id, "BackOfficeOfficer");
            await AssignUserToRole(canceller.Id, "BranchManager");

            var makerAction = new MakerAction
            {
                ActionType = testCase.ActionType,
                ResourceId = Guid.NewGuid(),
                ResourceType = "TestResource",
                Data = JsonSerializer.Serialize(new { TestData = "value" }),
                MakerId = maker.Id,
                BusinessJustification = "Test workflow cancellation",
                Amount = testCase.Amount,
                Priority = "Normal"
            };

            // Act
            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);
            var cancellationResult = await _makerCheckerService.CancelWorkflowAsync(
                workflow.WorkflowId, canceller.Id, testCase.CancellationReason);

            // Assert
            Assert.True(cancellationResult);

            var cancelledWorkflow = await _makerCheckerService.GetWorkflowInstanceAsync(workflow.WorkflowId);
            Assert.NotNull(cancelledWorkflow);
            Assert.Equal(WorkflowStatus.Cancelled.ToString(), cancelledWorkflow.Status);
            Assert.Equal(testCase.CancellationReason, cancelledWorkflow.CancellationReason);
            Assert.Equal(canceller.Id, cancelledWorkflow.CancelledBy);
            Assert.NotNull(cancelledWorkflow.CompletedAt);

            // Test that approval attempts fail after cancellation
            var approvalResult = await _makerCheckerService.SubmitForApprovalAsync(
                workflow.WorkflowId, canceller.Id, "Attempt after cancellation");
            Assert.False(approvalResult.IsSuccess);

            // Cleanup
            _context.Users.RemoveRange(maker, canceller);
            var workflowToRemove = await _context.WorkflowInstances.FindAsync(workflow.WorkflowId);
            if (workflowToRemove != null)
            {
                var stepsToRemove = await _context.ApprovalSteps
                    .Where(s => s.WorkflowId == workflow.WorkflowId).ToListAsync();
                _context.ApprovalSteps.RemoveRange(stepsToRemove);
                _context.WorkflowInstances.Remove(workflowToRemove);
            }
            await _context.SaveChangesAsync();
        }
    }

    private ApplicationUser CreateTestUser(string username, string roleName)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = $"{username}@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123!"),
            FullName = $"Test {username}",
            Role = UserRole.BackOfficeStaff,
            IsActive = true,
            FailedLoginAttempts = 0
        };
    }

    private async Task AssignUserToRole(Guid userId, string roleName)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        if (role != null)
        {
            var assignment = new UserRoleAssignment
            {
                UserRoleId = Guid.NewGuid(),
                UserId = userId,
                RoleId = role.RoleId,
                AssignedBy = Guid.NewGuid(),
                IsActive = true
            };
            _context.UserRoleAssignments.Add(assignment);
            await _context.SaveChangesAsync();
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}