using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Dashboard.Queries.GetSystemStats;

/// <summary>
/// Query to get comprehensive system statistics and KPIs
/// </summary>
public record GetSystemStatsQuery : IQuery<SystemStatsDto>
{
    public DateTime? AsOfDate { get; init; } = DateTime.UtcNow;
}

public record SystemStatsDto
{
    // Customer Statistics
    public int TotalCustomers { get; init; }
    public int NewCustomersToday { get; init; }
    public int NewCustomersThisMonth { get; init; }
    public int ActiveCustomers { get; init; }
    public int CIFsWithoutAccounts { get; init; }
    public int KYCPendingCustomers { get; init; }
    
    // Account Statistics
    public int TotalAccounts { get; init; }
    public int ActiveAccounts { get; init; }
    public int InactiveAccounts { get; init; }
    public int FrozenAccounts { get; init; }
    public int AccountsOpenedToday { get; init; }
    public decimal TotalAccountBalance { get; init; }
    public decimal AverageAccountBalance { get; init; }
    
    // Transaction Statistics
    public int TransactionsToday { get; init; }
    public decimal TransactionVolumeToday { get; init; }
    public int TransactionsThisMonth { get; init; }
    public decimal TransactionVolumeThisMonth { get; init; }
    public List<TransactionTypeStatsDto> TransactionsByType { get; init; } = new();
    
    // Loan Statistics
    public int TotalLoans { get; init; }
    public decimal TotalLoanAmount { get; init; }
    public decimal OutstandingLoanAmount { get; init; }
    public int LoansApprovedToday { get; init; }
    public int LoansDisbursedToday { get; init; }
    public decimal NPLRatio { get; init; }
    
    // Card Statistics
    public int TotalCards { get; init; }
    public int ActiveCards { get; init; }
    public int VirtualCards { get; init; }
    public int PhysicalCards { get; init; }
    public int CardsIssuedToday { get; init; }
    public decimal CardTransactionVolumeToday { get; init; }
    
    // Channel Statistics
    public int OnlineBankingUsers { get; init; }
    public int MobileBankingUsers { get; init; }
    public int USSDUsers { get; init; }
    public int ATMTransactionsToday { get; init; }
    public int POSTransactionsToday { get; init; }
    
    // Risk and Compliance
    public int HighRiskCustomers { get; init; }
    public int FlaggedTransactions { get; init; }
    public int PendingAMLCases { get; init; }
    public int SanctionsMatches { get; init; }
    
    // Workflow Statistics
    public int PendingApprovals { get; init; }
    public int ApprovalsCompletedToday { get; init; }
    public int OverdueApprovals { get; init; }
    
    // Branch Statistics
    public int TotalBranches { get; init; }
    public int ActiveBranches { get; init; }
    public List<BranchStatsDto> TopPerformingBranches { get; init; } = new();
    
    // System Health
    public double SystemUptime { get; init; }
    public int ActiveSessions { get; init; }
    public int BackgroundJobsRunning { get; init; }
    public int FailedJobsToday { get; init; }
}

public record TransactionTypeStatsDto
{
    public string TransactionType { get; init; } = string.Empty;
    public int Count { get; init; }
    public decimal Volume { get; init; }
    public decimal Percentage { get; init; }
}

public record BranchStatsDto
{
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public string BranchCode { get; init; } = string.Empty;
    public int TransactionsToday { get; init; }
    public decimal TransactionVolumeToday { get; init; }
    public int NewAccountsToday { get; init; }
    public int ActiveTellers { get; init; }
}