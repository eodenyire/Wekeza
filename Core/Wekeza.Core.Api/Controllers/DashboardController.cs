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
using Wekeza.Core.Application.Features.Dashboard.Queries.GetCardStatistics;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetBranchComparison;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetSystemHealth;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetPendingApprovalsSummary;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetTransactionStatistics;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetHighActivityAccounts;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetRestrictedAccounts;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetOnboardingTrends;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetLoanPerformance;

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
            TotalAccounts = result.Value.TotalAccounts,
            ActiveAccounts = result.Value.ActiveAccounts,
            InactiveAccounts = result.Value.InactiveAccounts,
            FrozenAccounts = result.Value.FrozenAccounts,
            AccountsByType = result.Value.AccountsByType,
            AccountsByBranch = result.Value.AccountsByBranch,
            TotalBalance = result.Value.TotalBalance,
            AverageBalance = result.Value.AverageBalance,
            TopAccountsByBalance = result.Value.TopAccountsByBalance
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
            TotalCustomers = result.Value.TotalCustomers,
            NewCustomersThisMonth = result.Value.NewCustomersThisMonth,
            CustomersBySegment = result.Value.CustomersBySegment,
            CustomersByBranch = result.Value.CustomersByBranch,
            CustomersWithMultipleAccounts = result.Value.CustomersWithMultipleAccounts,
            CIFsWithoutAccounts = result.Value.CIFsWithoutAccounts,
            KYCPendingCustomers = result.Value.KYCPendingCustomers,
            HighRiskCustomers = result.Value.HighRiskCustomers
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
            TotalLoans = result.Value.TotalLoans,
            TotalLoanAmount = result.Value.TotalLoanAmount,
            OutstandingAmount = result.Value.OutstandingAmount,
            LoansByStatus = result.Value.LoansByStatus,
            LoansByProduct = result.Value.LoansByProduct,
            NPLRatio = result.Value.NPLRatio,
            ProvisionCoverage = result.Value.ProvisionCoverage,
            AverageInterestRate = result.Value.AverageInterestRate
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
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);
            
        return Ok(result.Value);
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
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);
            
        return Ok(result.Value);
    }

    /// <summary>
    /// Get card statistics
    /// </summary>
    [HttpGet("cards/statistics")]
    public async Task<IActionResult> GetCardStatistics()
    {
        var query = new GetCardStatisticsQuery();
        var result = await Mediator.Send(query);
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);
            
        return Ok(result.Value);
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