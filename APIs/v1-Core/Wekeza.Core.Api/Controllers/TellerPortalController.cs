using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Claims;
using Wekeza.Core.Application.Features.Teller.Commands.StartSession;
using Wekeza.Core.Application.Features.Teller.Commands.EndSession;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessCashDeposit;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessCashWithdrawal;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessChequeDeposit;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessAccountOpening;
using Wekeza.Core.Application.Features.Teller.Commands.ProcessCustomerOnboarding;
using Wekeza.Core.Application.Features.Teller.Commands.VerifyCustomer;
using Wekeza.Core.Application.Features.Teller.Commands.PrintStatement;
using Wekeza.Core.Application.Features.Teller.Commands.BlockAccount;
using Wekeza.Core.Application.Features.Teller.Queries.GetTellerSession;
using Wekeza.Core.Application.Features.Teller.Queries.GetCashDrawerBalance;
using Wekeza.Core.Application.Features.Teller.Queries.GetCustomerAccounts;
using Wekeza.Core.Application.Features.Teller.Queries.GetAccountBalance;
using Wekeza.Core.Application.Features.Teller.Queries.GetTransactionHistory;
using Wekeza.Core.Application.Features.Teller.Queries.SearchCustomers;

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
    private readonly IConfiguration _configuration;

    public TellerPortalController(IMediator mediator, IConfiguration configuration) 
        : base(mediator) 
    {
        _configuration = configuration;
    }

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
    public async Task<IActionResult> EndSession([FromBody] EndSessionCommand command)
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

    #region Dashboard - Real Data

    /// <summary>
    /// Get teller dashboard data with real metrics from database
    /// Returns drawer balance, transactions today, customers served, and session duration
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Get drawer balance (sum of all active account balances)
            var drawerBalance = await ExecuteScalarAsync<decimal>(
                connection,
                "SELECT COALESCE(SUM(\"Balance\"), 0) FROM \"Accounts\" WHERE \"Status\" = 'Active'");

            // Get transactions today count
            var transactionsToday = await ExecuteScalarAsync<int>(
                connection,
                "SELECT COUNT(*) FROM \"Transactions\" WHERE DATE(\"CreatedAt\") = CURRENT_DATE");

            // Get unique customers served (from transactions)
            var customersServed = await ExecuteScalarAsync<int>(
                connection,
                @"SELECT COUNT(DISTINCT a.""CustomerId"") 
                  FROM ""Transactions"" t
                  JOIN ""Accounts"" a ON t.""AccountId"" = a.""Id""
                  WHERE DATE(t.""CreatedAt"") = CURRENT_DATE");

            // Get current teller info
            var tellerFullName = User.FindFirst("FullName")?.Value ?? "Unknown";
            var tellerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

            // Get session start time (for duration calculation) - use current session or login time
            var sessionDuration = "Active";
            try
            {
                var sessionStart = await ExecuteScalarAsync<DateTime?>(
                    connection,
                    @"SELECT ""CreatedAt"" FROM ""Sessions"" 
                      WHERE ""UserId"" = @userId AND ""EndTime"" IS NULL
                      ORDER BY ""CreatedAt"" DESC LIMIT 1",
                    ("@userId", tellerId));

                if (sessionStart.HasValue)
                {
                    var duration = DateTime.UtcNow - sessionStart.Value;
                    sessionDuration = $"{duration.Hours}h {duration.Minutes}m";
                }
            }
            catch
            {
                sessionDuration = "Active";
            }

            return Ok(new
            {
                drawerBalance = drawerBalance,
                transactionsToday = transactionsToday,
                customersServed = customersServed,
                sessionDuration = sessionDuration,
                tellerName = tellerFullName,
                lastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve dashboard data", details = ex.Message });
        }
    }

    /// <summary>
    /// Get recent transactions for teller dashboard
    /// </summary>
    [HttpGet("transactions/recent")]
    public async Task<IActionResult> GetRecentTransactions()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var transactions = new List<dynamic>();
            
            var query = @"
                SELECT 
                    t.""Id"",
                    t.""TransactionReference"" as ""Reference"",
                    t.""Type"",
                    a.""AccountNumber"",
                    t.""Amount"",
                    t.""CreatedAt"" as ""Timestamp"",
                    t.""Description"",
                    CASE WHEN t.""Status"" = 'Completed' THEN 'Success' 
                         WHEN t.""Status"" = 'Pending' THEN 'Pending'
                         ELSE t.""Status"" END as ""Status""
                FROM ""Transactions"" t
                JOIN ""Accounts"" a ON t.""AccountId"" = a.""Id""
                WHERE DATE(t.""CreatedAt"") = CURRENT_DATE
                ORDER BY t.""CreatedAt"" DESC
                LIMIT 10";

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transactions.Add(new
                {
                    Id = reader.IsDBNull(0) ? Guid.Empty : reader.GetGuid(0),
                    Reference = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Type = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    AccountNumber = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Amount = reader.IsDBNull(4) ? 0m : reader.GetDecimal(4),
                    Timestamp = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                    Description = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Status = reader.IsDBNull(7) ? "" : reader.GetString(7)
                });
            }

            return Ok(new
            {
                transactions = transactions,
                count = transactions.Count,
                lastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve transactions", details = ex.Message });
        }
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
                TransactionId = result.Value,
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
                CustomerId = result.Value,
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
                AccountId = result.Value,
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
    public async Task<IActionResult> VerifyCustomer(Guid customerId, [FromBody] VerifyCustomerRequest request)
    {
        var command = new VerifyCustomerCommand 
        {
            CustomerIdentifier = customerId.ToString(),
            IdentificationType = request.IdentificationType ?? "CustomerID"
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Print account statement
    /// </summary>
    [HttpPost("accounts/{accountId:guid}/statement")]
    public async Task<IActionResult> PrintStatement(Guid accountId, [FromBody] PrintStatementRequest request)
    {
        var command = new PrintStatementCommand 
        {
            AccountNumber = accountId.ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Block/Unblock account
    /// </summary>
    [HttpPost("accounts/block")]
    public async Task<IActionResult> BlockAccount([FromBody] BlockAccountCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Execute a scalar query and return typed result
    /// </summary>
    private static async Task<T> ExecuteScalarAsync<T>(NpgsqlConnection connection, string query)
    {
        await using var command = new NpgsqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();
        return result == null || result is DBNull ? default(T)! : (T)Convert.ChangeType(result, typeof(T))!;
    }

    /// <summary>
    /// Execute a scalar query with parameters and return typed result
    /// </summary>
    private static async Task<T> ExecuteScalarAsync<T>(NpgsqlConnection connection, string query, params (string, object)[] parameters)
    {
        await using var command = new NpgsqlCommand(query, connection);
        foreach (var (paramName, paramValue) in parameters)
        {
            command.Parameters.AddWithValue(paramName, paramValue ?? DBNull.Value);
        }
        var result = await command.ExecuteScalarAsync();
        return result == null || result is DBNull ? default(T)! : (T)Convert.ChangeType(result, typeof(T))!;
    }

    #endregion
}

// Request models
public class VerifyCustomerRequest
{
    public string? IdentificationType { get; set; }
}

public class PrintStatementRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}