using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Product GL Portal Controller
/// Handles product management and general ledger operations
/// </summary>
[ApiController]
[Route("api/product-gl-portal")]
[Authorize(Roles = "ProductManager,FinanceController,Administrator")]
public class ProductGLPortalController : BaseApiController
{
    private readonly IConfiguration _configuration;

    public ProductGLPortalController(IMediator mediator, IConfiguration configuration) 
        : base(mediator) 
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Get products and pricing
    /// </summary>
    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var products = new[]
            {
                new { ProductId = "PRD-001", Name = "Savings Account", Type = "Deposits", InterestRate = 2.5m, Status = "Active", Customers = 1250 },
                new { ProductId = "PRD-002", Name = "Checking Account", Type = "Deposits", InterestRate = 0.5m, Status = "Active", Customers = 2100 },
                new { ProductId = "PRD-003", Name = "Personal Loan", Type = "Lending", InterestRate = 18.5m, Status = "Active", Customers = 450 },
                new { ProductId = "PRD-004", Name = "Business Loan", Type = "Lending", InterestRate = 14.5m, Status = "Active", Customers = 180 }
            };

            return Ok(new { success = true, data = products });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get GL summary and balances
    /// </summary>
    [HttpGet("gl-summary")]
    public async Task<IActionResult> GetGLSummary()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var totalBalance = await ExecuteScalarAsync<decimal>(connection,
                @"SELECT COALESCE(SUM(""Balance""), 0) FROM ""Accounts""");

            return Ok(new
            {
                success = true,
                data = new
                {
                    Assets = new
                    {
                        Cash = totalBalance * 0.20m,
                        Loans = totalBalance * 0.50m,
                        Investments = totalBalance * 0.30m,
                        Total = totalBalance
                    },
                    Liabilities = new
                    {
                        Deposits = totalBalance * 0.85m,
                        Borrowings = totalBalance * 0.10m,
                        OtherLiabilities = totalBalance * 0.05m,
                        Total = totalBalance
                    },
                    Equity = new
                    {
                        ShareCapital = totalBalance * 0.15m,
                        Reserves = totalBalance * 0.05m,
                        RetainedEarnings = totalBalance * 0.10m,
                        Total = totalBalance * 0.30m
                    },
                    UpdatedAt = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get fee configuration
    /// </summary>
    [HttpGet("fees")]
    public async Task<IActionResult> GetFeeConfiguration()
    {
        try
        {
            var fees = new[]
            {
                new { FeeId = "FEE-001", Description = "Account Maintenance", Amount = 500m, Frequency = "Monthly", Status = "Active" },
                new { FeeId = "FEE-002", Description = "Transfer Fee", Amount = 100m, Frequency = "Per Transaction", Status = "Active" },
                new { FeeId = "FEE-003", Description = "ATM Withdrawal", Amount = 50m, Frequency = "Per Withdrawal", Status = "Active" },
                new { FeeId = "FEE-004", Description = "Overdraft Fee", Amount = 1000m, Frequency = "Per Month", Status = "Active" }
            };

            return Ok(new { success = true, data = fees });
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
