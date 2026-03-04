using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Treasury Portal Controller
/// Handles FX operations, liquidity, and money market deals
/// </summary>
[ApiController]
[Route("api/treasury-portal")]
[Authorize(Roles = "TreasuryDealer,TreasuryOfficer,Administrator")]
public class TreasuryPortalController : BaseApiController
{
    private readonly IConfiguration _configuration;

    public TreasuryPortalController(IMediator mediator, IConfiguration configuration) 
        : base(mediator) 
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Get liquidity metrics
    /// </summary>
    [HttpGet("liquidity")]
    public async Task<IActionResult> GetLiquidityMetrics()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var totalBalance = await ExecuteScalarAsync<decimal>(connection,
                @"SELECT COALESCE(SUM(""Balance""), 0) FROM ""Accounts""");

            var shortTermLiabilities = totalBalance * 0.15m;
            var lcrRatio = totalBalance > shortTermLiabilities 
                ? (totalBalance / shortTermLiabilities * 100) 
                : 100;

            return Ok(new
            {
                success = true,
                data = new
                {
                    LCR = Math.Round(lcrRatio, 2),
                    NSFR = 118.5m,
                    NetLiquidityPosition = totalBalance,
                    CashReserves = totalBalance * 0.20m,
                    RequiredReserves = totalBalance * 0.10m,
                    ExcessLiquidity = totalBalance * 0.10m,
                    UpdatedAt = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve liquidity metrics", details = ex.Message });
        }
    }

    /// <summary>
    /// Get FX deals
    /// </summary>
    [HttpGet("fx-deals")]
    public async Task<IActionResult> GetFXDeals([FromQuery] int limit = 10)
    {
        try
        {
            var deals = new List<object>
            {
                new { DealId = "FX-001", Pair = "USD/KES", Amount = 1000000, Rate = 158.40m, Direction = "Buy", Status = "Confirmed", TradeDate = DateTime.UtcNow.AddDays(-1) },
                new { DealId = "FX-002", Pair = "EUR/KES", Amount = 250000, Rate = 171.05m, Direction = "Sell", Status = "Settled", TradeDate = DateTime.UtcNow.AddDays(-2) },
                new { DealId = "FX-003", Pair = "GBP/KES", Amount = 500000, Rate = 199.75m, Direction = "Buy", Status = "Pending", TradeDate = DateTime.UtcNow }
            };

            return Ok(new { success = true, data = deals.Take(limit).ToList() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get money market operations
    /// </summary>
    [HttpGet("money-market")]
    public async Task<IActionResult> GetMoneyMarket()
    {
        try
        {
            return Ok(new
            {
                success = true,
                data = new
                {
                    InterbankPlacements = new[]
                    {
                        new { Bank = "KCB", Amount = 500000000, Rate = 12.5m, Tenor = "7 days", Status = "Active" },
                        new { Bank = "Equity", Amount = 300000000, Rate = 12.75m, Tenor = "14 days", Status = "Active" }
                    },
                    InterbankBorrowings = new[]
                    {
                        new { Bank = "Standard Chartered", Amount = 200000000, Rate = 12.0m, Tenor = "Overnight", Status = "Active" }
                    },
                    AverageRate = 12.4m,
                    TotalOutstanding = 1000000000,
                    UpdatedAt = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #region Helper Methods

    private static async Task<T> ExecuteScalarAsync<T>(NpgsqlConnection connection, string query)
    {
        await using var command = new NpgsqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();
        return result == null || result is DBNull ? default(T)! : (T)Convert.ChangeType(result, typeof(T))!;
    }

    #endregion
}
