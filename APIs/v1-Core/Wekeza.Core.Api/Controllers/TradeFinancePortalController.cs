using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Trade Finance Portal Controller
/// Handles letters of credit, guarantees, and documentary collections
/// </summary>
[ApiController]
[Route("api/trade-finance-portal")]
[Authorize(Roles = "TradeFinanceOfficer,CorporateBankingOfficer,Administrator")]
public class TradeFinancePortalController : BaseApiController
{
    private readonly IConfiguration _configuration;

    public TradeFinancePortalController(IMediator mediator, IConfiguration configuration) 
        : base(mediator) 
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Get letters of credit
    /// </summary>
    [HttpGet("letters-of-credit")]
    public async Task<IActionResult> GetLettersOfCredit([FromQuery] int limit = 10)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var lcs = new List<object>();
            var query = @"
                SELECT 
                    ""Id""::text as ""LCId"",
                    TRIM(COALESCE(""FirstName"", '') || ' ' || COALESCE(""LastName"", '')) as ""Beneficiary"",
                    'Active' as ""Status""
                FROM ""Customers""
                LIMIT 5";

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lcs.Add(new
                {
                    LCId = $"LC-{reader.GetString(0).Substring(0, 6).ToUpper()}",
                    Beneficiary = reader.GetString(1),
                    Amount = 500000000,
                    Currency = "USD",
                    Status = "Active",
                    IssuanceDate = DateTime.UtcNow.AddDays(-5),
                    ExpiryDate = DateTime.UtcNow.AddMonths(3)
                });
            }

            return Ok(new { success = true, data = lcs });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve letters of credit", details = ex.Message });
        }
    }

    /// <summary>
    /// Get bank guarantees
    /// </summary>
    [HttpGet("bank-guarantees")]
    public async Task<IActionResult> GetBankGuarantees()
    {
        try
        {
            var guarantees = new[]
            {
                new { GuaranteeId = "BG-001", Beneficiary = "JKIA Authority", Amount = 100000000, Status = "Active", Type = "Performance" },
                new { GuaranteeId = "BG-002", Beneficiary = "Construction Co", Amount = 250000000, Status = "Active", Type = "Bid" },
                new { GuaranteeId = "BG-003", Beneficiary = "Suppliers Ltd", Amount = 75000000, Status = "Released", Type = "Payment" }
            };

            return Ok(new { success = true, data = guarantees });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get documentary collections
    /// </summary>
    [HttpGet("documentary-collections")]
    public async Task<IActionResult> GetDocumentaryCollections()
    {
        try
        {
            var collections = new[]
            {
                new { CollectionId = "DC-001", Applicant = "Nairobi Traders", Amount = 250000000, Currency = "USD", Status = "In Transit", CreatedDate = DateTime.UtcNow.AddDays(-3) },
                new { CollectionId = "DC-002", Applicant = "Export Foods", Amount = 150000000, Currency = "EUR", Status = "Documents Received", CreatedDate = DateTime.UtcNow.AddDays(-1) }
            };

            return Ok(new { success = true, data = collections });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
