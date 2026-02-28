///We’ve set the foundation with the BaseApiController. Now, let's flesh out the Transactional Hub and the Credit/Lending Gateway. These are the most high-traffic areas of Wekeza Bank.
///📂 Wekeza.Core.Api/Controllers/
///2. TransactionsController.cs (The Movement Hub)
///This is where the money flows. It handles everything from internal transfers to M-Pesa callbacks and the high-speed statement queries for your Data Analytics engine
///
///
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Transactions.Commands.TransferFunds;
using Wekeza.Core.Application.Features.Transactions.Commands.DepositFunds;
using Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

namespace Wekeza.Core.Api.Controllers;

public class TransactionsController : BaseApiController
{
    /// <summary>
    /// Executes a fund transfer between two Wekeza accounts.
    /// </summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<Guid>> Transfer(TransferFundsCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Entry point for M-Pesa/Mobile Money deposits.
    /// </summary>
    [HttpPost("deposit/mobile")]
    public async Task<ActionResult<Guid>> MobileDeposit(DepositFundsCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// High-performance Statement Query with pagination.
    /// </summary>
    [HttpGet("statement/{accountNumber}")]
    public async Task<ActionResult<PagedStatementDto>> GetStatement(
        string accountNumber, 
        [FromQuery] DateTime from, 
        [FromQuery] DateTime to,
        [FromQuery] int page = 1)
    {
        var query = new GetStatementQuery 
        { 
            AccountNumber = accountNumber, 
            FromDate = from, 
            ToDate = to, 
            PageNumber = page 
        };
        return Ok(await Mediator.Send(query));
    }
}
