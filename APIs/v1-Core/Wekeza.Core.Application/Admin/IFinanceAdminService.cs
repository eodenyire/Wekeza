using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Finance Admin Service - GL Accounts, Reconciliation, Financial Reports, Interest Accrual
/// Finance operations portal for managing GL accounts, reconciliations, financial reporting, and accruals
/// </summary>
public interface IFinanceAdminService
{
    // ===== GL Account Management =====
    Task<GLAccountDTO> GetGLAccountAsync(string glCode);
    Task<List<GLAccountDTO>> SearchGLAccountsAsync(string? accountType = null, string? status = null, int page = 1, int pageSize = 50);
    Task<GLAccountDTO> CreateGLAccountAsync(CreateGLAccountRequest request, Guid createdByUserId);
    Task<GLAccountDTO> UpdateGLAccountAsync(string glCode, UpdateGLAccountRequest request, Guid updatedByUserId);
    Task ActivateGLAccountAsync(string glCode, Guid activatedByUserId);
    Task DeactivateGLAccountAsync(string glCode, string reason, Guid deactivatedByUserId);
    Task<GLAccountBalanceDTO> GetAccountBalanceAsync(string glCode, DateTime asOfDate);

    // ===== Journal Entry Management =====
    Task<JournalEntryDTO> GetJournalEntryAsync(Guid journalId);
    Task<List<JournalEntryDTO>> SearchJournalEntriesAsync(string? glCode = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50);
    Task<JournalEntryDTO> CreateJournalEntryAsync(CreateJournalEntryRequest request, Guid createdByUserId);
    Task<JournalEntryDTO> UpdateJournalEntryAsync(Guid journalId, UpdateJournalEntryRequest request, Guid updatedByUserId);
    Task ApproveJournalEntryAsync(Guid journalId, Guid approverUserId);
    Task RejectJournalEntryAsync(Guid journalId, string rejectionReason, Guid rejectedByUserId);
    Task PostJournalEntryAsync(Guid journalId, Guid postedByUserId);
    Task ReverseJournalEntryAsync(Guid journalId, string reversalReason, Guid reversalByUserId);

    // ===== Reconciliation Management =====
    Task<ReconciliationDTO> GetReconciliationAsync(Guid reconciliationId);
    Task<List<ReconciliationDTO>> SearchReconciliationsAsync(string? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50);
    Task<ReconciliationDTO> InitiateReconciliationAsync(InitiateReconciliationRequest request, Guid initiatedByUserId);
    Task<ReconciliationDTO> ReconcileAsync(Guid reconciliationId, ReconcileTransactionsRequest request, Guid reconciledByUserId);
    Task ApproveReconciliationAsync(Guid reconciliationId, Guid approverUserId);
    Task RejectReconciliationAsync(Guid reconciliationId, string rejectionReason, Guid rejectedByUserId);
    Task<ReconciliationDiscrepancyDTO> GetDiscrepanciesAsync(Guid reconciliationId);
    Task<ReconciliationReportDTO> GenerateReconciliationReportAsync(DateTime fromDate, DateTime toDate, Guid generatedByUserId);

    // ===== Interest Accrual Management =====
    Task<InterestAccrualDTO> GetAccrualAsync(Guid accrualId);
    Task<List<InterestAccrualDTO>> GetPendingAccrualsAsync(DateTime asOfDate, int page = 1, int pageSize = 50);
    Task<InterestAccrualDTO> CalculateAccrualAsync(CalculateInterestAccrualRequest request, Guid calculatedByUserId);
    Task ApproveAccrualAsync(Guid accrualId, Guid approverUserId);
    Task RejectAccrualAsync(Guid accrualId, string rejectionReason, Guid rejectedByUserId);
    Task PostAccrualAsync(Guid accrualId, Guid postedByUserId);
    Task<InterestAccrualSummaryDTO> GetAccrualSummaryAsync(DateTime asOfDate);

    // ===== Financial Reporting =====
    Task<TrialBalanceDTO> GenerateTrialBalanceAsync(DateTime asOfDate, Guid generatedByUserId);
    Task<BalanceSheetDTO> GenerateBalanceSheetAsync(DateTime asOfDate, Guid generatedByUserId);
    Task<IncomeStatementDTO> GenerateIncomeStatementAsync(DateTime fromDate, DateTime toDate, Guid generatedByUserId);
    Task<CashFlowStatementDTO> GenerateCashFlowStatementAsync(DateTime fromDate, DateTime toDate, Guid generatedByUserId);
    Task<FinancialRatioAnalysisDTO> GenerateRatioAnalysisAsync(DateTime asOfDate, Guid generatedByUserId);
    Task<ManagementReportDTO> GenerateManagementReportAsync(DateTime fromDate, DateTime toDate, Guid generatedByUserId);

    // ===== Finance Dashboard =====
    Task<FinanceDashboardDTO> GetFinanceDashboardAsync(DateTime? asOfDate = null);
    Task<FinancialHealthDTO> GetFinancialHealthAsync(DateTime asOfDate);
    Task<List<FinancialAlertDTO> > GetFinancialAlertsAsync();
}

// DTOs
public class GLAccountDTO
{
    public string GLCode { get; set; }
    public string AccountName { get; set; }
    public string AccountType { get; set; }
    public string SubGroup { get; set; }
    public decimal DebitBalance { get; set; }
    public decimal CreditBalance { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateGLAccountRequest
{
    public string GLCode { get; set; }
    public string AccountName { get; set; }
    public string AccountType { get; set; }
    public string SubGroup { get; set; }
    public string CostCenter { get; set; }
    public string Description { get; set; }
}

public class UpdateGLAccountRequest
{
    public string AccountName { get; set; }
    public string CostCenter { get; set; }
    public string Description { get; set; }
}

public class GLAccountBalanceDTO
{
    public string GLCode { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal Debits { get; set; }
    public decimal Credits { get; set; }
    public decimal ClosingBalance { get; set; }
    public DateTime AsOfDate { get; set; }
}

public class JournalEntryDTO
{
    public Guid Id { get; set; }
    public string VoucherNumber { get; set; }
    public DateTime EntryDate { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<JournalLineDTO> JournalLines { get; set; }
    public string CreatedBy { get; set; }
}

public class JournalLineDTO
{
    public string GLCode { get; set; }
    public string DebitOrCredit { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}

public class CreateJournalEntryRequest
{
    public string VoucherNumber { get; set; }
    public DateTime EntryDate { get; set; }
    public string Description { get; set; }
    public List<CreateJournalLineRequest> JournalLines { get; set; }
}

public class CreateJournalLineRequest
{
    public string GLCode { get; set; }
    public string DebitOrCredit { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}

public class UpdateJournalEntryRequest
{
    public string Description { get; set; }
    public List<CreateJournalLineRequest> JournalLines { get; set; }
}

public class ReconciliationDTO
{
    public Guid Id { get; set; }
    public DateTime ReconciliationDate { get; set; }
    public string BankName { get; set; }
    public decimal BankBalance { get; set; }
    public decimal GLBalance { get; set; }
    public decimal Variance { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InitiateReconciliationRequest
{
    public DateTime ReconciliationDate { get; set; }
    public string BankName { get; set; }
    public string BankStatement { get; set; }
    public decimal BankBalance { get; set; }
}

public class ReconcileTransactionsRequest
{
    public List<Guid> BankStatementTransactionIds { get; set; }
    public List<Guid> GLTransactionIds { get; set; }
    public List<ReconciliationAdjustmentRequest> Adjustments { get; set; }
}

public class ReconciliationAdjustmentRequest
{
    public string AdjustmentType { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}

public class ReconciliationDiscrepancyDTO
{
    public List<string> OutstandingDeposits { get; set; }
    public List<string> OutstandingChecks { get; set; }
    public decimal BankErrors { get; set; }
    public decimal GLErrors { get; set; }
}

public class ReconciliationReportDTO
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalReconciliations { get; set; }
    public int SuccessfulReconciliations { get; set; }
    public decimal TotalVariance { get; set; }
}

public class InterestAccrualDTO
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public decimal AccrualAmount { get; set; }
    public DateTime AccrualDate { get; set; }
    public string Status { get; set; }
    public int AffectedAccounts { get; set; }
}

public class CalculateInterestAccrualRequest
{
    public DateTime AccrualDate { get; set; }
    public List<string> ProductCodes { get; set; }
    public Dictionary<string, object> AccrualParameters { get; set; }
}

public class InterestAccrualSummaryDTO
{
    public decimal TotalAccrued { get; set; }
    public int ProcessedAccounts { get; set; }
    public int FailedAccounts { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class TrialBalanceDTO
{
    public DateTime AsOfDate { get; set; }
    public List<TrialBalanceLine> Lines { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public bool IsBalanced { get; set; }
}

public class TrialBalanceLine
{
    public string GLCode { get; set; }
    public string AccountName { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
}

public class BalanceSheetDTO
{
    public DateTime AsOfDate { get; set; }
    public BalanceSheetSectionDTO Assets { get; set; }
    public BalanceSheetSectionDTO Liabilities { get; set; }
    public BalanceSheetSectionDTO Equity { get; set; }
}

public class BalanceSheetSectionDTO
{
    public string SectionName { get; set; }
    public List<BalanceSheetLine> Lines { get; set; }
    public decimal TotalAmount { get; set; }
}

public class BalanceSheetLine
{
    public string ItemName { get; set; }
    public decimal Amount { get; set; }
}

public class IncomeStatementDTO
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetIncome { get; set; }
    public List<IncomeStatementLine> Lines { get; set; }
}

public class IncomeStatementLine
{
    public string ItemName { get; set; }
    public decimal Amount { get; set; }
}

public class CashFlowStatementDTO
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal OperatingCashFlow { get; set; }
    public decimal InvestingCashFlow { get; set; }
    public decimal FinancingCashFlow { get; set; }
    public decimal NetCashFlow { get; set; }
}

public class FinancialRatioAnalysisDTO
{
    public DateTime AsOfDate { get; set; }
    public double LiquidityRatio { get; set; }
    public double SolvencyRatio { get; set; }
    public double ProfitabilityRatio { get; set; }
    public double EfficiencyRatio { get; set; }
}

public class ManagementReportDTO
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<string> HighlightedMetrics { get; set; }
    public List<string> KeyInsights { get; set; }
    public List<string> Recommendations { get; set; }
}

public class FinanceDashboardDTO
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public double NetProfitMargin { get; set; }
    public double ROA { get; set; }
    public List<FinancialAlertDTO> Alerts { get; set; }
}

public class FinancialHealthDTO
{
    public double HealthScore { get; set; }
    public string Status { get; set; }
    public List<HealthIndicatorDTO> Indicators { get; set; }
}

public class HealthIndicatorDTO
{
    public string IndicatorName { get; set; }
    public double Value { get; set; }
    public string Status { get; set; }
}

public class FinancialAlertDTO
{
    public Guid AlertId { get; set; }
    public string AlertType { get; set; }
    public string Message { get; set; }
    public string Severity { get; set; }
    public DateTime CreatedAt { get; set; }
}
