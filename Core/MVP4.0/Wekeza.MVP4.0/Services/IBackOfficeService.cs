using Wekeza.MVP4._0.Models;

namespace Wekeza.MVP4._0.Services;

/// <summary>
/// Interface for Back Office Officer services handling account operations, KYC processing, and transaction management
/// </summary>
public interface IBackOfficeService
{
    // Account Operations
    Task<AccountCreationResult> CreateCustomerAccountAsync(AccountCreationRequest request);
    Task<VerificationResult> VerifyAccountOpeningAsync(Guid accountId, VerificationData verificationData);
    Task<ClosureResult> ProcessAccountClosureAsync(Guid accountId, string closureReason, Guid requestedBy);
    Task<UpdateResult> UpdateSignatoryRulesAsync(Guid accountId, List<SignatoryRuleRequest> signatoryRules, Guid requestedBy);
    Task<UpdateResult> UpdateAccountMandateAsync(Guid accountId, AccountMandateRequest mandateRequest, Guid requestedBy);
    
    // KYC & Compliance Processing
    Task<KYCResult> ProcessKYCUpdateAsync(Guid customerId, KYCUpdateRequest kycData, Guid processedBy);
    Task<DocumentResult> ProcessDocumentUploadAsync(Guid customerId, List<DocumentUploadRequest> documents, Guid uploadedBy);
    Task<FlagResult> FlagHighRiskCustomerAsync(Guid customerId, List<RiskIndicatorRequest> riskIndicators, Guid flaggedBy);
    Task<BackOfficeEscalationResult> EscalateAMLViolationAsync(Guid customerId, AMLViolationRequest violationDetails, Guid escalatedBy);
    Task<UpdateResult> UpdateRiskRatingAsync(Guid customerId, RiskRatingUpdateRequest riskUpdate, Guid updatedBy);
    
    // Transaction Processing
    Task<TransferResult> PostInternalTransferAsync(InternalTransferRequest transferRequest, Guid initiatedBy);
    Task<AdjustmentResult> ProcessChargesAndAdjustmentsAsync(AdjustmentRequest adjustmentRequest, Guid processedBy);
    Task<ReversalResult> HandleTransactionReversalAsync(ReversalRequest reversalRequest, Guid requestedBy);
    Task<ProcessingResult> ProcessStandingInstructionAsync(Guid standingInstructionId, Guid processedBy);
    
    // Validation and Business Rules
    Task<BackOfficeValidationResult> ValidateAccountOperationAsync(string operationType, Guid accountId, decimal? amount = null);
    Task<List<BusinessRuleViolation>> CheckBusinessRulesAsync(string operationType, object requestData);
    
    // Reporting and Inquiry
    Task<List<PendingOperation>> GetPendingOperationsAsync(Guid officerId);
    Task<List<AccountOperation>> GetAccountOperationHistoryAsync(Guid accountId, DateTime? fromDate = null, DateTime? toDate = null);
}

// Request/Response Models for Back Office Operations
public class AccountCreationRequest
{
    public Guid CustomerId { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public decimal InitialDeposit { get; set; }
    public List<SignatoryRuleRequest> SignatoryRules { get; set; } = new();
    public string OperatingInstructions { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class SignatoryRuleRequest
{
    public string SignatoryType { get; set; } = string.Empty; // Single, Joint, Either
    public int MinimumSignatures { get; set; } = 1;
    public decimal? MaximumAmount { get; set; }
    public List<Guid> AuthorizedSignatories { get; set; } = new();
}

public class VerificationData
{
    public bool KYCCompliant { get; set; }
    public List<string> DocumentsVerified { get; set; } = new();
    public string VerificationNotes { get; set; } = string.Empty;
    public bool RequiresManagerApproval { get; set; }
    public Dictionary<string, bool> ComplianceChecks { get; set; } = new();
}

public class AccountMandateRequest
{
    public string MandateType { get; set; } = string.Empty;
    public List<Guid> AuthorizedSignatories { get; set; } = new();
    public decimal? TransactionLimit { get; set; }
    public string OperatingInstructions { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
}

public class KYCUpdateRequest
{
    public List<DocumentUploadRequest> Documents { get; set; } = new();
    public string RiskRating { get; set; } = string.Empty;
    public List<ComplianceCheckRequest> ComplianceChecks { get; set; } = new();
    public string ReviewNotes { get; set; } = string.Empty;
    public bool RequiresManagerReview { get; set; }
}

public class DocumentUploadRequest
{
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public string DocumentStatus { get; set; } = "Pending";
    public byte[]? FileContent { get; set; }
}

public class ComplianceCheckRequest
{
    public string CheckType { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CheckedDate { get; set; }
}

public class RiskIndicatorRequest
{
    public string IndicatorType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Source { get; set; } = string.Empty;
    public DateTime IdentifiedDate { get; set; }
}

public class AMLViolationRequest
{
    public string ViolationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? TransactionAmount { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public DateTime ViolationDate { get; set; }
    public string RegulatoryReference { get; set; } = string.Empty;
    public bool RequiresImmediateAction { get; set; }
}

public class RiskRatingUpdateRequest
{
    public string NewRiskRating { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public List<string> SupportingFactors { get; set; } = new();
    public DateTime EffectiveDate { get; set; }
}

public class InternalTransferRequest
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string TransactionReference { get; set; } = string.Empty;
    public string Narration { get; set; } = string.Empty;
    public DateTime ValueDate { get; set; }
    public bool RequiresApproval { get; set; }
}

public class AdjustmentRequest
{
    public Guid AccountId { get; set; }
    public string AdjustmentType { get; set; } = string.Empty; // Charge, Credit, Fee, Reversal
    public decimal Amount { get; set; }
    public string GLCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public DateTime ValueDate { get; set; }
    public bool RequiresApproval { get; set; }
}

public class ReversalRequest
{
    public Guid OriginalTransactionId { get; set; }
    public string ReversalReason { get; set; } = string.Empty;
    public string AuthorizationCode { get; set; } = string.Empty;
    public bool PartialReversal { get; set; }
    public decimal? ReversalAmount { get; set; }
    public DateTime RequestedDate { get; set; }
}

// Result Models
public class AccountCreationResult
{
    public bool Success { get; set; }
    public Guid? AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? WorkflowId { get; set; }
    public bool RequiresApproval { get; set; }
}

public class VerificationResult
{
    public bool Success { get; set; }
    public bool Approved { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Issues { get; set; } = new();
    public Guid? WorkflowId { get; set; }
}

public class ClosureResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime? ClosureDate { get; set; }
    public Guid? WorkflowId { get; set; }
    public bool RequiresApproval { get; set; }
}

public class UpdateResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? WorkflowId { get; set; }
    public bool RequiresApproval { get; set; }
}

public class KYCResult
{
    public bool Success { get; set; }
    public string KYCStatus { get; set; } = string.Empty;
    public string RiskRating { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Issues { get; set; } = new();
    public bool RequiresManagerReview { get; set; }
}

public class DocumentResult
{
    public bool Success { get; set; }
    public List<Guid> DocumentIds { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public int ProcessedCount { get; set; }
    public int FailedCount { get; set; }
}

public class FlagResult
{
    public bool Success { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool RequiresImmediateAction { get; set; }
    public Guid? EscalationWorkflowId { get; set; }
}

public class BackOfficeEscalationResult
{
    public bool Success { get; set; }
    public string EscalationLevel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? WorkflowId { get; set; }
    public DateTime? ExpectedResolutionDate { get; set; }
}

public class TransferResult
{
    public bool Success { get; set; }
    public Guid? TransactionId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? WorkflowId { get; set; }
}

public class AdjustmentResult
{
    public bool Success { get; set; }
    public Guid? TransactionId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? WorkflowId { get; set; }
}

public class ReversalResult
{
    public bool Success { get; set; }
    public Guid? ReversalTransactionId { get; set; }
    public string ReversalReference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? WorkflowId { get; set; }
}

public class ProcessingResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public DateTime? ProcessedDate { get; set; }
}

public class BackOfficeValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, object> ValidationData { get; set; } = new();
}

public class BusinessRuleViolation
{
    public string RuleCode { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Warning, Error, Critical
    public bool CanOverride { get; set; }
    public string OverrideAuthority { get; set; } = string.Empty;
}

public class PendingOperation
{
    public Guid OperationId { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public Guid? AccountId { get; set; }
    public Guid? CustomerId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class AccountOperation
{
    public Guid OperationId { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OperationDate { get; set; }
    public Guid PerformedBy { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
}