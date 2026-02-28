using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Branch Admin Service - Branch Operations, Teller Management, Cash Management, Branch Users
/// Branch operations portal for managing branch-level operations, tellers, cash, and staff
/// </summary>
public interface IBranchAdminService
{
    // ===== Branch Management =====
    Task<BranchDTO> GetBranchAsync(Guid branchId);
    Task<List<BranchDTO>> GetAllBranchesAsync(int page = 1, int pageSize = 50);
    Task<BranchDTO> UpdateBranchAsync(Guid branchId, UpdateBranchRequest request, Guid updatedByUserId);
    Task CloseBranchAsync(Guid branchId, string closureReason, Guid closedByUserId);
    Task<BranchOperationalStatusDTO> GetBranchStatusAsync(Guid branchId);

    // ===== Teller Management =====
    Task<TellerDTO> GetTellerAsync(Guid tellerId);
    Task<List<TellerDTO>> GetBranchTellersAsync(Guid branchId, int page = 1, int pageSize = 50);
    Task<TellerSessionDTO> StartTellerSessionAsync(Guid tellerId, Guid branchId, decimal openingBalance, Guid startedByUserId);
    Task<TellerSessionDTO> EndTellerSessionAsync(Guid sessionId, decimal closingBalance, Guid endedByUserId);
    Task<TellerSessionDTO> GetTellerSessionAsync(Guid sessionId);
    Task<List<TellerSessionDTO>> GetTellerSessionHistoryAsync(Guid tellerId, DateTime? fromDate = null, DateTime? toDate = null);
    Task SuspendTellerAsync(Guid tellerId, string reason, Guid suspendedByUserId);
    Task ReactivateTellerAsync(Guid tellerId, Guid reactivatedByUserId);

    // ===== Cash Management =====
    Task<CashDrawerDTO> GetCashDrawerAsync(Guid drawerId);
    Task<List<CashDrawerDTO>> GetBranchCashDrawersAsync(Guid branchId);
    Task<CashDrawerDTO> OpenCashDrawerAsync(Guid drawerId, decimal openingBalance, Guid openedByUserId);
    Task<CashDrawerDTO> CloseCashDrawerAsync(Guid drawerId, decimal closingBalance, Guid closedByUserId);
    Task<CashCountDTO> CashCountAsync(Guid drawerId, Guid countedByUserId);
    Task ReconcileCashAsync(Guid drawerId, decimal expectedBalance, Guid reconciledByUserId);
    Task ReportCashSurplusAsync(Guid drawerId, decimal surplusAmount, string reason, Guid reportedByUserId);
    Task ReportCashShortageAsync(Guid drawerId, decimal shortageAmount, string reason, Guid reportedByUserId);

    // ===== Teller Transaction Management =====
    Task<TellerTransactionDTO> GetTellerTransactionAsync(Guid transactionId);
    Task<List<TellerTransactionDTO>> SearchTellerTransactionsAsync(Guid? tellerId = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50);
    Task ReverseTellerTransactionAsync(Guid transactionId, string reversalReason, Guid reversalByUserId);
    Task<TellerTransactionSummaryDTO> GetTellerTransactionSummaryAsync(Guid tellerId, DateTime fromDate, DateTime toDate);

    // ===== Branch User Management =====
    Task<BranchUserDTO> GetBranchUserAsync(Guid userId, Guid branchId);
    Task<List<BranchUserDTO>> GetBranchUsersAsync(Guid branchId, int page = 1, int pageSize = 50);
    Task<BranchUserDTO> AssignUserToBranchAsync(Guid userId, Guid branchId, string position, Guid assignedByUserId);
    Task RemoveUserFromBranchAsync(Guid userId, Guid branchId, string reason, Guid removedByUserId);
    Task UpdateBranchUserRoleAsync(Guid userId, Guid branchId, string newRole, Guid updatedByUserId);
    Task<List<BranchUserDTO>> GetBranchStaffAsync(Guid branchId, string? department = null);

    // ===== Branch Inventory Management =====
    Task<BranchInventoryDTO> GetBranchInventoryAsync(Guid branchId);
    Task<StockItemDTO> GetStockItemAsync(Guid branchId, string itemCode);
    Task UpdateStockAsync(Guid branchId, string itemCode, int quantity, string reason, Guid updatedByUserId);
    Task<List<LowStockAlertDTO>> GetLowStockAlertsAsync(Guid branchId);
    Task<InventoryReportDTO> GenerateInventoryReportAsync(Guid branchId, Guid generatedByUserId);

    // ===== Branch Reporting =====
    Task<BranchDailyReportDTO> GenerateDailyReportAsync(Guid branchId, DateTime reportDate, Guid generatedByUserId);
    Task<List<BranchPerformanceMetricDTO>> GetBranchMetricsAsync(Guid branchId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<BranchComplianceReportDTO> GenerateComplianceReportAsync(Guid branchId, DateTime? fromDate = null, DateTime? toDate = null, Guid generatedByUserId);

    // ===== Branch Dashboard =====
    Task<BranchDashboardDTO> GetBranchDashboardAsync(Guid branchId);
    Task<List<BranchAlertDTO>> GetBranchAlertsAsync(Guid branchId);
}

// DTOs
public class BranchDTO
{
    public Guid Id { get; set; }
    public string BranchCode { get; set; }
    public string BranchName { get; set; }
    public string Location { get; set; }
    public string Region { get; set; }
    public string Status { get; set; }
    public int StaffCount { get; set; }
    public decimal TellersAvailable { get; set; }
    public decimal CashOnHand { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateBranchRequest
{
    public string BranchName { get; set; }
    public string Location { get; set; }
    public Dictionary<string, object> BranchDetails { get; set; }
}

public class BranchOperationalStatusDTO
{
    public Guid BranchId { get; set; }
    public bool IsOpen { get; set; }
    public int ActiveTellers { get; set; }
    public decimal CashOnHand { get; set; }
    public int PendingTransactions { get; set; }
    public string OverallStatus { get; set; }
}

public class TellerDTO
{
    public Guid Id { get; set; }
    public string TellerCode { get; set; }
    public string UserName { get; set; }
    public Guid BranchId { get; set; }
    public string Status { get; set; }
    public DateTime? LastLoggedInAt { get; set; }
    public decimal CurrentBalance { get; set; }
}

public class TellerSessionDTO
{
    public Guid Id { get; set; }
    public Guid TellerId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public string Status { get; set; }
    public int TransactionCount { get; set; }
}

public class CashDrawerDTO
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string DrawerCode { get; set; }
    public string Status { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime LastCountedAt { get; set; }
    public string LastCountedBy { get; set; }
}

public class CashCountDTO
{
    public Guid DrawerId { get; set; }
    public decimal CurrencyNotes { get; set; }
    public decimal Coins { get; set; }
    public decimal Checks { get; set; }
    public decimal TotalCounted { get; set; }
    public decimal SystemBalance { get; set; }
    public decimal Variance { get; set; }
    public DateTime CountedAt { get; set; }
}

public class TellerTransactionDTO
{
    public Guid Id { get; set; }
    public Guid TellerId { get; set; }
    public string TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; }
    public string Reference { get; set; }
}

public class TellerTransactionSummaryDTO
{
    public Guid TellerId { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalCash { get; set; }
    public decimal TotalChecks { get; set; }
    public decimal AverageTransaction { get; set; }
}

public class BranchUserDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public Guid BranchId { get; set; }
    public string Position { get; set; }
    public string Department { get; set; }
    public string Status { get; set; }
    public DateTime AssignedAt { get; set; }
}

public class BranchInventoryDTO
{
    public Guid BranchId { get; set; }
    public List<StockItemDTO> Items { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime LastCountedAt { get; set; }
}

public class StockItemDTO
{
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public int QuantityOnHand { get; set; }
    public int MinimumQty { get; set; }
    public int MaximumQty { get; set; }
    public decimal UnitCost { get; set; }
    public string Status { get; set; }
}

public class LowStockAlertDTO
{
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public int CurrentQty { get; set; }
    public int MinimumQty { get; set; }
    public int DaysToStockout { get; set; }
}

public class InventoryReportDTO
{
    public Guid BranchId { get; set; }
    public DateTime ReportDate { get; set; }
    public int TotalItems { get; set; }
    public int LowStockItems { get; set; }
    public decimal TotalValue { get; set; }
}

public class BranchDailyReportDTO
{
    public Guid BranchId { get; set; }
    public DateTime ReportDate { get; set; }
    public int ActiveTellers { get; set; }
    public int TransactionsProcessed { get; set; }
    public decimal CashTransacted { get; set; }
    public decimal CashVariance { get; set; }
    public List<string> Issues { get; set; }
}

public class BranchPerformanceMetricDTO
{
    public string MetricName { get; set; }
    public decimal Value { get; set; }
    public decimal Target { get; set; }
    public decimal PercentageOfTarget { get; set; }
    public DateTime RecordedAt { get; set; }
}

public class BranchComplianceReportDTO
{
    public Guid BranchId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int ComplianceIssues { get; set; }
    public int PolicyViolations { get; set; }
    public double ComplianceScore { get; set; }
}

public class BranchDashboardDTO
{
    public Guid BranchId { get; set; }
    public string BranchName { get; set; }
    public bool IsOpen { get; set; }
    public int ActiveTellers { get; set; }
    public decimal CashOnHand { get; set; }
    public int DailyTransactions { get; set; }
    public decimal DailyVolume { get; set; }
    public List<BranchAlertDTO> Alerts { get; set; }
}

public class BranchAlertDTO
{
    public Guid AlertId { get; set; }
    public string AlertType { get; set; }
    public string Message { get; set; }
    public string Severity { get; set; }
    public DateTime CreatedAt { get; set; }
}
