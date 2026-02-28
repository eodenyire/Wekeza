using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Limit Definition Aggregate - Defines credit and operational limits
/// Core to risk management and compliance
/// </summary>
public class LimitDefinition : AggregateRoot
{
    public string LimitName { get; private set; }
    public string LimitCode { get; private set; }
    public Guid? CustomerId { get; private set; } // Null for system-level limits
    public Guid? ProductId { get; private set; }
    public string LimitType { get; private set; } // Credit, Debit, Transaction, Daily, Monthly, etc.
    public string Hierarchy { get; private set; } // Customer, Product, Branch, System
    public decimal LimitAmount { get; private set; }
    public string Currency { get; private set; }
    public string Status { get; private set; } // Active, Suspended, Revoked, Archived
    
    // Temporal
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    
    // Usage Tracking
    public decimal CurrentUtilization { get; private set; }
    public DateTime LastReservedAt { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private LimitDefinition() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static LimitDefinition Create(
        string limitName,
        string limitCode,
        string limitType,
        string hierarchy,
        decimal limitAmount,
        string currency,
        string createdBy)
    {
        var limit = new LimitDefinition
        {
            Id = Guid.NewGuid(),
            LimitName = limitName,
            LimitCode = limitCode,
            LimitType = limitType,
            Hierarchy = hierarchy,
            LimitAmount = limitAmount,
            Currency = currency,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            EffectiveDate = DateTime.UtcNow,
            CurrentUtilization = 0,
            LastReservedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>()
        };

        return limit;
    }

    public void UpdateAmount(decimal newAmount, string updatedBy)
    {
        LimitAmount = newAmount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateUtilization(decimal utilization)
    {
        CurrentUtilization = utilization;
        LastReservedAt = DateTime.UtcNow;
    }

    public void Revoke(string reason, string revokedBy)
    {
        Status = "Revoked";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = revokedBy;
    }

    public bool IsBreached => CurrentUtilization >= LimitAmount;
}
