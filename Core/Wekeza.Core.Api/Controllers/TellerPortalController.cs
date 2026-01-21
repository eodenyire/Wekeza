using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Teller.Commands.StartSession;
using Wekeza.Core.Application.Features.Teller.Commands.EndSession;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessCashDeposit;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessCashWithdrawal;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessChequeDeposit;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessAccountOpening;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessCustomerOnboarding;
using Wekeza.Core.Application.Features.Teller.Queries.GetTellerSession;
using Wekeza.Core.Application.Features.Teller.Queries.GetCashDrawerBalance;
using Wekeza.Core.Application.Features.Teller.Queries.GetCustomerAccounts;
using Wekeza.Core.Application.Features.Teller.Queries.GetAccountBalance;
using Wekeza.Core.Application.Features.Teller.Queries.GetTransactionHistory;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Teller Portal Controller - Complete teller operations interface
/// Handles all branch teller operations including cash management, customer service, and account operations
/// </summary>
[ApiController]
[Route("api/teller")]
[Authorize(Roles = "Teller,Supervisor,BranchManager")]
public class TellerPortalController : BaseApiController
{
    public TellerPortalController(IMediator mediator) : base(mediator) { }

    #region Session Management

    /// <summary>
    /// Start teller session
    /// </summary>
    [HttpPost("session/start")]
    public async Task<IActionResult> StartSession([FromBody] StartTellerSessionCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new { 
                SessionId = result.Value,
                Message = "Teller session started successfully",
                StartTime = DateTime.UtcNow
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// End teller session
    /// </summary>
    [HttpPost("session/end")]
    public async Task<IActionResult> EndSession([FromBody] EndTellerSessionCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get current teller session
    /// </summary>
    [HttpGet("session/current")]
    public async Task<IActionResult> GetCurrentSession()
    {
        var query = new GetTellerSessionQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get cash drawer balance
    /// </summary>
    [HttpGet("cash-drawer/balance")]
    public async Task<IActionResult> GetCashDrawerBalance()
    {
        var query = new GetCashDrawerBalanceQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Cash Operations

    /// <summary>
    /// Process cash deposit
    /// </summary>
    [HttpPost("transactions/cash-deposit")]
    public async Task<IActionResult> ProcessCashDeposit([FromBody] ProcessCashDepositCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new {
                TransactionId = result.Value.TransactionId,
                TransactionReference = result.Value.TransactionReference,
                Amount = result.Value.Amount,
                NewBalance = result.Value.NewBalance,
                Message = "Cash deposit processed successfully"
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Process cash withdrawal
    /// </summary>
    [HttpPost("transactions/cash-withdrawal")]
    public async Task<IActionResult> ProcessCashWithdrawal([FromBody] ProcessCashWithdrawalCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new {
                TransactionId = result.Value.TransactionId,
                TransactionReference = result.Value.TransactionReference,
                Amount = result.Value.Amount,
                NewBalance = result.Value.NewBalance,
                Message = "Cash withdrawal processed successfully"
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Process cheque deposit
    /// </summary>
    [HttpPost("transactions/cheque-deposit")]
    public async Task<IActionResult> ProcessChequeDeposit([FromBody] ProcessChequeDepositCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Customer Operations

    /// <summary>
    /// Process customer onboarding
    /// </summary>
    [HttpPost("customers/onboard")]
    public async Task<IActionResult> OnboardCustomer([FromBody] ProcessCustomerOnboardingCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new {
                CustomerId = result.Value.CustomerId,
                CIFNumber = result.Value.CIFNumber,
                Message = "Customer onboarded successfully"
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Process account opening
    /// </summary>
    [HttpPost("accounts/open")]
    public async Task<IActionResult> OpenAccount([FromBody] ProcessAccountOpeningCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new {
                AccountId = result.Value.AccountId,
                AccountNumber = result.Value.AccountNumber,
                Message = "Account opened successfully"
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Get customer accounts
    /// </summary>
    [HttpGet("customers/{customerId:guid}/accounts")]
    public async Task<IActionResult> GetCustomerAccounts(Guid customerId)
    {
        var query = new GetCustomerAccountsQuery { CustomerId = customerId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get account balance
    /// </summary>
    [HttpGet("accounts/{accountId:guid}/balance")]
    public async Task<IActionResult> GetAccountBalance(Guid accountId)
    {
        var query = new GetAccountBalanceQuery { AccountId = accountId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get transaction history
    /// </summary>
    [HttpGet("accounts/{accountId:guid}/transactions")]
    public async Task<IActionResult> GetTransactionHistory(Guid accountId, [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
    {
        var query = new GetTransactionHistoryQuery 
        { 
            AccountId = accountId,
            PageSize = pageSize,
            PageNumber = pageNumber
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Account Services

    /// <summary>
    /// Search customer by various criteria
    /// </summary>
    [HttpGet("customers/search")]
    public async Task<IActionResult> SearchCustomers([FromQuery] string searchTerm, [FromQuery] string searchType = "name")
    {
        var query = new SearchCustomersQuery 
        { 
            SearchTerm = searchTerm,
            SearchType = searchType
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Verify customer identity
    /// </summary>
    [HttpPost("customers/{customerId:guid}/verify")]
    public async Task<IActionResult> VerifyCustomer(Guid customerId, [FromBody] VerifyCustomerCommand command)
    {
        command.CustomerId = customerId;
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Print account statement
    /// </summary>
    [HttpPost("accounts/{accountId:guid}/statement")]
    public async Task<IActionResult> PrintStatement(Guid accountId, [FromBody] PrintStatementCommand command)
    {
        command.AccountId = accountId;
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Block/Unblock account
    /// </summary>
    [HttpPost("accounts/{accountId:guid}/block")]
    public async Task<IActionResult> BlockAccount(Guid accountId, [FromBody] BlockAccountCommand command)
    {
        command.AccountId = accountId;
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion
}