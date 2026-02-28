using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Interest Rate Table Aggregate - Manages interest rate structures
/// Enables tiered and variable interest rate configuration
/// </summary>
public class InterestRateTable : AggregateRoot
{
    public string TableName { get; private set; }
    public string TableCode { get; private set; }
    public Guid ProductId { get; private set; }
    public string RateType { get; private set; } // Fixed, Floating, Tiered
    public decimal BaseRate { get; private set; }
    public decimal? Spread { get; private set; }
    public string Status { get; private set; } // Active, Inactive, Archived
    
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    
    // Rate Tiers (stored as JSON or separate table)
    public string RateTiers { get; private set; } // JSON: [{min_balance, max_balance, rate}]
    public Dictionary<string, object> Metadata { get; private set; }

    private InterestRateTable() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static InterestRateTable Create(
        string tableName,
        string tableCode,
        Guid productId,
        string rateType,
        decimal baseRate,
        string createdBy)
    {
        var table = new InterestRateTable
        {
            Id = Guid.NewGuid(),
            TableName = tableName,
            TableCode = tableCode,
            ProductId = productId,
            RateType = rateType,
            BaseRate = baseRate,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            EffectiveDate = DateTime.UtcNow,
            RateTiers = "[]",
            Metadata = new Dictionary<string, object>()
        };

        return table;
    }

    public void UpdateBaseRate(decimal newRate, string updatedBy)
    {
        BaseRate = newRate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetEffectiveDate(DateTime effectiveDate, string updatedBy)
    {
        EffectiveDate = effectiveDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate(string deactivatedBy)
    {
        Status = "Inactive";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;
    }
}
