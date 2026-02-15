using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Payments.Commands.ProcessPayment;
using Wekeza.Core.Application.Features.Payments.Queries.GetPaymentHistory;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Payments Controller - Enterprise payment processing
/// Handles internal transfers, external payments, and payment history
/// Inspired by Finacle Payment Hub and T24 Payment Processing
/// </summary>
public class PaymentsController : BaseApiController
{
    /// <summary>
    /// Process Internal Transfer - Account to account within the bank
    /// Real-time processing with automatic GL posting
    /// </summary>
    [HttpPost("internal-transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProcessPaymentResult>> ProcessInternalTransfer(ProcessInternalTransferRequest request)
    {
        var command = new ProcessPaymentCommand
        {
            Type = PaymentType.InternalTransfer,
            Channel = PaymentChannel.Internal,
            FromAccountId = request.FromAccountId,
            FromAccountNumber = request.FromAccountNumber,
            ToAccountId = request.ToAccountId,
            ToAccountNumber = request.ToAccountNumber,
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            CustomerReference = request.CustomerReference,
            Priority = request.Priority,
            FeeBearer = request.FeeBearer,
            ProcessImmediately = true
        };

        var result = await Mediator.Send(command);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Process External Payment - Payment to external bank account
    /// Supports EFT, RTGS, and SWIFT channels
    /// </summary>
    [HttpPost("external-payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProcessPaymentResult>> ProcessExternalPayment(ProcessExternalPaymentRequest request)
    {
        var command = new ProcessPaymentCommand
        {
            Type = PaymentType.ExternalTransfer,
            Channel = request.Channel,
            FromAccountId = request.FromAccountId,
            FromAccountNumber = request.FromAccountNumber,
            BeneficiaryName = request.BeneficiaryName,
            BeneficiaryAccountNumber = request.BeneficiaryAccountNumber,
            BeneficiaryBank = request.BeneficiaryBank,
            BeneficiaryBankCode = request.BeneficiaryBankCode,
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            CustomerReference = request.CustomerReference,
            Priority = request.Priority,
            FeeBearer = request.FeeBearer,
            ValueDate = request.ValueDate,
            ProcessImmediately = request.ProcessImmediately
        };

        var result = await Mediator.Send(command);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get Payment History - Retrieve payment transactions
    /// Supports filtering by account, customer, date range, and status
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPaymentHistoryResult>> GetPaymentHistory(
        [FromQuery] Guid? accountId,
        [FromQuery] string? accountNumber,
        [FromQuery] Guid? customerId,
        [FromQuery] PaymentType? type,
        [FromQuery] PaymentStatus? status,
        [FromQuery] PaymentChannel? channel,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int pageSize = 50,
        [FromQuery] int pageNumber = 1,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetPaymentHistoryQuery
        {
            AccountId = accountId,
            AccountNumber = accountNumber,
            CustomerId = customerId,
            Type = type,
            Status = status,
            Channel = channel,
            FromDate = fromDate,
            ToDate = toDate,
            PageSize = Math.Min(pageSize, 100), // Limit page size
            PageNumber = Math.Max(pageNumber, 1),
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get Payment Status - Check status of a specific payment
    /// </summary>
    [HttpGet("{paymentReference}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentStatusDto>> GetPaymentStatus(string paymentReference)
    {
        var query = new GetPaymentHistoryQuery
        {
            SearchTerm = paymentReference,
            PageSize = 1
        };

        var result = await Mediator.Send(query);
        var payment = result.Payments.FirstOrDefault();

        if (payment == null)
            return NotFound($"Payment with reference {paymentReference} not found");

        return Ok(new PaymentStatusDto
        {
            PaymentReference = payment.PaymentReference,
            Status = payment.Status,
            Amount = payment.Amount,
            Currency = payment.Currency,
            ProcessedDate = payment.ProcessedDate,
            SettledDate = payment.SettledDate,
            FailureReason = payment.FailureReason,
            ExternalReference = payment.ExternalReference
        });
    }

    /// <summary>
    /// Get Pending Approvals - Retrieve payments awaiting approval
    /// </summary>
    [HttpGet("pending-approvals")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPaymentHistoryResult>> GetPendingApprovals()
    {
        var query = new GetPaymentHistoryQuery
        {
            Status = PaymentStatus.Pending,
            PageSize = 100
        };

        var result = await Mediator.Send(query);
        
        // Filter only those requiring approval
        var pendingApprovals = result.Payments.Where(p => p.RequiresApproval).ToList();
        
        return Ok(new GetPaymentHistoryResult
        {
            Payments = pendingApprovals,
            TotalCount = pendingApprovals.Count,
            PageSize = result.PageSize,
            PageNumber = result.PageNumber,
            TotalPages = 1,
            TotalAmount = pendingApprovals.Sum(p => p.Amount)
        });
    }

    /// <summary>
    /// Get Failed Payments - Retrieve failed payments for retry
    /// </summary>
    [HttpGet("failed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPaymentHistoryResult>> GetFailedPayments()
    {
        var query = new GetPaymentHistoryQuery
        {
            Status = PaymentStatus.Failed,
            PageSize = 100
        };

        var result = await Mediator.Send(query);
        return Ok(result);
    }
}

// Request DTOs
public record ProcessInternalTransferRequest
{
    public Guid? FromAccountId { get; init; }
    public string? FromAccountNumber { get; init; }
    public Guid? ToAccountId { get; init; }
    public string? ToAccountNumber { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string Description { get; init; } = string.Empty;
    public string? CustomerReference { get; init; }
    public PaymentPriority Priority { get; init; } = PaymentPriority.Normal;
    public FeeBearer FeeBearer { get; init; } = FeeBearer.Sender;
}

public record ProcessExternalPaymentRequest
{
    public Guid? FromAccountId { get; init; }
    public string? FromAccountNumber { get; init; }
    public string BeneficiaryName { get; init; } = string.Empty;
    public string BeneficiaryAccountNumber { get; init; } = string.Empty;
    public string BeneficiaryBank { get; init; } = string.Empty;
    public string? BeneficiaryBankCode { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    public string Description { get; init; } = string.Empty;
    public string? CustomerReference { get; init; }
    public PaymentChannel Channel { get; init; } = PaymentChannel.Eft;
    public PaymentPriority Priority { get; init; } = PaymentPriority.Normal;
    public FeeBearer FeeBearer { get; init; } = FeeBearer.Sender;
    public DateTime? ValueDate { get; init; }
    public bool ProcessImmediately { get; init; } = true;
}

// Response DTOs
public record PaymentStatusDto
{
    public string PaymentReference { get; init; } = string.Empty;
    public PaymentStatus Status { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime? ProcessedDate { get; init; }
    public DateTime? SettledDate { get; init; }
    public string? FailureReason { get; init; }
    public string? ExternalReference { get; init; }
}