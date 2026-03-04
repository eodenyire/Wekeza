using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MediatR;
using Npgsql;
using Wekeza.Core.Application.Features.Accounts.Commands.OpenAccount;
using Wekeza.Core.Application.Features.Accounts.Commands.OpenProductBasedAccount;
using Wekeza.Core.Application.Features.Accounts.Commands.RegisterBusiness;
using Wekeza.Core.Application.Features.Accounts.Commands.Management;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccountSummary;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccount;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
///📂 Wekeza.Core.Api/Controllers/AccountsController.cs
///This controller is designed for high-volume identity management. It handles Individual and Corporate onboarding with strict RESTful patterns.
/// The Identity & Lifecycle Gateway.
/// Manages account creation, corporate onboarding, and administrative controls.
/// </summary>
public class AccountsController : BaseApiController
{
    private readonly IConfiguration _configuration;

    public AccountsController(IMediator mediator, IConfiguration configuration) : base(mediator)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// QUERY: Retrieves a paginated list of all accounts with filters.
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var offset = (pageNumber - 1) * pageSize;
            
            var query = @"
                SELECT ""Id"", ""AccountNumber"", ""AccountType"", ""Balance"", ""Currency"", ""Status"", ""CreatedAt""
                FROM ""Accounts""
                WHERE ""Balance"" > 0" +
                (string.IsNullOrWhiteSpace(searchTerm) ? "" : @" AND (""AccountNumber"" ILIKE @search OR ""AccountType"" ILIKE @search)") +
                @" ORDER BY ""CreatedAt"" DESC
                LIMIT @pageSize OFFSET @offset;
                
                SELECT COUNT(*) as total FROM ""Accounts"" WHERE ""Balance"" > 0" +
                (string.IsNullOrWhiteSpace(searchTerm) ? "" : @" AND (""AccountNumber"" ILIKE @search OR ""AccountType"" ILIKE @search)");

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@offset", offset);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                command.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
            }

            await using var reader = await command.ExecuteReaderAsync();
            var accounts = new List<object>();
            
            while (await reader.ReadAsync())
            {
                accounts.Add(new
                {
                    id = reader.GetGuid(0),
                    accountNumber = reader.GetString(1),
                    accountType = reader.GetString(2),
                    balance = reader.GetDecimal(3),
                    currency = reader.GetString(4),
                    status = reader.GetString(5),
                    createdAt = reader.GetDateTime(6)
                });
            }

            await reader.NextResultAsync();
            int total = 0;
            if (await reader.ReadAsync())
            {
                total = reader.GetInt32(0);
            }

            return Ok(new
            {
                data = accounts,
                pagination = new
                {
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalRecords = total,
                    totalPages = (total + pageSize - 1) / pageSize
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve accounts", details = ex.Message });
        }
    }

    /// <summary>
    /// QUERY: Retrieves a single account by account number.
    /// </summary>
    [HttpGet("{accountNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNumber(string accountNumber)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(
                @"SELECT ""Id"", ""AccountNumber"", ""AccountType"", ""Balance"", ""Currency"", ""Status"", ""CreatedAt""
                  FROM ""Accounts""
                  WHERE ""AccountNumber"" = @accountNumber",
                connection);
            command.Parameters.AddWithValue("@accountNumber", accountNumber);

            await using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return Ok(new
                {
                    id = reader.GetGuid(0),
                    accountNumber = reader.GetString(1),
                    accountType = reader.GetString(2),
                    balance = reader.GetDecimal(3),
                    currency = reader.GetString(4),
                    status = reader.GetString(5),
                    createdAt = reader.GetDateTime(6)
                });
            }

            return NotFound(new { error = "Account not found", accountNumber = accountNumber });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve account", details = ex.Message });
        }
    }

    /// <summary>
    /// RETAIL: Opens a standard individual savings or current account.
    /// </summary>
    [HttpPost("individual")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> OpenIndividual(OpenAccountCommand command)
    {
        var accountId = await Mediator.Send(command);
        return CreatedAtAction(nameof(OpenIndividual), new { id = accountId }, accountId);
    }

    /// <summary>
    /// PRODUCT-BASED: Opens an account using Product Factory configuration with GL integration
    /// This is the new enterprise-grade account opening with automatic GL posting
    /// </summary>
    [HttpPost("product-based")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<OpenProductBasedAccountResult>> OpenProductBasedAccount(OpenProductBasedAccountCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(OpenProductBasedAccount), new { id = result.AccountId }, result);
    }

    /// <summary>
    /// CORPORATE: Onboards a business entity with UBO (Director) details.
    /// </summary>
    [HttpPost("business")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AccountDto>> RegisterBusiness(RegisterBusinessCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(RegisterBusiness), new { id = result }, result);
    }

    /// <summary>
    /// VISIBILITY: Fetches the real-time summary of an account.
    /// </summary>
    [HttpGet("{accountNumber}/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AccountSummaryDto>> GetSummary(string accountNumber)
    {
        return Ok(await Mediator.Send(new GetAccountSummaryQuery { AccountNumber = accountNumber }));
    }

    /// <summary>
    /// SECURITY: Freezes an account immediately (Managerial/Risk Engine Action).
    /// </summary>
    [HttpPatch("{accountNumber}/freeze")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> Freeze(string accountNumber, [FromBody] string reason)
    {
        return Ok(await Mediator.Send(new FreezeAccountCommand(accountNumber, reason)));
    }

    /// <summary>
    /// LIFECYCLE: Permanently closes an account (Only if balance is zero).
    /// </summary>
    [HttpDelete("{accountNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> Close(string accountNumber)
    {
        return Ok(await Mediator.Send(new CloseAccountCommand(accountNumber)));
    }
}
