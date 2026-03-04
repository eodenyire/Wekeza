using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Wekeza.Core.Api.Controllers;

[ApiController]
[Route("api/vif")]
[Authorize(Roles = "Administrator,Teller,Supervisor,BranchManager,Manager,CustomerService")]
public class VIFController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public VIFController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("customers/register")]
    public async Task<IActionResult> RegisterCustomer([FromBody] VifCustomerRegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.IdentificationNumber))
        {
            return BadRequest(new { message = "FirstName, LastName, and IdentificationNumber are required" });
        }

        var customerId = Guid.NewGuid();
        var cifNumber = BuildCif(customerId);
        var now = DateTime.UtcNow;

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string insertCustomerSql = @"
            INSERT INTO ""Customers""
            (""Id"", ""FirstName"", ""LastName"", ""Email"", ""PhoneNumber"", ""IdentificationNumber"", ""RiskRating"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
            VALUES
            (@id, @firstName, @lastName, @email, @phone, @idNumber, @riskRating, TRUE, @createdAt, @updatedAt);";

        await using (var command = new NpgsqlCommand(insertCustomerSql, connection))
        {
            command.Parameters.AddWithValue("@id", customerId);
            command.Parameters.AddWithValue("@firstName", request.FirstName.Trim());
            command.Parameters.AddWithValue("@lastName", request.LastName.Trim());
            command.Parameters.AddWithValue("@email", request.Email?.Trim() ?? $"{customerId:N}@vif.local");
            command.Parameters.AddWithValue("@phone", request.PhoneNumber?.Trim() ?? string.Empty);
            command.Parameters.AddWithValue("@idNumber", request.IdentificationNumber.Trim());
            command.Parameters.AddWithValue("@riskRating", request.RiskRating);
            command.Parameters.AddWithValue("@createdAt", now);
            command.Parameters.AddWithValue("@updatedAt", now);
            await command.ExecuteNonQueryAsync();
        }

        return Ok(new
        {
            CustomerId = customerId,
            CifNumber = cifNumber,
            Message = "Customer registered successfully. CIF generated."
        });
    }

    [HttpPost("accounts/register")]
    public async Task<IActionResult> RegisterAccount([FromBody] VifAccountRegistrationRequest request)
    {
        if (!TryParseCif(request.CifNumber, out var customerId))
        {
            return BadRequest(new { message = "Invalid CIF number format" });
        }

        if (request.InitialDeposit < 0)
        {
            return BadRequest(new { message = "InitialDeposit cannot be negative" });
        }

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        if (!await CustomerExistsAsync(connection, customerId))
        {
            return NotFound(new { message = "Customer not found for provided CIF" });
        }

        await using var tx = await connection.BeginTransactionAsync();

        var accountId = Guid.NewGuid();
        var accountNumber = await GenerateUniqueAccountNumberAsync(connection, tx);
        var openingBalance = request.InitialDeposit;
        var now = DateTime.UtcNow;

        const string insertAccountSql = @"
            INSERT INTO ""Accounts""
            (""Id"", ""AccountNumber"", ""CustomerId"", ""AccountType"", ""Currency"", ""Balance"", ""AvailableBalance"", ""Status"", ""OpenedDate"", ""CreatedAt"", ""UpdatedAt"", ""BranchCode"")
            VALUES
            (@id, @accountNumber, @customerId, @accountType, @currency, @balance, @availableBalance, 'Active', @openedDate, @createdAt, @updatedAt, @branchCode);";

        await using (var command = new NpgsqlCommand(insertAccountSql, connection, tx))
        {
            command.Parameters.AddWithValue("@id", accountId);
            command.Parameters.AddWithValue("@accountNumber", accountNumber);
            command.Parameters.AddWithValue("@customerId", customerId);
            command.Parameters.AddWithValue("@accountType", string.IsNullOrWhiteSpace(request.AccountType) ? "Savings" : request.AccountType.Trim());
            command.Parameters.AddWithValue("@currency", string.IsNullOrWhiteSpace(request.Currency) ? "KES" : request.Currency.Trim().ToUpperInvariant());
            command.Parameters.AddWithValue("@balance", openingBalance);
            command.Parameters.AddWithValue("@availableBalance", openingBalance);
            command.Parameters.AddWithValue("@openedDate", now);
            command.Parameters.AddWithValue("@createdAt", now);
            command.Parameters.AddWithValue("@updatedAt", now);
            command.Parameters.AddWithValue("@branchCode", string.IsNullOrWhiteSpace(request.BranchCode) ? "BR100001" : request.BranchCode.Trim());
            await command.ExecuteNonQueryAsync();
        }

        if (openingBalance > 0)
        {
            await InsertTransactionAsync(
                connection,
                tx,
                accountId,
                openingBalance,
                string.IsNullOrWhiteSpace(request.Currency) ? "KES" : request.Currency.Trim().ToUpperInvariant(),
                "Deposit",
                "Initial account funding",
                "Completed",
                openingBalance);
        }

        await tx.CommitAsync();

        return Ok(new
        {
            AccountId = accountId,
            AccountNumber = accountNumber,
            CustomerId = customerId,
            CifNumber = request.CifNumber,
            OpeningBalance = openingBalance,
            Message = "Account registered successfully"
        });
    }

    [HttpGet("accounts/{accountNumber}/balance")]
    public async Task<IActionResult> GetBalance(string accountNumber)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        var account = await GetAccountSnapshotAsync(connection, null, accountNumber, forUpdate: false);
        if (account is null)
        {
            return NotFound(new { message = "Account not found" });
        }

        return Ok(new
        {
            account.AccountNumber,
            account.Balance,
            account.AvailableBalance,
            account.Currency,
            account.Status
        });
    }

    [HttpPost("transactions/cash-deposit")]
    public async Task<IActionResult> CashDeposit([FromBody] VifAmountRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest(new { message = "Amount must be greater than zero" });

        var result = await ApplyCreditAsync(
            request.AccountNumber,
            request.Amount,
            request.Currency,
            "Deposit",
            request.Description ?? "Cash deposit via VIF");

        return Ok(result);
    }

    [HttpPost("transactions/cash-withdrawal")]
    public async Task<IActionResult> CashWithdrawal([FromBody] VifAmountRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest(new { message = "Amount must be greater than zero" });

        var result = await ApplyDebitAsync(
            request.AccountNumber,
            request.Amount,
            request.Currency,
            "Withdrawal",
            request.Description ?? "Cash withdrawal via VIF");

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPost("transactions/transfer")]
    public async Task<IActionResult> Transfer([FromBody] VifTransferRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest(new { message = "Amount must be greater than zero" });

        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();

        var source = await GetAccountSnapshotAsync(connection, tx, request.FromAccountNumber, forUpdate: true);
        var destination = await GetAccountSnapshotAsync(connection, tx, request.ToAccountNumber, forUpdate: true);

        if (source is null || destination is null)
        {
            return NotFound(new { message = "Source or destination account not found" });
        }

        if (source.AvailableBalance < request.Amount)
        {
            return BadRequest(new { message = "Insufficient balance" });
        }

        var sourceBalance = source.Balance - request.Amount;
        var destinationBalance = destination.Balance + request.Amount;
        var currency = NormalizeCurrency(request.Currency, source.Currency);
        var transferRef = BuildTransactionReference("TRF");

        await UpdateAccountBalanceAsync(connection, tx, source.Id, sourceBalance);
        await UpdateAccountBalanceAsync(connection, tx, destination.Id, destinationBalance);

        await InsertTransactionAsync(connection, tx, source.Id, request.Amount, currency, "Transfer", $"Transfer to {destination.AccountNumber} ({transferRef})", "Completed", sourceBalance);
        await InsertTransactionAsync(connection, tx, destination.Id, request.Amount, currency, "Transfer", $"Transfer from {source.AccountNumber} ({transferRef})", "Completed", destinationBalance);

        await tx.CommitAsync();

        return Ok(new
        {
            IsSuccess = true,
            TransferReference = transferRef,
            SourceAccount = source.AccountNumber,
            DestinationAccount = destination.AccountNumber,
            Amount = request.Amount,
            SourceBalance = sourceBalance,
            DestinationBalance = destinationBalance,
            Message = "Transfer completed successfully"
        });
    }

    [HttpPost("transactions/airtime")]
    public async Task<IActionResult> BuyAirtime([FromBody] VifAirtimeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            return BadRequest(new { message = "PhoneNumber is required" });

        var result = await ApplyDebitAsync(
            request.AccountNumber,
            request.Amount,
            request.Currency,
            "Payment",
            $"Airtime purchase {request.Provider} for {request.PhoneNumber}");

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPost("transactions/mpesa")]
    public async Task<IActionResult> SendToMpesa([FromBody] VifMpesaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            return BadRequest(new { message = "PhoneNumber is required" });

        var result = await ApplyDebitAsync(
            request.AccountNumber,
            request.Amount,
            request.Currency,
            "Payment",
            $"M-PESA transfer to {request.PhoneNumber}");

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPost("transactions/cheque-deposit")]
    public async Task<IActionResult> DepositCheque([FromBody] VifChequeDepositRequest request)
    {
        if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.ChequeNumber))
            return BadRequest(new { message = "Amount and ChequeNumber are required" });

        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();

        var account = await GetAccountSnapshotAsync(connection, tx, request.AccountNumber, forUpdate: true);
        if (account is null)
            return NotFound(new { message = "Account not found" });

        var transactionId = await InsertTransactionAsync(
            connection,
            tx,
            account.Id,
            request.Amount,
            NormalizeCurrency(request.Currency, account.Currency),
            "ChequeDeposit",
            $"Cheque {request.ChequeNumber} from {request.DrawerBank}",
            "Pending",
            account.Balance);

        await tx.CommitAsync();

        return Ok(new
        {
            TransactionId = transactionId,
            Status = "Pending",
            Message = "Cheque deposit captured and awaiting clearing"
        });
    }

    [HttpPost("investments/shares/buy")]
    public async Task<IActionResult> BuyShares([FromBody] VifInvestmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.InstrumentCode))
            return BadRequest(new { message = "InstrumentCode is required" });

        var result = await ApplyDebitAsync(
            request.AccountNumber,
            request.Amount,
            request.Currency,
            "Investment",
            $"Share purchase {request.InstrumentCode} qty {request.Quantity}");

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPost("investments/treasury/buy")]
    public async Task<IActionResult> BuyTreasuryInstrument([FromBody] VifTreasuryPurchaseRequest request)
    {
        var instrumentType = string.IsNullOrWhiteSpace(request.InstrumentType) ? "TBill" : request.InstrumentType.Trim();
        if (!instrumentType.Equals("TBill", StringComparison.OrdinalIgnoreCase) &&
            !instrumentType.Equals("TBond", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "InstrumentType must be TBill or TBond" });
        }

        var result = await ApplyDebitAsync(
            request.AccountNumber,
            request.Amount,
            request.Currency,
            "Investment",
            $"{instrumentType.ToUpperInvariant()} purchase {request.InstrumentCode} tenor {request.TenorDays} days");

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPost("services/atm-card/lock")]
    public async Task<IActionResult> LockAtmCard([FromBody] VifServiceRequest request)
    {
        var service = await RecordServiceRequestAsync(
            request.AccountNumber,
            "CardService",
            $"ATM card lock request: {request.Reason}");

        return Ok(service);
    }

    [HttpPost("services/mobile-loan/block")]
    public async Task<IActionResult> BlockMobileLoan([FromBody] VifServiceRequest request)
    {
        var service = await RecordServiceRequestAsync(
            request.AccountNumber,
            "LoanService",
            $"Mobile loan block request: {request.Reason}");

        return Ok(service);
    }

    [HttpGet("transactions/statement/{accountNumber}")]
    public async Task<IActionResult> GetStatement(
        string accountNumber,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var safePage = Math.Max(1, pageNumber);
        var safeSize = Math.Clamp(pageSize, 1, 100);
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        var account = await GetAccountSnapshotAsync(connection, null, accountNumber, forUpdate: false);
        if (account is null)
            return NotFound(new { message = "Account not found" });

        const string countSql = @"
            SELECT COUNT(*)
            FROM ""Transactions""
            WHERE ""AccountId"" = @accountId
              AND ""CreatedAt"" BETWEEN @fromDate AND @toDate;";

        long totalRecords;
        await using (var countCmd = new NpgsqlCommand(countSql, connection))
        {
            countCmd.Parameters.AddWithValue("@accountId", account.Id);
            countCmd.Parameters.AddWithValue("@fromDate", fromDate);
            countCmd.Parameters.AddWithValue("@toDate", toDate);
            totalRecords = (long)(await countCmd.ExecuteScalarAsync() ?? 0L);
        }

        const string dataSql = @"
            SELECT ""TransactionReference"", ""TransactionType"", ""Amount"", ""Currency"", ""Status"", ""Description"", ""CreatedAt"", ""BalanceAfter""
            FROM ""Transactions""
            WHERE ""AccountId"" = @accountId
              AND ""CreatedAt"" BETWEEN @fromDate AND @toDate
            ORDER BY ""CreatedAt"" DESC
            LIMIT @limit OFFSET @offset;";

        var items = new List<object>();
        await using (var dataCmd = new NpgsqlCommand(dataSql, connection))
        {
            dataCmd.Parameters.AddWithValue("@accountId", account.Id);
            dataCmd.Parameters.AddWithValue("@fromDate", fromDate);
            dataCmd.Parameters.AddWithValue("@toDate", toDate);
            dataCmd.Parameters.AddWithValue("@limit", safeSize);
            dataCmd.Parameters.AddWithValue("@offset", (safePage - 1) * safeSize);

            await using var reader = await dataCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new
                {
                    TransactionReference = reader.GetString(0),
                    TransactionType = reader.GetString(1),
                    Amount = reader.GetDecimal(2),
                    Currency = reader.GetString(3),
                    Status = reader.GetString(4),
                    Description = reader.GetString(5),
                    CreatedAt = reader.GetDateTime(6),
                    BalanceAfter = reader.GetDecimal(7)
                });
            }
        }

        return Ok(new
        {
            AccountNumber = account.AccountNumber,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = safePage,
            PageSize = safeSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)safeSize),
            Transactions = items
        });
    }

    private async Task<VifAccountOperationResult> ApplyCreditAsync(string accountNumber, decimal amount, string? currency, string transactionType, string description)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();

        var account = await GetAccountSnapshotAsync(connection, tx, accountNumber, forUpdate: true);
        if (account is null)
            return VifAccountOperationResult.Fail("Account not found");

        var newBalance = account.Balance + amount;
        await UpdateAccountBalanceAsync(connection, tx, account.Id, newBalance);
        var txnId = await InsertTransactionAsync(connection, tx, account.Id, amount, NormalizeCurrency(currency, account.Currency), transactionType, description, "Completed", newBalance);

        await tx.CommitAsync();

        return VifAccountOperationResult.Success(txnId, newBalance, transactionType, account.AccountNumber);
    }

    private async Task<VifAccountOperationResult> ApplyDebitAsync(string accountNumber, decimal amount, string? currency, string transactionType, string description)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();

        var account = await GetAccountSnapshotAsync(connection, tx, accountNumber, forUpdate: true);
        if (account is null)
            return VifAccountOperationResult.Fail("Account not found");

        if (account.AvailableBalance < amount)
            return VifAccountOperationResult.Fail("Insufficient balance");

        var newBalance = account.Balance - amount;
        await UpdateAccountBalanceAsync(connection, tx, account.Id, newBalance);
        var txnId = await InsertTransactionAsync(connection, tx, account.Id, amount, NormalizeCurrency(currency, account.Currency), transactionType, description, "Completed", newBalance);

        await tx.CommitAsync();

        return VifAccountOperationResult.Success(txnId, newBalance, transactionType, account.AccountNumber);
    }

    private async Task<object> RecordServiceRequestAsync(string accountNumber, string requestType, string description)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var tx = await connection.BeginTransactionAsync();

        var account = await GetAccountSnapshotAsync(connection, tx, accountNumber, forUpdate: false);
        if (account is null)
            return new { IsSuccess = false, Error = "Account not found" };

        var txnId = await InsertTransactionAsync(connection, tx, account.Id, 0m, account.Currency, requestType, description, "Pending", account.Balance);
        await tx.CommitAsync();

        return new
        {
            IsSuccess = true,
            ServiceRequestId = txnId,
            RequestType = requestType,
            Status = "Pending",
            Message = "Service request captured successfully"
        };
    }

    private NpgsqlConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection is not configured");
        }

        return new NpgsqlConnection(connectionString);
    }

    private static async Task<bool> CustomerExistsAsync(NpgsqlConnection connection, Guid customerId)
    {
        const string sql = "SELECT COUNT(*) FROM \"Customers\" WHERE \"Id\" = @id;";
        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", customerId);
        var count = (long)(await cmd.ExecuteScalarAsync() ?? 0L);
        return count > 0;
    }

    private static async Task<string> GenerateUniqueAccountNumberAsync(NpgsqlConnection connection, NpgsqlTransaction tx)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var candidate = $"ACC{DateTime.UtcNow:yyMMddHHmmss}{Random.Shared.Next(10, 99)}";
            const string existsSql = "SELECT COUNT(*) FROM \"Accounts\" WHERE \"AccountNumber\" = @accountNumber;";

            await using var cmd = new NpgsqlCommand(existsSql, connection, tx);
            cmd.Parameters.AddWithValue("@accountNumber", candidate);
            var count = (long)(await cmd.ExecuteScalarAsync() ?? 0L);
            if (count == 0)
                return candidate;
        }

        throw new InvalidOperationException("Failed to generate unique account number");
    }

    private static async Task<AccountSnapshot?> GetAccountSnapshotAsync(NpgsqlConnection connection, NpgsqlTransaction? tx, string accountNumber, bool forUpdate)
    {
        var sql = @"
            SELECT ""Id"", ""AccountNumber"", ""Balance"", ""AvailableBalance"", ""Currency"", ""Status""
            FROM ""Accounts""
            WHERE ""AccountNumber"" = @accountNumber" + (forUpdate ? " FOR UPDATE" : string.Empty) + ";";

        await using var cmd = tx is null ? new NpgsqlCommand(sql, connection) : new NpgsqlCommand(sql, connection, tx);
        cmd.Parameters.AddWithValue("@accountNumber", accountNumber);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new AccountSnapshot(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetDecimal(2),
            reader.GetDecimal(3),
            reader.GetString(4),
            reader.GetString(5));
    }

    private static async Task UpdateAccountBalanceAsync(NpgsqlConnection connection, NpgsqlTransaction tx, Guid accountId, decimal newBalance)
    {
        const string sql = @"
            UPDATE ""Accounts""
            SET ""Balance"" = @balance,
                ""AvailableBalance"" = @availableBalance,
                ""UpdatedAt"" = @updatedAt
            WHERE ""Id"" = @id;";

        await using var cmd = new NpgsqlCommand(sql, connection, tx);
        cmd.Parameters.AddWithValue("@balance", newBalance);
        cmd.Parameters.AddWithValue("@availableBalance", newBalance);
        cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@id", accountId);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<Guid> InsertTransactionAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction tx,
        Guid accountId,
        decimal amount,
        string currency,
        string transactionType,
        string description,
        string status,
        decimal balanceAfter)
    {
        var transactionId = Guid.NewGuid();
        var reference = BuildTransactionReference("TX");
        var now = DateTime.UtcNow;

        const string sql = @"
            INSERT INTO ""Transactions""
            (""Id"", ""TransactionReference"", ""AccountId"", ""TransactionType"", ""Amount"", ""Currency"", ""Status"", ""Description"", ""CreatedAt"", ""ProcessedAt"", ""BalanceAfter"")
            VALUES
            (@id, @reference, @accountId, @transactionType, @amount, @currency, @status, @description, @createdAt, @processedAt, @balanceAfter);";

        await using var cmd = new NpgsqlCommand(sql, connection, tx);
        cmd.Parameters.AddWithValue("@id", transactionId);
        cmd.Parameters.AddWithValue("@reference", reference);
        cmd.Parameters.AddWithValue("@accountId", accountId);
        cmd.Parameters.AddWithValue("@transactionType", transactionType);
        cmd.Parameters.AddWithValue("@amount", amount);
        cmd.Parameters.AddWithValue("@currency", currency);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.Parameters.AddWithValue("@description", description);
        cmd.Parameters.AddWithValue("@createdAt", now);
        cmd.Parameters.AddWithValue("@processedAt", now);
        cmd.Parameters.AddWithValue("@balanceAfter", balanceAfter);
        await cmd.ExecuteNonQueryAsync();

        return transactionId;
    }

    private static string BuildCif(Guid customerId) => $"CIF-{customerId:N}";

    private static bool TryParseCif(string? cif, out Guid customerId)
    {
        customerId = Guid.Empty;
        if (string.IsNullOrWhiteSpace(cif))
            return false;

        const string prefix = "CIF-";
        if (!cif.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        var guidPart = cif[prefix.Length..];
        if (guidPart.Length == 32 && Guid.TryParseExact(guidPart, "N", out var parsed))
        {
            customerId = parsed;
            return true;
        }

        return false;
    }

    private static string BuildTransactionReference(string prefix)
        => $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";

    private static string NormalizeCurrency(string? inputCurrency, string fallback)
        => string.IsNullOrWhiteSpace(inputCurrency) ? fallback : inputCurrency.Trim().ToUpperInvariant();

    private sealed record AccountSnapshot(
        Guid Id,
        string AccountNumber,
        decimal Balance,
        decimal AvailableBalance,
        string Currency,
        string Status);
}

public record VifCustomerRegistrationRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string IdentificationNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public int RiskRating { get; init; } = 1;
}

public record VifAccountRegistrationRequest
{
    public string CifNumber { get; init; } = string.Empty;
    public string AccountType { get; init; } = "Savings";
    public string Currency { get; init; } = "KES";
    public decimal InitialDeposit { get; init; }
    public string BranchCode { get; init; } = "BR100001";
}

public record VifAmountRequest
{
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string? Description { get; init; }
}

public record VifTransferRequest
{
    public string FromAccountNumber { get; init; } = string.Empty;
    public string ToAccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
}

public record VifAirtimeRequest : VifAmountRequest
{
    public string PhoneNumber { get; init; } = string.Empty;
    public string Provider { get; init; } = "Safaricom";
}

public record VifMpesaRequest : VifAmountRequest
{
    public string PhoneNumber { get; init; } = string.Empty;
}

public record VifChequeDepositRequest : VifAmountRequest
{
    public string ChequeNumber { get; init; } = string.Empty;
    public string DrawerBank { get; init; } = string.Empty;
}

public record VifInvestmentRequest : VifAmountRequest
{
    public string InstrumentCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
}

public record VifTreasuryPurchaseRequest : VifInvestmentRequest
{
    public string InstrumentType { get; init; } = "TBill";
    public int TenorDays { get; init; } = 91;
}

public record VifServiceRequest
{
    public string AccountNumber { get; init; } = string.Empty;
    public string Reason { get; init; } = "Customer request";
}

public record VifAccountOperationResult
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public Guid? TransactionId { get; init; }
    public decimal NewBalance { get; init; }
    public string TransactionType { get; init; } = string.Empty;
    public string AccountNumber { get; init; } = string.Empty;

    public static VifAccountOperationResult Success(Guid transactionId, decimal newBalance, string type, string accountNumber) => new()
    {
        IsSuccess = true,
        TransactionId = transactionId,
        NewBalance = newBalance,
        TransactionType = type,
        AccountNumber = accountNumber
    };

    public static VifAccountOperationResult Fail(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}
