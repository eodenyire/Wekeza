using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// SWIFT Message aggregate - Handles international payment messaging
/// Supports MT103, MT202, MT700 and other SWIFT message types
/// </summary>
public class SWIFTMessage : AggregateRoot<Guid>
{
    public string MessageReference { get; private set; }
    public string SWIFTReference { get; private set; }
    public SWIFTMessageType MessageType { get; private set; }
    public string MessageText { get; private set; }
    public SWIFTDirection Direction { get; private set; }
    public SWIFTStatus Status { get; private set; }
    public string SenderBIC { get; private set; }
    public string ReceiverBIC { get; private set; }
    public Money? Amount { get; private set; }
    public string? Currency { get; private set; }
    public DateTime ValueDate { get; private set; }
    public string? OrderingCustomer { get; private set; }
    public string? BeneficiaryCustomer { get; private set; }
    public string? RemittanceInfo { get; private set; }
    public string? ChargeBearer { get; private set; } // OUR, BEN, SHA
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? ProcessedBy { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorDescription { get; private set; }
    public string? AcknowledgmentReference { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? NextRetryAt { get; private set; }

    private readonly List<SWIFTMessageField> _fields = new();
    public IReadOnlyList<SWIFTMessageField> Fields => _fields.AsReadOnly();

    private SWIFTMessage() { } // EF Core

    public SWIFTMessage(
        Guid id,
        string messageReference,
        SWIFTMessageType messageType,
        SWIFTDirection direction,
        string senderBIC,
        string receiverBIC,
        DateTime valueDate,
        string createdBy)
    {
        Id = id;
        MessageReference = messageReference;
        MessageType = messageType;
        Direction = direction;
        SenderBIC = senderBIC;
        ReceiverBIC = receiverBIC;
        ValueDate = valueDate;
        CreatedBy = createdBy;
        Status = SWIFTStatus.Draft;
        MessageText = string.Empty;
        CreatedAt = DateTime.UtcNow;
        RetryCount = 0;

        // Generate SWIFT reference
        SWIFTReference = GenerateSWIFTReference();

        AddDomainEvent(new SWIFTMessageCreatedDomainEvent(Id, MessageReference, MessageType, Direction));
    }

    private string GenerateSWIFTReference()
    {
        // SWIFT reference format: YYYYMMDDXXXXXX (Date + 6 digit sequence)
        var datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var sequence = (MessageReference.GetHashCode() % 1000000).ToString("D6");
        return $"{datePrefix}{sequence}";
    }

    public void AddField(string fieldTag, string fieldValue, bool isMandatory = false)
    {
        if (Status != SWIFTStatus.Draft)
            throw new InvalidOperationException("Cannot modify fields after message is finalized");

        var field = new SWIFTMessageField(
            Guid.NewGuid(),
            Id,
            fieldTag,
            fieldValue,
            isMandatory,
            DateTime.UtcNow);

        _fields.Add(field);

        // Update message properties based on field
        UpdateMessagePropertiesFromField(fieldTag, fieldValue);
    }

    private void UpdateMessagePropertiesFromField(string fieldTag, string fieldValue)
    {
        switch (fieldTag)
        {
            case "32A": // Value Date, Currency, Amount
                ParseField32A(fieldValue);
                break;
            case "50K": // Ordering Customer
                OrderingCustomer = fieldValue;
                break;
            case "59": // Beneficiary Customer
                BeneficiaryCustomer = fieldValue;
                break;
            case "70": // Remittance Information
                RemittanceInfo = fieldValue;
                break;
            case "71A": // Details of Charges
                ChargeBearer = fieldValue;
                break;
        }
    }

    private void ParseField32A(string fieldValue)
    {
        // Format: YYMMDDCCCNNNNNNNNN (Date + Currency + Amount)
        if (fieldValue.Length >= 9)
        {
            var currencyCode = fieldValue.Substring(6, 3);
            var amountString = fieldValue.Substring(9);
            
            if (decimal.TryParse(amountString, out var amount))
            {
                Currency = currencyCode;
                Amount = new Money(amount, new Currency(currencyCode));
            }
        }
    }

    public void BuildMessage()
    {
        if (Status != SWIFTStatus.Draft)
            throw new InvalidOperationException("Message has already been built");

        var messageBuilder = new System.Text.StringBuilder();
        
        // Add message header
        messageBuilder.AppendLine($"{{1:F{(int)MessageType:D3}{SenderBIC}}}");
        messageBuilder.AppendLine($"{{2:I{(int)MessageType:D3}{ReceiverBIC}}}");
        messageBuilder.AppendLine($"{{3:{{108:{SWIFTReference}}}}}");
        
        // Add message body
        messageBuilder.AppendLine("{4:");
        
        foreach (var field in _fields.OrderBy(f => f.FieldTag))
        {
            messageBuilder.AppendLine($":{field.FieldTag}:{field.FieldValue}");
        }
        
        messageBuilder.AppendLine("-}");

        MessageText = messageBuilder.ToString();
        Status = SWIFTStatus.Ready;

        AddDomainEvent(new SWIFTMessageBuiltDomainEvent(Id, SWIFTReference, MessageType));
    }

    public void ValidateMessage()
    {
        if (Status != SWIFTStatus.Ready)
            throw new InvalidOperationException("Message must be built before validation");

        var validationErrors = new List<string>();

        // Validate mandatory fields based on message type
        ValidateMandatoryFields(validationErrors);

        // Validate BIC codes
        if (!IsValidBIC(SenderBIC))
            validationErrors.Add("Invalid sender BIC code");

        if (!IsValidBIC(ReceiverBIC))
            validationErrors.Add("Invalid receiver BIC code");

        // Validate amount for payment messages
        if (IsPaymentMessage() && (Amount == null || Amount.Amount <= 0))
            validationErrors.Add("Invalid or missing amount for payment message");

        if (validationErrors.Any())
        {
            Status = SWIFTStatus.ValidationFailed;
            ErrorDescription = string.Join("; ", validationErrors);
            AddDomainEvent(new SWIFTMessageValidationFailedDomainEvent(Id, SWIFTReference, ErrorDescription));
        }
        else
        {
            Status = SWIFTStatus.Validated;
            AddDomainEvent(new SWIFTMessageValidatedDomainEvent(Id, SWIFTReference));
        }
    }

    private void ValidateMandatoryFields(List<string> validationErrors)
    {
        var mandatoryFields = GetMandatoryFieldsForMessageType();
        
        foreach (var mandatoryField in mandatoryFields)
        {
            if (!_fields.Any(f => f.FieldTag == mandatoryField))
            {
                validationErrors.Add($"Missing mandatory field: {mandatoryField}");
            }
        }
    }

    private string[] GetMandatoryFieldsForMessageType()
    {
        return MessageType switch
        {
            SWIFTMessageType.MT103 => new[] { "20", "23B", "32A", "50K", "59" },
            SWIFTMessageType.MT202 => new[] { "20", "21", "32A", "52A", "58A" },
            SWIFTMessageType.MT700 => new[] { "20", "31C", "40A", "50", "59" },
            _ => new string[0]
        };
    }

    private bool IsValidBIC(string bic)
    {
        // Basic BIC validation (8 or 11 characters)
        return !string.IsNullOrEmpty(bic) && (bic.Length == 8 || bic.Length == 11);
    }

    private bool IsPaymentMessage()
    {
        return MessageType == SWIFTMessageType.MT103 || 
               MessageType == SWIFTMessageType.MT202;
    }

    public void SendMessage()
    {
        if (Status != SWIFTStatus.Validated)
            throw new InvalidOperationException("Message must be validated before sending");

        Status = SWIFTStatus.Sent;
        SentAt = DateTime.UtcNow;

        AddDomainEvent(new SWIFTMessageSentDomainEvent(Id, SWIFTReference, SentAt.Value));
    }

    public void ReceiveAcknowledgment(string acknowledgmentReference)
    {
        if (Status != SWIFTStatus.Sent)
            throw new InvalidOperationException("Message must be sent before receiving acknowledgment");

        Status = SWIFTStatus.Acknowledged;
        AcknowledgmentReference = acknowledgmentReference;
        ReceivedAt = DateTime.UtcNow;

        AddDomainEvent(new SWIFTMessageAcknowledgedDomainEvent(Id, SWIFTReference, acknowledgmentReference));
    }

    public void ProcessMessage(string processedBy)
    {
        if (Status != SWIFTStatus.Acknowledged && Status != SWIFTStatus.Received)
            throw new InvalidOperationException("Message must be acknowledged or received before processing");

        Status = SWIFTStatus.Processed;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new SWIFTMessageProcessedDomainEvent(Id, SWIFTReference, processedBy));
    }

    public void RejectMessage(string errorCode, string errorDescription)
    {
        Status = SWIFTStatus.Rejected;
        ErrorCode = errorCode;
        ErrorDescription = errorDescription;
        ProcessedAt = DateTime.UtcNow;

        AddDomainEvent(new SWIFTMessageRejectedDomainEvent(Id, SWIFTReference, errorCode, errorDescription));
    }

    public void ScheduleRetry(DateTime nextRetryAt)
    {
        if (Status != SWIFTStatus.ValidationFailed && Status != SWIFTStatus.Rejected)
            throw new InvalidOperationException("Only failed messages can be scheduled for retry");

        RetryCount++;
        NextRetryAt = nextRetryAt;
        Status = SWIFTStatus.PendingRetry;

        AddDomainEvent(new SWIFTMessageRetryScheduledDomainEvent(Id, SWIFTReference, RetryCount, nextRetryAt));
    }

    public bool CanRetry()
    {
        return RetryCount < 3 && (Status == SWIFTStatus.ValidationFailed || Status == SWIFTStatus.Rejected);
    }

    public string GetFieldValue(string fieldTag)
    {
        return _fields.FirstOrDefault(f => f.FieldTag == fieldTag)?.FieldValue ?? string.Empty;
    }

    public bool HasField(string fieldTag)
    {
        return _fields.Any(f => f.FieldTag == fieldTag);
    }

    public TimeSpan? GetProcessingDuration()
    {
        if (ProcessedAt.HasValue)
            return ProcessedAt.Value - CreatedAt;
        
        return null;
    }
}

/// <summary>
/// Individual field within a SWIFT message
/// </summary>
public class SWIFTMessageField
{
    public Guid Id { get; private set; }
    public Guid MessageId { get; private set; }
    public string FieldTag { get; private set; }
    public string FieldValue { get; private set; }
    public bool IsMandatory { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SWIFTMessageField() { } // EF Core

    public SWIFTMessageField(
        Guid id,
        Guid messageId,
        string fieldTag,
        string fieldValue,
        bool isMandatory,
        DateTime createdAt)
    {
        Id = id;
        MessageId = messageId;
        FieldTag = fieldTag;
        FieldValue = fieldValue;
        IsMandatory = isMandatory;
        CreatedAt = createdAt;
    }
}

// Enums
public enum SWIFTMessageType
{
    MT103 = 103, // Single Customer Credit Transfer
    MT202 = 202, // General Financial Institution Transfer
    MT700 = 700, // Issue of a Documentary Credit
    MT701 = 701, // Issue of a Documentary Credit
    MT710 = 710, // Advice of a Third Bank's Documentary Credit
    MT720 = 720, // Transfer of a Documentary Credit
    MT730 = 730, // Acknowledgement of a Documentary Credit
    MT740 = 740, // Authorization to Reimburse
    MT750 = 750, // Advice of Discrepancy
    MT760 = 760, // Guarantee
    MT900 = 900, // Confirmation of Debit
    MT910 = 910, // Confirmation of Credit
    MT950 = 950  // Statement Message
}

public enum SWIFTDirection
{
    Outgoing = 1,
    Incoming = 2
}

public enum SWIFTStatus
{
    Draft = 1,
    Ready = 2,
    Validated = 3,
    ValidationFailed = 4,
    Sent = 5,
    Acknowledged = 6,
    Received = 7,
    Processed = 8,
    Rejected = 9,
    PendingRetry = 10
}

// Domain Events
public record SWIFTMessageCreatedDomainEvent(
    Guid MessageId,
    string MessageReference,
    SWIFTMessageType MessageType,
    SWIFTDirection Direction) : IDomainEvent;

public record SWIFTMessageBuiltDomainEvent(
    Guid MessageId,
    string SWIFTReference,
    SWIFTMessageType MessageType) : IDomainEvent;

public record SWIFTMessageValidatedDomainEvent(
    Guid MessageId,
    string SWIFTReference) : IDomainEvent;

public record SWIFTMessageValidationFailedDomainEvent(
    Guid MessageId,
    string SWIFTReference,
    string ValidationErrors) : IDomainEvent;

public record SWIFTMessageSentDomainEvent(
    Guid MessageId,
    string SWIFTReference,
    DateTime SentAt) : IDomainEvent;

public record SWIFTMessageAcknowledgedDomainEvent(
    Guid MessageId,
    string SWIFTReference,
    string AcknowledgmentReference) : IDomainEvent;

public record SWIFTMessageProcessedDomainEvent(
    Guid MessageId,
    string SWIFTReference,
    string ProcessedBy) : IDomainEvent;

public record SWIFTMessageRejectedDomainEvent(
    Guid MessageId,
    string SWIFTReference,
    string ErrorCode,
    string ErrorDescription) : IDomainEvent;

public record SWIFTMessageRetryScheduledDomainEvent(
    Guid MessageId,
    string SWIFTReference,
    int RetryCount,
    DateTime NextRetryAt) : IDomainEvent;