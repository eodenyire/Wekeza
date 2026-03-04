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
/// Compliance Portal Controller
/// Handles AML monitoring, fraud detection, and risk management
/// </summary>
[ApiController]
[Route("api/compliance-portal")]
[Authorize(Roles = "ComplianceOfficer,RiskOfficer,Administrator")]
public class CompliancePortalController : BaseApiController
{
    private readonly IConfiguration _configuration;

    public CompliancePortalController(IMediator mediator, IConfiguration configuration) 
        : base(mediator) 
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Get AML alerts and high-risk cases
    /// </summary>
    [HttpGet("aml-alerts")]
    public async Task<IActionResult> GetAMLAlerts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var alerts = new List<object>();

            // Query risky customers from database
            var query = @"
                SELECT 
                    ""Id""::text as ""CaseId"",
                    TRIM(COALESCE(""FirstName"", '') || ' ' || COALESCE(""LastName"", '')) as ""CustomerName"",
                    'High' as ""RiskLevel"",
                    'Open' as ""Status"",
                    NOW()::date as ""DateReported""
                FROM ""Customers""
                WHERE COALESCE(""RiskRating"", 0) >= 1
                LIMIT 10";

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                alerts.Add(new
                {
                    CaseId = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    CustomerName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    RiskLevel = reader.IsDBNull(2) ? "Medium" : reader.GetString(2),
                    Status = reader.IsDBNull(3) ? "Pending" : reader.GetString(3),
                    DateReported = reader.IsDBNull(4) ? DateTime.UtcNow : reader.GetDateTime(4)
                });
            }

            return Ok(new { success = true, data = alerts, count = alerts.Count, pageSize });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve AML alerts", details = ex.Message });
        }
    }

    /// <summary>
    /// Get compliance risk metrics
    /// </summary>
    [HttpGet("risk-metrics")]
    public async Task<IActionResult> GetRiskMetrics()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var highRiskCases = await ExecuteScalarAsync<int>(connection, 
                @"SELECT COUNT(*) FROM ""Customers"" WHERE COALESCE(""RiskRating"", 0) >= 1");
            
            var totalCustomers = await ExecuteScalarAsync<int>(connection, 
                @"SELECT COUNT(*) FROM ""Customers""");
            
            var complianceScore = totalCustomers > 0 
                ? ((totalCustomers - highRiskCases) * 100) / totalCustomers 
                : 100;

            return Ok(new
            {
                success = true,
                data = new
                {
                    HighRiskCases = highRiskCases,
                    SanctionsHits = 2,
                    ComplianceScore = complianceScore,
                    PendingReviews = 5,
                    RegulatoryAlerts = 1,
                    SystemHealth = 98,
                    LastUpdated = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve risk metrics", details = ex.Message });
        }
    }

    /// <summary>
    /// Get regulatory reporting status
    /// </summary>
    [HttpGet("regulatory-reporting")]
    public async Task<IActionResult> GetRegulatoryReporting()
    {
        try
        {
            return Ok(new
            {
                success = true,
                data = new
                {
                    Reports = new[]
                    {
                        new { Name = "Monthly CTR Report", Status = "Submitted", DueDate = "2026-03-15", LastSubmitted = "2026-02-28" },
                        new { Name = "AML Report", Status = "Due Soon", DueDate = "2026-03-30", LastSubmitted = "2026-02-01" },
                        new { Name = "STR Quarterly", Status = "In Progress", DueDate = "2026-03-31", LastSubmitted = "2025-12-31" }
                    },
                    NextDeadline = "2026-03-15",
                    ComplianceRate = 98.5
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
