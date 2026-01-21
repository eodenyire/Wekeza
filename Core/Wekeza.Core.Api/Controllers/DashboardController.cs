using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetTransactionTrends;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetAccountStatistics;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetCustomerStatistics;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetLoanPortfolioStats;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetRiskMetrics;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetChannelStatistics;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetBranchPerformance;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetComplianceMetrics;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Dashboard Controller - Real-time analytics and KPIs
/// Provides comprehensive business intelligence and operational metrics
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : BaseApiController
{
    public DashboardController(IMediator mediator) : base(mediator) { }

    #region Transaction Analytics

    /// <summary>
    /// Get transaction trends and volume analytics
    /// </summary>
    [HttpGet("transactions/trends")]
    public async Task<IActionResult> GetTransactionTrends([FromQuery] string period = "daily", [FromQuery] int days = 30)
    {
        var query = new GetTransactionTrendsQuery 
        { 
            Period = period,
            Days = days
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get transaction statistics by type
    /// </summary>
    [HttpGet("transactions/statistics")]
    public async Task<IActionResult> GetTransactionStatistics([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
    {
        var query = new GetTransactionStatisticsQuery
        {
            FromDate = fromDate ?? DateTime.UtcNow.AddDays(-30),
            ToDate = toDate ?? DateTime.UtcNow
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Account Analytics

    /// <summary>
    /// Get account statistics and metrics
    /// </summary>
    [HttpGet("accounts/statistics")]
    public async Task<IActionResult> GetAccountStatistics()
    {
        var query = new GetAccountStatisticsQuery();
        var result = await Mediator.Send(query);
        return Ok(new
        {
            TotalAccounts = result.TotalAccounts,
            ActiveAccounts = result.ActiveAccounts,
            InactiveAccounts = result.InactiveAccounts,
            FrozenAccounts = result.FrozenAccounts,
            AccountsByType = result.AccountsByType,
            AccountsByBranch = result.AccountsByBranch,
            TotalBalance = result.TotalBalance,
            AverageBalance = result.AverageBalance,
            TopAccountsByBalance = result.TopAccountsByBalance
        });
    }

    /// <summary>
    /// Get accounts with highest transaction volumes
    /// </summary>
    [HttpGet("accounts/high-activity")]
    public async Task<IActionResult> GetHighActivityAccounts([FromQuery] int limit = 10)
    {
        var query = new GetHighActivityAccountsQuery { Limit = limit };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get accounts with liens or restrictions
    /// </summary>
    [HttpGet("accounts/restricted")]
    public async Task<IActionResult> GetRestrictedAccounts()
    {
        var query = new GetRestrictedAccountsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Customer Analytics

    /// <summary>
    /// Get customer statistics and demographics
    /// </summary>
    [HttpGet("customers/statistics")]
    public async Task<IActionResult> GetCustomerStatistics()
    {
        var query = new GetCustomerStatisticsQuery();
        var result = await Mediator.Send(query);
        return Ok(new
        {
            TotalCustomers = result.TotalCustomers,
            NewCustomersThisMonth = result.NewCustomersThisMonth,
            CustomersBySegment = result.CustomersBySegment,
            CustomersByBranch = result.CustomersByBranch,
            CustomersWithMultipleAccounts = result.CustomersWithMultipleAccounts,
            CIFsWithoutAccounts = result.CIFsWithoutAccounts,
            KYCPendingCustomers = result.KYCPendingCustomers,
            HighRiskCustomers = result.HighRiskCustomers
        });
    }

    /// <summary>
    /// Get customer onboarding trends
    /// </summary>
    [HttpGet("customers/onboarding-trends")]
    public async Task<IActionResult> GetOnboardingTrends([FromQuery] int months = 12)
    {
        var query = new GetOnboardingTrendsQuery { Months = months };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Loan Portfolio Analytics

    /// <summary>
    /// Get loan portfolio statistics
    /// </summary>
    [HttpGet("loans/portfolio")]
    public async Task<IActionResult> GetLoanPortfolioStats()
    {
        var query = new GetLoanPortfolioStatsQuery();
        var result = await Mediator.Send(query);
        return Ok(new
        {
            TotalLoans = result.TotalLoans,
            TotalLoanAmount = result.TotalLoanAmount,
            OutstandingAmount = result.OutstandingAmount,
            LoansByStatus = result.LoansByStatus,
            LoansByProduct = result.LoansByProduct,
            NPLRatio = result.NPLRatio,
            ProvisionCoverage = result.ProvisionCoverage,
            AverageInterestRate = result.AverageInterestRate
        });
    }

    /// <summary>
    /// Get loan performance metrics
    /// </summary>
    [HttpGet("loans/performance")]
    public async Task<IActionResult> GetLoanPerformance()
    {
        var query = new GetLoanPerformanceQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Risk and Compliance Metrics

    /// <summary>
    /// Get risk metrics and indicators
    /// </summary>
    [HttpGet("risk/metrics")]
    [Authorize(Roles = "RiskOfficer,ComplianceOfficer,Administrator,BranchManager")]
    public async Task<IActionResult> GetRiskMetrics()
    {
        var query = new GetRiskMetricsQuery();
        var result = await Mediator.Send(query);
        return Ok(new
        {
            HighRiskCustomers = result.HighRiskCustomers,
            FlaggedTransactions = result.FlaggedTransactions,
            PendingAMLCases = result.PendingAMLCases,
            SanctionsMatches = result.SanctionsMatches,
            LargeTransactions = result.LargeTransactions,
            UnusualActivityAlerts = result.UnusualActivityAlerts
        });
    }

    /// <summary>
    /// Get compliance metrics
    /// </summary>
    [HttpGet("compliance/metrics")]
    [Authorize(Roles = "ComplianceOfficer,Administrator")]
    public async Task<IActionResult> GetComplianceMetrics()
    {
        var query = new GetComplianceMetricsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Channel Analytics

    /// <summary>
    /// Get digital channel statistics
    /// </summary>
    [HttpGet("channels/statistics")]
    public async Task<IActionResult> GetChannelStatistics()
    {
        var query = new GetChannelStatisticsQuery();
        var result = await Mediator.Send(query);
        return Ok(new
        {
            OnlineBankingUsers = result.OnlineBankingUsers,
            MobileBankingUsers = result.MobileBankingUsers,
            USSDUsers = result.USSDUsers,
            ATMTransactions = result.ATMTransactions,
            POSTransactions = result.POSTransactions,
            ChannelTransactionVolumes = result.ChannelTransactionVolumes
        });
    }

    /// <summary>
    /// Get card statistics
    /// </summary>
    [HttpGet("cards/statistics")]
    public async Task<IActionResult> GetCardStatistics()
    {
        var query = new GetCardStatisticsQuery();
        var result = await Mediator.Send(query);
        return Ok(new
        {
            TotalCards = result.TotalCards,
            ActiveCards = result.ActiveCards,
            VirtualCards = result.VirtualCards,
            PhysicalCards = result.PhysicalCards,
            CardsByType = result.CardsByType,
            CardTransactionVolume = result.CardTransactionVolume,
            ATMWithdrawals = result.ATMWithdrawals,
            POSTransactions = result.POSTransactions
        });
    }

    #endregion

    #region Branch Performance

    /// <summary>
    /// Get branch performance metrics
    /// </summary>
    [HttpGet("branches/performance")]
    [Authorize(Roles = "BranchManager,Administrator,RegionalManager")]
    public async Task<IActionResult> GetBranchPerformance()
    {
        var query = new GetBranchPerformanceQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get branch comparison metrics
    /// </summary>
    [HttpGet("branches/comparison")]
    [Authorize(Roles = "Administrator,RegionalManager")]
    public async Task<IActionResult> GetBranchComparison([FromQuery] string metric = "transactions")
    {
        var query = new GetBranchComparisonQuery { Metric = metric };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region System Health

    /// <summary>
    /// Get system health metrics
    /// </summary>
    [HttpGet("system/health")]
    [Authorize(Roles = "ITAdministrator,Administrator")]
    public async Task<IActionResult> GetSystemHealth()
    {
        var query = new GetSystemHealthQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get pending approvals summary
    /// </summary>
    [HttpGet("approvals/summary")]
    public async Task<IActionResult> GetPendingApprovalsSummary()
    {
        var query = new GetPendingApprovalsSummaryQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion
}