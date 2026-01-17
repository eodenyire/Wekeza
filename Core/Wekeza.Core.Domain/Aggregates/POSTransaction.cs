using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// POS Transaction Aggregate - Handles all Point-of-Sale transactions
/// Supports purchase, refund, pre-authorization, and completion transactions
/// </summary>
public class POSTransaction : AggregateRoot
{
    public string MerchantId { get; private set; }
    public string MerchantName { get; private set; }
    public string MerchantCategory { get; private set; }
    public string TerminalId { get; private set; }
    public Guid CardId { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CardNumber { get; private set; } // Masked for security
    public POSTransactionType TransactionType { get; private set; }
    public Money Amount { get; private set; }
    public Money? TipAmount { get; private set; }
    public Money TotalAmount { get; private set; }
    public POSTransactionStatus Status { get; private set; }
    public string? ResponseCode { get; private set; }
    public string? ResponseMessage { get; private set; }
    public DateTime TransactionDateTime { get; private set; }
    public string? AuthorizationCode { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public Money AccountBalanceBefore { get; private set; }
    public Money AccountBalanceAfter { get; private set; }
    public string? FailureReason { get; private set; }
    
    // POS Specific Details
    public string? ReceiptNumber { get; private set; }
    public bool IsContactless { get; private set; }
    public bool IsPINVerified { get; private set; }
    public bool IsSignatureRequired { get; private set; }
    public string? BatchNumber { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public Currency TransactionCurrency { get; private set; }
    public Currency? BillingCurrency { get; private set; }
    public decimal? ExchangeRate { get; private set; }
    
    // Settlement Details
    public bool IsSettled { get; private set; }
    public DateTime? SettledDate { get; private set; }
    public string? SettlementBatchId { get; private set; }
    public Money? InterchangeFee { get; private set; }
    public Money? MerchantFee { get; private set; }
    public Money? NetworkFee { get; private set; }
    
    // Reversal and Refund
    public bool IsReversed { get; private set; }
    public DateTime? ReversedDate { get; private set; }
    public string? ReversalReason { get; private set; }
    public Guid? ReversalTransactionId { get; private set; }
    public bool IsRefunded { get; private set; }
    public DateTime? RefundedDate { get; private set; }
    public Money? RefundedAmount { get; private set; }
    public Guid? RefundTransactionId { get; private set; }
    
    // Security and Fraud
    public bool IsSuspicious { get; private set; }
    public string? SuspiciousReason { get; private set; }
    public string? FraudScore { get; private set; }
    public string? CVVResult { get; private set; }
    public string? AVSResult { get; private set; }

    private POSTransaction() : base(Guid.NewGuid()) { }

    public static POSTransaction CreatePurchase(
        string merchantId,
        string merchantName,
        string merchantCategory,
        string terminalId,
        Guid cardId,
        Guid accountId,
        Guid customerId,
        string maskedCardNumber,
        Money amount,
        Money? tipAmount,
        Money accountBalance,
        bool isContactless = false)
    {
        var totalAmount = tipAmount != null ? amount + tipAmount : amount;
        
        var transaction = new POSTransaction
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            MerchantName = merchantName,
            MerchantCategory = merchantCategory,
            TerminalId = terminalId,
            CardId = cardId,
            AccountId = accountId,
            CustomerId = customerId,
            CardNumber = maskedCardNumber,
            TransactionType = POSTransactionType.Purchase,
            Amount = amount,
            TipAmount = tipAmount,
            TotalAmount = totalAmount,
            Status = POSTransactionStatus.Initiated,
            TransactionDateTime = DateTime.UtcNow,
            ReferenceNumber = GenerateReferenceNumber(),
            AccountBalanceBefore = accountBalance,
            IsContactless = isContactless,
            TransactionCurrency = amount.Currency,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "POS_SYSTEM"
        };

        transaction.AddDomainEvent(new POSTransactionInitiatedDomainEvent(
            transaction.Id, cardId, accountId, merchantId, POSTransactionType.Purchase, totalAmount));

        return transaction;
    }

    public static POSTransaction CreateRefund(
        string merchantId,
        string merchantName,
        string merchantCategory,
        string terminalId,
        Guid cardId,
        Guid accountId,
        Guid customerId,
        string maskedCardNumber,
        Money refundAmount,
        Money accountBalance,
        Guid originalTransactionId)
    {
        var transaction = new POSTransaction
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            MerchantName = merchantName,
            MerchantCategory = merchantCategory,
            TerminalId = terminalId,
            CardId = cardId,
            AccountId = accountId,
            CustomerId = customerId,
            CardNumber = maskedCardNumber,
            TransactionType = POSTransactionType.Refund,
            Amount = refundAmount,
            TotalAmount = refundAmount,
            Status = POSTransactionStatus.Initiated,
            TransactionDateTime = DateTime.UtcNow,
            ReferenceNumber = GenerateReferenceNumber(),
            AccountBalanceBefore = accountBalance,
            TransactionCurrency = refundAmount.Currency,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "POS_SYSTEM"
        };

        transaction.AddDomainEvent(new POSTransactionInitiatedDomainEvent(
            transaction.Id, cardId, accountId, merchantId, POSTransactionType.Refund, refundAmount));

        return transaction;
    }

    public void Authorize(string authorizationCode, Money newAccountBalance)
    {
        if (Status != POSTransactionStatus.Initiated)
            throw new DomainException($"Cannot authorize transaction in {Status} status");

        Status = POSTransactionStatus.Authorized;
        AuthorizationCode = authorizationCode;
        AccountBalanceAfter = newAccountBalance;
        ResponseCode = "00"; // Success
        ResponseMessage = "Transaction Approved";

        AddDomainEvent(new POSTransactionAuthorizedDomainEvent(Id, CardId, AccountId, MerchantId, TotalAmount));
    }

    public void Complete(string receiptNumber, string? batchNumber = null, string? invoiceNumber = null)
    {
        if (Status != POSTransactionStatus.Authorized)
            throw new DomainException($"Cannot complete transaction in {Status} status");

        Status = POSTransactionStatus.Completed;
        ReceiptNumber = receiptNumber;
        BatchNumber = batchNumber;
        InvoiceNumber = invoiceNumber;

        AddDomainEvent(new POSTransactionCompletedDomainEvent(
            Id, CardId, AccountId, MerchantId, TransactionType, TotalAmount, AccountBalanceAfter));
    }

    public void Decline(string responseCode, string responseMessage, string? failureReason = null)
    {
        if (Status == POSTransactionStatus.Completed)
            throw new DomainException("Cannot decline a completed transaction");

        Status = POSTransactionStatus.Declined;
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        FailureReason = failureReason;

        AddDomainEvent(new POSTransactionDeclinedDomainEvent(
            Id, CardId, AccountId, MerchantId, responseCode, responseMessage));
    }

    public void Fail(string failureReason)
    {
        if (Status == POSTransactionStatus.Completed)
            throw new DomainException("Cannot fail a completed transaction");

        Status = POSTransactionStatus.Failed;
        FailureReason = failureReason;
        ResponseCode = "96"; // System Error
        ResponseMessage = "Transaction Failed";

        AddDomainEvent(new POSTransactionFailedDomainEvent(Id, CardId, AccountId, MerchantId, failureReason));
    }

    public void Reverse(string reversalReason, Guid reversalTransactionId)
    {
        if (Status != POSTransactionStatus.Completed)
            throw new DomainException("Can only reverse completed transactions");

        if (IsReversed)
            throw new DomainException("Transaction is already reversed");

        IsReversed = true;
        ReversedDate = DateTime.UtcNow;
        ReversalReason = reversalReason;
        ReversalTransactionId = reversalTransactionId;

        AddDomainEvent(new POSTransactionReversedDomainEvent(
            Id, CardId, AccountId, MerchantId, TotalAmount, reversalReason));
    }

    public void ProcessRefund(Money refundAmount, Guid refundTransactionId)
    {
        if (Status != POSTransactionStatus.Completed)
            throw new DomainException("Can only refund completed transactions");

        if (TransactionType != POSTransactionType.Purchase)
            throw new DomainException("Can only refund purchase transactions");

        if (refundAmount > TotalAmount)
            throw new DomainException("Refund amount cannot exceed original transaction amount");

        IsRefunded = true;
        RefundedDate = DateTime.UtcNow;
        RefundedAmount = refundAmount;
        RefundTransactionId = refundTransactionId;

        AddDomainEvent(new POSTransactionRefundedDomainEvent(
            Id, CardId, AccountId, MerchantId, refundAmount));
    }

    public void Settle(string settlementBatchId, Money? interchangeFee = null, Money? merchantFee = null, Money? networkFee = null)
    {
        if (Status != POSTransactionStatus.Completed)
            throw new DomainException("Can only settle completed transactions");

        if (IsSettled)
            throw new DomainException("Transaction is already settled");

        IsSettled = true;
        SettledDate = DateTime.UtcNow;
        SettlementBatchId = settlementBatchId;
        InterchangeFee = interchangeFee;
        MerchantFee = merchantFee;
        NetworkFee = networkFee;

        AddDomainEvent(new POSTransactionSettledDomainEvent(
            Id, CardId, AccountId, MerchantId, TotalAmount, settlementBatchId));
    }

    public void SetPINVerificationResult(bool isVerified)
    {
        IsPINVerified = isVerified;
        
        if (!isVerified)
        {
            AddDomainEvent(new POSPINVerificationFailedDomainEvent(Id, CardId, CustomerId, MerchantId));
        }
    }

    public void SetSignatureRequired(bool required)
    {
        IsSignatureRequired = required;
    }

    public void SetCVVResult(string cvvResult)
    {
        CVVResult = cvvResult;
    }

    public void SetAVSResult(string avsResult)
    {
        AVSResult = avsResult;
    }

    public void MarkAsSuspicious(string reason, string fraudScore)
    {
        IsSuspicious = true;
        SuspiciousReason = reason;
        FraudScore = fraudScore;

        AddDomainEvent(new POSTransactionMarkedSuspiciousDomainEvent(
            Id, CardId, AccountId, MerchantId, reason, fraudScore));
    }

    public void SetExchangeRate(Currency billingCurrency, decimal exchangeRate)
    {
        BillingCurrency = billingCurrency;
        ExchangeRate = exchangeRate;
    }

    private static string GenerateReferenceNumber()
    {
        return $"POS{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }
}

// Enums for POS Transaction Management
public enum POSTransactionType
{
    Purchase = 1,
    Refund = 2,
    PreAuthorization = 3,
    Completion = 4,
    Void = 5,
    CashAdvance = 6
}

public enum POSTransactionStatus
{
    Initiated = 1,
    Authorized = 2,
    Completed = 3,
    Declined = 4,
    Failed = 5,
    Reversed = 6,
    Refunded = 7,
    Settled = 8
}