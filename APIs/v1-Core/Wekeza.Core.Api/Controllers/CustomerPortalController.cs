using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Claims;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.SelfOnboard;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.UpdateProfile;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.ChangePassword;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.RequestCard;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.TransferFunds;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.PayBill;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollMobileBanking;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollInternetBanking;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollUSSD;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.BlockCard;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.ApplyForLoan;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.RepayLoan;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.BuyAirtime;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.DownloadStatement;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.RequestVirtualCard;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetProfile;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetAccounts;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetOnboardingStatus;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetCustomerProfile;
using Wekeza.Core.Application.Features.Teller.Queries.GetCustomerAccounts;
using Wekeza.Core.Application.Features.Teller.Queries.GetAccountBalance;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetAccountTransactions;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetTransactions;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetCards;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetLoans;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Customer Portal Controller - Self-service banking portal
/// Enables customers to onboard themselves and perform banking operations
/// </summary>
[ApiController]
[Route("api/customer-portal")]
public class CustomerPortalController : BaseApiController
{
    private readonly IConfiguration _configuration;

    public CustomerPortalController(IMediator mediator, IConfiguration configuration) 
        : base(mediator) 
    {
        _configuration = configuration;
    }

    #region Dashboard - Real Data

    /// <summary>
    /// Get customer dashboard with real account summary from database
    /// Returns account balances, recent transactions, and financial overview
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Get customer ID from JWT claims
            var customerIdClaim = User.FindFirst("CustomerId")?.Value 
                ?? User.FindFirst(ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(customerIdClaim))
            {
                return Unauthorized(new { error = "Customer ID not found in token" });
            }

            // Get total balance across all accounts
            var totalBalance = await ExecuteScalarAsync<decimal>(
                connection,
                @"SELECT COALESCE(SUM(""Balance""), 0) 
                  FROM ""Accounts"" 
                  WHERE ""CustomerId""::text = @customerId AND ""Status"" = 'Active'",
                ("@customerId", customerIdClaim));

            // Get number of accounts
            var accountCount = await ExecuteScalarAsync<int>(
                connection,
                @"SELECT COUNT(*) 
                  FROM ""Accounts"" 
                  WHERE ""CustomerId""::text = @customerId AND ""Status"" = 'Active'",
                ("@customerId", customerIdClaim));

            // Get recent transactions count (last 30 days)
            var recentTransactionCount = await ExecuteScalarAsync<int>(
                connection,
                @"SELECT COUNT(*) 
                  FROM ""Transactions"" t
                  JOIN ""Accounts"" a ON t.""AccountId"" = a.""Id""
                  WHERE a.""CustomerId""::text = @customerId 
                  AND t.""CreatedAt"" >= NOW() - INTERVAL '30 days'",
                ("@customerId", customerIdClaim));

            // Get account details
            var accounts = new List<dynamic>();
            var accountQuery = @"
                SELECT 
                    ""Id"",
                    ""AccountNumber"",
                    ""AccountType"",
                    ""Balance"",
                    ""Status"",
                    ""Currency""
                FROM ""Accounts""
                WHERE ""CustomerId""::text = @customerId AND ""Status"" = 'Active'
                ORDER BY ""CreatedAt"" DESC";

            await using var command = new NpgsqlCommand(accountQuery, connection);
            command.Parameters.AddWithValue("@customerId", customerIdClaim);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                accounts.Add(new
                {
                    Id = reader.GetGuid(0),
                    AccountNumber = reader.GetString(1),
                    AccountType = reader.IsDBNull(2) ? "Savings" : reader.GetString(2),
                    Balance = reader.GetDecimal(3),
                    Status = reader.GetString(4),
                    Currency = reader.IsDBNull(5) ? "KES" : reader.GetString(5)
                });
            }

            return Ok(new
            {
                totalBalance = totalBalance,
                accountCount = accountCount,
                recentTransactionCount = recentTransactionCount,
                accounts = accounts,
                customerId = customerIdClaim,
                lastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve dashboard data", details = ex.Message });
        }
    }

    /// <summary>
    /// Get recent transactions across all customer accounts
    /// </summary>
    [HttpGet("transactions/recent")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> GetRecentTransactions([FromQuery] int limit = 10)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var customerIdClaim = User.FindFirst("CustomerId")?.Value 
                ?? User.FindFirst(ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(customerIdClaim))
            {
                return Unauthorized(new { error = "Customer ID not found in token" });
            }

            var transactions = new List<dynamic>();
            
            var query = @"
                SELECT 
                    t.""Id"",
                    t.""TransactionReference"" as ""Reference"",
                    t.""TransactionType"" as ""Type"",
                    a.""AccountNumber"",
                    t.""Amount"",
                    t.""CreatedAt"" as ""Timestamp"",
                    t.""Description"",
                    CASE WHEN t.""Status"" = 'Completed' THEN 'Success' 
                         WHEN t.""Status"" = 'Pending' THEN 'Pending'
                         ELSE t.""Status"" END as ""Status""
                FROM ""Transactions"" t
                JOIN ""Accounts"" a ON t.""AccountId"" = a.""Id""
                WHERE a.""CustomerId""::text = @customerId
                ORDER BY t.""CreatedAt"" DESC
                LIMIT @limit";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@customerId", customerIdClaim);
            command.Parameters.AddWithValue("@limit", limit);
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

    #region Self-Onboarding

    /// <summary>
    /// Customer self-onboarding - Step 1: Basic Information
    /// </summary>
    [HttpPost("onboard/basic-info")]
    [AllowAnonymous]
    public async Task<IActionResult> OnboardBasicInfo([FromBody] SelfOnboardBasicInfoCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                OnboardingId = result.Value.OnboardingId,
                NextStep = "document-upload",
                Message = "Basic information saved. Please upload your identification documents."
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Customer self-onboarding - Step 2: Document Upload
    /// </summary>
    [HttpPost("onboard/documents")]
    [AllowAnonymous]
    public async Task<IActionResult> OnboardDocuments([FromBody] SelfOnboardDocumentsCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                OnboardingId = command.OnboardingId,
                NextStep = "verification",
                Message = "Documents uploaded successfully. Your application is under review."
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Customer self-onboarding - Step 3: Account Setup
    /// </summary>
    [HttpPost("onboard/account-setup")]
    [AllowAnonymous]
    public async Task<IActionResult> OnboardAccountSetup([FromBody] SelfOnboardAccountSetupCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                CustomerId = result.Value.CustomerId,
                CIFNumber = result.Value.CIFNumber,
                AccountNumber = result.Value.AccountNumber,
                TempPassword = result.Value.TempPassword,
                Message = "Account created successfully. Please change your password on first login."
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Check onboarding status
    /// </summary>
    [HttpGet("onboard/status/{onboardingId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOnboardingStatus(Guid onboardingId)
    {
        var query = new GetOnboardingStatusQuery { OnboardingId = onboardingId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Profile Management

    /// <summary>
    /// Get customer profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> GetProfile()
    {
        var query = new GetCustomerProfileQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update customer profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("profile/change-password")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Account Services

    /// <summary>
    /// Get customer accounts
    /// </summary>
    [HttpGet("accounts")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> GetAccounts()
    {
        var query = new GetCustomerAccountsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get account balance
    /// </summary>
    [HttpGet("accounts/{accountId:guid}/balance")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> GetAccountBalance(Guid accountId)
    {
        var query = new GetAccountBalanceQuery { AccountId = accountId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get account transactions
    /// </summary>
    [HttpGet("accounts/{accountId:guid}/transactions")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> GetTransactions(Guid accountId, [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
    {
        var query = new GetAccountTransactionsQuery
        {
            AccountId = accountId,
            PageSize = pageSize,
            PageNumber = pageNumber
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Download account statement
    /// </summary>
    [HttpPost("accounts/{accountId:guid}/statement")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> DownloadStatement(Guid accountId, [FromBody] DownloadStatementCommand command)
    {
        var updatedCommand = command with { AccountId = accountId };
        var result = await Mediator.Send(updatedCommand);
        return Ok(result);
    }

    #endregion

    #region Transactions

    /// <summary>
    /// Transfer funds between accounts
    /// </summary>
    [HttpPost("transactions/transfer")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> TransferFunds([FromBody] TransferFundsCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                TransactionId = result.Value,
                Message = "Transfer initiated successfully"
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Pay bill
    /// </summary>
    [HttpPost("transactions/pay-bill")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> PayBill([FromBody] PayBillCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Buy airtime
    /// </summary>
    [HttpPost("transactions/buy-airtime")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> BuyAirtime([FromBody] BuyAirtimeCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Card Services

    /// <summary>
    /// Get customer cards
    /// </summary>
    [HttpGet("cards")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> GetCards()
    {
        // TODO: Get customer ID from authenticated user claims
        var query = new GetCardsQuery { CustomerId = Guid.Empty };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Request new card
    /// </summary>
    [HttpPost("cards/request")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> RequestCard([FromBody] RequestCardCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Request virtual card
    /// </summary>
    [HttpPost("cards/request-virtual")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> RequestVirtualCard([FromBody] RequestVirtualCardCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                CardId = result.Value.CardId,
                CardNumber = result.Value.MaskedCardNumber,
                ExpiryDate = result.Value.ExpiryDate,
                CVV = result.Value.CVV,
                Message = "Virtual card created successfully"
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Block/Unblock card
    /// </summary>
    [HttpPost("cards/block")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> BlockCard([FromBody] BlockCardCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Loan Services

    /// <summary>
    /// Get customer loans
    /// </summary>
    [HttpGet("loans")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> GetLoans()
    {
        var query = new GetLoansQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Apply for loan
    /// </summary>
    [HttpPost("loans/apply")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> ApplyForLoan([FromBody] ApplyForLoanCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Make loan repayment
    /// </summary>
    [HttpPost("loans/repay")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> RepayLoan([FromBody] RepayLoanCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Digital Channel Enrollment

    /// <summary>
    /// Enroll in mobile banking
    /// </summary>
    [HttpPost("channels/mobile/enroll")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> EnrollMobileBanking([FromBody] EnrollMobileBankingCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Enroll in internet banking
    /// </summary>
    [HttpPost("channels/internet/enroll")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> EnrollInternetBanking([FromBody] EnrollInternetBankingCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Enroll in USSD banking
    /// </summary>
    [HttpPost("channels/ussd/enroll")]
    [Authorize(Roles = "Customer,RetailCustomer")]
    public async Task<IActionResult> EnrollUSSD([FromBody] EnrollUSSDCommand command)
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