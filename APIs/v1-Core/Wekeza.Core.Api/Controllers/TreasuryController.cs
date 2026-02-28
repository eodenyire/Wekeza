using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Wekeza.Core.Application.Features.Treasury.Commands.BookMoneyMarketDeal;
using Wekeza.Core.Application.Features.Treasury.Commands.ExecuteFXDeal;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Treasury & Markets operations including Money Market, FX Trading, and Securities
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TreasuryController : BaseApiController
{
    public TreasuryController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Book a new Money Market deal
    /// </summary>
    /// <param name="command">Money market deal details</param>
    /// <returns>Deal booking confirmation</returns>
    [HttpPost("money-market/deals")]
    [Authorize(Roles = "Administrator,RiskOfficer")]
    public async Task<ActionResult<BookMoneyMarketDealResponse>> BookMoneyMarketDealAsync([FromBody] BookMoneyMarketDealCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Execute an FX deal
    /// </summary>
    /// <param name="command">FX deal details</param>
    /// <returns>FX deal execution confirmation</returns>
    [HttpPost("fx/deals")]
    [Authorize(Roles = "Administrator,RiskOfficer")]
    public async Task<ActionResult<ExecuteFXDealResponse>> ExecuteFXDealAsync([FromBody] ExecuteFXDealCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get Money Market deal details
    /// </summary>
    /// <param name="dealId">Deal ID</param>
    /// <returns>Deal details</returns>
    [HttpGet("money-market/deals/{dealId}")]
    public async Task<ActionResult> GetMoneyMarketDealAsync(Guid dealId)
    {
        // TODO: Implement GetMoneyMarketDealQuery
        return Ok(new { Message = "Money market deal query will be implemented", DealId = dealId });
    }

    /// <summary>
    /// Get FX deal details
    /// </summary>
    /// <param name="dealId">Deal ID</param>
    /// <returns>Deal details</returns>
    [HttpGet("fx/deals/{dealId}")]
    public async Task<ActionResult> GetFXDealAsync(Guid dealId)
    {
        // TODO: Implement GetFXDealQuery
        return Ok(new { Message = "FX deal query will be implemented", DealId = dealId });
    }

    /// <summary>
    /// Settle a Money Market deal
    /// </summary>
    /// <param name="dealId">Deal ID</param>
    /// <returns>Settlement confirmation</returns>
    [HttpPost("money-market/deals/{dealId}/settle")]
    [Authorize(Roles = "Administrator,RiskOfficer,Teller")]
    public async Task<ActionResult> SettleMoneyMarketDealAsync(Guid dealId)
    {
        // TODO: Implement SettleMoneyMarketDealCommand
        return Ok(new { Message = "Money market deal settlement will be implemented", DealId = dealId });
    }

    /// <summary>
    /// Settle an FX deal
    /// </summary>
    /// <param name="dealId">Deal ID</param>
    /// <returns>Settlement confirmation</returns>
    [HttpPost("fx/deals/{dealId}/settle")]
    [Authorize(Roles = "Administrator,RiskOfficer,Teller")]
    public async Task<ActionResult> SettleFXDealAsync(Guid dealId)
    {
        // TODO: Implement SettleFXDealCommand
        return Ok(new { Message = "FX deal settlement will be implemented", DealId = dealId });
    }

    /// <summary>
    /// Execute a Security trade
    /// </summary>
    /// <param name="request">Security trade details</param>
    /// <returns>Trade execution confirmation</returns>
    [HttpPost("securities/trades")]
    [Authorize(Roles = "Administrator,RiskOfficer")]
    public async Task<ActionResult> ExecuteSecurityTradeAsync([FromBody] ExecuteSecurityTradeRequest request)
    {
        // TODO: Implement ExecuteSecurityTradeCommand
        return Ok(new { Message = "Security trade execution will be implemented" });
    }

    /// <summary>
    /// Get current liquidity position
    /// </summary>
    /// <param name="currency">Currency (optional)</param>
    /// <returns>Liquidity position</returns>
    [HttpGet("liquidity/position")]
    [Authorize(Roles = "Administrator,RiskOfficer")]
    public async Task<ActionResult> GetLiquidityPositionAsync([FromQuery] string? currency = null)
    {
        // TODO: Implement GetLiquidityPositionQuery
        return Ok(new 
        { 
            Message = "Liquidity position query will be implemented",
            Currency = currency ?? "ALL",
            Position = new
            {
                Date = DateTime.UtcNow.Date,
                OpeningBalance = 0,
                Inflows = 0,
                Outflows = 0,
                ClosingBalance = 0,
                RequiredReserves = 0,
                ExcessLiquidity = 0
            }
        });
    }

    /// <summary>
    /// Get FX position summary
    /// </summary>
    /// <returns>FX positions by currency</returns>
    [HttpGet("fx/positions")]
    [Authorize(Roles = "Administrator,RiskOfficer")]
    public async Task<ActionResult> GetFXPositionsAsync()
    {
        // TODO: Implement GetFXPositionsQuery
        return Ok(new 
        { 
            Message = "FX positions query will be implemented",
            Positions = new[]
            {
                new { Currency = "USD", Position = 0, Exposure = 0 },
                new { Currency = "EUR", Position = 0, Exposure = 0 },
                new { Currency = "GBP", Position = 0, Exposure = 0 }
            }
        });
    }

    /// <summary>
    /// Get securities portfolio
    /// </summary>
    /// <returns>Securities holdings</returns>
    [HttpGet("securities/portfolio")]
    [Authorize(Roles = "Administrator,RiskOfficer")]
    public async Task<ActionResult> GetSecuritiesPortfolioAsync()
    {
        // TODO: Implement GetSecuritiesPortfolioQuery
        return Ok(new 
        { 
            Message = "Securities portfolio query will be implemented",
            Portfolio = new
            {
                TotalValue = 0,
                GovernmentBonds = 0,
                CorporateBonds = 0,
                Equities = 0,
                UnrealizedPnL = 0
            }
        });
    }

    /// <summary>
    /// Get treasury dashboard metrics
    /// </summary>
    /// <returns>Treasury dashboard data</returns>
    [HttpGet("dashboard")]
    [Authorize(Roles = "Administrator,RiskOfficer")]
    public async Task<ActionResult> GetTreasuryDashboardAsync()
    {
        // TODO: Implement GetTreasuryDashboardQuery
        return Ok(new
        {
            Message = "Treasury dashboard will be implemented",
            Metrics = new
            {
                TotalAssets = 0,
                LiquidityRatio = 0,
                FXExposure = 0,
                InterestRateRisk = 0,
                ActiveDeals = 0,
                MaturingToday = 0,
                PnL = new
                {
                    Daily = 0,
                    Monthly = 0,
                    YearToDate = 0
                }
            }
        });
    }

    /// <summary>
    /// Update FX rates
    /// </summary>
    /// <param name="request">Rate update details</param>
    /// <returns>Update confirmation</returns>
    [HttpPost("fx/rates/update")]
    [Authorize(Roles = "Administrator,SystemService")]
    public async Task<ActionResult> UpdateFXRatesAsync([FromBody] UpdateFXRatesRequest request)
    {
        // TODO: Implement UpdateFXRatesCommand
        return Ok(new { Message = "FX rates update will be implemented", UpdatedRates = request.Rates.Count });
    }

    /// <summary>
    /// Get current FX rates
    /// </summary>
    /// <returns>Current FX rates</returns>
    [HttpGet("fx/rates")]
    public async Task<ActionResult> GetFXRatesAsync()
    {
        // TODO: Implement GetFXRatesQuery
        return Ok(new
        {
            Message = "FX rates query will be implemented",
            Timestamp = DateTime.UtcNow,
            Rates = new[]
            {
                new { Pair = "USD/KES", Rate = 129.50, Spread = 0.50 },
                new { Pair = "EUR/KES", Rate = 140.25, Spread = 0.75 },
                new { Pair = "GBP/KES", Rate = 155.80, Spread = 1.00 }
            }
        });
    }
}

// Request DTOs (to be moved to separate files later)
public record ExecuteSecurityTradeRequest
{
    public string DealNumber { get; init; } = string.Empty;
    public string SecurityId { get; init; } = string.Empty;
    public string SecurityType { get; init; } = string.Empty;
    public string TradeType { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "USD";
    public string TraderId { get; init; } = string.Empty;
    public DateTime? SettlementDate { get; init; }
    public decimal? YieldRate { get; init; }
}

public record UpdateFXRatesRequest
{
    public Dictionary<string, FXRateUpdate> Rates { get; init; } = new();
    public string Source { get; init; } = "SYSTEM";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record FXRateUpdate
{
    public decimal Rate { get; init; }
    public decimal Spread { get; init; }
}