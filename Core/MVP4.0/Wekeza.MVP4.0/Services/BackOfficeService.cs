using Microsoft.EntityFrameworkCore;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

/// <summary>
/// Implementation of Back Office Officer services for account operations, KYC processing, and transaction management
/// </summary>
public class BackOfficeService : IBackOfficeService
{
    private readonly MVP4DbContext _context;
    private readonly IRBACService _rbacService;
    private readonly IMakerCheckerService _makerCheckerService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<BackOfficeService> _logger;

    public BackOfficeService(
        MVP4DbContext context,
        IRBACService rbacService,
        IMakerCheckerService makerCheckerService,
        INotificationService notificationService,
        ILogger<BackOfficeService> logger)
    {
        _context = context;
        _rbacService = rbacService;
        _makerCheckerService = makerCheckerService;
        _notificationService = notificationService;
        _logger = logger;
    }

    #region Account Operations

    public async Task<AccountCreationResult> CreateCustomerAccountAsync(AccountCreationRequest request)
    {
        try
        {
            _logger.LogInformation("Creating customer account for CustomerId: {CustomerId}", request.CustomerId);

            // Validate business rules
            var businessRuleViolations = await CheckBusinessRulesAsync("AccountCreation", request);
            if (businessRuleViolations.Any(v => v.Severity == "Error"))
            {
                return new AccountCreationResult
                {
                    Success = false,
                    Message = "Business rule violations detected",
                    Errors = businessRuleViolations.Where(v => v.Severity == "Error").Select(v => v.Description).ToList()
                };
            }

            // Check if customer exists
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
            {
                return new AccountCreationResult
                {
                    Success = false,
                    Message = "Customer not found",
                    Errors = new List<string> { "Invalid customer ID provided" }
                };
            }

            // Generate account number
            var accountNumber = await GenerateAccountNumberAsync(request.AccountType, request.BranchCode);

            // Create account entity
            var account = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = accountNumber,
                CustomerId = request.CustomerId,
                AccountType = request.AccountType,
                Balance = request.InitialDeposit,
                AvailableBalance = request.InitialDeposit,
                Currency = request.Currency,
                Status = "PendingVerification",
                BranchCode = request.BranchCode,
                OpenedDate = DateTime.UtcNow
            };

            _context.Accounts.Add(account);

            // Create signatory rules
            foreach (var signatoryRule in request.SignatoryRules)
            {
                var rule = new SignatoryRule
                {
                    RuleId = Guid.NewGuid(),
                    AccountId = account.Id,
                    SignatoryType = signatoryRule.SignatoryType,
                    MinimumSignatures = signatoryRule.MinimumSignatures,
                    MaximumAmount = signatoryRule.MaximumAmount,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.SignatoryRules.Add(rule);

                // Add account signatories
                foreach (var signatoryId in signatoryRule.AuthorizedSignatories)
                {
                    var signatory = new AccountSignatory
                    {
                        SignatoryId = Guid.NewGuid(),
                        AccountId = account.Id,
                        CustomerId = signatoryId,
                        SignatoryRole = signatoryRule.SignatoryType,
                        IsActive = true,
                        AddedAt = DateTime.UtcNow
                    };
                    _context.AccountSignatories.Add(signatory);
                }
            }

            // Determine if approval is required
            bool requiresApproval = request.InitialDeposit > 10000 || 
                                  request.AccountType == "Corporate" ||
                                  businessRuleViolations.Any(v => v.Severity == "Warning");

            Guid? workflowId = null;
            if (requiresApproval)
            {
                // Initiate maker-checker workflow
                var makerAction = new MakerAction
                {
                    ActionType = "AccountCreation",
                    ResourceId = account.Id,
                    ResourceType = "Account",
                    Data = System.Text.Json.JsonSerializer.Serialize(request),
                    MakerId = Guid.NewGuid(), // This should be the actual user ID from context
                    BusinessJustification = "New account creation requiring approval",
                    Amount = request.InitialDeposit
                };

                var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);
                workflowId = workflow.WorkflowId;
                account.Status = "PendingApproval";
            }
            else
            {
                account.Status = "Active";
            }

            await _context.SaveChangesAsync();

            // Send notification if approval required
            if (requiresApproval && workflowId.HasValue)
            {
                await _notificationService.SendApprovalNotificationAsync(
                    workflowId.Value,
                    "BranchManager",
                    $"Account {accountNumber} for customer {customer.FullName} requires approval"
                );
            }

            _logger.LogInformation("Account created successfully: {AccountNumber}", accountNumber);

            return new AccountCreationResult
            {
                Success = true,
                AccountId = account.Id,
                AccountNumber = accountNumber,
                Message = requiresApproval ? "Account created and submitted for approval" : "Account created successfully",
                WorkflowId = workflowId,
                RequiresApproval = requiresApproval
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer account for CustomerId: {CustomerId}", request.CustomerId);
            return new AccountCreationResult
            {
                Success = false,
                Message = "An error occurred while creating the account",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<VerificationResult> VerifyAccountOpeningAsync(Guid accountId, VerificationData verificationData)
    {
        try
        {
            _logger.LogInformation("Verifying account opening for AccountId: {AccountId}", accountId);

            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                return new VerificationResult
                {
                    Success = false,
                    Message = "Account not found"
                };
            }

            // Perform verification checks
            var issues = new List<string>();

            if (!verificationData.KYCCompliant)
            {
                issues.Add("KYC compliance not satisfied");
            }

            if (!verificationData.DocumentsVerified.Any())
            {
                issues.Add("No documents verified");
            }

            // Check compliance requirements
            foreach (var check in verificationData.ComplianceChecks)
            {
                if (!check.Value)
                {
                    issues.Add($"Compliance check failed: {check.Key}");
                }
            }

            bool approved = !issues.Any() && !verificationData.RequiresManagerApproval;
            
            if (approved)
            {
                account.Status = "Active";
                await _context.SaveChangesAsync();

                _logger.LogInformation("Account verification approved for AccountId: {AccountId}", accountId);

                return new VerificationResult
                {
                    Success = true,
                    Approved = true,
                    Status = "Approved",
                    Message = "Account verification completed successfully"
                };
            }
            else
            {
                // Initiate approval workflow
                var makerAction = new MakerAction
                {
                    ActionType = "AccountVerification",
                    ResourceId = accountId,
                    ResourceType = "Account",
                    Data = System.Text.Json.JsonSerializer.Serialize(verificationData),
                    MakerId = Guid.NewGuid(), // This should be the actual user ID from context
                    BusinessJustification = "Account verification requires manager approval"
                };

                var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

                account.Status = "PendingManagerApproval";
                await _context.SaveChangesAsync();

                return new VerificationResult
                {
                    Success = true,
                    Approved = false,
                    Status = "PendingManagerApproval",
                    Message = "Account verification requires manager approval",
                    Issues = issues,
                    WorkflowId = workflow.WorkflowId
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying account opening for AccountId: {AccountId}", accountId);
            return new VerificationResult
            {
                Success = false,
                Message = "An error occurred during account verification"
            };
        }
    }

    public async Task<ClosureResult> ProcessAccountClosureAsync(Guid accountId, string closureReason, Guid requestedBy)
    {
        try
        {
            _logger.LogInformation("Processing account closure for AccountId: {AccountId}", accountId);

            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .Include(a => a.StandingInstructions)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                return new ClosureResult
                {
                    Success = false,
                    Message = "Account not found"
                };
            }

            // Validate closure conditions
            if (account.Balance != 0)
            {
                return new ClosureResult
                {
                    Success = false,
                    Message = "Account balance must be zero before closure"
                };
            }

            // Check for pending transactions
            var pendingTransactions = account.Transactions
                .Where(t => t.Status == "Pending" || t.Status == "Processing")
                .ToList();

            if (pendingTransactions.Any())
            {
                return new ClosureResult
                {
                    Success = false,
                    Message = "Account has pending transactions that must be resolved before closure"
                };
            }

            // Check for active standing instructions
            var activeStandingInstructions = account.StandingInstructions
                .Where(si => si.Status == "Active")
                .ToList();

            if (activeStandingInstructions.Any())
            {
                return new ClosureResult
                {
                    Success = false,
                    Message = "Account has active standing instructions that must be cancelled before closure"
                };
            }

            // Initiate closure workflow
            var makerAction = new MakerAction
            {
                ActionType = "AccountClosure",
                ResourceId = accountId,
                ResourceType = "Account",
                Data = System.Text.Json.JsonSerializer.Serialize(new { ClosureReason = closureReason, RequestedBy = requestedBy }),
                MakerId = requestedBy,
                BusinessJustification = closureReason
            };

            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

            account.Status = "PendingClosure";
            await _context.SaveChangesAsync();

            return new ClosureResult
            {
                Success = true,
                Status = "PendingClosure",
                Message = "Account closure request submitted for approval",
                WorkflowId = workflow.WorkflowId,
                RequiresApproval = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing account closure for AccountId: {AccountId}", accountId);
            return new ClosureResult
            {
                Success = false,
                Message = "An error occurred while processing account closure"
            };
        }
    }
    public async Task<UpdateResult> UpdateSignatoryRulesAsync(Guid accountId, List<SignatoryRuleRequest> signatoryRules, Guid requestedBy)
    {
        try
        {
            _logger.LogInformation("Updating signatory rules for AccountId: {AccountId}", accountId);

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return new UpdateResult
                {
                    Success = false,
                    Message = "Account not found",
                    Errors = new List<string> { "Invalid account ID provided" }
                };
            }

            // Validate signatory rules
            var validationErrors = ValidateSignatoryRules(signatoryRules);
            if (validationErrors.Any())
            {
                return new UpdateResult
                {
                    Success = false,
                    Message = "Signatory rule validation failed",
                    Errors = validationErrors
                };
            }

            // High-risk change requires approval
            var makerAction = new MakerAction
            {
                ActionType = "SignatoryRuleUpdate",
                ResourceId = accountId,
                ResourceType = "Account",
                Data = System.Text.Json.JsonSerializer.Serialize(signatoryRules),
                MakerId = requestedBy,
                BusinessJustification = "Signatory rule update"
            };

            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

            return new UpdateResult
            {
                Success = true,
                Message = "Signatory rule update submitted for approval",
                WorkflowId = workflow.WorkflowId,
                RequiresApproval = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating signatory rules for AccountId: {AccountId}", accountId);
            return new UpdateResult
            {
                Success = false,
                Message = "An error occurred while updating signatory rules",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<UpdateResult> UpdateAccountMandateAsync(Guid accountId, AccountMandateRequest mandateRequest, Guid requestedBy)
    {
        try
        {
            _logger.LogInformation("Updating account mandate for AccountId: {AccountId}", accountId);

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return new UpdateResult
                {
                    Success = false,
                    Message = "Account not found",
                    Errors = new List<string> { "Invalid account ID provided" }
                };
            }

            // Mandate updates require approval
            var makerAction = new MakerAction
            {
                ActionType = "AccountMandateUpdate",
                ResourceId = accountId,
                ResourceType = "Account",
                Data = System.Text.Json.JsonSerializer.Serialize(mandateRequest),
                MakerId = requestedBy,
                BusinessJustification = "Account mandate update"
            };

            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

            return new UpdateResult
            {
                Success = true,
                Message = "Account mandate update submitted for approval",
                WorkflowId = workflow.WorkflowId,
                RequiresApproval = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account mandate for AccountId: {AccountId}", accountId);
            return new UpdateResult
            {
                Success = false,
                Message = "An error occurred while updating account mandate",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    #endregion

    #region KYC & Compliance Processing

    public async Task<KYCResult> ProcessKYCUpdateAsync(Guid customerId, KYCUpdateRequest kycData, Guid processedBy)
    {
        try
        {
            _logger.LogInformation("Processing KYC update for CustomerId: {CustomerId}", customerId);

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return new KYCResult
                {
                    Success = false,
                    Message = "Customer not found"
                };
            }

            // Validate document completeness
            var issues = new List<string>();
            if (!kycData.Documents.Any())
            {
                issues.Add("No documents provided for KYC update");
            }

            // Check document authenticity (simplified validation)
            foreach (var doc in kycData.Documents)
            {
                if (string.IsNullOrEmpty(doc.DocumentType) || string.IsNullOrEmpty(doc.FileName))
                {
                    issues.Add($"Invalid document: {doc.FileName}");
                }
            }

            // Process compliance checks
            foreach (var check in kycData.ComplianceChecks)
            {
                if (check.Result != "Pass")
                {
                    issues.Add($"Compliance check failed: {check.CheckType}");
                }
            }

            // Create or update KYC data
            var existingKYC = await _context.KYCData.FirstOrDefaultAsync(k => k.CustomerId == customerId);
            if (existingKYC == null)
            {
                var newKYC = new KYCData
                {
                    KYCId = Guid.NewGuid(),
                    CustomerId = customerId,
                    RiskRating = kycData.RiskRating,
                    LastReviewDate = DateTime.UtcNow,
                    NextReviewDate = DateTime.UtcNow.AddYears(1),
                    ReviewedBy = processedBy.ToString()
                };
                _context.KYCData.Add(newKYC);
            }
            else
            {
                existingKYC.RiskRating = kycData.RiskRating;
                existingKYC.LastReviewDate = DateTime.UtcNow;
                existingKYC.NextReviewDate = DateTime.UtcNow.AddYears(1);
                existingKYC.ReviewedBy = processedBy.ToString();
            }

            // Update customer KYC status
            customer.KycStatus = issues.Count == 0 ? "Compliant" : "NonCompliant";
            
            await _context.SaveChangesAsync();

            return new KYCResult
            {
                Success = true,
                KYCStatus = customer.KycStatus,
                RiskRating = kycData.RiskRating,
                Message = issues.Count == 0 ? "KYC update completed successfully" : "KYC update completed with issues",
                Issues = issues,
                RequiresManagerReview = kycData.RequiresManagerReview || issues.Count > 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing KYC update for CustomerId: {CustomerId}", customerId);
            return new KYCResult
            {
                Success = false,
                Message = "An error occurred while processing KYC update"
            };
        }
    }

    public async Task<DocumentResult> ProcessDocumentUploadAsync(Guid customerId, List<DocumentUploadRequest> documents, Guid uploadedBy)
    {
        try
        {
            _logger.LogInformation("Processing document upload for CustomerId: {CustomerId}", customerId);

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return new DocumentResult
                {
                    Success = false,
                    Message = "Customer not found"
                };
            }

            var documentIds = new List<Guid>();
            var errors = new List<string>();
            int processedCount = 0;

            foreach (var docRequest in documents)
            {
                try
                {
                    // Validate document
                    if (string.IsNullOrEmpty(docRequest.DocumentType) || string.IsNullOrEmpty(docRequest.FileName))
                    {
                        errors.Add($"Invalid document data for {docRequest.FileName}");
                        continue;
                    }

                    // Store document securely (simplified - in real implementation, use secure file storage)
                    var document = new CustomerDocument
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerId,
                        DocumentType = docRequest.DocumentType,
                        FileName = docRequest.FileName,
                        FilePath = docRequest.FilePath,
                        Status = docRequest.DocumentStatus,
                        UploadedBy = uploadedBy.ToString(),
                        UploadedAt = DateTime.UtcNow,
                        ExpiryDate = docRequest.ExpiryDate
                    };

                    _context.CustomerDocuments.Add(document);
                    documentIds.Add(document.Id);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Error processing document {docRequest.FileName}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            return new DocumentResult
            {
                Success = true,
                DocumentIds = documentIds,
                Message = $"Processed {processedCount} documents successfully",
                Errors = errors,
                ProcessedCount = processedCount,
                FailedCount = documents.Count - processedCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document upload for CustomerId: {CustomerId}", customerId);
            return new DocumentResult
            {
                Success = false,
                Message = "An error occurred while processing document upload",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<FlagResult> FlagHighRiskCustomerAsync(Guid customerId, List<RiskIndicatorRequest> riskIndicators, Guid flaggedBy)
    {
        try
        {
            _logger.LogInformation("Flagging high-risk customer: {CustomerId}", customerId);

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return new FlagResult
                {
                    Success = false,
                    Message = "Customer not found"
                };
            }

            // Determine risk level based on indicators
            var highestSeverity = riskIndicators.Max(r => GetSeverityLevel(r.Severity));
            var riskLevel = GetRiskLevelFromSeverity(highestSeverity);

            // Create risk indicators
            var kycData = await _context.KYCData.FirstOrDefaultAsync(k => k.CustomerId == customerId);
            if (kycData != null)
            {
                foreach (var indicator in riskIndicators)
                {
                    var riskIndicator = new RiskIndicator
                    {
                        IndicatorId = Guid.NewGuid(),
                        KYCId = kycData.KYCId,
                        IndicatorType = indicator.IndicatorType,
                        Description = indicator.Description,
                        Severity = indicator.Severity,
                        IdentifiedBy = flaggedBy
                    };
                    _context.RiskIndicators.Add(riskIndicator);
                }
            }

            bool requiresImmediateAction = riskIndicators.Any(r => r.Severity == "Critical");
            Guid? escalationWorkflowId = null;

            if (requiresImmediateAction)
            {
                // Trigger AML review workflow
                var makerAction = new MakerAction
                {
                    ActionType = "AMLReview",
                    ResourceId = customerId,
                    ResourceType = "Customer",
                    Data = System.Text.Json.JsonSerializer.Serialize(riskIndicators),
                    MakerId = flaggedBy,
                    BusinessJustification = "High-risk customer flagged for AML review",
                    Priority = "Critical"
                };

                var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);
                escalationWorkflowId = workflow.WorkflowId;
            }

            await _context.SaveChangesAsync();

            return new FlagResult
            {
                Success = true,
                RiskLevel = riskLevel,
                Message = requiresImmediateAction ? "Customer flagged and escalated for immediate AML review" : "Customer risk level updated",
                RequiresImmediateAction = requiresImmediateAction,
                EscalationWorkflowId = escalationWorkflowId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error flagging high-risk customer: {CustomerId}", customerId);
            return new FlagResult
            {
                Success = false,
                Message = "An error occurred while flagging customer"
            };
        }
    }
    public async Task<BackOfficeEscalationResult> EscalateAMLViolationAsync(Guid customerId, AMLViolationRequest violationDetails, Guid escalatedBy)
    {
        try
        {
            _logger.LogInformation("Escalating AML violation for CustomerId: {CustomerId}", customerId);

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return new BackOfficeEscalationResult
                {
                    Success = false,
                    Message = "Customer not found"
                };
            }

            // AML violations cannot be overridden - must escalate to compliance team
            var makerAction = new MakerAction
            {
                ActionType = "AMLViolationEscalation",
                ResourceId = customerId,
                ResourceType = "Customer",
                Data = System.Text.Json.JsonSerializer.Serialize(violationDetails),
                MakerId = escalatedBy,
                BusinessJustification = "AML violation requires compliance team review",
                Priority = "Critical"
            };

            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

            // Send immediate notification to compliance team
            await _notificationService.SendEscalationNotificationAsync(
                workflow.WorkflowId,
                "ComplianceOfficer",
                $"AML violation detected for customer {customer.FullName}: {violationDetails.Description}"
            );

            return new BackOfficeEscalationResult
            {
                Success = true,
                EscalationLevel = "ComplianceTeam",
                Message = "AML violation escalated to compliance team",
                WorkflowId = workflow.WorkflowId,
                ExpectedResolutionDate = DateTime.UtcNow.AddDays(violationDetails.RequiresImmediateAction ? 1 : 5)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error escalating AML violation for CustomerId: {CustomerId}", customerId);
            return new BackOfficeEscalationResult
            {
                Success = false,
                Message = "An error occurred while escalating AML violation"
            };
        }
    }

    public async Task<UpdateResult> UpdateRiskRatingAsync(Guid customerId, RiskRatingUpdateRequest riskUpdate, Guid updatedBy)
    {
        try
        {
            _logger.LogInformation("Updating risk rating for CustomerId: {CustomerId}", customerId);

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return new UpdateResult
                {
                    Success = false,
                    Message = "Customer not found",
                    Errors = new List<string> { "Invalid customer ID provided" }
                };
            }

            // Risk rating updates require approval
            var makerAction = new MakerAction
            {
                ActionType = "RiskRatingUpdate",
                ResourceId = customerId,
                ResourceType = "Customer",
                Data = System.Text.Json.JsonSerializer.Serialize(riskUpdate),
                MakerId = updatedBy,
                BusinessJustification = riskUpdate.Justification
            };

            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

            return new UpdateResult
            {
                Success = true,
                Message = "Risk rating update submitted for approval",
                WorkflowId = workflow.WorkflowId,
                RequiresApproval = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating risk rating for CustomerId: {CustomerId}", customerId);
            return new UpdateResult
            {
                Success = false,
                Message = "An error occurred while updating risk rating",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    #endregion

    #region Transaction Processing

    public async Task<TransferResult> PostInternalTransferAsync(InternalTransferRequest transferRequest, Guid initiatedBy)
    {
        try
        {
            _logger.LogInformation("Processing internal transfer from {FromAccount} to {ToAccount}", 
                transferRequest.FromAccountId, transferRequest.ToAccountId);

            // Validate accounts
            var fromAccount = await _context.Accounts.FindAsync(transferRequest.FromAccountId);
            var toAccount = await _context.Accounts.FindAsync(transferRequest.ToAccountId);

            if (fromAccount == null || toAccount == null)
            {
                return new TransferResult
                {
                    Success = false,
                    Message = "One or both accounts not found",
                    Errors = new List<string> { "Invalid account IDs provided" }
                };
            }

            // Validate balance
            if (fromAccount.AvailableBalance < transferRequest.Amount)
            {
                return new TransferResult
                {
                    Success = false,
                    Message = "Insufficient funds",
                    Errors = new List<string> { "Available balance is insufficient for this transfer" }
                };
            }

            // Generate transaction reference
            var transactionReference = await GenerateTransactionReferenceAsync("INT");

            // Determine if approval is required
            bool requiresApproval = transferRequest.RequiresApproval || transferRequest.Amount > 50000;

            if (requiresApproval)
            {
                // Initiate workflow for approval
                var makerAction = new MakerAction
                {
                    ActionType = "InternalTransfer",
                    ResourceId = transferRequest.FromAccountId,
                    ResourceType = "Transaction",
                    Data = System.Text.Json.JsonSerializer.Serialize(transferRequest),
                    MakerId = initiatedBy,
                    BusinessJustification = "Internal transfer requiring approval",
                    Amount = transferRequest.Amount
                };

                var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

                return new TransferResult
                {
                    Success = true,
                    TransactionReference = transactionReference,
                    Status = "PendingApproval",
                    Message = "Transfer submitted for approval",
                    WorkflowId = workflow.WorkflowId
                };
            }
            else
            {
                // Execute transfer immediately
                var transactionId = await ExecuteTransferAsync(transferRequest, transactionReference, initiatedBy);

                return new TransferResult
                {
                    Success = true,
                    TransactionId = transactionId,
                    TransactionReference = transactionReference,
                    Status = "Completed",
                    Message = "Transfer completed successfully"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing internal transfer");
            return new TransferResult
            {
                Success = false,
                Message = "An error occurred while processing the transfer",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<AdjustmentResult> ProcessChargesAndAdjustmentsAsync(AdjustmentRequest adjustmentRequest, Guid processedBy)
    {
        try
        {
            _logger.LogInformation("Processing adjustment for AccountId: {AccountId}", adjustmentRequest.AccountId);

            var account = await _context.Accounts.FindAsync(adjustmentRequest.AccountId);
            if (account == null)
            {
                return new AdjustmentResult
                {
                    Success = false,
                    Message = "Account not found",
                    Errors = new List<string> { "Invalid account ID provided" }
                };
            }

            // Generate transaction reference
            var transactionReference = await GenerateTransactionReferenceAsync("ADJ");

            // Determine if approval is required
            bool requiresApproval = adjustmentRequest.RequiresApproval || 
                                  adjustmentRequest.Amount > 10000 ||
                                  adjustmentRequest.AdjustmentType == "Reversal";

            if (requiresApproval)
            {
                // Initiate workflow for approval
                var makerAction = new MakerAction
                {
                    ActionType = "ChargeAdjustment",
                    ResourceId = adjustmentRequest.AccountId,
                    ResourceType = "Transaction",
                    Data = System.Text.Json.JsonSerializer.Serialize(adjustmentRequest),
                    MakerId = processedBy,
                    BusinessJustification = adjustmentRequest.Reason,
                    Amount = adjustmentRequest.Amount
                };

                var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

                return new AdjustmentResult
                {
                    Success = true,
                    TransactionReference = transactionReference,
                    Status = "PendingApproval",
                    Message = "Adjustment submitted for approval",
                    WorkflowId = workflow.WorkflowId
                };
            }
            else
            {
                // Execute adjustment immediately
                var transactionId = await ExecuteAdjustmentAsync(adjustmentRequest, transactionReference, processedBy);

                return new AdjustmentResult
                {
                    Success = true,
                    TransactionId = transactionId,
                    TransactionReference = transactionReference,
                    Status = "Completed",
                    Message = "Adjustment processed successfully"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing adjustment for AccountId: {AccountId}", adjustmentRequest.AccountId);
            return new AdjustmentResult
            {
                Success = false,
                Message = "An error occurred while processing the adjustment",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ReversalResult> HandleTransactionReversalAsync(ReversalRequest reversalRequest, Guid requestedBy)
    {
        try
        {
            _logger.LogInformation("Processing transaction reversal for TransactionId: {TransactionId}", 
                reversalRequest.OriginalTransactionId);

            var originalTransaction = await _context.Transactions.FindAsync(reversalRequest.OriginalTransactionId);
            if (originalTransaction == null)
            {
                return new ReversalResult
                {
                    Success = false,
                    Message = "Original transaction not found",
                    Errors = new List<string> { "Invalid transaction ID provided" }
                };
            }

            // Validate reversal conditions
            if (originalTransaction.Status == "Reversed")
            {
                return new ReversalResult
                {
                    Success = false,
                    Message = "Transaction already reversed",
                    Errors = new List<string> { "Cannot reverse an already reversed transaction" }
                };
            }

            // Transaction reversals require approval
            var makerAction = new MakerAction
            {
                ActionType = "TransactionReversal",
                ResourceId = reversalRequest.OriginalTransactionId,
                ResourceType = "Transaction",
                Data = System.Text.Json.JsonSerializer.Serialize(reversalRequest),
                MakerId = requestedBy,
                BusinessJustification = reversalRequest.ReversalReason,
                Amount = reversalRequest.ReversalAmount ?? originalTransaction.Amount
            };

            var workflow = await _makerCheckerService.InitiateMakerActionAsync(makerAction);

            var reversalReference = await GenerateTransactionReferenceAsync("REV");

            return new ReversalResult
            {
                Success = true,
                ReversalReference = reversalReference,
                Status = "PendingApproval",
                Message = "Transaction reversal submitted for approval",
                WorkflowId = workflow.WorkflowId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transaction reversal for TransactionId: {TransactionId}", 
                reversalRequest.OriginalTransactionId);
            return new ReversalResult
            {
                Success = false,
                Message = "An error occurred while processing the reversal",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ProcessingResult> ProcessStandingInstructionAsync(Guid standingInstructionId, Guid processedBy)
    {
        try
        {
            _logger.LogInformation("Processing standing instruction: {StandingInstructionId}", standingInstructionId);

            var standingInstruction = await _context.StandingInstructions
                .Include(si => si.Account)
                .FirstOrDefaultAsync(si => si.Id == standingInstructionId);

            if (standingInstruction == null)
            {
                return new ProcessingResult
                {
                    Success = false,
                    Message = "Standing instruction not found"
                };
            }

            if (standingInstruction.Status != "Active")
            {
                return new ProcessingResult
                {
                    Success = false,
                    Message = "Standing instruction is not active"
                };
            }

            // Check if it's time to execute
            if (standingInstruction.NextExecutionDate > DateTime.UtcNow.Date)
            {
                return new ProcessingResult
                {
                    Success = false,
                    Message = "Standing instruction is not due for execution"
                };
            }

            // Validate account balance
            if (standingInstruction.Account.AvailableBalance < standingInstruction.Amount)
            {
                return new ProcessingResult
                {
                    Success = false,
                    Message = "Insufficient funds for standing instruction execution"
                };
            }

            // Execute standing instruction
            var transactionReference = await GenerateTransactionReferenceAsync("SI");
            
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionReference,
                AccountId = standingInstruction.AccountId,
                TransactionType = "StandingInstruction",
                Amount = standingInstruction.Amount,
                Currency = standingInstruction.Account.Currency,
                Description = $"Standing Instruction: {standingInstruction.BeneficiaryName}",
                Status = "Completed",
                TransactionDate = DateTime.UtcNow,
                ValueDate = DateTime.UtcNow,
                Channel = "BackOffice"
            };

            // Update account balance
            standingInstruction.Account.Balance -= standingInstruction.Amount;
            standingInstruction.Account.AvailableBalance -= standingInstruction.Amount;
            transaction.RunningBalance = standingInstruction.Account.Balance;

            // Update next execution date
            standingInstruction.NextExecutionDate = CalculateNextExecutionDate(
                standingInstruction.NextExecutionDate, 
                standingInstruction.Frequency);

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new ProcessingResult
            {
                Success = true,
                Status = "Completed",
                Message = "Standing instruction executed successfully",
                ProcessedDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing standing instruction: {StandingInstructionId}", standingInstructionId);
            return new ProcessingResult
            {
                Success = false,
                Message = "An error occurred while processing standing instruction"
            };
        }
    }

    #endregion
    #region Validation and Business Rules

    public async Task<BackOfficeValidationResult> ValidateAccountOperationAsync(string operationType, Guid accountId, decimal? amount = null)
    {
        try
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return new BackOfficeValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { "Account not found" }
                };
            }

            var errors = new List<string>();
            var warnings = new List<string>();

            // Common validations
            if (account.Status != "Active")
            {
                errors.Add($"Account is not active. Current status: {account.Status}");
            }

            // Operation-specific validations
            switch (operationType.ToLower())
            {
                case "debit":
                    if (amount.HasValue && account.AvailableBalance < amount.Value)
                    {
                        errors.Add("Insufficient available balance");
                    }
                    break;

                case "closure":
                    if (account.Balance != 0)
                    {
                        errors.Add("Account balance must be zero for closure");
                    }
                    break;

                case "freeze":
                    if (account.IsFrozen)
                    {
                        warnings.Add("Account is already frozen");
                    }
                    break;
            }

            return new BackOfficeValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors,
                Warnings = warnings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating account operation: {OperationType} for AccountId: {AccountId}", 
                operationType, accountId);
            return new BackOfficeValidationResult
            {
                IsValid = false,
                Errors = new List<string> { "Validation error occurred" }
            };
        }
    }

    public async Task<List<BusinessRuleViolation>> CheckBusinessRulesAsync(string operationType, object requestData)
    {
        var violations = new List<BusinessRuleViolation>();

        try
        {
            switch (operationType.ToLower())
            {
                case "accountcreation":
                    if (requestData is AccountCreationRequest accountRequest)
                    {
                        violations.AddRange(await ValidateAccountCreationRules(accountRequest));
                    }
                    break;

                case "internaltransfer":
                    if (requestData is InternalTransferRequest transferRequest)
                    {
                        violations.AddRange(await ValidateTransferRules(transferRequest));
                    }
                    break;

                // Add more operation types as needed
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking business rules for operation: {OperationType}", operationType);
            violations.Add(new BusinessRuleViolation
            {
                RuleCode = "SYSTEM_ERROR",
                RuleName = "System Error",
                Description = "An error occurred while validating business rules",
                Severity = "Error",
                CanOverride = false
            });
        }

        return violations;
    }

    #endregion

    #region Reporting and Inquiry

    public async Task<List<PendingOperation>> GetPendingOperationsAsync(Guid officerId)
    {
        try
        {
            // Get pending workflows assigned to this officer
            var pendingWorkflows = await _context.WorkflowInstances
                .Where(w => w.Status == "Pending" || w.Status == "InProgress")
                .Include(w => w.ApprovalSteps)
                .ToListAsync();

            var pendingOperations = new List<PendingOperation>();

            foreach (var workflow in pendingWorkflows)
            {
                var operation = new PendingOperation
                {
                    OperationId = workflow.WorkflowId,
                    OperationType = workflow.WorkflowType,
                    Description = $"{workflow.WorkflowType} - {workflow.ResourceType}",
                    CreatedDate = workflow.InitiatedAt,
                    Priority = DeterminePriority(workflow),
                    Status = workflow.Status
                };

                if (workflow.ResourceType == "Account")
                {
                    operation.AccountId = workflow.ResourceId;
                }
                else if (workflow.ResourceType == "Customer")
                {
                    operation.CustomerId = workflow.ResourceId;
                }

                pendingOperations.Add(operation);
            }

            return pendingOperations.OrderBy(p => p.CreatedDate).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending operations for OfficerId: {OfficerId}", officerId);
            return new List<PendingOperation>();
        }
    }

    public async Task<List<AccountOperation>> GetAccountOperationHistoryAsync(Guid accountId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            var query = _context.BankingAuditLogs
                .Where(al => al.ResourceType == "Account" && al.ResourceId == accountId);

            if (fromDate.HasValue)
            {
                query = query.Where(al => al.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(al => al.Timestamp <= toDate.Value);
            }

            var auditLogs = await query
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();

            return auditLogs.Select(al => new AccountOperation
            {
                OperationId = al.AuditId,
                OperationType = al.Action,
                AccountId = accountId,
                Description = al.Action,
                OperationDate = al.Timestamp,
                PerformedBy = al.UserId,
                Status = "Completed",
                Result = "Success"
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account operation history for AccountId: {AccountId}", accountId);
            return new List<AccountOperation>();
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task<string> GenerateAccountNumberAsync(string accountType, string branchCode)
    {
        // Simple account number generation - in real implementation, use proper numbering scheme
        var prefix = accountType.Substring(0, Math.Min(2, accountType.Length)).ToUpper();
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"{branchCode}{prefix}{timestamp}{random}";
    }

    private async Task<string> GenerateTransactionReferenceAsync(string prefix)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(100000, 999999);
        return $"{prefix}{timestamp}{random}";
    }

    private async Task<Guid> ExecuteTransferAsync(InternalTransferRequest request, string transactionReference, Guid initiatedBy)
    {
        var fromAccount = await _context.Accounts.FindAsync(request.FromAccountId);
        var toAccount = await _context.Accounts.FindAsync(request.ToAccountId);

        // Debit from account
        var debitTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionReference + "_DR",
            AccountId = request.FromAccountId,
            TransactionType = "InternalTransfer",
            Amount = -request.Amount,
            Currency = request.Currency,
            Description = request.Narration,
            Status = "Completed",
            TransactionDate = DateTime.UtcNow,
            ValueDate = request.ValueDate,
            Channel = "BackOffice"
        };

        fromAccount!.Balance -= request.Amount;
        fromAccount.AvailableBalance -= request.Amount;
        debitTransaction.RunningBalance = fromAccount.Balance;

        // Credit to account
        var creditTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionReference + "_CR",
            AccountId = request.ToAccountId,
            TransactionType = "InternalTransfer",
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Narration,
            Status = "Completed",
            TransactionDate = DateTime.UtcNow,
            ValueDate = request.ValueDate,
            Channel = "BackOffice"
        };

        toAccount!.Balance += request.Amount;
        toAccount.AvailableBalance += request.Amount;
        creditTransaction.RunningBalance = toAccount.Balance;

        _context.Transactions.AddRange(debitTransaction, creditTransaction);
        await _context.SaveChangesAsync();

        return debitTransaction.Id;
    }

    private async Task<Guid> ExecuteAdjustmentAsync(AdjustmentRequest request, string transactionReference, Guid processedBy)
    {
        var account = await _context.Accounts.FindAsync(request.AccountId);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionReference,
            AccountId = request.AccountId,
            TransactionType = request.AdjustmentType,
            Amount = request.AdjustmentType == "Charge" ? -request.Amount : request.Amount,
            Currency = account!.Currency,
            Description = request.Reason,
            Status = "Completed",
            TransactionDate = DateTime.UtcNow,
            ValueDate = request.ValueDate,
            Channel = "BackOffice"
        };

        if (request.AdjustmentType == "Charge")
        {
            account.Balance -= request.Amount;
            account.AvailableBalance -= request.Amount;
        }
        else
        {
            account.Balance += request.Amount;
            account.AvailableBalance += request.Amount;
        }

        transaction.RunningBalance = account.Balance;

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction.Id;
    }

    private List<string> ValidateSignatoryRules(List<SignatoryRuleRequest> signatoryRules)
    {
        var errors = new List<string>();

        foreach (var rule in signatoryRules)
        {
            if (string.IsNullOrEmpty(rule.SignatoryType))
            {
                errors.Add("Signatory type is required");
            }

            if (rule.MinimumSignatures < 1)
            {
                errors.Add("Minimum signatures must be at least 1");
            }

            if (rule.SignatoryType == "Joint" && rule.MinimumSignatures > rule.AuthorizedSignatories.Count)
            {
                errors.Add("Minimum signatures cannot exceed number of authorized signatories");
            }
        }

        return errors;
    }

    private async Task<List<BusinessRuleViolation>> ValidateAccountCreationRules(AccountCreationRequest request)
    {
        var violations = new List<BusinessRuleViolation>();

        // Check minimum deposit requirements
        var minimumDeposit = GetMinimumDepositForAccountType(request.AccountType);
        if (request.InitialDeposit < minimumDeposit)
        {
            violations.Add(new BusinessRuleViolation
            {
                RuleCode = "MIN_DEPOSIT",
                RuleName = "Minimum Deposit Requirement",
                Description = $"Initial deposit must be at least {minimumDeposit:C}",
                Severity = "Error",
                CanOverride = false
            });
        }

        // Check for duplicate accounts
        var existingAccounts = await _context.Accounts
            .Where(a => a.CustomerId == request.CustomerId && a.AccountType == request.AccountType && a.Status == "Active")
            .CountAsync();

        if (existingAccounts > 0 && request.AccountType == "Savings")
        {
            violations.Add(new BusinessRuleViolation
            {
                RuleCode = "DUPLICATE_SAVINGS",
                RuleName = "Duplicate Savings Account",
                Description = "Customer already has an active savings account",
                Severity = "Warning",
                CanOverride = true,
                OverrideAuthority = "BranchManager"
            });
        }

        return violations;
    }

    private async Task<List<BusinessRuleViolation>> ValidateTransferRules(InternalTransferRequest request)
    {
        var violations = new List<BusinessRuleViolation>();

        // Check daily transfer limits
        var dailyTransfers = await _context.Transactions
            .Where(t => t.AccountId == request.FromAccountId && 
                       t.TransactionDate.Date == DateTime.UtcNow.Date &&
                       t.TransactionType == "InternalTransfer")
            .SumAsync(t => Math.Abs(t.Amount));

        if (dailyTransfers + request.Amount > 100000) // Daily limit
        {
            violations.Add(new BusinessRuleViolation
            {
                RuleCode = "DAILY_LIMIT",
                RuleName = "Daily Transfer Limit",
                Description = "Transfer would exceed daily limit of $100,000",
                Severity = "Warning",
                CanOverride = true,
                OverrideAuthority = "BranchManager"
            });
        }

        return violations;
    }

    private decimal GetMinimumDepositForAccountType(string accountType)
    {
        return accountType.ToLower() switch
        {
            "savings" => 100,
            "current" => 1000,
            "corporate" => 10000,
            _ => 100
        };
    }

    private int GetSeverityLevel(string severity)
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

    private string GetRiskLevelFromSeverity(int severityLevel)
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

    private DateTime CalculateNextExecutionDate(DateTime currentDate, string frequency)
    {
        return frequency.ToLower() switch
        {
            "daily" => currentDate.AddDays(1),
            "weekly" => currentDate.AddDays(7),
            "monthly" => currentDate.AddMonths(1),
            "quarterly" => currentDate.AddMonths(3),
            "annually" => currentDate.AddYears(1),
            _ => currentDate.AddMonths(1)
        };
    }

    private string DeterminePriority(WorkflowInstance workflow)
    {
        // Determine priority based on workflow type and age
        var age = DateTime.UtcNow - workflow.InitiatedAt;
        
        if (workflow.WorkflowType.Contains("AML") || workflow.WorkflowType.Contains("Violation"))
        {
            return "High";
        }
        
        if (age.TotalDays > 2)
        {
            return "High";
        }
        
        if (age.TotalDays > 1)
        {
            return "Medium";
        }
        
        return "Normal";
    }

    #endregion
}