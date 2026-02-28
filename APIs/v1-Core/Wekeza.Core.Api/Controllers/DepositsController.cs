using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Deposits.Commands.BookFixedDeposit;
using Wekeza.Core.Application.Features.Deposits.Commands.OpenRecurringDeposit;
using Wekeza.Core.Application.Features.Deposits.Commands.ProcessInterestAccrual;
using Wekeza.Core.Application.Features.Deposits.Commands.BookTermDeposit;
using Wekeza.Core.Application.Features.Deposits.Commands.OpenCallDeposit;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Deposits Controller - Handles all deposit product operations
/// Supports Fixed Deposits, Recurring Deposits, and Interest Accrual
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepositsController : BaseApiController
{
    public DepositsController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Book a new Fixed Deposit
    /// </summary>
    /// <param name="command">Fixed deposit booking details</param>
    /// <returns>Fixed deposit ID</returns>
    [HttpPost("fixed-deposits")]
    [Authorize(Roles = "Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> BookFixedDeposit([FromBody] BookFixedDepositCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetFixedDeposit), 
                new { id = result.Value }, 
                new { DepositId = result.Value, Message = "Fixed deposit booked successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    /// <summary>
    /// Open a new Recurring Deposit
    /// </summary>
    /// <param name="command">Recurring deposit opening details</param>
    /// <returns>Recurring deposit ID</returns>
    [HttpPost("recurring-deposits")]
    [Authorize(Roles = "Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> OpenRecurringDeposit([FromBody] OpenRecurringDepositCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetRecurringDeposit), 
                new { id = result.Value }, 
                new { DepositId = result.Value, Message = "Recurring deposit opened successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    /// <summary>
    /// Book a new Term Deposit
    /// </summary>
    /// <param name="command">Term deposit booking details</param>
    /// <returns>Term deposit ID</returns>
    [HttpPost("term-deposits")]
    [Authorize(Roles = "Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> BookTermDeposit([FromBody] BookTermDepositCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetTermDeposit), 
                new { id = result.Value }, 
                new { DepositId = result.Value, Message = "Term deposit booked successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    /// <summary>
    /// Open a new Call Deposit
    /// </summary>
    /// <param name="command">Call deposit opening details</param>
    /// <returns>Call deposit ID</returns>
    [HttpPost("call-deposits")]
    [Authorize(Roles = "Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> OpenCallDeposit([FromBody] OpenCallDepositCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetCallDeposit), 
                new { id = result.Value }, 
                new { DepositId = result.Value, Message = "Call deposit opened successfully" });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    /// <summary>
    /// Process interest accrual for all eligible deposits
    /// </summary>
    /// <param name="command">Interest accrual processing parameters</param>
    /// <returns>Accrual engine ID</returns>
    [HttpPost("interest-accrual")]
    [Authorize(Roles = "SystemService,Administrator")]
    public async Task<IActionResult> ProcessInterestAccrual([FromBody] ProcessInterestAccrualCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { 
                AccrualEngineId = result.Value, 
                Message = "Interest accrual processing completed successfully" 
            });
        }
        
        return BadRequest(new { Error = result.Error });
    }

    /// <summary>
    /// Get Fixed Deposit details by ID
    /// </summary>
    /// <param name="id">Fixed deposit ID</param>
    /// <returns>Fixed deposit details</returns>
    [HttpGet("fixed-deposits/{id:guid}")]
    [Authorize(Roles = "Customer,Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> GetFixedDeposit(Guid id)
    {
        // This would be implemented with a query handler
        // For now, return a placeholder response
        return Ok(new { 
            DepositId = id, 
            Message = "Fixed deposit details would be returned here" 
        });
    }

    /// <summary>
    /// Get Recurring Deposit details by ID
    /// </summary>
    /// <param name="id">Recurring deposit ID</param>
    /// <returns>Recurring deposit details</returns>
    [HttpGet("recurring-deposits/{id:guid}")]
    [Authorize(Roles = "Customer,Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> GetRecurringDeposit(Guid id)
    {
        // This would be implemented with a query handler
        // For now, return a placeholder response
        return Ok(new { 
            DepositId = id, 
            Message = "Recurring deposit details would be returned here" 
        });
    }

    /// <summary>
    /// Get Term Deposit details by ID
    /// </summary>
    /// <param name="id">Term deposit ID</param>
    /// <returns>Term deposit details</returns>
    [HttpGet("term-deposits/{id:guid}")]
    [Authorize(Roles = "Customer,Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> GetTermDeposit(Guid id)
    {
        // This would be implemented with a query handler
        // For now, return a placeholder response
        return Ok(new { 
            DepositId = id, 
            Message = "Term deposit details would be returned here" 
        });
    }

    /// <summary>
    /// Get Call Deposit details by ID
    /// </summary>
    /// <param name="id">Call deposit ID</param>
    /// <returns>Call deposit details</returns>
    [HttpGet("call-deposits/{id:guid}")]
    [Authorize(Roles = "Customer,Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> GetCallDeposit(Guid id)
    {
        // This would be implemented with a query handler
        // For now, return a placeholder response
        return Ok(new { 
            DepositId = id, 
            Message = "Call deposit details would be returned here" 
        });
    }

    /// <summary>
    /// Get customer's deposit portfolio
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <returns>List of customer deposits</returns>
    [HttpGet("customers/{customerId:guid}/deposits")]
    [Authorize(Roles = "Customer,Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> GetCustomerDeposits(Guid customerId)
    {
        // This would be implemented with a query handler
        return Ok(new { 
            CustomerId = customerId, 
            Deposits = new[] {
                new { Type = "Fixed Deposit", Count = 0, TotalAmount = 0 },
                new { Type = "Recurring Deposit", Count = 0, TotalAmount = 0 }
            }
        });
    }

    /// <summary>
    /// Get deposits maturing within specified period
    /// </summary>
    /// <param name="days">Number of days to look ahead</param>
    /// <returns>List of maturing deposits</returns>
    [HttpGet("maturing")]
    [Authorize(Roles = "Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> GetMaturingDeposits([FromQuery] int days = 30)
    {
        // This would be implemented with a query handler
        return Ok(new { 
            Days = days,
            MaturingDeposits = new object[] { }
        });
    }

    /// <summary>
    /// Get interest accrual history
    /// </summary>
    /// <param name="pageSize">Page size</param>
    /// <param name="pageNumber">Page number</param>
    /// <returns>Interest accrual history</returns>
    [HttpGet("interest-accrual/history")]
    [Authorize(Roles = "Administrator,SystemService")]
    public async Task<IActionResult> GetInterestAccrualHistory(
        [FromQuery] int pageSize = 20, 
        [FromQuery] int pageNumber = 1)
    {
        // This would be implemented with a query handler
        return Ok(new { 
            PageSize = pageSize,
            PageNumber = pageNumber,
            AccrualHistory = new object[] { }
        });
    }

    /// <summary>
    /// Calculate fixed deposit maturity amount
    /// </summary>
    /// <param name="principal">Principal amount</param>
    /// <param name="rate">Interest rate</param>
    /// <param name="days">Term in days</param>
    /// <returns>Maturity calculation</returns>
    [HttpGet("fixed-deposits/calculate-maturity")]
    [Authorize(Roles = "Customer,Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> CalculateFixedDepositMaturity(
        [FromQuery] decimal principal,
        [FromQuery] decimal rate,
        [FromQuery] int days)
    {
        if (principal <= 0 || rate <= 0 || days <= 0)
            return BadRequest("Invalid parameters for maturity calculation");

        // Simple interest calculation
        var interestAmount = principal * (rate / 100) * (days / 365m);
        var maturityAmount = principal + interestAmount;

        return Ok(new {
            Principal = principal,
            InterestRate = rate,
            TermInDays = days,
            InterestAmount = Math.Round(interestAmount, 2),
            MaturityAmount = Math.Round(maturityAmount, 2)
        });
    }

    /// <summary>
    /// Calculate recurring deposit maturity amount
    /// </summary>
    /// <param name="monthlyInstallment">Monthly installment amount</param>
    /// <param name="rate">Interest rate</param>
    /// <param name="months">Tenure in months</param>
    /// <returns>Maturity calculation</returns>
    [HttpGet("recurring-deposits/calculate-maturity")]
    [Authorize(Roles = "Customer,Teller,LoanOfficer,Administrator")]
    public async Task<IActionResult> CalculateRecurringDepositMaturity(
        [FromQuery] decimal monthlyInstallment,
        [FromQuery] decimal rate,
        [FromQuery] int months)
    {
        if (monthlyInstallment <= 0 || rate <= 0 || months <= 0)
            return BadRequest("Invalid parameters for maturity calculation");

        // RD maturity calculation
        var totalDeposits = monthlyInstallment * months;
        var monthlyRate = rate / (12 * 100);
        
        // Compound interest formula for RD
        var compoundFactor = (decimal)(Math.Pow((double)(1 + monthlyRate), months) - 1) / monthlyRate;
        var maturityAmount = monthlyInstallment * compoundFactor * (1 + monthlyRate);

        return Ok(new {
            MonthlyInstallment = monthlyInstallment,
            InterestRate = rate,
            TenureInMonths = months,
            TotalDeposits = Math.Round(totalDeposits, 2),
            InterestEarned = Math.Round(maturityAmount - totalDeposits, 2),
            MaturityAmount = Math.Round(maturityAmount, 2)
        });
    }
}