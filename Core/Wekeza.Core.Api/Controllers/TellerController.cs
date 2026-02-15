using Microsoft.AspNetCore.Mvc;
using MediatR;
using Wekeza.Core.Application.Features.Teller.Commands.StartTellerSession;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessCashDeposit;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Teller Controller - Complete teller operations management
/// Handles teller sessions, cash management, and transaction processing
/// Inspired by Finacle Teller and T24 TELLER APIs
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TellerController : BaseApiController
{
    public TellerController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Start a new teller session
    /// </summary>
    /// <param name="command">Teller session details</param>
    /// <returns>Teller session start result</returns>
    [HttpPost("sessions/start")]
    [ProducesResponseType(typeof(StartTellerSessionResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<StartTellerSessionResult>> StartTellerSession([FromBody] StartTellerSessionCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Process a cash deposit
    /// </summary>
    /// <param name="command">Cash deposit details</param>
    /// <returns>Cash deposit processing result</returns>
    [HttpPost("transactions/cash-deposit")]
    [ProducesResponseType(typeof(ProcessCashDepositResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProcessCashDepositResult>> ProcessCashDeposit([FromBody] ProcessCashDepositCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get teller session details
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <returns>Teller session details</returns>
    [HttpGet("sessions/{sessionId:guid}")]
    [ProducesResponseType(typeof(TellerSessionDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<TellerSessionDto>> GetTellerSession(Guid sessionId)
    {
        // This would use a GetTellerSessionQuery - placeholder for now
        return Ok(new TellerSessionDto
        {
            SessionId = sessionId,
            SessionNumber = "SESSION-001",
            TellerName = "John Doe",
            Status = "Active",
            OpeningCashBalance = 100000,
            CurrentCashBalance = 150000,
            TransactionCount = 25,
            SessionStartTime = DateTime.UtcNow.AddHours(-4)
        });
    }

    /// <summary>
    /// Get active teller sessions for a branch
    /// </summary>
    /// <param name="branchId">Branch ID</param>
    /// <returns>List of active teller sessions</returns>
    [HttpGet("sessions/branch/{branchId:guid}/active")]
    [ProducesResponseType(typeof(List<TellerSessionSummaryDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<TellerSessionSummaryDto>>> GetActiveSessionsByBranch(Guid branchId)
    {
        // This would use a GetActiveSessionsByBranchQuery - placeholder for now
        return Ok(new List<TellerSessionSummaryDto>
        {
            new TellerSessionSummaryDto
            {
                SessionId = Guid.NewGuid(),
                SessionNumber = "SESSION-001",
                TellerCode = "T001",
                TellerName = "John Doe",
                Status = "Active",
                CurrentCashBalance = 150000,
                TransactionCount = 25,
                SessionStartTime = DateTime.UtcNow.AddHours(-4)
            }
        });
    }

    /// <summary>
    /// Get cash drawer status
    /// </summary>
    /// <param name="tellerId">Teller ID</param>
    /// <returns>Cash drawer details</returns>
    [HttpGet("cash-drawer/teller/{tellerId:guid}")]
    [ProducesResponseType(typeof(CashDrawerDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<CashDrawerDto>> GetCashDrawer(Guid tellerId)
    {
        // This would use a GetCashDrawerQuery - placeholder for now
        return Ok(new CashDrawerDto
        {
            DrawerId = "DRAWER-BR001-T001",
            TellerId = tellerId,
            Status = "Open",
            CurrentCashBalance = 150000,
            MaxCashLimit = 500000,
            MinCashLimit = 10000,
            LastReconciliationDate = DateTime.UtcNow.Date,
            CurrencyBalances = new Dictionary<string, decimal>
            {
                { "KES", 150000 },
                { "USD", 5000 }
            }
        });
    }

    /// <summary>
    /// Get teller transaction history
    /// </summary>
    /// <param name="sessionId">Session ID</param>
    /// <param name="pageSize">Page size (default: 50)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <returns>Teller transaction history</returns>
    [HttpGet("sessions/{sessionId:guid}/transactions")]
    [ProducesResponseType(typeof(TellerTransactionHistoryDto), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<TellerTransactionHistoryDto>> GetSessionTransactions(
        Guid sessionId,
        [FromQuery] int pageSize = 50,
        [FromQuery] int pageNumber = 1)
    {
        // This would use a GetSessionTransactionsQuery - placeholder for now
        return Ok(new TellerTransactionHistoryDto
        {
            SessionId = sessionId,
            TotalTransactions = 25,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Transactions = new List<TellerTransactionSummaryDto>
            {
                new TellerTransactionSummaryDto
                {
                    TransactionNumber = "TXN001",
                    TransactionType = "CashDeposit",
                    Amount = 50000,
                    Currency = "KES",
                    AccountNumber = "ACC001",
                    Status = "Completed",
                    TransactionDate = DateTime.UtcNow.AddMinutes(-30)
                }
            }
        });
    }

    /// <summary>
    /// Get branch cash summary
    /// </summary>
    /// <param name="branchId">Branch ID</param>
    /// <returns>Branch cash position summary</returns>
    [HttpGet("cash-summary/branch/{branchId:guid}")]
    [ProducesResponseType(typeof(BranchCashSummaryDto), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<BranchCashSummaryDto>> GetBranchCashSummary(Guid branchId)
    {
        // This would use a GetBranchCashSummaryQuery - placeholder for now
        return Ok(new BranchCashSummaryDto
        {
            BranchId = branchId,
            TotalCashInDrawers = 750000,
            ActiveTellers = 5,
            TotalTransactionsToday = 125,
            TotalCashDepositsToday = 500000,
            TotalCashWithdrawalsToday = 300000,
            NetCashMovement = 200000,
            CurrencyBreakdown = new Dictionary<string, decimal>
            {
                { "KES", 700000 },
                { "USD", 25000 },
                { "EUR", 15000 }
            }
        });
    }
}

// DTOs for API responses
public record TellerSessionDto
{
    public Guid SessionId { get; init; }
    public string SessionNumber { get; init; } = string.Empty;
    public string TellerName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal OpeningCashBalance { get; init; }
    public decimal CurrentCashBalance { get; init; }
    public int TransactionCount { get; init; }
    public DateTime SessionStartTime { get; init; }
    public DateTime? SessionEndTime { get; init; }
}

public record TellerSessionSummaryDto
{
    public Guid SessionId { get; init; }
    public string SessionNumber { get; init; } = string.Empty;
    public string TellerCode { get; init; } = string.Empty;
    public string TellerName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal CurrentCashBalance { get; init; }
    public int TransactionCount { get; init; }
    public DateTime SessionStartTime { get; init; }
}

public record CashDrawerDto
{
    public string DrawerId { get; init; } = string.Empty;
    public Guid TellerId { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal CurrentCashBalance { get; init; }
    public decimal MaxCashLimit { get; init; }
    public decimal MinCashLimit { get; init; }
    public DateTime? LastReconciliationDate { get; init; }
    public Dictionary<string, decimal> CurrencyBalances { get; init; } = new();
}

public record TellerTransactionHistoryDto
{
    public Guid SessionId { get; init; }
    public int TotalTransactions { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public List<TellerTransactionSummaryDto> Transactions { get; init; } = new();
}

public record TellerTransactionSummaryDto
{
    public string TransactionNumber { get; init; } = string.Empty;
    public string TransactionType { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string? AccountNumber { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; }
}

public record BranchCashSummaryDto
{
    public Guid BranchId { get; init; }
    public decimal TotalCashInDrawers { get; init; }
    public int ActiveTellers { get; init; }
    public int TotalTransactionsToday { get; init; }
    public decimal TotalCashDepositsToday { get; init; }
    public decimal TotalCashWithdrawalsToday { get; init; }
    public decimal NetCashMovement { get; init; }
    public Dictionary<string, decimal> CurrencyBreakdown { get; init; } = new();
}