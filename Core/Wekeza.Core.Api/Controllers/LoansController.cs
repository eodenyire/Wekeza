using Microsoft.AspNetCore.Mvc;
using MediatR;
using Wekeza.Core.Application.Features.Loans.Commands.ApplyForLoan;
using Wekeza.Core.Application.Features.Loans.Commands.ApproveLoan;
using Wekeza.Core.Application.Features.Loans.Commands.DisburseLoan;
using Wekeza.Core.Application.Features.Loans.Commands.ProcessRepayment;
using Wekeza.Core.Application.Features.Loans.Queries.GetLoanDetails;
using Wekeza.Core.Application.Features.Loans.Queries.GetLoanPortfolio;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Loans Controller - Complete loan management operations
/// Handles loan origination, servicing, and portfolio management
/// Inspired by Finacle LMS and T24 Lending APIs
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LoansController : BaseApiController
{
    public LoansController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Apply for a new loan
    /// </summary>
    /// <param name="command">Loan application details</param>
    /// <returns>Loan application result with credit assessment</returns>
    [HttpPost("apply")]
    [ProducesResponseType(typeof(ApplyForLoanResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApplyForLoanResult>> ApplyForLoan([FromBody] ApplyForLoanCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Approve a loan application
    /// </summary>
    /// <param name="command">Loan approval details</param>
    /// <returns>Loan approval result</returns>
    [HttpPost("approve")]
    [ProducesResponseType(typeof(ApproveLoanResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApproveLoanResult>> ApproveLoan([FromBody] ApproveLoanCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Disburse an approved loan
    /// </summary>
    /// <param name="command">Loan disbursement details</param>
    /// <returns>Loan disbursement result with GL posting</returns>
    [HttpPost("disburse")]
    [ProducesResponseType(typeof(DisburseLoanResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<DisburseLoanResult>> DisburseLoan([FromBody] DisburseLoanCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Process a loan repayment
    /// </summary>
    /// <param name="command">Repayment details</param>
    /// <returns>Repayment processing result with allocation details</returns>
    [HttpPost("repayment")]
    [ProducesResponseType(typeof(ProcessRepaymentResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProcessRepaymentResult>> ProcessRepayment([FromBody] ProcessRepaymentCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get detailed loan information
    /// </summary>
    /// <param name="loanId">Loan ID</param>
    /// <param name="includeSchedule">Include repayment schedule</param>
    /// <param name="includeCollaterals">Include collateral information</param>
    /// <param name="includeGuarantors">Include guarantor information</param>
    /// <returns>Comprehensive loan details</returns>
    [HttpGet("{loanId:guid}")]
    [ProducesResponseType(typeof(LoanDetailsDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<LoanDetailsDto>> GetLoanDetails(
        Guid loanId,
        [FromQuery] bool includeSchedule = true,
        [FromQuery] bool includeCollaterals = true,
        [FromQuery] bool includeGuarantors = true)
    {
        var query = new GetLoanDetailsQuery
        {
            LoanId = loanId,
            IncludeSchedule = includeSchedule,
            IncludeCollaterals = includeCollaterals,
            IncludeGuarantors = includeGuarantors
        };

        var result = await Mediator.Send(query);
        
        if (result == null)
        {
            return NotFound($"Loan with ID {loanId} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Get loan details by loan number
    /// </summary>
    /// <param name="loanNumber">Loan number</param>
    /// <param name="includeSchedule">Include repayment schedule</param>
    /// <param name="includeCollaterals">Include collateral information</param>
    /// <param name="includeGuarantors">Include guarantor information</param>
    /// <returns>Comprehensive loan details</returns>
    [HttpGet("by-number/{loanNumber}")]
    [ProducesResponseType(typeof(LoanDetailsDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<LoanDetailsDto>> GetLoanDetailsByNumber(
        string loanNumber,
        [FromQuery] bool includeSchedule = true,
        [FromQuery] bool includeCollaterals = true,
        [FromQuery] bool includeGuarantors = true)
    {
        var query = new GetLoanDetailsQuery
        {
            LoanNumber = loanNumber,
            IncludeSchedule = includeSchedule,
            IncludeCollaterals = includeCollaterals,
            IncludeGuarantors = includeGuarantors
        };

        var result = await Mediator.Send(query);
        
        if (result == null)
        {
            return NotFound($"Loan with number {loanNumber} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Get loan portfolio with filtering and analytics
    /// </summary>
    /// <param name="customerId">Filter by customer ID</param>
    /// <param name="status">Filter by loan status</param>
    /// <param name="subStatus">Filter by loan sub-status</param>
    /// <param name="riskGrade">Filter by risk grade</param>
    /// <param name="productType">Filter by product type</param>
    /// <param name="fromDate">Filter from date</param>
    /// <param name="toDate">Filter to date</param>
    /// <param name="searchTerm">Search term for loan number or customer name</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="includeAnalytics">Include portfolio analytics</param>
    /// <param name="includeRiskBreakdown">Include risk grade breakdown</param>
    /// <param name="includeStatusBreakdown">Include status breakdown</param>
    /// <returns>Loan portfolio with analytics</returns>
    [HttpGet("portfolio")]
    [ProducesResponseType(typeof(LoanPortfolioDto), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<LoanPortfolioDto>> GetLoanPortfolio(
        [FromQuery] Guid? customerId = null,
        [FromQuery] LoanStatus? status = null,
        [FromQuery] LoanSubStatus? subStatus = null,
        [FromQuery] CreditRiskGrade? riskGrade = null,
        [FromQuery] ProductType? productType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageSize = 50,
        [FromQuery] int pageNumber = 1,
        [FromQuery] bool includeAnalytics = true,
        [FromQuery] bool includeRiskBreakdown = true,
        [FromQuery] bool includeStatusBreakdown = true)
    {
        var query = new GetLoanPortfolioQuery
        {
            CustomerId = customerId,
            Status = status,
            SubStatus = subStatus,
            RiskGrade = riskGrade,
            ProductType = productType,
            FromDate = fromDate,
            ToDate = toDate,
            SearchTerm = searchTerm,
            PageSize = Math.Min(pageSize, 100), // Limit page size
            PageNumber = Math.Max(pageNumber, 1),
            IncludeAnalytics = includeAnalytics,
            IncludeRiskBreakdown = includeRiskBreakdown,
            IncludeStatusBreakdown = includeStatusBreakdown
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get loans by customer ID
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <returns>List of customer loans</returns>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(LoanPortfolioDto), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<LoanPortfolioDto>> GetLoansByCustomer(Guid customerId)
    {
        var query = new GetLoanPortfolioQuery
        {
            CustomerId = customerId,
            IncludeAnalytics = false,
            IncludeRiskBreakdown = false,
            IncludeStatusBreakdown = false
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get pending loan approvals
    /// </summary>
    /// <returns>Loans pending approval</returns>
    [HttpGet("pending-approvals")]
    [ProducesResponseType(typeof(LoanPortfolioDto), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<LoanPortfolioDto>> GetPendingApprovals()
    {
        var query = new GetLoanPortfolioQuery
        {
            Status = LoanStatus.Applied,
            IncludeAnalytics = false,
            IncludeRiskBreakdown = false,
            IncludeStatusBreakdown = false
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get loans ready for disbursement
    /// </summary>
    /// <returns>Approved loans awaiting disbursement</returns>
    [HttpGet("pending-disbursement")]
    [ProducesResponseType(typeof(LoanPortfolioDto), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<LoanPortfolioDto>> GetPendingDisbursement()
    {
        var query = new GetLoanPortfolioQuery
        {
            Status = LoanStatus.Approved,
            SubStatus = LoanSubStatus.AwaitingDisbursement,
            IncludeAnalytics = false,
            IncludeRiskBreakdown = false,
            IncludeStatusBreakdown = false
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get past due loans
    /// </summary>
    /// <param name="daysPastDue">Minimum days past due (default: 1)</param>
    /// <returns>Past due loans</returns>
    [HttpGet("past-due")]
    [ProducesResponseType(typeof(LoanPortfolioDto), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<LoanPortfolioDto>> GetPastDueLoans([FromQuery] int daysPastDue = 1)
    {
        var query = new GetLoanPortfolioQuery
        {
            Status = LoanStatus.Active,
            // Note: This would need additional filtering logic for days past due
            IncludeAnalytics = true,
            IncludeRiskBreakdown = false,
            IncludeStatusBreakdown = false
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get non-performing loans
    /// </summary>
    /// <returns>Non-performing loans</returns>
    [HttpGet("non-performing")]
    [ProducesResponseType(typeof(LoanPortfolioDto), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<LoanPortfolioDto>> GetNonPerformingLoans()
    {
        var query = new GetLoanPortfolioQuery
        {
            Status = LoanStatus.Active,
            SubStatus = LoanSubStatus.NonPerforming,
            IncludeAnalytics = true,
            IncludeRiskBreakdown = true,
            IncludeStatusBreakdown = false
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }
}