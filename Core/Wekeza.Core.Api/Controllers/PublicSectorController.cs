using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Public Sector Portal endpoints
/// </summary>
[ApiController]
[Route("api/public-sector")]
[Authorize]
public class PublicSectorController : ControllerBase
{
    private readonly IDbConnection _dbConnection;

    public PublicSectorController(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    /// <summary>
    /// Get dashboard metrics for public sector portal
    /// </summary>
    [HttpGet("dashboard/metrics")]
    [ProducesResponseType(typeof(DashboardMetricsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        try
        {
            // Ensure connection is open
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            
            // Query securities portfolio
            var securitiesQuery = @"
                SELECT 
                    COALESCE(SUM(CASE WHEN s.""SecurityType"" = 'TBILL' THEN so.""TotalAmount"" ELSE 0 END), 0) as tbillsvalue,
                    COALESCE(SUM(CASE WHEN s.""SecurityType"" = 'BOND' THEN so.""TotalAmount"" ELSE 0 END), 0) as bondsvalue,
                    COALESCE(SUM(CASE WHEN s.""SecurityType"" = 'STOCK' THEN so.""TotalAmount"" ELSE 0 END), 0) as stocksvalue,
                    COALESCE(SUM(so.""TotalAmount""), 0) as totalvalue
                FROM ""SecurityOrders"" so
                JOIN ""Securities"" s ON so.""SecurityId"" = s.""Id""
                WHERE so.""Status"" = 'Executed' AND so.""OrderType"" = 'BUY'";

            var securitiesData = await _dbConnection.QueryFirstOrDefaultAsync<SecuritiesQueryResult>(securitiesQuery) ?? new SecuritiesQueryResult();

            // Query loan portfolio
            var loansQuery = @"
                SELECT 
                    COALESCE(SUM(""OutstandingBalance""), 0) as totaloutstanding,
                    COALESCE(SUM(CASE WHEN ""LoanType"" = 'GOVERNMENT' THEN ""OutstandingBalance"" ELSE 0 END), 0) as nationalgovernment,
                    COALESCE(COUNT(*), 0) as totalloans
                FROM ""Loans""
                WHERE ""Status"" IN ('Disbursed', 'Active')";

            var loansData = await _dbConnection.QueryFirstOrDefaultAsync<LoansQueryResult>(loansQuery) ?? new LoansQueryResult();

            // Query banking metrics
            var bankingQuery = @"
                SELECT 
                    COALESCE(COUNT(DISTINCT a.""Id""), 0) as totalaccounts,
                    COALESCE(SUM(a.""BalanceAmount""), 0) as totalbalance,
                    COALESCE(COUNT(DISTINCT t.""Id""), 0) as monthlytransactions,
                    COALESCE(SUM(CASE WHEN t.""TransactionType"" = 'DEPOSIT' THEN t.""Amount"" ELSE 0 END), 0) as revenuecollected
                FROM ""Accounts"" a
                LEFT JOIN ""Transactions"" t ON a.""Id"" = t.""AccountId"" 
                    AND t.""ValueDate"" >= CURRENT_DATE - INTERVAL '30 days'
                WHERE a.""Status"" = 'Active'";

            var bankingData = await _dbConnection.QueryFirstOrDefaultAsync<BankingQueryResult>(bankingQuery) ?? new BankingQueryResult();

            // Query grants metrics
            var grantsQuery = @"
                SELECT 
                    COALESCE(SUM(""Amount""), 0) as totaldisbursed,
                    COALESCE(COUNT(*), 0) as activegrants,
                    COALESCE(AVG(""ComplianceRate""), 0) as compliancerate
                FROM ""Grants""
                WHERE ""Status"" IN ('Disbursed', 'Active')";

            var grantsData = await _dbConnection.QueryFirstOrDefaultAsync<GrantsQueryResult>(grantsQuery) ?? new GrantsQueryResult();

            var metrics = new DashboardMetricsResponse
            {
                Success = true,
                Data = new DashboardMetrics
                {
                    SecuritiesPortfolio = new SecuritiesPortfolioMetrics
                    {
                        TotalValue = securitiesData.TotalValue,
                        TbillsValue = securitiesData.TbillsValue,
                        BondsValue = securitiesData.BondsValue,
                        StocksValue = securitiesData.StocksValue,
                        YieldToMaturity = 12.5
                    },
                    LoanPortfolio = new LoanPortfolioMetrics
                    {
                        TotalOutstanding = loansData.TotalOutstanding,
                        NationalGovernment = loansData.NationalGovernment,
                        CountyGovernments = loansData.TotalOutstanding - loansData.NationalGovernment,
                        NplRatio = 2.3,
                        ExposureUtilization = 75.5
                    },
                    Banking = new BankingMetrics
                    {
                        TotalAccounts = bankingData.TotalAccounts,
                        TotalBalance = bankingData.TotalBalance,
                        MonthlyTransactions = bankingData.MonthlyTransactions,
                        RevenueCollected = bankingData.RevenueCollected
                    },
                    Grants = new GrantsMetrics
                    {
                        TotalDisbursed = grantsData.TotalDisbursed,
                        ActiveGrants = grantsData.ActiveGrants,
                        Beneficiaries = grantsData.ActiveGrants,
                        ComplianceRate = grantsData.ComplianceRate
                    }
                }
            };

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving dashboard metrics: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get revenue collection trends for the last 12 months
    /// </summary>
    [HttpGet("dashboard/revenue-trends")]
    public async Task<IActionResult> GetRevenueTrends()
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    TO_CHAR(t.""ValueDate"", 'Mon') as month,
                    EXTRACT(YEAR FROM t.""ValueDate"") as year,
                    EXTRACT(MONTH FROM t.""ValueDate"") as monthnumber,
                    SUM(t.""Amount"") as revenue
                FROM ""Transactions"" t
                WHERE t.""TransactionType"" = 'DEPOSIT'
                    AND t.""ValueDate"" >= CURRENT_DATE - INTERVAL '12 months'
                GROUP BY TO_CHAR(t.""ValueDate"", 'Mon'), EXTRACT(YEAR FROM t.""ValueDate""), EXTRACT(MONTH FROM t.""ValueDate"")
                ORDER BY year, monthnumber";

            var trends = await _dbConnection.QueryAsync<RevenueTrendData>(query);
            return Ok(new { success = true, data = trends });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving revenue trends: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get grant disbursement trends for the last 12 months
    /// </summary>
    [HttpGet("dashboard/grant-trends")]
    public async Task<IActionResult> GetGrantTrends()
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    TO_CHAR(g.""DisbursementDate"", 'Mon') as month,
                    EXTRACT(YEAR FROM g.""DisbursementDate"") as year,
                    EXTRACT(MONTH FROM g.""DisbursementDate"") as monthnumber,
                    SUM(g.""Amount"") as disbursed
                FROM ""Grants"" g
                WHERE g.""DisbursementDate"" IS NOT NULL
                    AND g.""DisbursementDate"" >= CURRENT_DATE - INTERVAL '12 months'
                GROUP BY TO_CHAR(g.""DisbursementDate"", 'Mon'), EXTRACT(YEAR FROM g.""DisbursementDate""), EXTRACT(MONTH FROM g.""DisbursementDate"")
                ORDER BY year, monthnumber";

            var trends = await _dbConnection.QueryAsync<GrantTrendData>(query);
            return Ok(new { success = true, data = trends });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving grant trends: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get list of available treasury bills
    /// </summary>
    [HttpGet("securities/treasury-bills")]
    public async Task<IActionResult> GetTreasuryBills()
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    ""Id"" as id,
                    ""SecurityCode"" as securitycode,
                    ""Name"" as name,
                    ""IssueDate"" as issuedate,
                    ""MaturityDate"" as maturitydate,
                    ""CouponRate"" as couponrate,
                    ""FaceValue"" as facevalue,
                    ""CurrentPrice"" as currentprice,
                    ""Status"" as status
                FROM ""Securities""
                WHERE ""SecurityType"" = 'TBILL' AND ""Status"" = 'Active'
                ORDER BY ""MaturityDate""";

            var tbills = await _dbConnection.QueryAsync<SecurityDto>(query);
            return Ok(new { success = true, data = tbills });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving treasury bills: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get list of available bonds
    /// </summary>
    [HttpGet("securities/bonds")]
    public async Task<IActionResult> GetBonds()
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    ""Id"" as id,
                    ""SecurityCode"" as securitycode,
                    ""Name"" as name,
                    ""IssueDate"" as issuedate,
                    ""MaturityDate"" as maturitydate,
                    ""CouponRate"" as couponrate,
                    ""FaceValue"" as facevalue,
                    ""CurrentPrice"" as currentprice,
                    ""Status"" as status
                FROM ""Securities""
                WHERE ""SecurityType"" = 'BOND' AND ""Status"" = 'Active'
                ORDER BY ""MaturityDate""";

            var bonds = await _dbConnection.QueryAsync<SecurityDto>(query);
            return Ok(new { success = true, data = bonds });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving bonds: {ex.Message}" });
        }
    }

    /// <summary>
    /// Place a security order
    /// </summary>
    [HttpPost("securities/orders")]
    public async Task<IActionResult> PlaceSecurityOrder([FromBody] PlaceOrderRequest request)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var orderId = Guid.NewGuid();
            var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{orderId.ToString().Substring(0, 8).ToUpper()}";

            var query = @"
                INSERT INTO ""SecurityOrders"" 
                (""Id"", ""OrderNumber"", ""CustomerId"", ""SecurityId"", ""OrderType"", ""Quantity"", ""Price"", ""TotalAmount"", ""Status"", ""OrderDate"")
                VALUES 
                (@Id, @OrderNumber, @CustomerId, @SecurityId, @OrderType, @Quantity, @Price, @TotalAmount, @Status, @OrderDate)";

            await _dbConnection.ExecuteAsync(query, new
            {
                Id = orderId,
                OrderNumber = orderNumber,
                CustomerId = request.CustomerId,
                SecurityId = request.SecurityId,
                OrderType = request.OrderType,
                Quantity = request.Quantity,
                Price = request.Price,
                TotalAmount = request.Quantity * request.Price,
                Status = "Pending",
                OrderDate = DateTime.UtcNow
            });

            return Ok(new { success = true, message = "Order placed successfully", orderNumber });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error placing order: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get loan applications
    /// </summary>
    [HttpGet("loans/applications")]
    public async Task<IActionResult> GetLoanApplications([FromQuery] string? status = null)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    l.""Id"" as id,
                    l.""LoanNumber"" as loannumber,
                    c.""Name"" as customername,
                    l.""LoanType"" as loantype,
                    l.""PrincipalAmount"" as principalamount,
                    l.""InterestRate"" as interestrate,
                    l.""TenorMonths"" as tenormonths,
                    l.""Status"" as status,
                    l.""ApplicationDate"" as applicationdate
                FROM ""Loans"" l
                JOIN ""Customers"" c ON l.""CustomerId"" = c.""Id""
                WHERE (@Status IS NULL OR l.""Status"" = @Status)
                ORDER BY l.""ApplicationDate"" DESC";

            var loans = await _dbConnection.QueryAsync<LoanApplicationDto>(query, new { Status = status });
            return Ok(new { success = true, data = loans });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving loan applications: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get government accounts
    /// </summary>
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    a.""Id"" as id,
                    a.""AccountNumber"" as accountnumber,
                    c.""Name"" as customername,
                    a.""AccountType"" as accounttype,
                    a.""BalanceAmount"" as balance,
                    a.""CurrencyCode"" as currency,
                    a.""Status"" as status
                FROM ""Accounts"" a
                JOIN ""Customers"" c ON a.""CustomerId"" = c.""Id""
                WHERE a.""Status"" = 'Active'
                ORDER BY c.""Name""";

            var accounts = await _dbConnection.QueryAsync<AccountDto>(query);
            return Ok(new { success = true, data = accounts });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving accounts: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get grant programs
    /// </summary>
    [HttpGet("grants/programs")]
    public async Task<IActionResult> GetGrantPrograms()
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    ""Id"" as id,
                    ""GrantNumber"" as grantnumber,
                    ""GrantType"" as granttype,
                    ""Amount"" as amount,
                    ""Purpose"" as purpose,
                    ""Status"" as status,
                    ""ApplicationDate"" as applicationdate
                FROM ""Grants""
                ORDER BY ""ApplicationDate"" DESC";

            var grants = await _dbConnection.QueryAsync<GrantDto>(query);
            return Ok(new { success = true, data = grants });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error retrieving grant programs: {ex.Message}" });
        }
    }
}

// Query result DTOs
public class SecuritiesQueryResult
{
    public decimal TbillsValue { get; set; }
    public decimal BondsValue { get; set; }
    public decimal StocksValue { get; set; }
    public decimal TotalValue { get; set; }
}

public class LoansQueryResult
{
    public decimal TotalOutstanding { get; set; }
    public decimal NationalGovernment { get; set; }
    public long TotalLoans { get; set; }
}

public class BankingQueryResult
{
    public long TotalAccounts { get; set; }
    public decimal TotalBalance { get; set; }
    public long MonthlyTransactions { get; set; }
    public decimal RevenueCollected { get; set; }
}

public class GrantsQueryResult
{
    public decimal TotalDisbursed { get; set; }
    public long ActiveGrants { get; set; }
    public decimal ComplianceRate { get; set; }
}

public record DashboardMetricsResponse
{
    public bool Success { get; init; }
    public DashboardMetrics? Data { get; init; }
}

public record DashboardMetrics
{
    public SecuritiesPortfolioMetrics SecuritiesPortfolio { get; init; } = new();
    public LoanPortfolioMetrics LoanPortfolio { get; init; } = new();
    public BankingMetrics Banking { get; init; } = new();
    public GrantsMetrics Grants { get; init; } = new();
}

public record SecuritiesPortfolioMetrics
{
    public decimal TotalValue { get; init; }
    public decimal TbillsValue { get; init; }
    public decimal BondsValue { get; init; }
    public decimal StocksValue { get; init; }
    public double YieldToMaturity { get; init; }
}

public record LoanPortfolioMetrics
{
    public decimal TotalOutstanding { get; init; }
    public decimal NationalGovernment { get; init; }
    public decimal CountyGovernments { get; init; }
    public double NplRatio { get; init; }
    public double ExposureUtilization { get; init; }
}

public record BankingMetrics
{
    public long TotalAccounts { get; init; }
    public decimal TotalBalance { get; init; }
    public long MonthlyTransactions { get; init; }
    public decimal RevenueCollected { get; init; }
}

public record GrantsMetrics
{
    public decimal TotalDisbursed { get; init; }
    public long ActiveGrants { get; init; }
    public long Beneficiaries { get; init; }
    public decimal ComplianceRate { get; init; }
}

// Additional DTOs for endpoints
public class RevenueTrendData
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public int MonthNumber { get; set; }
    public decimal Revenue { get; set; }
}

public class GrantTrendData
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public int MonthNumber { get; set; }
    public decimal Disbursed { get; set; }
}

public class SecurityDto
{
    public Guid Id { get; set; }
    public string SecurityCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? MaturityDate { get; set; }
    public decimal? CouponRate { get; set; }
    public decimal FaceValue { get; set; }
    public decimal CurrentPrice { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PlaceOrderRequest
{
    public Guid CustomerId { get; set; }
    public Guid SecurityId { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class LoanApplicationDto
{
    public Guid Id { get; set; }
    public string LoanNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string LoanType { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TenorMonths { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
}

public class AccountDto
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class GrantDto
{
    public Guid Id { get; set; }
    public string GrantNumber { get; set; } = string.Empty;
    public string GrantType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
}
