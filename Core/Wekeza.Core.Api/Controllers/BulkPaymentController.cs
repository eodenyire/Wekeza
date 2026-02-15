using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using Dapper;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Bulk Payment Processing Controller
/// Handles CSV/Excel file uploads, validation, and batch payment execution
/// </summary>
[ApiController]
[Route("api/public-sector/payments/bulk")]
[Authorize]
public class BulkPaymentController : ControllerBase
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<BulkPaymentController> _logger;

    public BulkPaymentController(IDbConnection dbConnection, ILogger<BulkPaymentController> logger)
    {
        _dbConnection = dbConnection;
        _logger = logger;
    }

    /// <summary>
    /// Upload bulk payment file (CSV format)
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadBulkPayments([FromForm] IFormFile file, [FromForm] Guid accountId)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, message = "Only CSV files are supported" });
            }

            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var batchId = Guid.NewGuid();
            var batchNumber = $"BULK-{DateTime.Now:yyyyMMdd}-{batchId.ToString().Substring(0, 8).ToUpper()}";

            // Parse CSV file
            var payments = new List<BulkPaymentItemDto>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                // Skip header line
                var header = await reader.ReadLineAsync();
                
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var fields = line.Split(',');
                    if (fields.Length >= 6)
                    {
                        payments.Add(new BulkPaymentItemDto
                        {
                            BeneficiaryName = fields[0].Trim(),
                            BeneficiaryAccount = fields[1].Trim(),
                            BeneficiaryBank = fields[2].Trim(),
                            Amount = decimal.TryParse(fields[3].Trim(), out var amount) ? amount : 0,
                            Narration = fields[4].Trim(),
                            Reference = fields[5].Trim()
                        });
                    }
                }
            }

            if (payments.Count == 0)
            {
                return BadRequest(new { success = false, message = "No valid payment records found in file" });
            }

            // Calculate totals
            var totalAmount = payments.Sum(p => p.Amount);
            var totalCount = payments.Count;

            // Verify account balance
            var accountQuery = @"
                SELECT ""BalanceAmount"" 
                FROM ""Accounts"" 
                WHERE ""Id"" = @AccountId AND ""Status"" = 'Active'";
            
            var balance = await _dbConnection.QueryFirstOrDefaultAsync<decimal>(accountQuery, new { AccountId = accountId });
            
            if (balance < totalAmount)
            {
                return BadRequest(new { 
                    success = false, 
                    message = $"Insufficient balance. Required: KES {totalAmount:N2}, Available: KES {balance:N2}" 
                });
            }

            // Create batch record
            var batchQuery = @"
                INSERT INTO ""BulkPaymentBatches""
                (""Id"", ""BatchNumber"", ""AccountId"", ""UploadedBy"", ""FileName"", 
                 ""TotalAmount"", ""TotalCount"", ""Status"")
                VALUES
                (@Id, @BatchNumber, @AccountId, @UploadedBy, @FileName, 
                 @TotalAmount, @TotalCount, @Status)";

            await _dbConnection.ExecuteAsync(batchQuery, new
            {
                Id = batchId,
                BatchNumber = batchNumber,
                AccountId = accountId,
                UploadedBy = userId,
                FileName = file.FileName,
                TotalAmount = totalAmount,
                TotalCount = totalCount,
                Status = "UPLOADED"
            });

            // Create payment items
            var itemQuery = @"
                INSERT INTO ""BulkPaymentItems""
                (""Id"", ""BatchId"", ""ItemNumber"", ""BeneficiaryName"", ""BeneficiaryAccount"", 
                 ""BeneficiaryBank"", ""Amount"", ""Narration"", ""Reference"", ""Status"")
                VALUES
                (@Id, @BatchId, @ItemNumber, @BeneficiaryName, @BeneficiaryAccount, 
                 @BeneficiaryBank, @Amount, @Narration, @Reference, @Status)";

            for (int i = 0; i < payments.Count; i++)
            {
                await _dbConnection.ExecuteAsync(itemQuery, new
                {
                    Id = Guid.NewGuid(),
                    BatchId = batchId,
                    ItemNumber = i + 1,
                    payments[i].BeneficiaryName,
                    payments[i].BeneficiaryAccount,
                    payments[i].BeneficiaryBank,
                    payments[i].Amount,
                    payments[i].Narration,
                    payments[i].Reference,
                    Status = "PENDING"
                });
            }

            // Log audit
            await LogAudit(userId, "BULK_PAYMENT_UPLOADED", "BulkPaymentBatch", batchId, null, batchNumber);

            return Ok(new
            {
                success = true,
                message = "Bulk payment file uploaded successfully",
                data = new
                {
                    batchId,
                    batchNumber,
                    totalCount,
                    totalAmount,
                    status = "UPLOADED"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading bulk payment file");
            return StatusCode(500, new { success = false, message = $"Error uploading file: {ex.Message}" });
        }
    }

    /// <summary>
    /// Validate bulk payment batch
    /// </summary>
    [HttpPost("{batchId}/validate")]
    public async Task<IActionResult> ValidateBulkPayments(Guid batchId)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Get batch
            var batchQuery = @"
                SELECT ""Status"" as status
                FROM ""BulkPaymentBatches""
                WHERE ""Id"" = @BatchId";

            var batch = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(batchQuery, new { BatchId = batchId });

            if (batch == null)
            {
                return NotFound(new { success = false, message = "Batch not found" });
            }

            // Get all items
            var itemsQuery = @"
                SELECT ""Id"" as id, ""BeneficiaryAccount"" as beneficiaryaccount, ""Amount"" as amount
                FROM ""BulkPaymentItems""
                WHERE ""BatchId"" = @BatchId";

            var items = await _dbConnection.QueryAsync<dynamic>(itemsQuery, new { BatchId = batchId });

            var validationErrors = new List<string>();
            var validCount = 0;
            var invalidCount = 0;

            // Validate each item
            foreach (var item in items)
            {
                var errors = new List<string>();

                // Validate account number format
                if (string.IsNullOrWhiteSpace(item.beneficiaryaccount) || item.beneficiaryaccount.Length < 10)
                {
                    errors.Add("Invalid account number format");
                }

                // Validate amount
                if (item.amount <= 0)
                {
                    errors.Add("Amount must be greater than zero");
                }

                if (errors.Any())
                {
                    invalidCount++;
                    var updateQuery = @"
                        UPDATE ""BulkPaymentItems""
                        SET ""Status"" = 'INVALID', ""ErrorMessage"" = @ErrorMessage
                        WHERE ""Id"" = @Id";
                    
                    await _dbConnection.ExecuteAsync(updateQuery, new
                    {
                        Id = item.id,
                        ErrorMessage = string.Join("; ", errors)
                    });
                }
                else
                {
                    validCount++;
                    var updateQuery = @"
                        UPDATE ""BulkPaymentItems""
                        SET ""Status"" = 'VALIDATED'
                        WHERE ""Id"" = @Id";
                    
                    await _dbConnection.ExecuteAsync(updateQuery, new { Id = item.id });
                }
            }

            // Update batch status
            var batchUpdateQuery = @"
                UPDATE ""BulkPaymentBatches""
                SET ""Status"" = @Status, ""ValidatedAt"" = CURRENT_TIMESTAMP
                WHERE ""Id"" = @BatchId";

            await _dbConnection.ExecuteAsync(batchUpdateQuery, new
            {
                BatchId = batchId,
                Status = invalidCount > 0 ? "VALIDATION_FAILED" : "VALIDATED"
            });

            // Log audit
            await LogAudit(userId, "BULK_PAYMENT_VALIDATED", "BulkPaymentBatch", batchId, 
                null, $"Valid: {validCount}, Invalid: {invalidCount}");

            return Ok(new
            {
                success = true,
                message = "Validation completed",
                data = new
                {
                    batchId,
                    totalItems = items.Count(),
                    validCount,
                    invalidCount,
                    status = invalidCount > 0 ? "VALIDATION_FAILED" : "VALIDATED"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating bulk payments");
            return StatusCode(500, new { success = false, message = $"Error validating: {ex.Message}" });
        }
    }

    /// <summary>
    /// Execute bulk payment batch
    /// </summary>
    [HttpPost("{batchId}/execute")]
    public async Task<IActionResult> ExecuteBulkPayments(Guid batchId)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Get batch
            var batchQuery = @"
                SELECT ""Status"" as status, ""AccountId"" as accountid
                FROM ""BulkPaymentBatches""
                WHERE ""Id"" = @BatchId";

            var batch = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(batchQuery, new { BatchId = batchId });

            if (batch == null)
            {
                return NotFound(new { success = false, message = "Batch not found" });
            }

            if (batch.status != "VALIDATED")
            {
                return BadRequest(new { success = false, message = $"Batch must be validated before execution. Current status: {batch.status}" });
            }

            // Get validated items
            var itemsQuery = @"
                SELECT ""Id"" as id, ""Amount"" as amount, ""BeneficiaryName"" as beneficiaryname
                FROM ""BulkPaymentItems""
                WHERE ""BatchId"" = @BatchId AND ""Status"" = 'VALIDATED'";

            var items = await _dbConnection.QueryAsync<dynamic>(itemsQuery, new { BatchId = batchId });

            var successCount = 0;
            var failedCount = 0;

            // Process each payment
            foreach (var item in items)
            {
                try
                {
                    // Simulate payment processing
                    // In production, this would integrate with payment gateway/IFMIS
                    
                    var updateQuery = @"
                        UPDATE ""BulkPaymentItems""
                        SET ""Status"" = 'SUCCESS', ""ProcessedAt"" = CURRENT_TIMESTAMP
                        WHERE ""Id"" = @Id";
                    
                    await _dbConnection.ExecuteAsync(updateQuery, new { Id = item.id });
                    successCount++;
                }
                catch (Exception ex)
                {
                    var updateQuery = @"
                        UPDATE ""BulkPaymentItems""
                        SET ""Status"" = 'FAILED', ""ErrorMessage"" = @ErrorMessage
                        WHERE ""Id"" = @Id";
                    
                    await _dbConnection.ExecuteAsync(updateQuery, new
                    {
                        Id = item.id,
                        ErrorMessage = ex.Message
                    });
                    failedCount++;
                }
            }

            // Update batch status
            var batchUpdateQuery = @"
                UPDATE ""BulkPaymentBatches""
                SET ""Status"" = @Status, ""ProcessedAt"" = CURRENT_TIMESTAMP
                WHERE ""Id"" = @BatchId";

            await _dbConnection.ExecuteAsync(batchUpdateQuery, new
            {
                BatchId = batchId,
                Status = failedCount > 0 ? "PARTIALLY_COMPLETED" : "COMPLETED"
            });

            // Log audit
            await LogAudit(userId, "BULK_PAYMENT_EXECUTED", "BulkPaymentBatch", batchId, 
                null, $"Success: {successCount}, Failed: {failedCount}");

            return Ok(new
            {
                success = true,
                message = "Bulk payment execution completed",
                data = new
                {
                    batchId,
                    totalProcessed = items.Count(),
                    successCount,
                    failedCount,
                    status = failedCount > 0 ? "PARTIALLY_COMPLETED" : "COMPLETED"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing bulk payments");
            return StatusCode(500, new { success = false, message = $"Error executing: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get batch status and details
    /// </summary>
    [HttpGet("{batchId}")]
    public async Task<IActionResult> GetBatchStatus(Guid batchId)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var batchQuery = @"
                SELECT 
                    b.""Id"" as id,
                    b.""BatchNumber"" as batchnumber,
                    b.""FileName"" as filename,
                    b.""TotalAmount"" as totalamount,
                    b.""TotalCount"" as totalcount,
                    b.""Status"" as status,
                    b.""UploadedAt"" as uploadedat,
                    b.""ValidatedAt"" as validatedat,
                    b.""ProcessedAt"" as processedat,
                    u.""Username"" as uploadedby
                FROM ""BulkPaymentBatches"" b
                JOIN ""Users"" u ON b.""UploadedBy"" = u.""Id""
                WHERE b.""Id"" = @BatchId";

            var batch = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(batchQuery, new { BatchId = batchId });

            if (batch == null)
            {
                return NotFound(new { success = false, message = "Batch not found" });
            }

            // Get items summary
            var itemsQuery = @"
                SELECT 
                    ""Status"" as status,
                    COUNT(*) as count,
                    SUM(""Amount"") as totalamount
                FROM ""BulkPaymentItems""
                WHERE ""BatchId"" = @BatchId
                GROUP BY ""Status""";

            var itemsSummary = await _dbConnection.QueryAsync<dynamic>(itemsQuery, new { BatchId = batchId });

            return Ok(new
            {
                success = true,
                data = new
                {
                    batch,
                    itemsSummary
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting batch status");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get all batches
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllBatches([FromQuery] string? status = null)
    {
        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            var query = @"
                SELECT 
                    b.""Id"" as id,
                    b.""BatchNumber"" as batchnumber,
                    b.""FileName"" as filename,
                    b.""TotalAmount"" as totalamount,
                    b.""TotalCount"" as totalcount,
                    b.""Status"" as status,
                    b.""UploadedAt"" as uploadedat,
                    u.""Username"" as uploadedby
                FROM ""BulkPaymentBatches"" b
                JOIN ""Users"" u ON b.""UploadedBy"" = u.""Id""
                WHERE (@Status IS NULL OR b.""Status"" = @Status)
                ORDER BY b.""UploadedAt"" DESC";

            var batches = await _dbConnection.QueryAsync<dynamic>(query, new { Status = status });

            return Ok(new { success = true, data = batches });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting batches");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    private async Task LogAudit(Guid userId, string action, string entityType, Guid entityId, string? oldValue, string? newValue)
    {
        var auditQuery = @"
            INSERT INTO ""AuditTrail""
            (""Id"", ""UserId"", ""Action"", ""EntityType"", ""EntityId"", ""OldValue"", ""NewValue"")
            VALUES
            (@Id, @UserId, @Action, @EntityType, @EntityId, @OldValue, @NewValue)";

        await _dbConnection.ExecuteAsync(auditQuery, new
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValue = oldValue,
            NewValue = newValue
        });
    }
}

// DTOs
public class BulkPaymentItemDto
{
    public string BeneficiaryName { get; set; } = string.Empty;
    public string BeneficiaryAccount { get; set; } = string.Empty;
    public string BeneficiaryBank { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}
