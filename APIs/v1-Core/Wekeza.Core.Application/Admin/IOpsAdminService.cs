using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Operations Admin Service - Account, Transaction, and Exception Case Management
/// Production-grade operations admin portal for operational administrators
/// </summary>
public interface IOpsAdminService
{
    // ===== Account Management =====
    Task<List<AccountSummaryDTO>> SearchAccountsAsync(string? accountNumber = null, Guid? customerId = null, string? status = null, int page = 1, int pageSize = 50);
    Task<AccountDetailsDTO> GetAccountDetailsAsync(string accountNumber);
    Task<List<TransactionSummaryDTO>> GetAccountTransactionsAsync(string accountNumber, int days = 90, int page = 1, int pageSize = 50);
    Task LockAccountAsync(string accountNumber, string reason);
    Task UnlockAccountAsync(string accountNumber);
    Task FreezeAccountAsync(string accountNumber, string reason);
    Task UnfreezeAccountAsync(string accountNumber);
    
    // ===== Transaction Management =====
    Task<TransactionDetailsDTO> GetTransactionDetailsAsync(Guid transactionId);
    Task ReverseTransactionAsync(Guid transactionId, string reason, Guid requestedByUserId);
    Task<List<PendingReversalDTO>> GetPendingReversalsAsync(int page = 1, int pageSize = 50);
    Task ApproveReversalAsync(Guid reversalId, Guid approverUserId, string approvalReason);
    Task RejectReversalAsync(Guid reversalId, Guid approverUserId, string rejectionReason);
    
    // ===== Batch Job Management =====
    Task<BatchJobDTO> GetBatchJobAsync(Guid jobId);
    Task<List<BatchJobDTO>> GetAllBatchJobsAsync(int page = 1, int pageSize = 50);
    Task<List<BatchJobExecutionDTO>> GetBatchJobHistoryAsync(Guid jobId, int limit = 10);
    Task StartBatchJobAsync(Guid jobId);
    Task StopBatchJobAsync(Guid jobId);
    Task RestartBatchJobAsync(Guid jobId);
    Task DisableBatchJobAsync(Guid jobId, string reason);
    Task EnableBatchJobAsync(Guid jobId);
    Task<BatchJobMonitoringDTO> GetBatchMonitoringDashboardAsync();
    
    // ===== Exception Case Management =====
    Task<ExceptionCaseDetailsDTO> GetExceptionCaseAsync(string caseNumber);
    Task<List<ExceptionCaseSummaryDTO>> SearchExceptionCasesAsync(string? status = null, string? priority = null, string? category = null, int page = 1, int pageSize = 50);
    Task<ExceptionCaseDetailsDTO> CreateExceptionCaseAsync(CreateExceptionCaseRequest request);
    Task AssignExceptionCaseAsync(string caseNumber, Guid assignedToUserId);
    Task AddCommentToExceptionAsync(string caseNumber, string commentText, Guid commentByUserId);
    Task ResolveExceptionCaseAsync(string caseNumber, ResolveExceptionRequest request);
    Task EscalateExceptionCaseAsync(string caseNumber, string escalationReason);
    Task CloseExceptionCaseAsync(string caseNumber);
    
    // ===== Monitoring & Dashboards =====
    Task<OpsAdminDashboardDTO> GetOpsAdminDashboardAsync();
    Task<ExceptionMetricsDTO> GetExceptionMetricsAsync(DateTime fromDate, DateTime toDate);
    Task<BatchJobMetricsDTO> GetBatchJobMetricsAsync(DateTime fromDate, DateTime toDate);
    Task<List<AlertDTO>> GetActiveAlertsAsync();
    Task<List<AlertDTO>> GetAlertHistoryAsync(DateTime fromDate, DateTime toDate);
}

// ===== DTOs =====
public record AccountSummaryDTO(
    string AccountNumber,
    Guid CustomerId,
    decimal AvailableBalance,
    decimal LedgerBalance,
    string Status,
    string Currency);

public record AccountDetailsDTO(
    string AccountNumber,
    Guid CustomerId,
    decimal AvailableBalance,
    decimal LedgerBalance,
    decimal OverdraftLimit,
    string Status,
    string Currency,
    DateTime OpenedDate,
    DateTime? ClosedDate,
    string AccountType);

public record TransactionSummaryDTO(
    Guid TransactionId,
    string AccountNumber,
    decimal Amount,
    string Type,
    string Status,
    DateTime Timestamp);

public record TransactionDetailsDTO(
    Guid TransactionId,
    string AccountNumber,
    decimal Amount,
    string Type,
    string Status,
    string Description,
    DateTime Timestamp,
    string? ReversalTransactionId = null);

public record PendingReversalDTO(
    Guid ReversalId,
    Guid OriginalTransactionId,
    decimal Amount,
    string Reason,
    string RequestedBy,
    DateTime RequestedAt);

public record BatchJobDTO(
    Guid JobId,
    string JobCode,
    string JobName,
    string Status,
    string? NextScheduledRun = null,
    DateTime? LastRunTime = null);

public record BatchJobExecutionDTO(
    Guid ExecutionId,
    Guid JobId,
    DateTime StartedAt,
    DateTime? CompletedAt,
    string Status,
    int RecordsProcessed,
    int RecordsFailed);

public record BatchJobMonitoringDTO(
    int RunningJobsCount,
    int TotalJobsCount,
    int FailedJobsCount,
    DateTime? LastUpdateTime);

public record ExceptionCaseSummaryDTO(
    string CaseNumber,
    string CaseTitle,
    string Status,
    string Priority,
    DateTime CreatedAt,
    string? AssignedToUser = null,
    DateTime? SLA_DueDate = null);

public record ExceptionCaseDetailsDTO(
    string CaseNumber,
    string CaseTitle,
    string CaseDescription,
    string Status,
    string Priority,
    string Category,
    DateTime CreatedAt,
    string CreatedBy,
    string? AssignedToUser = null,
    DateTime? ResolvedAt = null,
    string? ResolutionReason = null);

public record OpsAdminDashboardDTO(
    int OpenExceptionCaseCount,
    int EscalatedCaseCount,
    int PendingReversalCount,
    int RunningBatchJobCount,
    int ActiveAlertCount);

public record ExceptionMetricsDTO(
    int TotalCases,
    int OpenCases,
    int ResolvedCases,
    int EscalatedCases,
    decimal AverageResolutionHours);

public record BatchJobMetricsDTO(
    int TotalExecutions,
    int SuccessfulExecutions,
    int FailedExecutions,
    long TotalRecordsProcessed,
    long TotalRecordsFailed);

public record AlertDTO(
    Guid AlertId,
    string AlertType,
    string Message,
    string Severity,
    DateTime CreatedAt,
    bool Acknowledged = false);

// ===== Commands/Requests =====
public record CreateExceptionCaseRequest(
    string CaseTitle,
    string CaseDescription,
    string ExceptionType,
    string Category,
    string Priority,
    string EntityType,
    string EntityId,
    Dictionary<string, object>? EntityDetails = null);

public record ResolveExceptionRequest(
    string ResolutionReason,
    string? RootCauseAnalysis = null,
    decimal? FinancialImpact = null);
