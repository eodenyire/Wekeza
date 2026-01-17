using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// SystemParameter Aggregate - Centralized system and business parameter management
/// Supports enterprise-grade configuration management, validation, and change control
/// </summary>
public class SystemParameter : AggregateRoot<Guid>
{
    // Core Properties
    public string ParameterCode { get; private set; }
    public string ParameterName { get; private set; }
    public string Description { get; private set; }
    public ParameterType Type { get; private set; }
    public ParameterCategory Category { get; private set; }
    
    // Value & Configuration
    public string Value { get; private set; }
    public string DefaultValue { get; private set; }
    public ParameterDataType DataType { get; private set; }
    public List<string> AllowedValues { get; private set; }
    public string? ValidationRule { get; private set; }
    
    // Constraints & Validation
    public bool IsRequired { get; private set; }
    public bool IsEncrypted { get; private set; }
    public string? MinValue { get; private set; }
    public string? MaxValue { get; private set; }
    public int? MaxLength { get; private set; }
    public string? RegexPattern { get; private set; }
    
    // Access & Security
    public SecurityLevel SecurityLevel { get; private set; }
    public List<string> AllowedRoles { get; private set; }
    public bool RequiresApproval { get; private set; }
    public string? ApprovalWorkflow { get; private set; }
    
    // Change Management
    public DateTime? LastChangedAt { get; private set; }
    public string? LastChangedBy { get; private set; }
    public string? PreviousValue { get; private set; }
    public List<ParameterChange> ChangeHistory { get; private set; }
    
    // Environment & Deployment
    public string Environment { get; private set; }
    public DateTime? EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }
    
    // Audit & Tracking
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private SystemParameter()
    {
        AllowedValues = new List<string>();
        AllowedRoles = new List<string>();
        ChangeHistory = new List<ParameterChange>();
        Metadata = new Dictionary<string, object>();
    }

    public SystemParameter(
        string parameterCode,
        string parameterName,
        string description,
        ParameterType type,
        ParameterCategory category,
        string defaultValue,
        ParameterDataType dataType,
        string createdBy,
        string environment = "Production") : this()
    {
        if (string.IsNullOrWhiteSpace(parameterCode))
            throw new ArgumentException("Parameter code cannot be empty", nameof(parameterCode));
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new ArgumentException("Parameter name cannot be empty", nameof(parameterName));

        Id = Guid.NewGuid();
        ParameterCode = parameterCode;
        ParameterName = parameterName;
        Description = description;
        Type = type;
        Category = category;
        DefaultValue = defaultValue;
        Value = defaultValue;
        DataType = dataType;
        Environment = environment;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsActive = true;
        SecurityLevel = SecurityLevel.Internal;
        IsRequired = false;
        IsEncrypted = false;

        ValidateValue(Value);

        AddDomainEvent(new SystemParameterCreatedDomainEvent(Id, ParameterCode, ParameterName, Type, CreatedBy));
    }

    // Business Methods
    public void UpdateValue(string newValue, string changedBy, string? changeReason = null)
    {
        if (Value == newValue)
            return;

        ValidateValue(newValue);
        ValidateChangePermissions(changedBy);

        var oldValue = Value;
        PreviousValue = oldValue;
        Value = newValue;
        LastChangedAt = DateTime.UtcNow;
        LastChangedBy = changedBy;

        // Add to change history
        var change = new ParameterChange(oldValue, newValue, changedBy, changeReason);
        ChangeHistory.Add(change);

        // Keep only last 50 changes
        if (ChangeHistory.Count > 50)
        {
            ChangeHistory.RemoveAt(0);
        }

        AddDomainEvent(new SystemParameterValueUpdatedDomainEvent(Id, ParameterCode, oldValue, newValue, changedBy));
    }

    public void ValidateValue(string value)
    {
        if (IsRequired && string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Parameter {ParameterCode} is required and cannot be empty");

        // Data type validation
        switch (DataType)
        {
            case ParameterDataType.Integer:
                if (!int.TryParse(value, out var intValue))
                    throw new ArgumentException($"Parameter {ParameterCode} must be a valid integer");
                ValidateNumericRange(intValue);
                break;

            case ParameterDataType.Decimal:
                if (!decimal.TryParse(value, out var decimalValue))
                    throw new ArgumentException($"Parameter {ParameterCode} must be a valid decimal");
                ValidateNumericRange((double)decimalValue);
                break;

            case ParameterDataType.Boolean:
                if (!bool.TryParse(value, out _))
                    throw new ArgumentException($"Parameter {ParameterCode} must be a valid boolean (true/false)");
                break;

            case ParameterDataType.DateTime:
                if (!DateTime.TryParse(value, out _))
                    throw new ArgumentException($"Parameter {ParameterCode} must be a valid date/time");
                break;

            case ParameterDataType.Json:
                try
                {
                    JsonDocument.Parse(value);
                }
                catch (JsonException)
                {
                    throw new ArgumentException($"Parameter {ParameterCode} must be valid JSON");
                }
                break;

            case ParameterDataType.String:
            default:
                // String validation
                if (MaxLength.HasValue && value.Length > MaxLength.Value)
                    throw new ArgumentException($"Parameter {ParameterCode} exceeds maximum length of {MaxLength}");
                break;
        }

        // Allowed values validation
        if (AllowedValues.Any() && !AllowedValues.Contains(value))
            throw new ArgumentException($"Parameter {ParameterCode} must be one of: {string.Join(", ", AllowedValues)}");

        // Regex pattern validation
        if (!string.IsNullOrWhiteSpace(RegexPattern))
        {
            if (!Regex.IsMatch(value, RegexPattern))
                throw new ArgumentException($"Parameter {ParameterCode} does not match required pattern");
        }

        // Custom validation rule
        if (!string.IsNullOrWhiteSpace(ValidationRule))
        {
            ValidateCustomRule(value);
        }
    }

    public void AddToChangeHistory(string oldValue, string newValue, string changedBy, string? reason = null)
    {
        var change = new ParameterChange(oldValue, newValue, changedBy, reason);
        ChangeHistory.Add(change);

        // Keep only last 50 changes
        if (ChangeHistory.Count > 50)
        {
            ChangeHistory.RemoveAt(0);
        }
    }

    public void Activate(DateTime? effectiveFrom = null, string? activatedBy = null)
    {
        if (IsActive)
            return;

        IsActive = true;
        EffectiveFrom = effectiveFrom ?? DateTime.UtcNow;
        
        if (!string.IsNullOrWhiteSpace(activatedBy))
        {
            LastChangedAt = DateTime.UtcNow;
            LastChangedBy = activatedBy;
        }

        AddDomainEvent(new SystemParameterActivatedDomainEvent(Id, ParameterCode, EffectiveFrom.Value, activatedBy));
    }

    public void Deactivate(DateTime? effectiveTo = null, string? deactivatedBy = null)
    {
        if (!IsActive)
            return;

        IsActive = false;
        EffectiveTo = effectiveTo ?? DateTime.UtcNow;
        
        if (!string.IsNullOrWhiteSpace(deactivatedBy))
        {
            LastChangedAt = DateTime.UtcNow;
            LastChangedBy = deactivatedBy;
        }

        AddDomainEvent(new SystemParameterDeactivatedDomainEvent(Id, ParameterCode, EffectiveTo.Value, deactivatedBy));
    }

    public void SetSecurityLevel(SecurityLevel level, string updatedBy)
    {
        var oldLevel = SecurityLevel;
        SecurityLevel = level;
        LastChangedAt = DateTime.UtcNow;
        LastChangedBy = updatedBy;

        AddDomainEvent(new SystemParameterSecurityLevelUpdatedDomainEvent(Id, ParameterCode, oldLevel, level, updatedBy));
    }

    public void AddAllowedRole(string role, string updatedBy)
    {
        if (!AllowedRoles.Contains(role))
        {
            AllowedRoles.Add(role);
            LastChangedAt = DateTime.UtcNow;
            LastChangedBy = updatedBy;

            AddDomainEvent(new SystemParameterAllowedRoleAddedDomainEvent(Id, ParameterCode, role, updatedBy));
        }
    }

    public void RemoveAllowedRole(string role, string updatedBy)
    {
        if (AllowedRoles.Remove(role))
        {
            LastChangedAt = DateTime.UtcNow;
            LastChangedBy = updatedBy;

            AddDomainEvent(new SystemParameterAllowedRoleRemovedDomainEvent(Id, ParameterCode, role, updatedBy));
        }
    }

    public void SetValidationConstraints(
        List<string>? allowedValues = null,
        string? minValue = null,
        string? maxValue = null,
        int? maxLength = null,
        string? regexPattern = null,
        string? validationRule = null,
        string? updatedBy = null)
    {
        if (allowedValues != null)
            AllowedValues = allowedValues;
        
        MinValue = minValue;
        MaxValue = maxValue;
        MaxLength = maxLength;
        RegexPattern = regexPattern;
        ValidationRule = validationRule;

        if (!string.IsNullOrWhiteSpace(updatedBy))
        {
            LastChangedAt = DateTime.UtcNow;
            LastChangedBy = updatedBy;
        }

        // Re-validate current value with new constraints
        ValidateValue(Value);

        AddDomainEvent(new SystemParameterValidationUpdatedDomainEvent(Id, ParameterCode, updatedBy));
    }

    public void SetApprovalWorkflow(string workflow, string updatedBy)
    {
        ApprovalWorkflow = workflow;
        RequiresApproval = !string.IsNullOrWhiteSpace(workflow);
        LastChangedAt = DateTime.UtcNow;
        LastChangedBy = updatedBy;

        AddDomainEvent(new SystemParameterApprovalWorkflowUpdatedDomainEvent(Id, ParameterCode, workflow, updatedBy));
    }

    public void EnableEncryption(string updatedBy)
    {
        if (IsEncrypted)
            return;

        IsEncrypted = true;
        LastChangedAt = DateTime.UtcNow;
        LastChangedBy = updatedBy;

        AddDomainEvent(new SystemParameterEncryptionEnabledDomainEvent(Id, ParameterCode, updatedBy));
    }

    public void DisableEncryption(string updatedBy)
    {
        if (!IsEncrypted)
            return;

        IsEncrypted = false;
        LastChangedAt = DateTime.UtcNow;
        LastChangedBy = updatedBy;

        AddDomainEvent(new SystemParameterEncryptionDisabledDomainEvent(Id, ParameterCode, updatedBy));
    }

    // Query Methods
    public bool IsEffective(DateTime? checkDate = null)
    {
        var date = checkDate ?? DateTime.UtcNow;
        
        if (!IsActive)
            return false;

        if (EffectiveFrom.HasValue && date < EffectiveFrom.Value)
            return false;

        if (EffectiveTo.HasValue && date > EffectiveTo.Value)
            return false;

        return true;
    }

    public bool CanBeChangedBy(string userId, List<string> userRoles)
    {
        if (!AllowedRoles.Any())
            return true; // No role restrictions

        return AllowedRoles.Any(role => userRoles.Contains(role));
    }

    public T GetTypedValue<T>()
    {
        try
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Cannot convert parameter {ParameterCode} value '{Value}' to type {typeof(T).Name}", ex);
        }
    }

    public object GetTypedValue()
    {
        return DataType switch
        {
            ParameterDataType.Integer => int.Parse(Value),
            ParameterDataType.Decimal => decimal.Parse(Value),
            ParameterDataType.Boolean => bool.Parse(Value),
            ParameterDataType.DateTime => DateTime.Parse(Value),
            ParameterDataType.Json => JsonDocument.Parse(Value),
            _ => Value
        };
    }

    public ParameterChange? GetLastChange()
    {
        return ChangeHistory.LastOrDefault();
    }

    public List<ParameterChange> GetRecentChanges(int count = 10)
    {
        return ChangeHistory.TakeLast(count).ToList();
    }

    // Private Methods
    private void ValidateNumericRange(double value)
    {
        if (!string.IsNullOrWhiteSpace(MinValue) && double.TryParse(MinValue, out var min) && value < min)
            throw new ArgumentException($"Parameter {ParameterCode} must be >= {MinValue}");

        if (!string.IsNullOrWhiteSpace(MaxValue) && double.TryParse(MaxValue, out var max) && value > max)
            throw new ArgumentException($"Parameter {ParameterCode} must be <= {MaxValue}");
    }

    private void ValidateCustomRule(string value)
    {
        // This could be extended to support custom validation expressions
        // For now, we'll just log that custom validation was attempted
        Metadata["LastCustomValidation"] = DateTime.UtcNow;
    }

    private void ValidateChangePermissions(string changedBy)
    {
        // This would typically check against user roles and permissions
        // For now, we'll just record the change attempt
        Metadata["LastChangeAttemptBy"] = changedBy;
        Metadata["LastChangeAttemptAt"] = DateTime.UtcNow;
    }
}

// Supporting Classes
public class ParameterChange
{
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public string? Reason { get; set; }
    public Dictionary<string, object> Metadata { get; set; }

    public ParameterChange(string oldValue, string newValue, string changedBy, string? reason = null)
    {
        OldValue = oldValue;
        NewValue = newValue;
        ChangedAt = DateTime.UtcNow;
        ChangedBy = changedBy;
        Reason = reason;
        Metadata = new Dictionary<string, object>();
    }
}

// Enumerations
public enum ParameterType
{
    System,
    Business,
    Security,
    Integration,
    Performance,
    Compliance
}

public enum ParameterCategory
{
    General,
    Authentication,
    Authorization,
    Limits,
    Rates,
    Fees,
    Notifications,
    Integration,
    Reporting,
    Compliance
}

public enum ParameterDataType
{
    String,
    Integer,
    Decimal,
    Boolean,
    DateTime,
    Json
}

public enum SecurityLevel
{
    Public,
    Internal,
    Confidential,
    Restricted,
    Secret
}